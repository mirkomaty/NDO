using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO
{
	public interface IPersistenceHandlerCache : IDictionary<Type, IPersistenceHandler>
	{
	}
}
