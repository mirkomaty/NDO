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
		public const string String = ""{0} " + DateTime.Now.ToShortDateString() + " (V. {1} " + rev + @")"";
	}
}
");
		}
	}
}
