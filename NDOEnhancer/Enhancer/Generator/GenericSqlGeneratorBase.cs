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
using System.Data;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für GenericSqlGeneratorBase.
	/// </summary>
	internal class GenericSqlGeneratorBase
	{
		protected ISqlGenerator concreteGenerator;
		protected MessageAdapter messages;
		protected NDOMapping mappings;

		public GenericSqlGeneratorBase(ISqlGenerator concreteGenerator, MessageAdapter messages, NDOMapping mappings)
		{
			this.concreteGenerator = concreteGenerator;
			this.messages = messages;
			this.mappings = mappings;
		}

		protected string CreateTable(DataTable dt)
		{
			StringBuilder sb = new StringBuilder();
			IProvider provider = NDOProviderFactory.Instance[concreteGenerator.ProviderName];
			if (provider == null)
				throw new Exception("Can't find NDO provider '" + concreteGenerator.ProviderName + "'.");

			string tableName = QualifiedTableName.Get(dt.TableName, provider);

			Class cl = FindClass(dt.TableName, mappings);

			if (cl != null)
			{
				Connection conn = mappings.FindConnection(cl);
				if (conn != null)
					concreteGenerator.ConnectToDatabase(conn.Name);
			}

			sb.Append(concreteGenerator.BeginnTable(tableName));
			sb.Append('\n');

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

				sb.Append(CreateColumn(dc, cl, provider, isPrimary));
				if (i < vorletzterIndex)
				{
					sb.Append(",");
					sb.Append('\n');
				}
			}

			//			vorletzterIndex = dt.ParentRelations.Count - 1;
			//			if (vorletzterIndex > -1)
			//				sb.Append(",");
			//
			//			for (int i = 0; i < dt.ParentRelations.Count; i++)
			//			{
			//				DataRelation dr = dt.ParentRelations[i];
			//				sb.Append(concreteGenerator.CreateForeignKeyConstraint(dr.ChildColumns, dr.ParentColumns, provider.GetQuotedName(dr.RelationName), dr.ParentTable.TableName));
			//				if (i < vorletzterIndex)
			//					sb.Append(",");
			//			}

			if(concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InTable 
				&& hasPrimaryKeyColumns)
			{
				sb.Append(",");
				sb.Append('\n');
			}

			if (hasPrimaryKeyColumns && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InTable)
			{
				sb.Append(CreatePrimaryKeyConstraint(primaryKeyColumns, dt, provider));
			}
			else
			{
				sb.Append('\n');
			}
			sb.Append(concreteGenerator.EndTable(tableName));
			sb.Append('\n');

			//			CreateIndex(primaryKeyColumns, sb, dt, provider);

			if (hasPrimaryKeyColumns && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.AfterTable)
			{
				sb.Append(CreatePrimaryKeyConstraint(primaryKeyColumns, dt, provider));
			}

			sb.Append('\n');

			return sb.ToString();
		}

		protected NDO.Mapping.Class FindClass(string tableName, NDOMapping mappings)
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

		protected string CreateColumn(DataColumn dc, Class cl, IProvider provider, bool isPrimary)
		{
			string name = provider.GetQuotedName(dc.ColumnName);
			string columnType = null;
			string width = null;
			string precision = null;
			bool autoIncrement = false;
			StringBuilder sb = new StringBuilder();
            bool allowNull = true;

			if (cl != null)
			{
				Field field = FindField(dc.ColumnName, cl);

				if (null != field)
				{
					if (null != field.Column.DbType)
						columnType = field.Column.DbType;
					if (0 != field.Column.Size && !field.Column.IgnoreColumnSizeInDDL)
					{
						int dl = field.Column.Size;
						if (dl == -1 && dc.DataType == typeof( string ))
							width = "max";
						else
							width = dl.ToString();
					}
                    if (0 != field.Column.Precision && !field.Column.IgnoreColumnSizeInDDL)
						precision = field.Column.Precision.ToString();
                    allowNull = field.Column.AllowDbNull;
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
                try
                {
                    columnType = concreteGenerator.DbTypeFromType(dc.DataType);
                }
                catch 
                {
                    System.Diagnostics.Debug.Write("");
                }
			}

			if (null == width)
			{								
				int dl = provider.GetDefaultLength(dc.DataType);
				if (dl != 0)
				{
					if (dl == -1 && dc.DataType == typeof( string ))
						width = "max";
					else
						width = dl.ToString();
				}
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
				sb.Append(concreteGenerator.AutoIncrementColumn(name, dc.DataType, columnType, width));
			else if(isPrimary && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InColumn)
				sb.Append(concreteGenerator.PrimaryKeyColumn(name, dc.DataType, columnType, width));
			else if (width != null && precision != null)
				sb.Append(name + " " + columnType + "(" + width + "," + precision + ")");
			else if (width != null)
				sb.Append(name + " " + columnType + "(" + width + ")");			
			else
				sb.Append(name + " " + columnType);

			sb.Append(" ");

			sb.Append(concreteGenerator.NullExpression(allowNull && dc.AllowDBNull));
			return sb.ToString();
		}

		protected string CreatePrimaryKeyConstraint(DataColumn[] primaryKeyColumns, DataTable dt, IProvider provider)
		{
			if (primaryKeyColumns.Length == 0)
				return string.Empty;
			string[] strArr = dt.TableName.Split('.');
			string constraintName = provider.GetQuotedName("PK_" + strArr[strArr.Length - 1]);
			return concreteGenerator.CreatePrimaryKeyConstraint(primaryKeyColumns, constraintName, QualifiedTableName.Get(dt.TableName, provider)) + '\n';
		}


		protected Field FindField (string columnName, Class cl)
		{
			Field result = null;
			foreach (Field field in cl.Fields)
			{
				if (field.Column.Name == columnName)
				{
					result = field;
					break;
				}
			}
			return result;
		}


		protected Class GetClassForTable(string tableName, NDOMapping mapping)
		{
			foreach(Class cl in mapping.Classes)
				if (cl.TableName == tableName)
					return cl;
			return null;
		}


	}
}
