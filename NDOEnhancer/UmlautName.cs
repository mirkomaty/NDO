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

namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for UmlautName.
	/// </summary>
	internal class QuotedName 
	{
        public static string Convert(string name)
        {
            string result = string.Empty;
            string[] strarr = name.Split('.', '/');
            char[] separators = new char[strarr.Length - 1];
            int j = 0;
            foreach (char c in name)
                if (c == '.' || c == '/')
                    separators[j++] = c;
            j = 0;
            for (int i = 0; i < strarr.Length; i++)
            {
                string temp = strarr[i];
                if (!temp.StartsWith("'"))
                {
                    Regex re = new Regex(@"[A-Za-z_$@`?][A-Za-z0-9_$@`?]*", RegexOptions.Singleline);
                    Match match = re.Match(temp);
                    if (!match.Success || !(match.Value == temp))
                        temp = "'" + temp + "'";
                }
                result += temp;
                if (i < strarr.Length - 1)
                    result += separators[j++];
            }

            return result;
        }

		public static string ConvertTypename(string inname)
		{
			int p = inname.IndexOf("]");
			string name = inname.Substring(p + 1);
			string prefix = string.Empty;
			if (p > -1)
				prefix = inname.Substring(0, p + 1);

			return prefix + Convert(name);
		}

	}
}
