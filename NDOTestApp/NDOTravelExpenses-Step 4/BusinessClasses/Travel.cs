using System;
using System.Collections;
using NDO;
namespace BusinessClasses
{
    [NDOPersistent]
    public class Travel
    {
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
