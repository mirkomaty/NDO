using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// Creates a new objects for every Resolve
	/// </summary>
	public class TransientLifetimeManager : ILifetimeManager
	{
		///<inheritdoc/>
		public IResolver CreateResolver(Type tFrom, Type tTo)
		{
			return new TypeMappingResolver(tFrom, tTo);
		}
	}
}
