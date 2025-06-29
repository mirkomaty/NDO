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
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using NDO;
using Reisekosten.Personal;
using NDO.Query;
using NDO.Linq;

namespace NdoUnitTests
{
	[TestFixture] 
	public class NDOMitarbeiterTests : NDOTest
	{
		public NDOMitarbeiterTests() 
		{
		}

		private Mitarbeiter m;


		[SetUp]
		public void Setup() 
		{
			m = CreateMitarbeiter("Hartmut", "Kocher");
		}

		[TearDown]
		public void TearDown() 
		{
			var pm = PmFactory.NewPersistenceManager();
			if ( null != pm )
			{
				IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
				if ( mitarbeiterListe.Count > 0 )
				{
					pm.Delete( mitarbeiterListe );
					pm.Save();
				}
				pm.Close();
			}
		}

		[Test]
		public void TestMappingFileName()
		{
			var pm = PmFactory.NewPersistenceManager();
			Debug.WriteLine("Mapping file = " + pm.NDOMapping.FileName);
		}


		[Test]
		public void TestEmptyTransactionSave() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.Save();
		}

		[Test]
		public void TestEmptyTransactionAbort() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.Abort();
		}

		/// <summary>
		/// This is not a real test, it just ensures that the DB is empty.
		/// </summary>
		[Test]
		public void EmptyDB() 
		{
			var pm = PmFactory.NewPersistenceManager();
			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), true);
			pm.Delete(mitarbeiterListe);
			pm.Save();
		}


		[Test]
		public void TestObjectCreation() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
            Assert.That(m.NDOObjectId != null, "ObjectId should be valid");
            Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "Status wrong");
		}

		[Test]
		public void CheckBits()
		{
			Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
		}

		[Test]
		public void CheckPath()
		{
			Console.WriteLine( Environment.GetEnvironmentVariable( "PATH" ) );
		}

		[Test]
		public void TestObjectCreationSave() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestRequery() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			Mitarbeiter m1 = new NDOQuery<Mitarbeiter>( pm ).ExecuteSingle();
			m1.Nachname = "Matytschak";
			Mitarbeiter m2 = new NDOQuery<Mitarbeiter>( pm ).ExecuteSingle();
			Assert.That("Matytschak" ==  m2.Nachname, "Objekt nicht wiederverwendet" );
			m2.Vorname = "Mirko";
		}

        [Test]
		public void TestObjectCreationSaveChanged() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			m.Nachname = "Müller";
			pm.Save();
			Assert.That("Müller" ==  m.Nachname, "Nachname wrong");
		}

		[Test]
		public void TestObjectCreationAbort() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Abort();
			Assert.That(m.NDOObjectId == null, "Transient object shouldn't have ID");
			Assert.That(((IPersistenceCapable)m).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestObjectCreationAbortChanged() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			m.Nachname = "Müller";
			pm.Abort();
			Assert.That("Kocher" ==  m.Nachname, "Nachname wrong");
		}

		[Test]
		public void TestObjectId() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			ObjectId id = m.NDOObjectId;
			ObjectId id2 = new ObjectId(id);
			Assert.That(m.NDOObjectId ==  id, "IDs should be equal");
			Assert.That(m.NDOObjectId ==  id2, "IDs should be equal");
			if (!pm.HasOwnerCreatedIds && m.NDOObjectId.Id[0] is int)
                Assert.That((int)m.NDOObjectId.Id[0] < 0, "Negative key in DS");
			pm.Save();
			Assert.That(m.NDOObjectId ==  id, "IDs should be equal");
			Assert.That(m.NDOObjectId ==  id2, "IDs should be equal");
            if (!pm.HasOwnerCreatedIds && m.NDOObjectId.Id[0] is int)
                Assert.That((int)m.NDOObjectId.Id[0] > 0, "Positive key in DB");
		}

		[Test]
		public void TestCreateDeleteTransitionSave() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Delete(m);
			Assert.That(m.NDOObjectId == null, "Transient object shouldn't have ID");
			Assert.That(((IPersistenceCapable)m).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Status wrong");
			pm.Save();
			Assert.That(m.NDOObjectId == null, "Transient object shouldn't have ID");
			Assert.That(((IPersistenceCapable)m).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestCreateDeleteTransitionAbort() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			ObjectId id = m.NDOObjectId;
			pm.Delete(m);
			pm.Abort();
			Assert.That(m.NDOObjectId == null, "Transient object shouldn't have ID");
			Assert.That(((IPersistenceCapable)m).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Status wrong");
			IPersistenceCapable pc = pm.FindObject(id);
			Assert.That(pc != null, "There should be a hollow object." );
			Assert.That(NDOObjectState.Hollow ==  pc.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestCleanupCache() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.CleanupCache();
			Assert.That(Object.ReferenceEquals(m, pm.FindObject(m.NDOObjectId)), "Getting same object twice should return same object");
			pm.Save();
			pm.CleanupCache();
			Assert.That(Object.ReferenceEquals(m, pm.FindObject(m.NDOObjectId)), "Getting same object twice should return same object");
			ObjectId id = m.NDOObjectId;
			m = null;
			pm.CleanupCache();
			Assert.That(pm.FindObject(id) != null, "Should find object");
		}

		[Test]
		public void TestUnloadCache() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			Assert.That(Object.ReferenceEquals(m, pm.FindObject(m.NDOObjectId)), "Getting same object twice should return same object");
			pm.Save();
			pm.UnloadCache();
			Assert.That(m != pm.FindObject(m.NDOObjectId), "Getting same object twice should return different objects");
			ObjectId id = m.NDOObjectId;
			m = null;
			pm.UnloadCache();
			Assert.That(pm.FindObject(id) != null, "Should find object");
		}

		[Test]
		public void PmWithCachedMappingsWorks()
		{
			var pm = PmFactory.NewPersistenceManager();
			PersistenceManager pm2 = new PersistenceManager( pm.NDOMapping );

			pm2.MakePersistent( m );
			pm2.Save();
			pm2.UnloadCache();
			var m2 = pm2.Objects<Mitarbeiter>().First();

			Assert.That(m.NDOObjectId ==  m2.NDOObjectId );
		}

		[Test]
		public void TestClassExtent() 
		{
			var pm = PmFactory.NewPersistenceManager();
			ArrayList  mliste = new ArrayList();
			for(int i = 0; i < 100; i++) 
			{
				Mitarbeiter mm = CreateMitarbeiter("Hartmut", i.ToString());
				pm.MakePersistent(mm);
				mliste.Add(mm);
			}

			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.That(0 ==  mitarbeiterListe.Count, "Current extent should be empty");
			pm.Save();

			mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.That(100 ==  mitarbeiterListe.Count, "Number of read objects is wrong");
			// Check that all objects come from cache... 
			foreach(Mitarbeiter m1 in mitarbeiterListe) 
			{
				Assert.That(NDOObjectState.Persistent ==  m1.NDOObjectState, "Wrong state");
			}

			mliste.Clear();
			mliste = null;
			mitarbeiterListe.Clear();
			mitarbeiterListe = null;
			//pm.CleanupCache();
			pm.UnloadCache();
			mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.That(100 ==  mitarbeiterListe.Count, "Number of read objects is wrong");
			// Check that all objects are reloaded 
			foreach(Mitarbeiter m1 in mitarbeiterListe) 
			{
				Assert.That(NDOObjectState.Hollow ==  m1.NDOObjectState, "Wrong state");
			}
		}


		[Test]
		public void TestMakePersistentHierarchy() 
		{
			var pm = PmFactory.NewPersistenceManager();
			Reisekosten.Email email = new Reisekosten.Email();
			m.Hinzufuegen(email);
			email.Subject = new Reisekosten.Subject();
			email.Subject.Text = "Test";
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Mitarbeiter muss transient sein");
			Assert.That(NDOObjectState.Transient ==  email.NDOObjectState, "Email muss transient sein");
			Assert.That(NDOObjectState.Transient ==  email.Subject.NDOObjectState, "Subject muss transient sein");
			pm.MakePersistent(m);
			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "Mitarbeiter muss Created sein");
			Assert.That(NDOObjectState.Created ==  email.NDOObjectState, "Email muss Created sein");
			Assert.That(NDOObjectState.Created ==  email.Subject.NDOObjectState, "Subject muss Created sein");
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Mitarbeiter muss Persistent sein");
			Assert.That(NDOObjectState.Persistent ==  email.NDOObjectState, "Email muss Persistent sein");
			Assert.That(NDOObjectState.Persistent ==  email.Subject.NDOObjectState, "Subject muss Persistent sein");
			
		}

		[Test]
		public void TestQuery() 
		{
			var pm = PmFactory.NewPersistenceManager();
			ArrayList  mliste = new ArrayList();
			for(int i = 1; i <= 100; i++) 
			{
				Mitarbeiter mm = CreateMitarbeiter("Hartmut", (i %  3)  == 0 ? "Test" : "xxx");
				pm.MakePersistent(mm);
				mliste.Add(mm);
			}
			pm.Save();		

			// Empty query: just like class extent.
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null, true);
			IList mitarbeiterListe = q.Execute();
			Assert.That(100 ==  mitarbeiterListe.Count, "Number of read objects is wrong");

			q = new NDOQuery<Mitarbeiter>(pm, "nachname = 'Test'", true);
			//System.Diagnostics.Debug.WriteLine(q.GeneratedQuery);
			mitarbeiterListe = q.Execute();
			Assert.That(100/3 ==  mitarbeiterListe.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestLinqQuery() 
		{
			var pm = PmFactory.NewPersistenceManager();
			ArrayList  mliste = new ArrayList();
			for(int i = 1; i <= 100; i++) 
			{
				Mitarbeiter mm = CreateMitarbeiter("Hartmut", (i % 3)  == 0 ? "Test" : "xxx");
				pm.MakePersistent(mm);
				mliste.Add(mm);
			}
			pm.Save();		
			
			List<Mitarbeiter> mitarbeiterListe = from m in pm.Objects<Mitarbeiter>() select m;

			Assert.That(100 ==  mitarbeiterListe.Count, "Number of read objects is wrong #1");

			mitarbeiterListe = from m in pm.Objects<Mitarbeiter>() where m.Nachname == "Test" select m;
			Assert.That(100/3 ==  mitarbeiterListe.Count, "Number of read objects is wrong #2");

			// Partial select
			List<string> nameList = from m in pm.Objects<Mitarbeiter>() select m.Vorname;

			Assert.That(100 ==  nameList.Count, "Number of read objects is wrong #3");

			nameList = from m in pm.Objects<Mitarbeiter>() where m.Nachname == "Test" select m.Vorname;
			Assert.That(100/3 ==  mitarbeiterListe.Count, "Number of read objects is wrong #4");
		}

        [Test]
        public void LinqQueryWithParameterWorks()
        {
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
            pm.Save();
            Mitarbeiter m2 = Mitarbeiter.QueryByName(pm, "Hartmut");
            Assert.That(m2  != null);
        }

        [Test]
        public void LinqQueryWithNullParameterWorks()
        {
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
            pm.Save();
            Mitarbeiter m2 = Mitarbeiter.QueryByName(pm, null);
            Assert.That( m2 == null );
        }

        [Test]
		public void TestDeleteHollow() 
		{
			//			System.Diagnostics.Debug.WriteLine("TestDeleteHollow");
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #1");
			pm.Delete(m);
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "Wrong state #2");
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Wrong state #3");
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #4");
			pm.Delete(m);
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Wrong state #5");

			IList l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.That(0 ==  l.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestDeletePersistent() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			pm.Delete(m);
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "Wrong state #1");
			IList l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.That(1 ==  l.Count, "Number of read objects is wrong");
			pm.Save();
			l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.That(0 ==  l.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestDeletePersistentDirty() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			m.Nachname = "Test";
			Assert.That(NDOObjectState.PersistentDirty ==  m.NDOObjectState, "Wrong state #1");
			pm.Delete(m);
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "Wrong state #1");
			pm.Abort();
			Assert.That("Kocher" ==  m.Nachname, "Name shouldn't be changed");
			pm.Delete(m);
			pm.Save();
			IList l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.That(0 ==  l.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestPersistentDirty() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			m.Nachname = "Test";
			Assert.That(NDOObjectState.PersistentDirty ==  m.NDOObjectState, "Wrong state #1");
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Wrong state #2");
			Assert.That("Kocher" ==  m.Nachname, "Name shouldn't be changed");
		}

		[Test]
		public void TestMakeTransient() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			ObjectId id = m.NDOObjectId;
			pm.MakeTransient(m);
			Assert.That(id.Id != null, "MakeTransient shouldn't remove the ID");			
			Assert.That(((IPersistenceCapable)m).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Wrong state #1");
			Assert.That(id.IsValid(), "Id should still be valid #1");
			m = (Mitarbeiter)pm.FindObject(id);
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #2");
			pm.MakeTransient(m);
			Assert.That(((IPersistenceCapable)m).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Wrong state #3");
			Assert.That(id.IsValid(), "Id should still be valid #2");
			m = (Mitarbeiter)pm.FindObject(id);
			pm.Delete(m);
			pm.Save();
			Assert.That(id.IsValid(), "Id should be valid for the ChangeLog");
		}

		[Test]
		public void TestMakeHollow() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #1");
			string access = m.Nachname;
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Wrong state #2");
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #3");
		}

		[Test]
		public void TestChangeHollow() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			m.Nachname = "Test";
			pm.Abort();
			Assert.That("Kocher" ==  m.Nachname, "Name shouldn't be changed");
			pm.MakeHollow(m);
			m.Nachname = "Test";
			pm.Save();
			Assert.That("Test" ==  m.Nachname, "Name should be changed");
		}

		[Test]
		public void TestRefreshFailed() 
		{
			var pm = PmFactory.NewPersistenceManager();
			bool thrown = false;
			try
			{
				pm.Refresh( m );
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
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			pm.Refresh(m);
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Wrong state #1");
			pm.Refresh(m);
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Wrong state #2");
			ObjectId id = m.NDOObjectId;

			PersistenceManager pm2 = PmFactory.NewPersistenceManager();
			Mitarbeiter m2 = (Mitarbeiter)pm2.FindObject(id);
			Assert.That(m2 != null, "Cannot load object");
			m2.Nachname = "Test";
			pm2.Save();
			pm2.Close();

			Assert.That("Kocher" ==  m.Nachname, "Wrong name #1");
			Assert.That("Test" ==  m2.Nachname, "Wrong name #2");
			pm.Refresh(m);
			Assert.That("Test" ==  m.Nachname, "Wrong name #3");
		}

		[Test]
		public void TestHollowMode() 
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.HollowMode = true;
			pm.Save();			
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #1");
			m.Nachname = "Test";
			pm.Abort();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #2");
			m.Nachname = "Test";
			pm.Save();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #3");
		}

		[Test]
		public void TestAggregateQuery()
		{
			var pm = PmFactory.NewPersistenceManager();
			Mitarbeiter mm;
			IList l = new ArrayList();

			for (int i = 0; i < 20; i++)
			{
				mm = new Mitarbeiter();
				mm.Vorname = "lkj";
				mm.Nachname = i.ToString();
				mm.Position = new System.Drawing.Point(0, i);
				pm.MakePersistent(mm);
				l.Add(mm);
			}
			pm.Save();
			decimal sum = 0m;			
			foreach(Mitarbeiter m2 in l)
			{
				sum += m2.Position.Y;
			}
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null);
			decimal newsum = (decimal)q.ExecuteAggregate("position.Y", AggregateType.Sum);
			Assert.That(sum ==  newsum, "Summe stimmt nicht");
			decimal count = (decimal)q.ExecuteAggregate("position.X", AggregateType.Count);
			Assert.That(20 ==  count, "Count stimmt nicht");
		}


		[Test]
		public void LinqTestAggregateQuery()
		{
			var pm = PmFactory.NewPersistenceManager();
			Mitarbeiter mm;
			var l = new List<Mitarbeiter>();

			for (int i = 0; i < 20; i++)
			{
				mm = new Mitarbeiter();
				mm.Vorname = "lkj";
				mm.Nachname = i.ToString();
				mm.Gehalt = 5000m + i * 100m;
				pm.MakePersistent( mm );
				l.Add( mm );
			}
			pm.Save();
			decimal sum = 0m;
			foreach (Mitarbeiter m2 in l)
			{
				sum += m2.Gehalt;
			}

			decimal avg = sum / 20m;

			var virtualTable = pm.Objects<Mitarbeiter>();

			decimal count = virtualTable.Count;
			Assert.That(20 ==  count, "Count stimmt nicht" );

			decimal newsum = virtualTable.Sum( m => m.Gehalt );
			Assert.That(sum ==  newsum, "Summe stimmt nicht" );

			decimal newavg = virtualTable.Average( m => m.Gehalt );
			Assert.That(avg ==  newavg, "Durchschnitt stimmt nicht" );
		}


		[Test]
		public void TestPolymorphicQuery()
		{
			// If two classes are mapped to the same table, the query
			// should give us the class which is given as parameter
			var pm = PmFactory.NewPersistenceManager();

			pm.MakePersistent(m); // Manager with privilegstufe 0
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Mitarbeiter>(pm, null);
			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Only one object should be returned");
			Assert.That(typeof(Mitarbeiter) ==  l[0].GetType(), "Object should be a Mitarbeiter");

			// Fetch the same object as manager
			q = new NDOQuery<Manager>(pm, null);
			l = q.Execute();
			Assert.That(1 ==  l.Count, "Only one object should be returned");
			Assert.That(typeof(Manager) ==  l[0].GetType(), "Object should be a Manager");
		}

		[Test]
		public void TestStringLen()
		{
			var pm = PmFactory.NewPersistenceManager();
			string s = string.Empty;
			for (int i = 0; i < 255; i++)
			{
				s += "A";
			}
			m.Vorname = s;
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm);
			Mitarbeiter m2 = (Mitarbeiter) q.ExecuteSingle(true);
			Assert.That(255 ==  m2.Vorname.Length, "Lönge des Vornamens falsch");
		}


		[Test]
		public void TestHollowDelete()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			// Load hollow
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null, true);
			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Anzahl Mitarbeiter falsch #1");
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			q = new NDOQuery<Mitarbeiter>(pm);
			l = q.Execute();
			Assert.That(0 ==  l.Count, "Anzahl Mitarbeiter falsch #2");

		}

		[Test]
		public void TestHollowDelete2Pm()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			// Load hollow
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null, true);
			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Anzahl Mitarbeiter falsch #1");
			PersistenceManager pm2 = PmFactory.NewPersistenceManager();
			pm2.Delete(l);
			pm2.Save();
			q = new NDOQuery<Mitarbeiter>(pm);
			l = q.Execute();
			Assert.That(0 ==  l.Count, "Anzahl Mitarbeiter falsch #2");
		}


