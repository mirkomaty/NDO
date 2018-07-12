using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    public class OqlExpressionException : Exception
    {
        public OqlExpressionException(string s) : base (s)
        {
        }
    }
}
