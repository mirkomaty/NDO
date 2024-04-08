using System;
using System.Linq;
using System.Collections.Generic;
using NDO;

namespace BaseClasses
{
	/// <summary>
	/// Summary for PersistentBaseClass
	/// </summary>
	[NDOPersistent]
	public partial class PersistentBaseClass
	{
		string info1;
		public string Info1
		{
			get => this.info1; 
			set => this.info1 = value;
		}
		public PersistentBaseClass()
		{
		}
	}
}
