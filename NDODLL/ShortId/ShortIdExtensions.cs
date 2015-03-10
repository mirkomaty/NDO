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
		public static Type GetObjectType(this string shortId)
		{
			string[] arr = shortId.Decode().Split('~');
			return Type.GetType( arr[0] + "," + arr[1] );
		}

		/// <summary>
		/// Gets the ShortId of an object
		/// </summary>
		/// <param name="pc"></param>
		/// <returns></returns>
		public static string ShortId(this IPersistentObject pc)
		{
			if (pc.NDOObjectState == NDOObjectState.Transient)
				throw new Exception("Object is transient.");
			ObjectId oid = pc.NDOObjectId;
			if (oid == null)
				throw new NullReferenceException( "NDOObjectId is null." );
			if (!oid.IsValid())
				throw new NDOException(86, "ObjectId is not valid.");
			if (oid.Id.Values.Length > 1)
				throw new Exception( "Can't construct a ShortId because the object has multiple key values." );
			Type oidType = oid.Id.Value.GetType();
			if (oidType != typeof(int) && oidType != typeof(Guid) && oidType != typeof(string))
			{
				throw new Exception( "The oid type of the object does not allow the indication by a ShortId: " + oidType.FullName );
			}
			Type t = pc.GetType();
			return (t.FullName + "~" + new AssemblyName( t.Assembly.FullName ).Name + "~" + pc.NDOObjectId.Id[0]).Encode();
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
