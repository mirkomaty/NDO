using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Represents a string literal
    /// </summary>
    public class StringLiteralExpression : ConstantExpression
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public StringLiteralExpression(string value, int line, int col) : base(line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.String;
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            return Value.ToString();
            // return "'" + Value + '\'';
        }

        /// <summary>
		/// Constructs a clone of the object
        /// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new StringLiteralExpression( (string)Value, Line, Column );
			}
		}
    }
}
