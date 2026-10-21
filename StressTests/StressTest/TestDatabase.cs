using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NDO;
using NDO.Mapping;
using NDO.ProviderFactory;
using NDOInterfaces;
using StressTestClasses;

namespace StressTest
{
	/// <summary>
	/// Provides PersistenceManager instances for the configured provider and creates the database schema.
	/// </summary>
	/// <remarks>
	/// The mapping file of StressTestClasses contains a dummy connection. It is copied to a provider specific
	/// mapping file, in which the connection type and connection string are replaced by the configured values.
	/// The tables are generated with the ISqlGenerator of the provider from the DataSet, which NDO builds from the mapping.
	/// In addition to the tables, indexes for the foreign keys and the search columns are created.
	/// </remarks>
	public class TestDatabase
	{
		readonly StressTestOptions options;
		readonly NDOMapping sharedMapping;

		public string MappingFile { get; }

		public TestDatabase( StressTestOptions options )
		{
			this.options = options;

			var mappingSource = Path.Combine( AppContext.BaseDirectory, "StressTestClasses.NDOMapping.xml" );
			XDocument doc = XDocument.Load( mappingSource );
			foreach (var conn in doc.Descendants( "Connection" ))
			{
				conn.SetAttributeValue( "Type", options.Provider );
				conn.SetAttributeValue( "Name", options.ConnectionString );
			}
			MappingFile = Path.Combine( AppContext.BaseDirectory, $"NDOMapping.{options.Provider}.xml" );
			doc.Save( MappingFile );

			if (options.SharedMapping)
			{
				// The mapping object stays valid after the PersistenceManager is closed.
				using (var pm = new PersistenceManager( MappingFile ))
					this.sharedMapping = pm.NDOMapping;
			}
		}

		/// <summary>
		/// Creates a new PersistenceManager. Each worker must use its own instances.
		/// </summary>
		public PersistenceManager NewPersistenceManager()
		{
			var pm = this.sharedMapping != null
				? new PersistenceManager( this.sharedMapping )
				: new PersistenceManager( MappingFile );
			pm.TransactionMode = this.options.TransactionMode;
			if (this.options.IsolationLevel.HasValue)
				pm.IsolationLevel = this.options.IsolationLevel.Value;
			return pm;
		}

		IProvider GetProvider( PersistenceManager pm, out ISqlGenerator generator )
		{
			Connection conn = pm.NDOMapping.Connections.First();
			IProvider provider = pm.NDOMapping.GetProvider( conn );
			if (provider == null)
				throw new Exception( $"Provider '{this.options.Provider}' not found. Available providers: {String.Join( ", ", NDOProviderFactory.Instance.ProviderNames )}" );
			if (!NDOProviderFactory.Instance.Generators.TryGetValue( this.options.Provider, out generator ))
				throw new Exception( $"No ISqlGenerator for provider '{this.options.Provider}' found." );
			generator.Provider = provider;
			return provider;
		}

		/// <summary>
		/// Quotes a table name, which might contain a schema name.
		/// </summary>
		static string QualifiedTableName( IProvider provider, string name )
		{
			return String.Join( ".", name.Split( '.' ).Select( n => provider.GetQuotedName( n ) ) );
		}

		static string UnqualifiedName( string name )
		{
			int p = name.LastIndexOf( '.' );
			if (p > -1)
				name = name.Substring( p + 1 );
			return name.Trim( '"', '`', '[', ']' );
		}

		async Task<HashSet<string>> GetExistingTablesAsync( IProvider provider, CancellationToken ct )
		{
			using (DbConnection conn = provider.NewConnection( this.options.ConnectionString ))
			{
				await conn.OpenAsync( ct );
				return new HashSet<string>( provider.GetTableNames( conn ).Select( UnqualifiedName ), StringComparer.OrdinalIgnoreCase );
			}
		}

		/// <summary>
		/// Executes each statement in its own transaction.
		/// </summary>
		async Task ExecuteAsync( string sql, CancellationToken ct )
		{
			// Some databases (e.g. Oracle) don't accept the statement terminator in a command
			sql = sql.Trim().TrimEnd( ';' );
			await using (var pm = NewPersistenceManager())
			await using (var handler = pm.GetSqlPassThroughHandler())
			{
				await handler.ExecuteAsync( sql, false, null, ct );
				await handler.CommitTransactionAsync( ct );
			}
		}

		public async Task DropSchemaAsync( CancellationToken ct = default )
		{
			await using (var pm = NewPersistenceManager())
			{
				IProvider provider = GetProvider( pm, out ISqlGenerator generator );
				var existing = await GetExistingTablesAsync( provider, ct );
				foreach (DataTable dt in pm.DataSet.Tables)
				{
					if (existing.Contains( UnqualifiedName( dt.TableName ) ))
						await ExecuteAsync( generator.DropTable( QualifiedTableName( provider, dt.TableName ) ), ct );
				}
			}
		}

