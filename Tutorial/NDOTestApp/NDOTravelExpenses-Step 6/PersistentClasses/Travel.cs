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
using NDO;

namespace BusinessClasses
{
    [NDOPersistent]
    public partial class Travel
    {
        [NDORelation(RelationInfo.Composite)]
        List<Expense> expenses = new List<Expense>();
        public IEnumerable<Expense> Expenses
        {
        	get { return this.expenses; }
        	set { this.expenses = value.ToList(); }
        }
        public void AddExpense(Expense ex)
        {
            expenses.Add(ex);
        }
        public void RemoveExpense(Expense e)
        {
        	if (this.expenses.Contains(e))
        		this.expenses.Remove(e);
        }


        [NDORelation]
        Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set { employee = value; }
        }

        [NDORelation]
        List<Country> countries = new List<Country>();
        public IEnumerable<Country> Countries
        {
        	get { return this.countries; }
        	set { this.countries = value.ToList(); }
        }
        public void AddCountry(Country c)
        {
        	this.countries.Add(c);
        }
        public void RemoveCountry(Country c)
        {
        	if (this.countries.Contains(c))
        		this.countries.Remove(c);
        }

        string purpose;
        public string Purpose
        {
            get { return purpose; }
            set { purpose = value; }
        }

        public Travel()
        {
        }
    }
}
