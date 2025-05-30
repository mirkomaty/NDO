//
// Copyright (c) 2002-2024 Mirko Matytschak 
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


namespace NDO
{

	/// <summary>
	/// Callback which is used to alter connection strings at the begin of a transaction.
	/// </summary>
	public delegate string OpenConnectionListener(NDO.Mapping.Connection conn);


	/// <summary>
	/// Common interface for the PersistenceManager classes. 
	/// There are two implementations: PersistenceManager and TransactionalPersistenceManager (NDO Enterprise Edition only).
	/// </summary>
	public interface IPersistenceManager : IPersistenceManagerBase, IDisposable
	{
		/// <summary>
		/// Registers a listener which will be notified, if a new connection is opened.
		/// </summary>
		/// <param name="listener">Delegate of a listener function</param>
		void RegisterConnectionListener(OpenConnectionListener listener);


		/// <summary>
		/// Make an Object persistent
		/// </summary>
		/// <param name="pc">The object, which should be made persistent.</param>
		void MakePersistent(object pc);

		/// <summary>
		/// Make all objects in the list persistent.
		/// </summary>
		/// <param name="list"></param>
		void MakePersistent(IList list);


		/// <summary>
		/// Makes an object transient.
		/// The object can be used afterwards, but changes will not be saved.
		/// </summary>
		/// <param name="pc">Das Objekt</param>
		void MakeTransient(object pc);

		/// <summary>
		/// Make all objects in a list transient.
		/// </summary>
		/// <param name="list">An object list.</param>
		void MakeTransient(IList list);


		/// <summary>
		/// Delete an object from the database.
		/// </summary>
		/// <param name="pc">An IPersistenceCapable </param>
		void Delete(object pc);


		/// <summary>
		/// Delete a list of objects from the database.
		/// </summary>
		/// <param name="list"></param>
		void Delete(IList list);

		/// <summary>
		/// Makes an object hollow. If it gets touched by code, it will get reloaded.
		/// </summary>
		/// <param name="pc">An IPersistenceCapable object.</param>
		void MakeHollow(object pc);


		/// <summary>
		/// Make all objects of a list hollow. The objects will be reloaded from the database if they get touched by code.
		/// </summary>
		/// <param name="list">An object list.</param>
		void MakeHollow(IList list);

		/// <summary>
		/// Make all objects in the object cache hollow.
		/// </summary>
		void MakeAllHollow();


		/// <summary>
		/// Gets the requested object. If it is in the cache, the cached object is returned, otherwise, a new (hollow)
		/// instance of the object is returned. In either case, the DB is not accessed!
		/// </summary>
		/// <param name="id">Object id</param>
		/// <returns>The object to retrieve in hollow state</returns>		
		IPersistenceCapable FindObject(ObjectId id);

		/// <summary>
		/// Gets all objects of a given class.
		/// </summary>
		/// <param name="t">the type of the class</param>
		/// <returns>A list of all persistent objects of the given class. Subclasses will not be included in the result set.</returns>
		IList GetClassExtent(Type t);

		/// <summary>
		/// Gets all objects of a given class.
		/// </summary>
		/// <param name="t">The type of the class.</param>
		/// <param name="hollow">If true, return objects in hollow state instead of persistent state.</param>
		/// <returns>A list of all persistent objects of the given class.</returns>
		/// <remarks>Subclasses of the given type are not fetched.</remarks>
		IList GetClassExtent(Type t, bool hollow);

		/// <summary>
		/// Refreshes all unlocked objects in the cache.
		/// </summary>
		void RefreshAll();


		/// <summary>
		/// Refreshes all objects in the given list.
		/// </summary>
		/// <param name="list">An object list.</param>
		void Refresh(IList list);


		/// <summary>
		/// Reload an object from the database.
		/// </summary>
		/// <param name="pc">The object to be reloaded.</param>
		void Refresh(object pc);


		/// <summary>
		/// Save all changes made to the objects.
		/// </summary>
		/// <param name="deferCommit">Determines, if the commit should be immediately or if it should be deferred to a later call to Save().</param>
		void Save( bool deferCommit = false );

		/// <summary>
		/// Discard all changes and restore the state of all objects.
		/// </summary>
		void Abort();

		/// <summary>
		/// Close the PersistenceManger.
		/// </summary>
		void Close();

		/// <summary>
		/// Remove all unused entries from the cache.
		/// </summary>
		void CleanupCache();

		/// <summary>
		/// Remove all objects from the cache, even those that are still in use (but inactive).
		/// Objects that have changed and are part of the current transaction
		/// are not removed.
		/// </summary>
		/// <remarks>
		/// This should be used with care! If an object with the same object ID is requested multiple times from the PM, 
		/// the same object is returned every time. However, after the cache is unloaded, new instances are created and
		/// returned.
		/// </remarks>
		void UnloadCache();


		/// <summary>
		/// If hollow mode is true, all objects are retired to hollow after each transaction (Save, Abort).
		/// </summary>
		bool HollowMode  
		{
			get;
			set;
		}

	
		/// <summary>
		/// Sets or gets transaction mode. Uses TransactionMode enum.
		/// </summary>
		TransactionMode TransactionMode
		{
			get; set;
		}


		/// <summary>
		/// Sets or gets the Isolation Level. Setting the Isolation Level during an transaction will have effect only on Connections which will be opened after the setting.
		/// </summary>
		IsolationLevel IsolationLevel
		{
			get; set;
		}

		/// <summary>
		/// Indicates, whether the PersistenceManager has changed objects in the cache
		/// </summary>
		bool HasChanges
		{
			get;
		}

		/// <summary>
		/// Indicates, if there is a listener registered for the IdGenerationEvent.
		/// </summary>
		bool HasOwnerCreatedIds
		{
			get;
		}

		/// <summary>
		/// Loads all fields of an object
		/// </summary>
		/// <param name="pc"></param>
		void LoadData(object pc);

		/// <summary>
		/// Loads a certain field of an object
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="fieldNumber"></param>
		void LoadField(object pc, int fieldNumber);
		
		/// <summary>
		/// Loads related objects
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="fieldName"></param>
		/// <param name="hollow"></param>
		void LoadRelation(object pc, string fieldName, bool hollow);
		
		/// <summary>
		/// Creates an ObjectId
		/// </summary>
		/// <param name="oidToClone"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		ObjectId NewObjectId(ObjectId oidToClone, Type t);

		/// <summary>
		/// Resets an object to its previous state
		/// </summary>
		/// <param name="pc"></param>
		void Restore(object pc);

	}


	
}
