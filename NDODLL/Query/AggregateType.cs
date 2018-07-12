using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	/// <summary>
	/// Used as parameter for ExecuteAggregate. 
	/// </summary>
	public enum AggregateType
	{
		/// <summary>
		/// Average value
		/// </summary>
		Avg,
		/// <summary>
		/// Minimum
		/// </summary>
		Min,
		/// <summary>
		/// Maximum
		/// </summary>
		Max,
		/// <summary>
		/// Count
		/// </summary>
		Count,
		/// <summary>
		/// Sum 
		/// </summary>
		Sum,
		/// <summary>
		/// Standard Deviation
		/// </summary>
		StDev,
		/// <summary>
		/// Variance
		/// </summary>
		Var
	}
}
