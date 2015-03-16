using System;
using NDO;

namespace BusinessClasses
{
    [NDOPersistent]
    public partial class MileageAllowance : Expense
    {
        int milesDriven;
        public int MilesDriven
        {
            get { return milesDriven; }
            set { milesDriven = value; }
        }
        public MileageAllowance()
        {
        }

        public override decimal Amount
        {
            //TODO: call accountant and get correct values ;-) 
            get { return (decimal)milesDriven * (decimal)0.4; }
        }

        public override string Text
        {
            get { return "Mileage Allowance"; }
        }
    }
}
