using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NDO
{
	/// <summary>
	/// This class is used to serialize a list of ObjectIds into a list of ShortIds
	/// </summary>
    public class ObjectIdList : List<ObjectId>,ISerializable
    {
		/// <inheritdoc />
		public ObjectIdList() : base()
		{
		}

		/// <inheritdoc />
		public ObjectIdList(IEnumerable<ObjectId> collection) : base(collection)
		{
		}

		/// <summary>
		/// Serializes an ObjectIdList
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "ids", (from oid in this select oid.ToString()).ToList() );
		}
	}
}
