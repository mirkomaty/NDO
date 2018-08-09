using NDO.Mapping;
using NDO.Query;
using NDOql.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity;

namespace NDO.SqlPersistenceHandling
{
	class SqlQueryGenerator : IQueryGenerator
	{
		private readonly IUnityContainer configContainer;
		private List<QueryInfo> subQueries = new List<QueryInfo>();
		private Func<Dictionary<Relation, Class>, bool, Class, object, string> selectPartCreator;
		private object additionalSelectPartData = null;

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
		/// <param name="orderings">List of orderings for the resultset.</param>
		/// <param name="skip">Determines how many records of the resultset should be skipped. The resultset must be ordered.</param>
		/// <param name="take">Determines how many records of the resultset should be returned by the query. The resultset must be ordered.</param>
		/// <returns>A query string.</returns>
		/// <remarks>The result can be used for debugging and display purposes or with handlers, which don't support distributed databases.</remarks>
		public string GenerateQueryStringForAllTypes(
			List<QueryContextsEntry> queryContextsList, 
			OqlExpression expressionTree, 
			bool hollow, 
			List<QueryOrder> orderings,
			int skip,
			int take )
        {
			this.selectPartCreator = CreateQuerySelectPart;

			CreateSubQueriesForAllTypes(queryContextsList, expressionTree, hollow, orderings, skip, take );
            StringBuilder generatedQuery = new StringBuilder();

            foreach (QueryInfo qi in subQueries)
            {
                generatedQuery.Append(qi.QueryString);
                generatedQuery.Append(";\r\n");
            }

            generatedQuery.Length -= 3; // the trailing ;\r\n

            return generatedQuery.ToString();
		}

		/// <summary>
		/// Creates a SQL query string, which can be passed to the IPersistenceHandler to fetch the results for a given concrete type.
		/// </summary>
		/// <param name="queryContextsEntry">All contexts which define possible mutations of concrete classes in results and relations.</param>
		/// <param name="expressionTree">The syntax tree of the NDOql query expression.</param>
		/// <param name="hollow">Determines, if the query results should be hollow objects.</param>
		/// <param name="hasSubclassResultsets">Determines, if this query is part of a composed query which spans over several tables.</param>
		/// <param name="orderings">List of orderings for the resultset.</param>
		/// <param name="skip">Determines how many records of the resultset should be skipped. The resultset must be ordered.</param>
		/// <param name="take">Determines how many records of the resultset should be returned by the query. The resultset must be ordered.</param>
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
			this.selectPartCreator = CreateQuerySelectPart;

			return InnerGenerateQueryString( queryContextsEntry, expressionTree, hollow, hasSubclassResultsets, orderings, skip, take );
		}

		private string InnerGenerateQueryString( QueryContextsEntry queryContextsEntry, OqlExpression expressionTree, bool hollow, bool hasSubclassResultsets, List<QueryOrder> orderings, int skip, int take )
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
			List<QueryOrder> orderings,
			int skip,
			int take )
		{
			foreach (var item in queryContextsList)
			{
				CreateSubQueries( item, expressionTree, hollow, queryContextsList.Count > 1, orderings, skip, take );
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
				var generator = new SqlColumnListGenerator( cls );
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
			Class cls = this.configContainer.Resolve<Mappings>().FindClass( resultType );

			StringBuilder sb = new StringBuilder( "SELECT " );

			string columnList = this.selectPartCreator( relationContext, hollow, cls, this.additionalSelectPartData );
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

		private string CreateQuerySelectPart( Dictionary<Relation, Class> relationContext, bool hollow, Class cls, object additionalData )
		{
			var generator = CreateColumnListGenerator( cls );

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

			return columnList;
		}

		private string CreateAggregateSelectPart( Dictionary<Relation, Class> relationContext, bool hollow, Class cls, object additionalData )
		{
			var tuple = (Tuple<string, AggregateType>)additionalData;
			string field = tuple.Item1;
			AggregateType aggregateType = tuple.Item2;

			Column column;
			if (field.ToLower().StartsWith( "oid" ))
			{
#warning !!!! Oid-Queries: Ändern !!!!!
				Regex regex = new Regex( @"\(\s*(\d+)\s*\)" );
				Match match = regex.Match( field );
				int index = 0;
				if (match.Success)
					index = int.Parse( match.Groups[1].Value );
				column = ((OidColumn)cls.Oid.OidColumns[index]);
			}
			else
			{
				column = cls.FindField( field ).Column;
			}

			var provider = cls.Provider;

			return aggregateType.ToString().ToUpper() + "(" + column.GetQualifiedName() + ") as " + provider.GetQuotedName( "AggrResult" );
		}

		public string GenerateAggregateQueryString( string field, QueryContextsEntry queryContextsEntry, OqlExpression expressionTree, bool hasSubclassResultsets, AggregateType aggregateType )
		{
			this.selectPartCreator = CreateAggregateSelectPart;
			this.additionalSelectPartData = new Tuple<string,AggregateType>( field, aggregateType );

			var query = InnerGenerateQueryString( queryContextsEntry, expressionTree, true, hasSubclassResultsets, new List<QueryOrder>(), 0, 0 );
			return query.Replace( " DISTINCT", "" );
		}
	}
}
