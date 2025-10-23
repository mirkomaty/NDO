using NDO.Mapping;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace NDO
{
	/// <summary>
	/// class NDOTransactionScope
	/// </summary>
	public class NDOTransactionScope : INDOTransactionScope
	{
		private PersistenceManager pm;

		private Dictionary<string, IDbConnection> usedConnections = new Dictionary<string, IDbConnection>();
		private Dictionary<string, IDbTransaction> usedTransactions = new Dictionary<string, IDbTransaction>();

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
		public void CheckTransaction()
		{
			if (TransactionMode == TransactionMode.None)
			{
				return;
			}

			if (!this.isInTransaction)
			{
				foreach (var connId in this.usedConnections.Keys)
				{
					OpenConnAndStartTransaction( connId, this.usedConnections[connId] );
				}
			}

			this.isInTransaction = true;
		}

		private void OpenConnAndStartTransaction( string connId, IDbConnection conn )
		{
			conn.Open();
			var serverId = ((INdoDbConnection)conn).ConnectionId;
			pm.LogIfVerbose( $"Opening connection {serverId} = '{conn.DisplayName()}'" );
			var tx = conn.BeginTransaction(IsolationLevel);
			usedTransactions.Add( connId, tx );
			this.pm.LogIfVerbose( $"Starting transaction {tx.GetHashCode():X} at connection {serverId} = '{conn.DisplayName()}'" );
		}

		///<inheritdoc/>
		public void Complete()
		{
			CommitTransactions();
			CloseConnections();
			isInTransaction = false;
		}

		private void CommitTransactions()
		{
			foreach (var id in usedTransactions.Keys)
			{
				var tx = usedTransactions[id];
				tx.Commit();

				IDbConnection conn = null;
				usedConnections.TryGetValue( id, out conn );
				if (conn == null)
					throw new NDOException( 121, $"Can't commit. No open connection found for NDO Connection {id} ({conn.DisplayName()})" );
				var serverId = ((INdoDbConnection)conn).ConnectionId;
				this.pm.LogIfVerbose( $"Committing transaction {tx.GetHashCode():X} at connection {serverId} = '{conn.DisplayName()}'" );
			}

			usedTransactions.Clear();
		}

		///<inheritdoc/>
		public IDbConnection GetConnection( string id, Func<IDbConnection> factory )
		{
			if (this.usedConnections.ContainsKey( id ))
			{
				return this.usedConnections[id];
			}
			else
			{
				var conn = factory();
				this.usedConnections.Add( id, conn );
				if (this.isInTransaction)
					OpenConnAndStartTransaction( id, conn );
				return conn;
			}
		}


		///<inheritdoc/>
		public IDbTransaction GetTransaction( string id )
		{
			if (isInTransaction)
			{
				IDbTransaction tx = null;
				this.usedTransactions.TryGetValue( id, out tx );
				return tx;
			}

			return null;
		}

		///<inheritdoc/>
		public void Dispose()
		{
			RollbackTransactions();
			CloseConnections();
			this.isInTransaction = false;
		}

		private void RollbackTransactions()
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
					tx.Rollback();
				}
				catch
				{
				}
				IDbConnection conn = null;
				this.usedConnections.TryGetValue( key, out conn );
				{
					var serverId = ((INdoDbConnection)conn).ConnectionId;
					this.pm.LogIfVerbose( $"Rollback transaction {id.ToString( "X" )} at connection {serverId} = '{conn.DisplayName()}'" );
				}
			}

			usedTransactions.Clear();
		}

		private void CloseConnections()
		{
			foreach (var conn in this.usedConnections.Values.Where( c => c.State == ConnectionState.Open ))
			{
				var serverId = ((INdoDbConnection)conn).ConnectionId;
				pm.LogIfVerbose( $"Closed connection {serverId} = '{conn.DisplayName()}'" );
				conn.Close();
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
