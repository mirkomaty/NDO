namespace NDO.AsyncTests
{
	/// <summary>
	/// A Sqlite provider which doesn't support insert batches.
	/// </summary>
	/// <remarks>
	/// With this provider NDO reads autoincremented ids with a separate statement after each insert.
	/// That is the code path used by providers like Postgres. The NDOProviderFactory finds the provider,
	/// because the name of the test assembly starts with "NDO.".
	/// </remarks>
	public class SqliteNoBatchProvider : NDO.SqliteProvider.Provider
	{
		public override string Name => "SqliteNoBatch";

		public override bool SupportsInsertBatch => false;

		// Sqlite doesn't accept the parenthesized expression of the base class as a standalone statement.
		public override string GetLastInsertedId( string tableName, string columnName ) => "SELECT last_insert_rowid()";
	}
}
