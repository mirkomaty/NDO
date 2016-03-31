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
	/// Zusammenfassung f�r Function.
	/// </summary>
	public class Function : ITextProvider
	{
		string resultType;
		string name;
		string[] paramTypes;
		string[] paramNames;
		bool isStatic = false;
		public bool IsStatic
		{
			get { return isStatic; }
			set { isStatic = value; }
		}
		List<string> attributes = new List<string>();
		public List<string> Attributes
		{
			get { return attributes; }
		}
		bool isVirtual;
		public bool IsVirtual
		{
			get { return isVirtual; }
			set { isVirtual = value; }
		}
		string accessModifier = null;
		public string AccessModifier
		{
			get { return accessModifier; }
			set { accessModifier = value; }
		}
		List<string> statements = new List<string>();
		public List<string> Statements
		{
			get { return statements; }
			set { statements = value; }
		}

		public Function(string resultType, string name)
		{
			this.resultType = resultType;
			this.name = name;
			isVirtual = false;
			paramTypes = null;
			paramNames = null;
		}

		public Function(string resultType, string name, string[] paramTypes, string[] paramNames)
		{
			this.resultType = resultType;
			this.name = name;
			this.paramTypes = paramTypes;
			this.paramNames = paramNames;
			isVirtual = false;
		}

		public List<string> Text
		{
			get
			{
				List<string> result = new List<string>();
				StringBuilder sb = new StringBuilder();

				if (attributes.Count > 0)
				{
					sb.Append('[');
					for (int i = 0; i < attributes.Count; i++)
					{
						sb.Append(attributes[i]);
						if (i < attributes.Count - 1)
							sb.Append(", ");
					}
					sb.Append("]");
					result.Add(sb.ToString());
					sb.Length = 0;
				}

				if (accessModifier != null)
				{
					sb.Append(accessModifier);
					sb.Append(' ');
				}
				if (isVirtual)
					sb.Append("virtual ");
				if (isStatic)
					sb.Append("static ");
				sb.Append(resultType);
				sb.Append(' ');
				sb.Append(name);
				sb.Append('(');
				if (paramTypes != null)
				{
					for (int i = 0; i < paramTypes.Length; i++)
					{
						sb.Append(paramTypes[i]);
						sb.Append(' ');
						sb.Append(paramNames[i]);
						if (i < paramTypes.Length - 1)
							sb.Append(", ");
					}					
				}
				sb.Append(')');
				result.Add(sb.ToString());
				sb.Length = 0;
				result.Add("{");
				foreach(string s in this.statements)
					result.Add("\t" + s);
				result.Add("}");
				return result;
			}
		}
	}
}
