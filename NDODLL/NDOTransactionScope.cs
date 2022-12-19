using NDO.Logging;
using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace NDO
{
	/// <summary>
	/// 
	/// </summary>
	public class NDOTransactionScope : INDOTransactionScope
	{
		private Dictionary<string, DbConnection> usedConnections = new Dictionary<string, DbConnection>();
		private Dictionary<string, DbTransaction> usedTransactions = new Dictionary<string, DbTransaction>();

		///<inheritdoc/>
		public IsolationLevel IsolationLevel { get; set; }
		///<inheritdoc/>
		public TransactionMode TransactionMode { get; set; }

		bool isInTransaction;
		private readonly ILogAdapter logger;

		/// <summary>
		/// Constructs an NDOTransactionScope object.
		/// </summary>
		/// <param name="logger"></param>
		public NDOTransactionScope( ILogAdapter logger )
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

		private void OpenConnAndStartTransaction( string connId, DbConnection conn )
		{
			conn.Open();
			this.logger.Debug( $"+ Opening connection {conn.DisplayName()}" );
			var tx = conn.BeginTransaction(IsolationLevel);
			usedTransactions.Add( connId, tx );
			this.logger.Debug( $"+ Starting transaction {tx.GetHashCode():X} at connection '{conn.DisplayName()}'" );
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

				DbConnection conn = null;
				usedConnections.TryGetValue( id, out conn );
				this.logger.Debug( $"- Committing transaction {tx.GetHashCode():X} at connection '{conn.DisplayName()}'" );
			}

			usedTransactions.Clear();
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
				if (this.isInTransaction)
					OpenConnAndStartTransaction( id, conn );
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
				DbConnection conn = null;
				this.usedConnections.TryGetValue( key, out conn );
				this.logger.Debug( $"- Rollback transaction {id.ToString( "X" )} at connection '{conn.DisplayName()}'" );
			}

			usedTransactions.Clear();
		}

		private void CloseConnections()
		{
			foreach (var conn in this.usedConnections.Values.Where( c => c.State == ConnectionState.Open ))
			{
				try
				{
					conn.Close();
				}
				catch (Exception ex)
				{
					this.logger.Error( ex.ToString() );
					throw;
				}

				this.logger.Debug( $"- Closed connection {conn.DisplayName()}" );
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
