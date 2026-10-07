//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NDOInterfaces;

namespace NDO
{
	/// <summary>
	/// Interface to a class which is able to pass through sql statements to 
	/// a given NDO Connection.
	/// </summary>
	public interface ISqlPassThroughHandler : IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// Executes the given command.
		/// </summary>
		/// <param name="command">A SQL command string</param>
		/// <param name="returnReader">Determines, if the command should return a reader</param>
		/// <param name="parameters">Optional command parameters</param>
		/// <returns>A DataReader object which may be empty.</returns>
		/// <remarks>
		/// The command string must be formatted for the given database. 
		/// The names of the parameters in the query must have the names @px, 
		/// where x is the index of the parameter in the parameters array.
		/// </remarks>
		IDataReader Execute( string command, bool returnReader = false, params object[] parameters );

		/// <summary>
		/// Executes the given command asynchronously.
		/// </summary>
		/// <param name="command">A SQL command string</param>
		/// <param name="returnReader">Determines, if the command should return a reader</param>
		/// <param name="parameters">Optional command parameters</param>
		/// <param name="cancellationToken">A token to cancel the operation</param>
		/// <returns>A DataReader object which may be empty, or null, if returnReader is false.</returns>
		/// <remarks>
		/// The command string must be formatted for the given database. 
		/// The names of the parameters in the query must have the names @px, 
		/// where x is the index of the parameter in the parameters array.
		/// </remarks>
		Task<DbDataReader> ExecuteAsync( string command, bool returnReader, object[] parameters, CancellationToken cancellationToken = default );

		/// <summary>
		/// Executes the given command asynchronously.
		/// </summary>
		/// <param name="command">A SQL command string</param>
		/// <param name="returnReader">Determines, if the command should return a reader</param>
		/// <param name="parameters">Optional command parameters</param>
		/// <returns>A DataReader object which may be empty, or null, if returnReader is false.</returns>
		/// <remarks>
		/// This overload doesn't support a CancellationToken, because params arguments must be the last parameters.
		/// Use the overload with the object[] parameter to provide a CancellationToken.
		/// </remarks>
		Task<DbDataReader> ExecuteAsync( string command, bool returnReader = false, params object[] parameters );

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
		/// Starts an ADO.NET transaction asynchronously.
		/// </summary>
		/// <param name="cancellationToken">A token to cancel the operation</param>
		/// <remarks>This sets a temporary pessimistic TransactionMode. The TransactionMode will be reverted to the old mode after commit or Dispose(). The transaction will be commited, if pm.Save() is called.</remarks>
		Task BeginTransactionAsync( CancellationToken cancellationToken = default );

		/// <summary>
		/// Commits an ADO.NET transaction which has been started by BeginTransaction or a pessimistic PersistenceManager transaction.
		/// </summary>
		void CommitTransaction();

		/// <summary>
		/// Commits an ADO.NET transaction asynchronously, which has been started by BeginTransaction or a pessimistic PersistenceManager transaction.
		/// </summary>
		/// <param name="cancellationToken">A token to cancel the operation</param>
		Task CommitTransactionAsync( CancellationToken cancellationToken = default );
	}
}
