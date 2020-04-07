using NDO.Mapping;
using NDO.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.SqlPersistenceHandling
{
	internal class OrderGenerator
	{
		private readonly Class cls;

		public OrderGenerator( Class cls )
		{
			this.cls = cls;
		}



		public string GenerateOrderClause( List<QueryOrder> orderings, int skip, int take )
		{
			var ascOrderings = orderings.Where(o=>o.IsAscending);
			var descOrderings = orderings.Where(o=>!o.IsAscending);
			var ascOrderCols = (from o in ascOrderings select cls.FindField( o.FieldName ).Column.GetQualifiedName()).ToArray();
			var descOrderCols = (from o in descOrderings select cls.FindField( o.FieldName ).Column.GetQualifiedName()).ToArray();
			var ascOrderString = String.Join( ",", ascOrderCols );
			if (ascOrderString != String.Empty)
				ascOrderString += " ASC";
			var descOrderString = String.Join(",", descOrderCols);
			if (descOrderString != String.Empty)
				descOrderString += " DESC";
			string allOrderString;
			if (ascOrderString != String.Empty && descOrderString != String.Empty)
				allOrderString = ascOrderString + ", " + descOrderString;
			else if (ascOrderString != String.Empty)
				allOrderString = ascOrderString.Replace(" ASC", "");
			else
				allOrderString = descOrderString;

			var fetchLimit = String.Empty;
			if ((skip != 0 || take != 0) && this.cls.Provider.SupportsFetchLimit)
				fetchLimit = " " + this.cls.Provider.FetchLimit( skip, take );
			return $"ORDER BY {allOrderString}{fetchLimit}";
		}
	}
}
