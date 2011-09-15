using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class OrderDetail
	{
		public OrderDetail()
		{
		}
		decimal unitPrice;
		short quantity;
		float discount;

		[NDORelation]
		Order order;

		[NDORelation]
		Product product;

        public string ProductName
        {
            get { return this.product.ProductName; }
        }

        public decimal Price
        {
            get { return (this.unitPrice - this.unitPrice * (decimal) this.discount) * this.quantity; }
        }

#region Field accessors
		public decimal UnitPrice
		{
			get { return unitPrice; }
			set { unitPrice = value; }
		}
		public short Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}
		public float Discount
		{
			get { return discount; }
			set { discount = value; }
		}

		public Order Order
		{
			get { return order; }
			set { order = value; }
		}
		public Product Product
		{
			get { return product; }
			set { product = value; }
		}
#endregion Field accessors

	}
}
