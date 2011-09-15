using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class Product
	{
		public Product()
		{
		}
		string productName;
		string quantityPerUnit;
		decimal unitPrice;
		short unitsInStock;
		short unitsOnOrder;
		short reorderLevel;
		bool discontinued;

		[NDORelation]
		Supplier supplier;

		[NDORelation]
		Category category;


#region Field accessors
		public string ProductName
		{
			get { return productName; }
			set { productName = value; }
		}
		public string QuantityPerUnit
		{
			get { return quantityPerUnit; }
			set { quantityPerUnit = value; }
		}
		public decimal UnitPrice
		{
			get { return unitPrice; }
			set { unitPrice = value; }
		}
		public short UnitsInStock
		{
			get { return unitsInStock; }
			set { unitsInStock = value; }
		}
		public short UnitsOnOrder
		{
			get { return unitsOnOrder; }
			set { unitsOnOrder = value; }
		}
		public short ReorderLevel
		{
			get { return reorderLevel; }
			set { reorderLevel = value; }
		}
		public bool Discontinued
		{
			get { return discontinued; }
			set { discontinued = value; }
		}

		public Supplier Supplier
		{
			get { return supplier; }
			set { supplier = value; }
		}
		public Category Category
		{
			get { return category; }
			set { category = value; }
		}
#endregion Field accessors

	}
}
