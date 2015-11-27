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
