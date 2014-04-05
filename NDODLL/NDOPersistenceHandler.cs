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
using System.EnterpriseServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using NDO;
using NDO.Mapping;
using NDO.Logging;
using NDOInterfaces;
using System.Globalization;

namespace NDO
{
	/// <summary>
	/// Parameter type for the IProvider function RegisterRowUpdateHandler
	/// </summary>
	public delegate void RowUpdateHandler(DataRow row);

	internal class RelationFieldInfo
	{
		public Relation Rel;
		public string TableName;

		public RelationFieldInfo(Relation rel, string tableName)
		{
			Rel = rel;
			TableName = tableName;
		}
	}

	internal class ColumnListGenerator
	{
		IProvider p;
		IList columns = new ArrayList();
		string tableName;
		bool useTableName = true;
		bool generateAliasNames = false;

		public bool GenerateAliasNames
		{
			get { return this.generateAliasNames; }
			set { this.generateAliasNames = value; }
		}

		public bool UseTableName
		{
			get { return this.useTableName; }
			set { this.useTableName = value; }
		}

		public ColumnListGenerator(IProvider provider, string tableName)
		{
			this.tableName = tableName;
			p = provider;
		}

		public void Add(string columnName)
		{
			columns.Add(columnName);
		}

		public void Insert(string columnName)
		{
			columns.Insert(0, columnName);
		}

		public string Result
		{
			get 
			{
				StringBuilder result = new StringBuilder();
				int ende = columns.Count - 1;
				for (int i = 0; i < columns.Count; i++)
				{
					string f = (string) columns[i];
					if (this.useTableName)
					{
						result.Append(QualifiedTableName.Get(tableName, p));
						result.Append('.');
					}
					result.Append(p.GetQuotedName(f));
					if (this.generateAliasNames)
					{
						result.Append( " AS " );
						result.Append( p.GetQuotedName( f ) );
					}
					if (i < ende)
					{
						result.Append(", ");
					}
				}
				return result.ToString();
			}
		}
	}

	internal class ZuweisungsGenerator
	{
		IProvider p;
		IList fields = new ArrayList();

		public ZuweisungsGenerator(IProvider provider)
		{
			p = provider;
		}

		public void Add(string s)
		{
			fields.Add(s);
		}

//		public void Insert(string s)
//		{
//			fields.Insert(0, s);
//		}

		public string Result
		{
			get 
			{
				StringBuilder result = new StringBuilder();
				int ende = fields.Count - 1;
				for (int i = 0; i < fields.Count; i++)
				{
					string fieldName = (string)fields[i];
					result.Append(p.GetQuotedName(fieldName));
					result.Append(" = ");
					if (!p.UseNamedParams)
						result.Append("?");
					else
					{
						result.Append(p.GetNamedParameter("U_" + fieldName));
					}
					if (i < ende)
					{
						result.Append(", ");
					}
				}
				return result.ToString();
			}
		}
	}

	internal class ParamListGenerator
	{
		IProvider p;
		IList fields = new ArrayList();

		public ParamListGenerator(IProvider provider)
		{
			p = provider;
		}

		public void Add(string s)
		{
			fields.Add(s);
		}

		public void Insert(string s)
		{
			fields.Insert(0, s);
		}

		public string Result
		{
			get 
			{
				StringBuilder result = new StringBuilder();
				int ende = fields.Count - 1;
				for (int i = 0; i < fields.Count; i++)
				{
					if (p.UseNamedParams)
					{
						result.Append(p.GetNamedParameter((string)fields[i]));
						if (i < ende)
						{
							result.Append(", ");
						}
					}
					else
					{
						if (i < ende)
						{
							result.Append("?, ");
						}
						else
							result.Append("?");
					}
				}
				return result.ToString();
			}
		}
	}


	/// <summary>
	/// Summary description for NDOPersistenceHandler.
	/// </summary>
	/// 
	internal class NDOPersistenceHandler : IPersistenceHandler
	{
		public event ConcurrencyErrorHandler ConcurrencyError;

		private System.Data.IDbCommand selectCommand;
		private System.Data.IDbCommand insertCommand;
		private System.Data.IDbCommand updateCommand;
		private System.Data.IDbCommand deleteCommand;
		private System.Data.IDbConnection conn;
		private System.Data.Common.DbDataAdapter dataAdapter;
		private Class classMapping;
		private string selectFieldList;
		private string selectFieldListWithAlias;
		private string tableName;
		private string qualifiedTableName;
		private DataSet ds;
		private bool verboseMode;
		private ILogAdapter logAdapter;
		private Hashtable mappingTableHandlers = new Hashtable();  // all handlers
		private IProvider provider;
		private NDOMapping mappings;
		private string timeStampColumn = null;
        private Column typeNameColumn = null;
        private bool hasAutoincrementedColumn;
        private OidColumn autoIncrementColumn;
        private Hashtable persistentFields;
		private IList relationInfos;
//		private string where;
//		private string whereTS;
		private Type type;
		private int guidlength;
		private string hollowFields;
		private string hollowFieldsWithAlias;
		private bool hasGuidOid;

		
		private void GenerateSelectCommand()
		{
			this.selectCommand.CommandText = string.Empty;
		}

		private int ParameterLength(Mapping.Field fieldMapping, Type memberType)
		{
			if (0 == fieldMapping.Column.Size)
				return provider.GetDefaultLength(memberType);
			else
				return fieldMapping.Column.Size;
		}

