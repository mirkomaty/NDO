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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using NDO.Mapping;
using NDO.Logging;
using NDOInterfaces;
using NDO.Configuration;
using System.Threading.Tasks;

namespace NDO.SqlPersistenceHandling
{
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

		private List<string> insertCommands = new List<string>();
		private List<string> updateCommands = new List<string>();
		private List<string> deleteCommands = new List<string>();
		private List<DbParameterInfo> insertParameterInfos = new List<DbParameterInfo>();
		private List<DbParameterInfo> updateParameterInfos = new List<DbParameterInfo>();
		private List<DbParameterInfo> deleteParameterInfos = new List<DbParameterInfo>();
		private DbConnection connection;
		private DbTransaction transaction;
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
		private SqlDumper sqlDumper;


		/// <summary>
		/// Constructs a SqlPersistenceHandler object
		/// </summary>
		/// <param name="configContainer"></param>
		public SqlPersistenceHandler(INDOContainer configContainer)
		{
			this.configContainer = configContainer;
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
			this.insertParameterInfos.Clear();
			this.insertCommands.Clear();

			// Generate Parameters
			foreach (OidColumn oidColumn in this.classMapping.Oid.OidColumns)
			{
				if (!oidColumn.AutoIncremented && oidColumn.FieldName == null && oidColumn.RelationName == null)
				{
					insertParameterInfos.Add( new DbParameterInfo( oidColumn.Name, provider.GetDbType( oidColumn.SystemType ), provider.GetDefaultLength( oidColumn.SystemType ), false ));
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

				insertParameterInfos.Add( new DbParameterInfo( fieldMapping.Column.Name, fieldMapping.ColumnDbType, ParameterLength( fieldMapping, memberType ), true ) );
			}

			foreach (RelationFieldInfo ri in relationInfos)
			{
				Relation r = ri.Rel;
				foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
				{
					insertParameterInfos.Add( new DbParameterInfo( fkColumn.Name, provider.GetDbType( fkColumn.SystemType ), provider.GetDefaultLength( fkColumn.SystemType ), true ) );
				}
				if (r.ForeignKeyTypeColumnName != null)
				{
					insertParameterInfos.Add( new DbParameterInfo( r.ForeignKeyTypeColumnName, provider.GetDbType( typeof( int ) ), provider.GetDefaultLength( typeof( int ) ), true ) );
				}

			}

			if (this.timeStampColumn != null)
			{
				insertParameterInfos.Add( new DbParameterInfo( timeStampColumn, provider.GetDbType( typeof( Guid ) ), guidlength, false ) );
			}

			if (this.typeNameColumn != null)
			{
				Type tncType = Type.GetType( this.typeNameColumn.NetType );
				insertParameterInfos.Add( new DbParameterInfo( this.typeNameColumn.Name, provider.GetDbType( tncType ), provider.GetDefaultLength( tncType ), false ) );
			}

			if (hasAutoincrementedColumn)
			{
				if (provider.SupportsLastInsertedId)
				{
					//These statements are executed as a batch if supported by the provider, otherwise they are executed individually.
					this.insertCommands.Add( $"INSERT INTO {qualifiedTableName} ({this.fieldList}) VALUES ({this.namedParamList})" );
					this.insertCommands.Add( $"SELECT {provider.GetLastInsertedId( this.tableName, this.autoIncrementColumn.Name )} AS NdoInsertedId" );
				}
				else
				{
					if (!provider.SupportsLastInsertedId)
						throw new NDOException( 32, "The provider of type " + provider.GetType().FullName + " doesn't support Autonumbered Ids. Use Self generated Ids instead." );
				}
			}
			else
			{
				this.insertCommands.Add( $"INSERT INTO {qualifiedTableName} ({this.fieldList}) VALUES ({this.namedParamList})" );				
			}
		}

		private void GenerateUpdateCommand()
		{
			this.updateParameterInfos.Clear();
			this.updateCommands.Clear();
			NDO.Mapping.Field fieldMapping;
		
			//{0} = Tabellenname: Mitarbeiter
			//{1} = Zuweisungsliste: vorname = @vorname, nachname = @nachname 
			//{2} = Where-Bedingung: id = @Original_id [ AND TimeStamp = @Original_timestamp ]
			AssignmentGenerator assignmentGenerator = new AssignmentGenerator(this.classMapping);
			string zuwListe = assignmentGenerator.Result;
			int pindex = 0;

			foreach (var e in this.persistentFields)
			{
				Type memberType;
				if (e.Value is FieldInfo)
					memberType = ((FieldInfo)e.Value).FieldType;
				else
					memberType = ((PropertyInfo)e.Value).PropertyType;

				fieldMapping = classMapping.FindField( e.Key );
				if (fieldMapping != null)
				{
					this.updateParameterInfos.Add(new DbParameterInfo( fieldMapping.Column.Name, fieldMapping.ColumnDbType, ParameterLength( fieldMapping, memberType ), true ) );
					pindex++;
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
						this.updateParameterInfos.Add( new DbParameterInfo( fkColumn.Name, provider.GetDbType( systemType ), provider.GetDefaultLength( systemType ), true ) );
						pindex++;
					}
					if (r.ForeignKeyTypeColumnName != null)
					{
						this.updateParameterInfos.Add( new DbParameterInfo( r.ForeignKeyTypeColumnName, provider.GetDbType( typeof(int) ), provider.GetDefaultLength( typeof(int) ), true ) );
						pindex++;
					}
				}
			}

			string where = string.Empty;
			int whereStartIndex;

			if (this.timeStampColumn != null)
			{
				// The new timestamp value as parameter
				this.updateParameterInfos.Add( new DbParameterInfo( timeStampColumn, provider.GetDbType( typeof( Guid ) ), guidlength, false ) );
				pindex++;
				whereStartIndex = pindex;

				// This is the first WHERE parameter. It's the original time stamp value
				if (provider.UseNamedParams)
					where += provider.GetQuotedName(timeStampColumn) + " = {" + pindex + "}" + " AND ";
				else
					where += provider.GetQuotedName(timeStampColumn) + " = ? AND ";

				this.updateParameterInfos.Add( new DbParameterInfo( timeStampColumn, provider.GetDbType(typeof(Guid)), guidlength, false ) );
				pindex++;
			}

			int oidCount = classMapping.Oid.OidColumns.Count;
			for (int i = 0; i < oidCount; i++)
			{
                OidColumn oidColumn = (OidColumn)classMapping.Oid.OidColumns[i];
				// Oid as parameter
				this.updateParameterInfos.Add( new DbParameterInfo( oidColumn.Name, provider.GetDbType(oidColumn.SystemType), oidColumn.TypeLength, false ) );

				if (provider.UseNamedParams)
					where += provider.GetQuotedName(oidColumn.Name) + " = {" + pindex + "}";
				else
					where += provider.GetQuotedName(oidColumn.Name) + " = ?";

				pindex++;

				Relation r = oidColumn.Relation;
                if (!this.hasGuidOid && r != null && r.ForeignKeyTypeColumnName != null)
                {
                    where += " AND " + provider.GetQuotedName(r.ForeignKeyTypeColumnName) + " = {" + pindex + "}";
					this.updateParameterInfos.Add( new DbParameterInfo( r.ForeignKeyTypeColumnName, provider.GetDbType(typeof(int)), provider.GetDefaultLength(typeof(int)), false ) );
					pindex++;
				}

				if (i < oidCount - 1)
                    where += " AND ";
			}

			string sql;
			if (this.timeStampColumn != null)
			{
				sql = $"SELECT COUNT(*) FROM {qualifiedTableName} WHERE ({where})";
				this.updateCommands.Add( sql );
			}

			sql = $"UPDATE {qualifiedTableName} SET {zuwListe} WHERE ({where})";
			//Console.WriteLine(sql);
			this.updateCommands.Add(sql);
		}


