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