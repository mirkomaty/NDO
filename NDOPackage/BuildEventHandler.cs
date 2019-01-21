//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using VSLangProj;
using System.Collections.Generic;
using NDOEnhancer;
using System.Reflection;

namespace NETDataObjects.NDOVSPackage
{
	/// <summary>
	/// Summary description for BuildEventHandler.
	/// </summary>
	internal class BuildEventHandler
	{
		private MessageAdapter			messages = null;
		private BuildEvents				buildEvents;
		private _DTE					m_applicationObject;

		public BuildEventHandler( _DTE applicationObject )
		{
			m_applicationObject = applicationObject;
			ApplicationObject.VisualStudioApplication = applicationObject;

            Events events = applicationObject.Events;
            buildEvents = events.BuildEvents;

            buildEvents.OnBuildBegin			+= OnBuildBegin;
            buildEvents.OnBuildDone				+= OnBuildDone;
            buildEvents.OnBuildProjConfigDone	+= OnBuildProjConfigDone;
//            buildEvents.OnBuildProjConfigBegin	+= OnBuildProjConfigBegin;  // If needed, activate this line and the according method
		}

		public void OnBuildBegin( vsBuildScope scope, vsBuildAction action )
		{
            if (messages == null)
			{
				messages = new MessageAdapter(); 
			}
			messages.Success = true;
		}

		public void OnBuildDone( vsBuildScope scope, vsBuildAction action )
		{
			if (!messages.Success)
			{
				messages.WriteLine("    Build failed");
			}
		}

        void IncludeFiles(ConfigurationOptions options, Project project, ProjectDescription projectDescription)
        {
#if DEBUG
            messages.WriteLine("Including project files...");
#endif
            string schemaVersion = string.Empty;
#if DEBUG
            projectDescription.MessageAdapter = messages;
#endif
            string mappingFile = string.Empty;
            try
            {
#if DEBUG
                messages.WriteLine("  mapping...");
#endif
                mappingFile = projectDescription.DefaultMappingFileName;
                projectDescription.AddFileToProject(mappingFile);
            }
            catch (Exception ex)
            {
                messages.WriteLine("Warning: Can't add mapping file '" + mappingFile + "' to the project. " + ex.Message);
            }
            try
            {
                NDOMapping mapping = new NDOMapping(mappingFile);
                schemaVersion = mapping.SchemaVersion;
            }
            catch (Exception ex)
            {
                messages.WriteLine("Warning: Can't extract schema version from the mapping file. Error message: " + ex.Message);
            }
//            if (options.GenerateSQL)
//            {
//                string sqlFileName = string.Empty;
//#if DEBUG
//                messages.WriteLine( "  main sql script..." );
//#endif
//                try
//                {
//                    sqlFileName = Path.ChangeExtension( projectDescription.BinFile, ".ndo.sql" );
//                    projectDescription.AddFileToProject( sqlFileName );
//                }
//                catch (Exception ex)
//                {
//                    messages.WriteLine( "Warning: Can't add schema file '" + sqlFileName + "' to the project. " + ex.Message );
//                }

//#if DEBUG
//                messages.WriteLine( "  diff sql script..." );
//#endif
//                try
//                {
//                    sqlFileName = Path.ChangeExtension( projectDescription.BinFile, ".ndodiff." + schemaVersion + ".sql" );
//                    projectDescription.AddFileToProject( sqlFileName );
//                }
//                catch (Exception ex)
//                {
//                    messages.WriteLine( "Warning: Can't add schema diff file '" + sqlFileName + "' to the project. " + ex.Message );
//                }
//            }
#if DEBUG
            messages.WriteLine("...ready");
#endif
        }