		private void GenerateInsertCommand()
		{
			NDO.Mapping.Field fieldMapping;

			ColumnListGenerator columnListGenerator = new ColumnListGenerator(this.provider, this.tableName);
			columnListGenerator.UseTableName = false;
			ParamListGenerator paramListGenerator = new ParamListGenerator(this.provider);

#if nix
			if (this.useSelfGeneratedIds && !oidIsAField && !this.classMapping.Oid.IsMulti)
			{
				provider.AddParameter(insertCommand, provider.GetNamedParameter(oidColumnName), provider.GetDbType(classMapping.Oid.FieldType), provider.GetDefaultLength(classMapping.Oid.FieldType), this.oidColumnName);
				columnListGenerator.Add(oidColumnName);
				paramListGenerator.Add(oidColumnName); 
			}
#endif
            foreach (OidColumn oidColumn in this.classMapping.Oid.OidColumns)
            {
                if (!oidColumn.AutoIncremented && oidColumn.FieldName == null && oidColumn.RelationName == null)
                {
                    provider.AddParameter(insertCommand, provider.GetNamedParameter(oidColumn.Name), provider.GetDbType(oidColumn.SystemType), provider.GetDefaultLength(oidColumn.SystemType), oidColumn.Name);
                    columnListGenerator.Add(oidColumn.Name);
                    paramListGenerator.Add(oidColumn.Name);
                }
            }

			foreach (DictionaryEntry e in persistentFields)
			{
				Type memberType;
				if (e.Value is FieldInfo)
					memberType = ((FieldInfo)e.Value).FieldType;
				else
					memberType = ((PropertyInfo)e.Value).PropertyType;

				fieldMapping = classMapping.FindField((string) e.Key);

				// Ignore fields without mapping.
				if (null == fieldMapping) 
					continue;

				if (null == fieldMapping.Column.DbType)
				{
					fieldMapping.ColumnDbType = (int) provider.GetDbType(memberType);
				}
				else
				{
					fieldMapping.ColumnDbType = (int) provider.GetDbType(fieldMapping.Column.DbType);
				}

				provider.AddParameter(insertCommand, provider.GetNamedParameter(fieldMapping.Column.Name), fieldMapping.ColumnDbType, ParameterLength(fieldMapping, memberType), fieldMapping.Column.Name);

				columnListGenerator.Add(fieldMapping.Column.Name);
				paramListGenerator.Add(fieldMapping.Column.Name);
			}

			foreach (RelationFieldInfo ri in relationInfos)
			{
				Relation r = ri.Rel;
                foreach(ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
                {
                    provider.AddParameter(insertCommand, provider.GetNamedParameter(fkColumn.Name), provider.GetDbType(fkColumn.SystemType), provider.GetDefaultLength(fkColumn.SystemType), fkColumn.Name);
				    columnListGenerator.Add(fkColumn.Name);
				    paramListGenerator.Add(fkColumn.Name);
                }
				if (r.ForeignKeyTypeColumnName != null)
				{
					provider.AddParameter(insertCommand, provider.GetNamedParameter(r.ForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), r.ForeignKeyTypeColumnName);
					columnListGenerator.Add(r.ForeignKeyTypeColumnName);
					paramListGenerator.Add(r.ForeignKeyTypeColumnName);
				}

			}

			if (this.timeStampColumn != null)
			{
				provider.AddParameter(insertCommand, provider.GetNamedParameter(timeStampColumn), provider.GetDbType(typeof(Guid)), guidlength, this.timeStampColumn);
				columnListGenerator.Add(timeStampColumn);
				paramListGenerator.Add(this.timeStampColumn); 
			}

            if (this.typeNameColumn != null)
            {
                Type tncType = Type.GetType(this.typeNameColumn.NetType);
                provider.AddParameter(insertCommand, provider.GetNamedParameter(typeNameColumn.Name), provider.GetDbType(tncType), provider.GetDefaultLength(tncType), this.typeNameColumn.Name);
                columnListGenerator.Add(this.typeNameColumn.Name);
                paramListGenerator.Add(this.typeNameColumn.Name); 
            }

			string namedParamList = paramListGenerator.Result;
			string fieldList = columnListGenerator.Result;

            foreach (OidColumn oidColumn in this.classMapping.Oid.OidColumns)
            {
                if (oidColumn.FieldName == null && oidColumn.RelationName == null && oidColumn.AutoIncremented)
                    columnListGenerator.Insert(oidColumn.Name);
            }
			columnListGenerator.UseTableName = true;
			this.selectFieldList = columnListGenerator.Result;
			columnListGenerator.GenerateAliasNames = true;
			this.selectFieldListWithAlias = columnListGenerator.Result;
			string sql;
			//{0} = TableName: Mitarbeiter			
			//{1} = FieldList: vorname, nachname
			//{2} = NamedParamList mit @: @vorname, @nachname
			//{3} = FieldList mit Id: id, vorname, nachname 
			//{4} = Name der Id-Spalte
			if (hasAutoincrementedColumn && provider.SupportsLastInsertedId && provider.SupportsInsertBatch)
			{
#if NDO20
				sql = "INSERT INTO {0} ({1}) VALUES ({2}); SELECT {3} FROM {0} WHERE ({4} = " + provider.GetLastInsertedId(this.tableName, this.autoIncrementColumn.Name) + ")";
#else
				sql = "INSERT INTO {0} ({1}) VALUES ({2}); SELECT {3} FROM {0} WHERE ({4} = " + provider.GetLastInsertedId + ")";
#endif
                sql = string.Format(sql, qualifiedTableName, fieldList, namedParamList, selectFieldList, this.autoIncrementColumn.Name);
				this.insertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
			}
			else
			{
				sql = "INSERT INTO {0} ({1}) VALUES ({2})";
				sql = string.Format(sql, qualifiedTableName, fieldList, namedParamList);
			}
			if (hasAutoincrementedColumn && !provider.SupportsInsertBatch)
			{
				if (provider.SupportsLastInsertedId)
					provider.RegisterRowUpdateHandler(this);
				else
					throw new NDOException(32, "The provider of type " + provider.GetType().FullName + " doesn't support Autonumbered Ids. Use Self generated Ids instead.");
			}
			this.insertCommand.CommandText = sql;
			this.insertCommand.Connection = this.conn;
		}

		public void OnRowUpdate(DataRow row)
		{
			if (row.RowState == DataRowState.Deleted)
				return;
			if (!hasAutoincrementedColumn)
				return;
			string oidColumnName = this.autoIncrementColumn.Name;
			Type t = row[oidColumnName].GetType();
			if (t != typeof(int))
				return;
			// Ist schon eine ID vergeben?
			if (((int)row[oidColumnName]) > 0)
				return;
			bool unchanged = (row.RowState == DataRowState.Unchanged);
			IDbCommand cmd = provider.NewSqlCommand(this.conn);
            if (this.Transaction != null)
                cmd.Transaction = this.Transaction;
			cmd.CommandText = provider.GetLastInsertedId(this.tableName, this.autoIncrementColumn.Name);
			DumpBatch(cmd.CommandText);
			using (IDataReader reader = cmd.ExecuteReader())
			{
				if (reader.Read())
				{
					object oidValue = reader.GetValue(0);
					if ( this.verboseMode )
					{
						if ( oidValue == DBNull.Value )
							LogAdapter.Info( oidColumnName + " = DbNull" );
						else
							LogAdapter.Info( oidColumnName + " = " + oidValue );
					}
					row[oidColumnName] = oidValue;
					if (unchanged)
						row.AcceptChanges();
				}
				else
					throw new NDOException(33, "Can't read autonumbered id from the database.");
			}
		}

		private void GenerateUpdateCommand()
		{
			string sql;

			NDO.Mapping.Field fieldMapping;

//			if (!this.oidIsAField && provider is NDOSqlProvider)
//				sql = @"UPDATE {0} SET {1} WHERE ({2}) ; SELECT {3} FROM {0} WHERE ({4})";
//			else
				sql = @"UPDATE {0} SET {1} WHERE ({2})";

		
			//{0} = Tabellenname: Mitarbeiter
			//{1} = Zuweisungsliste: vorname = @vorname, nachname = @nachname 
			//{2} = Where-Bedingung: id = @Original_id [ AND TimeStamp = @Original_timestamp ]
			//{3} = Feldliste mit Id: id, vorname, nachname 
			//{4} = Where-Bedingung ohne Timestamp
			ZuweisungsGenerator zuwGenerator = new ZuweisungsGenerator(this.provider);
			foreach (DictionaryEntry e in persistentFields)
			{
				Type memberType;
				if (e.Value is FieldInfo)
					memberType = ((FieldInfo)e.Value).FieldType;
				else
					memberType = ((PropertyInfo)e.Value).PropertyType;

				fieldMapping = classMapping.FindField((string)e.Key);
				if (fieldMapping != null) 
				{ 
					zuwGenerator.Add(fieldMapping.Column.Name);
					provider.AddParameter(updateCommand, provider.GetNamedParameter("U_" + fieldMapping.Column.Name), fieldMapping.ColumnDbType, ParameterLength(fieldMapping, memberType), fieldMapping.Column.Name);
				} 
			}

			foreach (RelationFieldInfo ri in relationInfos) 
			{
				Relation r = ri.Rel;
				if(r.Multiplicity == RelationMultiplicity.Element && r.MappingTable == null
					|| r.Multiplicity == RelationMultiplicity.List && r.MappingTable == null && r.Parent.FullName != classMapping.FullName)
				{
                    foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
                    {
                        zuwGenerator.Add(fkColumn.Name);
                        Type systemType = fkColumn.SystemType;
                        provider.AddParameter(updateCommand, provider.GetNamedParameter("U_" + fkColumn.Name), provider.GetDbType(systemType), provider.GetDefaultLength(systemType), fkColumn.Name);
                    }
					if (r.ForeignKeyTypeColumnName != null)
					{
						zuwGenerator.Add(r.ForeignKeyTypeColumnName);
						provider.AddParameter(updateCommand, provider.GetNamedParameter("U_" + r.ForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), r.ForeignKeyTypeColumnName);
					}
				}
			}
			if (this.timeStampColumn != null)
			{
				zuwGenerator.Add(timeStampColumn);
			}

			string zuwListe = zuwGenerator.Result;

			string where = string.Empty;

			if (this.timeStampColumn != null)
			{
				if (provider.UseNamedParams)
					where += provider.GetQuotedName(timeStampColumn) + " = " + provider.GetNamedParameter("U_Original_" + timeStampColumn) + " AND ";
				else
					where += provider.GetQuotedName(timeStampColumn) + " = ? AND ";
				// The new timestamp value as parameter
				provider.AddParameter(updateCommand, provider.GetNamedParameter("U_" + timeStampColumn), provider.GetDbType(typeof(Guid)), guidlength, timeStampColumn);
				provider.AddParameter(updateCommand, provider.GetNamedParameter("U_Original_" + timeStampColumn), provider.GetDbType(typeof(Guid)), guidlength, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), timeStampColumn, System.Data.DataRowVersion.Original, null);
			}

            int oidCount = classMapping.Oid.OidColumns.Count;
			for (int i = 0; i < oidCount; i++)
			{
                OidColumn oidColumn = (OidColumn)classMapping.Oid.OidColumns[i];
				// Oid as parameter
				provider.AddParameter(updateCommand, provider.GetNamedParameter("U_Original_" + oidColumn.Name), provider.GetDbType(oidColumn.SystemType), oidColumn.TypeLength, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), oidColumn.Name, System.Data.DataRowVersion.Original, null);
				if (provider.UseNamedParams)
					where += provider.GetQuotedName(oidColumn.Name) + " = " + provider.GetNamedParameter("U_Original_" + oidColumn.Name);
				else
					where += provider.GetQuotedName(oidColumn.Name) + " = ?";

                Relation r = oidColumn.Relation;
                if (!this.hasGuidOid && r != null && r.ForeignKeyTypeColumnName != null)
                {
                    where += " AND " +
                        provider.GetQuotedName(r.ForeignKeyTypeColumnName) + " = " + provider.GetNamedParameter("U_Original_" + r.ForeignKeyTypeColumnName);
                    provider.AddParameter(updateCommand, provider.GetNamedParameter("U_Original_" + r.ForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), r.ForeignKeyTypeColumnName, System.Data.DataRowVersion.Original, null);
                }

                if (i < oidCount - 1)
                    where += " AND ";
			}
            //else
            //{
            //    // Dual oids are defined using two relations.
            //    MultiKeyHandler dkh = new MultiKeyHandler(this.classMapping);
				
