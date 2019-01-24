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
using System.Collections;
using NUnit.Framework;
using NDO;
using Reisekosten;
using Reisekosten.Personal;

namespace NdoUnitTests
{
	[TestFixture]
	public class NDOReiseTests
	{

		public NDOReiseTests()
		{
		}

		private PersistenceManager pm;
		private Reise r;
		private Mitarbeiter m;

		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			r = CreateReise( "ADC" );
			m = CreateMitarbeiter();
		}

		[TearDown]
		public void TearDown()
		{
			try
			{
				pm.Abort();
				IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), false );
				//				Console.WriteLine("TearDown löscht " + mitarbeiterListe.Count + " Mitarbeiter");
				pm.Delete( mitarbeiterListe );
				pm.Save();
				IList reiseListe = pm.GetClassExtent( typeof( Reise ), false );
				//				Console.WriteLine("TearDown löscht " + reiseListe.Count + " Reisen");
				pm.Delete( reiseListe );
				pm.Save();
				pm.Close();
				pm = null;
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine( "Exception in TearDown: " + ex );
			}
		}

		[Test]
		public void TestEmptyTransactionSave()
		{
			pm.Save();
		}

		[Test]
		public void TestEmptyTransactionAbort()
		{
			pm.Abort();
		}

		[Test]
		public void EmptyDB()
		{
			IList reiseListe = pm.GetClassExtent( typeof( Reise ), true );
			pm.Delete( reiseListe );
			pm.Save();
			IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
			pm.Delete( mitarbeiterListe );
			pm.Save();
		}

		[Test]
		public void TestObjectCreation()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			Assert.NotNull( r.NDOObjectId, "ObjectId should be valid" );
			Assert.AreEqual( NDOObjectState.Created, r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestObjectCreationSave()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			Assert.AreEqual( NDOObjectState.Persistent, r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestObjectCreationSaveChanged()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			r.Zweck = "Test";
			pm.Save();
			Assert.AreEqual( "Test", r.Zweck, "Zweck wrong" );
		}

		[Test]
		public void TestObjectCreationAbort()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Abort();
			Assert.Null( r.NDOObjectId, "Transient object shouldn't have ID" );
			Assert.Null( ((IPersistenceCapable)r).NDOStateManager, "Transient object shouldn't have state manager" );
			Assert.AreEqual( NDOObjectState.Transient, r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestObjectCreationAbortChanged()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			r.Zweck = "Möller";
			pm.Abort();
			Assert.AreEqual( "ADC", r.Zweck, "Nachname wrong" );
		}

		[Test]
		public void TestObjectId()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			ObjectId id = r.NDOObjectId;
			ObjectId id2 = new ObjectId( id );
			Assert.AreEqual( r.NDOObjectId, id, "IDs should be equal" );
			Assert.AreEqual( r.NDOObjectId, id2, "IDs should be equal" );
			if (!pm.HasOwnerCreatedIds && r.NDOObjectId.Id[0] is Int32)
				Assert.That( (int)id.Id[0] < 0, "Negative key in DS" );
			pm.Save();
			Assert.AreEqual( r.NDOObjectId, id, "IDs should be equal" );
			Assert.AreEqual( r.NDOObjectId, id2, "IDs should be equal" );
			if (!pm.HasOwnerCreatedIds && r.NDOObjectId.Id[0] is Int32)
				Assert.That( (int)id.Id[0] > 0, "Positive key in DB" );
		}

		[Test]
		public void TestCreateDeleteTransitionSave()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Delete( m );
			Assert.Null( r.NDOObjectId, "Transient object shouldn't have ID" );
			Assert.Null( ((IPersistenceCapable)r).NDOStateManager, "Transient object shouldn't have state manager" );
			Assert.AreEqual( NDOObjectState.Transient, r.NDOObjectState, "Status wrong" );
			pm.Save();
			Assert.Null( r.NDOObjectId, "Transient object shouldn't have ID" );
			Assert.Null( ((IPersistenceCapable)r).NDOStateManager, "Transient object shouldn't have state manager" );
			Assert.AreEqual( NDOObjectState.Transient, r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestCreateDeleteTransitionAbort()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			ObjectId id = m.NDOObjectId;
			pm.Delete( m );
			pm.Abort();
			Assert.Null( r.NDOObjectId, "Transient object shouldn't have ID" );
			Assert.Null( ((IPersistenceCapable)r).NDOStateManager, "Transient object shouldn't have state manager" );
			Assert.AreEqual( NDOObjectState.Transient, r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestCleanupCache()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.CleanupCache();
			Assert.AreSame( r, pm.FindObject( r.NDOObjectId ), "Getting same object twice should return same object" );
			pm.Save();
			pm.CleanupCache();
			Assert.AreSame( r, pm.FindObject( r.NDOObjectId ), "Getting same object twice should return same object" );
			ObjectId id = r.NDOObjectId;
			r = null;
			pm.CleanupCache();
			Assert.NotNull( pm.FindObject( id ), "Should find object" );
		}

		[Test]
		public void TestUnloadCache()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.UnloadCache();
			Assert.AreSame( r, pm.FindObject( r.NDOObjectId ), "Getting same object twice should return same object" );
			pm.Save();
			pm.UnloadCache();
			Assert.That( r != pm.FindObject( r.NDOObjectId ), "Getting same object twice should return different objects" );
			ObjectId id = r.NDOObjectId;
			r = null;
			pm.UnloadCache();
			Assert.NotNull( pm.FindObject( id ), "Should find object" );
		}

		[Test]
		public void TestClassExtent()
		{
			ArrayList rliste = new ArrayList();
			for (int i = 0; i < 100; i++)
			{
				Reise mr = CreateReise( "" + i );
				m.Hinzufuegen( mr );
				rliste.Add( mr );
			}
			pm.MakePersistent( m );

			IList ReiseListe = pm.GetClassExtent( typeof( Reise ) );
			Assert.AreEqual( 0, ReiseListe.Count, "Current extent should be empty" );
			pm.Save();

			ReiseListe = pm.GetClassExtent( typeof( Reise ) );
			Assert.AreEqual( 100, ReiseListe.Count, "Number of read objects is wrong" );
			// Check that all objects come from cache... 
			foreach (Reise m1 in ReiseListe)
			{
				Assert.AreEqual( NDOObjectState.Persistent, m1.NDOObjectState, "Wrong state" );
			}

			rliste.Clear();
			rliste = null;
			ReiseListe.Clear();
			ReiseListe = null;
			//pm.CleanupCache();
			pm.UnloadCache();
			ReiseListe = pm.GetClassExtent( typeof( Reise ) );
			Assert.AreEqual( 100, ReiseListe.Count, "Number of read objects is wrong" );
			// Check that all objects are reloaded 
			foreach (Reise m1 in ReiseListe)
			{
				Assert.AreEqual( NDOObjectState.Hollow, m1.NDOObjectState, "Wrong state" );
			}
		}

		[Test]
		public void TestDeleteHollow()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.UnloadCache();
			r = (Reise)pm.FindObject( r.NDOObjectId );
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #1" );
			pm.Delete( r );
			Assert.AreEqual( NDOObjectState.Deleted, r.NDOObjectState, "Wrong state #2" );
			pm.Abort();
			Assert.AreEqual( NDOObjectState.Persistent, r.NDOObjectState, "Wrong state #3" );
			pm.UnloadCache();
			r = (Reise)pm.FindObject( r.NDOObjectId );
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #4" );
			pm.Delete( r );
			pm.Save();
			Assert.AreEqual( NDOObjectState.Transient, r.NDOObjectState, "Wrong state #5" );

			IList l = pm.GetClassExtent( typeof( Reise ) );
			Assert.AreEqual( 0, l.Count, "Number of read objects is wrong" );
		}

		[Test]
		public void TestDeletePersistent()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.Delete( r );
			Assert.AreEqual( NDOObjectState.Deleted, r.NDOObjectState, "Wrong state #1" );
			IList l = pm.GetClassExtent( typeof( Reise ) );
			Assert.AreEqual( 1, l.Count, "Number of read objects is wrong" );
			pm.Save();
			l = pm.GetClassExtent( typeof( Reise ) );
			Assert.AreEqual( 0, l.Count, "Number of read objects is wrong" );
			pm.MakeHollow( m );  // Reread during TearDown will not load Reise anymore.
		}

		[Test]
		public void TestDeletePersistentDirty()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			r.Zweck = "Test";
			Assert.AreEqual( NDOObjectState.PersistentDirty, r.NDOObjectState, "Wrong state #1" );
			pm.Delete( r );
			Assert.AreEqual( NDOObjectState.Deleted, r.NDOObjectState, "Wrong state #1" );
			pm.Abort();
			Assert.AreEqual( "ADC", r.Zweck, "Name shouldn't be changed" );
			pm.Delete( r );
			pm.Save();
			IList l = pm.GetClassExtent( typeof( Reise ) );
			Assert.AreEqual( 0, l.Count, "Number of read objects is wrong" );
			pm.MakeHollow( m );  // Reread during TearDown will not load Reise anymore.
		}

		[Test]
		public void TestPersistentDirty()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			r.Zweck = "Test";
			Assert.AreEqual( NDOObjectState.PersistentDirty, r.NDOObjectState, "Wrong state #1" );
			pm.Abort();
			Assert.AreEqual( NDOObjectState.Persistent, r.NDOObjectState, "Wrong state #2" );
			Assert.AreEqual( "ADC", r.Zweck, "Name shouldn't be changed" );
		}

		[Test]
		public void TestMakeTransient()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			ObjectId id = r.NDOObjectId;
			pm.MakeTransient( r );
			Assert.Null( ((IPersistenceCapable)r).NDOStateManager, "Transient object shouldn't have state manager" );
			Assert.AreEqual( NDOObjectState.Transient, r.NDOObjectState, "Wrong state #1" );
			Assert.That( id.IsValid(), "Id should still be valid #1" );
			r = (Reise)pm.FindObject( id );
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #2" );
			pm.MakeTransient( r );
			Assert.Null( ((IPersistenceCapable)r).NDOStateManager, "Transient object shouldn't have state manager" );
			Assert.AreEqual( NDOObjectState.Transient, r.NDOObjectState, "Wrong state #3" );
			Assert.That( id.IsValid(), "Id should still be valid #2" );
			r = (Reise)pm.FindObject( id );
			pm.Delete( r );
			pm.Save();
			pm.MakeHollow( m );  // Reread during TearDown will not load Reise anymore.
		}

		[Test]
		public void TestMakeHollow()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( r );
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #1" );
			string access = r.Zweck;
			Assert.AreEqual( NDOObjectState.Persistent, r.NDOObjectState, "Wrong state #2" );
			pm.MakeAllHollow();
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #3" );
		}

		[Test]
		public void TestChangeHollow()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( r );
			r.Zweck = "Test";
			pm.Abort();
			Assert.AreEqual( "ADC", r.Zweck, "Name shouldn't be changed" );
			pm.MakeHollow( r );
			r.Zweck = "Test";
			pm.Save();
			Assert.AreEqual( "Test", r.Zweck, "Name should be changed" );
		}

		[Test]
		public void TestRelatedObject()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( r );
			r.Zweck = "Test";
			pm.Save();  // Bug would overwrite foreign key in database.
			Assert.AreEqual( "Test", r.Zweck, "Name should be changed" );
			r = null;
			ObjectId id = m.NDOObjectId;
			m = null;
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( id );
			Assert.AreEqual( 1, m.Reisen.Count, "Number of children" );
		}

		[Test]
		public void TestRefreshFailed()
		{
			var thrown = false;
			try
			{
				pm.Refresh( r );
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.AreEqual( true, thrown );
		}

		[Test]
		public void TestRefresh()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( r );
			pm.Refresh( r );
			Assert.AreEqual( NDOObjectState.Persistent, r.NDOObjectState, "Wrong state #1" );
			pm.Refresh( r );
			Assert.AreEqual( NDOObjectState.Persistent, r.NDOObjectState, "Wrong state #2" );
			ObjectId id = r.NDOObjectId;

			PersistenceManager pm2 = PmFactory.NewPersistenceManager();
			Reise m2 = (Reise)pm2.FindObject( id );
			Assert.NotNull( m2, "Cannot load object" );
			m2.Zweck = "Test";
			pm2.Save();
			pm2.Close();

			Assert.AreEqual( "ADC", r.Zweck, "Wrong name #1" );
			Assert.AreEqual( "Test", m2.Zweck, "Wrong name #2" );
			pm.Refresh( r );
			Assert.AreEqual( "Test", r.Zweck, "Wrong name #3" );
		}

		[Test]
		public void TestHollowMode()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.HollowMode = true;
			pm.Save();
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #1" );
			r.Zweck = "Test";
			pm.Abort();
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #2" );
			r.Zweck = "Test";
			pm.Save();
			Assert.AreEqual( NDOObjectState.Hollow, r.NDOObjectState, "Wrong state #3" );
		}

		private Reise CreateReise( string zweck )
		{
			Reise r = new Reise();
			r.Zweck = zweck;
			return r;
		}

		private Mitarbeiter CreateMitarbeiter()
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Nachname = "Kocher";
			m.Vorname = "Hartmut";
			return m;
		}
	}
}
