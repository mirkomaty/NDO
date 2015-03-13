using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NDO.ShortId
{
	public static class ShortIdExtensions
	{
		/// <summary>
		/// Encodes a raw short id to be used in urls.
		/// </summary>
		/// <param name="shortId"></param>
		/// <returns></returns>
		public static string Encode(this string shortId)
		{
			string[] arr = shortId.Split('~');
			for (int i = 0; i < arr.Length; i++)
				arr[i] = HttpUtility.UrlEncode( arr[i] );
			return string.Join( "~", arr );
		}

		/// <summary>
		/// Decodes a ShortId to a raw short id with umlauts.
		/// </summary>
		/// <param name="shortId"></param>
		/// <returns></returns>
		public static string Decode(this string shortId)
		{
			string[] arr = shortId.Split('~');
			for (int i = 0; i < arr.Length; i++)
				arr[i] = HttpUtility.UrlDecode( arr[i] );
			return string.Join( "~", arr );
		}

		/// <summary>
		/// Gets the type of the object denoted by the ShortId
		/// </summary>
		/// <param name="shortId"></param>
		/// <returns></returns>
		public static Type GetObjectType(this string shortId, PersistenceManager pm)
		{
			string[] arr = shortId.Decode().Split('~');
			int typeCode = 0;
			if (int.TryParse(arr[1],System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out typeCode))
			{
				return pm.TypeManager[typeCode];
			}
			else
			{
				return Type.GetType( arr[0] + "," + arr[1] );  // Try the readable format
			}			
		}

		/// <summary>
		/// Gets the entity name of the ShortId
		/// </summary>
		/// <param name="shortId"></param>
		/// <returns></returns>
		public static string GetEntityName(this string shortId)
		{
			string[] arr = shortId.Decode().Split('~');
			int typeCode = 0;
			if (int.TryParse(arr[1],System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out typeCode))
			{
				return arr[0];
			}
			else
			{
				Type t = Type.GetType( arr[0] + "," + arr[1] );  // Try the readable format
				if (t != null)
					return t.Name;
			}
			return null;
		}

		/// <summary>
		/// Gets the ShortId of an object
		/// </summary>
		/// <param name="pc"></param>
		/// <returns></returns>
		public static string ShortId(this IPersistentObject pc)
		{
			return pc.NDOObjectId.ToShortId();
		}

		/// <summary>
		/// Determines, if a given string has the right format to be a shortId
		/// </summary>
		/// <param name="shortId"></param>
		/// <returns></returns>
		public static bool IsShortId(this string shortId)
		{
			string[] arr = shortId.Split('~');
			return arr.Length == 3;
		}
	}
}
