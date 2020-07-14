using System;
using System.Collections.Generic;
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
	public class NDOPersistenceHandlerPool : IPersistenceHandlerPool
	{
		ConcurrentDictionary<Type, Stack<IPersistenceHandler>> handlerPools = new ConcurrentDictionary<Type, Stack<IPersistenceHandler>>();

		/// <summary>
		/// Gets a PersistenceHandler from the pool. If no handler exists, it will be created using the given factory method.
		/// </summary>
		/// <param name="type">The persistent class for which the handler is responsible</param>
		/// <param name="factory">A factory method to create a new handler of the type</param>
		/// <returns></returns>
		public IPersistenceHandler GetHandler( Type type, Func<Type, IPersistenceHandler> factory )
		{
			var list = handlerPools.GetOrAdd( type, (t)=>new Stack<IPersistenceHandler>() );
			lock(list)
			{
				if (list.Count > 0)
					return list.Pop();
			}
			return factory( type );
		}

		/// <summary>
		/// Returns a handler to the pool.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="handler"></param>
		public void ReleaseHandler( Type type, IPersistenceHandler handler )
		{
			var list = handlerPools.GetOrAdd( type, (t)=>new Stack<IPersistenceHandler>() );
			lock(list)
			{
				list.Push( handler );
			}
		}
	}
}
