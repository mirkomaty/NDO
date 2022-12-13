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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using NDO.Mapping;
using NDO.Logging;
using NDOInterfaces;
using NDO.Query;
using System.Globalization;
using NDO.Configuration;
using System.Threading.Tasks;

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

		private List<string> insertCommands;
		private List<string> updateCommands;
		private List<string> deleteCommands;
		private List<DbParameterInfo> insertParameterInfos;
		private List<DbParameterInfo> updateParameterInfos;
		private List<DbParameterInfo> deleteParameterInfos;
		private DbConnection conn;
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
		private static Regex parameterRegex = new Regex( @"\{(\d+)\}", RegexOptions.Compiled );
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

			if (hasAutoincrementedColumn && provider.SupportsLastInsertedId && provider.SupportsInsertBatch)
			{
				this.insertCommands.Add( $"INSERT INTO {qualifiedTableName} ({this.fieldList}) VALUES ({this.namedParamList})" );
				this.insertCommands.Add( $"SELECT {provider.GetLastInsertedId(this.tableName, this.autoIncrementColumn.Name)}" );
			}
			else
			{
				this.insertCommands.Add( $"INSERT INTO {qualifiedTableName} ({this.fieldList}) VALUES ({this.namedParamList})" );				
			}
			if (hasAutoincrementedColumn && !provider.SupportsInsertBatch)
			{
				if (provider.SupportsLastInsertedId)
#warning Das müssen wir noch implementieren, dass der Handler aufgerufen wird - überprüfen, welcher Provider das noch braucht.
					provider.RegisterRowUpdateHandler(this);
				else
					throw new NDOException(32, "The provider of type " + provider.GetType().FullName + " doesn't support Autonumbered Ids. Use Self generated Ids instead.");
			}
		}

#if masked_out  // This code seems to be abundant
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
#endif

		private void GenerateUpdateCommand()
		{
			string sql;

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

			sql = $"UPDATE {qualifiedTableName} SET {zuwListe} WHERE ({where})";
			//Console.WriteLine(sql);
			this.updateCommands.Add(sql);
		}


		private void GenerateDeleteCommand()
		{
			this.deleteCommands = new List<string>();

			var whereBuilder = new StringBuilder();

            int oidCount = this.classMapping.Oid.OidColumns.Count;
			int i = 0;
            foreach(var oidColumn in this.classMapping.Oid.OidColumns)
			{
				if (provider.UseNamedParams)
					whereBuilder.Append( provider.GetQuotedName( oidColumn.Name ) + " = {" + i + "}" );
				else
					whereBuilder.Append( provider.GetQuotedName( oidColumn.Name ) + " = ?" );

				deleteParameterInfos.Add( new DbParameterInfo( oidColumn.Name, provider.GetDbType( oidColumn.SystemType ), oidColumn.TypeLength, false ) );

                Relation r = oidColumn.Relation;
                if (!this.hasGuidOid && r != null && r.ForeignKeyTypeColumnName != null)
                {
					whereBuilder.Append( " AND " +
						provider.GetQuotedName( r.ForeignKeyTypeColumnName ) + " = {" + i + "}" );

					deleteParameterInfos.Add( new DbParameterInfo( r.ForeignKeyTypeColumnName, provider.GetDbType( typeof(int) ), provider.GetDefaultLength( typeof(int) ), false ) );
				}

				if (i < oidCount - 1)
					whereBuilder.Append( " AND " );

				i++;
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
				Log( "InsertAsync: Rows:" );
				Dump( rows, null, null );

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
				Log( "UpdateAsync: Rows:" );
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
					foreach (var info in this.deleteParameterInfos)
					{
						parameters.Add( row[info.ColumnName] );
					}
				}

				var results = await ExecuteBatchAsync( this.deleteCommands, parameters, true ).ConfigureAwait( false );

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
					Dump( null, cmd, rearrangedStatements );

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

			Dump( null, cmd, rearrangedStatements );
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
			CommandType commandType;
			if (sql.Trim().StartsWith( "EXEC", StringComparison.InvariantCultureIgnoreCase ))
				commandType = CommandType.StoredProcedure;
			else
				commandType = CommandType.Text;

			DataTable table = GetTemplateTable(templateDataSet, this.tableName).Clone();

			var command = (DbCommand) this.provider.NewSqlCommand(this.conn);
			command.CommandType = commandType;

			var rearrangedStatements = new List<string>();
			List<string> inputStatements = new List<string>()
			{
				sql
			};

			CreateQueryParameters( command, inputStatements, rearrangedStatements, parameters, false );

			// We know, that we only have one statement to perform
			command.CommandText = rearrangedStatements[0];

			Dump(null, command, rearrangedStatements); // Dumps the Select Command

            try
            {
				await this.sqlSelectBahavior.Select( command, table );
			}
            catch (System.Exception ex)
            {
                string text = "Exception of type " + ex.GetType().Name + " while executing a Query: " + ex.Message + "\n";
                text += "Sql Statement: " + sql + "\n";
                throw new NDOException(40, text);
            }

			return table;		
		}

		///// <inheritdoc/>
		//public DataTable PerformQuery( string sql, IList parameters, DataSet templateDataSet )
		//{
		//	if (sql.Trim().StartsWith( "EXEC", StringComparison.InvariantCultureIgnoreCase ))
		//		this.selectCommands.CommandType = CommandType.StoredProcedure;
		//	else
		//		this.selectCommands.CommandType = CommandType.Text;

		//	DataTable table = GetTemplateTable(templateDataSet, this.tableName).Clone();

		//	this.selectCommands.CommandText = sql;

		//	CreateQueryParameters( this.selectCommands, parameters );

		//	Dump( null ); // Dumps the Select Command

		//	try
		//	{
		//		dataAdapter.Fill( table );
		//	}
		//	catch (System.Exception ex)
		//	{
		//		string text = "Exception of type " + ex.GetType().Name + " while executing a Query: " + ex.Message + "\n";
		//		text += "Sql Statement: " + sql + "\n";
		//		throw new NDOException( 40, text );
		//	}

		//	return table;
		//}


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
			}
		}
	}
}
