using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Exception class for expressions
    /// </summary>
    public class OqlExpressionException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s"></param>
        public OqlExpressionException(string s) : base (s)
        {
        }
    }
}
