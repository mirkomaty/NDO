using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NDO.Linq
{
    /// <summary>
    /// Helper class used to combine expressions
    /// </summary>
    class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;
        public ReplaceVisitor( Expression from, Expression to )
        {
            this.from = from;
            this.to = to;
        }
        public override Expression Visit( Expression node )
        {
            return node == from ? to : base.Visit( node );
        }
    }
}
