using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Provider
{
	/// <summary>
	/// DI Interface for the NDOProviderFactory
	/// </summary>
	/// <remarks>Register your own implementation before you use the NDOProviderFactory for the first time.</remarks>
	public interface IProviderPathFinder
	{
		/// <summary>
		/// Returns a list of directories where providers can be located.
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetPaths();		
	}
}
