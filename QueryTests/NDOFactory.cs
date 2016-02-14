using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
					this.persistenceManager = new PersistenceManager();
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
