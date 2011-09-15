using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class Category
	{
		public Category()
		{
		}
		string categoryName;
		string description;
		byte[] picture;

		[NDORelation]
		List<Product> products = new List<Product>();
#region Field accessors
		public string CategoryName
		{
			get { return categoryName; }
			set { categoryName = value; }
		}
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
		public byte[] Picture
		{
			get { return picture; }
			set { picture = value; }
		}

        public List<Product> Products
        {
            get { return new NDOReadOnlyGenericList<Product>(products); }
            set { products = (List<Product>)value; }
        }
        public void AddProduct(Product p)
        {
            products.Add(p);
        }
        public void RemoveProduct(Product p)
        {
            if (products.Contains(p))
                products.Remove(p);
        }


#endregion Field accessors

	}
}
