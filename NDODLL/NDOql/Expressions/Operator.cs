using System;
using System.Collections.Generic;
using System.Text;

namespace NDOql.Expressions
{
    class Operator
    {
        static string[] operators = 
        {
                    // results  requires
            "AND",  // bool     bool
            "OR",   // bool     bool
            "=",    // bool
            "<>",   // bool
            "LIKE", // bool     string
		    "<",    // bool     
		    ">",    // bool
		    "<=",   // bool
		    ">=",   // bool     
            "|",    // bool     Number
            "^",    // bool     Number
            "&",    // bool     Number
            "+",    // bool     !bool
            "-",    // bool     Number
            "*",    // bool     Number
            "/",    // bool     Number
            "%",    // bool     Number
            "MOD"   // bool     Number
        };
    }
}