		private void GenerateDeleteCommand()
		{
			this.deleteParameterInfos.Clear();
			this.deleteCommands.Clear();

			var whereBuilder = new StringBuilder();

            int oidCount = this.classMapping.Oid.OidColumns.Count;
			int i = 0;
            foreach(var oidColumn in this.classMapping.Oid.OidColumns)
			{
				if (provider.UseNamedParams)
					whereBuilder.Append( provider.GetQuotedName( oidColumn.Name ) + " = {" + i++ + "}" );
				else
					whereBuilder.Append( provider.GetQuotedName( oidColumn.Name ) + " = ?" );

				deleteParameterInfos.Add( new DbParameterInfo( oidColumn.Name, provider.GetDbType( oidColumn.SystemType ), oidColumn.TypeLength, false ) );

                Relation r = oidColumn.Relation;
                if (!this.hasGuidOid && r != null && r.ForeignKeyTypeColumnName != null)
                {
					whereBuilder.Append( " AND " +
						provider.GetQuotedName( r.ForeignKeyTypeColumnName ) + " = {" + i++ + "}" );

					deleteParameterInfos.Add( new DbParameterInfo( r.ForeignKeyTypeColumnName, provider.GetDbType( typeof(int) ), provider.GetDefaultLength( typeof(int) ), false ) );
				}

				if (i < oidCount - 1)
					whereBuilder.Append( " AND " );
			}

			if (this.timeStampColumn != null)
			{
				if (provider.UseNamedParams)
					whereBuilder.Append( " AND " + provider.GetQuotedName( timeStampColumn ) + " = {" + i + "}" );
				else
					whereBuilder.Append( " AND " + provider.GetQuotedName( timeStampColumn ) + " = ?" );

				deleteParameterInfos.Add( new DbParameterInfo( timeStampColumn, provider.GetDbType( typeof( Guid ) ), guidlength, false ) );
			}

			string where = whereBuilder.ToString();

			if (this.timeStampColumn != null) // we need collision detection
				this.deleteCommands.Add( $"SELECT COUNT (*) FROM {qualifiedTableName} AS affected WHERE ({where})" );

			this.deleteCommands.Add( $"DELETE FROM {qualifiedTableName} WHERE ({where})" );
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

				if (value != null)
					this.sqlDumper = new SqlDumper( value, this.provider );
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
			this.connection = null;

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

			this.type = t;

			CollectFields();	// Alle Feldinformationen landen in persistentField
			// determine the relations, which have a foreign key
			// column in the table of the given class
			relationInfos = new RelationCollector(this.classMapping)
				.CollectRelations().ToList(); 

			GenerateInsertCommand();
			GenerateUpdateCommand();
			GenerateDeleteCommand();
		}

