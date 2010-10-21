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
