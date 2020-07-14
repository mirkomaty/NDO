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
			Func<QueryOrder, string> fieldSelector = o => cls.FindField(o.FieldName)?.Column?.GetQualifiedName();
			Func<QueryOrder, string> oidSelector = o => cls.Oid.OidColumns.First().GetQualifiedName();

			var ascOrderCols = orderings.Where(o=>o.IsAscending && o.FieldName != "oid").Select(fieldSelector)
				.Union(orderings.Where(o=>o.IsAscending && o.FieldName == "oid").Select(oidSelector));
			var descOrderCols = orderings.Where(o=>!o.IsAscending && o.FieldName != "oid").Select(fieldSelector)
				.Union(orderings.Where(o=>!o.IsAscending && o.FieldName == "oid").Select(oidSelector));
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
