using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NdoUnitTests
{
    public class ExpectedExceptionAttribute : Attribute
    {
        public Type ExpectedException { get; set; }
        public ExpectedExceptionAttribute(Type expectedExceptionType)
        {
            ExpectedException = expectedExceptionType;
        }

    }
}
