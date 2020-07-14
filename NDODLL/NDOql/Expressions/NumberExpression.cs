using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Represents a constant number
    /// </summary>
    public class NumberExpression : ConstantExpression
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public NumberExpression(object value, int line, int col) : base (line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.Number;
        }

        /// <summary>
        /// Clones the object
        /// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new NumberExpression(Value, Line, Column);
			}
		}
    }
}
