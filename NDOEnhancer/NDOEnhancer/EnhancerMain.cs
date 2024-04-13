//
// Copyright (c) 2002-2022 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using System.IO;
using System.Reflection;
using System.Linq;
using NDO.Configuration;
using NDO.Provider;
using System.Globalization;

namespace NDOEnhancer
{
	[Serializable]
	public class EnhancerMain
	{
	
        static bool verboseMode;
		ProjectDescription projectDescription;

		public static int Main( string[] args )
		{
			int result = 0;
			// Make the culture invariant, otherwise .NET tries to load .resources assemblies.
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			try
			{
				if (args.Length < 1)
					throw new Exception( "usage: NDOEnhancer <file_name>\n" );

				string arg = Path.GetFullPath( args[0] );
				string ndoProjFilePath = arg;

#if DEBUG
					Console.WriteLine( "Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory );
					Console.WriteLine( "Running as " + ( IntPtr.Size * 8 ) + " bit app." );
#endif

					new EnhancerMain().InternalStart( ndoProjFilePath );
#if NET48_OR_GREATER
				}
#endif
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine( "Error: " + ex.ToString() );
				result = -1;
			}
			return result;
		}



		void CopyFile(string source, string dest)
        {
            if (verboseMode)
			    Console.WriteLine("Copying: " + source + "->" + dest);

            File.Copy(source, dest, true);
        }


		public void InternalStart(string ndoProjFilePath)
		{
			Console.WriteLine("Runtime: " + typeof(string).Assembly.FullName);			
			ConfigurationOptions options;

			if (!File.Exists(ndoProjFilePath))
			{
				throw new Exception("Can't find file '" + ndoProjFilePath + "'");
			}

			this.projectDescription = new ProjectDescription( ndoProjFilePath );
			AppDomain.CurrentDomain.SetData( "ProjectDescription", this.projectDescription );

			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
			NDOContainer.Instance.RegisterType<IProviderPathFinder,ProviderPathFinder>();

#if DEBUG
			Console.WriteLine("Loading Project Description...");
#endif
            options = projectDescription.ConfigurationOptions;

			if (!options.EnableAddIn)
				return;

#if DEBUG
            Console.WriteLine("Loading Project Description ready.");

			verboseMode = true;
#else
            verboseMode = options.VerboseMode;

			// In Debug Mode the base directory is printed in the Main method
			if (verboseMode)
			{
				Console.WriteLine( "Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory );
			}
#endif
				Console.WriteLine( EnhDate.String, "NDO Enhancer", new AssemblyName( GetType().Assembly.FullName ).Version.ToString() );

            if (!this.projectDescription.IsWebProject && options.EnableEnhancer)
			    this.projectDescription.References.Add(projectDescription.AssemblyName, new NDOReference(projectDescription.AssemblyName, projectDescription.BinFile, true));

			MessageAdapter messages = new MessageAdapter();

			var basePath = Path.Combine(Path.GetDirectoryName(ndoProjFilePath), this.projectDescription.ObjPath);
			using (new ManagedLoadContext( basePath ).EnterContextualReflection())
			{
				new NDOEnhancer.Enhancer( this.projectDescription, messages ).DoIt();
			}

			if (options.EnableEnhancer)
			{
				string tempDir = Path.Combine(this.projectDescription.ObjPath, "ndotemp");
				string enhObjFile = Path.Combine(tempDir, Path.GetFileName(this.projectDescription.BinFile));
				string enhPdbFile = Path.ChangeExtension(enhObjFile, ".pdb");
				string binPdbFile = Path.ChangeExtension(this.projectDescription.BinFile, ".pdb");
				string objFile = Path.Combine(this.projectDescription.ObjPath, Path.GetFileName(this.projectDescription.BinFile));
				string objPdbFile = Path.Combine(this.projectDescription.ObjPath, Path.GetFileName(binPdbFile));
				bool objPathDifferent = String.Compare(objFile, this.projectDescription.BinFile, true) != 0;
				if (File.Exists( enhObjFile ))
				{
					CopyFile( enhObjFile, this.projectDescription.BinFile );
#warning We need to shadow copy the obj file
					// Das ist die Datei, die untersucht wird.
					//if (objPathDifferent)
					//	CopyFile( enhObjFile, objFile );
					//File.Delete( enhObjFile );

					if (File.Exists( enhPdbFile ))
					{
						CopyFile( enhPdbFile, binPdbFile );

						if (objPathDifferent)
							CopyFile( enhPdbFile, objPdbFile );
						try
						{
							File.Delete( enhPdbFile );
						}
						catch (Exception ex)
						{
							if (verboseMode)
								Console.WriteLine( "Warning: Ignored Exception: " + ex.ToString() );
						}
					}
				}

				Console.WriteLine( "Enhancer ready" );
			}
		}

		string GetPackageLibPath( string assyName )
		{
			if (this.projectDescription == null)
				throw new Exception( "Project Description is not defined" );

			var packageName = assyName.ToLowerInvariant();
			if (assyName.Equals( "NDO", StringComparison.OrdinalIgnoreCase ))
				packageName = "ndo.dll";

			var path = new NugetProps(this.projectDescription).DefaultNugetPackageFolder;
			if (path == null)
				return null;

			return Path.Combine( path, new ProjectAssets(this.projectDescription).GetPackageDir( packageName ) );
		}

		/// <summary>
		/// This is called, if an assembly can't be found in the bin directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		Assembly OnAssemblyResolve( object sender, ResolveEventArgs args )
		{
			if (verboseMode)
				Console.WriteLine( $"AssemblyResolve: {args?.Name}" );

			var assyName = args?.Name?.Split(',').FirstOrDefault();
			if (assyName == null)
				return null;

			string path = GetPackageLibPath( assyName );

			if (path == null)
				return null;

			Console.WriteLine( "Location: " + path );
			if (File.Exists( path ))
			{
				return Assembly.LoadFrom( path );
			}

			return null;
		}


		public EnhancerMain()
		{
		}
	}
}