		public async Task CreateSchemaAsync( CancellationToken ct = default )
		{
			await using (var pm = NewPersistenceManager())
			{
				IProvider provider = GetProvider( pm, out ISqlGenerator generator );
				foreach (DataTable dt in pm.DataSet.Tables)
				{
					foreach (string statement in CreateTableStatements( dt, pm.NDOMapping, provider, generator ))
						await ExecuteAsync( statement, ct );
				}

				foreach (string statement in CreateIndexStatements( pm.DataSet, pm.NDOMapping, provider, generator ))
					await ExecuteAsync( statement, ct );
			}
		}

		/// <summary>
		/// Generates indexes for the foreign key columns, the mapping tables and the columns used in the conditions of the workers.
		/// </summary>
		/// <remarks>
		/// NDO doesn't create indexes. Without them the database has to scan the whole table for each update, delete
		/// or query with a condition on such a column. With databases which use locks for ReadCommitted (e.g. SqlServer),
		/// the scans of concurrent transactions block each other and cause many deadlocks.
		/// </remarks>
		static IEnumerable<string> CreateIndexStatements( DataSet ds, NDOMapping mapping, IProvider provider, ISqlGenerator generator )
		{
			var created = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
			var counters = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );

			foreach (var (tableName, columnNames) in IndexDefinitions( mapping ))
			{
				DataTable dt = ds.Tables[tableName];
				if (dt == null || columnNames.Length == 0 || columnNames.Any( c => !dt.Columns.Contains( c ) ))
					continue;
				// The same columns can occur in both directions of a bidirectional relation
				if (!created.Add( tableName + ":" + String.Join( ",", columnNames ) ))
					continue;

				// Short names, because some databases limit the length of identifiers
				counters.TryGetValue( tableName, out int n );
				counters[tableName] = ++n;
				string indexName = provider.GetQuotedName( $"IX_{UnqualifiedName( tableName )}_{n}" );
				yield return generator.CreateIndex( indexName, QualifiedTableName( provider, tableName ), columnNames.Select( c => dt.Columns[c] ).ToArray(), false );
			}
		}

		static IEnumerable<(string TableName, string[] ColumnNames)> IndexDefinitions( NDOMapping mapping )
		{
			foreach (Class cl in mapping.Classes)
			{
				foreach (Relation r in cl.Relations)
				{
					var fkColumns = r.ForeignKeyColumns.Select( c => c.Name ).ToList();
					if (r.ForeignKeyTypeColumnName != null)
						fkColumns.Add( r.ForeignKeyTypeColumnName );

					if (r.MappingTable != null)
					{
						var childColumns = r.MappingTable.ChildForeignKeyColumns.Select( c => c.Name ).ToList();
						if (r.MappingTable.ChildForeignKeyTypeColumnName != null)
							childColumns.Add( r.MappingTable.ChildForeignKeyTypeColumnName );
						// Covers the access from the parent side and the deletion of single entries
						yield return (r.MappingTable.TableName, fkColumns.Concat( childColumns ).ToArray());
						// Access from the child side
						yield return (r.MappingTable.TableName, childColumns.ToArray());
					}
					else if (r.Multiplicity == RelationMultiplicity.Element)
					{
						// The foreign key is in the table of the class
						yield return (cl.TableName, fkColumns.ToArray());
					}
					else
					{
						// 1:n relation: the foreign key is in the table of the referenced class
						Class refClass = mapping.FindClass( r.ReferencedTypeName );
						if (refClass != null)
							yield return (refClass.TableName, fkColumns.ToArray());
					}
				}
			}

			// Columns used in the conditions of the workers
			yield return (mapping.FindClass( typeof( SalesOrder ) ).TableName, new[] { "OrderNumber" });
			yield return (mapping.FindClass( typeof( Customer ) ).TableName, new[] { "Name" });
		}

		/// <summary>
		/// Generates the CREATE TABLE statement in the same way as the GenericSqlGenerator of the NDOEnhancer.
		/// </summary>
		static IEnumerable<string> CreateTableStatements( DataTable dt, NDOMapping mapping, IProvider provider, ISqlGenerator generator )
		{
			string tableName = QualifiedTableName( provider, dt.TableName );
			Class cl = mapping.Classes.FirstOrDefault( c => c.TableName == dt.TableName );
			DataColumn[] pk = dt.PrimaryKey;
			var placement = generator.PrimaryConstraintPlacement;
			// A combined primary key can't be declared in the column definitions
			if (pk.Length > 1 && placement == PrimaryConstraintPlacement.InColumn)
				placement = PrimaryConstraintPlacement.InTable;

			var columns = new List<string>();
			foreach (DataColumn dc in dt.Columns)
				columns.Add( CreateColumn( dc, cl, provider, generator, pk.Length == 1 && pk.Contains( dc ) && placement == PrimaryConstraintPlacement.InColumn, pk.Contains( dc ) ) );

			string pkConstraint = null;
			if (pk.Length > 0 && placement != PrimaryConstraintPlacement.InColumn)
				pkConstraint = generator.CreatePrimaryKeyConstraint( pk, provider.GetQuotedName( "PK_" + UnqualifiedName( dt.TableName ) ), tableName );

			var sb = new StringBuilder();
			sb.Append( generator.BeginnTable( tableName ) ).Append( '\n' );
			sb.Append( String.Join( ",\n", columns ) );
			if (pkConstraint != null && placement == PrimaryConstraintPlacement.InTable)
				sb.Append( ",\n" ).Append( pkConstraint );
			sb.Append( '\n' ).Append( generator.EndTable( tableName ) );
			yield return sb.ToString();

			if (pkConstraint != null && placement == PrimaryConstraintPlacement.AfterTable)
				yield return pkConstraint;
		}

