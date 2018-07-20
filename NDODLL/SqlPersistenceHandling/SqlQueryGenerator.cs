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
	class SqlQueryGenerator
	{
		private readonly IUnityContainer configContainer;
		private List<QueryInfo> subQueries = new List<QueryInfo>();
		PersistenceManager pm;

		public SqlQueryGenerator(IUnityContainer configContainer)
		{
			this.configContainer = configContainer;
		}

		/// <summary>
		/// Retrieves the SQL code of a NDOql Query.
		/// </summary>
		/// <remarks>Note that the query string doesn't show the actually used field names. They are built in the PersistenceHandler.</remarks>
		//public string GeneratedQuery
		//{
		//	get
		//	{
		//		StringBuilder generatedQuery = new StringBuilder();
		//		if (subQueries.Count == 0)
		//			GenerateQuery();
		//		foreach (QueryInfo qi in subQueries)
		//		{
		//			generatedQuery.Append( qi.QueryString );
		//			generatedQuery.Append( ";\n" );
		//		}

		//		generatedQuery.Length--; // the trailing \n

		//		if (subQueries.Count == 1)
		//			generatedQuery.Length--; // the semicolon

		//		return generatedQuery.ToString();
		//	}
		//}

		void CreateSubQueriesForAllTypes( List<QueryContextsEntry> queryContextsList, OqlExpression expressionTree, bool hollow )
		{
			foreach (var item in queryContextsList)
			{
				CreateSubQueries( item.Type, item.QueryContexts, expressionTree, hollow );
			}
		}

		void CreateSubQueries( Type t, List<Dictionary<Relation, Class>> queryContexts, OqlExpression expressionTree, bool hollow )
		{
			if (queryContexts.Count == 0)
			{
				this.subQueries.Add( new QueryInfo( t, ConstructQueryString( t, new Dictionary<Relation, Class>(), expressionTree, hollow ) ) );
			}
			else
			{
				string queryString = string.Empty;
				int added = 0;
				for (int i = 0; i < queryContexts.Count; i++)
				{
					Dictionary<Relation, Class> queryContext = (Dictionary<Relation, Class>)queryContexts[i];

					if (!queryContext.Any( kvp => kvp.Value.SystemType == t ))
					{
						if (added > 0)
							queryString += " UNION \r\n";
						queryString += ConstructQueryString( t, queryContext, expressionTree, hollow );
						added++;
					}
				}
				this.subQueries.Add( new QueryInfo( t, queryString ) );
			}
		}

		string ConstructQueryString( Type resultType, Dictionary<Relation, Class> relationContext, OqlExpression expressionTree, bool hollow )
		{
			Class cls = this.configContainer.Resolve<NDOMapping>().FindClass( resultType );

			StringBuilder sb = new StringBuilder( "SELECT " );
			sb.Append( new ColumnListGenerator( cls, hollow ).GenerateColumnList( expressionTree ) );
			sb.Append( ' ' );
			sb.Append( new FromGenerator( cls, relationContext ).GenerateFromExpression( expressionTree ) );
			string where = new WhereGenerator( cls, relationContext ).GenerateWhereClause( expressionTree );
			if (where != string.Empty)
			{
				sb.Append( ' ' );
				sb.Append( where );
			}

			return sb.ToString();
		}
	}
}
