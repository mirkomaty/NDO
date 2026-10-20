using System;
using System.Collections.Generic;
using System.Linq;
using NDO;
using NDO.Mapping.Attributes;

namespace StressTestClasses
{
	/// <summary>
	/// A sales order with three kinds of relations:
	/// a directed n:1 relation to the customer (foreign key in the order table),
	/// a bidirectional composite 1:n relation to the order lines (foreign key in the order line table)
	/// and a directed n:m relation to tags (mapping table).
	/// </summary>
	[NDOPersistent]
	public class SalesOrder
	{
		string orderNumber;
		DateTime orderDate;
		string status;
		decimal total;

		[NDORelation]
		Customer customer;

		[NDORelation( RelationInfo.Composite )]
		List<OrderLine> lines = new List<OrderLine>();

		[NDORelation, MappingTable]
		List<Tag> tags = new List<Tag>();

		public SalesOrder()
		{
		}

		public string OrderNumber
		{
			get { return orderNumber; }
			set { orderNumber = value; }
		}

		public DateTime OrderDate
		{
			get { return orderDate; }
			set { orderDate = value; }
		}

		public string Status
		{
			get { return status; }
			set { status = value; }
		}

		public decimal Total
		{
			get { return total; }
			set { total = value; }
		}

		public Customer Customer
		{
			get { return customer; }
			set { customer = value; }
		}

		public IEnumerable<OrderLine> Lines => this.lines;

		public OrderLine NewLine( string product, int quantity, decimal price )
		{
			var line = new OrderLine() { Product = product, Quantity = quantity, Price = price };
			this.lines.Add( line );
			return line;
		}

		public void RemoveLine( OrderLine line )
		{
			this.lines.Remove( line );
		}

		public IEnumerable<Tag> Tags => this.tags;

		public void AddTag( Tag tag )
		{
			this.tags.Add( tag );
		}

		public void RemoveTag( Tag tag )
		{
			this.tags.Remove( tag );
		}

		public decimal ComputeTotal()
		{
			return this.lines.Sum( l => l.Quantity * l.Price );
		}
	}
}