		async Task InsertAsync( DataRow[] rows)
		{
			try
			{
				Log( "InsertAsync:" );
				var parameters = new List<object>();
				foreach (var row in rows)
				{
					var parameterSet = new List<object>();
					foreach (var info in this.insertParameterInfos)
					{
						parameterSet.Add( row[info.ColumnName] );
					}

					parameters.Add( parameterSet );
				}

				var results = await ExecuteBatchAsync( this.insertCommands, parameters, insertParameterInfos, true ).ConfigureAwait( false );
				Dump( rows, null, null );

				if (this.hasAutoincrementedColumn) // we need to retrieve the IDs
				{
					var ids = results.Where(d=>d.Values.Any()).Select( d => (int) d.Values.First() );
					if (ids.Count() != rows.Length)
					{
						Log( $"Concurrency failure: row count: {rows.Length}, affected: {ids.Count()}" );
						var ex = new DBConcurrencyException( "Concurrency failure: wasn't able to insert one or more rows.", null, rows );
						if (this.ConcurrencyError != null)
							this.ConcurrencyError( ex );
						else
							throw ex;
					}
					else
					{
						// We assume that the order of the IDs is the same as the order of the queries,
						// which is the same as the order of the rows.
						var autoColumn = this.classMapping.Oid.OidColumns.FirstOrDefault( c => c.AutoIncremented );
						var columnName = autoColumn.Name;
						var enumerator = rows.GetEnumerator();
						foreach (var id in ids)
						{
							enumerator.MoveNext();
							var row = (DataRow)enumerator.Current;
							row[columnName] = id;
						}						
					}
				}
			}
			catch (Exception ex)
			{
				string text = "Exception of type " + ex.GetType().Name + " while updating or inserting data rows: " + ex.Message + "\n";
				if (( ex.Message.IndexOf( "Die Variable" ) > -1 && ex.Message.IndexOf( "muss deklariert" ) > -1 ) || ( ex.Message.IndexOf( "Variable" ) > -1 && ex.Message.IndexOf( "declared" ) > -1 ))
					text += "Check the field names in the mapping file.\n";
				text += "Sql Insert statement: " + String.Join( "; ", this.insertCommands );
				throw new NDOException( 37, text );
			}
		}

