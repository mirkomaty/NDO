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
using System.Data;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für GenericSqlGenerator.
	/// </summary>
	internal class GenericSqlGenerator
	{
		ISqlGenerator concreteGenerator;
		MessageAdapter messages;

		public GenericSqlGenerator(ISqlGenerator concreteGenerator, MessageAdapter messages)
		{
			this.concreteGenerator = concreteGenerator;
			this.messages = messages;
		}

		public void Generate(System.Data.DataSet dsSchema, System.IO.StreamWriter stream, NDO.Mapping.NDOMapping mappings)
		{			
			foreach (DataTable dt in dsSchema.Tables)
				CreateTable(dt, stream, mappings);
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

		private void CreateTable(DataTable dt, System.IO.StreamWriter stream, NDO.Mapping.NDOMapping mappings)
		{
			IProvider provider = NDOProviderFactory.Instance[concreteGenerator.ProviderName];
			if (provider == null)
				throw new NDOException("Can't find NDO provider '" + concreteGenerator.ProviderName + "'.");

			string tableName = QualifiedTableName.Get(dt.TableName, provider);

			Class cl = FindClass(dt.TableName, mappings);
			
			if (cl != null)
			{
				Connection conn = mappings.FindConnection(cl);
				if (conn != null)
					concreteGenerator.ConnectToDatabase(conn.Name);
			}

			stream.WriteLine(concreteGenerator.DropTable(tableName));

			stream.WriteLine(concreteGenerator.BeginnTable(tableName));

			int vorletzterIndex = dt.Columns.Count - 1;
			DataColumn[] primaryKeyColumns = dt.PrimaryKey;
			bool hasPrimaryKeyColumns = primaryKeyColumns.Length > 0;

			for (int i = 0; i < dt.Columns.Count; i++)
			{
				System.Data.DataColumn dc = dt.Columns[i];

				bool isPrimary = false;
				foreach (DataColumn pkc in primaryKeyColumns)
				{
					if (pkc.ColumnName == dc.ColumnName)
					{
						isPrimary = true;
						break;
					}
				}

				CreateColumn(dc, stream, cl, provider, isPrimary);
				if (i < vorletzterIndex 
					|| (concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InTable && hasPrimaryKeyColumns))
						stream.WriteLine(",");
			}

			if (hasPrimaryKeyColumns && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InTable)
			{
				CreateConstraint(primaryKeyColumns, stream, dt, provider);
			}
			else
			{
				stream.WriteLine();
			}
			stream.WriteLine(concreteGenerator.EndTable(tableName));

			//			CreateIndex(primaryKeyColumns, stream, dt, provider);

			if (hasPrimaryKeyColumns && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.AfterTable)
			{
				CreateConstraint(primaryKeyColumns, stream, dt, provider);
			}

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
			string name = provider.GetQuotedName(dc.ColumnName);
			string columnType = null;
			string width = null;
			string precision = null;
			bool autoIncrement = false;

			if (cl != null)
			{
				Field field = FindField(dc.ColumnName, cl);

				if (null != field)
				{
					if (null != field.ColumnType)
						columnType = field.ColumnType;
					if (0 != field.ColumnLength && !field.IgnoreLengthInDDL)
						width = field.ColumnLength.ToString();
					if (0 != field.ColumnPrecision && !field.IgnoreLengthInDDL)
						precision = field.ColumnPrecision.ToString();
				}
				else if (cl.TimeStampColumn == dc.ColumnName)
				{
					if (!provider.SupportsNativeGuidType)
						width = "36";
				}
				else if (isPrimary && dc.AutoIncrement)
				{
					autoIncrement = true;
				}
			}
			if (null == columnType)
			{
				columnType = concreteGenerator.DbTypeFromType(dc.DataType);
			}

			if (null == width)
			{								
				int dl = provider.GetDefaultLength(dc.DataType);
				if (dl != 0)
					width = dl.ToString();
			}
			
			// Because there is no GetDefaultPrecision in the provider...
			// We assume the field to represent currency data
			if (precision == null && dc.DataType == typeof(decimal))
			{
				precision = "2";
			}

			if (columnType != null)
			{
				if (!concreteGenerator.LengthAllowed(columnType))
					width = null;
			}
			else
			{
				if (!concreteGenerator.LengthAllowed(dc.DataType))
					width = null;
			}


			if (autoIncrement && concreteGenerator.HasSpecialAutoIncrementColumnFormat)
				stream.Write(concreteGenerator.AutoIncrementColumn(name, dc.DataType, columnType, width));
			else if(isPrimary && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InColumn)
				stream.Write(concreteGenerator.PrimaryKeyColumn(name, dc.DataType, columnType, width));
			else if (width != null && precision != null)
				stream.Write(name + " " + columnType + "(" + width + "," + precision + ")");
			else if (width != null)
				stream.Write(name + " " + columnType + "(" + width + ")");			
			else
				stream.Write(name + " " + columnType);

			stream.Write(" ");

			stream.Write(concreteGenerator.NullExpression(dc.AllowDBNull));
			
		}

		private void CreateConstraint(DataColumn[] primaryKeyColumns, System.IO.StreamWriter stream, DataTable dt, IProvider provider)
		{
			if (primaryKeyColumns.Length == 0)
				return;
			string[] strArr = dt.TableName.Split('.');
			string constraintName = provider.GetQuotedName("PK_" + strArr[strArr.Length - 1]);
			stream.WriteLine(concreteGenerator.CreateConstraint(primaryKeyColumns, constraintName, QualifiedTableName.Get(dt.TableName, provider)));			
		}
	}
}
