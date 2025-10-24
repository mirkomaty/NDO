using NDO.Mapping;
using System;
using System.Data;

namespace NDO
{
	/// <summary>
	/// Interface for differerent implementations of NDOTransactionScope
	/// </summary>
	public interface INDOTransactionScope : IDisposable
	{
		/// <summary>
		/// This is called from NDO to provide the current pm to the transaction scope.
		/// </summary>
		/// <param name="pm"></param>
		/// <returns></returns>
		INDOTransactionScope Initialize( PersistenceManager pm );
		/// <summary>
		/// Sets the IsolationLevel of transactions started by the scope object
		/// </summary>
		IsolationLevel IsolationLevel { get; set; }

		/// <summary>
		/// Sets the Transaction Mode in which the PersistenceManager should run.
		/// </summary>
		TransactionMode TransactionMode { get; set; }

		/// <summary>
		/// Starts a transaction, if necessary
		/// </summary>
		void CheckTransaction();

		/// <summary>
		/// Commits a transaction, if necessary
		/// </summary>
		void Complete();

		/// <summary>
		/// Gets a connection from the cache, or creates one using the factory and puts it on the cache.
		/// </summary>
		/// <param name="ndoConnection">The NDO.Mapping.Connection object for which the IDbConnection is created.</param>
		/// <param name="factory">A lambda expression, which can create an IDbConnection object.</param>
		/// <returns></returns>
		IDbConnection GetConnection( Connection ndoConnection, Func<IDbConnection> factory );

		/// <summary>
		/// Gets a transaction, if one exists for the given connection id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		IDbTransaction GetTransaction( string id );
	}
}