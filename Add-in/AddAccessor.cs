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
using System.IO;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
#if NET11
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif
using System.Text.RegularExpressions;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für AddAccessor.
	/// </summary>
	internal class AddAccessor : AbstractCommand
	{
		public AddAccessor()
		{
//            MyCommandName = "NDOEnhancer.Connect.AddAccessor";
            this.CommandBarButtonText = "Add Accessor";
            this.CommandBarButtonToolTip = "Adds an accessor property";
        }


		public void DoIt()
		{
			Document document;
			TextDocument textDoc;

			document = this.VisualStudioApplication.ActiveDocument;
			if (document == null) 
				return;
			
			textDoc = (TextDocument) document.Object("TextDocument");
			if (textDoc == null) 
				return;

			string fileName = document.FullName.ToLower();
			if (fileName.EndsWith(".cs"))
				new AddAccessorCs(textDoc, document).DoIt();
			else if (fileName.EndsWith(".vb"))
				new AddAccessorVb(textDoc, document).DoIt();
	
			
		}
	
		#region IDTCommandTarget Member

		public override void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
		{
			if ( commandName == this.MyCommandName )
			{
				DoIt();
				handled = true;
			}
		}

		public override void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if ( commandName == this.MyCommandName )
			{
				bool enabled = false;
				Document document = this.VisualStudioApplication.ActiveDocument;
				if (document != null) 
				{
			
					TextDocument textDoc = (TextDocument) document.Object("TextDocument");
					if (textDoc != null) 
					{
						string fileName = document.FullName.ToLower();
						if (fileName.EndsWith(".cs"))
							enabled = true;
						else if (fileName.EndsWith(".vb"))
							enabled = true;
					}
				}

				if (enabled)
					status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
				else
					status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported;

			}
		}

		#endregion



		#region IDTExtensibility2 Member

		public override void OnConnection(object application, ext_ConnectMode connectMode, object addInInstance, ref Array custom)
		{
            this.VisualStudioApplication = (_DTE)application;
            this.AddInInstance = (AddIn)addInInstance;
            Debug.WriteLine("AddAccessor.OnConnection with connectMode " + connectMode.ToString());

            if (connectMode != ext_ConnectMode.ext_cm_UISetup && connectMode != ext_ConnectMode.ext_cm_AfterStartup)
                return;

            if (this.CommandExists)
            {
                Debug.WriteLine("AddAccessor.OnConnection: command already exists");
                return;
                //----------------
            }

            Debug.WriteLine("AddAccessor.OnConnection: creating command");


			try
			{

				// AddAccessor-Kommando
				Command command = this.AddNamedCommand( 103 );

                CommandBar commandBar = (CommandBar)((CommandBars)this.VisualStudioApplication.CommandBars)["Code Window"];
				CommandBarButton cbb = (CommandBarButton) command.AddControl(commandBar, 1);
				// Use default for cbb.Style (context menu)

				commandBar = NDOCommandBar.Instance.CommandBar;
				cbb = (CommandBarButton) command.AddControl( commandBar, 2 );
				cbb.Style = MsoButtonStyle.msoButtonIcon;
			}
			catch (Exception e)
			{
#if DEBUG
				MessageBox.Show(e.ToString(), "NDO Add Accessor");
#else
				MessageBox.Show(e.Message, "NDO Add Accessor");
#endif
            }
		}



		#endregion
	}
}
	


