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
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NDO;
using System.Expressions;
using BusinessClasses;


namespace BusinessClasses.Linq
{
    public delegate R Func<T1,R>(T1 arg1);
    public delegate R Func<T1,T2,R>(T1 arg1, T2 arg2);

    public abstract class LinqHelperBase
    {
        object theObject;
        public object TheObject
        {
            get { return theObject; }
            set { theObject = value; }
        }
        public abstract Type ResultType {get;}
    }
    

    public class EmployeeLinqHelper : LinqHelperBase
    {
        public string firstName {get{return null;}}
        public string lastName {get{return null;}}
        public Table<TravelLinqHelper>travels;
        public override Type ResultType { get {return typeof(Employee); } }
    }

    public class TravelLinqHelper : LinqHelperBase
    {
        public string purpose {get{return null;}}
        public override Type ResultType { get {return typeof(Travel); } }
    }

    public class Table<T> : List<T> where T : LinqHelperBase
    {
        PersistenceManager pm;

        public Table(PersistenceManager pm)
        {
            this.pm = pm;
        }
        public Table<T>Wherex(Expression<Func<T,bool>>expr)
        {
            StringBuilder sb = new StringBuilder();
            expr.BuildString(sb);
            Console.WriteLine(sb.ToString());
            return null;
        }
        private static void CopyItems(IList l, Table<T>table)
        {
            IList il = (IList) l;
            foreach(T item in table)
                il.Add(item.TheObject);
        }
        public static implicit operator List<Employee>(Table<T>table)
        {
            List<Employee> l = new List<Employee>(table.Count);
            CopyItems(l, table);
            return l;
        }
        public static implicit operator List<Travel>(Table<T>table)
        {
            List<Travel> l = new List<Travel>(table.Count);
            CopyItems(l, table);
            return l;
        }
    }

    public class DataContext
    {
        PersistenceManager pm;
        public DataContext()
        {
            this.pm = new PersistenceManager();
        }
        public DataContext(string s)
        {
            this.pm = new PersistenceManager(s);
        }

        public Table<T> GetTable<T>() where T : LinqHelperBase
        {
            return new Table<T>(pm);
        }
    }
}
