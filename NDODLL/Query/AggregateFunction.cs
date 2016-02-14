using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	class AggregateFunction
	{
		AggregateType type;
		public AggregateFunction( AggregateType type )
		{
			this.type = type;
		}

		public override string ToString()
		{
			switch (type)
			{
				case AggregateType.Avg:
					return "Avg";
				case AggregateType.Min:
					return "Min";
				case AggregateType.Max:
					return "Max";
				case AggregateType.Count:
					return "Count";
				case AggregateType.StDev:
					return "StDev";
				case AggregateType.Var:
					return "Var";
				case AggregateType.Sum:
					return "Sum";
				default:
					return string.Empty;
			}
		}

		public object ComputeResult( object[] parts )
		{
			if (parts.Length == 0)
				return 0m;
			if (parts.Length > 1 && (type == AggregateType.StDev || type == AggregateType.Var))
				throw new NDOException( 91, "Can't compute StDev or Var over more than one table." );
			if (type == AggregateType.StDev || type == AggregateType.Var)
				return parts[0];
			switch (type)
			{
				case AggregateType.Avg:
					if (parts.Length > 1)
						throw new NotImplementedException( "Average over more than one table is not yet implemented" );
					return parts[0];
				case AggregateType.Min:
					IComparable minVal = parts[0] as IComparable;
					for (int i = 1; i < parts.Length; i++)
						if (((IComparable)parts[i]).CompareTo( minVal ) < 0)
							minVal = (IComparable)parts[i];
					return minVal;
				case AggregateType.Max:
					IComparable maxVal = parts[0] as IComparable;
					for (int i = 1; i < parts.Length; i++)
						if (((IComparable)parts[i]).CompareTo( maxVal ) > 0)
							maxVal = (IComparable)parts[i];
					return maxVal;
				case AggregateType.Count:
				case AggregateType.Sum:
					decimal sum = 0m;
					for (int i = 0; i < parts.Length; i++)
					{
						if (parts[i] == null)
							continue;
						IConvertible ic = (IConvertible)parts[i];
						sum += ic.ToDecimal( null );
					}
					return sum;
				default:
					return 0m;
			}
		}
	}
}
