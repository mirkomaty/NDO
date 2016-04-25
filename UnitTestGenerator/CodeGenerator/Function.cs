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
	/// Zusammenfassung fï¿½r Function.
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
