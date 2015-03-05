//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.de.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using NDO.Mapping;
using NDOInterfaces;

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

//		/// <summary>
//		/// Called by the NDO Framework. Get the data for the specified object.
//		/// </summary>
//		DataRow Fill(ObjectId id);

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
		void UpdateDeleted0bjects(DataTable dt);

		/// <summary>
		/// Executes a batch of sql statements.
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
		IList ExecuteBatch(string[] statements, IList parameters);

		/// <summary>
		/// Execute a SQL query. The query has to beginn with 'SELECT *' where '*' will be 
		/// replaced with the correct field list depending on the mapping information and hollow state.
		/// </summary>
		/// <param name="expression">SQL expression</param>
		/// <param name="hollow">True if only ids should be read, false if all fields should be read.</param>
		/// <param name="dontTouch">True if expression should not be touched - i. e. if the expression is in sql language</param>
		/// <param name="parameters">A collection of objects, corresponding to the query parameters.</param>
		/// <returns>A DataTable object, containing the query result.</returns>
		DataTable SqlQuery(string expression, bool hollow, bool dontTouch, IList parameters);

		/// <summary>
		/// Gets a Handler which can store data in relation tables. The handler is an Implementation of IMappingTableHandler.
		/// </summary>
		/// <param name="r">Relation information</param>
		/// <returns>The handler</returns>
		IMappingTableHandler GetMappingTableHandler(Relation r);

		/// <summary>
		/// Called by the NDO Framework. Constructs a new handler in a polymorphic way. Each persistent class will have an own handler.
		/// </summary>
		/// <param name="mappings">Mapping information.</param>
		/// <param name="t">Type for which the Handler is constructed.</param>
		/// <param name="ds">DataSet, which is used to clone tables.</param>
		void Initialize(NDOMapping mappings, Type t, DataSet ds);


	}
}