using System;
using System.Collections;
using NDO;
namespace BusinessClasses
{
    [NDOPersistent]
    public partial class Country
    {

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [NDORelation(typeof(Travel))]
        IList travels = new ArrayList();
        public void AddTravel(Travel t)
        {
            travels.Add(t);
        }
        public void RemoveTravel(Travel t)
        {
            if (travels.Contains(t))
                travels.Remove(t);
        }
        public IList Travels
        {
            get { return ArrayList.ReadOnly(travels); }
            set { travels = value; }
        }
        public Country()
        {
        }
    }
}
