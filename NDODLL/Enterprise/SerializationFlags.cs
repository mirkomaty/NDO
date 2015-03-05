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


#if ENT
using System;

namespace NDO
{
	/// <summary>
	/// Specifies flags that control serialisation of objects in a container and the way in which the search of child objects is conducted by the serializer.
	/// This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.
	/// </summary>
	/// <remarks>
	/// Requires the NDO Enterprise Edition. The flag MarkAsTransient can be used to perform a kind
	/// of object based replication. Objects, which may be persistent at one machine, can be transfered
	/// to a second machine and treated by the receiving PersistenceManager like a newly created object.
	/// The receiving PersistenceManager will use MakePersistent to store the whole transient object tree.
	/// There is one difference to freshly created objects: If an object id exists, it will be serialized. 
	/// If the NDOOidType-Attribute is valid for the given class, the transfered oids will be reused 
	/// by the receiving PersistenceManager.
	/// </remarks>
	[Flags]
	public enum SerializationFlags
	{
		/// <summary>
		/// Don't serialize any child objects
		/// </summary>
		SerializeNone = 0,
		/// <summary>
		/// Serialize Aggregate Relations
		/// </summary>
		SerializeAggregateRelations = 1,
		/// <summary>
		/// Serialize Composite Relations
		/// </summary>
		SerializeCompositeRelations = 2,
		/// <summary>
		/// Serialize All Relations (Combination of SerializeAggregateRelations | SerializeCompositeRelations)
		/// </summary>
		SerializeAllRelations = 3,
		/// <summary>
		/// Mark objects as transient before serializing. All root objects and all child objects,
		/// which can be reached with composite relations, will be set into the Transient state. 
		/// </summary>
		MarkAsTransient = 4,
		/// <summary>
		/// Serialize objects, even if they are hollow. The ObjectId remains the same. 
        /// To null the ObjectId, combine this flag with NullObjectId.
		/// </summary>
		SerializeHollowObjects = 8,
		/// <summary>
		/// Serialize using the BinaryFormatter.
		/// </summary>
		UseBinaryFormat = 16,
        /// <summary>
        /// The ObjectId will be set to null for the root object and all child objects in the object tree
        /// reachable from the root object via composite relations.
        /// </summary>
        NullObjectId = 32
	}
}



#endif