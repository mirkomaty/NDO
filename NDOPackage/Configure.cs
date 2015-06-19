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
	class Configure : AbstractCommand
	{

		public Configure( _DTE dte, CommandID commandId )
			: base( dte, commandId )
		{
		}

		protected override void DoIt( object sender, EventArgs e )
		{
			OleMenuCommand item = sender as OleMenuCommand;

			try
			{
				Debug.WriteLine( "Configure.DoIt" );
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
					catch (Exception ex)
					{
						Debug.WriteLine( ex.ToString() );
						MessageBox.Show(ex.Message, "NDO Configuration");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine( ex.ToString() );
				MessageBox.Show( ex.Message, "Configure" );
			}
		}

		public static implicit operator OleMenuCommand( Configure abstractCommand )
		{
			return abstractCommand.command;
		}
	}
}
