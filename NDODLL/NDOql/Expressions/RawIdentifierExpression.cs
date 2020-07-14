using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
	/// <summary>
	/// Represents a name in the expression tree
	/// </summary>
    public class RawIdentifierExpression : OqlExpression
    {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value"></param>
		/// <param name="line"></param>
		/// <param name="col"></param>
        public RawIdentifierExpression(string value, int line, int col) : base (line, col)
        {
			base.Value = value;
            base.ExpressionType = ExpressionType.Raw;
        }

		/// <summary>
		/// Constructs a clone of the object
		/// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new RawIdentifierExpression( (string)Value, Line, Column );
			}
		}
    }
}
