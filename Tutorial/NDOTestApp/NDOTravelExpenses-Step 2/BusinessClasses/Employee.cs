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
        Point position;

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

        public void SetPosition(int x, int y)
        {
            position = new Point(x, y);
        } 

        public Employee()
        {
        }
    }
}
