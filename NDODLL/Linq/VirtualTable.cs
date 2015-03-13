//
// Copyright (C) 2002-2015 Mirko Matytschak 
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
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using NDO;
using System.Linq.Expressions;

namespace NDO.Linq
{
	/// <summary>
	/// This class represents a virtual table which allows for Linq queries against the NDO data store.
	/// </summary>
	/// <typeparam name="T">The type of the result element class.</typeparam>
    public class VirtualTable<T>
    {
        PersistenceManager pm;
        List<string> prefetches = new List<string>();
        NDOQuery<T> ndoquery = null;
        ArrayList parameters = new ArrayList();

		/// <summary>
		/// Constructs a virtual table object
		/// </summary>
		/// <param name="pm"></param>
        public VirtualTable(PersistenceManager pm)
        {
            this.pm = pm;
        }

		/// <summary>
		/// Implements the Linq select statement
		/// </summary>
		/// <typeparam name="S"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
        public List<S> Select<S>(Func<T, S> selector)
        {
			return this.ResultTable.Select( selector ).ToList();
        }

		/// <summary>
		/// Implements the Linq orderby statement
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <param name="keySelector"></param>
		/// <returns></returns>
        public VirtualTable<T> OrderBy<K>(Expression<Func<T,K>> keySelector)
        {
            ExpressionTreeTransformer transformer = 
                new ExpressionTreeTransformer((LambdaExpression)keySelector);
            string field = transformer.Transform();
            ndoquery.Orderings.Add(new Query.AscendingOrder(field));
            return this;
        }

		/// <summary>
		/// Implements the Linq orderby ... descending statement
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <param name="keySelector"></param>
		/// <returns></returns>
        public VirtualTable<T> OrderByDescending<K>(Expression<Func<T,K>> keySelector)
        {
            ExpressionTreeTransformer transformer = 
                new ExpressionTreeTransformer((LambdaExpression)keySelector);
            string field = transformer.Transform();
            ndoquery.Orderings.Add(new Query.DescendingOrder(field));
            return this;
        }

		/// <summary>
		/// Implements the Linq orderby statement
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <param name="keySelector"></param>
		/// <returns></returns>
        public VirtualTable<T> ThenBy<K>(Expression<Func<T,K>> keySelector)
        {
            return OrderBy<K>(keySelector);
        }

		/// <summary>
		/// Implements the Linq orderby statement
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <param name="keySelector"></param>
		/// <returns></returns>
        public VirtualTable<T> ThenByDescending<K>(Expression<Func<T,K>> keySelector)
        {
            return OrderByDescending<K>(keySelector);
        }

		/// <summary>
		/// Implements the Linq where clause
		/// </summary>
		/// <param name="expr"></param>
		/// <returns></returns>
        public VirtualTable<T>Where(Expression<Func<T,bool>>expr)
        {
            // Determine the result type of the query.
            // Construct a dummy instance of T to ask it.
            T instance = (T) Activator.CreateInstance(typeof(T));
            
			// Transform the expression to NDOql
            ExpressionTreeTransformer transformer = new ExpressionTreeTransformer((LambdaExpression)expr);
            string query = transformer.Transform();

            // Create the NDO query
            this.ndoquery = new NDOQuery<T>( pm, query);
            
			// Add the parameters collected by the transformer
            foreach(object o in transformer.Parameters)
                this.ndoquery.Parameters.Add(o);

            // Add the prefetch definitions
			this.ndoquery.Prefetches = new ArrayList( this.prefetches );
            return this;
        }

		/// <summary>
		/// Executes the Query and returns the result table
		/// </summary>
        public List<T> ResultTable
        {
            get 
            {
				if (this.ndoquery == null)
					this.ndoquery = new NDOQuery<T>( pm );
                return this.ndoquery.Execute();
            }
        }

		/// <summary>
		/// Returns the prefetches.
		/// </summary>
        public IEnumerable<string> Prefetches
        {
            get { return this.prefetches; }
        }

		/// <summary>
		/// Converts the VirtualTable to a List.
		/// </summary>
		/// <remarks>This operator makes sure that the results of the Linq query are usable as List classes.</remarks>
		/// <param name="table"></param>
		/// <returns></returns>
        public static implicit operator List<T>(VirtualTable<T> table)
        {
            return table.ResultTable;
        }

		public T FirstOrDefault()
		{
			List<T> result = ResultTable;
			if (result.Count == 0)
				return default(T);
			return result[0];
		}
	}
}