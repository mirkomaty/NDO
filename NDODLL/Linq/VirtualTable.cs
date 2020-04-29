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
		int skip = -1;
		int take = -1;
		List<QueryOrder> orderings = new List<QueryOrder>();
		string queryString = String.Empty;
		List<object> queryParameters = new List<object>();

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
            this.orderings.Add(new Query.AscendingOrder(field));
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
            this.orderings.Add(new Query.DescendingOrder(field));
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
			// Transform the expression to NDOql
            ExpressionTreeTransformer transformer = new ExpressionTreeTransformer((LambdaExpression)expr);
            this.queryString = transformer.Transform();

			this.queryParameters.Clear();
			// Add the parameters collected by the transformer
            foreach(object o in transformer.Parameters)
                this.queryParameters.Add(o);

            return this;
        }

		/// <summary>
		/// Allows to reuse a VirtualTable with different parameters.
		/// Parameters must have the same order, as they appear in the Linq query.
		/// </summary>
		/// <param name="newParameters"></param>
		public void ReplaceParameters(IEnumerable<object> newParameters)
		{
			this.queryParameters.Clear();
			foreach (var p in queryParameters)
			{
				this.queryParameters.Add( p );
			}
		}

		/// <summary>
		/// Executes the COUNT aggregate query for the given virtual table.
		/// </summary>
		public int Count
		{
			get
			{
				var ndoQuery = Ndoquery;
				return (int)(decimal)ndoQuery.ExecuteAggregate( "*", AggregateType.Count );
			}
		}

		/// <summary>
		/// Executes the MAX aggregate query for the given virtual table.
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="fieldSelector"></param>
		/// <returns></returns>
		public TP Max<TP>( Expression<Func<T, TP>> fieldSelector )
		{
			return ExecuteAggregate(fieldSelector, AggregateType.Max );
		}

		/// <summary>
		/// Executes the MAX aggregate query for the given virtual table.
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="fieldSelector"></param>
		/// <returns></returns>
		public TP Min<TP>( Expression<Func<T, TP>> fieldSelector )
		{
			return ExecuteAggregate( fieldSelector, AggregateType.Min );
		}

		/// <summary>
		/// Executes the MAX aggregate query for the given virtual table.
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="fieldSelector"></param>
		/// <returns></returns>
		public TP StandardDeviation<TP>( Expression<Func<T, TP>> fieldSelector )
		{
			return ExecuteAggregate( fieldSelector, AggregateType.StDev );
		}

		/// <summary>
		/// Executes the MAX aggregate query for the given virtual table.
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="fieldSelector"></param>
		/// <returns></returns>
		public TP Average<TP>( Expression<Func<T, TP>> fieldSelector )
		{
			return ExecuteAggregate( fieldSelector, AggregateType.Avg );
		}

		/// <summary>
		/// Executes the SUM aggregate query for the given virtual table.
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="fieldSelector"></param>
		/// <returns></returns>
		public TP Sum<TP>( Expression<Func<T, TP>> fieldSelector )
		{
			return ExecuteAggregate( fieldSelector, AggregateType.Sum );
		}

		/// <summary>
		/// Executes the VAR aggregate query for the given virtual table.
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="fieldSelector"></param>
		/// <returns></returns>
		public TP Variance<TP>( Expression<Func<T, TP>> fieldSelector )
		{
			return ExecuteAggregate( fieldSelector, AggregateType.Var );
		}

		string GetField<TP>( Expression<Func<T, TP>> fieldSelector )
		{
			ExpressionTreeTransformer transformer =
				new ExpressionTreeTransformer( fieldSelector );
			return transformer.Transform();
		}

		/// <summary>
		/// /// Executes the aggregate query for the given virtual table.
		/// </summary>
		/// <typeparam name="TP"></typeparam>
		/// <param name="fieldSelector"></param>
		/// <param name="aggregateType"></param>
		/// <returns></returns>
		TP ExecuteAggregate<TP>( Expression<Func<T, TP>> fieldSelector, AggregateType aggregateType )
		{
			return (TP)Ndoquery.ExecuteAggregate( GetField(fieldSelector), aggregateType );
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
		/// Add an expression which represents a relation. The query will try to fetch these related objects together with the main resultset.
		/// </summary>
		/// <typeparam name="TP">The type of the property - will be automatically determined by the compiler.</typeparam>
		/// <param name="expr"></param>
		public void AddPrefetch<TP>( Expression<Func<T, TP>> expr )
		{
			ExpressionTreeTransformer transformer =
				new ExpressionTreeTransformer( (LambdaExpression)expr );
			string field = transformer.Transform();
			this.prefetches.Add( field );
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

		NDOQuery<T> CreateNDOQuery()
		{
			var ndoquery = new NDOQuery<T>( pm, this.queryString );
			if (this.take > -1)
				ndoquery.Take = take;
			if (this.skip > -1)
				ndoquery.Skip = skip;
			ndoquery.Prefetches = this.prefetches;
			foreach (var p in this.queryParameters)
			{
				ndoquery.Parameters.Add( p );
			}
			ndoquery.Orderings = this.orderings;
			return ndoquery;
		}

		private NDOQuery<T> Ndoquery
		{
			get
			{
				return CreateNDOQuery();
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
		/// Supports Take and Skip
		/// </summary>
		/// <param name="take"></param>
		/// <returns></returns>
		public VirtualTable<T> Take( int take )
		{
			this.take = take;
			return this;
		}

		/// <summary>
		/// Supports Take and Skip
		/// </summary>
		/// <param name="skip"></param>
		/// <returns></returns>
		public VirtualTable<T> Skip( int skip )
		{
			this.skip = skip;
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