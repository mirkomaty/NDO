using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NDO;

namespace AsyncTests
{
	/// <summary>
	/// Creates a temporary Sqlite database with the schema of the PureBusinessClasses mapping.
	/// </summary>
	/// <remarks>
	/// The mapping file of PureBusinessClasses points to SQL Server. The connection is replaced by
	/// a Sqlite connection. The tables are generated from the DataSet of the PersistenceManager,
	/// which NDO builds from the mapping information.
	/// </remarks>
	class SqliteTestDatabase : IDisposable
	{
		readonly string directory;

		public string MappingFile { get; }
		public string DatabaseFile { get; }
		public string ConnectionString => $"Data Source={DatabaseFile}";

		public SqliteTestDatabase( string providerName = "Sqlite" )
		{
			this.directory = Path.Combine( Path.GetTempPath(), "ndo-async-tests-" + Guid.NewGuid().ToString( "N" ) );
			Directory.CreateDirectory( this.directory );
			DatabaseFile = Path.Combine( this.directory, "test.db" );
			MappingFile = Path.Combine( this.directory, "NDOMapping.xml" );

			var mappingSource = Path.Combine( AppContext.BaseDirectory, "PureBusinessClasses.NDOMapping.xml" );
			XDocument doc = XDocument.Load( mappingSource );
			foreach (var conn in doc.Descendants( "Connection" ))
			{
				conn.SetAttributeValue( "Type", providerName );
				conn.SetAttributeValue( "Name", ConnectionString );
			}
			doc.Save( MappingFile );

			CreateSchema();
		}

		public PersistenceManager NewPersistenceManager()
		{
			return new PersistenceManager( MappingFile );
		}

		void CreateSchema()
		{
			using (var pm = NewPersistenceManager())
			using (var handler = pm.GetSqlPassThroughHandler())
			{
				foreach (DataTable dt in pm.DataSet.Tables)
					handler.Execute( CreateTableSql( dt ) );
				// The handler runs in a transaction. Without commit the tables would be rolled back by Dispose.
				handler.CommitTransaction();
			}
		}

		static string Quote( string name ) => "\"" + name + "\"";

		static string CreateTableSql( DataTable dt )
		{
			var pk = dt.PrimaryKey;
			bool inlinePk = pk.Length == 1 && pk[0].AutoIncrement;
			var columns = new List<string>();
			foreach (DataColumn col in dt.Columns)
			{
				if (inlinePk && col == pk[0])
					columns.Add( Quote( col.ColumnName ) + " INTEGER PRIMARY KEY AUTOINCREMENT" );
				else
					columns.Add( Quote( col.ColumnName ) + " " + SqliteType( col.DataType ) );
			}

			if (!inlinePk && pk.Length > 0)
				columns.Add( "PRIMARY KEY (" + String.Join( ", ", pk.Select( c => Quote( c.ColumnName ) ) ) + ")" );

			var sb = new StringBuilder();
			sb.Append( "CREATE TABLE " ).Append( Quote( dt.TableName ) ).Append( " (" );
			sb.Append( String.Join( ", ", columns ) );
			sb.Append( ')' );
			return sb.ToString();
		}

		static string SqliteType( Type t )
		{
			if (t == typeof( string ) || t == typeof( char ))
				return "TEXT";
			if (t == typeof( int ) || t == typeof( long ) || t == typeof( short ) || t == typeof( byte ) || t == typeof( bool )
				|| t == typeof( uint ) || t == typeof( ulong ) || t == typeof( ushort ) || t == typeof( sbyte ))
				return "INTEGER";
			if (t == typeof( double ) || t == typeof( float ))
				return "REAL";
			if (t == typeof( decimal ))
				return "DECIMAL";
			if (t == typeof( DateTime ))
				return "DATETIME";
			if (t == typeof( Guid ))
				return "UNIQUEIDENTIFIER";
			if (t == typeof( byte[] ))
				return "BLOB";
			return "TEXT";
		}

		public void Dispose()
		{
			// Sqlite keeps pooled connections open, which would lock the database file.
			System.Data.SQLite.SQLiteConnection.ClearAllPools();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			try
			{
				Directory.Delete( this.directory, true );
			}
			catch (IOException)
			{
				// Not critical for the tests
			}
		}
	}
}
