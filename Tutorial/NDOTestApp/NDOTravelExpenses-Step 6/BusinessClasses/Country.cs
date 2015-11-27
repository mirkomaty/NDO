using System;
using System.Collections.Generic;
using System.Linq;
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

        [NDORelation]
        List<Travel> travels = new List<Travel>();
        public IEnumerable<Travel> Travels
        {
        	get { return this.travels; }
        	set { this.travels = value.ToList(); }
        }
        public void AddTravel(Travel t)
        {
        	this.travels.Add(t);
        }
        public void RemoveTravel(Travel t)
        {
        	if (this.travels.Contains(t))
        		this.travels.Remove(t);
        }

        public Country()
        {
        }
    }
}
