//
// Copyright (C) 2002-2009 Mirko Matytschak 
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
using System.Data;

namespace NDO
{

	/// <summary>
	/// State of persistence capable objects
	/// </summary>
	public enum NDOObjectState
	{
		/// <summary>
		/// Transient object state
		/// </summary>
		Transient = 0,
		/// <summary>
		/// Hollow state - object contains only the object id
		/// </summary>
		Hollow,
		/// <summary>
		/// Persistent state - object data are loaded
		/// </summary>
		Persistent,
		/// <summary>
		/// Persistent dirty state - object state has been changed
		/// </summary>
		PersistentDirty,
		/// <summary>
		/// Fresh constructed object
		/// </summary>
		Created,
		/// <summary>
		/// Object to be deleted from the database
		/// </summary>
		Deleted
	}

	/// <summary>
	/// This interface contains the part of IPersistenceCapable, which might be accessible to user code.
	/// </summary>
	public interface IPersistentObject
	{
		/// <summary>
		/// The current state of the object.
		/// </summary>
		NDO.NDOObjectState NDOObjectState
		{
			get;
			set;
		}

		/// <summary>
		/// The unique object ID of this object.
		/// </summary>
		ObjectId NDOObjectId
		{
			get;
			set;
		}


		/// <summary>
		/// Optional time stamp array.
		/// </summary>
		Guid NDOTimeStamp
		{
			get;
			set;
		}

		/// <summary>
		/// Marks an object as changed so that it will be saved to the database with the next call
		/// of PersistenceManager.Save().
		/// </summary>
		void NDOMarkDirty();
	}


	/// <summary>
	/// This is the interface which all persistent classes after enhancement will provide. 
	/// Under normal circumstances NDO users will only need the property NDOObjectId of this interface.
	/// </summary>
	public interface IPersistenceCapable : IPersistentObject
	{
		/// <summary>
		/// The internal state manager that is associated with this persistent object.
		/// The state manager should not be called directly by the application.
		/// </summary>
		IStateManager NDOStateManager
		{
			get;
			set;
		}

		/// <summary>
		/// Initialize all persistent fields from the supplied row. Note that the ObjectId is initialized
		/// by the PersistenceManager outside of this method.
		/// </summary>
		/// <param name="row">the data row that contains all data for this object</param>
		/// <param name="fieldNames">Sorted array of the column names to be read from the row.</param>
		/// <param name="startIndex">Start index of the array.</param>
		void NDORead(System.Data.DataRow row, string[] fieldNames, int startIndex);

		/// <summary>
		/// Initialize the supplied row from the persistent fields of this object. Note that the ObjectId is initialized
		/// by the PersistenceManager outside of this method.
		/// </summary>
		/// <param name="row">the row that should be filled</param>
		/// <param name="fieldNames">Sorted array of the column names to be read from the row.</param>
		/// <param name="startIndex">Start index of the array.</param>
		void NDOWrite(System.Data.DataRow row, string[] fieldNames, int startIndex);


		/// <summary>
		/// Gets a Singleton Persistence Handler for the concrete class of the object. Use the static function NDOSetHandler of the persistent classes to set an own Handler.
		/// </summary>
		IPersistenceHandler NDOHandler
		{
			get;
		}

		/// <summary>
		/// Returns the hash code computed by System.Object, so that NDO can use it before the object is fully initialized.
		/// </summary>
		/// <returns></returns>
		int NDOGetObjectHashCode();

		/// <summary>
		/// Checks, whether a relation is loaded.
		/// </summary>
		/// <param name="ordinal">The ordinal number of the relation.</param>
		/// <returns>True, if the relation is loaded</returns>
		bool NDOGetLoadState(int ordinal);

		/// <summary>
		/// Sets the load state of a relation. This is used, if an relation is loaded by the framework.
		/// </summary>
		/// <param name="ordinal">The ordinal number of the relation.</param>
		/// <param name="isLoaded">The new load state.</param>
		void NDOSetLoadState(int ordinal, bool isLoaded);


		/// <summary>
		/// Gets load state information. This property is used by the framework only.
		/// </summary>
		LoadState NDOLoadState { get; }

	}
}
