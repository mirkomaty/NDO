using NDO.Query;
using NDO.SqlPersistenceHandling;
using NDOql.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO
{
	public interface IQueryGenerator
	{
		/// <summary>
		/// Creates a query string, which can be passed to the IPersistenceHandler to fetch the results for a given concrete type.
		/// </summary>
		/// <param name="queryContextsEntry">All contexts which define possible mutations of concrete classes in results and relations.</param>
		/// <param name="expressionTree">The syntax tree of the NDOql query expression.</param>
		/// <param name="hollow">Determines, if the query results should be hollow objects.</param>
		/// <returns>A Query string.</returns>
		string GenerateQueryString( QueryContextsEntry queryContextsEntry, OqlExpression expressionTree, bool hollow, bool hasSubclassResultsets, List<QueryOrder> orderings, int skip, int take );

		/// <summary>
		/// Creates a query string for the complete query over all types.
		/// </summary>
		/// <param name="queryContextsList">List of all contexts which define possible mutations of concrete classes in results and relations.</param>
		/// <param name="expressionTree">The syntax tree of the NDOql query expression.</param>
		/// <param name="hollow">Determines, if the query results should be hollow objects.</param>
		/// <returns>A query string.</returns>
		/// <remarks>The result can be used for debugging and display purposes or with handlers, which don't support distributed databases.</remarks>
		string GenerateQueryStringForAllTypes( List<QueryContextsEntry> queryContextsList, OqlExpression expressionTree, bool hollow, bool hasSubclassResultsets, List<QueryOrder> orderings, int skip, int take );
	}
}
