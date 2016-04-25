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
using NDO.Mapping;

namespace NDO
{
	internal class ObjectLock
	{
		class Triple
		{
			IPersistenceCapable pc;
			IPersistenceCapable child;
			Relation r;
			int hash = 0;

			public Triple(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
			{
				this.r = r;
				this.pc = pc;
				this.child = child;
			}

			public override int GetHashCode()
			{
				if (hash == 0)
				{
					int i1;
					int i2;
					int i3;
					i1 = pc.NDOGetObjectHashCode();
					i2 = child.NDOGetObjectHashCode();
					i3 = r.GetHashCode();
					hash = (i1 ^ i2) ^ i3;
				}
				return hash;
			}

			public override bool Equals(object obj)
			{
				Triple t = obj as Triple;
				if (obj == null)
					return false;
				if (this.GetHashCode() != t.GetHashCode())
					return false;
				if (this.r.Equals(t.r))
				{
					if (Object.ReferenceEquals(this.pc, t.pc)
						&& Object.ReferenceEquals(this.child, t.child))
						return true;
					if (Object.ReferenceEquals(this.child, t.pc)
						&& Object.ReferenceEquals(this.pc, t.child))
						return true;
				}
				return false;
			}


		}

		Hashtable objects = new Hashtable();
		Hashtable relobjects = new Hashtable();

		public void Clear()
		{
			objects.Clear();
			Hashtable relobjects = new Hashtable();
		}

		/// <summary>
		/// Locks a relation and determines, whether the relation was locked before
		/// </summary>
		/// <param name="pc">Parent object</param>
		/// <param name="r">Relation</param>
		/// <param name="child">Child object</param>
		/// <returns>True, if the relation was locked before the function was called.</returns>
		public bool GetLock(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
//			Debug.WriteLine("Lock: " + pc.GetType().Name + " " + r.FieldName + " " + child.GetType().Name);
			Triple t = new Triple(pc, r, child);
			//int hash = this.GetHashCode(); seems to be debug code
			if (this.IsLocked(t))
				return false;
			relobjects.Add(t, null);
			return true;
		}

		public bool IsLocked(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
			Triple t = new Triple(pc, r, child);
			return (relobjects.Contains(t));
		}

		private bool IsLocked(Triple t)
		{
			return relobjects.Contains(t);
		}

		public void Unlock(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
//			Debug.WriteLine("Unlock: " + pc.GetType().Name + " " + r.FieldName + " " + child.GetType().Name);
			Triple t = new Triple(pc, r, child);
			if (relobjects.Contains(t))
				relobjects.Remove(t);
		}

		public bool GetLock(IPersistenceCapable pc)
		{
			PcWrapper pcw = new PcWrapper( pc );
			if ( objects.Contains( pcw ) )
				return false;
			objects.Add(pcw, null);
			return true;
		}


		public void Unlock(IPersistenceCapable pc)
		{
			PcWrapper pcw = new PcWrapper( pc );
			if (objects.Contains(pcw))
				objects.Remove(pcw);
		}

		public bool IsLocked(IPersistenceCapable pc)
		{
			return objects.Contains(new PcWrapper(pc));
		}

		class PcWrapper
		{
			IPersistenceCapable pc;
			public PcWrapper( IPersistenceCapable pc )
			{
				this.pc = pc;
			}

			public override bool Equals( object obj )
			{
				return Object.ReferenceEquals( (IPersistenceCapable) pc, this.pc );
			}

			public override int GetHashCode()
			{
				return pc.NDOGetObjectHashCode();
			}
		}

	}
}
