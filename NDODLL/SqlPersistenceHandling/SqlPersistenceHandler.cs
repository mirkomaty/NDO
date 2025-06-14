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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using NDO.Mapping;
using NDOInterfaces;
using NDO.Query;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// Parameter type for the IProvider function RegisterRowUpdateHandler
	/// </summary>
	public delegate void RowUpdateHandler(DataRow row);

	/// <summary>
	/// Summary description for NDOPersistenceHandler.
	/// </summary>
	/// 
	public class SqlPersistenceHandler : IPersistenceHandler
	{
		/// <summary>
		/// This event will be triggered, if a concurrency error occurs
		/// </summary>
		public event ConcurrencyErrorHandler ConcurrencyError;

		private IDbCommand selectCommand;
		private IDbCommand insertCommand;
		private IDbCommand updateCommand;
		private IDbCommand deleteCommand;
		private IDbConnection conn;
		private IDbTransaction transaction;
		private DbDataAdapter dataAdapter;
		private Class classMapping;
		private string selectFieldList;
		private string selectFieldListWithAlias;
		private string tableName;
		private string qualifiedTableName;
		private ILogger logger;
		private Dictionary<string, IMappingTableHandler> mappingTableHandlers = new Dictionary<string, IMappingTableHandler>();
		private IProvider provider;
		private NDOMapping ndoMapping;
		private string timeStampColumn = null;
        private Column typeNameColumn = null;
        private bool hasAutoincrementedColumn;
        private OidColumn autoIncrementColumn;
        private Dictionary<string,MemberInfo> persistentFields;
		private List<RelationFieldInfo> relationInfos;
		private Type type;
		private int guidlength;
		private string hollowFields;
		private string hollowFieldsWithAlias;
		private string fieldList;
		private string namedParamList;
		private bool hasGuidOid;
		private readonly IServiceProvider serviceProvider;
		private readonly ILoggerFactory loggerFactory;
		private Action<Type,IPersistenceHandler> disposeCallback;

		/// <summary>
		/// Constructs a SqlPersistenceHandler object
		/// </summary>
		/// <param name="serviceProvider"></param>
		public SqlPersistenceHandler(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			this.logger = serviceProvider.GetRequiredService<ILogger<SqlPersistenceHandler>>();
			this.loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
		}

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
			// Generate Parameters
			foreach (OidColumn oidColumn in this.classMapping.Oid.OidColumns)
			{
				if (!oidColumn.AutoIncremented && oidColumn.FieldName == null && oidColumn.RelationName == null)
				{
					provider.AddParameter( insertCommand, provider.GetNamedParameter( oidColumn.Name ), provider.GetDbType( oidColumn.SystemType ), provider.GetDefaultLength( oidColumn.SystemType ), oidColumn.Name );
				}
			}

			foreach (var e in this.persistentFields)
			{
				Type memberType;
				if (e.Value is FieldInfo)
					memberType = ((FieldInfo)e.Value).FieldType;
				else
					memberType = ((PropertyInfo)e.Value).PropertyType;

				var fieldMapping = classMapping.FindField( (string)e.Key );

				// Ignore fields without mapping.
				if (null == fieldMapping)
					continue;

				if (null == fieldMapping.Column.DbType)
				{
					fieldMapping.ColumnDbType = (int)provider.GetDbType( memberType );
				}
				else
				{
					fieldMapping.ColumnDbType = (int)provider.GetDbType( fieldMapping.Column.DbType );
				}

				provider.AddParameter( insertCommand, provider.GetNamedParameter( fieldMapping.Column.Name ), fieldMapping.ColumnDbType, ParameterLength( fieldMapping, memberType ), fieldMapping.Column.Name );
			}

			foreach (RelationFieldInfo ri in relationInfos)
			{
				Relation r = ri.Rel;
				foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
				{
					provider.AddParameter( insertCommand, provider.GetNamedParameter( fkColumn.Name ), provider.GetDbType( fkColumn.SystemType ), provider.GetDefaultLength( fkColumn.SystemType ), fkColumn.Name );
				}
				if (r.ForeignKeyTypeColumnName != null)
				{
					provider.AddParameter( insertCommand, provider.GetNamedParameter( r.ForeignKeyTypeColumnName ), provider.GetDbType( typeof( int ) ), provider.GetDefaultLength( typeof( int ) ), r.ForeignKeyTypeColumnName );
				}

			}

			if (this.timeStampColumn != null)
			{
				provider.AddParameter( insertCommand, provider.GetNamedParameter( timeStampColumn ), provider.GetDbType( typeof( Guid ) ), guidlength, this.timeStampColumn );
			}

			if (this.typeNameColumn != null)
			{
				Type tncType = Type.GetType( this.typeNameColumn.NetType );
				provider.AddParameter( insertCommand, provider.GetNamedParameter( typeNameColumn.Name ), provider.GetDbType( tncType ), provider.GetDefaultLength( tncType ), this.typeNameColumn.Name );
			}

			string sql;
			//{0} = TableName: Mitarbeiter			
			//{1} = FieldList: vorname, nachname
			//{2} = NamedParamList mit @: @vorname, @nachname
			//{3} = FieldList mit Id: id, vorname, nachname 
			//{4} = Name der Id-Spalte
			if (hasAutoincrementedColumn && provider.SupportsLastInsertedId && provider.SupportsInsertBatch)
			{
				sql = "INSERT INTO {0} ({1}) VALUES ({2}); SELECT {3} FROM {0} WHERE ({4} = " + provider.GetLastInsertedId(this.tableName, this.autoIncrementColumn.Name) + ")";
                sql = string.Format(sql, qualifiedTableName, this.fieldList, this.namedParamList, selectFieldList, this.autoIncrementColumn.Name);
				this.insertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
			}
			else
			{
				sql = "INSERT INTO {0} ({1}) VALUES ({2})";
				sql = string.Format(sql, qualifiedTableName, this.fieldList, this.namedParamList);
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

		/// <summary>
		/// Row update handler for providers that require Row Update Handling
		/// </summary>
		/// <param name="row"></param>
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

			cmd.CommandText = provider.GetLastInsertedId(this.tableName, this.autoIncrementColumn.Name);
			DumpBatch(cmd.CommandText);

			using (IDataReader reader = cmd.ExecuteReader())
			{
				if (reader.Read())
				{
					object oidValue = reader.GetValue(0);
					if ( oidValue == DBNull.Value )
						LogIfVerbose( oidColumnName + " = DbNull" );
					else
						LogIfVerbose( oidColumnName + " = " + oidValue );

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

			sql = @"UPDATE {0} SET {1} WHERE ({2})";

		
			//{0} = Tabellenname: Mitarbeiter
			//{1} = Zuweisungsliste: vorname = @vorname, nachname = @nachname 
			//{2} = Where-Bedingung: id = @Original_id [ AND TimeStamp = @Original_timestamp ]
			AssignmentGenerator assignmentGenerator = new AssignmentGenerator(this.classMapping);
			string zuwListe = assignmentGenerator.Result;

			foreach (var e in this.persistentFields)
			{
				Type memberType;
				if (e.Value is FieldInfo)
					memberType = ((FieldInfo)e.Value).FieldType;
				else
					memberType = ((PropertyInfo)e.Value).PropertyType;

				fieldMapping = classMapping.FindField( (string)e.Key );
				if (fieldMapping != null)
				{
					provider.AddParameter( updateCommand, provider.GetNamedParameter( "U_" + fieldMapping.Column.Name ), fieldMapping.ColumnDbType, ParameterLength( fieldMapping, memberType ), fieldMapping.Column.Name );
				}
			}

			foreach (RelationFieldInfo ri in relationInfos)
			{
				Relation r = ri.Rel;
				if (r.Multiplicity == RelationMultiplicity.Element && r.MappingTable == null
					|| r.Multiplicity == RelationMultiplicity.List && r.MappingTable == null && r.Parent.FullName != classMapping.FullName)
				{
					foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
					{
						Type systemType = fkColumn.SystemType;
						provider.AddParameter( updateCommand, provider.GetNamedParameter( "U_" + fkColumn.Name ), provider.GetDbType( systemType ), provider.GetDefaultLength( systemType ), fkColumn.Name );
					}
					if (r.ForeignKeyTypeColumnName != null)
					{
						provider.AddParameter( updateCommand, provider.GetNamedParameter( "U_" + r.ForeignKeyTypeColumnName ), provider.GetDbType( typeof( int ) ), provider.GetDefaultLength( typeof( int ) ), r.ForeignKeyTypeColumnName );
					}
				}
			}

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

			sql = string.Format(sql, qualifiedTableName, zuwListe, where);
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
		/// Initializes the PersistenceHandler
		/// </summary>
		/// <param name="ndoMapping">Mapping information.</param>
		/// <param name="t">Type for which the Handler is constructed.</param>
		/// <param name="disposeCallback">Method to be called at the end of the usage. The method can be used to push back the object to the PersistenceHandlerPool.</param>
		public void Initialize(NDOMapping ndoMapping, Type t, Action<Type,IPersistenceHandler> disposeCallback)
		{
			this.ndoMapping = ndoMapping;
			this.classMapping = ndoMapping.FindClass(t);
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
			Connection connInfo = ndoMapping.FindConnection(classMapping);
			this.provider = ndoMapping.GetProvider(connInfo);
			this.qualifiedTableName = provider.GetQualifiedTableName( tableName );
			// The connection object will be initialized by the pm, to 
			// enable connection string housekeeping.
			// CheckTransaction is the place, where this happens.
			this.conn = null;

			var columnListGenerator = SqlColumnListGenerator.Get( classMapping );	
			this.hollowFields = columnListGenerator.HollowFields;
			this.hollowFieldsWithAlias = columnListGenerator.HollowFieldsWithAlias;
			this.namedParamList = columnListGenerator.ParamList;
			this.fieldList = columnListGenerator.BaseList;
			this.selectFieldList = columnListGenerator.SelectList;
			this.selectFieldListWithAlias = columnListGenerator.SelectListWithAlias;
			this.guidlength = provider.GetDefaultLength(typeof(Guid));
            if (this.guidlength == 0)
                this.guidlength = provider.SupportsNativeGuidType ? 16 : 36;
			this.disposeCallback = disposeCallback;


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
				.CollectRelations().ToList(); 


			GenerateSelectCommand();
			GenerateInsertCommand();
			GenerateUpdateCommand();
			GenerateDeleteCommand();
		}
	
		#region Implementation of IPersistenceHandler

		/// <summary>
		/// Saves Changes to a DataTable
		/// </summary>
		/// <param name="dt"></param>
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
						foreach (DataRow r in rows)
						{
							if (r.RowState == DataRowState.Added ||
								r.RowState == DataRowState.Modified)
							{
								dbex.Row = r;
								break;
							}
						}
					}
					ConcurrencyError( dbex );
				}
				else
				{
					throw;
				}
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
			LogIfVerbose( "Batch: \r\n" + sql );
		}

		private void Dump(DataRow[] rows)
		{
			new SqlDumper(this.loggerFactory, this.provider, insertCommand, selectCommand, updateCommand, deleteCommand).Dump(rows);
		}

        DataRow[] Select(DataTable dt, DataViewRowState rowState)
        {
			// Mono Hack: Some rows in Mono are null after Select.
			DataRow[] rows = dt.Select(null, null, rowState).Where(dr=>dr != null).ToArray();
            return rows;
        }

		/// <summary>
		/// Delets all rows of a DataTable marked as deleted
		/// </summary>
		/// <param name="dt"></param>
        public void UpdateDeletedObjects(DataTable dt)
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
					throw;
			}
			catch (System.Exception ex)
			{
				string text = "Exception of type " + ex.GetType().Name + " while deleting data rows: " + ex.Message + "\n";
				text += "Sql statement: " + deleteCommand.CommandText + "\n";
				throw new NDOException(38, text);
			}
		}

		private DataTable GetTemplateTable(DataSet templateDataset, string name)
		{
			// The instance of ds is actually static,
			// since the SqlPersistenceHandler lives as
			// a static instance in the PersistenceHandlerCache.
			DataTable dt = templateDataset.Tables[name];
			if (dt == null)
				throw new NDOException(39, "Can't find table '" + name + "' in the schema. Check your mapping file.");
			return dt;
		}

		/// <summary>
		/// Executes a batch of sql statements.
		/// </summary>
		/// <param name="statements">Each element in the array is a sql statement.</param>
		/// <param name="parameters">A list of parameters (see remarks).</param>
		/// <returns>An List of Hashtables, containing the Name/Value pairs of the results.</returns>
		/// <remarks>
		/// For emty resultsets an empty Hashtable will be returned. 
		/// If parameters is a NDOParameterCollection, the parameters in the collection are valid for 
		/// all subqueries. If parameters is an ordinary IList, NDO expects to find a NDOParameterCollection 
		/// for each subquery. If an element is null, no parameters are submitted for the given query.
		/// </remarks>
		public IList<Dictionary<string, object>> ExecuteBatch( string[] statements, IList parameters )
		{
			List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
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
					IDbCommand cmd = this.provider.NewSqlCommand( conn );
					sql = this.provider.GenerateBulkCommand( statements );
					cmd.CommandText = sql;
					if (parameters != null && parameters.Count > 0)
					{
						// Only the first command gets parameters
						for (i = 0; i < statements.Length; i++)
						{
							if (i == 0)
								CreateQueryParameters( cmd, parameters );
							else
								CreateQueryParameters( null, null );
						}
					}

					// cmd.CommandText can be changed in CreateQueryParameters
					DumpBatch( cmd.CommandText );
					if (this.transaction != null)
						cmd.Transaction = this.transaction;

					dr = cmd.ExecuteReader();

					for (; ; )
					{
						var dict = new Dictionary<string, object>();
						while (dr.Read())
						{
							for (i = 0; i < dr.FieldCount; i++)
							{
								dict.Add( dr.GetName( i ), dr.GetValue( i ) );
							}
						}
						result.Add( dict );
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
						var dict = new Dictionary<string, object>();
						IDbCommand cmd = this.provider.NewSqlCommand( conn );

						cmd.CommandText = s;
						if (parameters != null && parameters.Count > 0)
						{
							CreateQueryParameters( cmd, parameters );
						}

						if (this.transaction != null)
							cmd.Transaction = this.transaction;

						dr = cmd.ExecuteReader();

						while (dr.Read())
						{
							for (int j = 0; j < dr.FieldCount; j++)
							{
								dict.Add( dr.GetName( j ), dr.GetValue( j ) );
							}
						}

						dr.Close();
						result.Add( dict );
					}

					DumpBatch( sql );
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
			if (parameters == null || parameters.Count == 0)
				return;

			string sql = command.CommandText;

			Regex regex = new Regex( @"\{(\d+)\}" );

			MatchCollection matches = regex.Matches( sql );
			Dictionary<string, object> tcValues = new Dictionary<string, object>();
			int endIndex = parameters.Count - 1;
			foreach (Match match in matches)
			{
				int nr = int.Parse( match.Groups[1].Value );
				if (nr > endIndex)
					throw new QueryException( 10009, "Parameter-Reference " + match.Value + " has no matching parameter." );

				sql = sql.Replace( match.Value,
					this.provider.GetNamedParameter( "p" + nr.ToString() ) );
			}

			command.CommandText = sql;

			for (int i = 0; i < parameters.Count; i++)
			{
				object p = parameters[i];
				if (p == null)
					p = DBNull.Value;
				Type type = p.GetType();
                if (type.FullName.StartsWith("System.Nullable`1"))
                    type = type.GetGenericArguments()[0];
				if (type == typeof( Guid ) && Guid.Empty.Equals( p ) || type == typeof( DateTime ) && DateTime.MinValue.Equals( p ))
				{
					p = DBNull.Value;
				}
                if (type.IsEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                    p = ((IConvertible)p ).ToType(type, CultureInfo.CurrentCulture);
                }
				else if (type == typeof(Guid) && !provider.SupportsNativeGuidType)
				{
					type = typeof(string);
					if (p != DBNull.Value)
						p = p.ToString();
				}
				string name = "p" + i.ToString();
				int length = this.provider.GetDefaultLength(type);
				if (type == typeof(string))
				{
					length = ((string)p).Length;
					if (provider.GetType().Name.IndexOf("Oracle") > -1)
					{
						if (length == 0)
							throw new QueryException(10001, "Empty string parameters are not allowed in Oracle. Use IS NULL instead.");
					}
				}
				else if (type == typeof(byte[]))
				{
					length = ((byte[])p).Length;
				}
				IDataParameter par = provider.AddParameter(
					command, 
					this.provider.GetNamedParameter(name),
					this.provider.GetDbType( type == typeof( DBNull ) ? typeof( string ) : type ),
					length, 
					this.provider.GetQuotedName(name)); 
				par.Value = p;
				par.Direction = ParameterDirection.Input;					
			}
		}

		/// <summary>
		/// Performs a query and returns a DataTable
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <param name="templateDataSet"></param>
		/// <returns></returns>
		public DataTable PerformQuery( string sql, IList parameters, DataSet templateDataSet )
		{
			if (sql.Trim().StartsWith( "EXEC", StringComparison.InvariantCultureIgnoreCase ))
				this.selectCommand.CommandType = CommandType.StoredProcedure;
			else
				this.selectCommand.CommandType = CommandType.Text;

			DataTable table = GetTemplateTable(templateDataSet, this.tableName).Clone();

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
			IMappingTableHandler handler;
			if (!mappingTableHandlers.TryGetValue( r.FieldName, out handler ))
			{
				handler = new NDOMappingTableHandler( this.loggerFactory );
				handler.Initialize(ndoMapping, r);
				mappingTableHandlers[r.FieldName] = handler;
			}
			return handler;
		}

		/// <summary>
		/// Disposes a SqlPersistenceHandler
		/// </summary>
		public void Dispose()
		{
			this.disposeCallback( this.type, this );
		}

		/// <summary>
		/// Gets or sets the connection to be used for the handler
		/// </summary>
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

		/// <summary>
		/// Gets or sets the connection to be used for the handler
		/// </summary>
		public IDbTransaction Transaction
		{
			get { return this.transaction; }
			set
			{
				this.transaction = value;
				this.selectCommand.Transaction = value;
				this.deleteCommand.Transaction = value;
				this.updateCommand.Transaction = value;
				this.insertCommand.Transaction = value;
			}
		}

		void LogIfVerbose(string msg)
		{
			if (this.logger != null && this.logger.IsEnabled( LogLevel.Debug ))
				this.logger.LogDebug( msg );
		}

		/// <summary>
		/// Gets the current DataAdapter.
		/// </summary>
		/// <remarks>
		/// This is needed by RegisterRowUpdateHandler.
		/// See the comment in SqlCeProvider.
		/// </remarks>
		public DbDataAdapter DataAdapter => this.dataAdapter;

		#endregion
	}
}
