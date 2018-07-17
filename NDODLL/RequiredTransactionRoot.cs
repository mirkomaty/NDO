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
using System.EnterpriseServices;
using NDO;

namespace NDO
{
	/// <summary>
	/// This is a transaction root for the .NET Enterprise Services, which requires a transaction. If there is no
	/// transaction running, a new transaction will be started by the Enterprise Services.
	/// </summary>
	/// <remarks>
	/// Call the Save and Abort functions of this class instead of the same functions of the PersistenceManager.
	/// The class is marked with the Attribute Transaction(TransactionOption.Required). 
	/// It constructs a PersistenceManager using the PersistenceManagerFactory. 
	/// The TransactionMode property is always set to TransactionMode.None, because we
	/// don't want to have local transactions mixed with distributed transactions.
	/// Please note, that the calling context never holds an instance of this class directly, 
	/// but a proxy. The real object behind the proxy will be constructed by COM+ dynamically
	/// and destroyed after a call to Save or Abort. Using the proxy after the Save or Abort calls
	/// leads to construction of a new TransactionRoot object with a new PersistenceManager by the Enterprise Services.
	/// Sample:
	/// <code>
	///	// The transaction root uses the PersistenceManagerFactory 
	///	// to construct PMs - so set any properties there;
	///	// for example: the VerboseMode should be set to true in the new pm.
	///	PersistenceManagerFactory.VerboseMode = true;
	///	RequiredTransactionRoot root = new RequiredTransactionRoot();
	///	PersistenceManager pm = root.PersistenceManager;
	///	
	///	Employee e = new Employee();
	///	e.FirstName = "Johnny";
	///	e.LastName = "Walker";
	///	pm.MakePersistent(e);
	///	root.Save(); // Ends the tx and dissolves the root
	///
	///	// A new root and pm will be created here.
	///	pm = root.PersistenceManager;
	///
	///	e = new Employee();
	///	e.FirstName = "Jack";
	///	e.LastName = "Daniels";
	///	pm.MakePersistent(e);
	///	root.Save();
	/// </code>
	/// <seealso cref="SupportedTransactionRoot"/>
	/// <seealso cref="RequiresNewTransactionRoot"/>
	/// </remarks>
	[Transaction(TransactionOption.Required)]
	public class RequiredTransactionRoot : ServicedComponent
	{
		PersistenceManager persistenceManager;

		/// <summary>
		/// Gets the PersistenceManager, which has been constructed by the TransactionRoot.
		/// </summary>
		public PersistenceManager PersistenceManager
		{
			get { return persistenceManager; }
			set { persistenceManager = value; }
		}


		/// <summary>
		/// Constructor. It constructs a PersistenceManager.
		/// </summary>
		public RequiredTransactionRoot()
		{
			persistenceManager = PersistenceManagerFactory.NewPersistenceManager();
			persistenceManager.TransactionMode = TransactionMode.None;
			if (persistenceManager.VerboseMode && persistenceManager.LogAdapter != null)
				persistenceManager.LogAdapter.Info("RequiredTransactionRoot: Constructing a new root");
		}

		/// <summary>
		/// Saves all chances and ends the transactions. Please note, that after calling save
		/// the instance of this class gets removed by the EnterpriseServices. 
		/// </summary>
		public void Save()
		{
			persistenceManager.Save();
			if (persistenceManager.VerboseMode && persistenceManager.LogAdapter != null)
				persistenceManager.LogAdapter.Info("RequiredTransactionRoot: SetComplete");
			ContextUtil.SetComplete();
		}


		/// <summary>
		/// Abort the transaction.
		/// </summary>
		/// <param name="rollbackObjects">Determines wheter all objects changes should be restored.</param>
		/// <remarks>
		/// Please note: During Abort() the PersistenceManager gets destroyed. 
		/// If there are any changes pending at the objects, they can't be stored. Reread the
		/// changed objects in a new transaction and apply the changes to the new object in order to store it.
		/// </remarks>
		public void Abort(bool rollbackObjects)
		{
			if (rollbackObjects)
				persistenceManager.Abort();
			if (persistenceManager.VerboseMode && persistenceManager.LogAdapter != null)
				persistenceManager.LogAdapter.Info("RequiredTransactionRoot: SetAbort");
			ContextUtil.SetAbort();
		}

	}
}
