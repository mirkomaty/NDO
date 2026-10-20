using System;
using NDO;

namespace StressTestClasses
{
	/// <summary>
	/// An order line. Part of the composite relation SalesOrder.lines.
	/// </summary>
	[NDOPersistent]
	public class OrderLine
	{
		string product;
		int quantity;
		decimal price;

		// Set by NDO as the counterpart of SalesOrder.lines
#pragma warning disable CS0649
		[NDORelation]
		SalesOrder order;
#pragma warning restore CS0649

		public OrderLine()
		{
		}

		public string Product
		{
			get { return product; }
			set { product = value; }
		}

		public int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		public decimal Price
		{
			get { return price; }
			set { price = value; }
		}

		public SalesOrder Order
		{
			get { return order; }
		}
	}
}
