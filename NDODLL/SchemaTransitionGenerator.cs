//
// Copyright (c) 2002-2024 Mirko Matytschak 
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using NDO.Mapping;
using NDOInterfaces;
using NDO.ProviderFactory;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für GenericDiffGenerator.
	/// </summary>
	internal class SchemaTransitionGenerator
	{
		private readonly IProvider provider;
        private readonly ISqlGenerator concreteGenerator;
        private readonly NDOMapping mappings;
		public SchemaTransitionGenerator( INDOProviderFactory providerFactory, string providerName, NDOMapping mappings ) 
		{
			this.provider = providerFactory[providerName];
			this.concreteGenerator = providerFactory.Generators[providerName];
			this.mappings = mappings;
		}

		public string Generate(XElement transElement)
		{
			StringBuilder sb = new StringBuilder();
			foreach(XElement actionElement in transElement.Elements())
			{
				if (actionElement.Name == "DropTable")
				{
					sb.Append( DropTable( actionElement ) );
				}
				else if (actionElement.Name=="CreateTable")
				{
					sb.Append( CreateTable( actionElement ) );
				}
				else if (actionElement.Name=="AlterTable")
				{
					sb.Append( ChangeTable( actionElement ) );
				}

			}
			return sb.ToString();
		}

		string ChangeTable(XElement actionElement)
		{
			List<XElement> addedColumns = actionElement.Elements("AddColumn").ToList();
			List<XElement> removedColumns = actionElement.Elements("DropColumn").ToList();
			List<XElement> changedColumns = actionElement.Elements("AlterColumn").ToList();

			if (addedColumns.Count == 0 && removedColumns.Count == 0 && changedColumns.Count == 0)
				return String.Empty;

			StringBuilder sb = new StringBuilder();

			string rawTableName = actionElement.Attribute( "name" ).Value;
			string tableName = this.provider.GetQualifiedTableName(rawTableName);
			string alterString = "ALTER TABLE " + tableName + ' ';

			foreach(XElement columnElement in addedColumns)
			{
				sb.Append(alterString);
				sb.Append(concreteGenerator.AddColumn());
				sb.Append(' ');
				sb.Append(CreateColumn(columnElement, GetClassForTable(rawTableName, this.mappings), this.provider, false));
				sb.Append(";\n");
			}

			foreach(XElement columnElement in removedColumns)
			{
				sb.Append(alterString);
				sb.Append( concreteGenerator.RemoveColumn( provider.GetQuotedName( columnElement.Attribute( "name" ).Value ) ) );
				sb.Append(";\n");
			}

			foreach(XElement columnElement in changedColumns)
			{
				sb.Append(alterString);
				sb.Append(concreteGenerator.AlterColumnType());
				sb.Append(' ');
				sb.Append(CreateColumn(columnElement, GetClassForTable(rawTableName, this.mappings), this.provider, false));
				sb.Append(";\n");
			}

			return sb.ToString();
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

		protected string DropTable(XElement actionElement)
		{
			string tableName = this.provider .GetQualifiedTableName( actionElement.Attribute( "name" ).Value );
			return concreteGenerator.DropTable( tableName );
		}

		protected string CreateTable(XElement actionElement)
		{
			StringBuilder sb = new StringBuilder();
			IProvider provider = NDOProviderFactory.Instance[concreteGenerator.ProviderName];
			if (provider == null)
				throw new Exception("Can't find NDO provider '" + concreteGenerator.ProviderName + "'.");

			string dtTableName = actionElement.Attribute( "name" ).Value;

			string tableName = this.provider.GetQualifiedTableName( dtTableName );

			Class cl = FindClass(dtTableName, mappings);

			if (cl != null)
			{
				Connection conn = mappings.FindConnection(cl);
				if (conn != null)
					concreteGenerator.ConnectToDatabase(conn.Name);
			}

			sb.Append(concreteGenerator.BeginnTable(tableName));
			sb.Append('\n');

			List<XElement> columnElements = actionElement.Elements( "CreateColumn" ).ToList();

			int vorletzterIndex = columnElements.Count - 1;
			List<XElement> primaryKeyColumns = columnElements.Where( e => 
			{ 
				XAttribute attr;
				return (attr = e.Attribute( "isPrimary" )) != null && String.Compare( attr.Value, "true", true ) == 0; 
			} ).ToList();

			bool hasPrimaryKeyColumns = primaryKeyColumns.Count > 0;

			for (int i = 0; i < columnElements.Count; i++)
			{
				XElement columnElement = columnElements[i];
				string columnName = columnElement.Attribute("name").Value;

				bool isPrimary = primaryKeyColumns.Any( e => e.Attribute( "name" ).Value == columnName ); 

				sb.Append(CreateColumn(columnElement, cl, provider, isPrimary));
				if (i < vorletzterIndex)
				{
					sb.Append(",");
					sb.Append('\n');
				}
			}

			if(concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InTable 
				&& hasPrimaryKeyColumns)
			{
				sb.Append(",");
				sb.Append('\n');
			}

			if (hasPrimaryKeyColumns && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InTable)
			{
				sb.Append(CreatePrimaryKeyConstraint(primaryKeyColumns, dtTableName, provider));
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
				sb.Append(CreatePrimaryKeyConstraint(primaryKeyColumns, dtTableName, provider));
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

		static bool IsNumeric( Type type )
		{
			switch (Type.GetTypeCode( type ))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				case TypeCode.Object:
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> ))
					{
						return IsNumeric(Nullable.GetUnderlyingType( type ));
					}
					return false;
				default:
					return false;
			}
		}

		protected string CreateColumn(XElement columnElement, Class cl, IProvider provider, bool isPrimary)
		{
			string rawName = columnElement.Attribute( "name" ).Value;
			Type dcDataType = Type.GetType( columnElement.Attribute( "type" ).Value );
			string name = provider.GetQuotedName(rawName);
			string columnType = columnElement.Attribute( "dbtype" )?.Value;
			if (columnType == String.Empty)  // Make sure the column type will be infered, if it is an empty string
				columnType = null;
			string width = columnElement.Attribute("size") == null ? null : columnElement.Attribute("size").Value;
			string precision = null;
			bool autoIncrement = false;
			StringBuilder sb = new StringBuilder();
            bool allowNull = true;
			string defaultValue = columnElement.Attribute( "default" )?.Value;

			if (cl != null)
			{
				Field field = FindField(rawName, cl);

				if (null != field)
				{
					if ( !String.IsNullOrEmpty(columnType) && null != field.Column.DbType)
						columnType = field.Column.DbType;
					if (0 != field.Column.Size && !field.Column.IgnoreColumnSizeInDDL)
					{
						int dl = field.Column.Size;
						if (dl == -1)
							width = "max";
						else
							width = dl.ToString();
					}
                    if (0 != field.Column.Precision && !field.Column.IgnoreColumnSizeInDDL)
						precision = field.Column.Precision.ToString();
                    allowNull = field.Column.AllowDbNull;
				}
				else if (cl.TimeStampColumn == rawName)
				{
					if (!provider.SupportsNativeGuidType)
						width = "36";
				}
				else if (isPrimary && columnElement.Attribute( "autoIncrement" ) != null && String.Compare(columnElement.Attribute( "autoIncrement" ).Value, "true", true) == 0)
				{
					autoIncrement = true;
				}
			}
			if (null == columnType)
			{
                try
                {
                    columnType = concreteGenerator.DbTypeFromType(dcDataType);
                }
                catch 
                {
                    System.Diagnostics.Debug.Write("");
                }
			}

			if (null == width)
			{
				int dl = provider.GetDefaultLength(dcDataType);
				if (dl != 0)
				{
					if (dl == -1)
						width = "max";
					else
						width = dl.ToString();
				}
			}
			
			// Because there is no GetDefaultPrecision in the provider...
			// We assume the field to represent currency data
			if (precision == null && dcDataType == typeof(decimal))
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
				if (!concreteGenerator.LengthAllowed(dcDataType))
					width = null;
			}


			if (autoIncrement && concreteGenerator.HasSpecialAutoIncrementColumnFormat)
				sb.Append(concreteGenerator.AutoIncrementColumn(name, dcDataType, columnType, width, isPrimary));
			else if(isPrimary && concreteGenerator.PrimaryConstraintPlacement == PrimaryConstraintPlacement.InColumn)
				sb.Append(concreteGenerator.PrimaryKeyColumn(name, dcDataType, columnType, width));
			else if (width != null && precision != null)
				sb.Append(name + " " + columnType + "(" + width + "," + precision + ")");
			else if (width != null)
				sb.Append(name + " " + columnType + "(" + width + ")");			
			else
				sb.Append(name + " " + columnType);

			sb.Append(" ");

			if (defaultValue != null)
			{
				sb.Append( "DEFAULT " );
				if (IsNumeric( dcDataType ))
					sb.Append( defaultValue);
				else
				{
					sb.Append( '\'' );
					sb.Append( defaultValue.Replace( "'", "''" ) );
					sb.Append( '\'' );
				}
				sb.Append( ' ' );
			}

			sb.Append( concreteGenerator.NullExpression( allowNull && columnElement.Attribute( "allowNull" ) != null && String.Compare( columnElement.Attribute( "allowNull" ).Value, "true", true ) == 0 ) );
			return sb.ToString();
		}

		protected string CreatePrimaryKeyConstraint(List<XElement> primaryKeyColumns, string tableName, IProvider provider)
		{
			if (primaryKeyColumns.Count == 0)
				return string.Empty;
			string[] strArr = tableName.Split('.');
			string constraintName = provider.GetQuotedName("PK_" + strArr[strArr.Length - 1]);
			DataColumn[] pkColumns = (from pk in primaryKeyColumns select new DataColumn( pk.Attribute( "name" ).Value )).ToArray();
			return concreteGenerator.CreatePrimaryKeyConstraint(pkColumns, constraintName, provider.GetQualifiedTableName(tableName)) + '\n';
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
