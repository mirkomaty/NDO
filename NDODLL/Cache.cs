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


using System;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace NDO
{
	/// <summary>
	/// This is a cache of persistent objects. While the object is not part of a transaction, only a weak reference is held.
	/// While the object is part of a transaction, a strong reference is held.
	/// </summary>
#if !DEBUG
	internal class Cache
#else
	public class Cache
#endif
	{
		/// <summary>
		/// Because all entries are put in a hash table, we use a class instead of a struct (which would be boxed anyway)
		/// </summary>
#if !DEBUG
		internal class Entry 
#else
		public class Entry 
#endif
		{
			public IPersistenceCapable pc;
			public DataRow row;
			public IList relations;

			public Entry(IPersistenceCapable pc, DataRow row, IList relations) {
				this.pc = pc;
				this.row = row;
				this.relations = relations;
			}
		}

		private Hashtable lockedObjects = new Hashtable(100);
		private Hashtable objects = new Hashtable(100);

		/// <summary>
		/// Defaul Constructor
		/// </summary>
		public Cache()
		{
		}

		public bool IsRegistered(IPersistenceCapable pc)
		{
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.IsRegistered: ObjectId of an object of type " + pc.GetType().FullName + " is null.");
            return objects.Contains(pc.NDOObjectId) || lockedObjects.Contains(pc.NDOObjectId);
		}

		/// <summary>
		/// Register an object
		/// </summary>
		/// <param name="pc">The Object</param>
        public void Register(IPersistenceCapable pc)
        {
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.Register: ObjectId of an object of type " + pc.GetType().FullName + " is null.");

            if (!pc.NDOObjectId.IsValid())
            {
                throw new ArgumentException("Cannot register object with invalid key: " + pc);
            }

            if (objects.Contains(pc.NDOObjectId) || lockedObjects.Contains(pc.NDOObjectId))
            {
                throw new ArgumentException("Object already cached: " + pc.NDOObjectId, "pc");
            }
            if (pc.NDOObjectState != NDO.NDOObjectState.Hollow && pc.NDOObjectState != NDO.NDOObjectState.Persistent && pc.NDOObjectState != NDO.NDOObjectState.Transient)
            {
                throw new ArgumentException("Object in wrong state " + pc.NDOObjectId, "pc");
            }
            objects.Add(pc.NDOObjectId, new WeakReference(pc));
        }

		/// <summary>
		/// Re-Register an object in the locked cache
		/// </summary>
		/// <param name="pc">The Object</param>
		/// <param name="row">The data row, which holds the original state of the object</param>
		/// <param name="relations">A list of all relations of the object.</param>		
		public void RegisterLockedObject(IPersistenceCapable pc, DataRow row, IList relations)
		{
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.RegisterLockedObject: ObjectId of an object of type " + pc.GetType().FullName + " is null.");

            if (!pc.NDOObjectId.IsValid()) 
			{
				throw new ArgumentException("Cannot register object with invalid key: " + pc);
			}

			if(lockedObjects.Contains(pc.NDOObjectId)) 
			{
				throw new ArgumentException("Object already cached: " + pc.NDOObjectId, "pc");
			}
			Cache.Entry e = new Entry(pc, row, relations);
			lockedObjects.Add(pc.NDOObjectId, e);
		}


		/// <summary>
		/// Put an object in the cache; remove older versions of the object if existent
		/// </summary>
		/// <param name="pc">The object to put into the cache</param>
        public void UpdateCache(IPersistenceCapable pc)
        {
            ObjectId oid = pc.NDOObjectId;
            if (lockedObjects.Contains(oid))
                return;
            Debug.Assert(pc.NDOObjectState == NDOObjectState.Persistent || pc.NDOObjectState == NDOObjectState.Hollow);
            WeakReference objRef = (WeakReference)objects[pc.NDOObjectId];
            if (objRef != null)
            {
                objRef.Target = pc;
            }
            else
            {
                objects.Add(pc.NDOObjectId, new WeakReference(pc));
            }
        }

		/// <summary>
		/// Remove an object from the cache
		/// </summary>
		/// <param name="pc">The object to remove</param>
        public void Deregister(IPersistenceCapable pc)
        {
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.Deregister: ObjectId of an object of type " + pc.GetType().FullName + " is null.");

            objects.Remove(pc.NDOObjectId);
            lockedObjects.Remove(pc.NDOObjectId);
        }

		/// <summary>
		/// Remove an object from the locked objects cache
		/// </summary>
		/// <param name="pc">The object to remove</param>
		public void DeregisterLockedObject(IPersistenceCapable pc) 
		{
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.DeregisterLockedObject: ObjectId of an object of type " + pc.GetType().FullName + " is null.");
            lockedObjects.Remove(pc.NDOObjectId);
		}

		/// <summary>
		/// Unload all unused objects, i.e., those that lost their weak reference.
		/// </summary>
		public void Cleanup() 
		{
			ArrayList unusedObjects = new ArrayList();
			foreach(DictionaryEntry e in objects) {
				if(!((WeakReference)e.Value).IsAlive) {
					unusedObjects.Add(e.Key);
				}
			}
			foreach(object key in unusedObjects) {
				objects.Remove(key);
			}
//			Debug.WriteLine("Cache unloaded " + unusedObjects.Count + " unused objects");
		}

		/// <summary>
		/// Unload all inactive objects.
		/// </summary>
		public void Unload() {
//			Debug.WriteLine("Cache unloaded " + objects.Count + " objects");
			objects.Clear();
		}

		/// <summary>
		/// Retrieve an object in the cache
		/// </summary>
		/// <param name="id">Object ID of the object</param>
		/// <returns></returns>
        public IPersistenceCapable GetObject(ObjectId id)
        {
            Entry e = (Entry)lockedObjects[id];
            IPersistenceCapable pc = e != null ? e.pc : null;
            if (pc == null)
            {
                WeakReference objRef = (WeakReference)objects[id];
                if (objRef != null)
                {
                    pc = (IPersistenceCapable)objRef.Target;
                    if (pc == null)
                    {
                        objects.Remove(id);
                    }
                }
            }
            return pc;
        }

		/// <summary>
		/// Retrieve a DataRow related to an object
		/// </summary>
		/// <param name="pc"></param>
		/// <returns></returns>
        public DataRow GetDataRow(IPersistenceCapable pc)
        {
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.GetDataRow: ObjectId of an object of type " + pc.GetType().FullName + " is null.");
            Entry e = (Entry)lockedObjects[pc.NDOObjectId];
            return e == null ? null : e.row;
        }

		/// <summary>
		/// Lock an object for use in a transaction; store the relatetd DataRow
		/// </summary>
		/// <param name="pc">The object to lock</param>
		/// <param name="row">The DataRow</param>
		/// <param name="relations">Relation info for the Object</param>
        public void Lock(IPersistenceCapable pc, DataRow row, IList relations)
        {
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.Lock: ObjectId of an object of type " + pc.GetType().FullName + " is null.");
            objects.Remove(pc.NDOObjectId);
            lockedObjects.Add(pc.NDOObjectId, new Entry(pc, row, relations));
        }

		/// <summary>
		/// Unlock an object
		/// </summary>
		/// <param name="pc">The object to unlock</param>
        public void Unlock(IPersistenceCapable pc)
        {
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.Unlock: ObjectId of an object of type " + pc.GetType().FullName + " is null.");
            Entry e = (Entry)lockedObjects[pc.NDOObjectId];
            if (e != null && e.relations != null)
            {
                // support GC
                e.relations.Clear();
                e.relations = null;
            }
            lockedObjects.Remove(pc.NDOObjectId);
            objects.Add(pc.NDOObjectId, new WeakReference(pc));
        }

		/// <summary>
		/// Returns all locked objects
		/// </summary>
		public ICollection LockedObjects {
			get { return lockedObjects.Values; }
		}


		/// <summary>
		/// Returns all known objects
		/// </summary>
		public IList AllObjects
		{
			get
			{
				Cleanup();
				ArrayList al = new ArrayList(this.lockedObjects.Count + this.objects.Count);

				foreach(DictionaryEntry de in this.lockedObjects)
				{
					Cache.Entry ce = (Cache.Entry) de.Value;
					if (ce.row.RowState != DataRowState.Deleted)
						al.Add(ce);
				}
				foreach(DictionaryEntry de in this.objects)
				{
					WeakReference weakReference = (WeakReference) de.Value;
					if (weakReference != null && weakReference.Target != null)
						al.Add(weakReference.Target);
				}
				return al;
			}
		}

		/// <summary>
		/// Check if object is locked
		/// </summary>
		/// <param name="pc">Object to check</param>
		/// <returns>True if object is in the LockedObjects Collection</returns>
		public bool IsLocked(IPersistenceCapable pc)
		{
            if (pc.NDOObjectId == null)
                throw new InternalException(60, "Cache.IsLocked: ObjectId of an object of type " + pc.GetType().FullName + " is null.");
            foreach (Cache.Entry e in lockedObjects.Values)
				if (e.pc == pc) return true;
			return false;
		}

		/// <summary>
		/// Returns all objects which are not locked
		/// </summary>
		public ICollection UnlockedObjects {
			get { return objects.Values; }
		}

		/// <summary>
		/// Unlock all locked objects
		/// </summary>
		public void UnlockAll() {
			foreach(Entry e in lockedObjects.Values) {
				objects.Add(e.pc.NDOObjectId, new WeakReference(e.pc));
				if(e.relations != null) {
					// support GC
					e.relations.Clear();
					e.relations = null;
				}
			}
			lockedObjects.Clear();
		}
	}
}