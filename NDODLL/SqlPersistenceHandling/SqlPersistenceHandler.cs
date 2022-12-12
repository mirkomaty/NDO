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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using NDO;
using NDO.Mapping;
using NDO.Logging;
using NDOInterfaces;
using NDO.Query;
using System.Globalization;
using NDO.Configuration;
using System.Threading.Tasks;
using System.Threading;

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

		private DbCommand selectCommand;
		private DbCommand insertCommand;
		private DbCommand updateCommand;
		private DbCommand deleteCommand;
		private DbConnection conn;
		private DbTransaction transaction;
		private DbDataAdapter dataAdapter;
		private Class classMapping;
		private string selectFieldList;
		private string selectFieldListWithAlias;
		private string tableName;
		private string qualifiedTableName;
		private bool verboseMode;
		private ILogAdapter logAdapter;
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
		private readonly INDOContainer configContainer;
		private Action<Type,IPersistenceHandler> disposeCallback;
		private SqlSelectBehavior sqlSelectBahavior = new SqlSelectBehavior();
		private static Regex parameterRegex = new Regex( @"\{(\d+)\}", RegexOptions.Compiled );


		/// <summary>
		/// Constructs a SqlPersistenceHandler object
		/// </summary>
		/// <param name="configContainer"></param>
		public SqlPersistenceHandler(INDOContainer configContainer)
		{
			this.configContainer = configContainer;
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
			int i = 0;
            foreach(var oidColumn in this.classMapping.Oid.OidColumns)
			{
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

				i++;
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
				foreach(var mth in this.mappingTableHandlers.Values)
				{
					mth.VerboseMode = value;
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
				foreach(var mth in this.mappingTableHandlers.Values)
				{
					mth.LogAdapter = value;
				}
			}
		}

		SqlColumnListGenerator CreateColumnListGenerator( Class cls )
		{
			var key = $"{nameof(SqlColumnListGenerator)}-{cls.FullName}";
			return configContainer.ResolveOrRegisterType<SqlColumnListGenerator>( new ContainerControlledLifetimeManager(), key, new ParameterOverride( "cls", cls ) );
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

			var columnListGenerator = CreateColumnListGenerator( classMapping );	
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


			this.selectCommand = (DbCommand) provider.NewSqlCommand(conn);
			this.insertCommand = (DbCommand) provider.NewSqlCommand(conn);
			this.updateCommand = (DbCommand) provider.NewSqlCommand(conn);
			this.deleteCommand = (DbCommand) provider.NewSqlCommand(conn);
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
		public void UpdateAsync(DataTable dt)
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
			// Mono Hack: Some rows in Mono are null after Select.
			DataRow[] rows = dt.Select(null, null, rowState).Where(dr=>dr != null).ToArray();
            return rows;
        }

		/// <summary>
		/// Delets all rows of a DataTable marked as deleted
		/// </summary>
		/// <param name="dt"></param>
        public async Task UpdateDeletedObjectsAsync(DataTable dt)
		{
			DataRow[] rows = Select(dt, DataViewRowState.Deleted);
			if (rows.Length == 0) return;
			var oidcolumns = this.classMapping.Oid.OidColumns.Select( c => c.Name );

			Dump( rows);
			try
			{
				var parameters = new List<object>();
				var statements = new List<string>();
				foreach (var row in rows)
				{
					statements.Add( this.deleteCommand.CommandText );
					foreach (var col in oidcolumns)
					{
						parameters.Add( row[col] );
					}

					if (this.classMapping.TimeStampColumn != null)
						parameters.Add( row[this.classMapping.TimeStampColumn] );
				}

				await ExecuteBatchAsync( statements, parameters ).ConfigureAwait( false );
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

		private DataTable GetTemplateTable(DataSet templateDataset, string name)
		{
			// The instance of templateDataset is actually static,
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
		/// <param name="inputStatements">Each element in the array is a sql statement.</param>
		/// <param name="parameters">A list of parameters (see remarks).</param>
		/// <param name="isCommandArray">Determines, if statements contains identical commands which all need parameters</param>
		/// <returns>An List of Hashtables, containing the Name/Value pairs of the results.</returns>
		/// <remarks>
		/// For emty resultsets an empty dictionary will be returned. 
		/// If we have no command array, the parameters in the collection are for 
		/// all subqueries. In case of a command array the statements will bei combined to a template. 
		/// parameters contains a list of lists, with one entry per repetition of the template.
		/// </remarks>
		public async Task<IList<Dictionary<string, object>>> ExecuteBatchAsync( IEnumerable<string> inputStatements, IList parameters, bool isCommandArray = false )
		{
/*
			These are examples of the two cases:

			-------------- Non Array

			INSERT INTO x (Col1, Col2) VALUES({0},{1})
			SELECT SCOPE_IDENTITY()

			Parameter 'abc', 4711

			=>

			Insert x values(@p0,@p1);
			Select SCOPE_IDENTITY();

			@p0 = 'abc'
			@p1 = 4711

			------------- Array

			DELETE FROM x WHERE id1 = {0} AND tstamp = {1}

			Parameter
			12, '927D81A1-07AA-477B-9EA9-73F7C19E754F'
			33, '9061A0EA-BB02-47DB-866F-04210546E493'
			44, '2B9D2573-226E-4486-AEE7-A3DEFCEB48FC'
			45, '3B9D2573-226E-4486-AEE7-A3DEFCEB48FD'
			46, '4B9D2573-226E-4486-AEE7-A3DEFCEB48FE'

			=>

			DELETE FROM x WHERE id1 = @p0 AND tstamp = @p1;
			DELETE FROM x WHERE id1 = @p2 AND tstamp = @p3;
			DELETE FROM x WHERE id1 = @p4 AND tstamp = @p5;
			DELETE FROM x WHERE id1 = @p6 AND tstamp = @p7;
			DELETE FROM x WHERE id1 = @p8 AND tstamp = @p9;

			@p0 = 12
			@p1 = '927D81A1-07AA-477B-9EA9-73F7C19E754F'
			@p2 = 33
			@p3 = '9061A0EA-BB02-47DB-866F-04210546E493'
			@p4 = 44, 
			@p5 = '2B9D2573-226E-4486-AEE7-A3DEFCEB48FC'
			@p6 = 45
			@p7 = '3B9D2573-226E-4486-AEE7-A3DEFCEB48FD'
			@p8 = 46
			@p9 = '4B9D2573-226E-4486-AEE7-A3DEFCEB48FE'

*/
			List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
			bool closeIt = false;
			DbDataReader dr = null;
			int i;
			try
			{
				if (this.conn.State != ConnectionState.Open)
				{
					closeIt = true;
					this.conn.Open();
				}

				string sql = string.Empty;
				var rearrangedStatements = new List<string>();

				var dict = new Dictionary<string, object>();


				if (this.provider.SupportsBulkCommands)
				{
					// cast is necessary here to get access to async methods
					DbCommand cmd = (DbCommand)this.provider.NewSqlCommand( conn );

					if (this.transaction != null)
						cmd.Transaction = this.transaction;

					if (parameters != null && parameters.Count > 0)
					{
						CreateQueryParameters( cmd, inputStatements, rearrangedStatements, parameters, isCommandArray );
					}
					else
					{
						rearrangedStatements = inputStatements.ToList();
					}

					sql = this.provider.GenerateBulkCommand( rearrangedStatements.ToArray() );
					cmd.CommandText = sql;

					// cmd.CommandText can be changed in CreateQueryParameters
					DumpBatch( cmd.CommandText );

					using (dr = await cmd.ExecuteReaderAsync().ConfigureAwait( false )) 
					{
						do
						{
							while (await dr.ReadAsync().ConfigureAwait( false ))
							{
								for (i = 0; i < dr.FieldCount; i++)
								{
									// GetFieldValueAsync uses just Task.FromResult(),
									// so we don't gain anything with an async call here.
									dict.Add( dr.GetName( i ), dr.GetValue( i ) );  
								}
							}

							result.Add( dict );

						} while (await dr.NextResultAsync().ConfigureAwait( false ));
					}
				}
				else
				{
					if (isCommandArray)
					{
						// A command array always has parameter sets, otherwise it wouldn't make any sense
						// to work with a command array
						foreach (IList parameterSet in parameters)
						{
							result.AddRange( await ExecuteStatementSetAsync( inputStatements, rearrangedStatements, parameterSet ).ConfigureAwait( false ) );
						}
					}
					else
					{
						result.AddRange( await ExecuteStatementSetAsync( inputStatements, rearrangedStatements, parameters ).ConfigureAwait( false ) );
					}
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

		private async Task<List<Dictionary<string, object>>> ExecuteStatementSetAsync( IEnumerable<string> inputStatements, List<string> rearrangedStatements, IList parameterSet )
		{
			var result = new List<Dictionary<string, object>>();

			// cast is necessary here to get access to async methods
			DbCommand cmd = (DbCommand)this.provider.NewSqlCommand( conn );

			if (this.transaction != null)
				cmd.Transaction = this.transaction;

			rearrangedStatements.Clear();
			// we repeat rearranging for each parameterSet, just as if it were an ordinary Bulk statement set
			CreateQueryParameters( cmd, inputStatements, rearrangedStatements, parameterSet, false );
			foreach (var statement in rearrangedStatements)
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();

				using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait( false ))
				{

					while (await dr.ReadAsync().ConfigureAwait( false ))
					{
						for (int j = 0; j < dr.FieldCount; j++)
						{
							dict.Add( dr.GetName( j ), dr.GetValue( j ) );
						}
					}
				}

				result.Add( dict );
			}

			DumpBatch( String.Join( "\r\n;", rearrangedStatements ) );
			return result;
		}

		private void CreateQueryParameters( DbCommand command, IEnumerable<string> inputStatements, List<string> rearrangedStatements, IList parameters, bool isCommandArray )
		{
#warning TODO We need test cases for this method

			var maxindex = -1;

			foreach (var statement in inputStatements)
			{
				var matches = parameterRegex.Matches(statement);
				foreach (Match match in matches)
				{
					var i = int.Parse( match.Groups[1].Value );
					maxindex = Math.Max( i, maxindex );
				}
			}

			// No parameters in the statements? Just return the statements
			if (maxindex == -1)
			{
				rearrangedStatements.AddRange( inputStatements );
				return;
			}

			if (isCommandArray)
			{
				var index = 0;

				foreach (IList parameterSet in parameters)
				{
					if (parameterSet.Count <= maxindex)
						throw new QueryException( 10009, $"Parameter-Reference {maxindex} has no matching parameter." );

					// inputStatements contains kind-of a template
					// which will be repeated for each parameterSet
					foreach (var statement in inputStatements)
					{
						var rearrangedStatement = statement;
						var matches = parameterRegex.Matches(statement);
						foreach (Match match in matches)
						{
							var i = int.Parse( match.Groups[1].Value );
							var parName = this.provider.GetNamedParameter( $"p{(i + index)}" );
							rearrangedStatement = rearrangedStatement.Replace( match.Value, parName );
						}

						rearrangedStatements.Add( rearrangedStatement );
					}

					foreach (var par in parameterSet)
					{
						AddParameter( command, index++, par );
					}

					index += parameterSet.Count;
				}
			}
			else
			{
				foreach (var statement in inputStatements)
				{
					var rearrangedStatement = statement;

					MatchCollection matches = parameterRegex.Matches( statement );
					Dictionary<string, object> tcValues = new Dictionary<string, object>();
					if (maxindex > parameters.Count - 1)
						throw new QueryException( 10009, $"Parameter-Reference {maxindex} has no matching parameter." );

					foreach (Match match in matches)
					{
						var i = int.Parse( match.Groups[1].Value );
						var parName = this.provider.GetNamedParameter( $"p{i}" );
						rearrangedStatement = rearrangedStatement.Replace( match.Value, parName );
					}

					rearrangedStatements.Add( rearrangedStatement );
				}

				for (int i = 0; i < parameters.Count; i++)
				{
					AddParameter( command, i, parameters[i] );
				}
			}
		}

		private object AddParameter( DbCommand command, int index, object p )
		{
			if (p == null)
				p = DBNull.Value;
			Type type = p.GetType();
			if (type.FullName.StartsWith( "System.Nullable`1" ))
				type = type.GetGenericArguments()[0];
			if (type == typeof( Guid ) && Guid.Empty.Equals( p ) || type == typeof( DateTime ) && DateTime.MinValue.Equals( p ))
			{
				p = DBNull.Value;
			}
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType( type );
				p = ( (IConvertible) p ).ToType( type, CultureInfo.CurrentCulture );
			}
			else if (type == typeof( Guid ) && !provider.SupportsNativeGuidType)
			{
				type = typeof( string );
				if (p != DBNull.Value)
					p = p.ToString();
			}
			string name = "p" + index.ToString();
			int length = this.provider.GetDefaultLength(type);
			if (type == typeof( string ))
			{
				length = ( (string) p ).Length;
				if (provider.GetType().Name.IndexOf( "Oracle" ) > -1)
				{
					if (length == 0)
						throw new QueryException( 10001, "Empty string parameters are not allowed in Oracle. Use IS NULL instead." );
				}
			}
			else if (type == typeof( byte[] ))
			{
				length = ( (byte[]) p ).Length;
			}
			IDataParameter par = provider.AddParameter(
					command,
					this.provider.GetNamedParameter(name),
					this.provider.GetDbType( type == typeof( DBNull ) ? typeof( string ) : type ),
					length,
					this.provider.GetQuotedName(name));
			par.Value = p;
			par.Direction = ParameterDirection.Input;
			return p;
		}

		/// <inheritdoc/>
		public async Task<DataTable> PerformQueryAsync( string sql, IList parameters, DataSet templateDataSet )
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
				await this.sqlSelectBahavior.Select( this.selectCommand, table );
			}
            catch (System.Exception ex)
            {
                string text = "Exception of type " + ex.GetType().Name + " while executing a Query: " + ex.Message + "\n";
                text += "Sql Statement: " + sql + "\n";
                throw new NDOException(40, text);
            }

			return table;		
		}

		/// <inheritdoc/>
		public DataTable PerformQuery( string sql, IList parameters, DataSet templateDataSet )
		{
			if (sql.Trim().StartsWith( "EXEC", StringComparison.InvariantCultureIgnoreCase ))
				this.selectCommand.CommandType = CommandType.StoredProcedure;
			else
				this.selectCommand.CommandType = CommandType.Text;

			DataTable table = GetTemplateTable(templateDataSet, this.tableName).Clone();

			this.selectCommand.CommandText = sql;

			CreateQueryParameters( this.selectCommand, parameters );

			Dump( null ); // Dumps the Select Command

			try
			{
				dataAdapter.Fill( table );
			}
			catch (System.Exception ex)
			{
				string text = "Exception of type " + ex.GetType().Name + " while executing a Query: " + ex.Message + "\n";
				text += "Sql Statement: " + sql + "\n";
				throw new NDOException( 40, text );
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
				handler = new NDOMappingTableHandler();
				handler.Initialize(ndoMapping, r);
				handler.VerboseMode = this.verboseMode;
				handler.LogAdapter = this.logAdapter;
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
#warning Das Property muss den Typ DbConnection bekommen
				this.conn = (DbConnection)value;
				this.selectCommand.Connection = this.conn;
				this.deleteCommand.Connection = this.conn;
				this.updateCommand.Connection = this.conn;
				this.insertCommand.Connection = this.conn;
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
				this.transaction = (DbTransaction)value;
				this.selectCommand.Transaction = this.transaction;
				this.deleteCommand.Transaction = this.transaction;
				this.updateCommand.Transaction = this.transaction;
				this.insertCommand.Transaction = this.transaction;
			}
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
