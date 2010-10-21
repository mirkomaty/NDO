using System;
using System.Collections.Generic;
using System.Text;
using NDO;

namespace PureBusinessClasses
{
	[NDOPersistent]
	public class OwnHashcodeParent
	{
		[NDORelation]
		OwnHashcodeClient client;
		public OwnHashcodeClient Client
		{
			get { return client; }
			set { client = value; }
		}

		public override int GetHashCode()
		{
			throw new Exception( "Test-Exeption" );
			//return client.GetHashCode();
		}
	}

	[NDOPersistent]
	public class OwnHashcodeClient
	{
		[NDORelation]
		OwnHashcodeParent parent;
		public OwnHashcodeParent Parent
		{
			get { return parent; }
			set { parent = value; }
		}


		int dummy;
		public int Dummy
		{
			get { return dummy; }
			set { dummy = value; }
		}
	}
}
