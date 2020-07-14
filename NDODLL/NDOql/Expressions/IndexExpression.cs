using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Represents an index expression
    /// </summary>
    public class IndexExpression : OqlExpression
    {
        /// <summary>
        /// Construcror
        /// </summary>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public IndexExpression(object value, int line, int col) : base (line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.Unknown;
            HasBrackets = true;
        }

        /// <summary>
        /// Clones an IndexExpression object
        /// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new IndexExpression(Value, Line, Column);
			}
		}
    }
}
