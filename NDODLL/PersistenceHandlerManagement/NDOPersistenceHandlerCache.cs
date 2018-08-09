using System;
using System.Collections.Concurrent;

namespace NDO
{
	/// <summary>
	/// Default implementation of the PersistenceHandler cache.
	/// Uses a static cache to add objects.
	/// </summary>
	public class NDOPersistenceHandlerCache : ConcurrentDictionary<Type, IPersistenceHandler>, IPersistenceHandlerCache
	{
		public static NDOPersistenceHandlerCache Instance { get; set; }

		static NDOPersistenceHandlerCache()
		{
			Instance = new NDOPersistenceHandlerCache();
		}
	}
}
