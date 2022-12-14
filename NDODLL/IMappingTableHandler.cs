//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.Data;
using System.Threading.Tasks;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// Implementations of this interface handle ADO.NET operations for relation tables.
	/// One Implementation per ADO.NET-Provider is needed.
	/// One Instance of a handler will be created for every NDO.Mapping.Relation, which has a 
	/// MappingTable.
	/// </summary>
	public interface IMappingTableHandler : IPersistenceHandlerBase
	{
		/// <summary>
		/// Called by the NDO Framework. Update all entries in the given DataTable. Each row represents a row
		/// in a relation table.
		/// </summary>
		/// <param name="ds">The DataSet to update. To extract the right table in your implementation, use <code>DataTable dt = ds.Tables[r.MappingTable.TableName];</code> where r is a NDO.Mapping.Relation.</param>
		Task UpdateAsync(DataSet ds);

		/// <summary>
		/// Called by the NDO Framework. Searches for all DataRows which represent objects contained in a specific relation.
		/// <seealso cref="ObjectId"/>
		/// </summary>
		/// <param name="id">ObjectId of the parent object.</param>
		/// <param name="templateDataSet">The resulting table will be cloned from this DataSet</param>
		/// <returns></returns>
		Task<DataTable> LoadRelatedObjectsAsync(ObjectId id, DataSet templateDataSet);

		/// <summary>
		/// Called by IPersistenceHandler implementations. Constructs a new handler in a polymorphic way.
		/// </summary>
		/// <param name="mappings">Mapping information. See <see cref="NDO.Mapping.NDOMapping"/> class.</param>
		/// <param name="r">The Relation for which the handler is constructed</param>
		void Initialize(NDOMapping mappings, Relation r);

		/// <summary>
		/// Gets the underlying relation of the mapping table.
		/// </summary>
		Relation Relation { get; }
	}
}
