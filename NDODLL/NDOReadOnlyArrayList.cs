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
using System.ComponentModel;

namespace NDO
{
	/// <summary>
	/// Summary description for NDOReadOnlyArrayList.
	/// </summary>
	public class NDOReadOnlyArrayList : IList, ITypedList
	{
		protected IList list;
		const string readOnly = "Can't change this list because it's read only";
		public NDOReadOnlyArrayList(IList list)
		{
			this.list = list;
		}

		#region Implementation of IList
		public void RemoveAt(int index)
		{
			throw new NotImplementedException(readOnly);
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException(readOnly);		
		}

		public void Remove(object value)
		{
			throw new NotImplementedException(readOnly);		
		}

		public bool Contains(object value)
		{
			return this.list.Contains(value);
		}

		public void Clear()
		{
			throw new NotImplementedException(readOnly);		
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf(readOnly);
		}

		public int Add(object value)
		{
			throw new NotImplementedException(readOnly);		
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public object this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				throw new NotImplementedException(readOnly);		
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region Implementation of ICollection
		public void CopyTo(System.Array array, int index)
		{
			list.CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get
			{
				return list.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return list.SyncRoot;
			}
		}
		#endregion

		#region Implementation of IEnumerable
		public System.Collections.IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region Implementation of ITypedList
		public System.ComponentModel.PropertyDescriptorCollection GetItemProperties(System.ComponentModel.PropertyDescriptor[] listAccessors)
		{
			ITypedList tlist = list as ITypedList;
			if (tlist == null)
				return null;
			else
				return tlist.GetItemProperties(listAccessors);
		}

		public string GetListName(System.ComponentModel.PropertyDescriptor[] listAccessors)
		{
			ITypedList tlist = list as ITypedList;
			if (tlist == null)
				return null;
			else
				return tlist.GetListName(listAccessors);
		}
		#endregion
	}
}
