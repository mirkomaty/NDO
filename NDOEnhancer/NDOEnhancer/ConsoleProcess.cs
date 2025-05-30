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
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für ConsoleProcess.
	/// </summary>
	public class ConsoleProcess
	{
		string stdout = string.Empty;
        bool verboseMode;

		public string Stdout
		{
			get { return stdout; }
		}

		string stderr = string.Empty;		
		public string Stderr
		{
			get { return stderr; }
		}

		public ConsoleProcess(bool verboseMode)
		{
            this.verboseMode = verboseMode;
		}

		public int Execute(string exeFileName, string parameters)
		{
			return Execute(exeFileName, parameters, null);
		}

		public int Execute(string exeFileName, string parameters, string workingDirectory)
		{
            if (this.verboseMode)
                Console.WriteLine("Execute: " + "\"" + exeFileName + "\" " + parameters);

			ProcessStartInfo psi = new ProcessStartInfo(exeFileName, parameters);

			psi.CreateNoWindow		   = true;
			psi.UseShellExecute		   = false;
			if (workingDirectory != null)
				psi.WorkingDirectory	   = workingDirectory;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError  = true;

			//var codePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
			//if (codePage == 1)  // Windows Bug, der 65001 in 1 ersetzt
			//	codePage = 437;
			//psi.StandardErrorEncoding = Encoding.GetEncoding( codePage );
			//psi.StandardOutputEncoding = Encoding.GetEncoding( codePage );

			Process proc = Process.Start( psi );

			var tasks = new Task[3];

			tasks[0] = proc.StandardError.ReadToEndAsync();
			tasks[1] = proc.StandardOutput.ReadToEndAsync();
			tasks[2] = proc.WaitForExitAsync();

			Task.WaitAll( tasks );
			return proc.ExitCode;
		}

	}
}
