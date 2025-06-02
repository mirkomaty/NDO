using System;
using System.IO;
using DataTypeTestClasses;
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
						pm = new PersistenceManager( mapping, null );
					}
				}
				// Setting an AccessorName to avoid changing the full chain of dependencies for PureBusinessClasses
				pm.NDOMapping.FindClass( typeof( DataContainerDerived ) ).FindField( "byteArrVar" ).AccessorName = "ByteArrVar";
				return pm;
			}
		}
	}
}
