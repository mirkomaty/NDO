using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// Interface for type resolvers
	/// </summary>
	public interface IResolver
	{
		/// <summary>
		/// Creates an object with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="overrides"></param>
		/// <returns></returns>
		object Resolve(string name, ParameterOverride[] overrides);
	}
}
