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

/*
This Code should test the TransactionScopes. Actually the tests in this file are copy & paste from TransactionTests.
*/


#if nix

using NUnit.Framework;
using System;
using NDO;
using Reisekosten.Personal;
using System.Text.RegularExpressions;
using NDO.Query;
using Reisekosten;
using Formfakten.TestLogger;

namespace NdoUnitTests
{
	//[Ignore( "These tests must be adapted to the TransactionScope." )]
	[TestFixture]
	public class TransactionScopeTests : NDOTest
	{
		public void Setup() 
		{
			Logger.ClearTestLogs();
		}

		public void TearDown() 
		{
			Logger.ClearTestLogs();
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
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.None;
				new NDOQuery<Mitarbeiter>( pm ).Execute();
				var startCount = Logger.FindLogsWith("Starting transaction").Count;
				var commitCount = Logger.FindLogsWith("Committing transaction").Count;
				Assert.That( startCount == 0, "Transaction shouldn't be started" );
				Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
			}
		}


		[Test]
		public void TestIfOptimisticQueryIsImmediatelyCommitted()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Optimistic;
				new NDOQuery<Mitarbeiter>( pm ).Execute();
				var commitCount = Logger.FindLogsWith("Committing transaction").Count;
				Assert.That( commitCount > 0, "Transaction should be committed" );
			}
		}

		[Test]
		public void TestIfPessimisticQueryIsNotCommitted()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				new NDOQuery<Mitarbeiter>( pm ).Execute();
				var startCount = Logger.FindLogsWith("Starting transaction").Count;
				var commitCount = Logger.FindLogsWith("Committing transaction").Count;
				Assert.That( startCount > 0, "Transaction should be started" );
				Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
			}
		}

		[Test]
		public void TestIfOptimisticSaveIsCommitted()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Optimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save();

				var startCount = Logger.FindLogsWith("Starting transaction").Count;
				var commitCount = Logger.FindLogsWith("Committing transaction").Count;
				Assert.That( startCount > 0, "Transaction should be started" );
				Assert.That( commitCount > 0, "Transaction should be committed" );
			}
		}

		[Test]
		public void TestIfPessimisticSaveIsCommitted()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save();

				var startCount = Logger.FindLogsWith("Starting transaction").Count;
				var commitCount = Logger.FindLogsWith("Committing transaction").Count;
				Assert.That( startCount > 0, "Transaction should be started" );
				Assert.That( commitCount > 0, "Transaction should be committed" );
			}
		}

		[Test]
		public void OptimisticQueryAndSaveShouldHaveTwoTransactions()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Optimistic;
				new NDOQuery<Mitarbeiter>( pm ).Execute();
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save();

				var startCount = Logger.FindLogsWith("Starting transaction").Count;
				var commitCount = Logger.FindLogsWith("Committing transaction").Count;
				Assert.That( startCount == 2, "Two Transactions should be started" );
				Assert.That( commitCount == 2, "Two Transactions should be committed" );
			}
		}

		[Test]
		public void PessimisticQueryAndSaveShouldHaveOneTransaction()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				new NDOQuery<Mitarbeiter>( pm ).Execute();
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save();

				var startCount = Logger.FindLogsWith("Starting transaction").Count;
				var commitCount = Logger.FindLogsWith("Committing transaction").Count;
				Assert.That( startCount == 1, "One Transaction should be started" );
				Assert.That( commitCount == 1, "One Transaction should be committed" );
			}
		}

		[Test]
		public void OptimisticDeferredSaveShouldNotCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Optimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save( true );

                var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount > 0, "Transaction should be started" );
                Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
			}
		}

		[Test]
		public void PessimisticDeferredSaveShouldNotCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save( true );
                var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount > 0, "Transaction should be started" );
                Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
			}
		}

		[Test]
		public void OptimisticDeferredSaveAndQueryShouldNotCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Optimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save( true );
				new NDOQuery<Mitarbeiter>( pm ).Execute();

                var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount > 0, "Transaction should be started" );
                Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
			}
		}

		[Test]
		public void PessimisticDeferredSaveAndQueryShouldNotCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save( true );
				new NDOQuery<Mitarbeiter>( pm ).Execute();

                var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount > 0, "Transaction should be started" );
                Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
			}
		}

		[Test]
		public void OptimisticSaveAfterDeferredSaveShouldCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Optimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save( true );
				m.Nachname = "Test";
				pm.Save();

                var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount == 1, "Transaction should be started" );
                Assert.That( commitCount == 1, "Transaction should be committed" );				
            }
        }

		[Test]
		public void PessimisticSaveAfterDeferredSaveShouldCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				Mitarbeiter m = new Mitarbeiter();
				pm.MakePersistent( m );
				pm.Save( true );
				m.Nachname = "Test";
				pm.Save();

                var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount == 1, "Transaction should be started" );
                Assert.That( commitCount == 1, "Transaction should be committed" );
			}
		}


		[Test]
		public void DirectSqlPassThroughWithoutTransactionShouldNotCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.None;
				using (ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler())
				{
					var reader = sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
					Assert.That( reader == null, "Reader should be null" );
				}
            
				var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount == 0, "Transaction shouldn't be started" );
                Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
            }
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
			using (var pm = PmFactory.NewPersistenceManager())
			{
				var landCount = LänderCount( pm );
				var fhCount = FlughafenCount( pm );

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

				Assert.That( landCount == LänderCount( pm ) );
				Assert.That( fhCount == FlughafenCount( pm ) );
			}
		}

		[Test]
		public void DirectSqlPassThroughWithTransactionShouldCommit()
		{
			using (var pm = PmFactory.NewPersistenceManager())
			{
				using (ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler())
				{
					sqlHandler.BeginTransaction();
					sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
					sqlHandler.Execute( "DELETE FROM Reise" );
					sqlHandler.CommitTransaction();
				}
                var startCount = Logger.FindLogsWith("Starting transaction").Count;
                var commitCount = Logger.FindLogsWith("Committing transaction").Count;
                Assert.That( startCount == 1, "1 Transaction should be started" );
                Assert.That( commitCount == 1, "1 Transaction should be committed" );
			}
		}

		[Test]
		public void DirectSqlPassThroughWithCombinedStatementsDoesNotCommit()
		{
			var pm = PmFactory.NewPersistenceManager();

			using (ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler())
			{
				sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
				sqlHandler.Execute( "DELETE FROM Reise" );
			}

            var commitCount = Logger.FindLogsWith("Committing transaction").Count;
            Assert.That( commitCount == 0, "Transaction shouldn't be committed" );
		}

		[Test]
		public void DirectSqlPassWithAbortWorks()
		{
			var pm = PmFactory.NewPersistenceManager();
			var m = new Mitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			pm.UnloadCache();

			using (ISqlPassThroughHandler sqlHandler = pm.GetSqlPassThroughHandler())
			{
				sqlHandler.BeginTransaction();
				sqlHandler.Execute( "DELETE FROM Mitarbeiter" );
				pm.Abort();
			}

            pm = PmFactory.NewPersistenceManager();
            bool hasRecords = new NDOQuery<Mitarbeiter>( pm ).Execute().Count > 0;
			Assert.That( hasRecords, "At least one record should be present" );
		}
	}
}
#endif