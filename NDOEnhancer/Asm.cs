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
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Globalization;

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
                    messages.WriteLine("Wrong ILAsm version in file: " + ilAsmPath + ". CompanyName='" + fvi.CompanyName + "'; Version='" + fvi.FileVersion + "' Method used='" + method + "'. Trying to find ILAsm with other methods.");
                    ilAsmPath = string.Empty;
                }
            }
        }

		string GetIlAsmPath()
		{
			string path = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Windows ), @"Microsoft.NET\Framework" );
			foreach ( string dir in Directory.GetDirectories( path ) )
			{
				if ( Path.GetFileName( dir ).StartsWith( "v4.0", true, CultureInfo.InvariantCulture ) )
				{
					return dir;
				}
			}

			return string.Empty;
		}

		public Asm(MessageAdapter messages, bool verboseMode) : base(verboseMode)
		{
            this.messages = messages;

            this.ilAsmPath = typeof(string).Assembly.Location;
			this.ilAsmPath = Path.Combine( Path.GetDirectoryName( ilAsmPath ), "ilasm.exe" );
            CheckVersion("AssemblyLocation");
			if ( !File.Exists( this.ilAsmPath ) )
			{
				this.ilAsmPath = Path.Combine( GetIlAsmPath(), "ilasm.exe" );
				// Don't need a version check here.
			}
			if (!File.Exists(this.ilAsmPath))
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
					string pn = p + "ilasm.exe";
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
				throw new Exception("Path for ILAsm not found.\n  Add the path to ilasm.exe to the PATH environment variable.");
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
			if (errorMessage != string.Empty)
				throw new Exception("ILAsm: " + errorMessage);
		}
	}
}
