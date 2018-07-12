using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    public class RawIdentifierExpression : OqlExpression
    {
        public RawIdentifierExpression(string value, int line, int col) : base (line, col)
        {
			base.Value = value;
            base.ExpressionType = ExpressionType.Raw;
        }

		public override OqlExpression DeepClone
		{
			get
			{
				return new RawIdentifierExpression( (string)Value, Line, Column );
			}
		}
    }
}
