using System;
using System.IO;
using NDO;
using NDO.Mapping;

namespace QueryTests
{
	class NDOFactory
	{
		static object lockObject = new object();
		static NDOFactory instance;
		static NDOMapping mapping;

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
				PersistenceManager pm;
				lock (lockObject)
				{
					if (mapping == null)
					{
						pm = new PersistenceManager();
						mapping = pm.NDOMapping;
					}
					else
					{
						pm = new PersistenceManager( mapping );
					}
				}

				return pm;
			}
		}
	}
}
