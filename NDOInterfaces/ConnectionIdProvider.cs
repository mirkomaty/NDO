using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace NDOInterfaces
{
	public class ConnectionIdProvider
	{
		static ConcurrentDictionary<IDbConnection, int> connectionIds = new ConcurrentDictionary<IDbConnection, int>();
		static int connectionId;

		/// <summary>
		/// This method can be used as EventHandler for the StateChange event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void HandleStateChange( object sender, StateChangeEventArgs e )
		{
			if (e.CurrentState == ConnectionState.Open)
				Register( (IDbConnection) sender );
			if (e.CurrentState == ConnectionState.Closed)
				Unregister( (IDbConnection) sender );
		}

		/// <summary>
		/// This should be called, if the StateChange of the connection changes to "Open".
		/// </summary>
		/// <param name="connection"></param>
		static void Register(IDbConnection connection)
		{
			// Create a new id
			var nextVal = Interlocked.Increment( ref connectionId );
			// set the id for the connection
			var x = connectionIds.GetOrAdd( connection, dbconn => nextVal );
		}

		/// <summary>
		/// This should be called, if the StateChange of the connection changes from "Open" to "Closed"
		/// </summary>
		/// <param name="connection"></param>
		static void Unregister(IDbConnection connection)
		{
			connectionIds.TryRemove( connection, out _ );
		}

		/// <summary>
		/// If a connection has been closed, it will be unregistered and it's Id is null.
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public static int Get(IDbConnection connection)
		{
			if (connectionIds.ContainsKey( connection ))
				return connectionIds[connection];

			return 0;
		}
	}
}
