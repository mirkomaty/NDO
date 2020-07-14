using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
	/// <summary>
	/// Represents a query parameter
	/// </summary>
    public class ParameterExpression : OqlExpression
    {
		/// <summary>
		/// Gets or sets the value represented by the parameter
		/// </summary>
		public object ParameterValue { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value"></param>
		/// <param name="line"></param>
		/// <param name="col"></param>
        public ParameterExpression(string value, int line, int col) : base(line, col)
        {
            base.Value = value;
            base.ExpressionType = ExpressionType.Unknown;
        }

		/// <summary>
		/// Gets or sets the index of the parameter in the parameter list
		/// </summary>
		public int Ordinal
		{
			get
			{
				string valStr = (string)Value;
				valStr = valStr.Substring( 1, valStr.Length - 2 );
				return int.Parse( valStr );
			}
			set
			{
				Value = "{" + value.ToString() + '}';
			}
		}

		/// <summary>
		/// Constructs a deep clone of the expression
		/// </summary>
		public override OqlExpression DeepClone
		{
			get
			{
				return new ParameterExpression( (string)Value, Line, Column );
			}
		}
    }
}
