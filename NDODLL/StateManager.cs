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
