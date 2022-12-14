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
using System.Data;
using System.Data.Common;
using NDO.Mapping;
using NDO.Logging;
using NDOInterfaces;
using NDO.SqlPersistenceHandling;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace NDO
{
	/// <summary>
	/// This class supports reading and writing of relation mapping tables.
	/// </summary>
	internal class NDOMappingTableHandler : IMappingTableHandler
	{
		private Relation relation;
		private string insertCommand;
		private string deleteCommand;
		private string selectCommand;
		private List<DbParameterInfo> selectParameterInfos = new List<DbParameterInfo>();
		private List<DbParameterInfo> insertParameterInfos = new List<DbParameterInfo>();
		private List<DbParameterInfo> deleteParameterInfos = new List<DbParameterInfo>();
		private DbConnection connection;
		private DbTransaction transaction;
		private IProvider provider;
		private bool verboseMode;
		private ILogAdapter logAdapter;
		private SqlSelectBehavior sqlSelectBehavior;
		private SqlDumper sqlDumper;
		private NDOMapping mappings;

		private string GetParameter(IProvider provider, string name)
		{
			return provider.UseNamedParams ? provider.GetNamedParameter(name) : "?";
		}

		public void Initialize(NDOMapping mappings, Relation relation)
		{
			this.mappings = mappings;
			this.relation = relation;

			Connection con = mappings.FindConnection(relation.MappingTable.ConnectionId);
			this.provider = mappings.GetProvider( con );

			// The connection object will be initialized in the pm, to 
			// enable the callback for getting the real connection string.
			// CheckTransaction is the place, where this happens.
			this.connection = null;
			this.sqlSelectBehavior = new SqlSelectBehavior();

			GenerateSelectCommand();
			GenerateInsertCommand();
			GenerateDeleteCommand();
		}


		private void GenerateDeleteCommand()
		{
			int i = 0;

			var whereBuilder = new StringBuilder();

			new ForeignKeyIterator( relation ).Iterate( delegate ( ForeignKeyColumn fkColumn, bool isLastIndex )
			{
				whereBuilder.Append( provider.GetQuotedName( fkColumn.Name ) + " = {" + i++ + "} AND " );
				this.deleteParameterInfos.Add(new DbParameterInfo( fkColumn.Name, provider.GetDbType( fkColumn.SystemType ), fkColumn.Size, false ) );
			}
			);

			new ForeignKeyIterator( relation.MappingTable ).Iterate( delegate ( ForeignKeyColumn fkColumn, bool isLastIndex )
			{
				whereBuilder.Append( provider.GetQuotedName( fkColumn.Name ) + " = {" + i++ + "}" );
				if (!isLastIndex)
					whereBuilder.Append( " AND " );
				this.deleteParameterInfos.Add( new DbParameterInfo( fkColumn.Name, provider.GetDbType( fkColumn.SystemType ), fkColumn.Size, false ) );
			}
			);

			if (relation.ForeignKeyTypeColumnName != null)
			{
				whereBuilder.Append(" AND " + provider.GetQuotedName( relation.ForeignKeyTypeColumnName ) + " = {" + i++ + "}" );
				this.deleteParameterInfos.Add( new DbParameterInfo( relation.ForeignKeyTypeColumnName, provider.GetDbType( typeof( int ) ), provider.GetDefaultLength(typeof(int)), false ) );
			}

			if (relation.MappingTable.ChildForeignKeyTypeColumnName != null)
			{
				whereBuilder.Append( " AND " + provider.GetQuotedName( relation.MappingTable.ChildForeignKeyTypeColumnName ) + " = {" + i++ + "}" );
				this.deleteParameterInfos.Add( new DbParameterInfo( relation.MappingTable.ChildForeignKeyTypeColumnName, provider.GetDbType( typeof( int ) ), provider.GetDefaultLength( typeof( int ) ), false ) );
			}

			deleteCommand = $"DELETE FROM {provider.GetQualifiedTableName( relation.MappingTable.TableName )} WHERE ({whereBuilder.ToString()})";
		}

		private void GenerateInsertCommand()
		{
			int i = 0;

			// IDOwn1, IDOwn2
			string columns = string.Empty;
			string namedParameters = string.Empty;

			new ForeignKeyIterator( this.relation ).Iterate
			(
				delegate ( ForeignKeyColumn fkColumn, bool isLastElement )
				{
					columns += provider.GetQuotedName( fkColumn.Name );
					namedParameters += "{" + i++ + "}";
					this.insertParameterInfos.Add(new DbParameterInfo( fkColumn.Name , provider.GetDbType( fkColumn.SystemType ), provider.GetDefaultLength( fkColumn.SystemType ), false ) );
					columns += ", ";
					namedParameters += ", ";
				}
			);


			// IDOther1, IDOther2
			new ForeignKeyIterator( this.relation.MappingTable ).Iterate
			(
				delegate ( ForeignKeyColumn fkColumn, bool isLastElement )
				{
					columns += provider.GetQuotedName( fkColumn.Name );
					namedParameters += "{" + i++ + "}";
					this.insertParameterInfos.Add( new DbParameterInfo( fkColumn.Name , provider.GetDbType( fkColumn.SystemType ), provider.GetDefaultLength( fkColumn.SystemType ), false ) );
					if (!isLastElement)
					{
						columns += ", ";
						namedParameters += ", ";
					}
				}
			);

			// Note: so far we've appended the commas, from here we'll prepend them

			// TCOwn
			if (this.relation.ForeignKeyTypeColumnName != null)
			{
				columns += ", " + provider.GetQuotedName( this.relation.ForeignKeyTypeColumnName );
				namedParameters += ", {" + i++ + "}";
				this.insertParameterInfos.Add( new DbParameterInfo( this.relation.ForeignKeyTypeColumnName , provider.GetDbType( typeof( int ) ), provider.GetDefaultLength( typeof( int ) ), false ) );
			}

			// TCOther
			if (this.relation.MappingTable.ChildForeignKeyTypeColumnName != null)
			{
				columns += ", " + provider.GetQuotedName( this.relation.MappingTable.ChildForeignKeyTypeColumnName );
				namedParameters += ", {" + i++ + "}";
				this.insertParameterInfos.Add( new DbParameterInfo( this.relation.MappingTable.ChildForeignKeyTypeColumnName, provider.GetDbType( typeof( int ) ), provider.GetDefaultLength( typeof( int ) ), false ) );
			}

			var tableName = provider.GetQualifiedTableName( this.relation.MappingTable.TableName );

			insertCommand = $"INSERT INTO {tableName} ({columns}) VALUES ({namedParameters})";

			Class relatedClass = this.mappings.FindClass(this.relation.ReferencedTypeName);
		}

		private void GenerateSelectCommand()
		{
			StringBuilder whereBuilder = new StringBuilder();

			int i = 0;
			new ForeignKeyIterator( this.relation ).Iterate( delegate ( ForeignKeyColumn fkColumn, bool isLastEntry )
			{
				
				whereBuilder.Append( provider.GetQuotedName( fkColumn.Name ) + " = {" + i++ + "}" );
				this.selectParameterInfos.Add(new DbParameterInfo (fkColumn.Name, provider.GetDbType(fkColumn.SystemType), fkColumn.Size, false ) );
				if (!isLastEntry)
					whereBuilder.Append( " AND " );
			} );

			if (relation.ForeignKeyTypeColumnName != null)
			{
				//sql += " AND " + provider.GetQuotedName(r.ForeignKeyTypeColumnName) + " = " + oid.Id.TypeId;
				whereBuilder.Append( " AND " + provider.GetQuotedName( relation.ForeignKeyTypeColumnName ) + " = {" + i + "}" );
				this.selectParameterInfos.Add( new DbParameterInfo( relation.ForeignKeyTypeColumnName, provider.GetDbType( typeof( int ) ), provider.GetDefaultLength( typeof( int ) ), false ) );
			}

			this.selectCommand = $"SELECT * FROM {provider.GetQualifiedTableName( this.relation.MappingTable.TableName )} WHERE {whereBuilder.ToString()}";
		}

		/// <summary>
		/// Helps Logging
		/// </summary>
		public ILogAdapter LogAdapter
		{
			get { return logAdapter; }
			set 
			{ 
				logAdapter = value;
				if (value != null)
					this.sqlDumper = new SqlDumper( value, this.provider );
			}
		}


		/// <summary>
		/// Gets or sets a value which determines, if database operations will be logged in a logging file.
		/// </summary>
		public bool VerboseMode
		{
			get { return this.verboseMode; }
			set { this.verboseMode = value; }
		}


		private void Dump( DataRow[] rows, IDbCommand cmd, IEnumerable<string> batch )
		{
			if (!this.verboseMode || this.sqlDumper == null)
				return;

			this.sqlDumper.Dump( rows, cmd, batch );
		}


		private DataTable GetTableTemplate(DataSet templateDataset, string name)
		{
			DataTable dt = templateDataset.Tables[name];
			if (dt == null)
				throw new NDOException(24, "Can't find mapping table '" + name + "' in the schema. Check your mapping file.");
			return dt;
		}
		
		public async Task<DataTable> LoadRelatedObjectsAsync(ObjectId oid, DataSet templateDataset) 
		{
			DataTable table = GetTableTemplate(templateDataset, relation.MappingTable.TableName).Clone();
			var parameters = new List<object>();

            int i = 0;
            new ForeignKeyIterator(relation).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastEntry)
            {                    
				object o = oid.Id[i];
				if ( o is Guid && !this.provider.SupportsNativeGuidType )
					o = ((Guid) o).ToString();

				parameters.Add( o );
                i++;
            });
            
			if (relation.ForeignKeyTypeColumnName != null) 
			{
                parameters.Add( oid.Id.TypeId );
			}

			var command = (DbCommand) this.provider.NewSqlCommand(this.connection);
			command.Transaction = this.transaction;
			var rearrangedStatements = new List<string>();

			new BatchExecutor( this.provider, this.connection, this.transaction, Dump )
				.CreateQueryParameters( command, new[] { this.selectCommand }, rearrangedStatements, parameters, this.selectParameterInfos, false );

			command.CommandText = rearrangedStatements[0];  // There is only one statement to execute

			try
			{
				Dump(null, command, rearrangedStatements );
				await this.sqlSelectBehavior.Select( command, table ).ConfigureAwait( false );
            }
            catch (System.Exception ex)
            {
                string text = "Exception in LoadRelatedObjectsAsync: " + ex.Message + "\n";
                text += "Sql-Anweisung: " + command.CommandText + "\n";
                throw new NDOException(25, text);
            }

			return table;		
		}

		public async Task InsertAsync( DataRow[] rows)
		{
			Dump( null, null, new[] { "MappingTableHandler.InsertAsync" } );
			var parameters = new List<object>();
			foreach (var row in rows)
			{
				foreach (var info in this.insertParameterInfos)
				{
					parameters.Add( row[info.ColumnName] );
				}
			}

			var batchExecutor = new BatchExecutor( this.provider, this.connection, this.transaction, Dump );

			var results = await batchExecutor.ExecuteBatchAsync( new[]{this.insertCommand }, parameters, insertParameterInfos, true ).ConfigureAwait( false );
			Dump( rows, null, null );
		}

		public async Task DeleteAsync( DataRow[] rows )
		{
			Dump( null, null, new[] { "MappingTableHandler.DeleteAsync" } );
			var parameters = new List<object>();
			foreach (var row in rows)
			{
				foreach (var info in this.deleteParameterInfos)
				{
					parameters.Add( row[info.ColumnName] );
				}
			}

			var batchExecutor = new BatchExecutor( this.provider, this.connection, this.transaction, Dump );

			var results = await batchExecutor.ExecuteBatchAsync( new[]{this.deleteCommand }, parameters, deleteParameterInfos, true ).ConfigureAwait( false );
			Dump( rows, null, null );
		}

		public async Task UpdateAsync(DataSet ds) 
		{
			DataTable dt = ds.Tables[relation.MappingTable.TableName];
			try 
			{
				DataRow[] rows = dt.Select( null, null, DataViewRowState.Added );
				if (rows.Length > 0)
					await InsertAsync( rows ).ConfigureAwait( false );

				rows = dt.Select( null, null, DataViewRowState.Deleted );
				if (rows.Length > 0)
					await DeleteAsync( rows ).ConfigureAwait( false );

			}
			catch (System.Exception ex) 
			{
				throw new NDOException(26, "Exception in dataAdapter.Update: " + ex.Message + "\n");
			}
		}

		public DbTransaction Transaction
		{
			get => this.transaction;
			set => this.transaction = value;
		}
		public DbConnection Connection
		{
			get => this.connection;
			set => this.connection = value;
		}

		public void Dispose()
		{
		}


		public Relation Relation
		{
			get { return this.relation; }
		}
	}
}
