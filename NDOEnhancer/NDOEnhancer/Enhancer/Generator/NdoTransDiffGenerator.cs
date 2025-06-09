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
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;
using NDO;
using NDO.Mapping;
using NDOInterfaces;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für SQLDiffGenerator.
	/// </summary>
	internal class NdoTransDiffGenerator
	{
		NDOMapping mappings;
		MessageAdapter messages;
		public void Generate(DataSet dsSchema, DataSet dsBak, string filename, NDOMapping mappings, MessageAdapter messages)
		{
			this.mappings = mappings;
			this.messages = messages;

			string diffFileName = filename.Replace(".ndo.sql", ".ndodiff." + mappings.SchemaVersion + ".xml");
            bool isNewDiffFile = !File.Exists(diffFileName);
			XElement rootElement = null;
            if (isNewDiffFile)
            {
				rootElement = new XElement( "NdoSchemaTransitions" );
				rootElement.Add( new XComment( "NDO accumulates all schema changes in this diff file. Note: If you change the mapping schema version, a new diff file will be created. You can change the mapping schema version in the NDO configuration dialog. Don't use the Mapping Tool to change the schema information, because it will be overwritten by the value set in the configuration. For automatic builds set the schema version value in the .ndoproj file." ) );
            }
			else
			{
				rootElement = XElement.Load( diffFileName );
			}

			XElement transElement = new XElement( "NdoSchemaTransition" );
			rootElement.Add( transElement );

			if (GenerateInternal( dsSchema, dsBak, transElement ))
			{
				Guid g = MakeHash( transElement.ToString() );
				transElement.SetAttributeValue( "id", g );
				rootElement.Save( diffFileName );
			}
		}

        public static Guid MakeHash( string input )
        {
            var buf = Encoding.UTF8.GetBytes(input);

            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash( buf );
				byte[] arr = new byte[16];
				Array.Copy( hash, arr, 16 );
				return new Guid( arr );
            }
        }

        bool GenerateInternal(System.Data.DataSet dsNewSchema, System.Data.DataSet dsOldSchema, XElement transElement)
		{
			bool hasChanges = false;
			foreach(DataTable dt in dsNewSchema.Tables)
			{
				if (!dsOldSchema.Tables.Contains(dt.TableName))
				{
					transElement.Add( CreateTable( dt ) );
					hasChanges = true;
				}
				else
				{
					DataTable dtOld = dsOldSchema.Tables[dt.TableName];
					List<DataColumn> addedColumns = new List<DataColumn>();
					List<DataColumn> removedColumns = new List<DataColumn>();
					List<DataColumn> changedColumns = new List<DataColumn>();
					foreach(DataColumn dc in dt.Columns)
					{
						if (!dtOld.Columns.Contains(dc.ColumnName))
						{
							addedColumns.Add(dc);
						}
						else
						{
							DataColumn dcOld = dtOld.Columns[dc.ColumnName];
							if (dc.DataType != dcOld.DataType || dc.MaxLength != dcOld.MaxLength)
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
					if (addedColumns.Count > 0 || removedColumns.Count > 0 || changedColumns.Count > 0)
						hasChanges = true;

					if (!hasChanges)
						continue;

					XElement changeTableElement = ChangeTable(dt, addedColumns, removedColumns, changedColumns);
					if (changeTableElement.HasElements)
						transElement.Add( changeTableElement );

				}
			}

			foreach (DataTable dt in dsOldSchema.Tables)
			{
				if (!dsNewSchema.Tables.Contains( dt.TableName ))
				{
					XElement dropTableElement = new XElement( "DropTable", new XAttribute( "name", dt.TableName ) );
					transElement.Add( dropTableElement );
					hasChanges = true;
				}
			}

			return hasChanges;
		}

		XElement ChangeTable(DataTable dt, List<DataColumn> addedColumns, List<DataColumn> removedColums, List<DataColumn> changedColumns)
		{
			XElement alterTableElement = new XElement( "AlterTable", new XAttribute( "name", dt.TableName ) );

			foreach(DataColumn dc in addedColumns)
			{
				XElement addColumnElement = new XElement( "AddColumn", new XAttribute( "name", dc.ColumnName ) );
				CreateColumn(addColumnElement, dc, GetClassForTable(dt.TableName, this.mappings), false);
				alterTableElement.Add( addColumnElement );
			}

			foreach(DataColumn dc in removedColums)
			{
				XElement dropColumnElement = new XElement( "DropColumn", new XAttribute( "name", dc.ColumnName ) );
				alterTableElement.Add( dropColumnElement );
			}

			foreach(DataColumn dc in changedColumns)
			{
				XElement alterColumnElement = new XElement( "AlterColumn", new XAttribute( "name", dc.ColumnName ) );
				CreateColumn(alterColumnElement, dc, GetClassForTable(dt.TableName, this.mappings), false);
				alterTableElement.Add( alterColumnElement );
			}

			if (removedColums.Count > 0)
			{
				alterTableElement.Add( new XComment( "If you need to rename a column use the following syntax:" ) );
				alterTableElement.Add( new XComment( "<RenameColumn oldName=\"...\" newName=\"...\" />" ) );
			}

			return alterTableElement;			
		}

		protected XElement CreateTable(DataTable dt)
		{
			XElement createTableElement = new XElement( "CreateTable", new XAttribute( "name", dt.TableName ) );

			Class cl = FindClass(dt.TableName, mappings);

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

				XElement createColumnElement = new XElement( "CreateColumn", new XAttribute( "name", dc.ColumnName ) );
				CreateColumn(createColumnElement, dc, cl, isPrimary);
				createTableElement.Add( createColumnElement );
			}

			return createTableElement;
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

		protected void CreateColumn(XElement addColumnElement, DataColumn dc, Class cl, bool isPrimary)
		{
			Type t = dc.DataType;
			addColumnElement.Add( new XAttribute( "type", t.FullName + "," + new AssemblyName( t.Assembly.FullName ).Name ) );

			if (isPrimary)
				addColumnElement.Add( new XAttribute( "isPrimary", "true" ) );

			if (cl != null)
			{
				Field field = FindField(dc.ColumnName, cl);

				if (null != field)
				{
					if (0 != field.Column.Size && !field.Column.IgnoreColumnSizeInDDL)
					{
						if (field.Column.Size != 0)
							addColumnElement.Add( new XAttribute( "size", field.Column.Size.ToString() ) );
					}
					if (0 != field.Column.Precision && !field.Column.IgnoreColumnSizeInDDL)
					{
						if (field.Column.Precision != 0)
							addColumnElement.Add( new XAttribute( "precision", field.Column.Precision.ToString() ) );
					}
					addColumnElement.Add( new XAttribute( "allowNull", field.Column.AllowDbNull.ToString() ) );                    
				}
				else if (isPrimary && dc.AutoIncrement)
				{
					addColumnElement.Add( new XAttribute( "autoIncrement", "true" ) );                    
				}
			}			
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
