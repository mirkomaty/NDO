using System;
using System.Linq;
using System.Collections.Generic;
using NDO;
using BaseClasses;

namespace NdoClasses
{
	/// <summary>
	/// Summary for PersistentClass
	/// </summary>
	[NDOPersistent]
	public partial class PersistentClass : PersistentBaseClass
	{
		string info2;
		public string Info2
		{
			get => this.info2; 
			set => this.info2 = value;
		}
		public PersistentClass()
		{
		}
	}
}
