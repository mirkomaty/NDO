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
using System.Text.RegularExpressions;
using NDO;
using System.Linq.Expressions;
using BusinessClasses;
using NDO.Linq;


namespace BusinessClasses.Linq
{
    public class EmployeeLinqHelper : LinqHelperBase
    {
        public EmployeeLinqHelper() {}
        public EmployeeLinqHelper(object o) {}
        public int geburtsJahr;
        public string firstName;
        public string lastName;
        public TravelLinqHelper travels;
        public AddressLinqHelper address;
        public override Type ResultType { get {return typeof(Employee); } }
    }

    public class TravelLinqHelper : LinqHelperBase
    {
        public string purpose;
        public override Type ResultType { get {return typeof(Travel); } }
    }

    public class AddressLinqHelper : LinqHelperBase
    {
        public string countryCode;
        public string zip;
        public string street;
        public string city;
        public override Type ResultType { get {return typeof(Address); } }
    }


    public class EmployeeTable : Table<EmployeeLinqHelper>
    {
        public EmployeeTable(PersistenceManager pm) : base(pm)
        {
        }
        public static implicit operator List<Employee>(EmployeeTable table)
        {
            return (List<Employee>) table.ResultTable;
        }
        public new EmployeeTable Where(Expression<Func<EmployeeLinqHelper, bool>> expr)
        {
            return (EmployeeTable) base.Where(expr);
        }
        public new EmployeeTable OrderBy<K>(Expression<Func<EmployeeLinqHelper,K>> keySelector)
        {
            return (EmployeeTable)base.OrderBy(keySelector);
        }
        public new EmployeeTable OrderByDescending<K>(Expression<Func<EmployeeLinqHelper,K>> keySelector)
        {
            return (EmployeeTable)base.OrderByDescending(keySelector);
        }
        public new EmployeeTable ThenBy<K>(Expression<Func<EmployeeLinqHelper,K>> keySelector)
        {
            return (EmployeeTable)base.OrderBy(keySelector);
        }
        public new EmployeeTable ThenByDescending<K>(Expression<Func<EmployeeLinqHelper,K>> keySelector)
        {
            return (EmployeeTable)base.OrderByDescending(keySelector);
        }
        public new EmployeeTable Select<S>(Expression<Func<EmployeeLinqHelper, S>> selector) where S : LinqHelperBase
        {
            return (EmployeeTable)base.Select<S>(selector);
        }

    }

    public class FetchGroup : LinqHelperBase
    {
        public FetchGroup(params object[] fields){}
        public override Type ResultType
        {
            get { return null; }
        }
    }

    public class TravelTable : Table<TravelLinqHelper>
    {
        public TravelTable(PersistenceManager pm) : base(pm)
        {
        }
        public static implicit operator List<Travel>(TravelTable table)
        {
            return (List<Travel>) table.ResultTable;
        }
    }

    public class AddressTable : Table<AddressLinqHelper>
    {
        public AddressTable(PersistenceManager pm) : base(pm)
        {
        }
        public static implicit operator List<Address>(AddressTable table)
        {
            return (List<Address>) table.ResultTable;
        }
    }

    public class BusinessClassesDataContext : DataContext
    {
        public BusinessClassesDataContext() : base()
        {
            ConstructTables();
        }
        public BusinessClassesDataContext(string mappingFile) : base(mappingFile)
        {
            ConstructTables();
        }
        void ConstructTables()
        {
            EmployeeTable = new EmployeeTable(this.pm);
            TravelTable  = new TravelTable(this.pm);
            AddressTable  = new AddressTable(this.pm);
        }
        public EmployeeTable EmployeeTable;
        public TravelTable TravelTable;
        public AddressTable AddressTable;
    }

}
