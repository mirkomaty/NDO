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