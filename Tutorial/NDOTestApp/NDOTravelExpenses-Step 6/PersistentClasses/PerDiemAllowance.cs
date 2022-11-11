//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using NDO;
using System.ComponentModel;

namespace BusinessClasses
{
    [NDOPersistent, DisplayName("Per Diem Allowance")]
    public partial class PerDiemAllowance : Expense
    {
        decimal hours;
        public decimal Hours
        {
            get { return hours; }
            set { hours = value; }
        }
        public PerDiemAllowance()
        {
        }

        public override decimal Amount
        {
            get
            {
                //TODO: call accountant once more  ;-) 
                if (hours <= 8)
                    return 0;
                if (hours <= 10)
                    return 5;
                if (hours <= 12)
                    return 10;
                return 20;
            }
        }

        public override string Text
        {
            get { return "Per Diem Allowance"; }
        }
    }
}
