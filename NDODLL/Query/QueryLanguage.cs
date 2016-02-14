using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	/// <summary>
	/// Type of query language used
	/// </summary>
	public enum QueryLanguage
	{
		/// <summary>
		/// use NDOql
		/// </summary>
		NDOql,
		/// <summary>
		/// use SQL (Professional Edition only)
		/// </summary>
		Sql
	}
}
