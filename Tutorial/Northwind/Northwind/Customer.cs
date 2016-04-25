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
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public partial class Customer
	{
		public Customer()
		{
		}
        [NDOObjectId]
        string customerId;
		string companyName;
		string contactName;
		string contactTitle;
		string address;
		string city;
		string region;
		string postalCode;
		string country;
		string phone;
		string fax;

		[NDORelation(typeof(Order), RelationInfo.Composite)]
		List<Order> orders = new List<Order>();

		[NDORelation]
		List<CustomerDemographics> customerDemographics = new List<CustomerDemographics>();

#region Field accessors
		public string CompanyName
		{
			get { return companyName; }
			set { companyName = value; }
		}
		public string ContactName
		{
			get { return contactName; }
			set { contactName = value; }
		}
		public string ContactTitle
		{
			get { return contactTitle; }
			set { contactTitle = value; }
		}
		public string Address
		{
			get { return address; }
			set { address = value; }
		}
		public string City
		{
			get { return city; }
			set { city = value; }
		}
		public string Region
		{
			get { return region; }
			set { region = value; }
		}
		public string PostalCode
		{
			get { return postalCode; }
			set { postalCode = value; }
		}
		public string Country
		{
			get { return country; }
			set { country = value; }
		}
		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}
		public string Fax
		{
			get { return fax; }
			set { fax = value; }
		}
        public string CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public List<CustomerDemographics> CustomerDemographics
        {
            get { return new NDOReadOnlyGenericList<CustomerDemographics>(customerDemographics); }
            set { customerDemographics = (List<CustomerDemographics>)value; }
        }
        public void AddCustomerDemographics(CustomerDemographics c)
        {
            customerDemographics.Add(c);
        }
        public void RemoveCustomerDemographics(CustomerDemographics c)
        {
            if (customerDemographics.Contains(c))
                customerDemographics.Remove(c);
        }

        public List<Order> Orders
        {
            get { return new NDOReadOnlyGenericList<Order>(orders); }
            set { orders = (List<Order>)value; }
        }
        public Order NewOrder()
        {
            Order o = new Order();
            orders.Add(o);
            return o;
        }
        public void RemoveOrder(Order o)
        {
            if (orders.Contains(o))
                orders.Remove(o);
        }
#endregion Field accessors

	}
}
