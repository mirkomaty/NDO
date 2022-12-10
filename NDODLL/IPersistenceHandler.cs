//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using NDO.Mapping;
using NDO.SqlPersistenceHandling;
using NDOInterfaces;
using NDOql.Expressions;

namespace NDO
{

	/// <summary>
	/// An event of this type should be raised by implementations of IPersistenceHandler, if a concurrency exception occurs.
	/// </summary>
	public delegate void ConcurrencyErrorHandler(DBConcurrencyException exception);

	/// <summary>
	/// This is the common interface of all PersistenceHandler classes. 
	/// A PersistenceHandler class stores or retrieves status information for one or more business classes.
	/// The default implementation of this interface in NDO is the NDOPersistenceHandler class.
	/// See also: <a href="Extensions.html">NDO extension interfaces</a>
	/// </summary>
	public interface IPersistenceHandler : IPersistenceHandlerBase, IRowUpdateListener
	{
		/// <summary>
		/// Internal handler for concurrency situations
		/// </summary>
		event ConcurrencyErrorHandler ConcurrencyError;

		/// <summary>
		/// Called by the NDO Framework. Write all changed rows back to DB
		/// </summary>
		/// <param name="dt">The data table containing the rows to be updated</param>
		void Update(DataTable dt);

		/// <summary>
		/// Called by the NDO Framework. Special Update function which processes only deleted rows. This is necessary, 
		/// because deleted rows must be updated in reverse order as changed or created rows.
		/// </summary>
		/// <param name="dt">DataTable containing the rows to delete</param>
		void UpdateDeletedObjects(DataTable dt);

		/// <summary>
		/// Executes a batch of sql statements async.
		/// </summary>
		/// <param name="statements">Each element in the array is a sql statement.</param>
		/// <param name="parameters">A list of parameters (see remarks).</param>
		/// <returns>An IList with Hashtables, containing the Name/Value pairs of the results.</returns>
		/// <remarks>
		/// For emty resultsets an empty Hashtable will be returned. 
		/// If parameters is a NDOParameterCollection, the parameters in the collection are valid for 
		/// all subqueries. If parameters is an ordinary IList, NDO expects to find a NDOParameterCollection 
		/// for each subquery. If an element is null, no parameters are submitted for the given query.
		/// </remarks>
		Task<IList<Dictionary<string, object>>> ExecuteBatchAsync( string[] statements, IList parameters );

		/// <summary>
		/// Execute a SQL query.
		/// </summary>
		/// <param name="expression">SQL expression</param>
		/// <param name="parameters">A collection of objects, corresponding to the query parameters.</param>
		/// <param name="templateDataset">The DataSet from which the DataTable for the results is cloned</param>
		/// <returns>A DataTable object, containing the query result.</returns>
		DataTable PerformQuery( string expression, IList parameters, DataSet templateDataset );

		/// <summary>
		/// Execute a SQL query.
		/// </summary>
		/// <param name="expression">SQL expression</param>
		/// <param name="parameters">A collection of objects, corresponding to the query parameters.</param>
		/// <param name="templateDataset">The DataSet from which the DataTable for the results is cloned</param>
		/// <returns>A DataTable object, containing the query result.</returns>
		Task<DataTable> PerformQueryAsync( string expression, IList parameters, DataSet templateDataset );


		/// <summary>
		/// Gets a Handler which can store data in relation tables. The handler is an Implementation of IMappingTableHandler.
		/// </summary>
		/// <param name="r">Relation information</param>
		/// <returns>The handler</returns>
		IMappingTableHandler GetMappingTableHandler(Relation r);

		/// <summary>
		/// Called by the NDO Framework. Constructs a new handler in a polymorphic way. Each persistent class will have an own handler.
		/// </summary>
		/// <param name="ndoMapping">Mapping information.</param>
		/// <param name="t">Type for which the Handler is constructed.</param>
		/// <param name="disposeCallback">Method to be called at the end of the usage. The method can be used to push back the object to the PersistenceHandlerPool.</param>
		void Initialize(NDOMapping ndoMapping, Type t, Action<Type, IPersistenceHandler> disposeCallback );
	}
}