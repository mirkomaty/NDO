using System;
using System.Collections.Generic;
using System.Linq;
using NDO;
using System.Drawing;

namespace BusinessClasses
{
    /// <summary>
    /// Summary for Employee
    /// </summary>
    [NDOPersistent]
    public partial class Employee
    {
        [NDORelation(RelationInfo.Composite)]
        List<Travel> travels = new List<Travel>();
        public IEnumerable<Travel> Travels
        {
        	get { return this.travels; }
        	set { this.travels = value.ToList(); }
        }
        public Travel NewTravel()
        {
        	Travel t = new Travel();
        	this.travels.Add(t);
        	return t;
        }
        public void RemoveTravel(Travel t)
        {
        	if (this.travels.Contains(t))
        		this.travels.Remove(t);
        }

        [NDORelation(RelationInfo.Composite)]
        Address address; 
        public Address Address
        {
            get { return address; }
            set { address = value; }
        }

        string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        string lastName;
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public Employee()
        {
        }
    }
}
