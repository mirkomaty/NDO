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

namespace NDO
{
	internal class KeyValuePair
	{
		object key;
		public object Key
		{
			get { return key; }
			set { key = value; }
		}
		object value;
		public object Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
		public KeyValuePair(object key, object value)
		{
			this.value = value;
			this.key = key;
		}
	}
	internal class KeyValueList : ArrayList
	{		
		public KeyValueList() : base()
		{
		}

		public KeyValueList(int size) : base (size)
		{
		}

		public new KeyValuePair this[int index]
		{
			get
			{
				if (base[index] == null)
					return null;

				return (KeyValuePair) base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		public KeyValuePair this[string s]
		{
			get 
			{ 
				foreach(KeyValuePair de in this)
					if ((string) de.Key == s)
						return de;
				return null;
			}
		}

		public int Add(KeyValuePair value)
		{
			return base.Add (value);
		}

	}
}
