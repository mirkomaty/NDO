using NDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTests
{
	class TestPersistenceHandlerManager : IPersistenceHandlerManager
	{
		private readonly IPersistenceHandler handler;

		public TestPersistenceHandlerManager(IPersistenceHandler handler)
		{
			this.handler = handler;
		}
		public IPersistenceHandler GetPersistenceHandler( IPersistenceCapable pc )
		{
			return handler;
		}

		public IPersistenceHandler GetPersistenceHandler( Type t )
		{
			return handler;
		}
	}
}
