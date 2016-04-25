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
using System.Data.OracleClient;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;


namespace NDOEnhancer
{
	internal class OracleGenerator : ISqlGenerator
	{
		IProvider provider;
		IMessageAdapter messages;

		public OracleGenerator()
		{
			provider = NDOProviderFactory.Instance[this.ProviderName];
		}

		/// <summary>
		/// Sets the message adapter object, where warnings and error messages are written to.
		/// </summary>
		/// <param name="messages">The message adapter object.</param>
		public void SetMessageAdapter(IMessageAdapter messages)
		{
			this.messages = messages;
		}

	
		public string ConnectToDatabase(string connString)
		{
			return string.Empty;
		}

		public string ProviderName
		{
			get { return "Oracle"; }
		}

		public string DropTable(string tableName)
		{
			if (tableName.Length > 35)  // 30 + 4 * '"' + '.'
				messages.WriteLine("Warning: TableName " + tableName + " is too long.");
			return "DROP TABLE " + tableName + ";";
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
			return t != typeof(DateTime);
		}

		public bool LengthAllowed(string dbType)
		{
			return dbType != "Date";
		}

		public string DbTypeFromType(Type t)
		{
			// The Oracle ADO.NET provider gives us an OracleType.DateTime instead
			// of an OracleType.Date
			if (t == typeof(DateTime))
				return "Date";
			return ((OracleType)provider.GetDbType(t)).ToString();
		}


		public string AutoIncrementColumn(string name, Type dataType, string columnType, string width)
		{
			return string.Empty;
		}

		public string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
		{
			return string.Empty; // no special format for PK Columns
		}


		public bool HasSpecialAutoIncrementColumnFormat
		{
			get { return false; }
		}


		private NDO.Mapping.Class FindClass(string tableName, NDOMapping mappings)
		{
			Class result = null;
			foreach(Class cl in mappings.Classes)
			{
				if (cl.TableName == tableName)
				{
					result = cl;
					break;
				}
			}
			return result;
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
				return "NULL";
			else
				return "NOT NULL";

		}

	}
}
