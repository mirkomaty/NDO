using NDO.Mapping;
using NDO.Query;
using NDOql.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NDO.Configuration;

namespace NDO.SqlPersistenceHandling
{
	class SqlQueryGenerator : IQueryGenerator
	{
		private readonly INDOContainer configContainer;
		private List<QueryInfo> subQueries = new List<QueryInfo>();
		private Func<Dictionary<Relation, Class>, bool, Class, bool, object, string> selectPartCreator;
		private object additionalSelectPartData = null;
		private Mappings mappings;

		public SqlQueryGenerator(INDOContainer configContainer)
		{
			this.configContainer = configContainer;
			this.mappings = this.configContainer.Resolve<Mappings>();
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
		/// <param name="prefetch">Query for the given prefetch relation.</param>
		/// <returns>A query string.</returns>
		/// <remarks>The result can be used for debugging and display purposes or with handlers, which don't support distributed databases.</remarks>
		public string GenerateQueryStringForAllTypes(
			List<QueryContextsEntry> queryContextsList, 
			OqlExpression expressionTree, 
			bool hollow, 
			List<QueryOrder> orderings,
			int skip,
			int take, string prefetch = null )
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
		/// <param name="prefetch">Query for the given prefetch relation.</param>
		/// <returns>A Query string.</returns>
		public string GenerateQueryString( 
			QueryContextsEntry queryContextsEntry, 
			OqlExpression expressionTree, 
			bool hollow, 
			bool hasSubclassResultsets,
			List<QueryOrder> orderings,
			int skip,
			int take, 
			string prefetch )
		{
			this.selectPartCreator = CreateQuerySelectPart;

			return InnerGenerateQueryString( queryContextsEntry, expressionTree, hollow, hasSubclassResultsets, orderings, skip, take, prefetch );
		}

		private string InnerGenerateQueryString( QueryContextsEntry queryContextsEntry, OqlExpression expressionTree, bool hollow, bool hasSubclassResultsets, List<QueryOrder> orderings, int skip, int take, string prefetch )
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
			int take, 
			string prefetch = null )
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
			var key = $"{nameof(SqlColumnListGenerator)}-{cls.FullName}";
			return configContainer.ResolveOrRegisterType<SqlColumnListGenerator>( new ContainerControlledLifetimeManager(), key, new ParameterOverride( "cls", cls ) );
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
			Class cls = this.mappings.FindClass( resultType );

			StringBuilder sb = new StringBuilder( "SELECT " );

			var from = new FromGenerator( cls, relationContext ).GenerateFromExpression( expressionTree );
			var qualifyWithTableName = from.IndexOf( "INNER JOIN" ) > -1;

			string columnList = this.selectPartCreator( relationContext, hollow, cls, qualifyWithTableName, this.additionalSelectPartData );
			sb.Append( columnList );

			// If we need to sort a set of hollow results for different subclasses
			// we need to fetch the ordering columns together with the oid columns
			if (hollow && orderings.Count > 0 && hasSubclassResultsets)
			{
				var orderCols = (from o in orderings select cls.FindField( o.FieldName ).Column.GetQualifiedName()).ToArray();
				columnList += ", " + String.Join( ", ", orderCols );
			}

			sb.Append( ' ' );
			sb.Append( from );
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

		private string CreateQuerySelectPart( Dictionary<Relation, Class> relationContext, bool hollow, Class cls, bool qualifyWithTableName, object additionalData )
		{
			var generator = CreateColumnListGenerator( cls );

			// We have to hack around a special behavior of SQLite, generating
			// new columns with fully specified column names, if the query
			// is a UNION			
			var generateAliasNames = relationContext.Count > 0 && cls.Provider.GetType().FullName.IndexOf( "Sqlite" ) > -1;

			return generator.Result( hollow, generateAliasNames, qualifyWithTableName );
		}

		private string CreateAggregateSelectPart( Dictionary<Relation, Class> relationContext, bool hollow, Class cls, bool qualifyWithTableName, object additionalData )
		{
			var tuple = (Tuple<string, AggregateType>)additionalData;
			string field = tuple.Item1;
			AggregateType aggregateType = tuple.Item2;
			bool isStar = field == "*";

			Column column = null;
			if (field.ToLower().StartsWith( "oid" ))
			{
				Regex regex = new Regex( @"\(\s*(\d+)\s*\)" );
				Match match = regex.Match( field );
				int index = 0;
				if (match.Success)
					index = int.Parse( match.Groups[1].Value );
				column = ((OidColumn)cls.Oid.OidColumns[index]);
			}
			else
			{
				if (!isStar)
					column = cls.FindField( field ).Column;
			}

			var provider = cls.Provider;
			var colName = isStar ? "*" : column.GetQualifiedName();
			//var tableName = qualifyWithTableName ? cls.GetQualifiedTableName() + "." : String.Empty;

			return $"{aggregateType.ToString().ToUpper()} ({colName}) AS {provider.GetQuotedName( "AggrResult" )}";
		}

		public string GenerateAggregateQueryString( string field, QueryContextsEntry queryContextsEntry, OqlExpression expressionTree, bool hasSubclassResultsets, AggregateType aggregateType )
		{
			this.selectPartCreator = CreateAggregateSelectPart;
			this.additionalSelectPartData = new Tuple<string,AggregateType>( field, aggregateType );

			var query = InnerGenerateQueryString( queryContextsEntry, expressionTree, true, hasSubclassResultsets, new List<QueryOrder>(), 0, 0, null );
			return query;
		}

		public string GeneratePrefetchQuery( Type parentType, IEnumerable<IPersistenceCapable> parents, string prefetch )
		{
			Class parentCls = this.mappings.FindClass( parentType );

			StringBuilder sb = new StringBuilder( "SELECT " );
			List<Relation> relations = new List<Relation>();

			if (prefetch.IndexOf('.') == -1)
			{
				var rel = parentCls.FindRelation( prefetch );
				if (rel != null)
					relations.Add( rel );
			}

			new RelationContextGenerator( this.mappings ).CreateContextForName( parentCls, prefetch, relations );

			if (relations.Count == 0)
				throw new NDOException( 76, $"Prefetch: Can't find relation mapping with name {prefetch} in class {parentCls.FullName}" );

			Class cls = mappings.FindClass( relations[relations.Count - 1].ReferencedTypeName );
			var relationContext = new Dictionary<Relation, Class>();
#warning Hier fehlt der INNER JOIN
			string columnList = CreateQuerySelectPart( relationContext, false, cls, true, null );
			sb.Append( columnList );

			sb.Append( ' ' );
			sb.Append( new FromGenerator( cls, relationContext ).GenerateFromExpression( null, relations ) );
			// We can be sure, that we have only one oid column here.
			var oidList = String.Join( ",", from p in parents select p.NDOObjectId.Id.Values[0].ToString() );
			string where = $"WHERE {parentCls.Oid.OidColumns.First().GetQualifiedName()} IN ({oidList})";
			sb.Append( ' ' );
			sb.Append( where );

			return sb.ToString();
		}
	}
}
