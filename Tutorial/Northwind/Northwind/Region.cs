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
	public class Region
	{
		public Region()
		{
		}
		string regionDescription;

		[NDORelation]
		IList<Territory> territories = new List<Territory>();

        [NDORelation]
        List<Supplier> suppliers = new List<Supplier>();


#region Field accessors
		public string RegionDescription
		{
			get { return regionDescription; }
			set { regionDescription = value; }
		}

        public IList<Territory> Territories
        {
            get { return new NDOReadOnlyGenericList<Territory>(territories); }
            set { territories = value; }
        }
        public void AddTerritory(Territory t)
        {
            territories.Add(t);
        }
        public void RemoveTerritory(Territory t)
        {
            if (territories.Contains(t))
                territories.Remove(t);
        }

        public List<Supplier> Suppliers
        {
            get { return new NDOReadOnlyGenericList<Supplier>(suppliers); }
            set { suppliers = (List<Supplier>)value; }
        }
        public void AddSupplier(Supplier s)
        {
            suppliers.Add(s);
        }
        public void RemoveSupplier(Supplier s)
        {
            if (suppliers.Contains(s))
                suppliers.Remove(s);
        }

#endregion Field accessors

	}
}
