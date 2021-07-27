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
			//CheckForSingleQuotes( value );
			base.Value = value;
            base.ExpressionType = ExpressionType.String;
        }

		///// <summary>
		///// Converts a string literal, so that it can't contain a single quote.
		///// </summary>
		///// <param name="value"></param>
		///// <returns></returns>
		//static void CheckForSingleQuotes( string value )
		//{
		//	var temp = value.Substring( 1, value.Length - 2 );
		//	var sb = new StringBuilder( "'" );
		//	var last = temp.Length - 1;
		//	for (int i = 0; i <= last; i++)
		//	{
		//		var c = temp[i];
		//		if (c == '\'')
		//		{
		//			if (i < last && temp[i + 1] == '\'')
		//			{
		//				i++; // Skip second '
		//			}
		//			sb.Append( "''" ); // Always append two quotes
		//		}
		//		else
		//		{
		//			sb.Append( c );
		//		}
		//	}
		//	sb.Append( '\'' );
		//	return sb.ToString();
		//}

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