//		[Test]
//		void TestVerboseMode()
//		{
//			PersistenceManager pm = new PersistenceManager();
//
//			pm.MakePersistent(m);
//			pm.Save();
//
//			m.Vorname = "Hallo";
//
//			m.NDOStateManager.PersistenceManager.LogPath = "C:\\Temp";
//			Assert.That(m.NDOStateManager.PersistenceManager.LogPath ==  pm.LogPath, "LogPath falsch");
//			if (File.Exists(pm.LogFile))
//				....
//			m.NDOStateManager.PersistenceManager.VerboseMode = true;
//         
//			m.NDOStateManager.PersistenceManager.Save();	
//		}


		[Test]
		public void Angriff()
		{
			var pm = PmFactory.NewPersistenceManager();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, "nachname LIKE {0}");
			q.Parameters.Add("dummy';SELECT * from Mitarbeiter; --");
			IList l = q.Execute();
		}

		[Test]
		public void LoadDataOnUnknownObjectThrows()
		{
			var pm = PmFactory.NewPersistenceManager();
			Mitarbeiter m = (Mitarbeiter)pm.FindObject( typeof( Mitarbeiter ), 1000000 );
			bool thrown = false;
			try
			{
				pm.LoadData( m );
			}
			catch (NDOException ex)
			{
				Assert.That(72 ==  ex.ErrorNumber );
				thrown = true;
			}

			Assert.That( thrown );
		}

		[Test]
		public void LoadDataOnUnknownObjectCallsEventHandler()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.ObjectNotPresentEvent += ea => true;  // prevents throwing an exception
			Mitarbeiter m = (Mitarbeiter)pm.FindObject( typeof( Mitarbeiter ), 1000000 );
			pm.LoadData( m );
		}

		[Test]
		public void LoadDataReturnsPersistentObject()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			var oid = m.NDOObjectId;
			pm.Save();
			pm.UnloadCache();
			var m2 = pm.FindObject( oid );
			Assert.That(NDOObjectState.Hollow ==  m2.NDOObjectState );
			pm.LoadData( m2 );
			Assert.That(NDOObjectState.Persistent ==  m2.NDOObjectState );
		}

		[Test]
		public void CanQueryWithInClause()
		{
			var pm = PmFactory.NewPersistenceManager();
			var m1 = CreateMitarbeiter("Mirko", "Matytschak");
			var m2 = CreateMitarbeiter("Hans", "Huber");
			pm.MakePersistent( m1 );
			pm.MakePersistent( m2 );
			pm.Save();
			pm.UnloadCache();
			var l = pm.Objects<Mitarbeiter>().Where(m=>m.Vorname.In(new []{ "Mirko" } )).ResultTable;
			Assert.That(1 ==  l.Count );
			l = pm.Objects<Mitarbeiter>().Where(m=>m.Vorname.In(new []{ "Mirko", "Hans" } )).ResultTable;
			Assert.That(2 ==  l.Count );
			Assert.That(0 ==  QueryByGuid( new[] { Guid.NewGuid() } ).Count );
		}

		List<Mitarbeiter> QueryByGuid(Guid[] guids)
		{
			var pm = PmFactory.NewPersistenceManager();
			return pm.Objects<Mitarbeiter>().Where( m => m.Vorname.In( guids ) );
		}

		[Test]
		public void CanInsertObjectsUsingSqlHandler()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.None;
			using (var handler = pm.GetSqlPassThroughHandler())
			{
				var sql = "INSERT INTO [Mitarbeiter] ([Vorname],[Nachname],[Gehalt]) VALUES (@p0, @p1, @p2)";
				handler.Execute( sql, false, "Fritz", "Müller", 123456.34m );
			}
			var m = pm.Objects<Mitarbeiter>().SingleOrDefault();
			Assert.That(m  != null);
			Assert.That("Fritz" ==  m.Vorname );
			Assert.That("Müller" ==  m.Nachname );
			Assert.That(123456.34m ==  m.Gehalt );
		}

		[Test]
		public void CanInsertObjectsWithNullParameterUsingSqlHandler()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.None;
			using (var handler = pm.GetSqlPassThroughHandler())
			{
				var sql = "INSERT INTO [Mitarbeiter] ([Vorname],[Nachname],[Gehalt]) VALUES (@p0, @p1, @p2)";
				handler.Execute( sql, false, null, "Müller", 123456.34m );
			}
			var m = pm.Objects<Mitarbeiter>().SingleOrDefault();
			Assert.That(m  != null);
			Assert.That("Müller" ==  m.Nachname );
			Assert.That(123456.34m ==  m.Gehalt );
		}

		[Test]
		public void CanPerformDirectDelete()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			var firstName = m.Vorname;

			Assert.That( pm.Objects<Mitarbeiter>().Where( m => m.Vorname == firstName ).Count > 0 );

			var q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
			q.Parameters.Add( firstName );
			q.DeleteDirectly();

			Assert.That(0 ==  pm.Objects<Mitarbeiter>().Where( m => m.Vorname == firstName ).Count );

			m = CreateMitarbeiter( "Mirko", "Matytschak" );
			pm.MakePersistent( m );
			pm.Save();
			firstName = m.Vorname;

			Assert.That( pm.Objects<Mitarbeiter>().Where( m => m.Vorname == firstName ).Count > 0 );

			pm.Objects<Mitarbeiter>().Where( m => m.Vorname == firstName ).DeleteDirectly();

			Assert.That(0 ==  pm.Objects<Mitarbeiter>().Where( m => m.Vorname == firstName ).Count );
		}

		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) 
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}
	}
}
