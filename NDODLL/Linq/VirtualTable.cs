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
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using NDO;
using NDO.Query;
using System.Linq.Expressions;

namespace NDO.Linq
{
	/// <summary>
	/// This class represents a virtual table which allows for Linq queries against the NDO data store.
	/// </summary>
	/// <typeparam name="T">The type of the result element class.</typeparam>
    public class VirtualTable<T> : IEnumerable<T> //where T: IPersistenceCapable
    {
        PersistenceManager pm;
        List<string> prefetches = new List<string>();
        NDOQuery<T> ndoquery = null;
        ArrayList parameters = new ArrayList();
		int skip = -1;
		int take = -1;

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
            Ndoquery.Orderings.Add(new Query.AscendingOrder(field));
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
            Ndoquery.Orderings.Add(new Query.DescendingOrder(field));
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

			// If we have a paged view
			// define the take and skip parameters
			if (this.take > -1)
				ndoquery.Take = take;
			if (this.skip > -1)
				ndoquery.Skip = skip;
			
			// Add the prefetch definitions
#warning Änderung überprüfen			// this.ndoquery.Prefetches = new ArrayList( this.prefetches );
			this.ndoquery.Prefetches = this.prefetches;
            return this;
        }

		/// <summary>
		/// Executes the Query and returns the result table
		/// </summary>
        public List<T> ResultTable
        {
            get 
            {
                return Ndoquery.Execute();
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
		/// Gets the Query String of the generated query.
		/// </summary>
		public string QueryString
		{
			get
			{
				return Ndoquery.GeneratedQuery;
			}
		}

		private NDOQuery<T> Ndoquery
		{
			get
			{
				if (this.ndoquery == null)
				{
					this.ndoquery = new NDOQuery<T>( pm );
					if (this.take > -1)
						ndoquery.Take = take;
					if (this.skip > -1)
						ndoquery.Skip = skip;
					this.ndoquery.Prefetches = this.prefetches;
				}
                return this.ndoquery;
			}
		}

		/// <summary>
		/// Supports getting a paged view with Linq
		/// </summary>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		public VirtualTable<T> PagedView(int skip, int take)
		{
			this.skip = skip;
			this.take = take;
			return this;
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

		/// <summary>
		/// Gets a single object with the query
		/// </summary>
		/// <returns></returns>
		public T FirstOrDefault()
		{
			return Ndoquery.ExecuteSingle( false );
		}

		/// <summary>
		/// Gets a single object with the query
		/// </summary>
		/// <returns></returns>
		public T SingleOrDefault()
		{
			return FirstOrDefault();
		}

		/// <summary>
		/// Gets a single object with the query
		/// </summary>
		/// <returns></returns>
		/// <remarks>Throws a NDOException, if the query fetches an empty result set.</remarks>
		public T First()
		{
			return Ndoquery.ExecuteSingle( true );
		}

		/// <summary>
		/// Gets a single object with the query
		/// </summary>
		/// <returns></returns>
		/// <remarks>Throws a NDOException, if the query fetches an empty result set.</remarks>
		public T Single()
		{
			return Ndoquery.ExecuteSingle( true );
		}

		/// <summary>
		/// Gets an Enumerable for iterating over a VirtualTable with foreach
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return ResultTable.GetEnumerator();
		}

		/// <summary>
		/// Gets an untyped Enumerable for iterating over a VirtualTable with foreach
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ResultTable.GetEnumerator();
		}
	}
}