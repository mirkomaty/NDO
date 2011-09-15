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
