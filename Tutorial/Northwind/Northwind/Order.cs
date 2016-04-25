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
	public class Order
	{
		public Order()
		{
		}
		DateTime orderDate;
		DateTime requiredDate;
		DateTime shippedDate;
		decimal freight;
		string shipName;
		string shipAddress;
		string shipCity;
		string shipRegion;
		string shipPostalCode;
		string shipCountry;

		[NDORelation]
		Customer customer;

		[NDORelation]
		Employee employee;

		[NDORelation]
		Shipper shipper;

		[NDORelation]
		List<OrderDetail> orderDetails = new List<OrderDetail>();

        #region Parent Object Accessors
        public string EmployeeName
        {
            get { return employee.FirstName + " " + employee.LastName; }
        }

        public string CustomerName
        {
            get { return customer.CompanyName; }
        }
        #endregion

        #region Field accessors
        public DateTime OrderDate
		{
			get { return orderDate; }
			set { orderDate = value; }
		}
		public DateTime RequiredDate
		{
			get { return requiredDate; }
			set { requiredDate = value; }
		}
		public DateTime ShippedDate
		{
			get { return shippedDate; }
			set { shippedDate = value; }
		}
		public decimal Freight
		{
			get { return freight; }
			set { freight = value; }
		}
		public string ShipName
		{
			get { return shipName; }
			set { shipName = value; }
		}
		public string ShipAddress
		{
			get { return shipAddress; }
			set { shipAddress = value; }
		}
		public string ShipCity
		{
			get { return shipCity; }
			set { shipCity = value; }
		}
		public string ShipRegion
		{
			get { return shipRegion; }
			set { shipRegion = value; }
		}
		public string ShipPostalCode
		{
			get { return shipPostalCode; }
			set { shipPostalCode = value; }
		}
		public string ShipCountry
		{
			get { return shipCountry; }
			set { shipCountry = value; }
		}

		public Customer Customer
		{
			get { return customer; }
			set { customer = value; }
		}
		public Employee Employee
		{
			get { return employee; }
			set { employee = value; }
		}
		public Shipper Shipper
		{
			get { return shipper; }
			set { shipper = value; }
		}

        public List<OrderDetail> OrderDetails
        {
            get { return new NDOReadOnlyGenericList<OrderDetail>(orderDetails); }
            set { orderDetails = (List<OrderDetail>)value; }
        }
        public void AddOrderDetail(OrderDetail o)
        {
            orderDetails.Add(o);
        }
        public void RemoveOrderDetail(OrderDetail o)
        {
            if (orderDetails.Contains(o))
                orderDetails.Remove(o);
        }


#endregion Field accessors

	}
}
