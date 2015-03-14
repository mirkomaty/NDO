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
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace NDOAddIn
{
	/// <summary>
	/// Zusammenfassung für ConsoleProcess.
	/// </summary>
	public class ConsoleProcess
	{
		StreamReader outReader;
		StreamReader errReader;
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
            psi.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            psi.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

			System.Diagnostics.Process proc = System.Diagnostics.Process.Start( psi );

			this.errReader = proc.StandardError;
			this.outReader = proc.StandardOutput;

			Thread threadOut = new Thread(new ThreadStart(ReadStdOut));
			Thread threadErr = new Thread(new ThreadStart(ReadStdErr));
			threadOut.Start();
			threadErr.Start();
			proc.WaitForExit();
			while (threadOut.ThreadState != System.Threading.ThreadState.Stopped
				|| threadErr.ThreadState != System.Threading.ThreadState.Stopped)
				Thread.Sleep(1);
			return proc.ExitCode;
		}

		void ReadStdOut()
		{
			stdout = outReader.ReadToEnd();
		}

		void ReadStdErr()
		{
			stderr = errReader.ReadToEnd();
		}


	}
}
