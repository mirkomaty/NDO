using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// Interface for type resolvers
	/// </summary>
	public interface IResolver : IDisposable
	{
		/// <summary>
		/// Creates an object with the given name.
		/// </summary>
		/// <param name="resolvingContainer">The container which started the resolve process</param>
		/// <param name="name"></param>
		/// <param name="overrides"></param>
		/// <returns></returns>
		object Resolve(INDOContainer resolvingContainer, string name, ParameterOverride[] overrides);
	}
}
