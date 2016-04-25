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
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für FirebirdGenerator.
	/// </summary>
	internal class FirebirdGenerator : ISqlGenerator
	{
		IMessageAdapter messages;
		IProvider provider;

		public FirebirdGenerator()
		{
			this.provider = NDOProviderFactory.Instance[this.ProviderName];
		}

		/// <summary>
		/// Sets the message adapter object, where warnings and error messages are written to.
		/// </summary>
		/// <param name="messages">The message adapter object.</param>
		public void SetMessageAdapter(IMessageAdapter messages)
		{
			this.messages = messages;
		}

		#region ISqlGenerator Member

		public string ProviderName
		{
			get
			{
				return "Firebird";
			}
		}

		public string DropTable(string tableName)
		{
			return "DROP TABLE " + tableName + ";";
		}

		public string ConnectToDatabase(string connectionString)
		{
			return string.Empty;
		}


		public string BeginnTable(string tableName)
		{
			return "CREATE TABLE " + tableName + "(";			
		}


		public string EndTable(string tableName)
		{
			return ");";
		}

		public PrimaryConstraintPlacement PrimaryConstraintPlacement
		{
			get { return PrimaryConstraintPlacement.InTable; }
		}

		public bool LengthAllowed(Type t)
		{
			return t == typeof(Decimal) || t == typeof(Guid) || t == typeof(decimal);
		}

		public bool LengthAllowed(string dbType)
		{
			string dbtl = dbType.ToLower();
			return dbtl == "varchar" || dbtl == "char" || dbtl == "decimal";
		}

		public string DbTypeFromType(Type t)
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


		public string AutoIncrementColumn(string name, Type dataType, string columnType, string width)
		{
			return string.Empty;
		}

		public string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
		{
			//"ID" INTEGER NOT NULL CONSTRAINT "PK_DataContainer" PRIMARY KEY ,
			return columnName + ' ' + columnType + " NOT NULL CONSTRAINT PRIMARYKEY PRIMARY KEY";
		}


		public bool HasSpecialAutoIncrementColumnFormat
		{
			get { return false; }
		}


		public string CreateConstraint(DataColumn[] primaryKeyColumns, string constraintName, string tableName)
		{
			StringBuilder sb = new StringBuilder("CONSTRAINT ");
			sb.Append(constraintName);
			sb.Append(" PRIMARY KEY (");
			int lastIndex = primaryKeyColumns.Length - 1;
			for (int i = 0; i < primaryKeyColumns.Length; i++)
			{
				DataColumn dc = primaryKeyColumns[i];
				sb.Append(provider.GetQuotedName(dc.ColumnName));
				if (i < lastIndex)
					sb.Append(",");
			}
			sb.Append(")");
			return sb.ToString();
		}

		public string NullExpression(bool allowNull)
		{
			if (allowNull)
				return "DEFAULT NULL";
			else
				return "NOT NULL";

		}

		#endregion
	}
}
