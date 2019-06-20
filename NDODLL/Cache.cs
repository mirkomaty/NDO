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

#pragma warning disable 1591

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NDO.Mapping;
using NDO.ShortId;

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
			public List<KeyValuePair<Relation, object>> relations;

			public Entry( IPersistenceCapable pc, DataRow row, List<KeyValuePair<Relation, object>> relations )
			{
				this.pc = pc;
				this.row = row;
				this.relations = relations;
			}
		}

		private Dictionary<ObjectId, Entry> lockedObjects = new Dictionary<ObjectId, Entry>( 100 );
		private Dictionary<ObjectId, WeakReference<IPersistenceCapable>> objects = new Dictionary<ObjectId, WeakReference<IPersistenceCapable>>( 100 );

		/// <summary>
		/// Defaul Constructor
		/// </summary>
		public Cache()
		{
		}

		public bool IsRegistered( IPersistenceCapable pc )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.IsRegistered: ObjectId of an object of type " + pc.GetType().FullName + " is null." );
			return objects.ContainsKey( pc.NDOObjectId ) || lockedObjects.ContainsKey( pc.NDOObjectId );
		}

		/// <summary>
		/// Register an object
		/// </summary>
		/// <param name="pc">The Object</param>
		public void Register( IPersistenceCapable pc )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.Register: ObjectId of an object of type " + pc.GetType().FullName + " is null." );

			if (!pc.NDOObjectId.IsValid())
			{
				throw new ArgumentException( "Cannot register object with invalid key: " + pc );
			}

			if (objects.ContainsKey( pc.NDOObjectId ) || lockedObjects.ContainsKey( pc.NDOObjectId ))
			{
				throw new ArgumentException( "Object already cached: " + pc.NDOObjectId, "pc" );
			}
			if (pc.NDOObjectState != NDO.NDOObjectState.Hollow && pc.NDOObjectState != NDO.NDOObjectState.Persistent && pc.NDOObjectState != NDO.NDOObjectState.Transient)
			{
				throw new ArgumentException( "Object in wrong state " + pc.NDOObjectId, "pc" );
			}
			objects.Add( pc.NDOObjectId, new WeakReference<IPersistenceCapable>( pc ) );
		}

		/// <summary>
		/// Re-Register an object in the locked cache
		/// </summary>
		/// <param name="pc">The Object</param>
		/// <param name="row">The data row, which holds the original state of the object</param>
		/// <param name="relations">A list of all relations of the object.</param>		
		public void RegisterLockedObject( IPersistenceCapable pc, DataRow row, List<KeyValuePair<Relation, object>> relations )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.RegisterLockedObject: ObjectId of an object of type " + pc.GetType().FullName + " is null." );

			if (!pc.NDOObjectId.IsValid())
			{
				throw new ArgumentException( "Cannot register object with invalid key: " + pc );
			}

			if (lockedObjects.ContainsKey( pc.NDOObjectId ))
			{
				throw new ArgumentException( "Object already cached: " + pc.NDOObjectId, "pc" );
			}
			Cache.Entry e = new Entry( pc, row, relations );
			lockedObjects.Add( pc.NDOObjectId, e );
		}


		/// <summary>
		/// Put an object in the cache; remove older versions of the object if existent
		/// </summary>
		/// <param name="pc">The object to put into the cache</param>
		public void UpdateCache( IPersistenceCapable pc )
		{
			ObjectId oid = pc.NDOObjectId;
			if (lockedObjects.ContainsKey( oid ))
				return;
			Debug.Assert( pc.NDOObjectState == NDOObjectState.Persistent || pc.NDOObjectState == NDOObjectState.Hollow );
			WeakReference<IPersistenceCapable> objRef = null;
			if (objects.TryGetValue( oid, out objRef ))
			{
				objRef.SetTarget( pc );
			}
			else
			{
				objects.Add( pc.NDOObjectId, new WeakReference<IPersistenceCapable>( pc ) );
			}
		}

		/// <summary>
		/// Remove an object from the cache
		/// </summary>
		/// <param name="pc">The object to remove</param>
		public void Deregister( IPersistenceCapable pc )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.Deregister: ObjectId of an object of type " + pc.GetType().FullName + " is null." );

			objects.Remove( pc.NDOObjectId );
			lockedObjects.Remove( pc.NDOObjectId );
		}

		/// <summary>
		/// Remove an object from the locked objects cache
		/// </summary>
		/// <param name="pc">The object to remove</param>
		public void DeregisterLockedObject( IPersistenceCapable pc )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.DeregisterLockedObject: ObjectId of an object of type " + pc.GetType().FullName + " is null." );
			lockedObjects.Remove( pc.NDOObjectId );
		}

		/// <summary>
		/// Unload all unused objects, i.e., those that lost their weak reference.
		/// </summary>
		public void Cleanup()
		{
			IPersistenceCapable target; // dummy
			List<ObjectId> unusedObjects = new List<ObjectId>();
			foreach (var e in objects)
			{
				if (!e.Value.TryGetTarget( out target ))
				{
					unusedObjects.Add( e.Key );
				}
			}
			foreach (var key in unusedObjects)
			{
				objects.Remove( key );
			}
			//			Debug.WriteLine("Cache unloaded " + unusedObjects.Count + " unused objects");
		}

		/// <summary>
		/// Unload all inactive objects.
		/// </summary>
		public void Unload()
		{
			//			Debug.WriteLine("Cache unloaded " + objects.Count + " objects");
			objects.Clear();
		}

		/// <summary>
		/// Retrieve an object in the cache
		/// </summary>
		/// <param name="id">Object ID of the object</param>
		/// <returns></returns>
		public IPersistenceCapable GetObject( ObjectId id )
		{
			Entry e = null;
			lockedObjects.TryGetValue( id, out e );
			IPersistenceCapable pc = e?.pc;
			if (pc == null)
			{
				WeakReference<IPersistenceCapable> objRef = null;
				objects.TryGetValue( id, out objRef );
				if (objRef == null || !objRef.TryGetTarget( out pc ))
				{
					objects.Remove( id );
				}
			}
			return pc;
		}

		/// <summary>
		/// Retrieve a DataRow related to an object
		/// </summary>
		/// <param name="pc"></param>
		/// <returns></returns>
		public DataRow GetDataRow( IPersistenceCapable pc )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.GetDataRow: ObjectId of an object of type " + pc.GetType().FullName + " is null." );
			Entry e = null;
			lockedObjects.TryGetValue( pc.NDOObjectId, out e );
			return e?.row;
		}

		/// <summary>
		/// Lock an object for use in a transaction; store the relatetd DataRow
		/// </summary>
		/// <param name="pc">The object to lock</param>
		/// <param name="row">The DataRow</param>
		/// <param name="relations">Relation info for the Object</param>
		public void Lock( IPersistenceCapable pc, DataRow row, List<KeyValuePair<Relation, object>> relations )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.Lock: ObjectId of an object of type " + pc.GetType().FullName + " is null." );
			objects.Remove( pc.NDOObjectId );
			lockedObjects.Add( pc.NDOObjectId, new Entry( pc, row, relations ) );
		}

		Entry GetEntry(IPersistenceCapable pc)
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.GetEntry: ObjectId of an object of type " + pc.GetType().FullName + " is null." );
			Entry e = null;
			lockedObjects.TryGetValue( pc.NDOObjectId, out e );
			return e;
		}


		/// <summary>
		/// Unlock an object
		/// </summary>
		/// <param name="pc">The object to unlock</param>
		public void Unlock( IPersistenceCapable pc )
		{
			Entry e = GetEntry(pc);
			lockedObjects.Remove( pc.NDOObjectId );
			objects.Add( pc.NDOObjectId, new WeakReference<IPersistenceCapable>( pc ) );
		}

		/// <summary>
		/// Returns all locked objects
		/// </summary>
		public ICollection<Entry> LockedObjects
		{
			get { return lockedObjects.Values; }
		}


		/// <summary>
		/// Returns all known objects
		/// </summary>
		public List<IPersistenceCapable> AllObjects
		{
			get
			{
				Cleanup();
				List<IPersistenceCapable> al = new List<IPersistenceCapable>( this.lockedObjects.Count + this.objects.Count );

				foreach (var de in this.lockedObjects)
				{
					Cache.Entry ce = de.Value;
					if (ce.row.RowState != DataRowState.Deleted)
						al.Add( ce.pc );
				}

				foreach (var de in this.objects)
				{
					var weakReference = de.Value;
					if (weakReference != null)
					{
						IPersistenceCapable pc;
						if (weakReference.TryGetTarget( out pc ))
							al.Add( pc );
					}
				}
				return al;
			}
		}

		/// <summary>
		/// Check if object is locked
		/// </summary>
		/// <param name="pc">Object to check</param>
		/// <returns>True if object is in the LockedObjects Collection</returns>
		public bool IsLocked( IPersistenceCapable pc )
		{
			if (pc.NDOObjectId == null)
				throw new InternalException( 60, "Cache.IsLocked: ObjectId of an object of type " + pc.GetType().FullName + " is null." );
			return lockedObjects.ContainsKey( pc.NDOObjectId );
		}

		/// <summary>
		/// Returns all objects which are not locked
		/// </summary>
		public IEnumerable<IPersistenceCapable> UnlockedObjects
		{
			get
			{
				IPersistenceCapable pc = null;
				foreach (var wr in objects.Values)
				{
					if (wr.TryGetTarget( out pc ))
						yield return pc;
				}
			}
		}

		/// <summary>
		/// Unlock all locked objects
		/// </summary>
		public void UnlockAll()
		{
			foreach (Entry e in lockedObjects.Values)
			{
				objects.Add( e.pc.NDOObjectId, new WeakReference<IPersistenceCapable>( e.pc ) );
				if (e.relations != null)
				{
					// support GC
					e.relations.Clear();
					e.relations = null;
				}
			}

			lockedObjects.Clear();
		}
	}
}

#pragma warning restore 1591