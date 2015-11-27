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
using System.Collections;
using NDO.Logging;


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
		void Save();

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
		/// Create a new Query object. The query will return objects of the given type, selected by the 
		/// expression.
		/// </summary>
		/// <param name="t">Result type</param>
		/// <param name="expression">NDOql expression - syntax is similar to the where clause of a SQL statement.</param>
		/// <returns>An object of type Query</returns>
		Query NewQuery(Type t, string expression);


		/// <summary>
		/// Create a new Query object. The query will return objects of the given type, selected by the 
		/// expression.
		/// </summary>
		/// <param name="t">Result type</param>
		/// <param name="expression">NDOql expression - syntax is similar to the where clause of a SQL statement.</param>
		/// <param name="hollow">If true, return objects in hollow state</param>
		/// <returns>An object of type Query</returns>
		Query NewQuery(Type t, string expression, bool hollow);
		

		/// <summary>
		/// Create a new Query object. The query will return objects of the given type, selected by the 
		/// expression. A NDOql query expects expressions using field names of the application classes. 
		/// A SQL query expects expressions with column names of the underlying db. SQL expressions should
		/// start with 'SELECT *'. The query should return data rows which correspond to objects of the type given in parameter 1. 
		/// </summary>
		/// <param name="t">Result type</param>
		/// <param name="expression">NDOql expression - syntax is similar to the where clause of a SQL statement.</param>
		/// <param name="hollow">If true, return objects in hollow state</param>
		/// <param name="queryLanguage">The language of the query - NDOql or SQL.</param>
		/// <returns>An object of type Query</returns>
		Query NewQuery(Type t, string expression, bool hollow, Query.Language queryLanguage);

		
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


		bool HasChanges
		{
			get;
		}

		bool HasOwnerCreatedIds
		{
			get;
		}

		void LoadData(object pc);

        void LoadField(object pc, int fieldNumber);

		void LoadRelation(object pc, string fieldName, bool hollow);
		
		ObjectId NewObjectId(ObjectId oidToClone, Type t);

		//ObjectId NewObjectId(object keyData, Type t);
		
		void Restore(object pc);

	}


	
}
