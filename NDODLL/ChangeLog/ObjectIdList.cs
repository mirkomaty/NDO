using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.ChangeLogging
{
	class ObjectIdList : List<ObjectId>
	{
		public ObjectIdList() : base()
		{
		}

		public ObjectIdList(IEnumerable<ObjectId> objects) : base(objects)
		{
		}

		public ObjectIdList Clone()
		{
			return new ObjectIdList( this );
		}

		public IEnumerable<string> ToStringList()
		{
			List<string> result = new List<string>();
			foreach (var item in this)
			{
				result.Add( item.ToShortId() );
			}

			return result;
		}
	}
}