		async Task UpdateAsync( DataRow[] rows )
		{
			try
			{
				Log( "UpdateAsync:" );

				var parameters = new List<object>();
				foreach (var row in rows)
				{
					var parameterSet = new List<object>();
					foreach (var info in this.insertParameterInfos)
					{
						parameterSet.Add( row[info.ColumnName] );
					}

					parameters.Add( parameterSet );
				}

				var results = await ExecuteBatchAsync( this.updateCommands, parameters, updateParameterInfos, true ).ConfigureAwait( false );

				if (this.timeStampColumn != null) // we have to check for concurrency error
				{
					int sum = 0;
					results.Select( d => (int) d.Values.First() ).Select( c => sum += c );
					if (sum != rows.Length)
					{
						Log( $"Concurrency failure: row count: {rows.Length}, affected: {sum}" );
						var ex = new DBConcurrencyException( "Concurrency failure: wasn't able to delete one or more rows.", null, rows );
						if (this.ConcurrencyError != null)
							this.ConcurrencyError( ex );
						else
							throw ex;
					}
				}

				Dump( rows, null, null );
			}
			catch (Exception ex)
			{
				string text = "Exception of type " + ex.GetType().Name + " while updating or inserting data rows: " + ex.Message + "\n";
				if (( ex.Message.IndexOf( "Die Variable" ) > -1 && ex.Message.IndexOf( "muss deklariert" ) > -1 ) || ( ex.Message.IndexOf( "Variable" ) > -1 && ex.Message.IndexOf( "declared" ) > -1 ))
					text += "Check the field names in the mapping file.\n";
				text += "Sql Update statement: " + String.Join( "; ", this.updateCommands );
				throw new NDOException( 37, text );
			}
		}


		/// <summary>
		/// Saves Changes to a DataTable
		/// </summary>
		/// <param name="dt"></param>
		public async Task UpdateAsync(DataTable dt)
		{
			DataRow[] rows = null;
			if (this.timeStampColumn != null)
			{
				foreach (DataRow r in rows)
					r[timeStampColumn] = Guid.NewGuid(); ;
			}

			rows = Select( dt, DataViewRowState.Added );
			if (rows.Length > 0)
				await InsertAsync( rows ).ConfigureAwait( false );

			rows = Select( dt, DataViewRowState.ModifiedCurrent);
			if (rows.Length > 0)
				await UpdateAsync( rows ).ConfigureAwait( false );
			
#warning Error Handling should happen in UpdateAsync and InsertAsync
			//catch (System.Data.DBConcurrencyException dbex)
			//{
			//	if (this.ConcurrencyError != null)
			//	{
			//		// This is a Firebird Hack because Fb doesn't set the row
			//		if (dbex.Row == null)
			//		{
			//			foreach(DataRow r in rows)
			//			{
			//				if (r.RowState == DataRowState.Added ||
			//					r.RowState == DataRowState.Modified)
			//				{
			//					dbex.Row = r;
			//					break;
			//				}
			//			}
			//		}
			//		ConcurrencyError(dbex);
			//	}
			//	else
			//		throw dbex;
			//}
   //         catch (System.Exception ex)
   //         {
   //             string text = "Exception of type " + ex.GetType().Name + " while updating or inserting data rows: " + ex.Message + "\n";
   //             if ((ex.Message.IndexOf("Die Variable") > -1 && ex.Message.IndexOf("muss deklariert") > -1) || (ex.Message.IndexOf("Variable") > -1 && ex.Message.IndexOf("declared") > -1))
   //                 text += "Check the field names in the mapping file.\n";
   //             text += "Sql Update statement: " + updateCommands.CommandText + "\n";
   //             text += "Sql Insert statement: " + insertCommands.CommandText;
   //             throw new NDOException(37, text);
   //         }
		}

