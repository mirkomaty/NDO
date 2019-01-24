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
using System.Collections;
using System.Data;
using System.IO;
using NDO;
using NDOInterfaces;

namespace FirebirdProvider
{

	/// <summary>
	/// This is the part of the provider, which generates DDL. 
	/// The DDL code can be used to construct databases.
	/// For more information see the ISqlGenerator interface.
	/// </summary>
	public class FirebirdGenerator : AbstractSQLGenerator
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
			// We unquote the name first, 
			// then we cut the name to a maximum of 30 characters.
			// After shortening we have to requote the name.
			string indexName2 = indexName.Substring(1, indexName.Length - 2);
			uint hash = (uint) indexName2.GetHashCode();
			if (indexName2.Length > 30)
				indexName2 = indexName2.Substring(0, 22) + hash.ToString("X");

			return base.CreateIndex (Provider.GetQuotedName(indexName2), tableName, indexColums);
		}

		// True, if NDO should place the length of the field in the ddl. 
		public override bool LengthAllowed(Type t)
		{
			return t == typeof(Decimal) || t == typeof(Guid) || t == typeof(decimal);
		}

		// True, if NDO should place the length of the field in the ddl. 
		public override bool LengthAllowed(string dbType)
		{
			return string.Compare(dbType, "varchar", true) == 0
				|| string.Compare(dbType, "char", true) == 0
				|| string.Compare(dbType, "decimal", true) == 0;
		}

		// Standard conversion of types to DbTypes.
		// ColumnType entries in the mapping file will override this entries.
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
