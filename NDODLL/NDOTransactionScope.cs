using NDO.Mapping;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NDO
{
	class UsedConnectionsInfo
	{
		public DbConnection Connection;
		public IProvider Provider;
	}

	/// <summary>
	/// class NDOTransactionScope
	/// </summary>
	public class NDOTransactionScope : INDOTransactionScope
	{
		private PersistenceManager pm;

		private Dictionary<string, UsedConnectionsInfo> usedConnections = new Dictionary<string, UsedConnectionsInfo>();
		private Dictionary<string, DbTransaction> usedTransactions = new Dictionary<string, DbTransaction>();

		///<inheritdoc/>
		public IsolationLevel IsolationLevel { get; set; }
		///<inheritdoc/>
		public TransactionMode TransactionMode { get; set; }

		bool isInTransaction = false;

		/// <summary>
		/// Constructs an NDOTransactionScope object.
		/// </summary>
		public NDOTransactionScope()
		{
			IsolationLevel = IsolationLevel.ReadCommitted;
			TransactionMode = TransactionMode.Optimistic;
		}

		///<inheritdoc/>
		public async Task CheckTransactionAsync( CancellationToken cancellationToken = default )
		{
			if (TransactionMode == TransactionMode.None)
			{
				return;
			}

			if (!this.isInTransaction)
			{
				foreach (var connId in this.usedConnections.Keys)
				{
					await OpenConnAndStartTransactionAsync( connId, cancellationToken ).ConfigureAwait( false );
				}
			}

			this.isInTransaction = true;
		}

		private async Task OpenConnAndStartTransactionAsync( string id, CancellationToken cancellationToken )
		{
			var cinfo = this.usedConnections[id];
			var provider = cinfo.Provider;
			var conn = cinfo.Connection;
			await conn.OpenAsync( cancellationToken ).ConfigureAwait( false );
			var serverId = provider.GetConnectionId(conn);
			pm.LogIfVerbose( $"Opening connection {serverId} = '{conn.DisplayName()}'" );
			var tx = await conn.BeginTransactionAsync( IsolationLevel, cancellationToken ).ConfigureAwait( false );
			usedTransactions.Add( id, tx );
			this.pm.LogIfVerbose( $"Starting transaction {tx.GetHashCode():X} at connection {serverId} = '{conn.DisplayName()}'" );
		}

		///<inheritdoc/>
		public async Task CompleteAsync( CancellationToken cancellationToken = default )
		{
			await CommitTransactionsAsync( cancellationToken ).ConfigureAwait( false );
			await CloseConnectionsAsync().ConfigureAwait( false );
			isInTransaction = false;
		}

		private async Task CommitTransactionsAsync( CancellationToken cancellationToken )
		{
			foreach (var id in usedTransactions.Keys)
			{
				var tx = usedTransactions[id];
				await tx.CommitAsync( cancellationToken ).ConfigureAwait( false );

				usedConnections.TryGetValue( id, out var cinfo );
				var conn = cinfo?.Connection;
				if (conn == null)
					throw new NDOException( 121, $"Can't commit. No open connection found for NDO Connection {id} ({conn.DisplayName()})" );
				var serverId = cinfo.Provider.GetConnectionId(conn);
				this.pm.LogIfVerbose( $"Committing transaction {tx.GetHashCode():X} at connection {serverId} = '{conn.DisplayName()}'" );
			}

			usedTransactions.Clear();
		}

		///<inheritdoc/>
		public async Task<DbConnection> GetConnectionAsync( Connection ndoConnection, Func<DbConnection> factory, CancellationToken cancellationToken = default )
		{
			var id = ndoConnection.ID;
			if (this.usedConnections.ContainsKey( id ))
			{
				return this.usedConnections[id].Connection;
			}
			else
			{
				var conn = factory();
				this.usedConnections.Add( id, new UsedConnectionsInfo { Connection = conn, Provider = ndoConnection.Provider } );
				if (this.isInTransaction)
					await OpenConnAndStartTransactionAsync( id, cancellationToken ).ConfigureAwait( false );
				return conn;
			}
		}


		///<inheritdoc/>
		public DbTransaction GetTransaction( string id )
		{
			if (isInTransaction)
			{
				DbTransaction tx = null;
				this.usedTransactions.TryGetValue( id, out tx );
				return tx;
			}

			return null;
		}

		///<inheritdoc/>
		public void Dispose()
		{
			DisposeAsync().ConfigureAwait( false ).GetAwaiter().GetResult();
		}

		///<inheritdoc/>
		public async ValueTask DisposeAsync()
		{
			await RollbackTransactionsAsync().ConfigureAwait( false );
			await CloseConnectionsAsync().ConfigureAwait( false );
			this.isInTransaction = false;
		}

		private async Task RollbackTransactionsAsync()
		{
			foreach (var key in usedTransactions.Keys)
			{
				var tx = this.usedTransactions[key];
				var id = tx.GetHashCode();
				try
				{
					// See https://github.com/dotnet/runtime/issues/95399
					// We don't have any information about the state of the transaction.
					// If it is completed, we will get an exception here.
					// Given that in most cases it's possible to track the tx state outside of NDO,
					// we are safe here in the most cases.
					// A rollback must not be canceled, otherwise transactions and connections stay open.
					await tx.RollbackAsync( CancellationToken.None ).ConfigureAwait( false );
				}
				catch
				{
				}

				if (this.usedConnections.TryGetValue( key, out var cinfo ))
				{
					var conn = cinfo.Connection;
					var provider = cinfo.Provider;
					var serverId = provider.GetConnectionId(conn);
					this.pm.LogIfVerbose( $"Rollback transaction {id.ToString( "X" )} at connection {serverId} = '{conn.DisplayName()}'" );
				}
			}

			usedTransactions.Clear();
		}

		private async Task CloseConnectionsAsync()
		{
			foreach (var cinfo in this.usedConnections.Values)
			{
				var conn = cinfo.Connection;
				if (conn.State != ConnectionState.Open)
					continue;
				var serverId = cinfo.Provider.GetConnectionId(conn);
				pm.LogIfVerbose( $"Closed connection {serverId} = '{conn.DisplayName()}'" );
				await conn.DisposeAsync().ConfigureAwait( false );
			}

			this.usedConnections.Clear();
		}

		/// <inheritdoc/>
		public INDOTransactionScope Initialize( PersistenceManager pm )
		{
			this.pm = pm;
			return this;
		}
	}

	static class ConnectionExtension
	{
		public static string DisplayName(this IDbConnection conn)
		{
			if (conn == null)
				return "??";

			var str = conn.ConnectionString;
			if (String.IsNullOrEmpty( str ))
				return str;
			Regex regex = new Regex( "password=[^;]*" );
			return regex.Replace( str, "password=***" );
		}
	}
}