		private void Log(string msg)
		{
			if (!this.verboseMode || this.logAdapter == null)
				return;

			this.logAdapter.Info( msg );
		}

		private void Dump(DataRow[] rows, IDbCommand cmd, IEnumerable<string> batch)
		{
			if (!this.verboseMode || this.sqlDumper == null)
				return;

			this.sqlDumper.Dump( rows, cmd, batch );
		}

        DataRow[] Select(DataTable dt, DataViewRowState rowState)
        {
			// Mono Hack: Some rows in Mono are null after Select.
			DataRow[] rows = dt.Select(null, null, rowState).Where(dr=>dr != null).ToArray();
            return rows;
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

		public Task<IList<Dictionary<string, object>>> ExecuteBatchAsync( IEnumerable<string> inputStatements, IList parameters, IEnumerable<DbParameterInfo> parameterInfos = null, bool isCommandArray = false )
		{
			return new BatchExecutor( this.provider, this.connection, this.transaction, this.Dump )
				.ExecuteBatchAsync( inputStatements, parameters, parameterInfos, isCommandArray );
		}

		/// <summary>
		/// Delets all rows of a DataTable marked as deleted
		/// </summary>
		/// <param name="dt"></param>
		public async Task UpdateDeletedObjectsAsync(DataTable dt)
		{
			DataRow[] rows = Select(dt, DataViewRowState.Deleted);
			if (rows.Length == 0) return;

			try
			{
				Log( nameof( UpdateDeletedObjectsAsync ) );
				var parameters = new List<object>();
				foreach (var row in rows)
				{
					var parameterSet = new List<object>();
					foreach (var info in this.insertParameterInfos)
					{
						parameterSet.Add( row[info.ColumnName] );
					}

					parameters.Add( parameterSet );
				}

				var results = await ExecuteBatchAsync( this.deleteCommands, parameters, deleteParameterInfos, true ).ConfigureAwait( false );

				Dump( rows, null, null );

				if (this.timeStampColumn != null) // we have to check for concurrency error
				{
					int sum = 0;
					results.Select( d => (int) d.Values.First() ).Select(c => sum += c );
					if (sum < rows.Length)
					{
						Log( $"Concurrency failure: row count: {rows.Length}, affected: {sum}" );
						var ex = new DBConcurrencyException( "Concurrency failure: wasn't able to delete one or more rows.", null, rows );
						if (this.ConcurrencyError != null)
							this.ConcurrencyError( ex );
						else
							throw ex;
					}
				}
			}
			catch (System.Exception ex)
			{
				string text = $"Exception of type {ex.GetType().Name} while deleting data rows: {ex.Message}\n";
				text += $"Sql statement: {String.Join("; ", this.deleteCommands)}\n";
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


		/// <inheritdoc/>
		public async Task<DataTable> PerformQueryAsync( string sql, IList parameters, DataSet templateDataSet )
		{
			CommandType commandType;
			if (sql.Trim().StartsWith( "EXEC", StringComparison.InvariantCultureIgnoreCase ))
				commandType = CommandType.StoredProcedure;
			else
				commandType = CommandType.Text;

			DataTable table = GetTemplateTable(templateDataSet, this.tableName).Clone();

			var command = (DbCommand) this.provider.NewSqlCommand(this.connection);
			command.Transaction = this.transaction;
			command.CommandType = commandType;

			var rearrangedStatements = new List<string>();
			List<string> inputStatements = new List<string>()
			{
				sql
			};

			new BatchExecutor( this.provider, this.connection, this.transaction, Dump )
				.CreateQueryParameters( command, inputStatements, rearrangedStatements, parameters, null, false );

			// We know, that we only have one statement to perform
			command.CommandText = rearrangedStatements[0];

			Dump(null, command, rearrangedStatements); // Dumps the Select Command

            try
            {
				await this.sqlSelectBahavior.Select( command, table ).ConfigureAwait( false );
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
		public DbConnection Connection
		{
			get => this.connection;
			set => this.connection = value;			
		}

		/// <summary>
		/// Gets or sets the connection to be used for the handler
		/// </summary>
		public DbTransaction Transaction
		{
			get => this.transaction;
			set => this.transaction = value;
		}
	}
}
