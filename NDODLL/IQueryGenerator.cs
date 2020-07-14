using NDO.Query;
using NDO.SqlPersistenceHandling;
using NDOql.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO
{
	/// <summary>
	/// DI interface for classes which generate query strings for different target systems.
	/// </summary>
	/// <remarks>You can inject an implementation of this class, if your system works with other languages other than Sql</remarks>
	public interface IQueryGenerator
	{
		/// <summary>
		/// Creates a query string, which can be passed to the IPersistenceHandler to fetch the results for a given concrete type.
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
		string GenerateQueryString( QueryContextsEntry queryContextsEntry, OqlExpression expressionTree, bool hollow, bool hasSubclassResultsets, List<QueryOrder> orderings, int skip, int take, string prefetch = null );

		/// <summary>
		/// Creates a query string for the complete query over all types.
		/// </summary>
		/// <param name="queryContextsList">List of all contexts which define possible mutations of concrete classes in results and relations.</param>
		/// <param name="expressionTree">The syntax tree of the NDOql query expression.</param>
		/// <param name="hollow">Determines, if the query results should be hollow objects.</param>
		/// <param name="orderings">List of orderings for the resultset</param>
		/// <param name="skip">Determines how many records of the resultset should be skipped. The resultset must be ordered.</param>
		/// <param name="take">Determines how many records of the resultset should be returned by the query. The resultset must be ordered.</param>
		/// <param name="prefetch">Query for the given prefetch relation.</param>
		/// <returns>A query string.</returns>
		/// <remarks>The result can be used for debugging and display purposes or with handlers, which don't support distributed databases.</remarks>
		string GenerateQueryStringForAllTypes( List<QueryContextsEntry> queryContextsList, OqlExpression expressionTree, bool hollow, List<QueryOrder> orderings, int skip, int take, string prefetch = null );

		/// <summary>
		/// Creates a query string, which can be passed to the IPersistenceHandler to fetch the results of an aggregate operation for a given concrete type
		/// </summary>
		/// <param name="field">The field name which will be translated to a column name. You can enter a * for arbitrary fields.</param>
		/// <param name="queryContextsEntry">All contexts which define possible mutations of concrete classes in results and relations.</param>
		/// <param name="expressionTree">The syntax tree of the NDOql query expression.</param>
		/// <param name="hasSubclassResultsets">Determines, if this query is part of a composed query which spans over several tables.</param>
		/// <param name="aggregateType">The type of the aggregate function which should be performed</param>
		/// <returns></returns>
		string GenerateAggregateQueryString( string field, QueryContextsEntry queryContextsEntry, OqlExpression expressionTree, bool hasSubclassResultsets, Query.AggregateType aggregateType );

		/// <summary>
		/// Generates a query with an IN clause to fetch the child objects of a given relation
		/// </summary>
		/// <param name="parentType">Concrete type of the parent</param>
		/// <param name="parents">All parent objects</param>
		/// <param name="prefetch">The name of the relation, which has to be fetched</param>
		/// <returns></returns>
		string GeneratePrefetchQuery( Type parentType, IEnumerable<IPersistenceCapable> parents, string prefetch );
	}
}
