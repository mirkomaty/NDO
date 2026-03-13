using System;
using NDO;

namespace PersistentEnhancerTestClasses
{
	/// <summary>
	/// Use this project to test the enhancer during development. 
	/// The project compiles for several target framework versions.
	/// Change BinPath and ObjPath .ndoproj file to test the different framework versions.
	/// </summary>
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
