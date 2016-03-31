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
using System.Collections.Generic;

namespace CodeGenerator
{
	/// <summary>
	/// Zusammenfassung f�r Property.
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
