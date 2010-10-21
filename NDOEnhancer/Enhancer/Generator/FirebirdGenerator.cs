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


#if nix
using System;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für FirebirdGenerator.
	/// </summary>
	internal class FirebirdGenerator : AbstractSQLGenerator
	{
		public FirebirdGenerator()
		{
		}


		#region ISqlGenerator Member

		public override string ProviderName
		{
			get
			{
				return "Firebird";
			}
		}

		public override string CreateIndex(string indexName, string tableName, DataColumn[] indexColums)
		{
			string indexName2 = indexName.Substring(1, indexName.Length - 2);
			uint hash = (uint) indexName2.GetHashCode();
			if (indexName2.Length > 30)
				indexName2 = indexName2.Substring(0, 22) + hash.ToString("X");

			return base.CreateIndex (Provider.GetQuotedName(indexName2), tableName, indexColums);
		}


		public override bool LengthAllowed(Type t)
		{
			return t == typeof(Decimal) || t == typeof(Guid) || t == typeof(decimal);
		}

		public override bool LengthAllowed(string dbType)
		{
			return string.Compare(dbType, "varchar", true) == 0
				|| string.Compare(dbType, "char", true) == 0
				|| string.Compare(dbType, "decimal", true) == 0;
		}

		public override string DbTypeFromType(Type t)
		{
			if ( t == typeof(bool) ||  t == typeof(byte) || t == typeof(sbyte) 
				|| t == typeof(char) || t == typeof(short) || t == typeof(ushort) )
				return "SMALLINT";
			else if ( t == typeof(int)|| t == typeof(uint) || t.IsSubclassOf(typeof(System.Enum)))
				return "INTEGER";
			else if ( t == typeof(long)|| t == typeof(ulong))
				return "BIGINT";
			else if ( t == typeof(System.Guid))
				return "CHAR";
			else if ( t == typeof(float))
				return "FLOAT";
			else if ( t == typeof(double))
				return "DOUBLE PRECISION";
			else if ( t == typeof(string))
				return "VARCHAR";
			else if ( t == typeof(byte[]))
				return "BLOB";
			else if ( t == typeof(decimal))
				return "DECIMAL";
			else if ( t == typeof(System.DateTime))
				return "DATE";
			else
				return "VARCHAR";
		}



		public override string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
		{
			//"ID" INTEGER NOT NULL CONSTRAINT "PK_DataContainer" PRIMARY KEY ,
			return columnName + ' ' + columnType + " NOT NULL CONSTRAINT PRIMARYKEY PRIMARY KEY";
		}



		public override string NullExpression(bool allowNull)
		{
			if (allowNull)
				return "DEFAULT NULL";
			else
				return "NOT NULL";

		}

		#endregion
	}
}

#endif