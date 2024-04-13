using System;
using NDO;

namespace PersistentEnhancerTestClasses
{
	[NDOPersistent]
	public class Person
	{
		string firstName;
		string lastName;
		Guid id;
		public Guid Id
		{
			get => this.id; 
			set => this.id = value;
		}

		public string FirstName
		{
			get => this.firstName;
			set => this.firstName = value;
		}

		public string LastName
		{
			get => this.lastName;
			set => this.lastName = value;
		}
	}
}
