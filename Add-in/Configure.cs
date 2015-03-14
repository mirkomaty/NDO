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
using System.Diagnostics;
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

namespace NDOAddIn
{
	/// <summary>
	/// Zusammenfassung für Configure.
	/// </summary>
	internal class Configure : AbstractCommand
	{

		public Configure()
		{
            this.CommandBarButtonText = "NDO Configuration";
            this.CommandBarButtonToolTip = ".NET Data Objects Configuration";
//            this.MyCommandName = "NDOAddIn.Connect.Configuration";
        }

	
		#region IDTExtensibility2 Member

		public override void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInstance, ref Array custom)
		{
            this.VisualStudioApplication = (_DTE)application;
            this.AddInInstance = (AddIn)addInInstance;

            Debug.WriteLine("Configure.OnConnection with connectMode " + connectMode.ToString());

            if (connectMode != ext_ConnectMode.ext_cm_UISetup && connectMode != ext_ConnectMode.ext_cm_AfterStartup)
                return;

            if (this.CommandExists)
            {
                Debug.WriteLine("Configure.OnConnection: command already exists");
                return;
                //----------------
            }

            Debug.WriteLine("Configure.OnConnection: creating command");


			try
			{
				Command command = this.AddNamedCommand( 102 );

                CommandBar commandBar = NDOCommandBar.Instance.CommandBar;
                CommandBarButton commandBarButton = (CommandBarButton)command.AddControl(commandBar, 1);
				commandBarButton.Style = MsoButtonStyle.msoButtonIcon;
		
				commandBar = (CommandBar) ((CommandBars)VisualStudioApplication.CommandBars)["Project"];
				commandBarButton = (CommandBarButton) command.AddControl( commandBar, 2 );
				commandBarButton.Style = MsoButtonStyle.msoButtonIconAndCaption;
#if !NDO11
                commandBar = (CommandBar)((CommandBars)VisualStudioApplication.CommandBars)["Web Project Folder"];
				commandBarButton = (CommandBarButton) command.AddControl( commandBar, 2 );
				commandBarButton.Style = MsoButtonStyle.msoButtonIconAndCaption;                
#endif
			}
			catch ( System.Exception ex )
			{
#if DEBUG
                string msg = ex.ToString();
#else
                string msg = ex.Message;
#endif
                System.Windows.Forms.MessageBox.Show(msg, "NDO Configure");
			}

		}



		#endregion
	
		#region IDTCommandTarget Member

		public override void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
		{
			if ( commandName == MyCommandName )
			{
				Array projects = VisualStudioApplication.ActiveSolutionProjects as Array;
				if ( 1 == projects.Length )
				{
					try
					{
						Project project = projects.GetValue( 0 ) as Project;
						ProjectDescription pd = new ProjectDescription(VisualStudioApplication.Solution, project);
						ConfigurationDialog dlg = new ConfigurationDialog( project, pd );
						dlg.ShowDialog();
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message, "NDO Configuration");
					}
				}

				handled = true;

				return;
			}
		}

		#endregion
	}
}
