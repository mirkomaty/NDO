using System;
using System.IO;
using NDO;

namespace QueryTests
{
	class NDOFactory
	{
		static NDOFactory instance;
		PersistenceManager persistenceManager;
		static NDOFactory()
		{
			instance = new NDOFactory();
		}

		public static NDOFactory Instance
		{
			get { return instance; }
		}

		public PersistenceManager PersistenceManager
		{
			get
			{
				if (this.persistenceManager == null)
				{
					var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
					this.persistenceManager = new PersistenceManager( Path.Combine( baseDirectory, "NDOMapping.xml" ) );
				}
				else
				{
					this.persistenceManager.Abort();
					this.persistenceManager.UnloadCache();
				}
				return this.persistenceManager;
			}
		}
	}
}
