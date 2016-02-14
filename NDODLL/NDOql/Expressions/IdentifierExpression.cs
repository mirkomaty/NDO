using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    public class IdentifierExpression : OqlExpression
    {
        public IdentifierExpression(string value, int line, int col) : base (line, col)
        {
            string temp = value;
            if (value.StartsWith("["))
                temp = temp.Substring(1);
            if (value.EndsWith("]"))
                temp = temp.Substring(0, temp.Length - 1);
            base.Value = temp;

            base.ExpressionType = ExpressionType.Unknown;
        }

		public override OqlExpression DeepClone
		{
			get
			{
				return new IdentifierExpression( (string)Value, Line, Column );
			}
		}
    }
}
