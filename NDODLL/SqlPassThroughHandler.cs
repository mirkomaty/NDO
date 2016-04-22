using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOInterfaces;
using NDO.Mapping;
using System.Data;

namespace NDO
{
	internal class SqlPassThroughHandler : ISqlPassThroughHandler
	{
		PersistenceManager pm;
		Connection connection;
		TransactionMode oldTransactionMode;
		bool forcedTransactionMode = false;

		public SqlPassThroughHandler(PersistenceManager pm, Connection connection)
		{
			this.pm = pm;
			this.connection = connection;
		}

		public void BeginTransaction()
		{
			this.forcedTransactionMode = true;
			this.oldTransactionMode = this.pm.TransactionMode;
			this.pm.TransactionMode = TransactionMode.Pessimistic;
			this.pm.CheckTransaction( null, this.connection );
		}

		public void CommitTransaction()
		{
			this.pm.CheckEndTransaction( true );
		}

		public IDataReader Execute( string command )
		{
			if (this.pm.VerboseMode && this.pm.LogAdapter != null)
				this.pm.LogAdapter.Info( command );

			IProvider provider = this.pm.NDOMapping.GetProvider( this.connection );
			TransactionInfo ti = this.pm.GetTransactionInfo( this.connection );
			IDbConnection dbConnection = ti.Connection;
			if (dbConnection.State == ConnectionState.Closed)
				dbConnection.Open();
			IDbCommand cmd = provider.NewSqlCommand( dbConnection );
			cmd.CommandText = command;
			cmd.Transaction = ti.Transaction;

			return cmd.ExecuteReader();
		}

		public IProvider Provider
		{
			get
			{
				return this.pm.NDOMapping.GetProvider( this.connection );
			}
		}

		public void Dispose()
		{
			if (this.forcedTransactionMode)
				this.pm.TransactionMode = this.oldTransactionMode;
		}
	}
}
