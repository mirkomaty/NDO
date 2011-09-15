using System;
using System.Collections;
using System.Collections.Generic;
using NDO;
using System.Drawing;

namespace BusinessClasses
{
    /// <summary>
    /// Summary for Employee
    /// </summary>
    [NDOPersistent]
    public class Employee
    {
        [NDORelation(typeof(Travel), RelationInfo.Composite)]
        IList travels = new ArrayList();

        public IList Travels
        {
            get { return ArrayList.ReadOnly(travels); }
            set { travels = value; }
        }
        public Travel NewTravel()
        {
        	Travel t = new Travel();
        	travels.Add(t);
        	return t;
        }

        public void RemoveTravel(Travel t)
        {
        	if (travels.Contains(t))
        		travels.Remove(t);
        }

#if UseGenerics
		[NDORelation(RelationInfo.Composite)]
		List<Travel> travels = new List<Travel>();
        public List<Travel> Travels
        {
            get { return new NDOReadOnlyGenericList<Travel>(travels); }
            set { travels = (List<Travel>)value; }
        }
#endif

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
