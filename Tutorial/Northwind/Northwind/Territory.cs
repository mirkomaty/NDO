//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


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
