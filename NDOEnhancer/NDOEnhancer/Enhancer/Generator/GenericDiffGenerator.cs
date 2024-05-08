//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.IO;
using System.Collections.Generic;
using System.Data;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für GenericDiffGenerator.
	/// </summary>
	internal class GenericDiffGenerator : GenericSqlGeneratorBase
	{
		IProvider provider;

		public GenericDiffGenerator(ISqlGenerator concreteGenerator, MessageAdapter messages, NDO.Mapping.NDOMapping mappings) : base(concreteGenerator, messages, mappings)
		{
			provider = EnhancerProviderFactory.Instance[concreteGenerator.ProviderName];
		}

		public void Generate(System.Data.DataSet dsNewSchema, System.Data.DataSet dsOldSchema, System.IO.StreamWriter stream)
		{
			foreach(DataTable dt in dsNewSchema.Tables)
			{
				if (!dsOldSchema.Tables.Contains(dt.TableName))
				{
					stream.Write(CreateTable(dt));
				}
				else
				{
					DataTable dtOld = dsOldSchema.Tables[dt.TableName];
					var addedColumns = new List<DataColumn>();
					var removedColumns = new List<DataColumn>();
					var changedColumns = new List<DataColumn>();
					foreach(DataColumn dc in dt.Columns)
					{
						if (!dtOld.Columns.Contains(dc.ColumnName))
						{
							addedColumns.Add(dc);
						}
						else
						{
							if (dc.DataType != dtOld.Columns[dc.ColumnName].DataType)
								changedColumns.Add(dc);
						}
					}
					foreach(DataColumn dc in dtOld.Columns)
					{
						if (!dt.Columns.Contains(dc.ColumnName))
						{
							removedColumns.Add(dc);
						}
					}
					ChangeTable(dt, addedColumns, removedColumns, changedColumns, stream);
				}

			}
		}

		void ChangeTable(DataTable dt, List<DataColumn> addedColumns, List<DataColumn> removedColums, List<DataColumn> changedColumns, StreamWriter sw)
		{
			if (addedColumns.Count == 0 && removedColums.Count == 0 && changedColumns.Count == 0)
				return;
			string tableName = QualifiedTableName.Get(dt.TableName, this.provider);
			string alterString = "ALTER TABLE " + tableName + ' ';
			//alter table mitarbeiter modify [column] position_x char(255); (mysql)
			//ALTER TABLE EMPLOYEE ALTER EMP_NUM TYPE CHAR(20); (Fb)

			foreach(DataColumn dc in addedColumns)
			{
				sw.Write(alterString);
				sw.Write(concreteGenerator.AddColumn());
				sw.Write(' ');
				sw.Write(base.CreateColumn(dc, GetClassForTable(dt.TableName, this.mappings), this.provider, false));
				sw.WriteLine(";");
			}

			foreach(DataColumn dc in removedColums)
			{
				sw.Write(alterString);
				sw.Write(concreteGenerator.RemoveColumn(provider.GetQuotedName(dc.ColumnName)));
				sw.WriteLine(";");
			}

			foreach(DataColumn dc in changedColumns)
			{
				sw.Write(alterString);
				sw.Write(concreteGenerator.AlterColumnType());
				sw.Write(' ');
				sw.Write(base.CreateColumn(dc, GetClassForTable(dt.TableName, this.mappings), this.provider, false));
				sw.WriteLine(";");
			}

			if (removedColums.Count > 0)
			{
				sw.WriteLine("-- If you need to rename a column use the following syntax:");
				string rename = RenameColumn(provider.GetQuotedName("YourTableName"), 
					provider.GetQuotedName("OldColumnName"), provider.GetQuotedName("NewColumnName"), 
					"NewColumnType [(Precision)] [" + concreteGenerator.NullExpression(true) + " | " + concreteGenerator.NullExpression(false) + "]");
				string[] splittedRename = rename.Split('\n');
				foreach(string s in splittedRename)
					sw.WriteLine("-- " + s);
				messages.WriteLine("Warning: Dropping one or more columns. See the comment in the in ndodiff.sql file how to rename columns, if required.");
			}

		}

		string RenameColumn(string tableName, string oldColumn, string newColumn, string typeString)
		{
			string s = concreteGenerator.RenameColumn(tableName, oldColumn, newColumn, typeString);
			
			if (s != string.Empty)
			return s;
		
			string alterString = "ALTER TABLE " + tableName + ' ';

			StringBuilder sb = new StringBuilder(alterString);
			sb.Append(concreteGenerator.AddColumn());
			sb.Append(' ');
			sb.Append(newColumn);
			sb.Append(' ');
			sb.Append(typeString);
			sb.Append(";\n");

			sb.Append("UPDATE ");
			sb.Append(tableName);
			sb.Append(" SET ");			
			sb.Append(newColumn);
			sb.Append(" = ");
			sb.Append(oldColumn);
			sb.Append(";\n");

			sb.Append(alterString);
			sb.Append(concreteGenerator.RemoveColumn(oldColumn));
			sb.Append(';');
			return sb.ToString();
		}
	}
}
