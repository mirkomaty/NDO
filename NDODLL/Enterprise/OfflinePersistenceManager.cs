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
using System.Collections;
using System.Collections.Generic;
using NDO.Mapping;
using System.Linq;
using System.Threading.Tasks;

namespace NDO
{
	/// <summary>
	/// This class manages offline objects. It receives objects with an object container 
	/// and records changes in a ChangeContainer.
	/// </summary>
	/// <remarks>
	/// Only available in the NDO Enterprise Edition.
	/// This class doesn't implement all functions of the IPersistenceManager interface, 
	/// because they wouldn't make much sense in a offline scenario. 
	/// Not implemented functions will throw an NotImplementedException.
	/// </remarks>
	public class OfflinePersistenceManager : PersistenceManager
	{
        /// <summary>
        /// Constructs an OfflinePersistenceManager object
        /// </summary>
		public OfflinePersistenceManager() : base ()
		{
		}

        /// <summary>
        /// Constructs an OfflinePersistenceManager object
        /// </summary>
        /// <param name="mappingFile"></param>
		public OfflinePersistenceManager(string mappingFile) : base(mappingFile)
		{
		}

        /// <summary>
        /// Constructs an OfflinePersistenceManager object
        /// </summary>
        /// <param name="mapping"></param>
		public OfflinePersistenceManager(NDOMapping mapping) : base(mapping)
		{
		}

		#region IPersistenceManager Member


		/// <summary>
		/// Returns all objects of a given type, which are listed in the persistence manager cache.
		/// </summary>
		/// <param name="t">Type of the objects, which are searched for.</param>
		/// <returns></returns>
		public override System.Collections.IList GetClassExtent(Type t)
		{
			var result = new List<IPersistenceCapable>();
			foreach(var ce in cache.LockedObjects)
			{
				if (t == ce.pc.GetType())
					result.Add(ce.pc);
			}
			foreach(var o in cache.UnlockedObjects)
			{
				if (t == o.GetType())
					result.Add(o);
			}
			return result;
		}


        /// <summary>
        /// Merges objects from an ObjectContainer into the Opm cache
        /// </summary>
        /// <param name="oc"></param>
		public void MergeObjectContainer(ObjectContainer oc)
		{
			OfflineMergeIterator omi = new OfflineMergeIterator(this.sm, this.cache);
			foreach(Object o in oc.RootObjects)
			{
				omi.Iterate(this.CheckPc(o));
			}
		}

		ChangeSetContainer CreateChangeSet(bool acceptChanges)
		{
			ChangeSetContainer csc = new ChangeSetContainer();
			var addedObjects = new List<IPersistenceCapable>();
			var deletedObjects = new List<ObjectId>();
			var changedObjects = new List<IPersistenceCapable>();
			foreach(var cacheEntry in cache.LockedObjects)
			{
				IPersistenceCapable pc = cacheEntry.pc;
				if (pc.NDOObjectState == NDOObjectState.Created)
				{
					addedObjects.Add(pc);
					if (acceptChanges)
						pc.NDOObjectState = NDOObjectState.Persistent;
				}
				else if (pc.NDOObjectState == NDOObjectState.Deleted)
				{
					deletedObjects.Add(pc.NDOObjectId);
					if (acceptChanges)
						pc.NDOObjectState = NDOObjectState.Transient;
				}
				else if (pc.NDOObjectState == NDOObjectState.PersistentDirty)
				{
					changedObjects.Add(pc);
					if (acceptChanges)
						pc.NDOObjectState = NDOObjectState.Persistent;
				}
			}
			csc.RelationChanges = RelationChanges.ToList();
			csc.ChangedObjects = changedObjects;
			csc.AddedObjects = addedObjects;
			csc.DeletedObjects = deletedObjects;
			if (acceptChanges)
			{
				cache.UnlockAll();
				RelationChanges.Clear();
			}
			return csc;
		}      

  //      /// <inheritdoc/> 
  //      protected override void InternalRemoveRelatedObject(IPersistenceCapable pc, NDO.Mapping.Relation r, IPersistenceCapable child, bool calledFromStateManager)
		//{
		//	int delCount = cache.LockedObjects.Count;
		//	NDOObjectState oldState = child.NDOObjectState;
		//	base.InternalRemoveRelatedObject (pc, r, child, calledFromStateManager);

  //          if (r.Composition && cache.LockedObjects.Count > delCount)
		//	{
		//		cache.Unlock(child);
		//		child.NDOObjectState = oldState;
		//	}
  //      }


        /// <summary>
        /// Determines, if any objects are new, changed or deleted.
        /// </summary>
        public override bool HasChanges
		{
			get 
			{
				return cache.LockedObjects.Count > 0 || RelationChanges.Count > 0;
			}
		}

        /// <inheritdoc/> 
		protected override void WriteLostForeignKeysToRow(NDO.Mapping.Class cl, IPersistenceCapable pc, DataRow row)
		{
			// We simply don't write the keys.
		}


		/// <summary>
		/// Gets a serializable container, which contains all changes made to objects and relations.
		/// The serialization will be in binary format. All changes will be accepted, which means,
		/// a subsequent call to GetChangeSet will return an empty ChangeSetContainer.
		/// </summary>
		/// <returns>A ChangeSetContainer object.</returns>
		public virtual ChangeSetContainer GetChangeSet()
		{
			return GetChangeSet(true);
		}


		/// <summary>
		/// Gets a serializable container, which contains all changes made to objects and relations.
		/// Serialization will be in binary format.
		/// </summary>
		/// <param name="acceptChanges">
		/// If true, a subsequent call to GetChangeSet would return an empty ChangeSetContainer.
		/// If false, a subsequent call to GetChangeSet would return the same ChangeSetContainer.
		/// <seealso cref="NDO.ChangeSetContainer"/>
		/// </param>
		/// <returns>A ChangeSetContainer object.</returns>
		public virtual ChangeSetContainer GetChangeSet(bool acceptChanges)
		{
			ChangeSetContainer csc = CreateChangeSet(acceptChanges);
            return csc;
		}


		#region Not implemented functions

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="hollow"></param>
		/// <returns></returns>
		public override System.Collections.IList GetClassExtent(Type t, bool hollow)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		public override void AbortTransaction()
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}


		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="listener"></param>
		public override void RegisterConnectionListener(NDO.OpenConnectionListener listener)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// This property gets always the value false. The setter throws a NotImplementedException.
		/// </summary>
		public override bool HollowMode
		{
			get
			{
				return false;
			}
			set
			{				
				throw new NotImplementedException("This function isn't supported in offline mode");
			}
		}


		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="pc"></param>
		public override void MakeHollow(object pc)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="list"></param>
		public override void MakeHollow(System.Collections.IList list)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		public override void MakeAllHollow()
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="pc"></param>
		public override void LoadData(object pc)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="fieldName"></param>
		/// <param name="hollow"></param>
		public override void LoadRelation(object pc, string fieldName, bool hollow)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		public override void RefreshAll()
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="list"></param>
		public override void Refresh(IList list)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		/// <param name="pc"></param>
		public override void Refresh(object pc)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		/// <summary>
		/// The function is not implemented. 
		/// A NotImplementedException will be thrown after calling that function.
		/// </summary>
		public override Task SaveAsync(bool deferCommit = false)
		{
			throw new NotImplementedException("This function isn't supported in offline mode");
		}

		#endregion


		#endregion

		#region IDisposable Member


		/// <summary>
		/// Safely disposes the PersistenceManager.
		/// </summary>
		public override void Dispose()
		{
			this.Close();
		}

		#endregion
	}
}

