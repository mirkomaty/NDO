using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	/// <summary>
	/// Provides a polymorphic interface to NDOQuery&lt;T&gt;
	/// </summary>
	public interface IQuery
	{
		/// <summary>
		/// Executes the Query and returns a result set.
		/// </summary>
		/// <returns>A list of objects</returns>
		IList Execute();
		/// <summary>
		/// Executes the query and returns a single object.
		/// </summary>
		/// <returns>A single object</returns>
		IPersistenceCapable ExecuteSingle();

		/// <summary>
        /// Executes the query and returns a single object.
        /// </summary>
        /// <param name="throwIfResultCountIsWrong"></param>
        /// <returns>The fetched object or null, if the object wasn't found and throwIfResultCountIsWrong is false.</returns>
        /// <remarks>
        /// If throwIfResultCountIsWrong is true, an Exception will be throwed, if the result count isn't exactly 1. 
        /// If throwIfResultCountIsWrong is false and the query has more than one result, the first of the results will be returned.
        /// </remarks>
		IPersistenceCapable ExecuteSingle(bool throwIfResultCountIsWrong);
		
		/// <summary>
		/// Gets the Parameters Collection
		/// </summary>
		ICollection<object> Parameters { get; }

		/// <summary>
		/// Gets the Parameters Collection
		/// </summary>
		ICollection<QueryOrder> Orderings { get; }

		/// <summary>
		/// Executes an aggregate operation
		/// </summary>
		/// <param name="field"></param>
		/// <param name="aggregateType"></param>
		/// <returns></returns>
		object ExecuteAggregate( string field, AggregateType aggregateType );

		/// <summary>
		/// Defines, if the query should return subclasses of the given Query Type
		/// </summary>
		bool AllowSubclasses { get; set; }

		/// <summary>
		/// Returns the generated SQL Query
		/// </summary>
		string GeneratedQuery { get; }

		/// <summary>
		/// Gets or sets the amount of elements to be skipped in an ordered query
		/// </summary>
		/// <remarks>This is for paging support.</remarks>
		int Skip { get; set; }

		/// <summary>
		/// Gets or sets the amount of elements to be taken in an ordered query
		/// </summary>
		/// <remarks>This is for paging support.</remarks>
		int Take { get; set; }
	}
}
