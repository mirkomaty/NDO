using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO
{
	/// <summary>
	/// Manages getting PersistenceHandler for given objects and types.
	/// Caches the handlers.
	/// </summary>
	public interface IPersistenceHandlerManager
	{
		/// <summary>
		/// Get a persistence handler for the type of a given object.
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="useSelfGeneratedIds"></param>
		/// <returns></returns>
		IPersistenceHandler GetPersistenceHandler( IPersistenceCapable pc, bool useSelfGeneratedIds );

		/// <summary>
		/// Get a persistence handler for the given type.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="useSelfGeneratedIds"></param>
		/// <returns></returns>
		IPersistenceHandler GetPersistenceHandler( Type t, bool useSelfGeneratedIds );
	}
}
