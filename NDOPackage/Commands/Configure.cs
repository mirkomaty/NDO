﻿//
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

using System.Diagnostics;
using MessageBox = System.Windows.Forms.MessageBox;

namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidNDOConfiguration)]
    internal sealed class Configure : BaseCommand<Configure>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
			try
			{
				var project = await VS.Solutions.GetActiveProjectAsync();
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
				
				if (project != null)
				{
					try
					{
						ProjectDescription pd = new ProjectDescription( project );
						ConfigurationDialog dlg = new ConfigurationDialog( project.DteProject(), pd );
						dlg.ShowDialog();
					}
					catch (Exception ex)
					{
						Debug.WriteLine( ex.ToString() );
						MessageBox.Show( ex.Message, "NDO Configuration" );
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine( ex.ToString() );
				MessageBox.Show( ex.Message, "Configure" );
			}
		}

        protected override void BeforeQueryStatus(EventArgs e)
        {
            Command.Visible = true;
        }
    }
}
