using System;
using NDO;

namespace StressTestClasses
{
	/// <summary>
	/// A tag. The tags are created once before the workers start and are shared by all workers,
	/// so that the mapping table between SalesOrder and Tag gets concurrent inserts and deletes for the same tags.
	/// </summary>
	[NDOPersistent]
	public class Tag
	{
		string name;

		public Tag()
		{
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
