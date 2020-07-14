using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO
{
	/// <summary>
	/// Pool for PersistenceHandler
	/// </summary>
	public interface IPersistenceHandlerPool
	{
		/// <summary>
		/// Gets a PersistenceHandler from the pool. If no handler exists, it will be created using the given factory method.
		/// </summary>
		/// <param name="t">The persistent class for which the handler is responsible</param>
		/// <param name="factory">A factory method to create a new handler of the type</param>
		/// <returns></returns>
		IPersistenceHandler GetHandler( Type t, Func<Type, IPersistenceHandler> factory );

		/// <summary>
		/// Returns a handler to the pool.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="handler"></param>
		void ReleaseHandler( Type t, IPersistenceHandler handler );
	}
}
