//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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
