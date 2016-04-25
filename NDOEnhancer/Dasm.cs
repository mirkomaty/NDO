//
// Copyright (c) 2002-2016 Mirko Matytschak 
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


		string GetILDasmPath()
		{
			string path = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 ), @"Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools" );
			if ( Directory.Exists( path ) )
				return path ;

			path = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 ), @"Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools" );
			if ( Directory.Exists( path ) )
				return path ;

			return string.Empty;
		}

		public Dasm(MessageAdapter messages, bool verboseMode)
		{
			this.messages = messages;
            this.verboseMode = verboseMode;

			this.ilDasmPath = Path.Combine( GetILDasmPath(), "ildasm.exe" );
			CheckVersion( "GetFolderPath" );

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
				throw new Exception("Path for ILDasm not found. Add the path to ildasm.exe to the PATH environment variable.");
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
