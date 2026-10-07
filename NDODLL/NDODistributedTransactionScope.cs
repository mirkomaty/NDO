using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ST = System.Transactions;

namespace NDO
{
	/// <summary>
	/// TransactionScope class which uses System.Transactions to manage distributed transactions.
	/// </summary>
	/// <remarks>
	/// Register this type for the interface INDOTransactionScope, if you know about the consequences.
	/// See: https://docs.microsoft.com/en-us/dotnet/api/system.transactions.transactionscope
	/// The scope doesn't rely on an ambient transaction (Transaction.Current) because an ambient transaction
	/// started inside an async method doesn't flow back to the caller. Instead each connection is enlisted
	/// explicitly when it is opened. If the caller runs inside an ambient transaction, the scope joins it
	/// (like TransactionScopeOption.Required), otherwise it creates a CommittableTransaction.
	/// </remarks>
	public class NDODistributedTransactionScope : INDOTransactionScope
	{
		ST.Transaction transaction;
		ST.CommittableTransaction ownTransaction;
		ST.DependentTransaction dependentTransaction;
		private PersistenceManager pm;

		private Dictionary<string, DbConnection> usedConnections = new Dictionary<string, DbConnection>();

		///<inheritdoc/>
		public IsolationLevel IsolationLevel { get; set; }
		///<inheritdoc/>
		public TransactionMode TransactionMode { get; set; }

		/// <summary>
		/// Constructs an NDOTransactionScope object.
		/// </summary>
		public NDODistributedTransactionScope()
		{
			IsolationLevel = IsolationLevel.ReadCommitted;
			TransactionMode = TransactionMode.Optimistic;
		}

		///<inheritdoc/>
		public Task CheckTransactionAsync( CancellationToken cancellationToken = default )
		{
			if (TransactionMode == TransactionMode.None)
			{
				this.transaction = null;
				this.ownTransaction = null;
				this.dependentTransaction = null;
				return Task.CompletedTask;
			}

			if (this.transaction == null)
			{
				var ambient = ST.Transaction.Current;
				if (ambient != null)
				{
					this.dependentTransaction = ambient.DependentClone( ST.DependentCloneOption.BlockCommitUntilComplete );
					this.transaction = ambient;
					this.pm.LogIfVerbose( "Joining the ambient transaction" );
				}
				else
				{
					this.ownTransaction = new ST.CommittableTransaction( new ST.TransactionOptions() { IsolationLevel = (ST.IsolationLevel) Enum.Parse( typeof( ST.IsolationLevel ), this.IsolationLevel.ToString() ) } );
					this.transaction = this.ownTransaction;
					this.pm.LogIfVerbose( "Creating a new transaction" );
				}
			}

			return Task.CompletedTask;
		}

		void OnConnectionStateChange( object sender, StateChangeEventArgs e )
		{
			if (e.CurrentState != ConnectionState.Open || this.transaction == null)
				return;

			// The connection enlisted automatically, if the transaction is the ambient transaction.
			var ambient = ST.Transaction.Current;
			if (ambient != null && ambient.Equals( this.transaction ))
				return;

			((DbConnection) sender).EnlistTransaction( this.transaction );
		}

		void ReleaseTransaction()
		{
			this.ownTransaction?.Dispose();
			this.dependentTransaction?.Dispose();
			this.ownTransaction = null;
			this.dependentTransaction = null;
			this.transaction = null;
		}

		///<inheritdoc/>
		public async Task CompleteAsync( CancellationToken cancellationToken = default )
		{
			if (this.transaction != null)
			{
				this.pm.LogIfVerbose( "Completing the transaction" );
				// System.Transactions doesn't provide async APIs.
				if (this.ownTransaction != null)
					this.ownTransaction.Commit();
				else
					this.dependentTransaction.Complete();
			}

			await CloseConnectionsAsync().ConfigureAwait( false );

			ReleaseTransaction();
		}

		///<inheritdoc/>
		public Task<DbConnection> GetConnectionAsync( Connection ndoConnection, Func<DbConnection> factory, CancellationToken cancellationToken = default )
		{
			var id = ndoConnection.ID;
			if (this.usedConnections.ContainsKey( id ))
			{
				return Task.FromResult( this.usedConnections[id] );
			}
			else
			{
				var conn = factory();
				conn.StateChange += OnConnectionStateChange;
				this.usedConnections.Add( id, conn );
				return Task.FromResult( conn );
			}
		}

		///<inheritdoc/>
		public void Dispose()
		{
			DisposeAsync().ConfigureAwait( false ).GetAwaiter().GetResult();
		}

		///<inheritdoc/>
		public async ValueTask DisposeAsync()
		{
			if (this.transaction != null)
			{
				this.pm.LogIfVerbose( "Rolling back the transaction" );
				try
				{
					if (this.ownTransaction != null)
						this.ownTransaction.Rollback();
					else
						this.dependentTransaction.Rollback();
				}
				catch
				{
				}

				ReleaseTransaction();
			}

			await CloseConnectionsAsync().ConfigureAwait( false );
		}

		private async Task CloseConnectionsAsync()
		{
			foreach (var conn in this.usedConnections.Values)
			{
				conn.StateChange -= OnConnectionStateChange;
				if (conn.State != ConnectionState.Open)
					continue;
				await conn.CloseAsync().ConfigureAwait( false );
				pm.LogIfVerbose( $"Closed connection {new Connection( null ) { Name = conn.ConnectionString }.DisplayName }" );
			}

			this.usedConnections.Clear();
		}

		///<inheritdoc/>
		public DbTransaction GetTransaction( string id )
		{
			return null;
		}

		/// <inheritdoc/>
		public INDOTransactionScope Initialize( PersistenceManager pm )
		{
			this.pm = pm;
			return this;
		}
	}
}
