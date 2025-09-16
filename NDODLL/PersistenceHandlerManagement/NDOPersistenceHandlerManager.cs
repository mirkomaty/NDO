using NDO.SqlPersistenceHandling;
using System;
using Microsoft.Extensions.DependencyInjection;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// Default implementation of a PersistenceHandlerManager
	/// </summary>
	public class NDOPersistenceHandlerManager : IPersistenceHandlerManager
	{
		private readonly IServiceProvider configContainer;
		private readonly NDOMapping mappings;
		private readonly IPersistenceHandlerPool persistenceHandlerPool;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <param name="mappings"></param>
		public NDOPersistenceHandlerManager(IServiceProvider serviceProvider, NDOMapping mappings)
		{
			this.configContainer = serviceProvider;
			this.mappings = mappings;
			this.persistenceHandlerPool = serviceProvider.GetRequiredService< IPersistenceHandlerPool>();
		}
		/// <summary>
		/// Get a persistence handler for the given object.
		/// </summary>
		/// <param name="pc"></param>
		/// <returns></returns>
		public IPersistenceHandler GetPersistenceHandler( IPersistenceCapable pc )
		{
			return GetPersistenceHandler( pc.GetType() );
		}

		void ReleaseHandler(Type t, IPersistenceHandler handler)
		{
			// Don't close the connection or transaction here
			// because it might be used with other handlers.
			handler.Connection = null;
			this.persistenceHandlerPool.ReleaseHandler( t, handler );
		}

		/// <summary>
		/// Gets a persistence handler for a given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IPersistenceHandler GetPersistenceHandler( Type type )
		{
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();

			IPersistenceHandler handler = persistenceHandlerPool.GetHandler( type, (t)=>
			{
				// 1. If a handler type is registered, use an instance of this handler
				var newHandler = this.configContainer.GetService<IPersistenceHandler>();

				// 2. try to use an NDOPersistenceHandler
				if (newHandler == null)
					newHandler = new SqlPersistenceHandler( this.configContainer );

				return newHandler;
			});

			// The dataSet will be used as template to create a DataTable for the query results.
			handler.Initialize( this.mappings, type, ReleaseHandler );

			return handler;
		}
	}
}
