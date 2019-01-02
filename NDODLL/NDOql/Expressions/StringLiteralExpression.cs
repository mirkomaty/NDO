using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    public class StringLiteralExpression : ConstantExpression
    {
        public StringLiteralExpression(string value, int line, int col) : base(line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.String;
        }

        public override string ToString()
        {
            return Value.ToString();
            // return "'" + Value + '\'';
        }

		public override OqlExpression DeepClone
		{
			get
			{
				return new StringLiteralExpression( (string)Value, Line, Column );
			}
		}
    }
}
