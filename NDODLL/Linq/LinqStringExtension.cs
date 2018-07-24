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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDO.Linq
{
	public static class LinqStringExtension
	{
		/// <summary>
		/// Helper Method for creating the LIKE statement
		/// </summary>
		/// <param name="s"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static bool Like(this object s, object parameter)
		{
			return true;
		}

		/// <summary>
		/// Helper Method for creating BETWEEN statements
		/// </summary>
		/// <param name="s"></param>
		/// <param name="firstParameter"></param>
		/// <param name="secondParameter"></param>
		/// <returns></returns>
		public static bool Between(this object s, object firstParameter, object secondParameter)
		{
			return true;
		}

		/// <summary>
		/// Helper Method for Linq queries.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static ObjectId Oid(this object o)
		{
			// If Oid() is used at the right side of a comparison
			// we try to return the NDOObjectId.
			IPersistentObject po = o as IPersistentObject;
			if (po == null)
				return default(ObjectId);
			return po.NDOObjectId;
		}

		/// <summary>
		/// Helper Method for Linq queries. Used to query for columns of multikey oids.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="index">The index of the Multikey value to compare with</param>
		/// <returns></returns>
		public static object Oid( this object o, int index )
		{
			// If Oid() is used at the right side of a comparison
			// we try to return the NDOObjectId.
			IPersistentObject po = o as IPersistentObject;
			if (po == null)
				return null;
			return po.NDOObjectId.Id.Values[index];
		}
	}
}
