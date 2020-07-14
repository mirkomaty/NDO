using NDO.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// This class is used by the query engine.
	/// </summary>
	public class QueryContextsEntry
	{
		/// <summary/>
		public QueryContexts QueryContexts { get; set; }
		/// <summary/>
		public Type Type { get; set; }
	}
}
