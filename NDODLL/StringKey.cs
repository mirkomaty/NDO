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

namespace NDO
{
	/// <summary>
	/// This is a key class that can be used as an object identifier.
	/// The key value might be changed without affecting the object.
	/// </summary>
	public class StringKey : Key
	{
		string key;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="t">Type of the object to which the key belongs</param>
		/// <param name="key">Key value</param>
		public StringKey(Type t, string key) : base(t) {
			this.key = key;
		}

		/// <summary>
		/// Gets or sets the Key value of the StringKey
		/// </summary>
		public string Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}

		/// <summary>
		/// Gets or sets the Key value of the StringKey
		/// </summary>
		public override object Value 
        {
			get { return key; }
            set { this.key = value.ToString(); }
        }

		/// <summary>
		/// Checks two keys for equality
		/// </summary>
		/// <param name="obj">Key object to compare with</param>
		/// <returns>True if keys are equal</returns>
		public override bool Equals(object obj) {
			if(base.Equals(obj)) {
				StringKey k = obj as StringKey;
				if(k != null && key.Equals(k.key))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a hash code to be used in hash tables
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode() {
			return key.GetHashCode() * 21047 + base.GetHashCode();
		}

		/// <summary>
		/// Returns a string representation of the key
		/// </summary>
		/// <returns>String representation of the key</returns>
		public override string ToString() {
			return key;
		}

		const string quote = "'";

		/// <summary>
		/// Gets a string to be used in SQL or NDOql expressions
		/// </summary>
		public override string ExpressionString
		{
			get
			{
				return quote + key + quote;
			}
		}

	}
}

#endif