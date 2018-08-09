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
		private readonly IPersistenceHandlerCache persistenceHandlerCache;

		public NDOPersistenceHandlerManager(IUnityContainer configContainer, IPersistenceHandlerCache persistenceHandlerCache)
		{
			this.configContainer = configContainer;
			this.persistenceHandlerCache = persistenceHandlerCache;
		}
		/// <summary>
		/// Get a persistence handler for the given object.
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="useSelfGeneratedIds"></param>
		/// <returns></returns>
		public IPersistenceHandler GetPersistenceHandler( IPersistenceCapable pc, bool useSelfGeneratedIds )
		{
			return GetPersistenceHandler( pc.GetType(), useSelfGeneratedIds );
		}

		public IPersistenceHandler GetPersistenceHandler( Type t, bool useSelfGeneratedIds )
		{
			if (t.IsGenericType)
				t = t.GetGenericTypeDefinition();

			IPersistenceHandler handler;
			if (!persistenceHandlerCache.TryGetValue( t, out handler ))
			{
				// 1. Standard-Handler des pm versuchen

				handler = this.configContainer.Resolve<IPersistenceHandler>();

				// 3. NDOPersistenceHandler versuchen
				if (handler == null)
					handler = new SqlPersistenceHandler( this.configContainer );

				this.persistenceHandlerCache.Add( t, handler );
			}

			Mappings mappings = configContainer.Resolve<Mappings>();
			
			handler.Initialize( mappings, t, mappings.DataSet );

			return handler;
		}
	}
}
