using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace NDO
{
	/// <summary>
	/// 
	/// </summary>
	public class NDOTransactionScope : INDOTransactionScope
	{
		private readonly PersistenceManager pm;

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
		/// <param name="pm"></param>
		public NDOTransactionScope( PersistenceManager pm )
		{
			IsolationLevel = IsolationLevel.ReadCommitted;
			TransactionMode = TransactionMode.Optimistic;
			this.pm = pm;
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
			pm.LogIfVerbose( $"Opening connection {conn.DisplayName()}" );
			var tx = conn.BeginTransaction(IsolationLevel);
			usedTransactions.Add( connId, tx );
			this.pm.LogIfVerbose( String.Format( "Starting transaction {0:X} at connection '{1}'", tx.GetHashCode(), conn.DisplayName() ) );
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
				this.pm.LogIfVerbose( String.Format( "Committing transaction {0:X} at connection '{1}'", tx.GetHashCode(), conn.DisplayName() ) );
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
				tx.Rollback();
				IDbConnection conn = null;
				this.usedConnections.TryGetValue( key, out conn );
				pm.LogIfVerbose( $"Rollback transaction {id.ToString( "X" )} at connection '{conn.DisplayName()}'" );
			}

			usedTransactions.Clear();
		}

		private void CloseConnections()
		{
			foreach (var conn in this.usedConnections.Values.Where( c => c.State == ConnectionState.Open ))
			{
				conn.Close();
				pm.LogIfVerbose( $"Closed connection {conn.DisplayName()}" );
			}

			this.usedConnections.Clear();
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
