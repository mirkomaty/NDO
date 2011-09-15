using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class Supplier
	{
		public Supplier()
		{
		}
		string companyName;
		string contactName;
		string contactTitle;
		string address;
		string city;
		string postalCode;
		string country;
		string phone;
		string fax;
		string homePage;

		[NDORelation]
		List<Product> products = new List<Product>();

		[NDORelation]
		Region region;


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
		public string HomePage
		{
			get { return homePage; }
			set { homePage = value; }
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
        
        public Region Region
		{
			get { return region; }
			set { region = value; }
		}
#endregion Field accessors

	}
}
