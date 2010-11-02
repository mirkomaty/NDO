//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
using VSLangProj;
using VSLangProj2;
using System.Collections.Generic;

namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for BuildEventHandler.
	/// </summary>
	internal class BuildEventHandler
	{
		MessageAdapter messages = null;
//		internal static bool expired;

		public
		BuildEventHandler( _DTE applicationObject )
		{
			m_applicationObject = applicationObject;
			ApplicationObject.VisualStudioApplication = applicationObject;
		}

		public void
		onBuildBegin( vsBuildScope scope, vsBuildAction action )
		{
			messages.Success = true;
#if TRIAL || BETA
//			expired = !NDOKey.CheckDate(new LicenceKey().TheKey, DateTime.Now);
#endif
            if (messages == null)
			{
				messages = new MessageAdapter(); 
			}

            //if (expired)
            //{
            //    messages.ShowError("NDO version expired");
            //}
            //messages.Success = !expired;			
		}

		public void
		onBuildDone( vsBuildScope scope, vsBuildAction action )
		{
			if (!messages.Success)
			{
				messages.WriteLine("    Build failed");
			}
		}



		public void
		onBuildProjConfigBegin( string projectName, string projectConfig, string platform, string solutionConfig )
		{
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
                NDO.Mapping.NDOMapping mapping = new NDO.Mapping.NDOMapping(mappingFile);
                schemaVersion = mapping.SchemaVersion;
            }
            catch (Exception ex)
            {
                messages.WriteLine("Warning: Can't extract schema version from the mapping file. Error message: " + ex.Message);
            }
            if (options.GenerateSQL)
            {
                string sqlFileName = string.Empty;
#if DEBUG
                messages.WriteLine("  main sql script...");
#endif
                try
                {
                    sqlFileName = Path.ChangeExtension(projectDescription.BinFile, ".ndo.sql");
                    projectDescription.AddFileToProject(sqlFileName);
                }
                catch (Exception ex)
                {
                    messages.WriteLine("Warning: Can't add schema file '" + sqlFileName + "' to the project. " + ex.Message);
                }

#if DEBUG
                messages.WriteLine("  diff sql script...");
#endif
                try
                {
                    sqlFileName = Path.ChangeExtension(projectDescription.BinFile, ".ndodiff." + schemaVersion + ".sql");
				    projectDescription.AddFileToProject(sqlFileName);
                }
                catch (Exception ex)
                {
                    messages.WriteLine("Warning: Can't add schema diff file '" + sqlFileName + "' to the project. " + ex.Message);
                }
        }
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
					MessageBox.Show("The NDO project file '" + fileName + "' is write protected, probably due to a checked in state of your Source Code Control system. NDO needs to update this file now. The ReadOnly attribute will be removed. If you check out the file the next time, make sure to keep the local version of the file.", "NDO Add-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					fi.Attributes &= (~FileAttributes.ReadOnly);
				}
				options.Save(pd);
			}
		}

		void PostProcess( ProjectDescription pd )
		{
		}

		public void
		onBuildProjConfigDone( string projectName, string projectConfig, string platform, string solutionConfig, bool success )
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
#if !NDO11
                try
                {
                    project = new ProjectIterator(solution)[projectName];
                }
                catch (Exception ex) { messages.WriteLine(ex.ToString()); } // project remains null, exception will be thrown later
#else
				try
				{
					project = solution.Projects.Item(projectName);
				}
				catch (Exception ex) { messages.WriteLine(ex.ToString()); } // project remains null, exception will be thrown later

#endif

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

                messages.WriteLine(EnhDate.String);

				ProjectDescription projectDescription = new ProjectDescription(solution, project);
                string projFileName = options.FileName;
                CheckProjectDescription(options, projectDescription, projFileName);

				string appName = "NDOEnhancer.exe";
				if ( platform == "x86" && OperatingSystem.Is64Bit )
				{
					appName = "Enhancerx86Stub.exe";
					messages.WriteLine("NDO-Addin: Using x86 Stub");
				}
				ConsoleProcess cp = new ConsoleProcess(false);
				int result = cp.Execute("\"" + Path.Combine(ApplicationObject.AssemblyPath, appName) + "\"",
					"\"" + projFileName + "\"");

                if (cp.Stdout != String.Empty)
					messages.WriteLine(cp.Stdout);

				string stderr = cp.Stderr;
				if(stderr != string.Empty)
				{
					Regex regex = new Regex(@"Error:");
					MatchCollection mc = regex.Matches(stderr);
					int lastmatch = mc.Count - 1;
					for(int i = 0; i < mc.Count; i++)
					{
						int endindex;
						if (i == lastmatch)
							endindex = stderr.Length;
						else
							endindex = mc[i + 1].Index;
						int startindex = mc[i].Index;
//						messages.WriteLine("[" + i + "]:" + startindex + ',' + endindex);
						// The substring always ends with a '\n', which should be removed.
						string outString = stderr.Substring(startindex, endindex - startindex);
						if (outString.EndsWith("\r\n"))
							outString = outString.Substring(0, outString.Length - 2);
//						messages.ShowError("|" + outString + "|");
						messages.ShowError(outString);
					}
				}
				//if ( projectDescription.ConfigurationOptions.EnableEnhancer )
				//    PostProcess( projectDescription );

                if (result != 0)
				{
					if (messages.Success)
						messages.ShowError("An unknown error occured in the NDO Enhancer");
					// Now messages.Success is false
				}
				if (messages.Success)
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
#if DEBUG
                    messages.ShowError( ex.ToString());
#else
                    messages.ShowError( ex.Message);
#endif
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
            finally
            {
                //ValueTypes.ClearInstance();
                System.GC.Collect();
            }
		}

		private _DTE					m_applicationObject;
	}
}
