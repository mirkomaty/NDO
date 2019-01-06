using System;
using System.Collections.Concurrent;

namespace NDO
{
	/// <summary>
	/// Default implementation of the PersistenceHandler cache.
	/// </summary>
	/// <remarks>
	/// The PersistenceManager registers an instance of this cache.
	/// Per Default the PersistenceHandlerManager uses this instance to cache PersistenceHandlers.
	/// This decouples caching of the PersistenceHandlers from the PersistenceManager.
	/// You can register your own cache instance with <c>pm.ConfigContainer.RegisterInstance&lt;IPersistenceHandlerCache&gt;(yourInstance)</c>.
	/// It's generally a bad idea, to establish static instances of a cache.
	/// </remarks>
	public class NDOPersistenceHandlerCache : ConcurrentDictionary<Type, IPersistenceHandler>, IPersistenceHandlerCache
	{
	}
}
