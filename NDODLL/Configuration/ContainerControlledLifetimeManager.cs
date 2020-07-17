using System;

namespace NDO.Configuration
{
	/// <summary>
	/// Creates one instance of a type during the lifetime of a controller
	/// </summary>
	public class ContainerControlledLifetimeManager : ILifetimeManager
	{
		///<inheritdoc/>
		public IResolver CreateResolver(Type tFrom, Type tTo)
		{
			return new ContainerControlledTypeMappingResolver( tFrom, tTo );
		}
	}
}
