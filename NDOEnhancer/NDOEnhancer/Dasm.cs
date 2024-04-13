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
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;

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

		void CheckILDasmPath()
		{
			string path;
			if (System.OperatingSystem.IsLinux())
				path = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "runtimes", "linux-x64", "native", "ildasm" );
			else if (System.OperatingSystem.IsWindows())
				path = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "runtimes", "win-x64", "native", "ildasm.exe" );
			else
				throw new Exception( "Can't determine, if NDOEnhancer is running on Windows or Linux." );

			this.ilDasmPath = path;
		}

		public Dasm(MessageAdapter messages, bool verboseMode)
		{
			this.messages = messages;
            this.verboseMode = verboseMode;

			CheckILDasmPath();

		}

		public void DoIt(string dllName, string ilName)
		{
            if (verboseMode)
			    messages.WriteLine("Disassembling: " + dllName + "->" + ilName);

			ProcessStartInfo psi = new ProcessStartInfo( ilDasmPath, 
				" /LINENUM"
#if !NETCOREAPP2_0 && !NET6_0_OR_GREATER
				+ " /NOBAR"
#endif
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
