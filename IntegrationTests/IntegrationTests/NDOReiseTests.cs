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
			Assert.That(r.NDOObjectId != null, "ObjectId should be valid" );
			Assert.That(NDOObjectState.Created ==  r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestObjectCreationSave()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestObjectCreationSaveChanged()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			r.Zweck = "Test";
			pm.Save();
			Assert.That("Test" ==  r.Zweck, "Zweck wrong" );
		}

		[Test]
		public void TestObjectCreationAbort()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Abort();
			Assert.That(r.NDOObjectId == null, "Transient object shouldn't have ID" );
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager" );
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestObjectCreationAbortChanged()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			r.Zweck = "Möller";
			pm.Abort();
			Assert.That("ADC" ==  r.Zweck, "Nachname wrong" );
		}

		[Test]
		public void TestObjectId()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			ObjectId id = r.NDOObjectId;
			ObjectId id2 = new ObjectId( id );
			Assert.That(r.NDOObjectId ==  id, "IDs should be equal" );
			Assert.That(r.NDOObjectId ==  id2, "IDs should be equal" );
			if (!pm.HasOwnerCreatedIds && r.NDOObjectId.Id[0] is Int32)
				Assert.That( (int)id.Id[0] < 0, "Negative key in DS" );
			pm.Save();
			Assert.That(r.NDOObjectId ==  id, "IDs should be equal" );
			Assert.That(r.NDOObjectId ==  id2, "IDs should be equal" );
			if (!pm.HasOwnerCreatedIds && r.NDOObjectId.Id[0] is Int32)
				Assert.That( (int)id.Id[0] > 0, "Positive key in DB" );
		}

		[Test]
		public void TestCreateDeleteTransitionSave()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Delete( m );
			Assert.That(r.NDOObjectId == null, "Transient object shouldn't have ID" );
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager" );
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Status wrong" );
			pm.Save();
			Assert.That(r.NDOObjectId == null, "Transient object shouldn't have ID" );
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager" );
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestCreateDeleteTransitionAbort()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			ObjectId id = m.NDOObjectId;
			pm.Delete( m );
			pm.Abort();
			Assert.That(r.NDOObjectId == null, "Transient object shouldn't have ID" );
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager" );
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Status wrong" );
		}

		[Test]
		public void TestCleanupCache()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.CleanupCache();
			Assert.That(Object.ReferenceEquals(r, pm.FindObject( r.NDOObjectId )), "Getting same object twice should return same object" );
			pm.Save();
			pm.CleanupCache();
			Assert.That(Object.ReferenceEquals(r, pm.FindObject( r.NDOObjectId )), "Getting same object twice should return same object" );
			ObjectId id = r.NDOObjectId;
			r = null;
			pm.CleanupCache();
			Assert.That(pm.FindObject( id ) != null, "Should find object" );
		}

		[Test]
		public void TestUnloadCache()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			Assert.That(Object.ReferenceEquals(r, pm.FindObject( r.NDOObjectId )), "Getting same object twice should return same object" );
			pm.Save();
			pm.UnloadCache();
			Assert.That( r != pm.FindObject( r.NDOObjectId ), "Getting same object twice should return different objects" );
			ObjectId id = r.NDOObjectId;
			r = null;
			pm.UnloadCache();
			Assert.That(pm.FindObject( id ) != null, "Should find object" );
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
			Assert.That(0 ==  ReiseListe.Count, "Current extent should be empty" );
			pm.Save();

			ReiseListe = pm.GetClassExtent( typeof( Reise ) );
			Assert.That(100 ==  ReiseListe.Count, "Number of read objects is wrong" );
			// Check that all objects come from cache... 
			foreach (Reise m1 in ReiseListe)
			{
				Assert.That(NDOObjectState.Persistent ==  m1.NDOObjectState, "Wrong state" );
			}

			rliste.Clear();
			rliste = null;
			ReiseListe.Clear();
			ReiseListe = null;
			//pm.CleanupCache();
			pm.UnloadCache();
			ReiseListe = pm.GetClassExtent( typeof( Reise ) );
			Assert.That(100 ==  ReiseListe.Count, "Number of read objects is wrong" );
			// Check that all objects are reloaded 
			foreach (Reise m1 in ReiseListe)
			{
				Assert.That(NDOObjectState.Hollow ==  m1.NDOObjectState, "Wrong state" );
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
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #1" );
			pm.Delete( r );
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "Wrong state #2" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Wrong state #3" );
			pm.UnloadCache();
			r = (Reise)pm.FindObject( r.NDOObjectId );
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #4" );
			pm.Delete( r );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Wrong state #5" );

			IList l = pm.GetClassExtent( typeof( Reise ) );
			Assert.That(0 ==  l.Count, "Number of read objects is wrong" );
		}

		[Test]
		public void TestDeletePersistent()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.Delete( r );
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "Wrong state #1" );
			IList l = pm.GetClassExtent( typeof( Reise ) );
			Assert.That(1 ==  l.Count, "Number of read objects is wrong" );
			pm.Save();
			l = pm.GetClassExtent( typeof( Reise ) );
			Assert.That(0 ==  l.Count, "Number of read objects is wrong" );
			pm.MakeHollow( m );  // Reread during TearDown will not load Reise anymore.
		}

		[Test]
		public void TestDeletePersistentDirty()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			r.Zweck = "Test";
			Assert.That(NDOObjectState.PersistentDirty ==  r.NDOObjectState, "Wrong state #1" );
			pm.Delete( r );
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "Wrong state #1" );
			pm.Abort();
			Assert.That("ADC" ==  r.Zweck, "Name shouldn't be changed" );
			pm.Delete( r );
			pm.Save();
			IList l = pm.GetClassExtent( typeof( Reise ) );
			Assert.That(0 ==  l.Count, "Number of read objects is wrong" );
			pm.MakeHollow( m );  // Reread during TearDown will not load Reise anymore.
		}

		[Test]
		public void TestPersistentDirty()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			r.Zweck = "Test";
			Assert.That(NDOObjectState.PersistentDirty ==  r.NDOObjectState, "Wrong state #1" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Wrong state #2" );
			Assert.That("ADC" ==  r.Zweck, "Name shouldn't be changed" );
		}

		[Test]
		public void TestMakeTransient()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			ObjectId id = r.NDOObjectId;
			pm.MakeTransient( r );
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager" );
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Wrong state #1" );
			Assert.That( id.IsValid(), "Id should still be valid #1" );
			r = (Reise)pm.FindObject( id );
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #2" );
			pm.MakeTransient( r );
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager" );
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Wrong state #3" );
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
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #1" );
			string access = r.Zweck;
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Wrong state #2" );
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #3" );
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
			Assert.That("ADC" ==  r.Zweck, "Name shouldn't be changed" );
			pm.MakeHollow( r );
			r.Zweck = "Test";
			pm.Save();
			Assert.That("Test" ==  r.Zweck, "Name should be changed" );
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
			Assert.That("Test" ==  r.Zweck, "Name should be changed" );
			r = null;
			ObjectId id = m.NDOObjectId;
			m = null;
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( id );
			Assert.That(1 ==  m.Reisen.Count, "Number of children" );
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
			Assert.That(true ==  thrown );
		}

		[Test]
		public void TestRefresh()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( r );
			pm.Refresh( r );
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Wrong state #1" );
			pm.Refresh( r );
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Wrong state #2" );
			ObjectId id = r.NDOObjectId;

			PersistenceManager pm2 = PmFactory.NewPersistenceManager();
			Reise m2 = (Reise)pm2.FindObject( id );
			Assert.That(m2 != null, "Cannot load object" );
			m2.Zweck = "Test";
			pm2.Save();
			pm2.Close();

			Assert.That("ADC" ==  r.Zweck, "Wrong name #1" );
			Assert.That("Test" ==  m2.Zweck, "Wrong name #2" );
			pm.Refresh( r );
			Assert.That("Test" ==  r.Zweck, "Wrong name #3" );
		}

		[Test]
		public void TestHollowMode()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.HollowMode = true;
			pm.Save();
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #1" );
			r.Zweck = "Test";
			pm.Abort();
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #2" );
			r.Zweck = "Test";
			pm.Save();
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #3" );
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
