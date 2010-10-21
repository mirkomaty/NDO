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

namespace NDO
{
	/// <summary>
	/// Represents a collection of parameters of a query.
	/// </summary>
	public class NDOParameterCollection : ArrayList
	{
		internal NDOParameterCollection()
		{
		}

		/// <summary>
		/// Adds a parameter to the NDOParameterCollection.
		/// </summary>
		/// <param name="value">The parameter to add to the collection.</param>
		/// <returns>The index of the added parameter object.</returns>
		public int Add(Query.Parameter value)
		{
			return base.Add (value);
		}

		/// <summary>
		/// Inserts a parameter into the NDOParameterCollection at the given index.
		/// </summary>
		/// <param name="index">The zero-based index where the parameter is to be inserted within the collection.</param>
		/// <param name="value">The parameter to add to the collection.</param>
		public void Insert(int index, Query.Parameter value)
		{
			base.Insert (index, value);
		}

		/// <summary>
		/// Removes the specified Parameter from the collection.
		/// </summary>
		/// <param name="obj">A Query.Parameter to remove from the collection.</param>
		public void Remove(Query.Parameter obj)
		{
			base.Remove (obj);
		}

		/// <summary>
		/// Gets the location of a parameter in the collection.
		/// </summary>
		/// <param name="value">The Query.Parameter to locate.</param>
		/// <returns>The zero based location of the parameter in the collection.</returns>
		public int IndexOf(Query.Parameter value)
		{
			return base.IndexOf (value);
		}

		/// <summary>
		/// Gets the location of a parameter in the collection.
		/// </summary>
		/// <param name="value">The Query.Parameter to locate.</param>
		/// <param name="startIndex">The zero based start index for the search.</param>
		/// <returns>The zero based location of the parameter in the collection.</returns>
		public int IndexOf(Query.Parameter value, int startIndex)
		{
			return base.IndexOf (value, startIndex);
		}

		/// <summary>
		/// Gets the location of a parameter in the collection.
		/// </summary>
		/// <param name="value">The Query.Parameter to locate.</param>
		/// <param name="startIndex">The zero based start index for the search.</param>
		/// <param name="count">The size of the range to search for the parameter object.</param>
		/// <returns>The zero based location of the parameter in the collection.</returns>
		public int IndexOf(Query.Parameter value, int startIndex, int count)
		{
			return base.IndexOf (value, startIndex, count);
		}


	}
}
