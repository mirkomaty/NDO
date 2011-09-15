using System;
using System.Collections.Generic;
using NDO;

namespace Northwind
{
	[NDOPersistent]
	public class Territory
	{
		public Territory()
		{
		}
		string territoryDescription;

		[NDORelation]
		List<Employee> employees = new List<Employee>();

		[NDORelation]
		Region region;


#region Field accessors
		public string TerritoryDescription
		{
			get { return territoryDescription; }
			set { territoryDescription = value; }
		}

        public List<Employee> Employees
        {
            get { return new NDOReadOnlyGenericList<Employee>(employees); }
            set { employees = (List<Employee>)value; }
        }
        public void AddEmployee(Employee e)
        {
            employees.Add(e);
        }
        public void RemoveEmployee(Employee e)
        {
            if (employees.Contains(e))
                employees.Remove(e);
        }
        
        public Region Region
		{
			get { return region; }
			set { region = value; }
		}
#endregion Field accessors

	}
}
