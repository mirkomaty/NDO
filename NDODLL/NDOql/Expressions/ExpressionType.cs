using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    /// <summary>
    /// Available Expression Types for Oql expressions.
    /// </summary>
    public enum ExpressionType
    {
        /// <summary/>
        Unknown = 0,
        /// <summary/>
        Number,
        /// <summary/>
        String,
        /// <summary/>
        Boolean,
        /// <summary/>
		Raw
    }
}
