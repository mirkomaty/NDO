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
using System.Text;
using System.Collections.Generic;

namespace CodeGenerator
{
	/// <summary>
	/// Zusammenfassung fï¿½r Property.
	/// </summary>
	public class Property : ITextProvider
	{
		string typeName;
		string name;
		bool hasSetter; 
		bool hasGetter;
		string varName;

		public Property(string typeName, string varName, bool hasSetter, bool hasGetter)
		{
			this.typeName = typeName;
			this.varName = varName;
			this.hasGetter = hasGetter;
			this.hasSetter = hasSetter;
			if (char.IsUpper(varName[0]))
				throw new ArgumentException("varName shouldn't start with upper case letter", "varName");
			this.name = varName.Substring(0, 1).ToUpper() + varName.Substring(1);
		}

		public Property(string typeName, string name, string varName, bool hasSetter, bool hasGetter)
		{
			this.typeName = typeName;
			this.name = name;
			this.hasGetter = hasGetter;
			this.hasSetter = hasSetter;
			this.varName = varName;
		}

		public List<string> Text
		{
			get
			{
				List<string> result = new List<string>();
				StringBuilder sb = new StringBuilder();
				sb.Append("public ");
				sb.Append(typeName);
				sb.Append(' ');
				sb.Append(name);
				result.Add(sb.ToString());
				result.Add("{");
				if (hasGetter)
				{
					sb.Length = 0;
					sb.Append("\tget { return ");
					sb.Append(varName);
					sb.Append("; }");
					result.Add(sb.ToString());
				}
				if (hasSetter)
				{
					sb.Length = 0;
					sb.Append("\tset { ");
					sb.Append(varName);
					sb.Append(" = value; }");
					result.Add(sb.ToString());
				}
				result.Add("}");
				return result;
			}
		}
	}		
}
