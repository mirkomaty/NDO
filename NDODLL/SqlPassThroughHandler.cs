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

		public IDataReader Execute( string command, bool returnReader = false )
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
			if (this.forcedTransactionMode)
				this.pm.TransactionMode = this.oldTransactionMode;
		}
	}
}
