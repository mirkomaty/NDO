//
// Copyright (c) 2002-2022 Mirko Matytschak 
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


using EnvDTE;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageBox = System.Windows.Forms.MessageBox;
using Project = EnvDTE.Project;


namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidOpenMappingTool)]
    internal sealed class OpenMappingTool : BaseCommand<OpenMappingTool>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
			try
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
				System.Array solObjects = (Array) ApplicationObject.VisualStudioApplication.ActiveSolutionProjects;
				if (solObjects.Length < 1)
					return;

				Project project = (Project) solObjects.GetValue( 0 );
				//				string s = "";
				//				foreach(EnvDTE.Property p in project.ConfigurationManager.ActiveConfiguration.Properties)
				//					s += p.Name + " ";
				//				MessageBox.Show(s);

				string exeFile = Path.Combine( ApplicationObject.AssemblyPath, "MappingTool", "Mapping.exe" );
				string mappingFile = project.MappingFilePath();
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
				MessageBox.Show( ex.Message, "Open Mapping Tool" );
			}
		}

        protected override void BeforeQueryStatus( EventArgs e )
        {
			ThreadHelper.JoinableTaskFactory.Run( async () =>
			{
                var activeProj = await VS.Solutions.GetActiveProjectAsync();
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Command.Enabled = activeProj.MappingFileExists();
			} );
		}
    }
}
