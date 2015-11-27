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
using System.IO;
using System.Globalization;
using System.Threading;

namespace MakeEnhancerDate
{
	/// <summary>
	/// Zusammenfassung für Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			ConsoleProcess cp = new ConsoleProcess(false);
			string path = AppDomain.CurrentDomain.BaseDirectory;
			path = Path.GetFullPath(Path.Combine(path, @"..\..\.."));
			//Console.Error.WriteLine("Path: " + path);
			cp.Execute(args[1], "log -1", path);
			string response = cp.Stdout;
			Regex regex = new Regex( @"commit ([a-f0-9]{7})" );
			Match match = regex.Match(response);
			string rev = string.Empty;
			if (match.Success)
				rev = "r. " + match.Groups[1].Value;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Console.Write(@"namespace NDOEnhancer
{
	public class EnhDate
	{
		public const string String = ""NDO: " + DateTime.Now.ToShortDateString() + " (" + args[0] + " " + rev + @")"";
	}
}
");
		}
	}
}
