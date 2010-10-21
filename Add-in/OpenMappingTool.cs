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
using System.Diagnostics;
using System.Collections;
using System.IO;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
using Extensibility;
#if NET11
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif
using System.Windows.Forms;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für Configure.
	/// </summary>
	internal class OpenMappingTool : AbstractCommand
	{

		public OpenMappingTool()
		{
		    this.CommandBarButtonText	= "Open Mapping Tool";
		    this.CommandBarButtonToolTip	= "Opens the NDO mapping tool";
//            this.MyCommandName = "NDOEnhancer.Connect.OpenMappingTool";
		}


		#region IDTExtensibility2 Member

		public override void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInstance, ref Array custom)
		{
            this.VisualStudioApplication = (_DTE)application;
            this.AddInInstance = (AddIn)addInInstance;

            Debug.WriteLine("OpenMappingTool.OnConnection with connectMode " + connectMode.ToString());

            if (connectMode != ext_ConnectMode.ext_cm_UISetup && connectMode != ext_ConnectMode.ext_cm_AfterStartup)
                return;

            if (this.CommandExists)
            {
                Debug.WriteLine("OpenMappingTool.OnConnection: command already exists");
                return;
                //----------------
            }

            CreateCommand();
		}

        private void CreateCommand()
        {
            object[] contextGUIDS = new object[] { };
            Commands commands = this.VisualStudioApplication.Commands;
            Debug.WriteLine("OpenMappingTool.OnConnection: creating command");

            try
            {
                Command command = this.AddNamedCommand( 106 );

                CommandBar commandBar = NDOCommandBar.Instance.CommandBar;
                CommandBarButton commandBarButton;

                if (commandBar != null)
                {
                    commandBarButton = (CommandBarButton)command.AddControl(commandBar, 5);
                    commandBarButton.Style = MsoButtonStyle.msoButtonIcon;
                }

                commandBar = (CommandBar)((CommandBars)this.VisualStudioApplication.CommandBars)["Project"];
                if (commandBar != null)
                {
                    commandBarButton = (CommandBarButton)command.AddControl(commandBar, 2);
                    commandBarButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
                }

#if !NDO11
                commandBar = (CommandBar)((CommandBars)this.VisualStudioApplication.CommandBars)["Web Project Folder"];
                if (commandBar != null)
                {
                    commandBarButton = (CommandBarButton)command.AddControl(commandBar, 2);
                    commandBarButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
                }
#endif

            }
            catch (System.Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "NDO Open Mapping Tool");
#else
				System.Windows.Forms.MessageBox.Show(ex.Message, "NDO Open Mapping Tool");
#endif
            }
        }


		#endregion
	
		#region IDTCommandTarget Member

		public override void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
		{
			if ( commandName == this.MyCommandName )
			{
				System.Array solObjects = (Array) this.VisualStudioApplication.ActiveSolutionProjects;
				if (solObjects.Length < 1)
					return;

				Project project = (Project) solObjects.GetValue(0);
//				string s = "";
//				foreach(EnvDTE.Property p in project.ConfigurationManager.ActiveConfiguration.Properties)
//					s += p.Name + " ";
//				MessageBox.Show(s);

                //string assName = project.Properties.Item("AssemblyName").Value.ToString().ToLower();
                //string projPath = Path.GetDirectoryName(project.FileName) + "\\";

#if nix
				Configuration conf = project.ConfigurationManager.ActiveConfiguration;
				string binPath = projPath 
					+ conf.Properties.Item("OutputPath").Value 
					+ project.Properties.Item("OutputFileName").Value;



				
				MessageBox.Show(exeFile + " " + "-a:" + binPath + " -m:" + mappingFile);
#endif
				ProjectDescription pd = new ProjectDescription(this.VisualStudioApplication.Solution, project);
                if (!pd.MappingFileExists)
                {
                    MessageBox.Show("No Mapping File", "Open Mapping File");
                    goto exit;
                }
				string exeFile = Path.Combine(ApplicationObject.AssemblyPath, "Mapping.exe");
                string mappingFile = pd.CheckedMappingFileName;
                //string mappingFile = null;
                //string mapFile = assName + ".ndo.xml";
                //foreach(ProjectItem item in project.ProjectItems)
                //{
                //    if (string.Compare(item.Name, "ndomapping.xml", true) == 0 
                //        || string.Compare(item.Name, mapFile, true) == 0)
						
                //        mappingFile = item.Name;
                //}

                //if (mappingFile == null)
                //{
                //    MessageBox.Show("No Mapping File", "Open Mapping File");
                //    goto exit;
                //}

                //mappingFile = Path.Combine(projPath, mappingFile);
                //if (mappingFile.IndexOf(' ') > -1)
                mappingFile = "\"" + mappingFile + '"';
				string s = "-m:" + mappingFile;
				ProcessStartInfo psi = new ProcessStartInfo( exeFile, s);
				psi.CreateNoWindow		   = false;
				psi.UseShellExecute		   = false;
				psi.RedirectStandardError  = false;
				psi.RedirectStandardOutput = false;

				System.Diagnostics.Process proc = System.Diagnostics.Process.Start( psi );
				

			exit:
				handled = true;
				return;
			}
		}

		public override void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if ( commandName == this.MyCommandName )
			{
                bool hasMapping = false;
                Array projects = this.VisualStudioApplication.ActiveSolutionProjects as Array;
				if ( 1 == projects.Length )
				{
                    System.Array solObjects = (Array)this.VisualStudioApplication.ActiveSolutionProjects;
					if (solObjects.Length < 1)
						return;

					Project project = (Project) solObjects.GetValue(0);


                    ConfigurationOptions options = new ConfigurationOptions(project);
                    ProjectDescription pd = new ProjectDescription(this.VisualStudioApplication.Solution, project);
                    if (options.EnableAddIn)
                    {
                        //string assName = project.Properties.Item("AssemblyName").Value.ToString().ToLower();
                        //string assName = pd.AssemblyName;
                        //string mapFile = assName + ".ndo.xml";
                        //foreach (ProjectItem item in project.ProjectItems)
                        //{
                        //    string itemName = item.Name.ToLower();
                        //    if (itemName == "ndomapping.xml" || itemName == mapFile)
                        //        hasMapping = true;
                        //}
                        hasMapping = pd.MappingFileExists;
                    }
				}
                if (hasMapping)
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                else
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported;
			}
		}

		#endregion
	}
}
