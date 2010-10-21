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
        string path;
        string docPath;
        string hxregPath;
        const string addInFileName = "NDO21.AddIn";

		public CustomizeInstaller()
		{
            path = Path.GetDirectoryName(this.GetType().Assembly.Location);
            docPath = Path.Combine(path, "doc");
            hxregPath = "\"" + Path.Combine(docPath, "InnovaHxReg.exe") + "\"";
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

/*
		string GetVSVersion()
		{
            try
            {
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(@"VisualStudio.DTE\CurVer");
                string vsCurVersion = (string) regKey.GetValue("");
                Regex regex = new Regex(@"VisualStudio\.DTE\.(\d+\.\d+)");
                Match match = regex.Match(vsCurVersion);
                if (!match.Success)
                    Log("Wrong vsCurVersion format: " + vsCurVersion);
                else
                    return match.Groups[1].Value;
            }
            catch(Exception ex)
            {
                Log("GetVSVersion: " + ex.Message);
            }
            return "10.0";  // try to somehow accomplish the 

            //Type t = Type.GetTypeFromProgID("VisualStudio.DTE");
            //object o = Activator.CreateInstance(t);
            //return (string) o.GetType().InvokeMember("Version", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty, null, o, new object[]{});
		}
*/		

		protected override void OnCommitted(IDictionary savedState)
		{
			base.OnCommitted (savedState);

			string enhPath = Path.Combine(path, "NDOEnhancer.dll");
            string addInPath = Path.Combine(path, addInFileName);
            try
            {
                StreamReader sr = new StreamReader(addInPath, System.Text.Encoding.Unicode);
                string addInText = sr.ReadToEnd();
                sr.Close();
				//
				//string vsVersion = "9.0";
				//try
				//{
				//    vsVersion = GetVSVersion();
				//}
				//catch { }
				//if (vsVersion != "8.0")
				//    addInText = addInText.Replace("8.0", vsVersion);
                addInText = addInText.Replace(@"YourAddInPath", enhPath);
                string addInTargetPath = Path.Combine(GetAddInTargetPath(true), addInFileName);
                StreamWriter sw = new StreamWriter(addInTargetPath, false, System.Text.Encoding.Unicode);
                sw.Write(addInText);
                sw.Close();
            }
            catch { }

#if NoReference
            string hxcPath = "\"" + Path.Combine(docPath, "NDOReferenceCollection.HxC") + "\"";
            string hxsPath = "\"" + Path.Combine(docPath, "NDOReference.HxS") + "\"";
            
            this.Execute(hxregPath, @"/R /N /Namespace:HoT.NDOReference /Description:"".NET Data Objects (NDO) Reference"" /Collection:" + hxcPath);
            this.Execute(hxregPath, @"/R /T /Namespace:HoT.NDOReference /Id:NDOReference /LangId:1033 /Helpfile:" + hxsPath);
            this.Execute(hxregPath, @"/R /F /Filtername:""NDO Reference"" /Filterquery:\""DocSet\""=\""NDOReference\"" /Namespace:HoT.NDOReference");
            if (GetVSVersion() == "9.0")
                this.Execute(hxregPath, @"/R /P /productnamespace:MS.VSIPCC.v90 /producthxt:_DEFAULT /namespace:HoT.NDOReference /hxt:_DEFAULT");
            else
                this.Execute(hxregPath, @"/R /P /productnamespace:MS.VSIPCC.v80 /producthxt:_DEFAULT /namespace:HoT.NDOReference /hxt:_DEFAULT");

            hxcPath = "\"" + Path.Combine(docPath, "COL_NDO_Help.HxC") + "\"";
            hxsPath = "\"" + Path.Combine(docPath, "NDOUsersGuide.hxs") + "\"";

            this.Execute(hxregPath, @"/R /N /Namespace:HoT.NDOUsersGuide /Description:"".NET Data Objects (NDO) Users Guide"" /Collection:" + hxcPath);
            this.Execute(hxregPath, @"/R /T /Namespace:HoT.NDOUsersGuide /Id:NDOUsersGuide /LangId:1033 /Helpfile:" + hxsPath);
            this.Execute(hxregPath, @"/R /F /Filtername:""NDO UsersGuide"" /Filterquery:\""DocSet\""=\""HoT.NDOUsersGuide\"" /Namespace:HoT.NDOUsersGuide");
            if (GetVSVersion() == "9.0")
                this.Execute(hxregPath, @"/R /P /productnamespace:MS.VSIPCC.v90 /producthxt:_DEFAULT /namespace:HoT.NDOUsersGuide /hxt:_DEFAULT");
            else
                this.Execute(hxregPath, @"/R /P /productnamespace:MS.VSIPCC.v80 /producthxt:_DEFAULT /namespace:HoT.NDOUsersGuide /hxt:_DEFAULT");
            this.Execute(hxregPath, @"/R /F /Filtername:""(no filter)"" /Filterquery: /Namespace:HoT.NDOReference");
#endif
        }

        void Log(string msg)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(docPath, "InstLog.txt"), true))
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
                if (root == null)
                {
                    root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    subPath = @"Visual Studio 2010\Addins";
                }
                else
                {
					Regex regex = new Regex( @"\d\.\d" );
					Match match = regex.Match( Environment.OSVersion.ToString() );
					if ( match.Value == "6.1" )
						subPath = @"Microsoft\MSEnvShared\Addins";
					else
						subPath = @"Application Data\Microsoft\MSEnvShared\Addins";
                }

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
#if NoReference
            try
            {
                ConsoleProcess cp = new ConsoleProcess();

                if (GetVSVersion() == "9.0")
                {
                    cp.Execute(hxregPath, @"/U /P /productnamespace:MS.VSIPCC.v90 /producthxt:_DEFAULT /namespace:HoT.NDOReference /hxt:_DEFAULT");
                    cp.Execute(hxregPath, @"/U /P /productnamespace:MS.VSIPCC.v90 /producthxt:_DEFAULT /namespace:HoT.NDOUsersGuide /hxt:_DEFAULT");
                }
                else
                {
                    cp.Execute(hxregPath, @"/U /P /productnamespace:MS.VSIPCC.v80 /producthxt:_DEFAULT /namespace:HoT.NDOReference /hxt:_DEFAULT");
                    cp.Execute(hxregPath, @"/U /P /productnamespace:MS.VSIPCC.v80 /producthxt:_DEFAULT /namespace:HoT.NDOUsersGuide /hxt:_DEFAULT");
                }

                cp.Execute(hxregPath, @"InnovaHxReg /U /N /Namespace:HoT.NDOReference");
            }
            catch { }
#endif
            try
            {
				string addInTargetPath = Path.Combine(GetAddInTargetPath(false), addInFileName);
				if (File.Exists(addInTargetPath))
					File.Delete(addInTargetPath);
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
