//
// Copyright (C) 2002-2015 Mirko Matytschak 
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

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETDataObjects.NDOVSPackage
{
	class AddPersistentClass : AbstractCommand
	{

		public AddPersistentClass( _DTE dte, CommandID commandId )
			: base( dte, commandId )
		{
		}

		protected override void DoIt( object sender, EventArgs e )
		{
			try
			{
				System.Array solObjects = (Array)this.VisualStudioApplication.ActiveSolutionProjects;
				if (solObjects.Length < 1)
					return;

				Project project = (Project)solObjects.GetValue( 0 );

				if (!(CodeGenHelper.IsVbProject( project ) || CodeGenHelper.IsCsProject( project )))
					return;

				PersistentClassDialog pcd = new PersistentClassDialog();
				pcd.ShowDialog();
				if (pcd.Result == DialogResult.Cancel)
					return;

				SelectedItems selItems = VisualStudioApplication.SelectedItems;
				ProjectItem parentItem = null;
				if (selItems.Count == 1)
				{
					IEnumerator ienum = selItems.GetEnumerator();
					ienum.MoveNext();
					SelectedItem si = (SelectedItem)ienum.Current;
					if (si.ProjectItem != null && si.ProjectItem.Kind == "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}") // Folder
						parentItem = si.ProjectItem;
				}

				if (CodeGenHelper.IsVbProject( project ))
					new AddPersistentClassVb( project, pcd.ClassName, pcd.Serializable, parentItem ).DoIt();
				else if (CodeGenHelper.IsCsProject( project ))
					new AddPersistentClassCs( project, pcd.ClassName, pcd.Serializable, parentItem ).DoIt();
			}
			catch (Exception ex)
			{
				Debug.WriteLine( ex.ToString() );
				MessageBox.Show( ex.Message, "Configure" );
			}
		}

		protected override void OnBeforeQueryStatus( object sender, EventArgs e )
		{
			OleMenuCommand item = sender as OleMenuCommand;

			Array projects = VisualStudioApplication.ActiveSolutionProjects as Array;
			bool enabled = false;
			if (projects.Length >= 1)
			{
				Project project = (Project)projects.GetValue( 0 );
				enabled = CodeGenHelper.IsVbProject( project ) || CodeGenHelper.IsCsProject( project );
			}

			item.Enabled = enabled;
		}

		public static implicit operator OleMenuCommand( AddPersistentClass abstractCommand )
		{
			return abstractCommand.command;
		}
	}
}
