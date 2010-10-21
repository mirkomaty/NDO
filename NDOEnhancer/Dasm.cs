//
// Copyright (C) 2002-2009 Mirko Matytschak 
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
//


using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for Dasm.
	/// </summary>
	internal class Dasm
	{
		string ilDasmPath = string.Empty;
		MessageAdapter messages;
        bool verboseMode;

        void CheckVersion(string method)
        {
            if (File.Exists(ilDasmPath))
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(ilDasmPath);
                if (string.Compare(fvi.CompanyName, "Microsoft Corporation", true) != 0
                    && fvi.FileVersion.StartsWith("4.0"))
                {
                    messages.WriteLine("Wrong ILDasm version in file: " + ilDasmPath + ". CompanyName='" + fvi.CompanyName + "'; Version='" + fvi.FileVersion + "' Method used='" + method + "'. Trying to find ILDasm with other methods.");
                    ilDasmPath = string.Empty;
                }
            }
        }


		public Dasm(MessageAdapter messages, bool verboseMode)
		{
			this.messages = messages;
            this.verboseMode = verboseMode;

#if NET20
            RegistryKey sdkKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\.NETFramework\v2.0");
            if (sdkKey != null)
            {
                string installDir = (string) sdkKey.GetValue("InstallationFolder");
                if (installDir != null)
                {
                    ilDasmPath = Path.Combine(installDir, @"bin\ildasm.exe");
                }
                CheckVersion("SDKRegistry");
            }
            if (!File.Exists(ilDasmPath))
            {
#endif
			RegistryKey vsKey = Registry.ClassesRoot.OpenSubKey(@"VisualStudio.Solution\CLSID");
			if (vsKey != null)
			{
				string vsClsid = (string) vsKey.GetValue("");
				if (vsClsid != null)
				{
					RegistryKey dasmKey = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + vsClsid + @"\LocalServer32");

					if (dasmKey != null)
					{
						ilDasmPath = (string)dasmKey.GetValue(null);
						ilDasmPath = ilDasmPath.Replace("\"", string.Empty);
						ilDasmPath = Path.GetDirectoryName(ilDasmPath);
						ilDasmPath = Directory.GetParent(ilDasmPath).ToString();
						ilDasmPath = Directory.GetParent(ilDasmPath).ToString();
#if NET11
						ilDasmPath = Path.Combine(ilDasmPath, @"SDK\v1.1\Bin");
#else
                        ilDasmPath = Path.Combine(ilDasmPath, @"SDK\v2.0\Bin");
#endif
						ilDasmPath = Path.Combine(ilDasmPath, "ILDasm.exe");
                        CheckVersion("VSClsId");
					}
				}
			}
#if NET20
            }// !File.Exists
#endif

			//----Path
			if (!File.Exists(ilDasmPath))
			{
                string pathVar = System.Environment.GetEnvironmentVariable("PATH");
			    string[] thePaths = pathVar.Split(';');
                for (int i = 0; i < thePaths.Length; i++)
                {
                    if (!thePaths[i].EndsWith("\\"))
                        thePaths[i] += '\\';
                }

				ilDasmPath = null;
				foreach(string p in thePaths)
				{
					if (File.Exists(p + "ILDasm.exe"))
					{
						ilDasmPath = p + "ILDasm.exe";
						break;
					}
				}
                CheckVersion("PathEnvironment");
			}

			if (!File.Exists(ilDasmPath))
			{
				throw new Exception("Path for ILDasm not found; set PATH environment variable");
			}

		}

		public void DoIt(string dllName, string ilName)
		{
            if (verboseMode)
			    messages.WriteLine("Disassembling: " + dllName + "->" + ilName);

			ProcessStartInfo psi = new ProcessStartInfo( ilDasmPath, 
				" /LINENUM"
				+ " /NOBAR"
				+ " /UNICODE "
				+ '"' + dllName + '"'
				+ @" /OUT=""" + ilName + '"');
			psi.CreateNoWindow		   = true;
			psi.UseShellExecute		   = false;
			psi.WorkingDirectory	   = Path.GetDirectoryName(dllName);
			psi.RedirectStandardError  = true;

			System.Diagnostics.Process proc = System.Diagnostics.Process.Start( psi );
			proc.WaitForExit();
			string stderr = proc.StandardError.ReadToEnd();

			if ( 0 < stderr.Length )
			{
				if (stderr.IndexOf("ERROR") > -1)
					throw new Exception(stderr);
			}
		}
	}
}
