using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	/// <summary>
	/// Specialized Class to support Ascending ordering
	/// </summary>
	public class AscendingOrder : QueryOrder
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fn">Field name to be used in the ordering</param>
		public AscendingOrder( string fn )
			: base( "ASC", fn )
		{
		}

		/// <summary>
		/// Returns always true.
		/// </summary>
		public override bool IsAscending
		{
			get { return true; }
		}
	}
}
