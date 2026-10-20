using System;
using NDO;

namespace StressTestClasses
{
	/// <summary>
	/// A customer. Each worker of the stress test creates its own customers.
	/// </summary>
	[NDOPersistent]
	public class Customer
	{
		string name;
		string city;
		DateTime created;
		int updateCount;

		public Customer()
		{
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string City
		{
			get { return city; }
			set { city = value; }
		}

		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		public int UpdateCount
		{
			get { return updateCount; }
			set { updateCount = value; }
		}
	}
}
