using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NDOInterfaces
{
	/// <summary>
	/// Replacement for the obsolete IFormatter interface.
	/// </summary>
	public interface INdoFormatter
	{
		/// <summary>
		/// Deserializes an object or object graph from a stream
		/// </summary>
		/// <param name="serializationStream"></param>
		/// <returns></returns>
		object Deserialize( Stream serializationStream );

		/// <summary>
		/// Serializes an object graph to a stream
		/// </summary>
		/// <param name="serializationStream"></param>
		/// <param name="graph"></param>
		void Serialize( Stream serializationStream, object graph );
	}
}
