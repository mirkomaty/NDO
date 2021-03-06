using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Represents an identifier
    /// </summary>
    public class IdentifierExpression : OqlExpression
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public IdentifierExpression(string value, int line, int col) : base (line, col)
        {
            string temp = value;
            if (value.StartsWith("["))
                temp = temp.Substring(1);
            if (value.EndsWith("]"))
                temp = temp.Substring(0, temp.Length - 1);
            base.Value = temp;

            base.ExpressionType = ExpressionType.Raw;
        }

        /// <summary>
        /// Clones an IdentifierExpression and all it's children
        /// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new IdentifierExpression( (string)Value, Line, Column );
			}
		}
    }
}
