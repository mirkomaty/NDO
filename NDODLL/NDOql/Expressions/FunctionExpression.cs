using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Represents a function expression
    /// </summary>
    public class FunctionExpression : OqlExpression
    {
        /// <summary>
        /// Construcror
        /// </summary>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public FunctionExpression(object value, int line, int col) : base (line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.Unknown;
            HasBrackets = false;
        }

        /// <summary>
        /// Clones an FunctionExpression object
        /// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new FunctionExpression(Value, Line, Column);
			}
		}

        /// <inheritdoc/>>
		public override string ToString()
		{
            StringBuilder sb = new StringBuilder( (string)Value );
            var parList = Children[0];
            sb.Append( parList.ToString() );
            return sb.ToString();
		}

        /// <inheritdoc/>>
		public override OqlExpression Simplify()
		{
			return this;
		}
	}
}
