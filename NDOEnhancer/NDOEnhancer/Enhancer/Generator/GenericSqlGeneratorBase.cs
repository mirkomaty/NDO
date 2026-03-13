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
        private readonly IProvider provider;
        protected ISqlGenerator concreteGenerator;
		protected MessageAdapter messages;
		protected NDOMapping mappings;

		public GenericSqlGeneratorBase(IProvider provider, ISqlGenerator concreteGenerator, MessageAdapter messages, NDOMapping mappings)
		{
            this.provider = provider;
            this.concreteGenerator = concreteGenerator;
			this.messages = messages;
			this.mappings = mappings;
		}

		protected string CreateTable(DataTable dt)
		{
			StringBuilder sb = new StringBuilder();

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
			int size = 0;

			if (cl != null)
			{
				Field field = FindField(dc.ColumnName, cl);
				if (null != field)
				{
					if (null != field.Column.DbType)
						columnType = field.Column.DbType;
					size = field.Column.Size;
					var defaultDbType = concreteGenerator.DbTypeFromType(dc.DataType, size);
					bool ignoreColumnSize = field.Column.IgnoreColumnSizeInDDL;
					if (0 != size && !ignoreColumnSize)
					{
						int dl = field.Column.Size;
						if (dl == -1 && String.Compare(defaultDbType, "nvarchar", true) == 0 && concreteGenerator.ProviderName == "SqlServer" )
							width = "max";
						else
							width = dl.ToString();
					}
                    if (0 != field.Column.Precision && !ignoreColumnSize)
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
                    columnType = concreteGenerator.DbTypeFromType(dc.DataType, size);
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
				sb.Append(concreteGenerator.AutoIncrementColumn(name, dc.DataType, columnType, width, isPrimary));
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
