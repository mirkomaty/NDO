using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class CustomerDemographics
	{
		public CustomerDemographics()
		{
		}
		string customerDesc;

		[NDORelation]
		IList<Customer> customers = new List<Customer>();


#region Field accessors
		public string CustomerDesc
		{
			get { return customerDesc; }
			set { customerDesc = value; }
		}

        public IList<Customer> Customers
        {
            get { return new NDOReadOnlyGenericList<Customer>(customers); }
            set { customers = value; }
        }
        public void AddCustomer(Customer c)
        {
            customers.Add(c);
        }
        public void RemoveCustomer(Customer c)
        {
            if (customers.Contains(c))
                customers.Remove(c);
        }
#endregion Field accessors

	}
}
