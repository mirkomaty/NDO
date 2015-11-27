using System;
using NDO;

namespace BusinessClasses
{
    [NDOPersistent]
    public partial class PictureHeader
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        DateTime creationDate;
        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        public PictureHeader()
        {
        }
    }
}
