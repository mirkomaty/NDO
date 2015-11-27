using System;
using NDO;
namespace BusinessClasses
{
    [NDOPersistent]
    public abstract partial class Expense
    {
        DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public abstract string Text
        {
            get;
        }
        public abstract decimal Amount
        {
            get;
        }
        public Expense()
        {
        }
    }
}