		static string CreateColumn( DataColumn dc, Class cl, IProvider provider, ISqlGenerator generator, bool primaryKeyInColumn, bool isPrimary )
		{
			string name = provider.GetQuotedName( dc.ColumnName );
			string columnType = null;
			string width = null;
			string precision = null;
			bool allowNull = true;
			int size = 0;

			Field field = cl?.Fields.FirstOrDefault( f => f.Column.Name == dc.ColumnName );
			if (field != null)
			{
				columnType = field.Column.DbType;
				size = field.Column.Size;
				if (size != 0)
					width = size.ToString();
				if (field.Column.Precision != 0)
					precision = field.Column.Precision.ToString();
				allowNull = field.Column.AllowDbNull;
			}

			bool autoIncrement = field == null && isPrimary && dc.AutoIncrement;

			if (columnType == null)
				columnType = generator.DbTypeFromType( dc.DataType, size );

			if (width == null)
			{
				int dl = provider.GetDefaultLength( dc.DataType );
				if (dl != 0)
					width = dl.ToString();
			}

			if (precision == null && dc.DataType == typeof( decimal ))
				precision = "2";

			if (!generator.LengthAllowed( columnType ))
				width = null;

			string definition;
			if (autoIncrement && generator.HasSpecialAutoIncrementColumnFormat)
				definition = generator.AutoIncrementColumn( name, dc.DataType, columnType, width, isPrimary );
			else if (primaryKeyInColumn)
				definition = generator.PrimaryKeyColumn( name, dc.DataType, columnType, width );
			else if (width != null && precision != null)
				definition = $"{name} {columnType}({width},{precision})";
			else if (width != null)
				definition = $"{name} {columnType}({width})";
			else
				definition = $"{name} {columnType}";

			return definition + " " + generator.NullExpression( allowNull && dc.AllowDBNull );
		}

		/// <summary>
		/// Creates the tags, which are shared by all workers.
		/// </summary>
		/// <returns>The ids of the tags</returns>
		public async Task<int[]> CreateTagsAsync( int count, CancellationToken ct = default )
		{
			await using (var pm = NewPersistenceManager())
			{
				var tags = new List<Tag>();
				for (int i = 0; i < count; i++)
				{
					var tag = new Tag() { Name = $"Tag{i}" };
					pm.MakePersistent( tag );
					tags.Add( tag );
				}
				await pm.SaveAsync( false, ct );
				return tags.Select( t => (int) ((IPersistenceCapable) t).NDOObjectId.Id[0] ).ToArray();
			}
		}

		public class RowCounts
		{
			public int Customers;
			public int Orders;
			public int OrderLines;
			public int Tags;
			public long MappingTableEntries;

			public override string ToString() => $"Customers: {Customers}, Orders: {Orders}, OrderLines: {OrderLines}, Tags: {Tags}, Order-Tag relations: {MappingTableEntries}";
		}

		public async Task<RowCounts> GetRowCountsAsync( CancellationToken ct = default )
		{
			var result = new RowCounts();
			await using (var pm = NewPersistenceManager())
			{
				result.Customers = await pm.Objects<Customer>().CountAsync( ct );
				result.Orders = await pm.Objects<SalesOrder>().CountAsync( ct );
				result.OrderLines = await pm.Objects<OrderLine>().CountAsync( ct );
				result.Tags = await pm.Objects<Tag>().CountAsync( ct );
			}

			// The mapping table has no class, so we count it with SQL.
			// We use a separate PersistenceManager: GetSqlPassThroughHandler() uses a new Connection object,
			// so in pessimistic mode the handler would open a second transaction besides the one of the queries above,
			// which blocks e.g. with Sqlite and IsolationLevel.Serializable.
			await using (var pm = NewPersistenceManager())
			{
				var relation = pm.NDOMapping.FindClass( typeof( SalesOrder ) ).Relations.Single( r => r.FieldName == "tags" );
				IProvider provider = GetProvider( pm, out _ );
				string table = QualifiedTableName( provider, relation.MappingTable.TableName );
				await using (var handler = pm.GetSqlPassThroughHandler())
				{
					await using (DbDataReader reader = await handler.ExecuteAsync( $"SELECT COUNT(*) FROM {table}", true, null, ct ))
					{
						await reader.ReadAsync( ct );
						result.MappingTableEntries = Convert.ToInt64( reader.GetValue( 0 ) );
					}
					await handler.CommitTransactionAsync( ct );
				}
			}
			return result;
		}
	}
}
