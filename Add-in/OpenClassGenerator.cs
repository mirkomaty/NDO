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

namespace NDOAddIn
{
	/// <summary>
	/// Zusammenfassung für Configure.
	/// </summary>
	internal class OpenClassGenerator : AbstractCommand
	{

		public OpenClassGenerator()
		{
		    this.CommandBarButtonText	= "Open Class Generator";
		    this.CommandBarButtonToolTip	= "Opens the NDO class generator";
//            this.MyCommandName = "NDOAddIn.Connect.OpenClassGenerator";
		}


		#region IDTExtensibility2 Member

		public override void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInstance, ref Array custom)
		{
            this.VisualStudioApplication = (_DTE)application;
            this.AddInInstance = (AddIn)addInInstance;

            Debug.WriteLine("OpenClassGenerator.OnConnection with connectMode " + connectMode.ToString());

            if (connectMode != ext_ConnectMode.ext_cm_UISetup && connectMode != ext_ConnectMode.ext_cm_AfterStartup)
                return;

            if (this.CommandExists)
            {
                Debug.WriteLine("OpenClassGenerator.OnConnection: command already exists");
                return;
                //----------------
            }

            Debug.WriteLine("OpenClassGenerator.OnConnection: creating command");

            CreateCommand();

		}

        private void CreateCommand()
        {

            try
            {
                Command command = this.AddNamedCommand( 109 );

                CommandBar commandBar = NDOCommandBar.Instance.CommandBar;
                CommandBarButton cbb = null;
                if (commandBar != null)
                {
                    cbb = (CommandBarButton)command.AddControl(commandBar, 6);
                    cbb.Style = MsoButtonStyle.msoButtonIcon;
                }

                commandBar = (CommandBar)((CommandBars)this.VisualStudioApplication.CommandBars)["Project"];
                cbb = (CommandBarButton)command.AddControl(commandBar, 3);
                cbb.Style = MsoButtonStyle.msoButtonIconAndCaption;

            }
            catch (System.Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "NDO Connection to VS (OpenClassGenerator)");
#else
				System.Windows.Forms.MessageBox.Show(ex.Message, "NDO Connection to VS (OpenClassGenerator)");
#endif
            }
        }


		#endregion
	
		#region IDTCommandTarget Member

		public override void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
		{
			if ( commandName == MyCommandName )
			{
                string exeFile = Path.Combine(ApplicationObject.AssemblyPath, "ClassGenerator.exe");
				if (!File.Exists(exeFile))
				{
					MessageBox.Show("Can't find the class generator executable at " + exeFile, "NDO");
					return;
				}
				string solDir = Path.GetDirectoryName(this.VisualStudioApplication.Solution.Properties.Item( "Path" ).Value as string);
				
				ProcessStartInfo psi = new ProcessStartInfo(exeFile, "\"" + solDir + "\"");
				psi.CreateNoWindow		   = false;
				psi.UseShellExecute		   = false;
				psi.RedirectStandardError  = false;
				psi.RedirectStandardOutput = false;

				System.Diagnostics.Process proc = System.Diagnostics.Process.Start( psi );

				handled = true;
				return;
			}
		}

		public override void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if ( commandName == MyCommandName )
			{
				if (this.VisualStudioApplication.Solution.FullName != string.Empty)
				{
					status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
				}
				else
					status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported;
			}
		}

		#endregion
	}
}
