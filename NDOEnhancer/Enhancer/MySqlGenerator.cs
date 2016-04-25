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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	internal class MySqlGenerator : ISqlGenerator
	{
		IProvider provider;
		IMessageAdapter messages;

		public MySqlGenerator()
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


		public string ProviderName
		{
			get { return "MySql"; }
		}

		public string DropTable(string tableName)
		{
			return "DROP TABLE IF EXISTS " + tableName + ";";
		}

		public string ConnectToDatabase(string connectionString)
		{
			Regex regex = new Regex(@"Database\s*=\s*([^;]+)");
			Match match = regex.Match(connectionString);
			if (match.Success)
			{
				return("USE " + match.Groups[1] + ";");
			}
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
			return t == typeof(string) || t == typeof(decimal);
		}

		public bool LengthAllowed(string dbType)
		{
			string temp = dbType.ToLower();
			return (temp == "varchar" || temp == "decimal");
		}

		public string DbTypeFromType(Type t)
		{
			if (t == typeof(bool) || t == typeof(Byte))
				return "Tinyint";
			else if (t == typeof(DateTime))
				return "Datetime";
			else if (t == typeof(decimal))
				return "Decimal";
			else if (t == typeof(double))
				return "Double";
			else if (t.IsEnum)
				return "Int";
			else if (t == typeof(float))
				return "Float";
			else if (t == typeof(Guid))
				return "Varchar";
			else if (t == typeof(Int16) || t == typeof(UInt16))
				return "Smallint";
			else if (t == typeof(Int32) || t == typeof(UInt32))
				return "Int";
			else if (t == typeof(Int64) || t == typeof(UInt64))
				return "BigInt";
			else if (t == typeof(string))
				return "Varchar";
			throw new NDOException("Can't resolve type " + t.FullName + " as storable.");
/*
 * Der Provider wurde bereits auf MySql.Data.dll umgeschrieben,
 * als der Generator geschrieben wurde. Man konnte nicht mehr
 * die richtigen ByteFX-Typen zuordnen.
			object dbt = provider.GetDbType(t);
			Type dbtType = dbt.GetType();
			string fullName = dbtType.FullName;
			string nameSpace = fullName.Substring(0, fullName.LastIndexOf("."));
			object par = dbtType.Assembly.CreateInstance(nameSpace + "." + "MySqlParameter", false, BindingFlags.CreateInstance, null, new object[]{"@test", dbt}, null, null);
			FieldInfo fi = par.GetType().GetField("dbType", BindingFlags.NonPublic | BindingFlags.Instance);
			string parType = fi.GetValue(par).ToString();	

			return parType;
			*/
		}
		public string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
		{
			return string.Empty; // no special format for PK Columns
		}

		public string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + " Int AUTO_INCREMENT";
		}

		public bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
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

#if nix
//---------


		private bool IsPrimary(DataColumn[] primaryColumns, DataColumn dc)
		{
			string name = dc.ColumnName;
			foreach (DataColumn dc2 in primaryColumns)
				if (dc2.ColumnName == name)
					return true;
			return false;
		}

		private void CreateTable(DataTable dt, System.IO.StreamWriter stream, NDO.Mapping.NDOMapping mappings)
		{

			IProvider provider = new MySqlProvider();
			string tableName = QualifiedTableName.Get(dt.TableName, provider);
			Class cl = FindClass(dt.TableName, mappings);
			//bool selfGeneratedOid = cl.Oid.FieldName != null;
			if (cl != null)
			{
				string conn = mappings.FindConnection(cl).Name;
				Regex regex = new Regex(@"Database\s*=\s*([^;]+)");
				Match match = regex.Match(conn);
				if (match.Success)
				{
					stream.WriteLine("USE " + match.Groups[1] + ";");
				}
			}
			else
			{
				stream.WriteLine("/* TODO: Insert correct USE statement */");
			}
			stream.WriteLine("DROP TABLE IF EXISTS " + tableName + ";");
			stream.WriteLine("CREATE TABLE " + tableName + "(");
			int vorletzterIndex = dt.Columns.Count - 1;
			DataColumn[] primaryKeyColumns = dt.PrimaryKey;
			bool hasPrimaryKeyColumns = primaryKeyColumns.Length > 0;
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				System.Data.DataColumn dc = dt.Columns[i];
//				bool isPrimary = false;
//				if (!selfGeneratedOid)
//					isPrimary = IsPrimary(primaryKeyColumns, dc);
				CreateColumn(dc, stream, cl, provider, IsPrimary(primaryKeyColumns, dc));
				if (i < vorletzterIndex || hasPrimaryKeyColumns)
					stream.WriteLine(",");				
			}
			if (!hasPrimaryKeyColumns)
				stream.WriteLine();
			else
				CreateConstraint(primaryKeyColumns, stream, dt, provider);
			stream.WriteLine(");");
			stream.WriteLine();
		}

		private Field FindField (string columnName, Class cl)
		{
			Field result = null;
			foreach (Field field in cl.Fields)
			{
				if (field.ColumnName == columnName)
				{
					result = field;
					break;
				}
			}
			return result;
		}


		private void CreateColumn(DataColumn dc, System.IO.StreamWriter stream, Class cl, IProvider provider, bool isPrimary)
		{
			//col_name type [NOT NULL | NULL] [DEFAULT default_value] [AUTO_INCREMENT]
			//            [[PRIMARY] KEY] [COMMENT 'string'] [reference_definition]

			string name = provider.GetQuotedName(dc.ColumnName);
			string columnType = null;
			string width = null;
//			string precision = null;
			bool autoIncrement = false;
			

			if (cl != null)
			{
				Field field = FindField(dc.ColumnName, cl);

				if (null != field)
				{
					if (null != field.ColumnType)
						columnType = field.ColumnType;
					if (0 != field.ColumnLength)
					{
						width = field.ColumnLength.ToString();
						//TODO: Precision sollte festgelegt werden können. Mit einem Int gehts natürlich nicht.
//						int p = width.IndexOf(',');
//						if (p > -1)
//						{
//							precision = width.Substring(p + 1);
//							width = width.Substring(0, p);
//						}
					}
				}
//				else if (cl.TimeStampColumn == dc.ColumnName)
//					width = "16";
				else if (isPrimary && dc.DataType == typeof(int))
					autoIncrement = true;
			}
			if (dc.DataType == typeof(System.Guid))
			{
				columnType = "Varchar";
				width = "36";
			}
			if (null == columnType)
			{
				columnType = ((MySqlDbType)provider.GetDbType(dc.DataType)).ToString();
				//columnType = GetDbTypeFromCsType(dc.DataType.FullName);
			}

			if (null == width)
			{
				
				int length = provider.GetDefaultLength(dc.DataType);
				if (length != 0)
				{
					width = length.ToString();
				}
			}
			if (width != null)
				width = "(" + width + ")";
			else
				width = string.Empty;
			stream.Write(name + " " + columnType + width);
			if (autoIncrement)
				stream.Write(" " + "AUTO_INCREMENT");
			else if (isPrimary)
				stream.Write(" NOT NULL");
			else
				stream.Write(" NULL");
		}
#endif

	}

}
