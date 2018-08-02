using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    public class IndexExpression : OqlExpression
    {
        public IndexExpression(object value, int line, int col) : base (line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.Unknown;
            HasBrackets = true;
        }

		public override OqlExpression DeepClone
		{
			get
			{
				return new IndexExpression(Value, Line, Column);
			}
		}
    }
}
