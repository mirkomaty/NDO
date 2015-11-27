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

namespace NDO
{
	/// <summary>
	/// Helper class to determine if a type is a storable field type.
	/// </summary>
	public class StorableTypes
	{
		/// <summary>
		/// Determines if a type is a storable field type.
		/// </summary>
		/// <param name="t">The type to check.</param>
		/// <returns>True, if the type is storable.</returns>
		public static bool Contains(Type t)
		{
            if (t == null)
                return false;
			if (t == typeof(System.IntPtr))
				return false;
#if !NET11
            if (t.IsGenericParameter)
                return true;
			if (t.FullName.StartsWith("System.Nullable`1"))
				return true;
#endif
			return t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid) || t == typeof(byte[]) || t.IsSubclassOf(typeof(System.Enum));
		}
	}
}
