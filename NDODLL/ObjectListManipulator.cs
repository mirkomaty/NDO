using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace NDO
{
	class ObjectListManipulator
	{
		public static void Remove(IList l, object o)
		{
			int index = -1;
			for(int i = 0; i < l.Count; i++)
			{
				if (Object.ReferenceEquals( l[i], o))
				{
					index = i;
					break;
				}
			}

			if (index > -1)
				l.RemoveAt(index);
		}

		public static bool Contains( IList l, object o )
		{
			for ( int i = 0; i < l.Count; i++ )
			{
				if ( Object.ReferenceEquals( l[i], o ) )
				{
					return true;
				}
			}

			return false;
		}
	}
}
