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
