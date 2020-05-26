using System;
using System.Transactions;

namespace NDO
{
	class NDOTransactionScope : IDisposable
	{
		TransactionScope innerScope;
		private readonly PersistenceManager pm;

		public IsolationLevel IsolationLevel { get; set; }
		public TransactionMode TransactionMode { get; set; }

		public NDOTransactionScope(PersistenceManager pm)
		{
			IsolationLevel = IsolationLevel.ReadCommitted;
			TransactionMode = TransactionMode.Optimistic;
			this.pm = pm;
		}

		public void CheckTransaction()
		{
			if (TransactionMode == TransactionMode.None)
			{
				innerScope = null;
				return;
			}

			if (innerScope == null)
			{
				innerScope = new TransactionScope( TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = this.IsolationLevel } );
				this.pm.LogIfVerbose( "Creating a new TransactionScope" );
			}
		}

		public void Complete()
		{
			if (innerScope != null)
			{
				this.pm.LogIfVerbose( "Completing the TransactionScope" );
				innerScope.Complete();
				innerScope.Dispose();
			}

			innerScope = null;
		}

		public void Dispose()
		{
			if (innerScope != null)
			{
				this.pm.LogIfVerbose( "Disposing the TransactionScope" );
				innerScope.Dispose();
				innerScope = null;
			}
		}
	}
}
