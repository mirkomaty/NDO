//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
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
// there is a commercial licence available at www.netdataobjects.com.
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
using System.Collections;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// NDO Standard implementation of ISerializationIterator. The serializer iterates through
	/// all object relations to make shure, that all child objects are loaded before the 
	/// parent objects are getting serialized.
	/// </summary>
	/// <remarks>
	/// Only available in the NDO Enterprise Edition.
	/// </remarks>
	public class SerializationIterator : ISerializationIterator
	{
		SerializationFlags serFlags;
		Mappings mapping;
		Hashtable objects;
		IPersistenceManager pm;

		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="serFlags">Determines, how the search for child objects is conducted.</param>
		public SerializationIterator(SerializationFlags serFlags)
		{
			this.serFlags = serFlags;
			objects = new Hashtable();
		}


		/// <summary>
		/// Iterate through an object tree to load child object instances.
		/// </summary>
		/// <param name="root">The root object of the object tree.</param>
		public void Iterate(IPersistenceCapable root)
		{
			this.pm = ObjectHelper.GetPersistenceManager(root);
			this.mapping = (Mappings) this.pm.NDOMapping;
			if (root.NDOObjectState == NDOObjectState.Hollow)
				this.pm.LoadData(root);
			Search(root);
			if (MarkAsTransient)
			{
				root.NDOObjectState = NDOObjectState.Transient;
				//root.NDOObjectId = null;
			}
		}

		internal bool SerializeCompositeRelations
		{
			get { return (serFlags & SerializationFlags.SerializeCompositeRelations) != 0; }
		}

		internal bool SerializeAggregateRelations
		{
			get { return (serFlags & SerializationFlags.SerializeAggregateRelations) != 0; }
		}

		internal bool SerializeRelations
		{
			get { return (serFlags & SerializationFlags.SerializeAllRelations) != 0; }
		}

		internal bool SerializeHollowObjects
		{
			get { return (serFlags & SerializationFlags.SerializeHollowObjects) != 0; }
		}

		internal bool MarkAsTransient
		{
			get { return (serFlags & SerializationFlags.MarkAsTransient) != 0; }
		}

        internal bool NullObjectId
        {
            get { return (serFlags & SerializationFlags.NullObjectId) != 0; }
        }

		private void Search(IPersistenceCapable pc)
		{
			if (objects.Contains(pc))
				return;
			objects.Add(pc, null);

			if (SerializeRelations)
			{
				Class cl = mapping.FindClass(pc.GetType());

				foreach(Relation r in cl.Relations)
				{
					if (r.Composition && SerializeCompositeRelations
						|| !r.Composition && SerializeAggregateRelations)
					{
						pm.LoadRelation(pc, r.FieldName, false);
						if (r.Multiplicity == RelationMultiplicity.Element)
						{
							IPersistenceCapable relObj = mapping.GetRelationField(pc, r.FieldName) as IPersistenceCapable;
							if (relObj == null)
								continue;
							if (relObj.NDOObjectState == NDOObjectState.Hollow)
								pm.LoadData(relObj);
							Search(relObj);
							if (MarkAsTransient && r.Composition)
							{
								relObj.NDOObjectState = NDOObjectState.Transient;
                                if (NullObjectId)
								    relObj.NDOObjectId = null;
							}

						}
						else
						{
							IList l = mapping.GetRelationContainer(pc, r);
							foreach(IPersistenceCapable relObj2 in l)
							{
								if (relObj2.NDOObjectState == NDOObjectState.Hollow)
									pm.LoadData(relObj2);
								Search(relObj2);
								if (MarkAsTransient && r.Composition)
								{
									relObj2.NDOObjectState = NDOObjectState.Transient;
                                    if (NullObjectId)
									    relObj2.NDOObjectId = null;
								}
							}
						}
					}
				}
			}
		}

	}
}

#endif