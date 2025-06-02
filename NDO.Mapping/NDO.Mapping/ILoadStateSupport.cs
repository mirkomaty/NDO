using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Mapping
{
	/// <summary>
	/// Interface used by the NDO framework and JSONFormatter. Do not use this interface in user code.
	/// </summary>
	public interface ILoadStateSupport
	{
		/// <summary>
		/// Gets the ordinal of an initialized mapping object.
		/// </summary>
		int Ordinal { get; }
	}
}
