using System;

namespace BusinessClasses
{
    public class ExpenseFactory
    {
        string[] theTypes = new string[] { "Expense Voucher", "Milage Allowance", "Per Diem Allowance" };
        public string[] Types
        {
            get { return theTypes; }
        }
        public Expense NewExpense(string type)
        {
            switch (type)
            {
                case "Expense Voucher":
                    return new ExpenseVoucher();
                    break;
                case "Milage Allowance":
                    return new MileageAllowance();
                    break;
                case "Per Diem Allowance":
                    return new PerDiemAllowance();
                    break;
                default:
                    throw new Exception
                        (String.Format("Unknown Expense Type: {0}", type));
            }
        }
    }
}
