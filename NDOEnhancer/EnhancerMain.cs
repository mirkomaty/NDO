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
using NDOEnhancer;
using System.IO;
using System.Reflection;
using System.Linq;
using NDO.Configuration;
using NDO.Provider;

namespace EnhancerTest
{
	[Serializable]
	public class EnhancerTest
	{
	
        static bool verboseMode;
		ProjectDescription projectDescription;

		void CopyFile(string source, string dest)
        {
            if (verboseMode)
			    Console.WriteLine("Copying: " + source + "->" + dest);

            File.Copy(source, dest, true);
        }

		int DomainLaunch(string arg)
		{
			ProjectDescription pd;
			ConfigurationOptions options;

			if (!File.Exists(arg))
			{
				throw new Exception("Can't find file '" + arg + "'");
			}
			pd = new ProjectDescription(arg);
			options = pd.ConfigurationOptions;

			if (!options.EnableAddIn)
				return 0;

			verboseMode = options.VerboseMode;
			string appDomainDir = Path.GetDirectoryName(pd.BinFile);
			AppDomain cd = AppDomain.CurrentDomain;
			AppDomain ad = AppDomain.CreateDomain("NDOEnhancerDomain", cd.Evidence, appDomainDir, "", false);
			ad.SetData("arg", arg);
			string loadPath = this.GetType().Assembly.Location;
			if (!File.Exists(loadPath))
				throw new Exception("File not found: " + loadPath);
			int result = ad.ExecuteAssembly(loadPath);
			AppDomain.Unload(ad);
			if (options.EnableEnhancer)
			{
                string tempDir = Path.Combine(pd.ObjPath, "ndotemp");
				string enhObjFile = Path.Combine(tempDir, Path.GetFileName(pd.BinFile));
				string enhPdbFile = Path.ChangeExtension(enhObjFile, ".pdb");
				string binPdbFile = Path.ChangeExtension(pd.BinFile, ".pdb");
				string objFile = Path.Combine(pd.ObjPath, Path.GetFileName(pd.BinFile));
                string objPdbFile = Path.Combine(pd.ObjPath, Path.GetFileName(binPdbFile));
                bool objPathDifferent = String.Compare(objFile, pd.BinFile, true) != 0;
                if (File.Exists(enhObjFile))
				{
					CopyFile(enhObjFile, pd.BinFile);

                    if (objPathDifferent)
                        CopyFile(enhObjFile, objFile);
                    File.Delete(enhObjFile);

					if (File.Exists(enhPdbFile))
					{
                        CopyFile(enhPdbFile, binPdbFile);

                        if (objPathDifferent)
                            CopyFile(enhPdbFile, objPdbFile);
                        try
                        {
                            File.Delete(enhPdbFile);
                        }
                        catch (Exception ex)
                        {
                            if (verboseMode)
                                Console.WriteLine("Warning: Ignored Exception: " + ex.ToString());
                        }
					}
				}
			}
			return result;
		}

		public void InternalStart(string arg)
		{
			Console.WriteLine("Runtime: " + typeof(string).Assembly.FullName);			
			ConfigurationOptions options;

			if (!File.Exists(arg))
			{
				throw new Exception("Can't find file '" + arg + "'");
			}

			this.projectDescription = new ProjectDescription( arg );
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
				Console.WriteLine( $"Project Style: {(this.projectDescription.IsSdkStyle ? "Sdk" : "Old MSBuild")}" );
			}
#endif
				Console.WriteLine( EnhDate.String, "NDO Enhancer", new AssemblyName( GetType().Assembly.FullName ).Version.ToString() );

            if (!this.projectDescription.IsWebProject && options.EnableEnhancer)
			    this.projectDescription.References.Add(projectDescription.AssemblyName, new NDOReference(projectDescription.AssemblyName, projectDescription.BinFile, true));

			MessageAdapter messages = new MessageAdapter();

			new NDOEnhancer.Enhancer(this.projectDescription, messages).DoIt();

			if (options.EnableEnhancer)
				Console.WriteLine("Enhancer ready");
		}

		public static int Main(string[] args)
		{
			int result = 0;

            try
            {
				string locationDir = Path.GetDirectoryName(typeof(EnhancerTest).Assembly.Location);
				string appDomainDir = AppDomain.CurrentDomain.BaseDirectory;
				if (appDomainDir.EndsWith("\\"))
					appDomainDir = appDomainDir.Substring(0, appDomainDir.Length - 1);

				if (string.Compare(appDomainDir, locationDir, true) == 0)
				{
					if (args.Length < 1)
						throw new Exception("usage: NDOEnhancer <file_name>\n");
					string arg = args[0];
					arg = Path.GetFullPath(arg);

					Console.WriteLine( "Enhancer executable: " + typeof(EnhancerTest).Assembly.Location );

					result = new EnhancerTest().DomainLaunch(arg);
				}
				else
				{
#if DEBUG
					Console.WriteLine( "Domain base directory is: " + AppDomain.CurrentDomain.BaseDirectory );
					Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
#endif

					string newarg = (string) AppDomain.CurrentDomain.GetData("arg");
					new EnhancerTest().InternalStart(newarg);
				}
            }
            catch(Exception ex)
            {
#if DEBUG
                Console.Error.WriteLine("Error: " + ex.ToString());
#else
                Console.Error.WriteLine("Error: " + ex.Message);
#endif
                result = -1;
            }
			return result;
		}

		string GetPackageLibPath( string assName )
		{


			var packageName = assName.ToLowerInvariant();
			if (assName.Equals( "NDO", StringComparison.OrdinalIgnoreCase ))
				packageName = "ndo.dll";

			var path = NugetProps.DefaultNugetPackageFolder;
			if (path == null)
				return null;

			return Path.Combine( path, ProjectAssets.GetPackageDir( packageName ) );
		}

		Assembly OnAssemblyResolve( object sender, ResolveEventArgs args )
		{
			if (verboseMode)
				Console.WriteLine( $"AssemblyResolve: {args?.Name}" );

			var assName = args.Name.Split(',').FirstOrDefault();
			if (assName == null)
				return null;

			string path = null;
			if (this.projectDescription.IsSdkStyle)
				path = GetPackageLibPath( assName );
			else
				path = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), assName + ".dll" );

			if (path == null)
				return null;

			Console.WriteLine( "Location: " + path );
			if (File.Exists( path ))
			{
				return Assembly.LoadFrom( path );
			}
			return null;
		}


		public EnhancerTest()
		{
		}

	}
}
