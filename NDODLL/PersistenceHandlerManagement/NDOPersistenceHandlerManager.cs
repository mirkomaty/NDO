using NDO.SqlPersistenceHandling;
using System;
using Microsoft.Extensions.DependencyInjection;
using NDO.Mapping;
using System.Collections.Generic;
using NDO.Logging;

namespace NDO
{
	/// <summary>
	/// Default implementation of a PersistenceHandlerManager
	/// </summary>
	public class NDOPersistenceHandlerManager
	{
		private readonly IServiceProvider serviceProvider;
		private readonly NDOMapping mappings;
		private readonly Dictionary<Type, IPersistenceHandler> handlers = new Dictionary<Type, IPersistenceHandler>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <param name="mappings"></param>
		public NDOPersistenceHandlerManager(IServiceProvider serviceProvider, NDOMapping mappings)
		{
			this.serviceProvider = serviceProvider;
			this.mappings = mappings;
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
		}

		/// <summary>
		/// Gets a persistence handler for a given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IPersistenceHandler GetPersistenceHandler( Type type )
		{
			// This code doesn't need to be thread safe because
			// each thread needs another pm and therefore another PersistenceHandler instance.
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();
			
			if( !handlers.TryGetValue( type, out var handler ))
			{
				// If a handler type is registered, use an instance of this handler
				handler = this.serviceProvider.GetService<IPersistenceHandler>();

				// We shouldn't reach this code, since NDO registers the NdoPersistenceHandler
				if (handler == null)
					handler = new SqlPersistenceHandler( this.serviceProvider );

				handlers.Add( type, handler );
			};


			// The dataSet will be used as template to create a DataTable for the query results.
			handler.Initialize( this.mappings, type, ReleaseHandler );

			return handler;
		}
	}
}
