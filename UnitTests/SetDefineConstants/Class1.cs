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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SetDDLSriptLanguage
{
	/// <summary>
	/// Zusammenfassung f�r Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Der Haupteinstiegspunkt f�r die Anwendung.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("usage: SetDefineConstants unittests-proj-file [CONST1;CONST2]");
				return -1;
			}

			if (!File.Exists(args[0]))
			{
				Console.WriteLine("File not found: " + args[0]);
				return -2;
			}
#if !NDO11
            Encoding encoding = Encoding.UTF8;
#else
            Encoding encoding = Encoding.Default;
#endif
            string newConstants;

			if (args.Length < 2)
				newConstants = string.Empty;
			else
				newConstants= args[1];
			StreamReader sr = new StreamReader(args[0], encoding);
			string newName = Path.ChangeExtension(args[0], ".new");
			StreamWriter sw = new StreamWriter(newName, false, encoding);
			try
			{
				string l;
				//                    DefineConstants = "DEBUG;TRACE;ORACLE"
                // <DefineConstants>DEBUG;TRACE;</DefineConstants>
#if !NDO11
                Regex regex1 = new Regex(@"(\s*)<DefineConstants>[^<]+</DefineConstants>");
#else
				Regex regex1 = new Regex(@"(\s*)DefineConstants");
#endif
                while ((l = sr.ReadLine()) != null)
				{
					Match match = regex1.Match(l);
					if (match.Success)
					{
						string constants = MakeConstants(l, newConstants);
#if !NDO11
                        l = match.Groups[1].Value + "<DefineConstants>" + constants + "</DefineConstants>";
#else
						l = match.Groups[1].Value + "DefineConstants = \"" + constants + "\"";
#endif
                    }
					sw.WriteLine(l);
				}
			}
			finally
			{
				sw.Close();
				sr.Close();
				File.Copy(newName, args[0], true);
				File.Delete(newName);
			}
			return 0;
		}

		static string MakeConstants(string oldLine, string newConstants)
		{
#if !NDO11
            Regex regex = new Regex("<DefineConstants>([^<]+)</DefineConstants>");
            Match match = regex.Match(oldLine);
			string oldConstants = match.Groups[1].Value.Trim();
#else
			string oldConstants = oldLine.Substring(oldLine.IndexOf("=") + 1).Replace("\"", string.Empty).Trim();
#endif

            string[] arr = oldConstants.Split(';');
			string result = string.Empty;
			foreach(string s in arr)
			{
				if (s.Trim() != string.Empty)
				{
					if (s == "DEBUG" || s == "TRACE")
						result += s + ";";
				}
			}
			return result + newConstants;
		}
	}
}
