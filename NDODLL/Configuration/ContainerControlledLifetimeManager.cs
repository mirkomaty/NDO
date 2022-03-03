using NDO.LightInject;
using System;

namespace NDO.Configuration
{
	/// <summary>
	/// Obsolete ContainerControlledLifetime
	/// </summary>
	[Obsolete("Use PerContainerLifetime")]
	public class ContainerControlledLifetimeManager : PerContainerLifetime
	{
	}
}
