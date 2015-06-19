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
	class OpenMappingTool : AbstractCommand
	{

		public OpenMappingTool( _DTE dte, CommandID commandId )
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
				//				string s = "";
				//				foreach(EnvDTE.Property p in project.ConfigurationManager.ActiveConfiguration.Properties)
				//					s += p.Name + " ";
				//				MessageBox.Show(s);

				ProjectDescription pd = new ProjectDescription( this.VisualStudioApplication.Solution, project );
				if (!pd.MappingFileExists)
				{
					MessageBox.Show( "No Mapping File", "Open Mapping File" );
					return;
				}
				string exeFile = Path.Combine( ApplicationObject.AssemblyPath, "Mapping.exe" );
				string mappingFile = pd.CheckedMappingFileName;
				mappingFile = "\"" + mappingFile + '"';
				string s = "-m:" + mappingFile;
				ProcessStartInfo psi = new ProcessStartInfo( exeFile, s );
				psi.CreateNoWindow = false;
				psi.UseShellExecute = false;
				psi.RedirectStandardError = false;
				psi.RedirectStandardOutput = false;

				System.Diagnostics.Process proc = System.Diagnostics.Process.Start( psi );
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
			bool hasMapping = false;
			Array projects = this.VisualStudioApplication.ActiveSolutionProjects as Array;
			if (1 == projects.Length)
			{
				System.Array solObjects = (Array)this.VisualStudioApplication.ActiveSolutionProjects;
				if (solObjects.Length < 1)
					return;

				Project project = (Project)solObjects.GetValue( 0 );

				ConfigurationOptions options = new ConfigurationOptions( project );
				ProjectDescription pd = new ProjectDescription( this.VisualStudioApplication.Solution, project );
				if (options.EnableAddIn)
				{
					hasMapping = pd.MappingFileExists;
				}
			}
			item.Enabled = hasMapping;
		}

		public static implicit operator OleMenuCommand( OpenMappingTool abstractCommand )
		{
			return abstractCommand.command;
		}
	}
}
