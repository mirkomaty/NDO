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
using System.Collections.Generic;
using System.ComponentModel;

namespace NDO
{
	/// <summary>
	/// Summary description for NDOReadOnlyArrayList.
	/// </summary>
	public class NDOReadOnlyGenericList<T> : List<T>, IList, ICollection<T>, IList<T>
	{
		const string readOnly = "Can't change this list because it's read only";
		public NDOReadOnlyGenericList(IList<T> list) : base(list)
		{
		}

		#region IList<T> Member

        void IList<T>.Insert(int index, T item)
		{
			throw new NotImplementedException(readOnly);
		}

		T IList<T>.this[int index]
		{
			get
			{
				return (T)base[index];
			}
			set
			{
				throw new NotImplementedException(readOnly);
			}
		}

		#endregion

		#region ICollection<T> Member

		bool ICollection<T>.Contains( T item )
		{
			return base.Contains(item);
		}

		void ICollection<T>.CopyTo( T[] array, int arrayIndex )
		{
			base.CopyTo( array, arrayIndex );
		}

		int ICollection<T>.Count
		{
			get { return base.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return true; }
		}


        void ICollection<T>.Add(T item)
		{
			throw new NotImplementedException(readOnly);
		}


		bool ICollection<T>.Remove( T item )
		{
			throw new NotImplementedException(readOnly);
		}


		void ICollection<T>.Clear()
        {
            throw new NotImplementedException(readOnly);
        }

		#endregion


        #region IList Members

        int IList.Add(object value)
        {
            throw new Exception(readOnly);
        }

        void IList.Clear()
        {
            throw new Exception(readOnly);
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            throw new Exception(readOnly);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        void IList.Remove(object value)
        {
            throw new Exception(readOnly);
        }

        void IList.RemoveAt(int index)
        {
            throw new Exception(readOnly);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new Exception(readOnly);
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            int j = index;
            for (int i = 0; i < this.Count; i++)
                array.SetValue(this[i], j++);
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException("IsSynchronized is not supported by this class."); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException("SyncRoot is not supported by this class."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

	}
}
