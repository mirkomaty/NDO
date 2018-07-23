using NDO.Mapping;
using NDO.Query;
using NDOql.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;

namespace NDO.SqlPersistenceHandling
{
	class SqlQueryGenerator : IQueryGenerator
	{
		private readonly IUnityContainer configContainer;
		private List<QueryInfo> subQueries = new List<QueryInfo>();

		public SqlQueryGenerator(IUnityContainer configContainer)
		{
			this.configContainer = configContainer;
		}

		/// <summary>
		/// Creates a Sql query string for the complete query over all types.
		/// </summary>
		/// <param name="queryContextsList">List of all contexts which define possible mutations of concrete classes in results and relations.</param>
		/// <param name="expressionTree">The syntax tree of the NDOql query expression.</param>
		/// <param name="hollow">Determines, if the query results should be hollow objects.</param>
		/// <returns>A query string.</returns>
		/// <remarks>The result can be used for debugging and display purposes or with handlers, which don't support distributed databases.</remarks>
		public string GenerateQueryStringForAllTypes(
			List<QueryContextsEntry> queryContextsList, 
			OqlExpression expressionTree, 
			bool hollow, 
			bool hasSubclassResultsets,
			List<QueryOrder> orderings,
			int skip,
			int take )
        {
            CreateSubQueriesForAllTypes(queryContextsList, expressionTree, hollow, hasSubclassResultsets, orderings, skip, take );
            StringBuilder generatedQuery = new StringBuilder();

            foreach (QueryInfo qi in subQueries)
            {
                generatedQuery.Append(qi.QueryString);
                generatedQuery.Append(";\n");
            }

            generatedQuery.Length--; // the trailing \n

            if (subQueries.Count == 1)
                generatedQuery.Length--; // the semicolon

            return generatedQuery.ToString();
		}

		/// <summary>
		/// Creates a SQL query string, which can be passed to the IPersistenceHandler to fetch the results for a given concrete type.
		/// </summary>
		/// <param name="queryContextsEntry">All contexts which define possible mutations of concrete classes in results and relations.</param>
		/// <param name="expressionTree">The syntax tree of the NDOql query expression.</param>
		/// <param name="hollow">Determines, if the query results should be hollow objects.</param>
		/// <returns>A Query string.</returns>
		public string GenerateQueryString( 
			QueryContextsEntry queryContextsEntry, 
			OqlExpression expressionTree, 
			bool hollow, 
			bool hasSubclassResultsets,
			List<QueryOrder> orderings,
			int skip,
			int take )
		{
			CreateSubQueries( queryContextsEntry, expressionTree, hollow, hasSubclassResultsets, orderings, skip, take );
			StringBuilder generatedQuery = new StringBuilder();

			foreach (QueryInfo qi in subQueries)
			{
				generatedQuery.Append( qi.QueryString );
				generatedQuery.Append( ";\n" );
			}

			generatedQuery.Length--; // the trailing \n

			if (subQueries.Count == 1)
				generatedQuery.Length--; // the semicolon

			return generatedQuery.ToString();
		}

		void CreateSubQueriesForAllTypes( 
			List<QueryContextsEntry> queryContextsList, 
			OqlExpression expressionTree, 
			bool hollow, 
			bool hasSubclassResultsets,
			List<QueryOrder> orderings,
			int skip,
			int take )
		{
			foreach (var item in queryContextsList)
			{
				CreateSubQueries( item, expressionTree, hollow, hasSubclassResultsets, orderings, skip, take );
			}
		}

		void CreateSubQueries( QueryContextsEntry queryContextsEntry, 
			OqlExpression expressionTree, 
			bool hollow, 
			bool hasSubclassResultsets,
			List<QueryOrder> orderings,
			int skip,
			int take )
		{
			Type t = queryContextsEntry.Type;
			var queryContexts = queryContextsEntry.QueryContexts;

			if (queryContexts.Count == 0)
			{
				this.subQueries.Add( new QueryInfo( t, ConstructQueryString( t, new Dictionary<Relation, Class>(), expressionTree, hollow, hasSubclassResultsets, orderings, skip, take ) ) );
			}
			else
			{
				string queryString = string.Empty;
				int added = 0;
				foreach (var queryContext in queryContexts)
				{
					if (!queryContext.Any( kvp => kvp.Value.SystemType == t ))
					{
						if (added > 0)
							queryString += " UNION \r\n";
						queryString += ConstructQueryString( t, queryContext, expressionTree, hollow, hasSubclassResultsets, orderings, skip, take );
						added++;
					}
				}
				this.subQueries.Add( new QueryInfo( t, queryString ) );
			}
		}

		SqlColumnListGenerator CreateColumnListGenerator( Class cls )
		{
			var isRegistered = configContainer.IsRegistered<SqlColumnListGenerator>( cls.FullName );
			if (!isRegistered)
			{
				var generator = new SqlColumnListGenerator();
				generator.Init( cls );
				configContainer.RegisterInstance<SqlColumnListGenerator>( cls.FullName, generator );
				return generator;
			}

			return configContainer.Resolve<SqlColumnListGenerator>( cls.FullName );
		}

		string ConstructQueryString( 
			Type resultType, 
			Dictionary<Relation, Class> relationContext, 
			OqlExpression expressionTree, 
			bool hollow, 
			bool hasSubclassResultsets, 
			List<QueryOrder> orderings, 
			int skip, 
			int take )
		{
			Class cls = this.configContainer.Resolve<NDOMapping>().FindClass( resultType );

			StringBuilder sb = new StringBuilder( "SELECT " );

			var generator = CreateColumnListGenerator(cls);

			// We have to hack around a special behavior of SQLite, generating
			// new columns with fully specified column names, if the query
			// is a UNION			
			var useSqliteHack = relationContext.Count > 0 && cls.Provider.GetType().FullName.IndexOf( "Sqlite" ) > -1;
			string columnList;
			if (hollow)
			{
				if (useSqliteHack)
				{
					columnList = generator.HollowFieldsWithAlias;
				}
				else
				{
					columnList = generator.HollowFields;
				}
			}
			else
			{
				if (useSqliteHack)
				{
					columnList = generator.SelectListWithAlias;
				}
				else
				{
					columnList = generator.SelectList;
				}
			}
			sb.Append( columnList );

			// If we need to sort a set of hollow results for different subclasses
			// we need to fetch the ordering columns together with the oid columns
			if (hollow && orderings.Count > 0 && hasSubclassResultsets)
			{
				var orderCols = (from o in orderings select cls.FindField( o.FieldName ).Column.GetQualifiedName()).ToArray();
				columnList += ", " + String.Join( ", ", orderCols );
			}

			sb.Append( ' ' );
			sb.Append( new FromGenerator( cls, relationContext ).GenerateFromExpression( expressionTree ) );
			string where = new WhereGenerator( cls, relationContext ).GenerateWhereClause( expressionTree );
			if (where != string.Empty)
			{
				sb.Append( ' ' );
				sb.Append( where );
			}

			if (orderings.Count > 0)
			{
				sb.Append( ' ' );
				sb.Append( new OrderGenerator( cls ).GenerateOrderClause( orderings, skip, take ) );
			}

			return sb.ToString();
		}
	}
}
