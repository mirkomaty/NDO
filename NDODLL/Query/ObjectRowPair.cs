using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	class ObjectRowPair<T> : IComparable
	{
		public ObjectRowPair( T obj, DataRow row )
		{
			Obj = obj;
			Row = row;
		}
		public T Obj { get; set; }
		public DataRow Row { get; set; }

		#region IComparable Member

		public int CompareTo( object obj )
		{
			if (!(obj is ObjectRowPair<T>))
				throw new InternalException( 96, "ObjectRowPair.cs" );
			DataRow row1 = ((ObjectRowPair<T>)obj).Row;
			foreach (DataColumn dc in row1.Table.Columns)
			{
				object own = Row[dc.ColumnName];
				object other = row1[dc];
				if (own == DBNull.Value && other == DBNull.Value)
					continue; // equal
				if (other == DBNull.Value)
					return (int)dc.AutoIncrementStep;  // null is always < other values
				if (own == DBNull.Value)
					return -((int)dc.AutoIncrementStep); // null is always < other values
				int cmp = ((IComparable)own).CompareTo( other );
				if (cmp != 0)
				{
					// In AutoIncrementStep is the Ordering.
					// 1=> Asc, -1 => Desc
					return cmp * (int)dc.AutoIncrementStep;
				}
			}
			return 0;  // Equal
		}

		#endregion
	}
}