		void CheckProjectDescription(ConfigurationOptions options, ProjectDescription pd, string fileName)
		{
			Dictionary<string, NDOReference> ht = pd.References;  // Build the references
			ProjectDescription storedPd = new ProjectDescription(fileName);
//			messages.WriteLine(pd.BinFile + ", " + storedPd.BinFile);
//			messages.WriteLine(pd.ProjPath + ", " + storedPd.ProjPath);
//			messages.WriteLine(pd.ObjPath + ", " + storedPd.ObjPath);
			bool storeIt = (string.Compare(storedPd.BinFile, pd.BinFile, true) != 0
				|| string.Compare(storedPd.ObjPath, pd.ObjPath, true) != 0
                || string.Compare(storedPd.KeyFile, pd.KeyFile, true) != 0);
			
			storeIt = storeIt || (pd.References.Count != storedPd.References.Count);
//			messages.WriteLine(pd.References.Count.ToString() + ", " + storedPd.References.Count);

			foreach ( string key in pd.References.Keys )
			{
				//					messages.WriteLine("Key " + de.Key + " contained: " + storedPd.References.Contains(de.Key));
				if ( !storedPd.References.ContainsKey( key ) )
				{
					storeIt = true;
					break;
				}
				NDOReference r1 = pd.References[key];
				NDOReference r2 = storedPd.References[key];

				// If we save the collected data, we should use the previously stored CheckThisDLL settings
				r1.CheckThisDLL = r2.CheckThisDLL;

				//					messages.WriteLine("  " + s1 + ", " + s2);
				if ( string.Compare( Path.GetFullPath(r1.Path), Path.GetFullPath(r2.Path), true ) != 0)
				{
					storeIt = true;
				}
			}
			if (storeIt)
			{
				FileInfo fi = new FileInfo(fileName);
				if ((fi.Attributes & FileAttributes.ReadOnly) != 0)
				{
					MessageBox.Show("The NDO project file '" + fileName + "' is write protected, probably due to your Source Code Control system. NDO needs to update this file now. NDO tries to remove the write protect attribute in order to update the file.", "NDO Add-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					fi.Attributes &= (~FileAttributes.ReadOnly);
				}
				options.Save(pd);
			}
		}

		void PostProcess( ProjectDescription pd )
		{
		}

		public void OnBuildProjConfigDone( string projectName, string projectConfig, string platform, string solutionConfig, bool success )
		{
			if (messages == null)
				messages = new MessageAdapter();

			if ( ! success )
			{
				//messages.DumpTasks();
				return;
			}

            try
            {
				Solution solution = m_applicationObject.Solution;
                
                // projectName can be like 'path\path\abc.def, where abc.def is the project name in the 
                // solution explorer
                Project  project = null;
                try
                {
                    project = new ProjectIterator(solution)[projectName];
                }
                catch (Exception ex) 
				{
					messages.WriteLine(ex.ToString()); 
				} // project remains null, exception will be thrown later
                if (project == null)
                {
                    messages.WriteLine("NDO: Project " + projectName + " skipped.");
                    return;
                }
				ConfigurationOptions options = new ConfigurationOptions(project);
                if (!options.EnableAddIn)
				{
					messages.WriteLine("NDO Add-in disabled");
					return;
				}

				messages.WriteLine(String.Format(EnhDate.String, "NDO Extension", new AssemblyName(GetType().Assembly.FullName).Version.ToString()));

				ProjectDescription projectDescription = new ProjectDescription(solution, project);
                string projFileName = options.FileName;
                CheckProjectDescription(options, projectDescription, projFileName);

				string targetFramework = projectDescription.TargetFramework;
				// .NETCoreApp,Version=v2.0 müsste beim Überprüfen ebenfalls gültig sein.
				//if (!string.IsNullOrEmpty( targetFramework ) && (!targetFramework.StartsWith( ".NETFramework,Version=v4" ) && !targetFramework.StartsWith( ".NETStandard,Version=v2" )))
				//{
				//	messages.ShowError( "Project " + project.Name + " has been built with " + targetFramework + ". NDO requires .NETFramework 4.x or .NET Standard. You need to reconfigure your project." );
				//	messages.WriteInsertedLine( targetFramework );
				//	project.DTE.ExecuteCommand( "Build.Cancel", "" );
				//	messages.ActivateErrorList();
				//	return;
				//}

				// ------------------ MsBuild Support -----------------------
#if EnhancerLaunch  // Masked out. Shouldn't be used in future.
				if (!options.UseMsBuild)
				{

					// The platform target is the more correct platform description.
					// If it is available, let's use it.
					string platform2 = projectDescription.PlatformTarget;
					if (!string.IsNullOrEmpty( platform2 ))
						platform = platform2;

					string appName = "NDOEnhancer.exe";
					if (platform == "x86" && OperatingSystem.Is64Bit)
					{
						appName = "Enhancerx86Stub.exe";
						messages.WriteLine( "NDO-Addin: Using x86 Stub" );
					}
					ConsoleProcess cp = new ConsoleProcess( false );
					int result = cp.Execute( "\"" + Path.Combine( ApplicationObject.AssemblyPath, appName ) + "\"",
						"\"" + projFileName + "\"" );

					if (cp.Stdout != String.Empty)
						messages.WriteLine( cp.Stdout );

					string stderr = cp.Stderr;
					if (stderr != string.Empty)
					{
						Regex regex = new Regex( @"Error:" );
						MatchCollection mc = regex.Matches( stderr );
						int lastmatch = mc.Count - 1;
						for (int i = 0; i < mc.Count; i++)
						{
							int endindex;
							if (i == lastmatch)
								endindex = stderr.Length;
							else
								endindex = mc[i + 1].Index;
							int startindex = mc[i].Index;
							//						messages.WriteLine("[" + i + "]:" + startindex + ',' + endindex);
							// The substring always ends with a '\n', which should be removed.
							string outString = stderr.Substring( startindex, endindex - startindex );
							if (outString.EndsWith( "\r\n" ))
								outString = outString.Substring( 0, outString.Length - 2 );
							//						messages.ShowError("|" + outString + "|");
							messages.ShowError( outString );
						}
					}

					//if ( projectDescription.ConfigurationOptions.EnableEnhancer )
					//    PostProcess( projectDescription );

					if (result != 0)
					{
						if (messages.Success)
							messages.ShowError( "An unknown error occured in the NDO Enhancer" );
						// Now messages.Success is false
					}
				}
#endif
				if (messages.Success || options.UseMsBuild)
				{
					IncludeFiles(options, project, projectDescription);
				}
				else
				{
					project.DTE.ExecuteCommand("Build.Cancel", "");
					messages.ActivateErrorList();
				}
            }
            catch ( System.Exception ex )
            {				
                messages.WriteLine( "*** Enhancer Add-in Error: ***" );
                if (!(ex is EnhancerEmptyException))
                {
                    messages.ShowError( ex.ToString());
                }
                if (ex is System.Runtime.InteropServices.COMException)
                {
                    messages.ShowError("An error in the Visual Studio automation system occured. The error should disappear after a restart of Visual Studio.");
                }
                else
                {
                    messages.WriteLine("");
                    messages.WriteLine("This is possibly a follow-up error. Look at error messages above this line.");
                }				
            }
		}

		//public void OnBuildProjConfigBegin( string projectName, string projectConfig, string platform, string solutionConfig )
		//{
		//}

	}
}
