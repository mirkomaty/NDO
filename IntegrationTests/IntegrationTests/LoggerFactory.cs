using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdoUnitTests
{
	class LoggerFactory : ILoggerFactory
	{
		ILogger logger;
		public void AddProvider( ILoggerProvider provider )
		{
		}

		public ILogger CreateLogger( string categoryName )
		{
			if (logger == null)
				logger = new TestLogger( categoryName );
			return logger;
		}

		public void Dispose()
		{			
		}
	}
}
