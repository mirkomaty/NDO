using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;

namespace NDOInterfaces
{
	/// <summary>
	/// Wrapper class for IDbConnection to add additional information.
	/// </summary>
	/// <remarks>This class contains a reusable generation of ConnectionIds.</remarks>
	public class NdoDbConnection : DbConnection, INdoDbConnection
	{
		IDbConnection innerConnection;
		public IDbConnection InnerConnection => innerConnection;

		static int connectionId = 0;
		int myConnectionId = 0;
		public NdoDbConnection(IDbConnection innerConnection)
		{
			this.innerConnection = innerConnection;
		}

		///<inheritdoc/>
		public virtual object ConnectionId => myConnectionId;

		///<inheritdoc/>
		public override string ConnectionString { get => innerConnection.ConnectionString; set => innerConnection.ConnectionString = value; }
		///<inheritdoc/>
		public override int ConnectionTimeout => innerConnection.ConnectionTimeout;
		///<inheritdoc/>
		public override string Database => innerConnection.Database;
		///<inheritdoc/>
		public override ConnectionState State => innerConnection.State;

		public override string DataSource => throw new NotImplementedException();

		public override string ServerVersion => throw new NotImplementedException();

		///<inheritdoc/>
		IDbTransaction IDbConnection.BeginTransaction()
		{
			return innerConnection.BeginTransaction();
		}
		///<inheritdoc/>
		IDbTransaction IDbConnection.BeginTransaction( IsolationLevel il )
		{
			return innerConnection.BeginTransaction( il );
		}
		///<inheritdoc/>
		public override void ChangeDatabase( string databaseName )
		{
			innerConnection.ChangeDatabase( databaseName );
		}
		///<inheritdoc/>
		public override void Close()
		{
			innerConnection.Close();
			this.myConnectionId = 0;
		}
		///<inheritdoc/>
		IDbCommand IDbConnection.CreateCommand()
		{
			return innerConnection.CreateCommand();
		}
		///<inheritdoc/>
		void IDisposable.Dispose()
		{
			innerConnection.Dispose();
			this.myConnectionId = 0;
		}
		///<inheritdoc/>
		public override void Open()
		{
			innerConnection.Open();
			this.myConnectionId = Interlocked.Increment( ref connectionId );
		}

		protected override DbTransaction BeginDbTransaction( IsolationLevel isolationLevel )
		{
			return (DbTransaction)innerConnection.BeginTransaction( isolationLevel );
		}

		protected override DbCommand CreateDbCommand()
		{
			return (DbCommand) innerConnection.CreateCommand();
		}
	}
}
