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
		/// <summary>
		/// Gets or sets an action which can be used to mark all elements of a graph as transient
		/// </summary>
		/// <remarks>You can replace the action to add additional functionality</remarks>
        public static Action<IPersistenceCapable> MarkAsTransientAction;
        private readonly Predicate<Relation> relationSelector;
        private readonly Action<IPersistenceCapable> actionOnIncludedObject;

        Mappings mapping;
		Hashtable objects;
		IPersistenceManager pm;

        static SerializationIterator()
        {
            MarkAsTransientAction = pc => pc.NDOObjectState = NDOObjectState.Transient;
        }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="relationSelector">Determines whether a certain relation should be included in the container. Null means that no relations should be searched.</param>
        /// <param name="actionOnRelatedObject">Action to be executed on every object included in the container. Use this action to mark objects as NDOTransient.</param>
        /// <remarks>
        /// You can use SerializationIterator.MarkAsTransientAction as parameter to make all found objects NDOTransient.
        /// </remarks>
        public SerializationIterator(Predicate<Relation> relationSelector = null, Action<IPersistenceCapable> actionOnRelatedObject = null)
		{
			objects = new Hashtable();
            this.relationSelector = relationSelector;
            this.actionOnIncludedObject = actionOnRelatedObject;
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
			if (actionOnIncludedObject != null)
			{
                actionOnIncludedObject( root );
			}
		}

		private void Search(IPersistenceCapable pc)
		{
			if (objects.Contains(pc))
				return;
			objects.Add(pc, null);

			if (this.relationSelector != null)
			{
				Class cl = mapping.FindClass(pc.GetType());

				foreach(Relation r in cl.Relations)
				{
					if (this.relationSelector(r))
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
                            this.actionOnIncludedObject?.Invoke( relObj );
                        }
						else
						{
							IList l = mapping.GetRelationContainer(pc, r);
							foreach(IPersistenceCapable relObj2 in l)
							{
								if (relObj2.NDOObjectState == NDOObjectState.Hollow)
									pm.LoadData(relObj2);
								Search(relObj2);
                                this.actionOnIncludedObject?.Invoke( relObj2 );
                            }
                        }
					}
				}
			}
		}

	}
}
