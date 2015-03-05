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
using System.Collections;
using System.Diagnostics;

namespace NDO
{
	/// <summary>
	/// Handles the state management of persistence capable classes.
	/// </summary>
	internal class StateManager : IStateManager
	{
		private PersistenceManager pm;

		internal StateManager(PersistenceManager pm)
		{
			this.pm = pm;
		}

		#region Implementation of IStateManager

		public void LoadData(NDO.IPersistenceCapable pc) {
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient, "Transient object shouldn't have a state manager");
			
			if(pc.NDOObjectState == NDOObjectState.Hollow) {
				pm.LoadData(pc);
			}
		}
        
        public void LoadField(NDO.IPersistenceCapable pc, int fieldOrdinal)
        {
            pm.LoadField(pc, fieldOrdinal);
        }

		public void MarkDirty(IPersistenceCapable pc) 
		{
			pm.LoadAndMarkDirty(pc);
		}		
				
		public void AddRelatedObject(IPersistenceCapable pc, string fieldName, IPersistenceCapable relatedObject) {
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient, "Transient object shouldn't have a state manager");
			pm.AddRelatedObject(pc, fieldName, relatedObject);
		}

		public void AddRangeRelatedObjects(IPersistenceCapable pc, string fieldName, IList relatedObjects) {
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient, "Transient object shouldn't have a state manager");
			foreach(IPersistenceCapable relObj in relatedObjects) {
				pm.AddRelatedObject(pc, fieldName, relObj);
			}
		}

		public void RemoveRelatedObject(IPersistenceCapable pc, string fieldName, IPersistenceCapable relatedObject) {
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient, "Transient object shouldn't have a state manager");
			pm.RemoveRelatedObject(pc, fieldName, relatedObject);
		}

		public void RemoveRangeRelatedObjects(IPersistenceCapable pc, string fieldName, IList relatedObjects) {
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient, "Transient object shouldn't have a state manager");
			foreach(IPersistenceCapable relObj in relatedObjects) {
				pm.RemoveRelatedObject(pc, fieldName, relObj);
			}
		}


		public void AssignRelation(IPersistenceCapable pc, string fieldName, IList oldRelation, IList newRelation) {
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient, "Transient object shouldn't have a state manager");
			// This is the same logic as in _NDOContainerStack.RegisterContainer
			if (pc.NDOObjectState != NDOObjectState.Created)
			{
				IList result = LoadRelation(pc, fieldName);
				if (result != null)
					oldRelation = result;
			}
			if(oldRelation != null) {
				RemoveRangeRelatedObjects(pc, fieldName, oldRelation);
			}
			if(newRelation != null) {
				AddRangeRelatedObjects(pc, fieldName, newRelation);
			}
		}

		public IList LoadRelation(IPersistenceCapable pc, string fieldName)
		{
			return pm.LoadRelationInternal(pc, fieldName, true);
		}



		public NDO.IPersistenceManager PersistenceManager {
			get {
				return pm;
			}
		}

		#endregion

	}
}
