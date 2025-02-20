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
			string ndoRootPath = AppDomain.CurrentDomain.BaseDirectory;
			string gitDir;

			do
			{
				ndoRootPath = Path.GetFullPath( Path.Combine( ndoRootPath, ".." ) );
				gitDir = Path.Combine( ndoRootPath, ".git" );
			} while (!Directory.Exists( gitDir ));

				string head = null;
			char[] buf = new char[7];
			using (StreamReader sr = new StreamReader( Path.Combine( gitDir, "HEAD" ) ))
			{
				sr.Read( buf, 0, 5 );
				head = sr.ReadToEnd().Trim();
			}

			var refHeadPath = Path.Combine( gitDir, head.Replace( "/", "\\" ) );
			if (!File.Exists( refHeadPath ))
				throw new Exception( "Ref head doesn't exist: " + refHeadPath );

			using (StreamReader sr = new StreamReader( refHeadPath ))
			{
				sr.Read( buf, 0, 7 );  // first 7 chars of the head				
			}

			string rev = "r. " + new string( buf );
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
