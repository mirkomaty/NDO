using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class Shipper
	{
		public Shipper()
		{
		}
		string companyName;
		string phone;

		[NDORelation]
		List<Order> orders = new List<Order>();


#region Field accessors
		public string CompanyName
		{
			get { return companyName; }
			set { companyName = value; }
		}
		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}

        public List<Order> Orders
        {
            get { return new NDOReadOnlyGenericList<Order>(orders); }
            set { orders = (List<Order>)value; }
        }
        public void AddOrder(Order o)
        {
            orders.Add(o);
        }
        public void RemoveOrder(Order o)
        {
            if (orders.Contains(o))
                orders.Remove(o);
        }

#endregion Field accessors

	}
}
