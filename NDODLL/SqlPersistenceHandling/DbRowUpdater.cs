using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// Writes changed DataRows back to the database. Replaces DbDataAdapter.Update.
	/// </summary>
	/// <remarks>
	/// The semantics follow DbDataAdapter.Update with UpdateBatchSize = 1, AcceptChangesDuringUpdate = true
	/// and ContinueUpdateOnError = false:
	/// <list type="bullet">
	/// <item>The command is chosen by the RowState of the row.</item>
	/// <item>The parameter values are taken from the row using IDataParameter.SourceColumn.
	/// Insert commands use the current values, delete commands the original values and update commands
	/// the SourceVersion of the parameter.</item>
	/// <item>If UpdatedRowSource is FirstReturnedRecord or Both, the first returned record is written back to the row.
	/// If it is OutputParameters or Both, output parameters are written back to the row.</item>
	/// <item>If an update or delete command affects no rows, a DBConcurrencyException is thrown.</item>
	/// <item>After a successful command the row is accepted. Deleted rows are detached.</item>
	/// </list>
	/// </remarks>
	internal class DbRowUpdater
	{
		readonly DbCommand insertCommand;
		readonly DbCommand updateCommand;
		readonly DbCommand deleteCommand;
		readonly Func<DataRow, CancellationToken, Task> afterInsert;

		/// <summary>
		/// Constructs a DbRowUpdater object
		/// </summary>
		/// <param name="insertCommand">The command for added rows. Can be null, if there are no added rows.</param>
		/// <param name="updateCommand">The command for modified rows. Can be null, if there are no modified rows.</param>
		/// <param name="deleteCommand">The command for deleted rows. Can be null, if there are no deleted rows.</param>
		/// <param name="afterInsert">Optional callback, which is executed after a row has been inserted.
		/// This is used to fetch autoincremented ids from databases, which don't support insert batches.</param>
		public DbRowUpdater( DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, Func<DataRow, CancellationToken, Task> afterInsert = null )
		{
			this.insertCommand = insertCommand;
			this.updateCommand = updateCommand;
			this.deleteCommand = deleteCommand;
			this.afterInsert = afterInsert;
		}

		/// <summary>
		/// Writes the given rows to the database in the given order.
		/// </summary>
		/// <param name="rows">The rows to be written</param>
		/// <param name="cancellationToken">A token to cancel the operation</param>
		/// <returns>The number of successfully processed rows</returns>
		public async Task<int> UpdateAsync( IEnumerable<DataRow> rows, CancellationToken cancellationToken )
		{
			int count = 0;
			foreach (DataRow row in rows)
			{
				DbCommand cmd;
				StatementType statementType;
				switch (row.RowState)
				{
					case DataRowState.Added:
						cmd = this.insertCommand;
						statementType = StatementType.Insert;
						break;
					case DataRowState.Modified:
						cmd = this.updateCommand;
						statementType = StatementType.Update;
						break;
					case DataRowState.Deleted:
						cmd = this.deleteCommand;
						statementType = StatementType.Delete;
						break;
					default:
						continue;
				}

				if (cmd == null)
					throw new InvalidOperationException( $"Update requires a valid {statementType}Command when passed DataRow collection with {row.RowState.ToString().ToLowerInvariant()} rows." );

				if (await UpdateRowAsync( cmd, statementType, row, cancellationToken ).ConfigureAwait( false ))
					count++;
			}

			return count;
		}

		async Task<bool> UpdateRowAsync( DbCommand cmd, StatementType statementType, DataRow row, CancellationToken cancellationToken )
		{
			SetParameterValues( cmd, statementType, row );

			var conn = cmd.Connection;
			// Like DbDataAdapter.Update: a closed connection is opened and closed again.
			bool closeIt = conn != null && conn.State == ConnectionState.Closed;
			if (closeIt)
				await conn.OpenAsync( cancellationToken ).ConfigureAwait( false );

			int recordsAffected;
			try
			{
				var rowSource = cmd.UpdatedRowSource;
				if (rowSource == UpdateRowSource.FirstReturnedRecord || rowSource == UpdateRowSource.Both)
					recordsAffected = await ExecuteAndReadBackAsync( cmd, row, cancellationToken ).ConfigureAwait( false );
				else
					recordsAffected = await cmd.ExecuteNonQueryAsync( cancellationToken ).ConfigureAwait( false );

				if (rowSource == UpdateRowSource.OutputParameters || rowSource == UpdateRowSource.Both)
					ReadOutputParameters( cmd, row );

				if (recordsAffected == 0)
				{
					if (statementType == StatementType.Insert)
						return false; // DbDataAdapter doesn't report an error, but doesn't accept the row either.

					throw new DBConcurrencyException( $"Concurrency violation: the {statementType}Command affected 0 of the expected 1 records.", null, new[] { row } );
				}

				if (statementType == StatementType.Insert && this.afterInsert != null)
					await this.afterInsert( row, cancellationToken ).ConfigureAwait( false );
			}
			finally
			{
				if (closeIt)
					await conn.CloseAsync().ConfigureAwait( false );
			}

			row.AcceptChanges(); // Deleted rows will be detached
			return true;
		}

		static void SetParameterValues( DbCommand cmd, StatementType statementType, DataRow row )
		{
			DataColumnCollection columns = row.Table.Columns;
			foreach (DbParameter parameter in cmd.Parameters)
			{
				if (parameter.Direction != ParameterDirection.Input && parameter.Direction != ParameterDirection.InputOutput)
					continue;

				string sourceColumn = parameter.SourceColumn;
				if (String.IsNullOrEmpty( sourceColumn ))
					continue;

				DataColumn column = columns[sourceColumn];
				if (column == null)
					continue;  // Like DbDataAdapter: parameters without a matching column are left untouched

				DataRowVersion version;
				switch (statementType)
				{
					case StatementType.Insert:
						version = DataRowVersion.Current;
						break;
					case StatementType.Delete:
						version = DataRowVersion.Original;
						break;
					default:
						version = parameter.SourceVersion;
						break;
				}

				if (!row.HasVersion( version ))
					version = row.HasVersion( DataRowVersion.Current ) ? DataRowVersion.Current : DataRowVersion.Original;

				parameter.Value = row[column, version];
			}
		}

		static async Task<int> ExecuteAndReadBackAsync( DbCommand cmd, DataRow row, CancellationToken cancellationToken )
		{
			var reader = await cmd.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );
			await using (reader.ConfigureAwait( false ))
			{
				if (reader.FieldCount > 0 && await reader.ReadAsync( cancellationToken ).ConfigureAwait( false ))
				{
					var values = new object[reader.FieldCount];
					reader.GetValues( values );
					DataColumnCollection columns = row.Table.Columns;
					for (int i = 0; i < values.Length; i++)
					{
						DataColumn column = columns[reader.GetName( i )];
						if (column != null)
							SetValue( row, column, values[i] );
					}
				}

				// Consume the remaining results, so that RecordsAffected is complete.
				while (await reader.NextResultAsync( cancellationToken ).ConfigureAwait( false ))
				{
				}
			}

			// RecordsAffected is only valid after the reader has been closed.
			return reader.RecordsAffected;
		}

		static void ReadOutputParameters( DbCommand cmd, DataRow row )
		{
			DataColumnCollection columns = row.Table.Columns;
			foreach (DbParameter parameter in cmd.Parameters)
			{
				if (parameter.Direction == ParameterDirection.Input || String.IsNullOrEmpty( parameter.SourceColumn ))
					continue;
				DataColumn column = columns[parameter.SourceColumn];
				if (column != null)
					SetValue( row, column, parameter.Value );
			}
		}

		static void SetValue( DataRow row, DataColumn column, object value )
		{
			// Autoincrement columns may be read only.
			bool readOnly = column.ReadOnly;
			if (readOnly)
				column.ReadOnly = false;
			try
			{
				row[column] = value ?? DBNull.Value;
			}
			finally
			{
				if (readOnly)
					column.ReadOnly = true;
			}
		}
	}
}
