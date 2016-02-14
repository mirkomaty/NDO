using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    public class ParameterExpression : OqlExpression
    {
        public ParameterExpression(string value, int line, int col) : base(line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.Unknown;
        }

		public int Ordinal
		{
			get
			{
				string valStr = (string)Value;
				valStr = valStr.Substring( 1, valStr.Length - 2 );
				return int.Parse( valStr );
			}
			set
			{
				Value = "{" + value.ToString() + '}';
			}
		}

		public override OqlExpression DeepClone
		{
			get
			{
				return new ParameterExpression( (string)Value, Line, Column );
			}
		}
    }
}
