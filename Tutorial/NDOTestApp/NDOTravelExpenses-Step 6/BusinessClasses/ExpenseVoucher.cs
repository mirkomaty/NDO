using System;
using NDO;
namespace BusinessClasses
{
    [NDOPersistent]
    public class ExpenseVoucher : Expense
    {
        string text;
        public string VoucherText
        {
            get { return text; }
            set { text = value; }
        }
        decimal sum;
        public decimal Sum
        {
            get { return sum; }
            set { sum = value; }
        }
        public ExpenseVoucher()
        {
        }
        public override decimal Amount
        {
            get { return sum; }
        }

        public override string Text
        {
            get { return text; }
        }
    }
}
