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
using System.Threading;
using System.Collections;

namespace NDO
{
	/// <summary>
	/// Don't even think about using this class... 
	/// We included it in the docs to explicitly tell you, that it is verboten to use it.
	/// If you call any of the functions of this class, an immediate disfunction of the
	/// NDO framework will occur.
	/// </summary>
	[CLSCompliant(false)]
	public class _NDOContainerStack
	{
		private class Triple
		{
			public Triple(object p, string f)
			{
				this.parent = p;
				this.field = f;
			}
			object parent;
			public object Parent
			{
				get { return parent; }
				set { parent = value; }
			}
			string field;
			public string Field
			{
				get { return field; }
				set { field = value; }
			}

		}



		public static object RegisterContainer (object parent, object container, Hashtable table, string field)
		{
			IPersistenceCapable pc = parent as IPersistenceCapable;
			if (pc == null)
				goto exit;

			if (pc.NDOStateManager == null || pc.NDOObjectState == NDOObjectState.Transient)
				goto exit;

			if (pc.NDOObjectState != NDOObjectState.Created)
			{
				IList result = pc.NDOStateManager.LoadRelation(pc, field);
				if (result != null)
					container = result;
			}

			if (container == null)
				goto exit;

			if (!table.Contains(container))
				table.Add(container, new Triple(parent, field));
exit:
			return container;
		}

		public static void AddRange(object container, IEnumerable coll, Hashtable table)
		{
			if (container == null)
			{
				throw new NDOException(1, "AddRange: container is null");
			}

			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;

			if (coll == null)
				goto exit;

			foreach(IPersistenceCapable pc in coll)
				stateManager.AddRelatedObject(parent, field, pc);

			exit:
                foreach(object o in coll)
				((IList)container).Add(o);
		}

		public static void InsertRange(object container, int index, IEnumerable coll, Hashtable table)
		{
			if (container == null)
			{
				throw new NDOException(2, "InsertRange: container is null");
			}
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;

			if (coll == null)
				goto exit;

			foreach(IPersistenceCapable pc in coll)
				stateManager.AddRelatedObject(parent, field, pc);

        exit:
            ArrayList temp = new ArrayList();
            foreach (object o in coll)
                temp.Add(o);
            for (int i = temp.Count - 1; i >= 0; i--)
                ((IList)container).Insert(index, temp[i]);
		}


#if !NET11
        public static int RemoveAll(object container, Delegate pred, Hashtable table)
        {
			if (container == null)
				throw new NDOException(3, "RemoveAll: container is null");

            if (pred == null)
				return 0;

            bool validRelation = false;
            IPersistenceCapable parent = null;
            string field = null;
            IStateManager stateManager = null;

			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			field = t.Field;

			stateManager = parent.NDOStateManager;
			if (stateManager == null)
				goto exit;

            validRelation = true;

            exit:
            IList ilist = (IList) container;
            ArrayList toDelete = new ArrayList();
            foreach(object relObj in ilist)
            {
                if((bool) pred.DynamicInvoke(new object[]{relObj}))
                {
                    toDelete.Add(relObj);
                    if (validRelation)
                        stateManager.RemoveRelatedObject(parent, field, (IPersistenceCapable) relObj);
                }
            }
            foreach(object o in toDelete)
                ilist.Remove(o);

            return toDelete.Count;
        }
#endif


        public static void SetRange(object container, int index, ICollection coll, Hashtable table)
		{
			if (container == null)
			{
					throw new NDOException(3, "SetRange: container is null");
			}
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;

			if (coll == null)
				goto exit;

			int lastIndex = index + coll.Count;
			if (lastIndex > ((IList)container).Count)
			{
				ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException();
				throw new ArgumentOutOfRangeException(string.Empty, "SetRange: " + ex.Message);
			}

			for (int i = index; i < lastIndex; i++)
				stateManager.RemoveRelatedObject(parent, field, (IPersistenceCapable)((IList)container)[i]);
			foreach(IPersistenceCapable pc in coll)
				stateManager.AddRelatedObject(parent, field, pc);

			exit:
				((ArrayList)container).SetRange(index, coll);
		}

		public static void RemoveRange(object container, int index, int count, Hashtable table)
		{
			if (container == null)
			{
				throw new NDOException(4, "RemoveRange: container is null");
			}
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;

			int lastIndex = index + count;
			if (lastIndex > ((IList)container).Count)
			{
				ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException();
				throw new ArgumentOutOfRangeException(string.Empty, "RemoveRange: " + ex.Message);
			}

			for (int i = index; i < lastIndex; i++)
				stateManager.RemoveRelatedObject(parent, field, (IPersistenceCapable)((IList)container)[i]);

        exit:
            for (int i = 0; i < count; i++)
                ((IList)container).RemoveAt(index);
		}


        public static int Add(object container, object element, Hashtable table)
		{
			if (container == null)
			{
				if (element != null)
					throw new NDOException(5, "Adding related object of type " + element.GetType().FullName + ": container is null");
				else
					throw new NDOException(5, "Adding related object: container is null");
			}
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;
			stateManager.AddRelatedObject(parent, field, (IPersistenceCapable) element);

exit:
			return ((IList)container).Add(element);			
		}

		public static void Insert(object container, int index, object element, Hashtable table)
		{
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;
			stateManager.AddRelatedObject(parent, field, (IPersistenceCapable) element);

			exit:
				((IList)container).Insert(index, (IPersistenceCapable) element);			
		}

		public static void SetItem(object container, int index, object element, Hashtable table)
		{
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;
			object elToRemove = ((IList)container)[index];
			stateManager.RemoveRelatedObject(parent, field, (IPersistenceCapable) elToRemove);
			stateManager.AddRelatedObject(parent, field, (IPersistenceCapable) element);

			exit:
				((IList)container)[index] = element;
		}


		public static bool Remove(object container, object element, Hashtable table)
		{
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;
			stateManager.RemoveRelatedObject(parent, field, (IPersistenceCapable) element);

        exit:
            IList ilist = ((IList)container);
            int ix = ilist.IndexOf(element);
            if (ix < 0)
                return false;
			ilist.RemoveAt(ix);
            return true;
		}

		public static void RemoveAt(object container, int index, Hashtable table)
		{
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;
			IPersistenceCapable element = (IPersistenceCapable)((IList)container)[index];
			stateManager.RemoveRelatedObject(parent, field, (IPersistenceCapable) element);

			exit:
				((IList)container).RemoveAt(index);			
		}

		public static void Clear(object container, Hashtable table)
		{
			Triple t = (Triple) table[container]; 
			if (t == null)
				goto exit;
			IPersistenceCapable parent = t.Parent as IPersistenceCapable;
			if (parent == null)
				goto exit;

			string field = t.Field;

			IStateManager stateManager;
			if ((stateManager = parent.NDOStateManager) == null)
				goto exit;

			stateManager.RemoveRangeRelatedObjects(parent, field, (IList) container);

			exit:
				((IList)container).Clear();			
		}

		// Maintaining old function names out of compatibility reasons.
#if NDO11
		public static int AddRelatedObject(object container, object element, Hashtable table)
		{
			return Add(container, element, table);
		}
		public static void RemoveRelatedObject(object container, object element, Hashtable table)
		{
			Remove(container, element, table);
		}
		public static void RemoveRelatedObjectAt(object container, int index, Hashtable table)
		{
			RemoveAt(container, index, table);
		}
		public static void RemoveRange(object container, Hashtable table)
		{
			Clear(container, table);
		}
#endif

	}
}
