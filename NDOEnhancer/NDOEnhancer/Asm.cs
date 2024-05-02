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


using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Globalization;
using System.Reflection;

namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for Asm.
	/// </summary>
	internal class Asm : ConsoleProcess
	{
		string ilAsmPath;
        MessageAdapter messages;

		void CheckIlAsmPath()
		{
			string path;
			if (System.OperatingSystem.IsLinux())
				path = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "runtimes", "linux-x64", "native", "ilasm" );
			else if (System.OperatingSystem.IsWindows())
				path = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "runtimes", "win-x64", "native", "ilasm.exe" );
			else
				throw new Exception( "Can't determine, if NDOEnhancer is running on Windows or Linux." );

			this.ilAsmPath = path;
		}

		public Asm(MessageAdapter messages, bool verboseMode) : base(verboseMode)
		{
            this.messages = messages;

			CheckIlAsmPath();
		}

		public void DoIt(string ilFileName, string dllFileName, string keyFileName, bool debug)
		{
			if (!File.Exists(ilFileName))
				throw new Exception("Assembling: File not found: " + ilFileName);

			// Dll or Exe?
			string libMode = "/" + Path.GetExtension(dllFileName).Substring(1).ToUpper();
			string debugMode = debug ? " /DEBUG /PDB" : string.Empty;
			string key = keyFileName != null ? " /KEY=\"" + keyFileName + '"' : string.Empty;

			string parameters = 
				libMode
				+ debugMode
				+ key
				+ " /QUIET"
                + " /NOLOGO"
                + " /OUTFILE=\"" + dllFileName + "\""
				+ " \"" + ilFileName + "\"" ;

			Execute(ilAsmPath, parameters/*, Path.GetDirectoryName(dllFileName)*/);
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
			{
				if (errorMessage.IndexOf( "error", StringComparison.InvariantCultureIgnoreCase ) > -1)
				{
					Console.WriteLine( errorMessage );
					throw new Exception( "ILAsm failure" );
				}
				else
				{
					Console.WriteLine( errorMessage );
				}
			}
		}
	}
}
