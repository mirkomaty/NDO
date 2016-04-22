using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NDOInterfaces;

namespace NDO
{
	/// <summary>
	/// Interface to a class which is able to pass through sql statements to 
	/// a given NDO Connection.
	/// </summary>
	public interface ISqlPassThroughHandler : IDisposable
	{
		/// <summary>
		/// Executes the given command.
		/// </summary>
		/// <param name="command">A SQL command string</param>
		/// <returns>A DataReader object which may be empty.</returns>
		/// <remarks>The command string must be formatted for the given database</remarks>
		IDataReader Execute( string command );

		/// <summary>
		/// Returns the NDO Provider for the Database, which is configured in the given NDO Connection
		/// </summary>
		IProvider Provider { get; }

		/// <summary>
		/// Starts an ADO.NET transaction.
		/// </summary>
		/// <remarks>This sets a temporary pessimistic TransactionMode. The TransactionMode will be reverted to the old mode after commit or Dispose(). The transaction will be commited, if pm.Save() is called.</remarks>
		void BeginTransaction();

		/// <summary>
		/// Commits an ADO.NET transaction which has been started by BeginTransaction or a pessimistic PersistenceManager transaction.
		/// </summary>
		void CommitTransaction();
	}
}
