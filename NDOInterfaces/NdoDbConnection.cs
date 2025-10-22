using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace NDOInterfaces
{
	/// <summary>
	/// Wrapper class for IDbConnection to add additional information.
	/// </summary>
	/// <remarks>This class contains a reusable generation of ConnectionIds.</remarks>
	public class NdoDbConnection : INdoDbConnection
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
		public string ConnectionString { get => innerConnection.ConnectionString; set => innerConnection.ConnectionString = value; }
		///<inheritdoc/>
		public int ConnectionTimeout => innerConnection.ConnectionTimeout;
		///<inheritdoc/>
		public string Database => innerConnection.Database;
		///<inheritdoc/>
		public ConnectionState State => innerConnection.State;
		///<inheritdoc/>
		public IDbTransaction BeginTransaction()
		{
			return innerConnection.BeginTransaction();
		}
		///<inheritdoc/>
		public IDbTransaction BeginTransaction( IsolationLevel il )
		{
			return innerConnection.BeginTransaction( il );
		}
		///<inheritdoc/>
		public void ChangeDatabase( string databaseName )
		{
			innerConnection.ChangeDatabase( databaseName );
		}
		///<inheritdoc/>
		public void Close()
		{
			innerConnection.Close();
			this.myConnectionId = 0;
		}
		///<inheritdoc/>
		public IDbCommand CreateCommand()
		{
			return innerConnection.CreateCommand();
		}
		///<inheritdoc/>
		public void Dispose()
		{
			innerConnection.Dispose();
			this.myConnectionId = 0;
		}
		///<inheritdoc/>
		public void Open()
		{
			innerConnection.Open();
			this.myConnectionId = Interlocked.Increment( ref connectionId );
		}
	}
}
