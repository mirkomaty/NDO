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
	public class Int32Key : Key
	{
		int key;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="t">Persistent class type</param>
		/// <param name="key">Key value</param>
		public Int32Key(Type t, int key) : base(t)
		{
			this.key = key;
		}

		/// <summary>
		/// Retrieve the Key value
		/// </summary>
		public int Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}

		/// <summary>
		/// Gets or sets the Key value (polymorphic version)
		/// </summary>
		public override object Value 
        {
			get { return key; }
			set 
            { 
                if (!typeof(int).IsAssignableFrom(value.GetType()))
                    throw new NDOException(108, "Can't assign value of type " + value.GetType() + " to Int32Key.Value.");
                this.key = (int)value;
            }
		}

		/// <summary>
		/// Test if two keys are equal
		/// </summary>
		/// <param name="obj">Object to compare with</param>
		/// <returns>True if keys are of the same host type and Key value</returns>
		public override bool Equals(object obj) {
			if(base.Equals(obj)) {
				Int32Key k = obj as Int32Key;
				return k != null && key == k.key;
			}
			return false;
		}

		/// <summary>
		/// Generate hash code
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode() {
			return key.GetHashCode() * 21047 + base.GetHashCode();
		}

		/// <summary>
		/// Converts Key value into a string value
		/// </summary>
		/// <returns>String value</returns>
		public override string ToString() {
			return key.ToString();
		}

		/// <summary>
		/// Returns a string which can be used in a NDOql expression
		/// </summary>
		public override string ExpressionString
		{
			get
			{
				return key.ToString();
			}
		}
	}
}
#endif