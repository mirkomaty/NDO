using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NDOInterfaces
{
	public interface INdoDbConnection : IDbConnection
	{
		/// <summary>
		/// Gets a unique id of the current server connection. This requires the connection to be open.
		/// </summary>
		object ConnectionId { get; }

		IDbConnection InnerConnection { get; }
	}
}
