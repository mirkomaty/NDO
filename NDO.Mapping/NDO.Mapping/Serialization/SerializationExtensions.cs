using System;
using System.Collections.Generic;
using System.Text;

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
		/// <remarks>
		/// We use a serialization mechanism which is independent from the runtime. 
		/// System.Text.Json imports dependencies to certain runtime implementations.
		/// </remarks>
		public static string Serialize( this object attr )
		{
			if (attr.GetType().Name != "NDORelationAttribute")
				throw new ArgumentException( "Wrong parameter type", "attr" );
			Type t = attr.GetType();
			Type rt = (Type)t.GetProperty("RelationType").GetValue(attr);
			string rts = null;
			if (rt != null)
				rts = rt.FullName;
            string ri = t.GetProperty("Info").GetValue(attr).ToString();
			string rn = (string)t.GetProperty("RelationName").GetValue(attr);
            return $@"{rts??"#null"},{ri},{rn??"#null"}";
		}

        /// <summary>
        /// Converts an attribute object to the correct type
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        /// <remarks>
        /// We use a serialization mechanism which is independent from the runtime. 
        /// System.Text.Json imports dependencies to certain runtime implementations.
        /// </remarks>
        public static NDORelationAttribute ConvertToNdoRelation( this object attr )
		{
			var strAttr = Serialize(attr);
			var arr = strAttr.Split(',');
			var rt = arr[0];
			if (rt == "#null")
				rt = null;
			Type relationType = null;
			if (rt != null)
				relationType = Type.GetType( rt );
			Enum.TryParse<RelationInfo>(arr[1], out var ri);
			var rn = arr[2];
			if (rn == "#null")
				rn = null;
			return new NDORelationAttribute( relationType, ri, rn );
		}
	}
}