            //    for (int i = 0; i < 2; i++)
            //    {
            //        where += provider.GetQuotedName(dkh.ForeignKeyColumnName(i)) + " = " + provider.GetNamedParameter("U_Original_" + dkh.ForeignKeyColumnName(i));
            //        provider.AddParameter(updateCommand, provider.GetNamedParameter("U_Original_" + dkh.ForeignKeyColumnName(i)), provider.GetDbType(dkh.GetClass(i).Oid.FieldType), dkh.GetClass(i).Oid.TypeLength, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), dkh.ForeignKeyColumnName(i), System.Data.DataRowVersion.Original, null);
            //        if (dkh.ForeignKeyTypeColumnName(i) != null && dkh.GetClass(i).Oid.FieldType != typeof(Guid))
            //        {
            //            where += " AND " + 
            //                provider.GetQuotedName(dkh.ForeignKeyTypeColumnName(i)) + " = " + provider.GetNamedParameter("U_Original_" + dkh.ForeignKeyTypeColumnName(i));
            //            provider.AddParameter(updateCommand, provider.GetNamedParameter("U_Original_" + dkh.ForeignKeyTypeColumnName(i)), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), dkh.ForeignKeyTypeColumnName(i), System.Data.DataRowVersion.Original, null);
            //        }
            //        if (i == 0)
            //            where += " AND ";
            //    }
            //}

