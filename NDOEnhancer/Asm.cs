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
using Microsoft.Win32;

namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for Asm.
	/// </summary>
	internal class Asm : ConsoleProcess
	{
		string ilAsmPath;
        MessageAdapter messages;

        void CheckVersion(string method)
        {
            if (File.Exists(ilAsmPath))
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(ilAsmPath);
                if (string.Compare(fvi.CompanyName, "Microsoft Corporation", true) != 0)
                {
                    messages.WriteLine("Wrong ILAsm version in file: " + ilAsmPath + ". CompanyName='" + fvi.CompanyName + "'; Version='" + fvi.FileVersion + "' Method used='" + method + "'. Trying to find ILDasm with other methods.");
                    ilAsmPath = string.Empty;
                }
            }
        }


		public Asm(MessageAdapter messages, bool verboseMode) : base(verboseMode)
		{
            this.messages = messages;

            ilAsmPath = typeof(string).Assembly.Location;
			ilAsmPath = Path.GetDirectoryName(ilAsmPath) + "\\ILAsm.exe";
            CheckVersion("AssemblyLocation");
			
			if (!File.Exists(ilAsmPath))
			{
                string pathVar = Environment.GetEnvironmentVariable("PATH");
                string[] thePaths = pathVar.Split(';');
                for (int i = 0; i < thePaths.Length; i++)
                {
                    if (!thePaths[i].EndsWith("\\"))
                        thePaths[i] += '\\';
                }
                foreach (string p in thePaths)
				{
					string pn = p + "ILAsm.exe";
					if (File.Exists(pn))
					{
						ilAsmPath = pn;
						break;
					}
				}
                CheckVersion("PathEnvironment");
			}
			if (!File.Exists(ilAsmPath))
			{
				throw new Exception("Path for ILAsm not found.\n  Set the PATH environment variable.");
			}
		}

		public void DoIt(string ilFileName, string dllFileName, string keyFileName, bool debug)
		{
			if (!File.Exists(ilFileName))
				throw new Exception("Assembling: File not found: " + ilFileName);

			// Dll or Exe?
			string libMode = "/" + Path.GetExtension(dllFileName).Substring(1).ToUpper();

#if NDO11
			string debugMode = debug ? " /DEBUG" : string.Empty;
#else
			string debugMode = debug ? " /DEBUG /PDB" : string.Empty;
#endif
			string key = keyFileName != null ? " /KEY=\"" + keyFileName + '"' : string.Empty;

			string resourceFile = Path.ChangeExtension(dllFileName, ".res");

			string resource = File.Exists(resourceFile) ? " /RESOURCE=\"" + resourceFile + '"' : string.Empty;

			string parameters = 
				libMode
				+ debugMode
				+ key
				+ resource
				+ " /QUIET"
                + " /NOLOGO"
                + " /OUTFILE=\"" + dllFileName + "\""
				+ " \"" + ilFileName + "\"" ;

			Execute(ilAsmPath, parameters, Path.GetDirectoryName(dllFileName));
			string errorMessage = string.Empty;
			if (Stderr != string.Empty)
				errorMessage = Stderr;
			int p = Stdout.IndexOf("Error:");
			if (p > -1)
			{
				if (errorMessage != string.Empty)
					errorMessage += '\n';
				errorMessage += Stdout.Substring(p + 7);
			}
//			p = 0;
//			int q; 
//			while ((q = stdout.IndexOf("error --", p)) > -1)
//			{
//				int r = stdout.IndexOf('\n', q);
//				if (r == -1)
//					r = stdout.Length - 1;
//				errorMessage += stdout.Substring(q, r - 1 + 1);
//				p = q;
//			}
			if (errorMessage != string.Empty)
				throw new Exception("ILAsm: " + errorMessage);
		}
	}
}
