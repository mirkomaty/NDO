using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using NDO.ProviderFactory;
using NDOInterfaces;
using NUnit.Framework;

namespace AsyncTests
{
	/// <summary>
	/// Compares IProvider.GetDatabaseStructure, which uses ExecuteReader(SchemaOnly | KeyInfo) and DataTable.Load,
	/// with the result of DbDataAdapter.FillSchema, which has been used before.
	/// </summary>
	/// <remarks>
	/// Sqlite runs always. The other providers run, if a connection string to a database with tables
	/// is provided in an environment variable NDO_SCHEMATEST_&lt;PROVIDERNAME&gt;, e.g. NDO_SCHEMATEST_SQLSERVER.
	/// </remarks>
	[TestFixture]
	public class DatabaseStructureTests
	{
		[TestCase( "Sqlite" )]
		[TestCase( "SqlServer" )]
		[TestCase( "Oracle" )]
		[TestCase( "MySql" )]
		[TestCase( "MySqlConnector" )]
		[TestCase( "Postgre" )]
		public void GetDatabaseStructureEqualsFillSchema( string providerName )
		{
			SqliteTestDatabase sqliteDb = null;
			string connectionString;
			if (providerName == "Sqlite")
			{
				sqliteDb = new SqliteTestDatabase();
				connectionString = sqliteDb.ConnectionString;
			}
			else
			{
				connectionString = Environment.GetEnvironmentVariable( "NDO_SCHEMATEST_" + providerName.ToUpperInvariant() );
				if (String.IsNullOrEmpty( connectionString ))
					Assert.Ignore( $"No connection string for {providerName} provided." );
			}

			try
			{
				IProvider provider = NDOProviderFactory.Instance[providerName];
				using (DbConnection conn = provider.NewConnection( connectionString ))
				{
					conn.Open();
					DbProviderFactory factory = DbProviderFactories.GetFactory( conn );
					Assert.That( factory, Is.Not.Null, "The ADO.NET provider doesn't expose a DbProviderFactory" );

					DataSet actual = provider.GetDatabaseStructure( conn, null );
					string[] tableNames = provider.GetTableNames( conn, null );
					Assert.That( tableNames, Is.Not.Empty );
					Assert.That( actual.Tables.Count, Is.EqualTo( tableNames.Length ) );

					foreach (string tableName in tableNames)
					{
						DataTable expected = FillSchema( provider, factory, conn, tableName );
						DataTable actualTable = actual.Tables[tableName];
						Assert.That( actualTable, Is.Not.Null, tableName );
						CompareTables( expected, actualTable );
					}
				}
			}
			finally
			{
				sqliteDb?.Dispose();
			}
		}

		[Test]
		public void AutoIncrementedPrimaryKeyIsDetected()
		{
			using (var sqliteDb = new SqliteTestDatabase())
			{
				IProvider provider = NDOProviderFactory.Instance["Sqlite"];
				using (DbConnection conn = provider.NewConnection( sqliteDb.ConnectionString ))
				{
					DataSet ds = provider.GetDatabaseStructure( conn, null );
					DataTable dt = ds.Tables["Mitarbeiter"];
					Assert.That( dt, Is.Not.Null );
					Assert.That( dt.PrimaryKey.Select( c => c.ColumnName ), Is.EqualTo( new[] { "ID" } ) );
					Assert.That( dt.PrimaryKey[0].AutoIncrement, Is.True );
					Assert.That( dt.Rows.Count, Is.EqualTo( 0 ) );
					Assert.That( conn.State, Is.EqualTo( ConnectionState.Closed ), "A closed connection must be closed again" );
				}
			}
		}

		static DataTable FillSchema( IProvider provider, DbProviderFactory factory, DbConnection conn, string tableName )
		{
			DbCommand cmd = provider.NewSqlCommand( conn );
			cmd.CommandText = "SELECT * FROM " + provider.GetQuotedName( tableName );
			DbDataAdapter adapter = factory.CreateDataAdapter();
			adapter.SelectCommand = cmd;
			DataSet ds = new DataSet();
			adapter.FillSchema( ds, SchemaType.Source );
			return ds.Tables[0];
		}

		static void CompareTables( DataTable expected, DataTable actual )
		{
			string table = actual.TableName;
			Assert.That( actual.Columns.Cast<DataColumn>().Select( c => c.ColumnName ),
				Is.EqualTo( expected.Columns.Cast<DataColumn>().Select( c => c.ColumnName ) ), $"{table}: column names" );

			foreach (DataColumn e in expected.Columns)
			{
				DataColumn a = actual.Columns[e.ColumnName];
				string col = $"{table}.{e.ColumnName}";
				Assert.That( a.DataType, Is.EqualTo( e.DataType ), $"{col}: DataType" );
				Assert.That( a.AllowDBNull, Is.EqualTo( e.AllowDBNull ), $"{col}: AllowDBNull" );
				Assert.That( a.AutoIncrement, Is.EqualTo( e.AutoIncrement ), $"{col}: AutoIncrement" );
				Assert.That( a.MaxLength, Is.EqualTo( e.MaxLength ), $"{col}: MaxLength" );
				Assert.That( a.ReadOnly, Is.EqualTo( e.ReadOnly ), $"{col}: ReadOnly" );
				Assert.That( a.Unique, Is.EqualTo( e.Unique ), $"{col}: Unique" );
			}

			Assert.That( actual.PrimaryKey.Select( c => c.ColumnName ),
				Is.EqualTo( expected.PrimaryKey.Select( c => c.ColumnName ) ), $"{table}: primary key" );
		}
	}
}
