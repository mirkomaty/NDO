//
// Copyright (C) 2002-2015 Mirko Matytschak 
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
using System.Text;
using NDOInterfaces;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// Summary description for QualifiedTableName.
	/// </summary>
	internal class QualifiedColumnName
	{
		public static string Get(Column column)
		{
			Class cls = null;
			MappingNode node = column.Parent;
			while (!(node is Class))
			{
				if (node.NodeParent == null)
					break;
				node = node.NodeParent;
			}
			cls = (Class)node;
			IProvider provider = cls.Provider;
			StringBuilder sb = new StringBuilder( QualifiedTableName.Get( cls ) );
			sb.Append( '.' );
			sb.Append( provider.GetQuotedName( column.Name ) );
			return sb.ToString();
		}

		public static string Get(Class parentClass, Column column)
		{
			IProvider provider = parentClass.Provider;
			StringBuilder sb = new StringBuilder( QualifiedTableName.Get( parentClass ) );
			sb.Append( '.' );
			sb.Append( provider.GetQuotedName( column.Name ) );
			return sb.ToString();
		}

		public static string Get(MappingTable mappingTable, Column column)
		{
			IProvider provider = mappingTable.Parent.Parent.Provider;
			StringBuilder sb = new StringBuilder( provider.GetQuotedName( mappingTable.TableName ) );
			sb.Append( '.' );
			sb.Append( provider.GetQuotedName( column.Name ) );
			return sb.ToString();
		}

		public static string Get(MappingTable mappingTable, string columnName)
		{
			IProvider provider = mappingTable.Parent.Parent.Provider;
			StringBuilder sb = new StringBuilder( provider.GetQuotedName( mappingTable.TableName ) );
			sb.Append( '.' );
			sb.Append( provider.GetQuotedName( columnName ) );
			return sb.ToString();
		}

		public static string Get(Class cls, string columnName)
		{
			IProvider provider = cls.Provider;
			StringBuilder sb = new StringBuilder( provider.GetQuotedName( cls.TableName ) );
			sb.Append( '.' );
			sb.Append( provider.GetQuotedName( columnName ) );
			return sb.ToString();
		}
	}
}
