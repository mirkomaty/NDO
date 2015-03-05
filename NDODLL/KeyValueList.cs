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
