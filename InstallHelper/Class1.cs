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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;
using Microsoft.Win32;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;

namespace InstallHelper
{
	/// <summary>
	/// Summary description for CustomizeInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class CustomizeInstaller : Installer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container		components = null;
        string installPath;
        string[] addInFileNames = {"NDO21-VS2010.AddIn", "NDO21-VS2012.AddIn"};

		public CustomizeInstaller()
		{
            this.installPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
            // This call is required by the Designer.
			InitializeComponent();
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion


		public override void Install( IDictionary stateSaver )
		{
			try
			{
				AssemblyInstaller assemblyInstaller = Parent as AssemblyInstaller;
				base.Install( stateSaver );
			}
			catch
			{
			}
		}


		protected override void OnCommitted(IDictionary savedState)
		{
			base.OnCommitted (savedState);

			string enhPath = Path.Combine(this.installPath, "NDOEnhancer.dll");
			string addInPath = Path.Combine( this.installPath, "NDO21.AddIn" );
			string addInText = null; ;
			using ( StreamReader sr = new StreamReader( addInPath, System.Text.Encoding.Unicode ) )
			{
				addInText = sr.ReadToEnd();
				sr.Close();
			}
			for ( int i = 0; i < this.addInFileNames.Length; i++ )
			{
				string addInFileName = this.addInFileNames[i];
				try
				{
					addInText = addInText.Replace( @"YourAddInPath", enhPath );

					if ( i == 1 )
						addInText = addInText.Replace( "<Version>10.0", "<Version>11.0" );

					string addInTargetPath = Path.Combine( GetAddInTargetPath( true ), addInFileName );
					StreamWriter sw = new StreamWriter( addInTargetPath, false, System.Text.Encoding.Unicode );
					sw.Write( addInText );
					sw.Close();
				}
				catch (Exception ex)
				{
					Log( ex.Message );
				}
			}
        }

        void Log(string msg)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(this.installPath, "InstLog.txt"), true))
            {
                sw.WriteLine(msg);
            }
        }

        void Execute(string exePath, string parameters)
        {
            ConsoleProcess cp = new ConsoleProcess();
            try
            {
                Log(">" + exePath + " " + parameters);
                cp.Execute(exePath, parameters);
                Log("Err: " + cp.Stderr);
                Log("Out:" + cp.Stdout);
            }
            catch (Exception ex)
            {
                Log("Execute: " + ex.Message);
            }
        }

		string GetAddInTargetPath(bool createIfNotExists)
        {
            string root = Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
            string subPath = null;
            try
            {
				Regex regex = new Regex( @"\d\.\d" );
				Match match = regex.Match( Environment.OSVersion.ToString() );
				if ( match.Value == "6.1" || match.Value == "6.2")  // Windows 7 / Windows 8
					subPath = @"Microsoft\MSEnvShared\Addins";
				else  // Windows Vista
					subPath = @"Application Data\Microsoft\MSEnvShared\Addins";

                if (createIfNotExists)
                {
                    DirectoryInfo di = new DirectoryInfo(root);
                    di.CreateSubdirectory(subPath);
                }
            }
            catch { }
            return Path.Combine(root, subPath);
        }


        public override void Uninstall(IDictionary savedState)
        {
            try
            {
				foreach ( string addInFileName in this.addInFileNames )
				{
					string addInTargetPath = Path.Combine( GetAddInTargetPath( false ), addInFileName );
					if ( File.Exists( addInTargetPath ) )
						File.Delete( addInTargetPath );
				}
            }
            catch { }

            try
            {
	            Type t = Type.GetTypeFromProgID("VisualStudio.DTE");
	            EnvDTE.DTE dte = (DTE) Activator.CreateInstance(t);
	            CommandBars cbs = (CommandBars) dte.CommandBars;
        	    Commands cmds = dte.Commands;
	            foreach (CommandBar cb in cbs)
        	    {
                	if (cb.Name.StartsWith(".NET Data Objects"))
                	    cmds.RemoveCommandBar(cb);
        	    }
            }
            catch { }


        }

	}
}
