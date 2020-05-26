using System;
using System.Transactions;

namespace NDO
{
	class NDOTransactionScope : IDisposable
	{
		public IsolationLevel IsolationLevel { get; set; }
		public TransactionMode TransactionMode { get; set; }

		public NDOTransactionScope()
		{
			IsolationLevel = IsolationLevel.ReadCommitted;
			TransactionMode = TransactionMode.Optimistic;
		}

		TransactionScope innerScope;

		public void CheckTransaction()
		{
			if (TransactionMode == TransactionMode.None)
			{
				innerScope = null;
				return;
			}

			if (innerScope == null)
				innerScope = new TransactionScope( TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = this.IsolationLevel } );
		}

		public void Complete()
		{
			if (innerScope != null)
			{
				innerScope.Complete();
				innerScope.Dispose();
			}

			innerScope = null;
		}

		public void Dispose()
		{
			if (innerScope != null)
			{
				innerScope.Dispose();
				innerScope = null;
			}
		}
	}
}
