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

namespace NDO
{
	/// <summary>
	/// This interface is for internal use of the NDO Framework. Don't use it in your application code.
	/// </summary>
	public interface IStateManager
	{
		/// <summary>
		/// Retrieve the corresponding Persistence Manager
		/// </summary>
		IPersistenceManager PersistenceManager {
			get;
		}

		/// <summary>
		/// Called to load the data of an object.
		/// </summary>
		/// <param name="pc">the persistennt object</param>
		void LoadData(IPersistenceCapable pc);

        /// <summary>
        /// Called to load field data of an object.
        /// </summary>
        /// <param name="pc">The persistennt object</param>
        /// <param name="fieldOrdinal">A number with which NDO identifies the field.</param>
        void LoadField(IPersistenceCapable pc, int fieldOrdinal);


		/// <summary>
		/// Mark the object as dirty. It will be saved automatically. It will call LoadData internally, if necessary.
		/// </summary>
		/// <param name="pc">the persistennt object</param>
		void MarkDirty(IPersistenceCapable pc);

		// Relation Management
		/// <summary>
		/// Add a related object to the parent object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <param name="relatedObject">the object that should be added</param>
		void AddRelatedObject(IPersistenceCapable pc, string fieldName, IPersistenceCapable relatedObject);

		/// <summary>
		/// Add a list of related objects to the parent object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <param name="relatedObjects">the list of objects that should be added</param>
		void AddRangeRelatedObjects(IPersistenceCapable pc, string fieldName, IList relatedObjects);

		/// <summary>
		/// Remove a related object from the parent object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <param name="relatedObject">the object that should be removed</param>
		void RemoveRelatedObject(IPersistenceCapable pc, string fieldName, IPersistenceCapable relatedObject);

		/// <summary>
		/// Remove a list of objects from the parent object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <param name="relatedObjects">the list of objects that should be removed</param>
		void RemoveRangeRelatedObjects(IPersistenceCapable pc, string fieldName, IList relatedObjects);

		/// <summary>
		/// Assign a new list to an existing relation. Old objects are removed from the relation, new one are added to the relation.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <param name="oldRelation">current content of the relation</param>
		/// <param name="newRelation">future content of the relation</param>
		void AssignRelation(IPersistenceCapable pc, string fieldName, IList oldRelation, IList newRelation);

		/// <summary>
		/// Loads related objects of a relation as hollow objects. This function will be called, 
		/// if a relation field is touched and the relation is not marked as loaded.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <returns>If the relation is a n-Relation, the container will be returned</returns>
		IList LoadRelation(IPersistenceCapable pc, string fieldName);

	}
}
