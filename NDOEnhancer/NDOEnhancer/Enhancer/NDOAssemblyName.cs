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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using NDO;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für NDOAssemblyName.
	/// </summary>
	internal class NDOAssemblyName
	{
		const string Null = "null";
		string name = null;
        string fullName;

        public string FullName
        {
            get 
            {
                if (fullName == null)
                    return string.Empty;
                return fullName; 
            }
        }

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		CultureInfo cultureInfo = null;
		public CultureInfo CultureInfo
		{
			get { return cultureInfo; }
			set { cultureInfo = value; }
		}
		string publicKeyToken = null;
		public string PublicKeyToken
		{
			get { return publicKeyToken; }
			set { publicKeyToken = value; }
		}

		public string PublicKeyTokenBytes
		{
			get
			{
				if (publicKeyToken == Null)
					return Null;
				StringBuilder result = new StringBuilder(publicKeyToken.Length * 2);
				for (int i = 0; i < publicKeyToken.Length; i += 2)
				{
					result.Append(publicKeyToken.Substring(i, 2));
					result.Append(" ");
				}
				return result.ToString();
			}
		}
		string version = null;
		public string Version
		{
			get { return version; }
			set { version = value; }
		}

		public System.Version AssemblyVersion
		{
			get 
			{
				return new System.Version(version);
			}
			set
			{
				this.version = value.ToString();
			}
		}
 
		public NDOAssemblyName(string fullName)
		{
            this.fullName = fullName;
			string[] arr = fullName.Split(',');
			name = arr[0];  // always the first element
			for(int i = 1; i < arr.Length; i++)
			{
				string s = arr[i].Trim();
				Regex regex = new Regex(@"Culture\s*=\s*(.+)");
				Match match = regex.Match(s);
				if (match.Success)
				{
					string cultureName = match.Groups[1].Value;
					if (cultureName == "\"\"" || cultureName == "neutral")
						cultureInfo = new CultureInfo(string.Empty);
					else
						cultureInfo = new CultureInfo(match.Groups[1].Value);
					continue;
				}
				regex = new Regex(@"PublicKeyToken\s*=\s*(.+)");
				match = regex.Match(s);
				if (match.Success)
				{
					this.publicKeyToken = match.Groups[1].Value;
					continue;
				}
				regex = new Regex(@"Version\s*=\s*(.+)");
				match = regex.Match(s);
				if (match.Success)
				{
					this.version = match.Groups[1].Value;
					continue;
				}
				throw new Exception("Unrecognized substring in assembly name: " + s);
			}
			if (cultureInfo == null)
				cultureInfo = new CultureInfo(string.Empty);
			if (publicKeyToken == null)
				publicKeyToken = Null;
		}
	}
}
