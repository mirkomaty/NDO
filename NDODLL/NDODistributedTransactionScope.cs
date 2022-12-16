using NDO.Logging;
using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ST = System.Transactions;

namespace NDO
{
	/// <summary>
	/// TransactionScope class which uses System.Transactions.TransactionScope to manage distributed transactions.
	/// </summary>
	/// <remarks>
	/// Register this type for the interface INDOTransactionScope, if you know about the consequences.
	/// See: https://docs.microsoft.com/en-us/dotnet/api/system.transactions.transactionscope
	/// </remarks>
	public class NDODistributedTransactionScope : INDOTransactionScope
	{
		ST.TransactionScope innerScope;
		//private readonly PersistenceManager pm;

		private Dictionary<string, DbConnection> usedConnections = new Dictionary<string, DbConnection>();
		private readonly ILogAdapter logger;

		///<inheritdoc/>
		public IsolationLevel IsolationLevel { get; set; }
		///<inheritdoc/>
		public TransactionMode TransactionMode { get; set; }

		/// <summary>
		/// Constructs an NDOTransactionScope object.
		/// </summary>
		/// <param name="logger"></param>
		public NDODistributedTransactionScope( ILogAdapter logger )
		{
			IsolationLevel = IsolationLevel.ReadCommitted;
			TransactionMode = TransactionMode.Optimistic;
			this.logger = logger;
		}

		///<inheritdoc/>
		public void CheckTransaction()
		{
			if (TransactionMode == TransactionMode.None)
			{
				innerScope = null;
				return;
			}

			if (innerScope == null)
			{
				innerScope = new ST.TransactionScope( ST.TransactionScopeOption.Required, new ST.TransactionOptions() { IsolationLevel = (ST.IsolationLevel) Enum.Parse( typeof( ST.IsolationLevel ), this.IsolationLevel.ToString() ) } );
				this.logger.Debug( "Creating a new TransactionScope" );
			}
		}

		///<inheritdoc/>
		public void Complete()
		{
			if (innerScope != null)
			{
				this.logger.Debug( "Completing the TransactionScope" );
				innerScope.Complete();
				innerScope.Dispose();
			}

			CloseConnections();

			innerScope = null;
		}

		///<inheritdoc/>
		public DbConnection GetConnection( string id, Func<DbConnection> factory )
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

		///<inheritdoc/>
		public void Dispose()
		{
			if (innerScope != null)
			{
				this.logger.Debug( "Disposing the TransactionScope" );
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
				this.logger.Debug( $"Closed connection {new Connection( null ) { Name = conn.ConnectionString }.DisplayName }" );
			}

			this.usedConnections.Clear();
		}

		///<inheritdoc/>
		public DbTransaction GetTransaction( string id )
		{
			return null;
		}
	}
}
