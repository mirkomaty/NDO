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


#if nix
using System;
using System.Collections;

namespace NDO.Mapping
{
	internal class ClassHashView : Hashtable, IList
	{
		public ClassHashView() : base()
		{
		}

		public ClassHashView(int n) : base (n)
		{
		}

		public Class this[string s]
		{
			get
			{
				return (Class) base[s];
			}
		}

		public Class this[Class cl]
		{
			get
			{
				return (Class) base[cl.FullName];
			}
		}

		#region IList Member

		public object this[int index]
		{
			get
			{
				if (index >= base.Count)
					throw new ArgumentException("ClassHashView: index > Count");
				IDictionaryEnumerator ie = base.GetEnumerator();
				for (int i = 0; i <= index; i++)
				{
					ie.MoveNext();
				}
				return ((DictionaryEntry) ie.Current).Value;
			}
			set
			{
				throw new NotImplementedException("ClassHashView doesn't support this[int] setter");
			}
		}

		private object GetKey(int index)
		{
			IDictionaryEnumerator ie = base.GetEnumerator();
			for (int i = 0; i <= index; i++)
			{
				ie.MoveNext();
			}
			return ((DictionaryEntry) ie.Current).Key;
		}

		public void RemoveAt(int index)
		{
			base.Remove(GetKey(index));
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException("ClassHashView doesn't support 'Insert(int index, object value)'");
		}

		public override void Remove(object value)
		{
			Class cl = value as Class;
			if ((cl) != null)
				base.Remove(cl.FullName);
			else
				throw new ArgumentException("ClassHashView.Remove: Argument must be of type 'NDO.Mapping.Class'", "value");
		}

		public override bool Contains(object value)
		{
			Class cl = value as Class;
			if ((cl) != null)
				return base.Contains(cl.FullName);
			else
				throw new ArgumentException("ClassHashView.Contains: Argument must be of type 'NDO.Mapping.Class'", "value");
		}


		public int IndexOf(object value)
		{
			Class cl = value as Class;
			if ((cl) != null)
			{
				IDictionaryEnumerator ie = base.GetEnumerator();
				for (int i = 0; i < base.Count; i++)
				{
					ie.MoveNext();
					if (((DictionaryEntry)ie.Current).Key.Equals(value))
						return i;
				}
				return -1;
			}
			else
				throw new ArgumentException("ClassHashView.Remove: Argument must be of type 'NDO.Mapping.Class'", "value");
		}

		public int Add(object value)
		{
			Class cl = value as Class;
			if ((cl) != null)
			{
				base.Add(cl.FullName, cl);
				return this.IndexOf(cl);
			}
			else
				throw new ArgumentException("ClassHashView.Add: Argument must be of type 'NDO.Mapping.Class'", "value");
		}


		#endregion

		#region ICollection Member

		public override void CopyTo(Array array, int index)
		{
			foreach(object o in this)
			{
				array.SetValue(o, index++);
			}
		}


		#endregion

		#region IEnumerable Member

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ClassHashViewEnumerator(base.GetEnumerator());
		}

		public new IEnumerator GetEnumerator()
		{
			return new ClassHashViewEnumerator(base.GetEnumerator());
		}

		#endregion

		#region Enumerator

		private sealed class ClassHashViewEnumerator
			: IEnumerator, ICloneable 
		{			
			private IDictionaryEnumerator ie;

			public ClassHashViewEnumerator(IDictionaryEnumerator ie)
			{
				this.ie = ie;
			}

			public object Clone() 
			{
				return this.MemberwiseClone();
			}


			public object Current 
			{
				get 
				{
					return ((DictionaryEntry)ie.Current).Value;
				}
			}

			public bool MoveNext() 
			{
				return ie.MoveNext();
			}

			public void Reset() 
			{
				ie.Reset();
			}
		}
		
		#endregion

	}
}
#endif