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
