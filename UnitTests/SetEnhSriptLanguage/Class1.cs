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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SetDDLSriptLanguage
{
	/// <summary>
	/// Zusammenfassung fï¿½r Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Der Haupteinstiegspunkt fï¿½r die Anwendung.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("usage: SetEnhScriptLanguage enhancer-parameter-file ddl-language");
				return -1;
			}

			if (!File.Exists(args[0]))
			{
				Console.WriteLine("File not found: " + args[0]);
				return -2;
			}

			string ddlLanguage = args[1];
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(args[0]);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Can't read Xml file: " + args[0] + ex.ToString());
				return -3;
			}
			XmlNode node = doc.SelectSingleNode("/Enhancer/Options/SQLScriptLanguage");
			if (node == null)
			{
				Console.WriteLine("Node /Enhancer/Options/SQLScriptLanguage not found");
				return -4;
			}
			node.InnerText = ddlLanguage;

            //node = doc.SelectSingleNode("/Enhancer/Options/Utf8Encoding");
            //if (node == null)
            //{
            //    Console.WriteLine("Node /Enhancer/Options/Utf8Encoding not found");
            //    return -5;
            //}
            //if (ddlLanguage == "Postgres")
            //    node.InnerText = "False";
            //else
            //    node.InnerText = "True";
			doc.Save(args[0]);

#if nix
			StreamReader sr = new StreamReader(args[0], Encoding.UTF8);
			string newName = Path.ChangeExtension(args[0], ".new");
			StreamWriter sw = new StreamWriter(newName, false, Encoding.UTF8);
			try
			{
				string l;
				Regex regex1 = new Regex(@"(\s*)NDOSQLScriptLanguage");
				while ((l = sr.ReadLine()) != null)
				{
					Match match = regex1.Match(l);
					if (match.Success)
					{
						l = match.Groups[1].Value + "NDOSQLScriptLanguage = \"" + ddlLanguage + "\"";
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
#endif
			return 0;
		}
	}
}
