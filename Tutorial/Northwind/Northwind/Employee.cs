using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class Employee
	{
		public Employee()
		{
		}
		string lastName;
		string firstName;
		string title;
		string titleOfCourtesy;
		DateTime birthDate;
		DateTime hireDate;
		string address;
		string city;
		string region;
		string postalCode;
		string country;
		string homePhone;
		string extension;
		byte[] photo;
		string notes;
		int reportsTo;
		string photoPath;

		[NDORelation]
		List<Order> orders = new List<Order>();

        [NDORelation]
        IList<Territory> territories = new List<Territory>();
        
#region Field accessors
		public string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}
		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		public string TitleOfCourtesy
		{
			get { return titleOfCourtesy; }
			set { titleOfCourtesy = value; }
		}
		public DateTime BirthDate
		{
			get { return birthDate; }
			set { birthDate = value; }
		}
		public DateTime HireDate
		{
			get { return hireDate; }
			set { hireDate = value; }
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
		public string HomePhone
		{
			get { return homePhone; }
			set { homePhone = value; }
		}
		public string Extension
		{
			get { return extension; }
			set { extension = value; }
		}
		public byte[] Photo
		{
			get { return photo; }
			set { photo = value; }
		}
		public string Notes
		{
			get { return notes; }
			set { notes = value; }
		}
		public int ReportsTo
		{
			get { return reportsTo; }
			set { reportsTo = value; }
		}
		public string PhotoPath
		{
			get { return photoPath; }
			set { photoPath = value; }
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

        public IList<Territory> Territories
        {
            get { return new NDOReadOnlyGenericList<Territory>(territories); }
            set { territories = value; }
        }
        public void AddTerritory(Territory t)
        {
            territories.Add(t);
        }
        public void RemoveTerritory(Territory t)
        {
            if (territories.Contains(t))
                territories.Remove(t);
        }

#endregion Field accessors

	}
}
