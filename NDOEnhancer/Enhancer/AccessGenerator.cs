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
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	internal class AccessGenerator : ISqlGenerator
	{
		IMessageAdapter messages;

		/// <summary>
		/// Sets the message adapter object, where warnings and error messages are written to.
		/// </summary>
		/// <param name="messages">The message adapter object.</param>
		public void SetMessageAdapter(IMessageAdapter messages)
		{
			this.messages = messages;
		}

		public AccessGenerator()
		{
		}



		public string ProviderName
		{
			get { return "Access"; }
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
			get { return PrimaryConstraintPlacement.InColumn; }
		}

		public bool LengthAllowed(Type t)
		{
			return t == typeof(string);
		}

		public bool LengthAllowed(string dbType)
		{
			string ltype = dbType.ToLower();
			return ltype == "text" || ltype == "decimal";
		}

		public string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + " COUNTER CONSTRAINT PrimaryKey PRIMARY KEY";
		}

		public string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + ' ' + columnType + " CONSTRAINT PrimaryKey PRIMARY KEY";
		}

		public bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
		}


		public string DbTypeFromType(Type t)
		{
			return GetDbTypeFromCsType(t);
		}


		public string CreateConstraint(DataColumn[] primaryKeyColumns, string constraintName, string tableName)
		{
			return string.Empty;
		}

#if nix
		private void CreateTable(DataTable dt, System.IO.StreamWriter stream, NDO.Mapping.NDOMapping mappings)
		{
			/* 

DROP TABLE DataContainer;
CREATE TABLE [Table6] (
Id COUNTER CONSTRAINT PrimaryKey PRIMARY KEY, 
...
); 
			 */

			IProvider provider = new NDOOleDbProvider();
			string tableName = QualifiedTableName.Get(dt.TableName, provider);
			Class cl = FindClass(dt.TableName, mappings);

			bool selfGeneratedOid = false;
			if (cl != null)
				selfGeneratedOid = cl.Oid.FieldName != null;

			stream.WriteLine("DROP TABLE " + tableName + ";");
			stream.WriteLine("CREATE TABLE " + tableName + "(");
			int vorletzterIndex = dt.Columns.Count - 1;
			DataColumn[] primaryKeyColumns = dt.PrimaryKey;
			bool hasPrimaryKeyColumns = primaryKeyColumns.Length > 0;
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				System.Data.DataColumn dc = dt.Columns[i];
				bool isPrimary = false;
				if (!selfGeneratedOid)
					isPrimary = IsPrimary(primaryKeyColumns, dc);
				CreateColumn(dc, stream, cl, provider, IsPrimary(primaryKeyColumns, dc));
				if (i < vorletzterIndex)
					stream.WriteLine(",");				
			}
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
			/* 

			DROP TABLE DataContainer;
			CREATE TABLE Table6 (
			Id COUNTER CONSTRAINT PrimaryKey PRIMARY KEY, 
			MyText TEXT (255), 
			intval integer, 
			byteval byte, 
			longval long, 
			doubleval double, 
			singleval single, 
			guidval guid, 
			datetimeval datetime, 
			decval decimal, 
			rawval oleobject, 
			boolval yesno) 
			 */

			string name = provider.GetQuotedName(dc.ColumnName);
			string columnType = null;
			string width = null;
			//			string precision = null;
			bool autoIncrement = false;
			bool omitWidth = false;

			if (cl != null)
			{
				Field field = FindField(dc.ColumnName, cl);

				if (null != field)
				{
					if (null != field.ColumnType)
						columnType = field.ColumnType;
					if (0 != field.ColumnLength)
					{
						omitWidth = (field.ColumnLength < 0);
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
				else if (isPrimary && dc.DataType == typeof(int))
					autoIncrement = true;
			}
			if (dc.DataType == typeof(System.Guid))
			{
				columnType = "Guid";
//				width = "36";
			}
			if (null == columnType)
			{
				columnType = GetDbTypeFromCsType(dc.DataType);
			}

			if (null == width)
			{				
				int length = provider.GetDefaultLength(dc.DataType);
				if (length != 0)
				{
					width = length.ToString();
				}
			}
			if (width != null && !omitWidth)
				width = "(" + width + ")";
			else
				width = string.Empty;
			stream.Write(name);
			if (!autoIncrement)
			{
				stream.Write(" " + columnType + width);
				if (isPrimary)
					stream.Write(" NOT NULL");
			}
			else
				stream.Write(" " + "COUNTER CONSTRAINT PrimaryKey PRIMARY KEY");
		}
#endif
		public string GetDbTypeFromCsType(System.Type t) 
		{
			if ( t == typeof(bool) )
				return "Yesno";
			else if ( t == typeof(byte) )
				return "Byte";
			else if ( t == typeof(sbyte) )
				return "Byte";
			else if ( t == typeof(char) )
				return "Integer";
			else if ( t == typeof(short))
				return "Short";
			else if ( t == typeof(ushort))
				return "Short";
			else if ( t == typeof(int))
				return "Long";
			else if ( t == typeof(uint))
				return "Long";
			else if ( t == typeof(long))
				return "Decimal";
			else if ( t == typeof(ulong))
				return "Decimal";
			else if ( t == typeof(float))
				return "Single";
			else if ( t == typeof(double))
				return "Double";
			else if ( t == typeof(string))
				return "Text";
			else if ( t == typeof(byte[]))
				return "Oleobject";
			else if ( t == typeof(decimal))
				return "Currency";
			else if ( t == typeof(DateTime))
				return "datetime";
			else if ( t == typeof(Guid))
				return "Guid";
			else
				throw new NDOException("NDOOleDbProvider.GetDbType: Typ " + t.Name + " kann nicht in OleDbType konvertiert werden");
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
