using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Represents a ParameterList expression
    /// </summary>
    public class ParameterListExpression : OqlExpression
    {
        /// <summary>
        /// Construcror
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public ParameterListExpression(int line, int col) : base (line, col)
        {
            base.ExpressionType = ExpressionType.Unknown;
            HasBrackets = true;
        }

        /// <summary>
        /// Clones an ParameterListExpression object
        /// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new ParameterListExpression(Line, Column);
			}
		}

		/// <inheritdoc/>>
		public override bool IsTerminating => false;

		/// <inheritdoc/>>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder( (string) Value );
			int end = Children.Count - 1;
			var i = 0;
			foreach (var child in Children)
			{
				sb.Append( child.ToString() );
				if (i < end)
					sb.Append( ", " );
				i++;
			}
			return sb.ToString();
		}

		/// <inheritdoc/>>
		public override OqlExpression Simplify()
		{
			return this;
		}
	}
}
