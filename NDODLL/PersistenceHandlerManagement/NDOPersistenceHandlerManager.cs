using NDO.SqlPersistenceHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;

namespace NDO
{
	/// <summary>
	/// Default implementation of a PersistenceHandlerManager
	/// </summary>
	public class NDOPersistenceHandlerManager : IPersistenceHandlerManager
	{
		private readonly IUnityContainer configContainer;
		private readonly IPersistenceHandlerPool persistenceHandlerPool;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configContainer"></param>
		/// <param name="persistenceHandlerPool"></param>
		public NDOPersistenceHandlerManager(IUnityContainer configContainer, IPersistenceHandlerPool persistenceHandlerPool)
		{
			this.configContainer = configContainer;
			this.persistenceHandlerPool = persistenceHandlerPool;
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
			handler.Transaction = null;
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
				var newHandler = this.configContainer.Resolve<IPersistenceHandler>();

				// 2. try to use an NDOPersistenceHandler
				if (newHandler == null)
					newHandler = new SqlPersistenceHandler( this.configContainer );

				return newHandler;
			});

			Mappings mappings = configContainer.Resolve<Mappings>();
			// The dataSet will be used as template to create a DataTable for the query results.
			handler.Initialize( mappings, type, ReleaseHandler );

			return handler;
		}
	}
}