			sql = string.Format(sql, qualifiedTableName, zuwListe, where, selectFieldList, where);
			//Console.WriteLine(sql);
			this.updateCommand.CommandText = sql;
		}


		private void GenerateDeleteCommand()
		{
			string sql = "DELETE FROM {0} WHERE ({1})";
			//{0} = Tabellenname: Mitarbeiter
			//{1} = Where-Bedingung: id = @Original_id

			string where = string.Empty;

            int oidCount = this.classMapping.Oid.OidColumns.Count;
            for(int i = 0; i < oidCount; i++)
			{
                OidColumn oidColumn = (OidColumn)this.classMapping.Oid.OidColumns[i];
				if (provider.UseNamedParams)
					where += provider.GetQuotedName(oidColumn.Name) + " = " + provider.GetNamedParameter("D_Original_" + oidColumn.Name);
				else
					where += provider.GetQuotedName(oidColumn.Name) + " = ?";
				provider.AddParameter(deleteCommand, provider.GetNamedParameter("D_Original_" + oidColumn.Name), provider.GetDbType(oidColumn.SystemType), oidColumn.TypeLength, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), oidColumn.Name, System.Data.DataRowVersion.Original, null);

                Relation r = oidColumn.Relation;
                if (!this.hasGuidOid && r != null && r.ForeignKeyTypeColumnName != null)
                {
                    where += " AND " +
                        provider.GetQuotedName(r.ForeignKeyTypeColumnName) + " = " + provider.GetNamedParameter("D_Original_" + r.ForeignKeyTypeColumnName);
                    provider.AddParameter(updateCommand, provider.GetNamedParameter("D_Original_" + r.ForeignKeyTypeColumnName), provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), r.ForeignKeyTypeColumnName, System.Data.DataRowVersion.Original, null);
                }

