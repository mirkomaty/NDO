using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDODev.Linq
{
	public static class LinqStringExtension
	{
		public static bool Like(this object s, object parameter)
		{
			return true;
		}
		public static T Oid<T>(this object s)
		{
			return default(T);
		}
	}
}
