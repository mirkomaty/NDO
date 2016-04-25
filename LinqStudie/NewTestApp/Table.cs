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
using System.Linq.Expressions;
using BusinessClasses;

namespace NDODev.Linq
{
    public class VirtualTable<T>
    {
        PersistenceManager pm;
        ArrayList prefetches = new ArrayList();
        NDOQuery<T> ndoquery = null;
        ArrayList parameters = new ArrayList();

        public VirtualTable(PersistenceManager pm)
        {
            this.pm = pm;
        }

        public IEnumerable<S> Select<S>(Func<T, S> selector)
        {
			return this.ResultTable.Select( selector );
        }

        public VirtualTable<T> OrderBy<K>(Expression<Func<T,K>> keySelector)
        {
            ExpressionTreeTransformer transformer = 
                new ExpressionTreeTransformer((LambdaExpression)keySelector);
            string field = transformer.Transform();
            ndoquery.Orderings.Add(new Query.AscendingOrder(field));
            return this;
        }

        public VirtualTable<T> OrderByDescending<K>(Expression<Func<T,K>> keySelector)
        {
            ExpressionTreeTransformer transformer = 
                new ExpressionTreeTransformer((LambdaExpression)keySelector);
            string field = transformer.Transform();
            ndoquery.Orderings.Add(new Query.DescendingOrder(field));
            return this;
        }

        public VirtualTable<T> ThenBy<K>(Expression<Func<T,K>> keySelector)
        {
            return OrderBy<K>(keySelector);
        }

        public VirtualTable<T> ThenByDescending<K>(Expression<Func<T,K>> keySelector)
        {
            return OrderByDescending<K>(keySelector);
        }


        public VirtualTable<T>Where(Expression<Func<T,bool>>expr)
        {
            // Determine the result type of the query.
            // Construct a dummy instance of T to ask it.
            T instance = (T) Activator.CreateInstance(typeof(T));
            // Transform the expression to NDOql
            ExpressionTreeTransformer transformer = new ExpressionTreeTransformer((LambdaExpression)expr);
            string query = transformer.Transform();

            // Now, use the NDO query
            ndoquery = new NDOQuery<T>( pm, query);
            // Add the parameters collected by the transformer
            foreach(object o in transformer.Parameters)
                ndoquery.Parameters.Add(o);
            // Add the prefetch definitions
            ndoquery.Prefetches = this.prefetches;

            return this;
        }

        public List<T> ResultTable
        {
            get 
            {
				Console.WriteLine(ndoquery.GeneratedQuery);
                return ndoquery.Execute();
            }
        }

        public ArrayList Prefetches
        {
            get { return prefetches; }
        }

        public static implicit operator List<T>(VirtualTable<T> table)
        {
            return table.ResultTable;
        }

    }

}