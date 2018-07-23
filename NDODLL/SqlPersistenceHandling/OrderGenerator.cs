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
			var orderCols = (from o in orderings select cls.FindField( o.FieldName ).Column.GetQualifiedName()).ToArray();
			var fetchLimit = String.Empty;
			if ((skip != 0 || take != 0) && this.cls.Provider.SupportsFetchLimit)
				fetchLimit = " " + this.cls.Provider.FetchLimit( skip, take );
			return $"ORDER BY {String.Join( ", ", orderCols )}{fetchLimit}";
		}
	}
}
