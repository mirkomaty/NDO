using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Represents a constant
    /// </summary>
    public class NamedConstantExpression : OqlExpression
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public NamedConstantExpression(string value, int line, int col) : base (line, col)
        {
            base.Value = value;
            if (String.Compare(value, "true", true) == 0 || String.Compare(value, "false", true) == 0)
                base.ExpressionType = ExpressionType.Boolean;
            else
                base.ExpressionType = ExpressionType.Unknown;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="negated"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public NamedConstantExpression(string value, bool negated, int line, int col) : this(value, line, col)
        {
            if (negated)
                base.UnaryOp = "NOT";
        }

        /// <summary>
        /// Clones the object
        /// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new NamedConstantExpression( (string)Value, Line, Column );
			}
		}
    }
}
