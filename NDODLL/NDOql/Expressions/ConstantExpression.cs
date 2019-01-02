using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDOql.Expressions
{
	public class ConstantExpression : OqlExpression
	{
		public ConstantExpression( int line, int col ) : base( line, col )
		{
		}
	}
}
