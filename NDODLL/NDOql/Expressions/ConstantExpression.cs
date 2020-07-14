using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDOql.Expressions
{
	/// <summary>
	/// Represents a constant
	/// </summary>
	public class ConstantExpression : OqlExpression
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="line"></param>
		/// <param name="col"></param>
		public ConstantExpression( int line, int col ) : base( line, col )
		{
		}
	}
}
