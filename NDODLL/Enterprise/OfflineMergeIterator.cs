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
using System.Reflection;
using System.Collections;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// The standard iterator class used to merge 
	/// a <see cref="ObjectContainer"/> object to a <see cref="OfflinePersistenceManager"/>.
	/// </summary>
	/// <remarks>
	/// Only available in the NDO Enterprise Edition.
	/// You don't need to instantiate objects of this class. NDO will do it automatically,
	/// if <see cref="OfflinePersistenceManager.MergeObjectContainer"/> is called.
	/// </remarks>
	public class OfflineMergeIterator : ISerializationIterator
	{
		Mappings mapping;
		Hashtable objects = new Hashtable();
		IPersistenceManager pm;
		Cache cache;
		IStateManager stateManager;

		/// <summary>
		/// NDO will call this constructor.
		/// </summary>
		/// <remarks>
		/// You don't need to instantiate objects of this class. NDO will do it automatically,
		/// if the <see cref="PersistenceManager.MergeObjectContainer"/> is called.
		/// </remarks>
		/// <param name="stateManager">The standard state manager used by the PersistenceManager.</param>
		/// <param name="cache">The cache used by the PersistenceManager.</param>
		internal OfflineMergeIterator(IStateManager stateManager, Cache cache)
		{
			this.stateManager = stateManager;
			this.pm = stateManager.PersistenceManager;
			this.mapping = (Mappings) this.pm.NDOMapping;
			this.cache = cache;
		}


		/// <summary>
		/// Iterate through an object tree to load child object instances.
		/// </summary>
		/// <param name="root">The root object of the object tree.</param>
		public void Iterate(IPersistenceCapable root)
		{
			Search(root);
		}



		private void Search(IPersistenceCapable pc)
		{
			if (pc.NDOObjectState == NDOObjectState.Transient)
				return;
			if (objects.Contains(pc.NDOObjectId))
				return;
			objects.Add(pc.NDOObjectId, pc);

			this.cache.Register(pc);
			pc.NDOStateManager = this.stateManager;
			if (pc.NDOLoadState.RelationLoadState == null)
				pc.NDOLoadState.RelationLoadState = new BitArray(64);
			
			Class cl = mapping.FindClass(pc.GetType());

			foreach(Relation r in cl.Relations)
			{
				pc.NDOSetLoadState(r.Ordinal, true);  // Always let relations appear loaded
				if (r.Multiplicity == RelationMultiplicity.Element)
				{
					IPersistenceCapable relObj = mapping.GetRelationField(pc, r.FieldName) as IPersistenceCapable;
					if (relObj != null)
					{
						if (relObj.NDOObjectState == NDOObjectState.Hollow)
							mapping.SetRelationField(pc, r.FieldName, null);
						else
							Search(relObj);
					}
				}
				else
				{
					IList l = mapping.GetRelationContainer(pc, r);
                    //if (l == null)
                    //{
                    //    FieldInfo fi = new BaseClassReflector(pc.GetType()).GetField(r.FieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    //    l = (IList) ListCloner.CreateList(fi.FieldType);
                    //}
                    //else
                    if (l != null)					
                    {
                        ArrayList objectsToRemove = new ArrayList();
                        foreach (IPersistenceCapable relObj2 in l)
						{
							if (relObj2.NDOObjectState == NDOObjectState.Hollow)
								objectsToRemove.Add(relObj2);
							else
								Search(relObj2);
						}
						foreach(IPersistenceCapable relObj2 in objectsToRemove)
							l.Remove(relObj2);
					}
				}
			}
		}

	}
}
#endif