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
