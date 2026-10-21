using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NDO.Mapping;

namespace NDOEnhancer
{
	/// <summary>
	/// Describes an index, which NDO creates for a table.
	/// </summary>
	/// <remarks>
	/// Without these indexes the database has to scan the whole table for each access via a foreign key.
	/// Databases which use locks for ReadCommitted (e.g. SqlServer) lock all rows they read during the scan.
	/// So concurrent transactions block each other and cause deadlocks, even if they don't access the same rows.
	/// The index on both sides of a mapping table is unique, because NDO doesn't allow to add the same object twice to a relation.
	/// </remarks>
	internal class IndexDefinition
	{
		public IndexDefinition( string name, DataColumn[] columns, bool isUnique )
		{
			Name = name;
			Columns = columns;
			IsUnique = isUnique;
		}

		/// <summary>
		/// The unquoted name of the index.
		/// </summary>
		public string Name { get; }

		public DataColumn[] Columns { get; }

		/// <summary>
		/// True, if the database should reject two rows with the same values in the columns of the index.
		/// </summary>
		public bool IsUnique { get; }

		/// <summary>
		/// Determines the indexes of a table.
		/// </summary>
		/// <remarks>
		/// Tables of classes get an index for the foreign key columns of each relation.
		/// Mapping tables get a unique index for the columns of both sides (used for the access from the own side and the deletion
		/// of single entries) and an index for the columns of the other side.
		/// </remarks>
		/// <param name="dt">The table</param>
		/// <param name="mappings">The mapping information</param>
		/// <returns>The indexes in a stable order, so that the names don't change between two runs of the enhancer.</returns>
		public static IEnumerable<IndexDefinition> ForTable( DataTable dt, NDOMapping mappings )
		{
			var columnLists = new List<(DataColumn[] Columns, bool IsUnique)>();

			Relation r = mappings.Classes
				.SelectMany( c => c.Relations )
				.FirstOrDefault( rel => rel.MappingTable != null && rel.MappingTable.TableName == dt.TableName );

			if (r != null)
			{
				DataColumn[] ownColumns = GetColumns( dt, r.ForeignKeyColumns.Select( c => c.Name ), r.ForeignKeyTypeColumnName );
				DataColumn[] otherColumns = GetColumns( dt, r.MappingTable.ChildForeignKeyColumns.Select( c => c.Name ), r.MappingTable.ChildForeignKeyTypeColumnName );
				if (ownColumns != null && otherColumns != null)
				{
					columnLists.Add( (ownColumns.Concat( otherColumns ).ToArray(), true) );
					columnLists.Add( (otherColumns, false) );
				}
			}
			else
			{
				// The SchemaGenerator creates a DataRelation for each foreign key in a table of a class.
				foreach (DataRelation dr in dt.ParentRelations)
					columnLists.Add( (dr.ChildColumns, false) );
			}

			// Short names, because some databases limit the length of identifiers
			string tableName = dt.TableName.Substring( dt.TableName.LastIndexOf( '.' ) + 1 );
			var created = new HashSet<string>();
			int n = 0;
			foreach (var (columns, isUnique) in columnLists)
			{
				// Two DataRelations might use the same columns
				if (!created.Add( String.Join( ",", columns.Select( c => c.ColumnName ) ) ))
					continue;
				yield return new IndexDefinition( $"IX_{tableName}_{++n}", columns, isUnique );
			}
		}

		static DataColumn[] GetColumns( DataTable dt, IEnumerable<string> columnNames, string typeColumnName )
		{
			var names = columnNames.ToList();
			if (typeColumnName != null)
				names.Add( typeColumnName );
			if (names.Count == 0 || names.Any( name => !dt.Columns.Contains( name ) ))
				return null;
			return names.Select( name => dt.Columns[name] ).ToArray();
		}
	}
}
