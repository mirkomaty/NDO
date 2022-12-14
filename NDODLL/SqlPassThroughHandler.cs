﻿//
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
using System.Linq;
using System.Text;
using NDOInterfaces;
using NDO.Mapping;
using System.Data;
using System.Data.Common;

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
			this.pm.TransactionScope.CheckTransaction();
		}

		public void CommitTransaction()
		{
			this.pm.TransactionScope.Complete();
			if (this.forcedTransactionMode)
				this.pm.TransactionMode = this.oldTransactionMode;
			this.forcedTransactionMode = false;
		}

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
		public IDataReader Execute( string command, bool returnReader = false, params object[] parameters )
		{
			this.pm.TransactionScope.CheckTransaction();
			if (this.pm.VerboseMode && this.pm.LogAdapter != null)
				this.pm.LogAdapter.Info( command );

			IProvider provider = this.pm.NDOMapping.GetProvider( this.connection );

			var dbConnection = this.pm.TransactionScope.GetConnection(this.connection.ID, () => (DbConnection)provider.NewConnection(this.connection.Name) );

			IDbCommand cmd = provider.NewSqlCommand( dbConnection );
			cmd.CommandText = command;
			var tx = this.pm.TransactionScope.GetTransaction(this.connection.ID);
			if (tx != null)
				cmd.Transaction = tx;

			int pcount = 0;
			foreach (var par in parameters)
			{
				var dbpar = cmd.CreateParameter();
				dbpar.ParameterName = $"@p{pcount++}";
				dbpar.Value = par ?? DBNull.Value;
				cmd.Parameters.Add( dbpar );
			}

			if (dbConnection.State == ConnectionState.Closed)
				dbConnection.Open();

			if (returnReader)
				return cmd.ExecuteReader();

			cmd.ExecuteNonQuery();
			return null;
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
			this.pm.TransactionScope.Dispose();
			if (this.forcedTransactionMode)
			{
				this.pm.TransactionMode = this.oldTransactionMode;
				this.forcedTransactionMode = false;
			}
		}
	}
}
