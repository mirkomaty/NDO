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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using CodeGenerator;

namespace TestGenerator
{
	/// <summary>
	/// Summary for Relation.
	/// </summary>
	public class Relation : ITextProvider
	{
		Class parent;
		RelInfo ri;
		string relatedTypeName;
		public const string StandardFieldName = "relField";

		public Relation(Class parent, RelInfo ri, string relatedTypeName)
		{
			this.parent = parent;
			this.ri = ri;
			this.relatedTypeName = relatedTypeName;
		}


		public List<string> Text
		{
			get
			{
                List<string> result = new List<string>();
				StringBuilder sb = new StringBuilder();
				string listLeft = "IEnumerable<" + relatedTypeName + ">";
				string listRight = "List<"+ relatedTypeName + ">";

				sb.Append("[NDORelation");
				if (ri.IsList || ri.IsComposite)
				{
					bool needsComma = false;
					sb.Append('(');
					if (ri.IsComposite)
					{
						if (needsComma)
							sb.Append(", ");
						sb.Append("RelationInfo.Composite");
						needsComma = true;
					}
					sb.Append(')');					
				}

				if (!this.ri.MustHaveTable && this.ri.HasTable)
					sb.Append( ", MappingTable" );

				sb.Append("]");
				
				result.Add(sb.ToString());
				sb.Length = 0;

				if (ri.IsList)
				{
					sb.Append( listRight );
					sb.Append( ' ' );
				}
				else
				{
					sb.Append( relatedTypeName );
					sb.Append( ' ' );
				}

				sb.Append(Relation.StandardFieldName);

				if (ri.IsList)
				{
					sb.Append(" = new ");
					sb.Append(listRight);
					sb.Append("()");
				}
					
				sb.Append(";\n");
				result.Add(sb.ToString());

				return result;
			}
		}

	}
}
