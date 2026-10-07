using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// Fills a DataTable from a DbCommand. Replaces DbDataAdapter.Fill.
	/// </summary>
	/// <remarks>
	/// The semantics follow DbDataAdapter.Fill with MissingSchemaAction.Add and AcceptChangesDuringFill = true:
	/// Columns are mapped case-insensitive by name, missing columns are added,
	/// rows with an existing primary key are overwritten and all rows are Unchanged afterwards.
	/// Only the first result set is used.
	/// </remarks>
	internal static class DbDataTableFiller
	{
		/// <summary>
		/// Executes the command and loads the first result set into the table.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="table">The table to be filled.</param>
		/// <param name="cancellationToken">A token to cancel the operation</param>
		/// <returns>The number of rows read.</returns>
		public static async Task<int> FillAsync( DbCommand command, DataTable table, CancellationToken cancellationToken )
		{
			var conn = command.Connection;
			// Like DbDataAdapter.Fill: a closed connection is opened and closed again.
			bool closeIt = conn != null && conn.State == ConnectionState.Closed;
			if (closeIt)
				await conn.OpenAsync( cancellationToken ).ConfigureAwait( false );

			try
			{
				var reader = await command.ExecuteReaderAsync( cancellationToken ).ConfigureAwait( false );
				await using (reader.ConfigureAwait( false ))
				{
					return await LoadAsync( reader, table, cancellationToken ).ConfigureAwait( false );
				}
			}
			finally
			{
				if (closeIt)
					await conn.CloseAsync().ConfigureAwait( false );
			}
		}

		static async Task<int> LoadAsync( DbDataReader reader, DataTable table, CancellationToken cancellationToken )
		{
			int fieldCount = reader.FieldCount;
			if (fieldCount == 0)
				return 0;

			string[] fieldNames = GetUniqueFieldNames( reader );
			var targetColumns = new DataColumn[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				// The indexer of DataColumnCollection is case-insensitive
				DataColumn col = table.Columns[fieldNames[i]];
				if (col == null)
					col = table.Columns.Add( fieldNames[i], reader.GetFieldType( i ) );
				targetColumns[i] = col;
			}

			int rowCount = 0;
			var values = new object[fieldCount];
			table.BeginLoadData();
			try
			{
				while (await reader.ReadAsync( cancellationToken ).ConfigureAwait( false ))
				{
					// The row is buffered after ReadAsync, so the synchronous GetValues doesn't do any IO.
					reader.GetValues( values );
					// Columns without a value in the result set get their default value
					var rowValues = new object[table.Columns.Count];
					for (int i = 0; i < fieldCount; i++)
						rowValues[targetColumns[i].Ordinal] = values[i];
					// OverwriteChanges: the rows are Unchanged and existing rows with the same primary key are overwritten
					table.LoadDataRow( rowValues, LoadOption.OverwriteChanges );
					rowCount++;
				}
			}
			finally
			{
				table.EndLoadData();
			}

			return rowCount;
		}

		/// <summary>
		/// Makes the field names of a result set unique, like DbDataAdapter does: The second occurrence of
		/// a name gets the suffix 1, the third the suffix 2 and so on. Empty names are replaced by "Column".
		/// </summary>
		static string[] GetUniqueFieldNames( DbDataReader reader )
		{
			int fieldCount = reader.FieldCount;
			var names = new string[fieldCount];
			var usedNames = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
			for (int i = 0; i < fieldCount; i++)
			{
				string name = reader.GetName( i );
				if (String.IsNullOrEmpty( name ))
					name = "Column";
				string uniqueName = name;
				int suffix = 1;
				while (usedNames.Contains( uniqueName ))
					uniqueName = name + suffix++;
				usedNames.Add( uniqueName );
				names[i] = uniqueName;
			}

			return names;
		}
	}
}
