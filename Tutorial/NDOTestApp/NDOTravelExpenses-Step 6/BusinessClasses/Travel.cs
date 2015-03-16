using System;
using System.Collections;
using NDO;

namespace BusinessClasses
{
    [NDOPersistent]
    public partial class Travel
    {
        [NDORelation(typeof(Expense), RelationInfo.Composite)]
        IList expenses = new ArrayList();
        public IList Expenses
        {
            get { return ArrayList.ReadOnly(expenses); }
            set { expenses = value; }
        }
        public void RemoveExpense(Expense ex)
        {
            if (expenses.Contains(ex))
                expenses.Remove(ex);
        }
        public void AddExpense(Expense ex)
        {
            expenses.Add(ex);
        }

        [NDORelation]
        Employee employee;
        public Employee Employee
        {
            get { return employee; }
            set { employee = value; }
        }

        [NDORelation(typeof(Country))]
        IList countries = new ArrayList();

        public void AddCountry(Country l)
        {
            countries.Add(l);
        }
        public void RemoveCountry(Country l)
        {
            countries.Remove(l);
        }
        public IList Countries
        {
            get { return ArrayList.ReadOnly(countries); }
            set { countries = value; }
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
