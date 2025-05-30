//
// Copyright (c) 2002-2024 Mirko Matytschak 
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
//using NDO.Provider;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NDO.ProviderFactory;
using NDOInterfaces;
//using NDO.SqlPersistenceHandling;

namespace NDOEnhancer
{
	[Serializable]
	public class EnhancerMain
	{
	
        static bool verboseMode;
		ProjectDescription projectDescription;
        private IServiceProvider serviceProvider;

        public static int Main( string[] args )
		{
			int result = 0;
			// Make the culture invariant, otherwise .NET tries to load .resources assemblies.
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			try
			{
				if (args.Length < 2)
					throw new Exception( "usage: NDOEnhancer <file_name> <target_framework>\n" );

				string ndoProjFilePath = args[0];
				string targetFramework = args[1];

#if DEBUG
					Console.WriteLine( "Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory );
					Console.WriteLine( "Running as " + ( IntPtr.Size * 8 ) + " bit app." );
#endif

					new EnhancerMain().InternalStart( ndoProjFilePath, targetFramework );
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine( "Error: " + ex.ToString() );
				result = -1;
            }
            return result;
        }

        void Build( Action<IServiceCollection> configure = null )
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureServices( services =>
            {
                services.AddLogging( b =>
                {
                    b.ClearProviders();
                    b.AddConsole();
                } );

				services.AddSingleton<INDOProviderFactory, NDOProviderFactory>();
				services.AddSingleton<IProviderPathFinder, ProviderPathFinder>();
                if (configure != null)
                    configure( services );
            } );

            var host = builder.Build();
            this.serviceProvider = host.Services;
        }

        void CopyFile(string source, string dest)
        {
            if (verboseMode)
			    Console.WriteLine("Copying: " + source + "->" + dest);

            File.Copy(source, dest, true);
        }


		public void InternalStart(string ndoProjFilePath, string targetFramework)
		{
			Console.WriteLine("Runtime: " + typeof(string).Assembly.FullName);			
			ConfigurationOptions options;

			if (!File.Exists(ndoProjFilePath))
			{
				throw new Exception("Can't find file '" + ndoProjFilePath + "'");
			}

#if DEBUG
            Console.WriteLine( $"Loading Project Description from {ndoProjFilePath}..." );
#endif
            this.projectDescription = new ProjectDescription( ndoProjFilePath, targetFramework );

			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
			//NDOContainer.Instance.RegisterType<IProviderPathFinder,ProviderPathFinder>();
			//// This is needed as parameter for ProviderPathFinder
			//NDOContainer.Instance.RegisterInstance( this.projectDescription );

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
			// We should consider using the following code to determine the version:
			//public static string? GetInformationalVersion() =>
			//	Assembly
			//		.GetEntryAssembly()
			//		?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
			//		?.InformationalVersion;
			// The PatchNdoVersion tool is able to enter the informational version.


			Console.WriteLine( EnhDate.String, "NDO Enhancer", new AssemblyName( GetType().Assembly.FullName ).Version.ToString() );

            if (options.EnableEnhancer)
			    this.projectDescription.References.Add(projectDescription.AssemblyName, new NDOReference(projectDescription.AssemblyName, projectDescription.BinFile, true));

			MessageAdapter messages = new MessageAdapter();

			var basePath = Path.GetDirectoryName( this.projectDescription.BinFile );
			var loadContext = new ManagedLoadContext( basePath, verboseMode );
			using (loadContext.EnterContextualReflection())
			{
				new Enhancer( this.projectDescription, messages, NDOProviderFactory.Instance ).DoIt();
				loadContext.Unload();
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
					if (objPathDifferent)
						CopyFile( enhObjFile, objFile );

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
