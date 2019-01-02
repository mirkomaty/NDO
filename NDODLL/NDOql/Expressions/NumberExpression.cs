using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    public class NumberExpression : ConstantExpression
    {
        public NumberExpression(object value, int line, int col) : base (line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.Number;
        }

		public override OqlExpression DeepClone
		{
			get
			{
				return new NumberExpression(Value, Line, Column);
			}
		}
    }
}
