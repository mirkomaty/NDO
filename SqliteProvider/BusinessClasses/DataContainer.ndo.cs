using NDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessClasses
{
	public partial class DataContainer : IPersistentObject
	{
		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}
	}
}
