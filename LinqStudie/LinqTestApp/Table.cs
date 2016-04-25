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
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using NDO;
using System.Expressions;
using BusinessClasses;
using System.Query;

namespace NDO.Linq
{
    //public delegate R Func<T1,R>(T1 arg1);
    //public delegate R Func<T1,T2,R>(T1 arg1, T2 arg2);

    public abstract class LinqHelperBase
    {
        public abstract Type ResultType {get;}
    }
    public class GenericLinqHelperBase<T> : LinqHelperBase
    {
        object element;
        public GenericLinqHelperBase(object element)
        {
            this.element = element;
        }
        public override Type ResultType
        {
            get { return typeof(T); }
        }
    }
    public class Table<T> where T : LinqHelperBase
    {
        PersistenceManager pm;
        ArrayList prefetches = new ArrayList();
        Query ndoquery = null;
        ArrayList parameters = new ArrayList();
        Type resultType;
        bool useLikeOperator;


        public Table(PersistenceManager pm)
        {
            this.pm = pm;
        }

        public Table<T> Select<S>(Expression<Func<T, S>> selector) where S : LinqHelperBase
        {
            return this;
        }

        public Table<T> OrderBy<K>(Expression<Func<T,K>> keySelector)
        {
            ExpressionTreeTransformer transformer = 
                new ExpressionTreeTransformer((LambdaExpression)keySelector);
            string field = transformer.Transform();
            ndoquery.Orderings.Add(new Query.AscendingOrder(field));
            return this;
        }

        public Table<T> OrderByDescending<K>(Expression<Func<T,K>> keySelector)
        {
            ExpressionTreeTransformer transformer = 
                new ExpressionTreeTransformer((LambdaExpression)keySelector);
            string field = transformer.Transform();
            ndoquery.Orderings.Add(new Query.DescendingOrder(field));
            return this;
        }

        public Table<T> ThenBy<K>(Expression<Func<T,K>> keySelector)
        {
            return OrderBy<K>(keySelector);
        }

        public Table<T> ThenByDescending<K>(Expression<Func<T,K>> keySelector)
        {
            return OrderByDescending<K>(keySelector);
        }


        public Table<T>Where(Expression<Func<T,bool>>expr)
        {
            // Determine the result type of the query.
            // Construct a dummy instance of T to ask it.
            T instance = (T) Activator.CreateInstance(typeof(T));
            resultType = instance.ResultType;
            // Transform the expression to NDOql
            ExpressionTreeTransformer transformer = new ExpressionTreeTransformer((LambdaExpression)expr);
            transformer.UseLikeOperator = this.useLikeOperator;
            string query = transformer.Transform();

            // Now, use the polymorphic version of the NDO query
            ndoquery = pm.NewQuery(resultType, query, false);
            // Add the parameters collected by the transformer
            foreach(object o in transformer.Parameters)
                ndoquery.Parameters.Add(o);
            // Add the prefetch definitions
            ndoquery.Prefetches = this.prefetches;
            //// Return a new instance of table with the result
            //Table<T> newTable = (Table<T>) Activator.CreateInstance(this.GetType(), new object[]{pm});
            //newTable.resultTable = resultList;
            return this;
        }

        public IList ResultTable
        {
            get 
            { 
                // Construct a generic result list using  the result type
                Type queryType = typeof(List<>);
                IList resultList = (IList) Activator.CreateInstance(queryType.MakeGenericType(resultType));
                // List<T> implements IList, which makes it possible to fill the list
                // in a polymorphic way.

                // Now execute the query
                IList l = ndoquery.Execute();
                // We'll change that in future...
                foreach(object o in l)
                    resultList.Add(o);
                return resultList; 
            }
        }

        public bool UseLikeOperator
        {
            get { return useLikeOperator; }
            set { useLikeOperator = value; }
        }

        public ArrayList Prefetches
        {
            get { return prefetches; }
        }
    }

}