using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NDO.Mapping.Serialization
{
	/// <summary>
	/// Extension class to convert attribute instances to the correct type.
	/// </summary>
	public static class SerializationExtensions
	{
		/// <summary>
		/// Serializes an object which should be an NDORelationAttribute
		/// </summary>
		/// <param name="attr"></param>
		/// <returns></returns>
		public static string Serialize( this object attr )
		{
			if (attr.GetType().Name != "NDORelationAttribute")
				throw new ArgumentException( "Wrong parameter type", "attr" );

			var d = (dynamic)attr;
			string rt = null;
			if (d.RelationType != null)
				rt = d.RelationType.FullName;
			return JsonSerializer.Serialize( new { FullName = rt, d.Info, d.RelationName } );
		}

		/// <summary>
		/// Converts an attribute object to the correct type
		/// </summary>
		/// <param name="attr"></param>
		/// <returns></returns>
		public static NDORelationAttribute ConvertToNdoRelation( this object attr )
		{
			var json = Serialize(attr);
			var obj = JsonSerializer.Deserialize<JsonObject>(json);
			var rt = (string)obj["FullName"];
			Type relationType = null;
			if (rt != null)
				relationType = Type.GetType( rt );
			return new NDORelationAttribute( relationType, (RelationInfo) (int) obj["Info"], (string) obj["RelationName"] );
		}
	}
}
