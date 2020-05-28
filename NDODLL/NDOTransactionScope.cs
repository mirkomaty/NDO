using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using ST = System.Transactions;

namespace NDO
{
	class NDOTransactionScope : IDisposable
	{
		TransactionScope innerScope;
		private readonly PersistenceManager pm;

		private Dictionary<string, IDbConnection> usedConnections = new Dictionary<string, IDbConnection>();

		public ST.IsolationLevel IsolationLevel { get; set; }
		public TransactionMode TransactionMode { get; set; }

		public NDOTransactionScope(PersistenceManager pm)
		{
			IsolationLevel = ST.IsolationLevel.ReadCommitted;
			TransactionMode = TransactionMode.Optimistic;
			this.pm = pm;
		}

		public void CheckTransaction()
		{
			if (TransactionMode == TransactionMode.None)
			{
				innerScope = null;
				return;
			}

			if (innerScope == null)
			{
				innerScope = new TransactionScope( TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = this.IsolationLevel } );
				this.pm.LogIfVerbose( "Creating a new TransactionScope" );
			}
		}

		public void Complete()
		{
			if (innerScope != null)
			{
				this.pm.LogIfVerbose( "Completing the TransactionScope" );
				innerScope.Complete();
				innerScope.Dispose();
			}

			CloseConnections();

			innerScope = null;
		}

		public IDbConnection GetConnection(string id, Func<IDbConnection> factory)
		{
			if (this.usedConnections.ContainsKey( id ))
			{
				return this.usedConnections[id];
			}
			else
			{
				var conn = factory();
				this.usedConnections.Add( id, conn );
				return conn;
			}
		}

		public void Dispose()
		{
			if (innerScope != null)
			{
				this.pm.LogIfVerbose( "Disposing the TransactionScope" );
				innerScope.Dispose();
				innerScope = null;
			}

			CloseConnections();
		}

		private void CloseConnections()
		{
			foreach (var conn in this.usedConnections.Values.Where( c => c.State == ConnectionState.Open ))
			{
				conn.Close();
				pm.LogIfVerbose( $"Closed connection {new Connection( null ) { Name = conn.ConnectionString }.DisplayName }" );
			}

			this.usedConnections.Clear();
		}
	}
}
