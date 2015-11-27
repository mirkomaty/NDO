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
using System.Reflection;
using System.Data;


namespace NDO
{
	/// <summary>
	/// Summary description for ObjectHelper.
	/// </summary>
	public class ObjectHelper
	{

		#region Implementation of IPersistenceCapable

		/// <summary>
		/// Initialize all persistent fields from the supplied row. Note that the ObjectId is initialized
		/// by the PersistenceManager outside of this method.
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		/// <param name="row">The DataRow, containing the object state.</param>
		/// <param name="fieldNames">A String array with the field names.</param>
		/// <remarks>The field names and right order of the names in the array will be listed by the enhancer in the file FieldNames.txt.</remarks>
		public static void NDORead(object pc, System.Data.DataRow row, string[] fieldNames)
		{
			((IPersistenceCapable) pc).NDORead(row, fieldNames, 0);
		}

		/// <summary>
		/// Initialize the supplied row from the persistent fields of this object. Note that the ObjectId is initialized
		/// by the PersistenceManager outside of this method.
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		/// <param name="row">the row that should be filled</param>
		/// <param name="fieldNames">A String array with the field names.</param>
		/// <remarks>The field names and right order of the names in the array will be listed by the enhancer in the file FieldNames.txt.</remarks>
		public static void NDOWrite(object pc, System.Data.DataRow row, string[] fieldNames)
		{
			((IPersistenceCapable) pc).NDOWrite(row, fieldNames, 0);
		}


		/// <summary>
		/// Get the current state of the object.
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		/// <returns></returns>
		public static NDO.NDOObjectState GetObjectState(object pc)
		{
			return ((IPersistenceCapable) pc).NDOObjectState;
		}


		/// <summary>
		/// Get the object id of the object.
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		/// <returns></returns>
		public static NDO.ObjectId GetObjectId(object pc)
		{
			return ((IPersistenceCapable) pc).NDOObjectId;
		}


		/// <summary>
		/// Get the Persistence Handler for the object - usually you shouldn't need this function
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		/// <returns></returns>
		public static NDO.IPersistenceHandler GetNDOHandler(object pc)
		{
			return ((IPersistenceCapable) pc).NDOHandler;
		}


		/// <summary>
		/// Set the Persistence Handler for all objects of the same type as the parameter object
		/// </summary>
		/// <param name="pc">Persistence capable object</param>
		/// <param name="ph">The handler object</param>
		public static void SetNDOHandler(object pc, IPersistenceHandler ph)
		{
			pc.GetType().InvokeMember("NDOSetPersistenceHandler", BindingFlags.InvokeMethod | BindingFlags.Static, null, null, new object[]{ph});
		}

		#endregion

		
		/// <summary>
		/// Returns the PersistenceManager where the object is registered.
		/// </summary>
		/// <param name="pc">The object.</param>
		/// <returns>The PersistenceManager object or null if the object is transient.</returns>
		public static IPersistenceManager GetPersistenceManager(object pc)
		{
			IStateManager sm = ((IPersistenceCapable) pc).NDOStateManager;
			if (sm == null)
				return null;
			return sm.PersistenceManager;
		}

		/// <summary>
		/// Marks an object as dirty, if the enhanced code isn't able to do so by itself.
		/// </summary>
		/// <param name="pc">An object of a persistent class.</param>
		public static void MarkDirty(object pc)
		{
			IPersistenceCapable ipc = (IPersistenceCapable) pc;
			IStateManager sm = ipc.NDOStateManager;
			if (sm == null)
				return;
			sm.MarkDirty(ipc);
		}

	}
}
