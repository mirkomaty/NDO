using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	/// <summary>
	/// Specialized Class to support Descending ordering
	/// </summary>
	public class DescendingOrder : QueryOrder
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fn">Field name to be used in the ordering</param>
		public DescendingOrder( string fn )
			: base( "DESC", fn )
		{
		}

		/// <inheritdoc/>
		public override bool IsAscending
		{
			get { return false; }
		}
	}
}
