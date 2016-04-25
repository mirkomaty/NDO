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
