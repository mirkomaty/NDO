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
using System.Linq;
using System.Linq.Expressions;

namespace NDO.Linq
{
	/// <summary>
	/// This class helps formulating linq queries.
	/// </summary>
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
		/// Helper Method for Linq queries.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static bool In(this object o, IEnumerable list)
        {
            // In Linq queries this code doesn't execute 
            foreach (object o2 in list)
            {
                if (object.ReferenceEquals(o, o2))
                    return true;
            }

            return false;
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

		/// <summary>
		/// Compares two strings and converts the Method call to a Sql operator.
		/// </summary>
		/// <param name="l"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static bool GreaterEqual(this string l, string r)
		{
			return String.Compare( l, r ) >= 0;
		}

		/// <summary>
		/// Compares two strings and converts the Method call to a Sql operator.
		/// </summary>
		/// <param name="l"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static bool GreaterThan( this string l, string r )
		{
#error GreaterThan only supports strings. Should support byte[] for rowversions.			
			return String.Compare( l, r ) > 0;
		}

		/// <summary>
		/// Compares two strings and converts the Method call to a Sql operator.
		/// </summary>
		/// <param name="l"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static bool LowerEqual( this string l, string r )
		{
			return String.Compare( l, r ) <= 0;
		}

		/// <summary>
		/// Compares two strings and converts the Method call to a Sql operator.
		/// </summary>
		/// <param name="l"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static bool LowerThan( this string l, string r )
		{
			return String.Compare( l, r ) < 0;
		}

		/// <summary>
		/// Overrides the Count() linq method to deliver the count using database queries.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="vt"></param>
		/// <returns></returns>
		public static int Count<T>(this VirtualTable<T> vt)
		{
			return vt.Count;
		}

		/// <summary>
		/// Combines two partial expressions with a binary operator.
		/// </summary>
		/// <typeparam name="T">The type of the query result</typeparam>
		/// <param name="ex1">First expression to be combined</param>
		/// <param name="ex2">Second expression to be combined</param>
		/// <param name="expressionType">Operation type. Use And, Or, AndAlso or OrElse</param>
		/// <returns></returns>
		public static Expression<Func<T,bool>>Combine<T>(this Expression<Func<T, bool>> ex1, Expression<Func<T, bool>> ex2, ExpressionType expressionType )
		{
			var lambdaParameter = ex1.Parameters[0];
			var newEx2 = new ReplaceVisitor( ex2.Parameters[0], lambdaParameter ).VisitAndConvert( ex2, String.Empty );
			var combined = Expression.Lambda<Func<T,bool>>( Expression.MakeBinary(expressionType, ex1.Body, newEx2.Body ), lambdaParameter);
			return combined;
		}
	}
}
