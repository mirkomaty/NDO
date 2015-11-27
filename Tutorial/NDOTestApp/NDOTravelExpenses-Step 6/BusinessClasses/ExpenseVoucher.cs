using System;
using NDO;
using System.ComponentModel;

namespace BusinessClasses
{
    [NDOPersistent, DisplayName("Expense Voucher")]
    public partial class ExpenseVoucher : Expense
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
