using System;
using System.Collections;
using NDO;
namespace BusinessClasses
{
    [NDOPersistent]
    public class Travel
    {
        string purpose;
        public string Purpose
        {
            get { return purpose; }
            set { purpose = value; }
        }

        public Travel()
        {
        }
    }
}
