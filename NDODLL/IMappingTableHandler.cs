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
using System.Data;
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
		void Update(DataSet ds);

		/// <summary>
		/// Called by the NDO Framework. Searches for all DataRows which represent objects contained in a specific relation.
		/// <seealso cref="ObjectId"/>
		/// </summary>
		/// <param name="id">ObjectId of the parent object.</param>
		/// <returns></returns>
		DataTable FindRelatedObjects(ObjectId id);

		/// <summary>
		/// Called by IPersistenceHandler implementations. Constructs a new handler in a polymorphic way.
		/// </summary>
		/// <param name="mappings">Mapping information. See <see cref="NDO.Mapping.NDOMapping"/> class.</param>
		/// <param name="r">The Relation for which the handler is constructed</param>
		/// <param name="ds">The data set containing all data</param>
		void Initialize(NDOMapping mappings, Relation r, DataSet ds);


	}
}
