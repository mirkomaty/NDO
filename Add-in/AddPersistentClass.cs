//
// Copyright (C) 2002-2010 Mirko Matytschak 
// (www.netdataobjects.de)
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
using System.Collections;
#endif
//using System.Text.RegularExpressions;

namespace NDOAddIn
{
	/// <summary>
	/// Zusammenfassung für AddPersistentClass.
	/// </summary>
	internal class AddPersistentClass : AbstractCommand
	{

		public AddPersistentClass()
		{
//            this.MyCommandName = "NDOAddIn.Connect.AddPersistentClass";
            this.CommandBarButtonToolTip = "Adds a persistent class for NDO";
            this.CommandBarButtonText = "Add Persistent Class";
		}

		public void DoIt()
		{
			System.Array solObjects = (Array) this.VisualStudioApplication.ActiveSolutionProjects;
			if (solObjects.Length < 1)
				return;

			Project project = (Project) solObjects.GetValue(0);

			if (!(CodeGenHelper.IsVbProject(project) || CodeGenHelper.IsCsProject(project)))
				return;

			PersistentClassDialog pcd = new PersistentClassDialog();
			pcd.ShowDialog();
			if (pcd.Result == DialogResult.Cancel)
				return;

			SelectedItems selItems = VisualStudioApplication.SelectedItems;
			ProjectItem parentItem = null;
			if ( selItems.Count == 1 )
			{
				IEnumerator ienum = selItems.GetEnumerator();
				ienum.MoveNext();
				SelectedItem si = (SelectedItem) ienum.Current;
				if ( si.ProjectItem != null && si.ProjectItem.Kind == "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}" ) // Folder
					parentItem = si.ProjectItem;
			}

			if (CodeGenHelper.IsVbProject(project))
				new AddPersistentClassVb(project, pcd.ClassName, pcd.Serializable, parentItem).DoIt();
			else if (CodeGenHelper.IsCsProject(project))
				new AddPersistentClassCs(project, pcd.ClassName, pcd.Serializable, parentItem).DoIt();
		}

	
		#region IDTExtensibility2 Member

		public override void OnConnection(object application, ext_ConnectMode connectMode, object addInInstance, ref Array custom)
		{
			this.VisualStudioApplication = (_DTE) application;
            this.AddInInstance = (AddIn)addInInstance;
            Debug.WriteLine("AddPersistentClass.OnConnection with connectMode " + connectMode.ToString());

            if (connectMode != ext_ConnectMode.ext_cm_UISetup && connectMode != ext_ConnectMode.ext_cm_AfterStartup)
                return;

            if (this.CommandExists)
            {
                Debug.WriteLine("AddPersistentClass.OnConnection: command already exists");
                return;
              //----------------
            }

            Debug.WriteLine("AddPersistentClass.OnConnection: creating command");

			try
			{
				Command command = this.AddNamedCommand( 104 );

                CommandBar commandBar = (CommandBar)((CommandBars)VisualStudioApplication.CommandBars)["Code Window"];

				CommandBarButton cbb = (CommandBarButton) command.AddControl(commandBar, 2);
				// Use default style (context menu)

				commandBar = NDOCommandBar.Instance.CommandBar;
				cbb = (CommandBarButton) command.AddControl( commandBar, 3 );
				cbb.Style = MsoButtonStyle.msoButtonIcon;

                commandBar = (CommandBar)((CommandBars)VisualStudioApplication.CommandBars)["Project"];
				cbb = (CommandBarButton) command.AddControl( commandBar, 2 );
				cbb.Style = MsoButtonStyle.msoButtonIconAndCaption;

			}
			catch (Exception e)
			{
#if DEBUG
                MessageBox.Show(e.ToString(), "NDO AddPersistentClass");
#else
				MessageBox.Show(e.Message, "NDO AddPersistentClass");
#endif
			}
		}


		#endregion
	
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
			if ( commandName != this.MyCommandName )
				return;
			
			Array projects = VisualStudioApplication.ActiveSolutionProjects as Array;
			if ( projects.Length >= 1 )
			{
				Project project = (Project) projects.GetValue(0);
				if (CodeGenHelper.IsVbProject(project) || CodeGenHelper.IsCsProject(project))
					status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported 
							| vsCommandStatus.vsCommandStatusEnabled;
				return;
			}
				
			status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported;
			
		}

		#endregion


	}

}