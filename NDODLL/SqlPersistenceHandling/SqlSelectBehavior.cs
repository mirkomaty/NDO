using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NDO.SqlPersistenceHandling
{
	internal class SqlSelectBehavior
	{
		public SqlSelectBehavior()
		{
		}

		public async Task Select(DbCommand selectCommand, DataTable table)
		{
			var reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);
			using (reader)
			{
				while (await reader.ReadAsync().ConfigureAwait( false ))
				{
					var row = table.NewRow();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						// We don't need to check, if the row defines all columns, the reader returns,
						// since all column informations come from the mapping file.
						var name = reader.GetName( i );
						row[name] = reader.GetValue( i );
					}

					table.Rows.Add( row );
				}
			}
		}
	}
}
