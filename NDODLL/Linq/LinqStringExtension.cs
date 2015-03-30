using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDO.Linq
{
	public static class LinqStringExtension
	{
		/// <summary>
		/// Helper Method for creating the LIKE statement
		/// </summary>
		/// <param name="s"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static bool Like(this object s, object parameter)
		{
			return true;
		}

		/// <summary>
		/// Helper Method for creating BETWEEN statements
		/// </summary>
		/// <param name="s"></param>
		/// <param name="firstParameter"></param>
		/// <param name="secondParameter"></param>
		/// <returns></returns>
		public static bool Between(this object s, object firstParameter, object secondParameter)
		{
			return true;
		}

		/// <summary>
		/// Helper Method for Linq queries. Use this method only, if you can't implement the IPersistentObject interface. 
		/// If possible use a linq query like that:
		/// <code>
		/// Customer c1 = (Customer)pm.FindObject( typeof( Customer ), "ALFKI" );  // get an object with a valid object id
		/// var vt3 = from c in vt where c.NDOObjectId == c1.NDOObjectId select c;
		/// </code>
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static ObjectId Oid(this object o)
		{
			// If Oid() is used at the right side of a comparison
			// we try to return the NDOObjectId.
			IPersistentObject po = o as IPersistentObject;
			if (po == null)
				return default(ObjectId);
			return po.NDOObjectId;
		}
	}
}
