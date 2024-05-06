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


using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDO;
using Reisekosten.Personal;
using System.Text.RegularExpressions;
using NDO.Query;
using Reisekosten;

namespace NdoUnitTests
{
	[TestFixture]
	public class TransactionTests
	{
		public void Setup() { }

		public void TearDown() 
		{
			var pm = PmFactory.NewPersistenceManager();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm );
			pm.Delete( q.Execute() );
		}


		/*
		forceCommit should be:
		 
					Query	Save	Save(true)
		Optimistic	1		1		0
		Pessimistic	0		1		0
			
		Deferred Mode			
					Query	Save	Save(true)
		Optimistic	0		1		0
		Pessimistic	0		1		0

		 */

		[Test]
		public void TestIfQueryWithoutTransactionDoesNotStartAndNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.None;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			new NDOQuery<Mitarbeiter>( pm ).Execute();
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should be committed" );
			Assert.That( log.IndexOf( "Starting transaction" ) == -1, "Transaction should be committed" );
		}


		[Test]
		public void TestIfOptimisticQueryIsImmediatelyCommitted()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Optimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			new NDOQuery<Mitarbeiter>( pm ).Execute();
			Assert.That( pm.LogAdapter.ToString().IndexOf( "Committing transaction" ) > -1, "Transaction should be committed" );
		}

		[Test]
		public void TestIfPessimisticQueryIsNotCommitted()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Pessimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			new NDOQuery<Mitarbeiter>( pm ).Execute();
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Starting transaction" ) > -1, "Transaction should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction shouldn't be committed" );
			pm.Abort();
		}

		[Test]
		public void TestIfOptimisticSaveIsCommitted()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Optimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Starting transaction" ) > -1, "Transaction should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) > -1, "Transaction should be committed" );
		}

		[Test]
		public void TestIfPessimisticSaveIsCommitted()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Pessimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Starting transaction" ) > -1, "Transaction should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) > -1, "Transaction should be committed" );
		}

		[Test]
		public void OptimisticQueryAndSaveShouldHaveTwoTransactions()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Optimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			new NDOQuery<Mitarbeiter>( pm ).Execute();
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			string log = pm.LogAdapter.ToString();
			Assert.That( new Regex( "Starting transaction" ).Matches(log).Count == 2, "Two Transactions should be started" );
			Assert.That( new Regex( "Committing transaction" ).Matches(log).Count == 2, "Two Transactions should be committed" );
		}

		[Test]
		public void PessimisticQueryAndSaveShouldHaveOneTransaction()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Pessimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			new NDOQuery<Mitarbeiter>( pm ).Execute();
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			string log = pm.LogAdapter.ToString();
			Console.WriteLine( log );
			Assert.That( new Regex( "Starting transaction" ).Matches( log ).Count == 1, "One Transactions should be started" );
			Assert.That( new Regex( "Committing transaction" ).Matches( log ).Count == 1, "One Transactions should be committed" );
		}

		[Test]
		public void OptimisticDeferredSaveShouldNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Optimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save(true);
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Starting transaction" ) > -1, "Transaction should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should be committed" );
			pm.Abort();
		}

		[Test]
		public void PessimisticDeferredSaveShouldNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Pessimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save(true);
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Starting transaction" ) > -1, "Transaction should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should be committed" );
			pm.Abort();
		}

		[Test]
		public void OptimisticDeferredSaveAndQueryShouldNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Optimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save( true );
			new NDOQuery<Mitarbeiter>( pm ).Execute();
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Starting transaction" ) > -1, "Transaction should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should not be committed" );
			pm.Abort();
		}

		[Test]
		public void PessimisticDeferredSaveAndQueryShouldNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Pessimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save( true );
			new NDOQuery<Mitarbeiter>( pm ).Execute();
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Starting transaction" ) > -1, "Transaction should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should not be committed" );
			pm.Abort();
		}

		[Test]
		public void OptimisticSaveAfterDeferredSaveShouldCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Optimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save( true );
			m.Nachname = "Test";
			pm.Save();
			string log = pm.LogAdapter.ToString();
			Assert.That( new Regex( "Starting transaction" ).Matches( log ).Count == 1, "One Transactions should be started" );
			Assert.That( new Regex( "Committing transaction" ).Matches( log ).Count == 1, "One Transactions should be committed" );
			pm.Abort();
		}

		[Test]
		public void PessimisticSaveAfterDeferredSaveShouldCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.Pessimistic;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			Mitarbeiter m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save( true );
			m.Nachname = "Test";
			pm.Save();
			string log = pm.LogAdapter.ToString();
			Assert.That( new Regex( "Starting transaction" ).Matches( log ).Count == 1, "One Transactions should be started" );
			Assert.That( new Regex( "Committing transaction" ).Matches( log ).Count == 1, "One Transactions should be committed" );
			pm.Abort();
		}


		[Test]
		public void DirectSqlPassThroughWithoutTransactionShouldNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.None;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler();
			var reader = sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should be committed" );
			Assert.That( log.IndexOf( "Starting transaction" ) == -1, "Transaction should be committed" );
			Assert.That(reader == null, "Reader should be null" );
		}

		int FlughafenCount(PersistenceManager pm)
		{
			NDOQuery<Flughafen> q = new NDOQuery<Flughafen>( pm );
			return (int)(decimal)q.ExecuteAggregate( "oid", AggregateType.Count );
		}

		int LänderCount( PersistenceManager pm )
		{
			NDOQuery<Land> q = new NDOQuery<Land>( pm );
			return (int)(decimal)q.ExecuteAggregate( "oid", AggregateType.Count );
		}

		[Test]
		public void AbortedDeferredMultiStepTransactionDoesNotCommit()
		{
			PersistenceManager pm = new PersistenceManager();
			var landCount = LänderCount( pm );
			var fhCount = FlughafenCount( pm );

			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			pm.TransactionMode = TransactionMode.Optimistic;

			Land land = new Land();
			land.Name = "Germany";
			pm.MakePersistent( land );
			pm.Save( true );

			var flughafen = new Flughafen();
			flughafen.Kürzel = "MUC";
			pm.MakePersistent( flughafen );
			land.AddFlughafen( flughafen );
			pm.Save( true );

			pm.Abort();

			string log = pm.LogAdapter.ToString();
			Assert.That( new Regex( "Starting transaction" ).Matches( log ).Count == 1, "One Transactions should be started" );
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should be committed" );
			Assert.That( new Regex( "Rollback transaction" ).Matches( log ).Count == 1, "One Transactions should be rolled back" );

			pm.TransactionMode = TransactionMode.None;
			Assert.That(landCount ==  LänderCount( pm ) );
			Assert.That(fhCount ==  FlughafenCount( pm ) );
		}

		[Test]
		public void DirectSqlPassThroughWithTransactionShouldCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler();
			sqlHandler.BeginTransaction();
			sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
			sqlHandler.Execute( "DELETE FROM Reise" );
			sqlHandler.CommitTransaction();
			string log = pm.LogAdapter.ToString();
			Assert.That( new Regex( "Starting transaction" ).Matches( log ).Count == 1, "One Transactions should be started" );
			Assert.That( new Regex( "Committing transaction" ).Matches( log ).Count == 1, "One Transactions should be committed" );
		}

		[Test]
		public void DirectSqlPassThroughWithCombinedStatementsDoesNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.None;
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			using (ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler())
			{
				sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
				sqlHandler.Execute( "DELETE FROM Reise" );
			}
			string log = pm.LogAdapter.ToString();
			Assert.That( log.IndexOf( "Committing transaction" ) == -1, "Transaction should not be started" );
			Assert.That( log.IndexOf( "Starting transaction" ) == -1, "Transaction should not be committed" );
		}

		[Test]
		public void DirectSqlPassWithAbortWorks()
		{
			var pm = PmFactory.NewPersistenceManager();
			var m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			pm.UnloadCache();
			pm.LogAdapter = new TestLogAdapter();
			pm.VerboseMode = true;
			using (ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler())
			{
				sqlHandler.BeginTransaction();
				sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
				pm.Abort();
			}
			bool hasRecords = new NDOQuery<Mitarbeiter>( pm ).Execute().Count > 0;
			Assert.That( hasRecords, "At least one record should be present" );
		}
	}
}