                if (i < oidCount - 1)
                    where += " AND ";
			}

			string whereTS = string.Empty;
			if (this.timeStampColumn != null)
			{
				if (provider.UseNamedParams)
					whereTS = " AND " + provider.GetQuotedName(timeStampColumn) + " = " + provider.GetNamedParameter("D_Original_" + timeStampColumn);
				else
					whereTS = " AND " + provider.GetQuotedName(timeStampColumn) + " = ?";
				provider.AddParameter(deleteCommand, provider.GetNamedParameter("D_Original_" + timeStampColumn), provider.GetDbType(typeof(Guid)), guidlength, System.Data.ParameterDirection.Input, false, ((System.Byte)(0)), ((System.Byte)(0)), timeStampColumn, System.Data.DataRowVersion.Original, null);
			}

			sql = string.Format(sql, qualifiedTableName, where + whereTS);
			this.deleteCommand.CommandText = sql;
		}

		private void CollectFields()
		{
			FieldMap fm = new FieldMap(this.classMapping);
			this.persistentFields = fm.PersistentFields;
		}


		/// <summary>
		/// Gets or sets a value which determines, if database operations will be logged in a logging file.
		/// </summary>
		public bool VerboseMode
		{
			get { return this.verboseMode; }
			set 
			{ 
				this.verboseMode = value; 
				foreach(DictionaryEntry de in this.mappingTableHandlers)
				{
					((IMappingTableHandler)de.Value).VerboseMode = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the log adapter to determine the sink where log entries are written to.
		/// </summary>
		public ILogAdapter LogAdapter
		{
			get 
			{ 
				return this.logAdapter; 
			}
			set 
			{ 
				this.logAdapter = value; 
				foreach(DictionaryEntry de in this.mappingTableHandlers)
				{
					((IMappingTableHandler)de.Value).LogAdapter = value;
				}
			}
		}

		public void Initialize(NDOMapping mappings, Type t, DataSet ds)
		{
			this.mappings = mappings;
			this.ds = ds;
			this.classMapping = mappings.FindClass(t);
			this.timeStampColumn = classMapping.TimeStampColumn;
            this.typeNameColumn = classMapping.TypeNameColumn;
            this.hasAutoincrementedColumn = false;
            foreach (OidColumn oidColumn in this.classMapping.Oid.OidColumns)
            {
                if (oidColumn.AutoIncremented)
                {
                    this.hasAutoincrementedColumn = true;
                    this.autoIncrementColumn = oidColumn;
                    break;
                }
            }
            this.hasGuidOid = this.classMapping.HasGuidOid;
			this.tableName = classMapping.TableName;
			Connection connInfo = mappings.FindConnection(classMapping);
			this.provider = mappings.GetProvider(connInfo);
			this.qualifiedTableName = QualifiedTableName.Get(tableName, provider);
			// The connection object will be initialized in the pm, to 
			// enable the callback for getting the real connection string.
			// CheckTransaction is the place, where this happens.
			this.conn = null;

            ColumnListGenerator generator = new ColumnListGenerator(this.provider, this.classMapping.TableName);
            foreach(OidColumn oidColumn in this.classMapping.Oid.OidColumns)
            {
                generator.Add(oidColumn.Name);
                Relation r = oidColumn.Relation;
                if (r != null && r.ForeignKeyTypeColumnName != null)
                    generator.Add(r.ForeignKeyTypeColumnName);
            }

#if MaskedOut
            hollowFields = generator.Result;
			if (this.timeStampColumn != null)
				this.hollowFields += ", " + (this.qualifiedTableName + "." + provider.GetQuotedName(this.timeStampColumn));

            if (this.typeNameColumn != null)
                this.hollowFields += ", " + (this.qualifiedTableName + "." + provider.GetQuotedName(this.typeNameColumn.Name));
#else
			if ( this.timeStampColumn != null )
				generator.Add( this.timeStampColumn );

			if ( this.typeNameColumn != null )
				generator.Add( this.typeNameColumn.Name );

#endif
			this.hollowFields = generator.Result;
			generator.GenerateAliasNames = true;
			this.hollowFieldsWithAlias = generator.Result;

			this.guidlength = provider.GetDefaultLength(typeof(Guid));
            if (this.guidlength == 0)
                this.guidlength = provider.SupportsNativeGuidType ? 16 : 36;


			this.selectCommand = provider.NewSqlCommand(conn);
			this.insertCommand = provider.NewSqlCommand(conn);
			this.updateCommand = provider.NewSqlCommand(conn);
			this.deleteCommand = provider.NewSqlCommand(conn);
			this.dataAdapter = provider.NewDataAdapter(selectCommand, updateCommand, insertCommand, deleteCommand);
			this.type = t;

			CollectFields();	// Alle Feldinformationen landen in persistentField
			// determine the relations, which have a foreign key
			// column in the table of the given class
			relationInfos = new RelationCollector(this.classMapping)
				.CollectRelations(); 


			GenerateSelectCommand();
			GenerateInsertCommand();
			GenerateUpdateCommand();
			GenerateDeleteCommand();
		}
	
		#region Implementation of IPersistenceHandler
#if obsolete
		public System.Data.DataRow Fill(NDO.ObjectId id) 
		{
			try
			{
				string sql;

				DataTable table = GetTable(this.tableName).Clone();
			
				sql = "SELECT {0} FROM {1} WHERE {2} = ";  // we add the value later, because String.Format doesn't like "{guid-xxx}" literals
				//{0} = FieldList mit Id: id, vorname, nachname 
				//{1} = TableName: Mitarbeiter			
				//{2} = Name der ID-Spalte: id

				sql = string.Format(sql, selectFieldList, qualifiedTableName, provider.GetQuotedName(oidColumnName));

				sql += provider.GetSqlLiteral(id.Id.Value);

				this.selectCommand.CommandText = sql;
				this.selectCommand.Parameters.Clear();
				this.Dump(null);
				dataAdapter.Fill(table);

				if(table.Rows.Count != 1) 
				{
					throw new NDOxxException(35, String.Format("Object {0} not found", id));
				}
				return table.Rows[0];
			}
			catch (System.Exception ex)
			{
				string text = "Exception vom Typ " + ex.GetType().Name + " bei Select: " + ex.Message + "\n";
				if ((ex.Message.IndexOf("Die Variable") > -1 && ex.Message.IndexOf("muss deklariert") > -1) || (ex.Message.IndexOf("Variable") > -1 && ex.Message.IndexOf("declared") > -1))
					text += "Sind die Feldnamen in der Mapping-Datei richtig geschrieben?\n";
				text += "Sql-Select-Anweisung: " + selectCommand.CommandText + "\n";
				throw new NDOException(36, text);
			}
		}

#endif


		public void Update(DataTable dt) 
		{
			DataRow[] rows = null;
			try
			{
				rows = Select(dt, DataViewRowState.Added | DataViewRowState.ModifiedCurrent);
				if (rows.Length == 0)
					return;
				Dump(rows);
				if (this.timeStampColumn != null)
				{
					Guid newTs = Guid.NewGuid();
					foreach(DataRow r in rows)
						r[timeStampColumn] = newTs;
				}
                dataAdapter.Update(rows);
			}
			catch (System.Data.DBConcurrencyException dbex)
			{
				if (this.ConcurrencyError != null)
				{
					// This is a Firebird Hack because Fb doesn't set the row
					if (dbex.Row == null)
					{
						foreach(DataRow r in rows)
						{
							if (r.RowState == DataRowState.Added ||
								r.RowState == DataRowState.Modified)
							{
								dbex.Row = r;
								break;
							}
						}
					}
					ConcurrencyError(dbex);
				}
				else
					throw dbex;
			}
            catch (System.Exception ex)
            {
                string text = "Exception of type " + ex.GetType().Name + " while updating or inserting data rows: " + ex.Message + "\n";
                if ((ex.Message.IndexOf("Die Variable") > -1 && ex.Message.IndexOf("muss deklariert") > -1) || (ex.Message.IndexOf("Variable") > -1 && ex.Message.IndexOf("declared") > -1))
                    text += "Check the field names in the mapping file.\n";
                text += "Sql Update statement: " + updateCommand.CommandText + "\n";
                text += "Sql Insert statement: " + insertCommand.CommandText;
                throw new NDOException(37, text);
            }
		}


		private void DumpBatch(string sql)
		{
			if (!this.verboseMode)
				return;
			this.logAdapter.Info("Batch: \r\n" + sql);
		}

		private void Dump(DataRow[] rows)
		{
			if (!this.verboseMode)
				return;
			new SqlDumper(this.logAdapter, this.provider, insertCommand, selectCommand, updateCommand, deleteCommand).Dump(rows);
		}

        DataRow[] Select(DataTable dt, DataViewRowState rowState)
        {
            DataRow[] rows = dt.Select(null, null, rowState);
            // Mono Hack: Since some rows in Mono are null after Select.
            // We have to remove it manually
            int count = 0;
            for (int i = 0; i < rows.Length; i++)
                if (rows[i] != null)
                    count++;
            if (count < rows.Length)
            {
                DataRow[] rows2 = new DataRow[count];
                count = 0;
                for (int i = 0; i < rows.Length; i++)
                    if (rows[i] != null)
                        rows2[count++] = rows[i];
                return rows2;
            }
            // End Mono Hack
            return rows;
        }

        public void UpdateDeleted0bjects(DataTable dt)
		{
			DataRow[] rows = Select(dt, DataViewRowState.Deleted);
			if (rows.Length == 0) return;
			Dump(rows);
			try
			{
				dataAdapter.Update(rows);
			}
			catch (System.Data.DBConcurrencyException dbex)
			{
				if (this.ConcurrencyError != null)
					ConcurrencyError(dbex);
				else
					throw dbex;
			}
			catch (System.Exception ex)
			{
				string text = "Exception of type " + ex.GetType().Name + " while deleting data rows: " + ex.Message + "\n";
				text += "Sql statement: " + deleteCommand.CommandText + "\n";
				throw new NDOException(38, text);
			}
		}

		private DataTable GetTable(string name)
		{
			DataTable dt = ds.Tables[name];
			if (dt == null)
				throw new NDOException(39, "Can't find table '" + name + "' in the schema. Check your mapping file.");
			return dt;
		}

		/// <summary>
		/// Executes a batch of sql statements.
		/// </summary>
		/// <param name="statements">Each element in the array is a sql statement.</param>
		/// <param name="parameters">A list of parameters (see remarks).</param>
		/// <returns>An IList with Hashtables, containing the Name/Value pairs of the results.</returns>
		/// <remarks>
		/// For emty resultsets an empty Hashtable will be returned. 
		/// If parameters is a NDOParameterCollection, the parameters in the collection are valid for 
		/// all subqueries. If parameters is an ordinary IList, NDO expects to find a NDOParameterCollection 
		/// for each subquery. If an element is null, no parameters are submitted for the given query.
		/// </remarks>
		public IList ExecuteBatch(string[] statements, IList parameters)
		{
			IList result = new ArrayList();
			bool closeIt = false;
			IDataReader dr = null;
			int i;
			try
			{
				if (this.conn.State != ConnectionState.Open)
				{
					closeIt = true;
					this.conn.Open();
				}
				string sql = string.Empty;

				if (this.provider.SupportsBulkCommands)
				{
					IDbCommand cmd = this.provider.NewSqlCommand(conn);
					if (parameters != null && parameters.Count > 0)
					{
						if (parameters is NDOParameterCollection)
						{
							// Only the first command gets parameters
							for (i = 0; i < statements.Length; i++)
							{
								if (i == 0)
									CreateQueryParameters(ref statements[i], cmd, parameters, 0);
								else
									CreateQueryParameters(ref statements[i], null, null, 0);
							}
						}
						else
						{
							for (i = 0; i < statements.Length; i++)
							{
								CreateQueryParameters(ref statements[i], cmd, (NDOParameterCollection) parameters[i], cmd.Parameters.Count);
							}
						}
					}
					sql = this.provider.GenerateBulkCommand(statements);
					DumpBatch(sql);
					if (this.Transaction != null)
						cmd.Transaction = this.Transaction;
					cmd.CommandText = sql;
					dr = cmd.ExecuteReader();
					for(;;)
					{
						Hashtable ht = new Hashtable();
						while(dr.Read())
						{
							for (i = 0; i < dr.FieldCount; i++)
							{
								ht.Add(dr.GetName(i), dr.GetValue(i));
							}
						}
						result.Add(ht);
						if (!dr.NextResult())
							break;
					} 
					dr.Close();
				}
				else
				{
					for (i = 0; i < statements.Length; i++)
					{
						string s = statements[i];
						sql += s + ";\n"; // For DumpBatch only
						Hashtable ht = new Hashtable();
						IDbCommand cmd = this.provider.NewSqlCommand(conn);
                        if (this.Transaction != null)
                            cmd.Transaction = this.Transaction;
						if (parameters != null && parameters.Count > 0)
						{
							if (parameters is NDOParameterCollection)
							{
								CreateQueryParameters(ref s, cmd, parameters, 0);
							}
							else
							{
								CreateQueryParameters(ref s, cmd, (NDOParameterCollection) parameters[i], 0);
							}
						}
						cmd.CommandText = s;
						dr = cmd.ExecuteReader();
						while(dr.Read())
						{
							for (int j = 0; j < dr.FieldCount; j++)
							{
								ht.Add(dr.GetName(j), dr.GetValue(j));
							}
						}
						dr.Close();
						result.Add(ht);
					}
					DumpBatch(sql);
				}
			}
			finally
			{
				if (dr != null && !dr.IsClosed)
					dr.Close();
				if (closeIt)
					this.conn.Close();
			}
			return result;
		}

		private void CreateQueryParameters(IDbCommand command, IList parameters)
		{
			command.Parameters.Clear();

			if (parameters == null || parameters.Count == 0)
				return;

			string sql = command.CommandText;
			CreateQueryParameters(ref sql, command, parameters, 0);
			command.CommandText = sql;
		}

		private void CreateQueryParameters(ref string commandText, IDbCommand command, IList parameters, int offset)
		{
			string sql = commandText;
			int nr;
			Regex regex = new Regex(@"\{(tc:|)(\d+)\}");
			
			MatchCollection matches = regex.Matches(sql);
			Dictionary<string, object> tcValues = new Dictionary<string, object>();
			foreach(Match match in matches)
			{
				nr = int.Parse(match.Groups[2].Value) + offset;
				string tc = match.Groups[1].Value.Replace( ":", string.Empty );
				if ( tc != string.Empty )
				{
					object p = parameters[nr];
					if (p is Query.Parameter)
						p = ((Query.Parameter)p).Value;
					ObjectId oid = p as ObjectId;
					if ( oid == null )
						throw new QueryException( 10005, "Parameter {" + nr + "} was expected to be an ObjectId." );
					string key = "ptc" + nr;
					if ( !tcValues.ContainsKey( key ) )
						tcValues.Add( key, oid.Id.TypeId );
				}
				sql = sql.Replace(match.Value, 
					this.provider.GetNamedParameter("p" + tc + nr.ToString()));
			}

			Dictionary<string, object> oidParameters = new Dictionary<string, object>();
			regex = new Regex( @"\{oid:(\d+):(\d+)\}" );
			matches = regex.Matches( sql );
			if ( matches.Count > 0 )
			{
				foreach ( Match match in matches )
				{
					int parIndex = int.Parse( match.Groups[1].Value ) + offset;
					int oidIndex = int.Parse( match.Groups[2].Value ) + offset;
					object p = parameters[parIndex];
					if ( p is Query.Parameter )
						p = ( (Query.Parameter) p ).Value;
					ObjectId oid = p as ObjectId;

					if ( oid == null && oidIndex > 0 )
						throw new QueryException( 10005, "Parameter {" + parIndex + "} was expected to be an ObjectId." );

					if (oid != null && this.type != oid.Id.Type)
						throw new QueryException( 10006, "Oid-Parameter {" + parIndex + "} has the wrong type: '" + oid.Id.Type.Name +"'");


					string key = null;

					if ( oidIndex > 0 )  // need to add additional parameters for the 2nd, 3rd etc. column
					{
						key = "poid" + oidIndex;
						if ( !tcValues.ContainsKey( key ) )
							tcValues.Add( key, oid.Id[oidIndex] );
					}
					else
					{
						key = "p" + parIndex;
					}
					sql = sql.Replace( match.Value,
						this.provider.GetNamedParameter( key ) );
				}
			}

			commandText = sql;

			if (parameters == null || parameters.Count == 0)
				return;

			for (int i = 0; i < parameters.Count; i++)
			{
				nr = offset + i;
				object val = parameters[i];
				Query.Parameter p;
                if (val is Query.Parameter)
                {
                    p = (Query.Parameter)val;
                }
                else
                {
                    p = new Query.Parameter(val);
                }
				Type type = p.Value.GetType();
                if (type.FullName.StartsWith("System.Nullable`1"))
                    type = type.GetGenericArguments()[0];
				if ( type == typeof( ObjectId ) )
				{
					type = ( (ObjectId) p.Value ).Id.Value.GetType();
					p.Value = ( (ObjectId) p.Value ).Id.Value;
				}
                if (type.IsEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                    p.Value = ((IConvertible)p.Value).ToType(type, CultureInfo.CurrentCulture);
                }
				else if (type == typeof(Guid) && !provider.SupportsNativeGuidType)
				{
					type = typeof(string);
					p.Value = p.Value.ToString();
				}
				string name = "p" + nr.ToString();
				int length = this.provider.GetDefaultLength(type);
				if (type == typeof(string))
				{
					length = ((string)p.Value).Length;
					if (provider.GetType().Name.IndexOf("Oracle") > -1)
					{
						if (length == 0)
							throw new QueryException(10001, "Empty string parameters are not allowed in Oracle. Use IS NULL instead.");
					}
				}
				else if (type == typeof(byte[]))
				{
					length = ((byte[])p.Value).Length;
				}
				IDataParameter par = provider.AddParameter(
					command, 
					this.provider.GetNamedParameter(name), 
					this.provider.GetDbType(type), 
					length, 
					this.provider.GetQuotedName(name)); 
				par.Value = p.Value;
				par.Direction = ParameterDirection.Input;					
			}

			foreach ( string key in tcValues.Keys )
			{
				string name = this.provider.GetNamedParameter( key );
				object o = tcValues[key];
				Type type = o.GetType();
				IDataParameter par = provider.AddParameter(
					command,
					name,
					this.provider.GetDbType( type ),
					this.provider.GetDefaultLength( type ),
					key );
				par.Value = tcValues[key];
				par.Direction = ParameterDirection.Input;
			}
		}

		public DataTable SqlQuery( string expression, bool hollow, bool dontTouch, IList parameters )
		{
			if (expression.Trim().IndexOf(" ") == -1)
				this.selectCommand.CommandType = CommandType.StoredProcedure;
			else
				this.selectCommand.CommandType = CommandType.Text;

			DataTable table = GetTable(this.tableName).Clone();
			string sql;
			if (expression.IndexOf(Query.FieldMarker) > -1) // Abfragesprache ist SQL
			{
				string fields = hollow ? this.hollowFields : this.selectFieldList;
				if ( provider.GetType().FullName.IndexOf( "Sqlite" ) > -1 )
				{
					// We have to hack around a special behavior of SQLite, generating
					// new columns with fully specified column names, if the query
					// is a UNION
					if ( expression.IndexOf( " UNION ", StringComparison.InvariantCultureIgnoreCase ) > 0 )
					{
						fields = hollow ? this.hollowFieldsWithAlias : this.selectFieldListWithAlias;
					}
				}
				sql = expression.Replace(Query.FieldMarker, fields);
			}
			else
				sql = expression;

			this.selectCommand.CommandText = sql;

			CreateQueryParameters(this.selectCommand, parameters);

			Dump(null); // Dumps the Select Command

            try
            {
				dataAdapter.Fill(table);
            }
            catch (System.Exception ex)
            {
                string text = "Exception of type " + ex.GetType().Name + " while executing a Query: " + ex.Message + "\n";
                text += "Sql Statement: " + sql + "\n";
                throw new NDOException(40, text);
            }

			return table;		
		}


		/// <summary>
		/// Gets a Handler which can store data in relation tables
		/// </summary>
		/// <param name="r">Relation information</param>
		/// <returns>The handler</returns>
		public IMappingTableHandler GetMappingTableHandler(Relation r) 
		{
			IMappingTableHandler handler = (NDOMappingTableHandler)mappingTableHandlers[r.FieldName];
			if(handler == null) 
			{
				handler = new NDOMappingTableHandler();
				handler.Initialize(mappings, r, this.ds);
				handler.VerboseMode = this.verboseMode;
				handler.LogAdapter = this.logAdapter;
				mappingTableHandlers[r.FieldName] = handler;
			}
			return handler;
		}

	
	
		public IDbConnection Connection
		{
			get { return this.conn; }
			set
			{
                this.conn = value;
				this.selectCommand.Connection = value;
				this.deleteCommand.Connection = value;
				this.updateCommand.Connection = value;
				this.insertCommand.Connection = value;
			}
		}
	
		public IDbTransaction Transaction
		{
			get { return this.selectCommand.Transaction; }
			set
			{
				this.selectCommand.Transaction = value;
				this.deleteCommand.Transaction = value;
				this.updateCommand.Transaction = value;
				this.insertCommand.Transaction = value;
			}
		}


		public DbDataAdapter DataAdapter
		{
			get { return dataAdapter; }
		}
		#endregion
	}
}
