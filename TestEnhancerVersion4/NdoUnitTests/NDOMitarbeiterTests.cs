//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


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

namespace NdoUnitTests
{
	[TestFixture] 
	public class NDOMitarbeiterTests 
	{
		public NDOMitarbeiterTests() 
		{
		}

		private PersistenceManager pm;
		private Mitarbeiter m;


		[SetUp]
		public void Setup() 
		{
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Hartmut", "Kocher");
		}

		[TearDown]
		public void TearDown() 
		{
			if ( null != pm )
			{
				IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
				if ( mitarbeiterListe.Count > 0 )
				{
					pm.Delete( mitarbeiterListe );
					pm.Save();
				}
				pm.Close();
				pm = null;
			}
		}

		[Test]
		public void TestMappingFileName()
		{
			Debug.WriteLine("Mapping file = " + pm.NDOMapping.FileName);
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

		/// <summary>
		/// This is not a real test, it just ensures that the DB is empty.
		/// </summary>
		[Test]
		public void EmptyDB() 
		{
			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), true);
			pm.Delete(mitarbeiterListe);
			pm.Save();
		}


		[Test]
		public void TestObjectCreation() 
		{
			pm.MakePersistent(m);
            Assert.NotNull(m.NDOObjectId, "ObjectId should be valid");
            Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "Status wrong");
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
			pm.MakePersistent(m);
			pm.Save();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestRequery() 
		{
			pm.MakePersistent(m);
			pm.Save();
			Mitarbeiter m1 = new NDOQuery<Mitarbeiter>( pm ).ExecuteSingle();
			m1.Nachname = "Matytschak";
			Mitarbeiter m2 = new NDOQuery<Mitarbeiter>( pm ).ExecuteSingle();
			Assert.AreEqual( "Matytschak", m2.Nachname, "Objekt nicht wiederverwendet" );
			m2.Vorname = "Mirko";
		}


		[Test]
		public void TestObjectCreationSaveChanged() 
		{
			pm.MakePersistent(m);
			m.Nachname = "Müller";
			pm.Save();
			Assert.AreEqual("Müller", m.Nachname, "Nachname wrong");
		}

		[Test]
		public void TestObjectCreationAbort() 
		{
			pm.MakePersistent(m);
			pm.Abort();
			Assert.Null(m.NDOObjectId, "Transient object shouldn't have ID");
			Assert.Null(m.NDOStateManager, "Transient object shouldn't have state manager");
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestObjectCreationAbortChanged() 
		{
			pm.MakePersistent(m);
			m.Nachname = "Müller";
			pm.Abort();
			Assert.AreEqual("Kocher", m.Nachname, "Nachname wrong");
		}

		[Test]
		public void TestObjectId() 
		{
			pm.MakePersistent(m);
			ObjectId id = m.NDOObjectId;
			ObjectId id2 = new ObjectId(id);
			Assert.AreEqual(m.NDOObjectId, id, "IDs should be equal");
			Assert.AreEqual(m.NDOObjectId, id2, "IDs should be equal");
			if (!pm.HasOwnerCreatedIds && m.NDOObjectId.Id[0] is int)
                Assert.That((int)m.NDOObjectId.Id[0] < 0, "Negative key in DS");
			pm.Save();
			Assert.AreEqual(m.NDOObjectId, id, "IDs should be equal");
			Assert.AreEqual(m.NDOObjectId, id2, "IDs should be equal");
            if (!pm.HasOwnerCreatedIds && m.NDOObjectId.Id[0] is int)
                Assert.That((int)m.NDOObjectId.Id[0] > 0, "Positive key in DB");
		}

		[Test]
		public void TestCreateDeleteTransitionSave() 
		{
			pm.MakePersistent(m);
			pm.Delete(m);
			Assert.Null(m.NDOObjectId, "Transient object shouldn't have ID");
			Assert.Null(m.NDOStateManager, "Transient object shouldn't have state manager");
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Status wrong");
			pm.Save();
			Assert.Null(m.NDOObjectId, "Transient object shouldn't have ID");
			Assert.Null(m.NDOStateManager, "Transient object shouldn't have state manager");
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestCreateDeleteTransitionAbort() 
		{
			pm.MakePersistent(m);
			ObjectId id = m.NDOObjectId;
			pm.Delete(m);
			pm.Abort();
			Assert.Null(m.NDOObjectId, "Transient object shouldn't have ID");
			Assert.Null(m.NDOStateManager, "Transient object shouldn't have state manager");
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Status wrong");
			IPersistenceCapable pc = pm.FindObject(id);
			Assert.NotNull( pc, "There should be a hollow object." );
			Assert.AreEqual(NDOObjectState.Hollow, pc.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestCleanupCache() 
		{
			pm.MakePersistent(m);
			pm.CleanupCache();
			Assert.AreSame(m, pm.FindObject(m.NDOObjectId), "Getting same object twice should return same object");
			pm.Save();
			pm.CleanupCache();
			Assert.AreSame(m, pm.FindObject(m.NDOObjectId), "Getting same object twice should return same object");
			ObjectId id = m.NDOObjectId;
			m = null;
			pm.CleanupCache();
			Assert.NotNull(pm.FindObject(id), "Should find object");
		}

		[Test]
		public void TestUnloadCache() 
		{
			pm.MakePersistent(m);
			pm.UnloadCache();
			Assert.AreSame(m, pm.FindObject(m.NDOObjectId), "Getting same object twice should return same object");
			pm.Save();
			pm.UnloadCache();
			Assert.That(m != pm.FindObject(m.NDOObjectId), "Getting same object twice should return different objects");
			ObjectId id = m.NDOObjectId;
			m = null;
			pm.UnloadCache();
			Assert.NotNull(pm.FindObject(id), "Should find object");
		}

		[Test]
		public void TestClassExtent() 
		{
			ArrayList  mliste = new ArrayList();
			for(int i = 0; i < 100; i++) 
			{
				Mitarbeiter mm = CreateMitarbeiter("Hartmut", i.ToString());
				pm.MakePersistent(mm);
				mliste.Add(mm);
			}

			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.AreEqual(0, mitarbeiterListe.Count, "Current extent should be empty");
			pm.Save();

			mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.AreEqual(100, mitarbeiterListe.Count, "Number of read objects is wrong");
			// Check that all objects come from cache... 
			foreach(Mitarbeiter m1 in mitarbeiterListe) 
			{
				Assert.AreEqual(NDOObjectState.Persistent, m1.NDOObjectState, "Wrong state");
			}

			mliste.Clear();
			mliste = null;
			mitarbeiterListe.Clear();
			mitarbeiterListe = null;
			//pm.CleanupCache();
			pm.UnloadCache();
			mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.AreEqual(100, mitarbeiterListe.Count, "Number of read objects is wrong");
			// Check that all objects are reloaded 
			foreach(Mitarbeiter m1 in mitarbeiterListe) 
			{
				Assert.AreEqual(NDOObjectState.Hollow, m1.NDOObjectState, "Wrong state");
			}
		}


		[Test]
		public void TestMakePersistentHierarchy() 
		{
			Reisekosten.Email email = new Reisekosten.Email();
			m.Hinzufuegen(email);
			email.Subject = new Reisekosten.Subject();
			email.Subject.Text = "Test";
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Mitarbeiter muss transient sein");
			Assert.AreEqual(NDOObjectState.Transient, email.NDOObjectState, "Email muss transient sein");
			Assert.AreEqual(NDOObjectState.Transient, email.Subject.NDOObjectState, "Subject muss transient sein");
			pm.MakePersistent(m);
			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "Mitarbeiter muss Created sein");
			Assert.AreEqual(NDOObjectState.Created, email.NDOObjectState, "Email muss Created sein");
			Assert.AreEqual(NDOObjectState.Created, email.Subject.NDOObjectState, "Subject muss Created sein");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Mitarbeiter muss Persistent sein");
			Assert.AreEqual(NDOObjectState.Persistent, email.NDOObjectState, "Email muss Persistent sein");
			Assert.AreEqual(NDOObjectState.Persistent, email.Subject.NDOObjectState, "Subject muss Persistent sein");
			
		}

		[Test]
		public void TestQuery() 
		{
			ArrayList  mliste = new ArrayList();
			for(int i = 1; i <= 100; i++) 
			{
				Mitarbeiter mm = CreateMitarbeiter("Hartmut", (i %  3)  == 0 ? "Test" : "xxx");
				pm.MakePersistent(mm);
				mliste.Add(mm);
			}
			pm.Save();		

			// Empty query: just like class extent.
			Query q = pm.NewQuery(typeof(Mitarbeiter), null, true);
			IList mitarbeiterListe = q.Execute();
			Assert.AreEqual(100, mitarbeiterListe.Count, "Number of read objects is wrong");

			q = pm.NewQuery(typeof(Mitarbeiter), "nachname = 'Test'", true);
			//System.Diagnostics.Debug.WriteLine(q.GeneratedQuery);
			mitarbeiterListe = q.Execute();
			Assert.AreEqual(100/3, mitarbeiterListe.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestLinqQuery() 
		{
			ArrayList  mliste = new ArrayList();
			for(int i = 1; i <= 100; i++) 
			{
				Mitarbeiter mm = CreateMitarbeiter("Hartmut", (i % 3)  == 0 ? "Test" : "xxx");
				pm.MakePersistent(mm);
				mliste.Add(mm);
			}
			pm.Save();		
			
			List<Mitarbeiter> mitarbeiterListe = from m in pm.Objects<Mitarbeiter>() select m;

			Assert.AreEqual(100, mitarbeiterListe.Count, "Number of read objects is wrong #1");

			mitarbeiterListe = from m in pm.Objects<Mitarbeiter>() where m.Nachname == "Test" select m;
			Assert.AreEqual(100/3, mitarbeiterListe.Count, "Number of read objects is wrong #2");

			// Partial select
			List<string> nameList = from m in pm.Objects<Mitarbeiter>() select m.Vorname;

			Assert.AreEqual(100, nameList.Count, "Number of read objects is wrong #3");

			nameList = from m in pm.Objects<Mitarbeiter>() where m.Nachname == "Test" select m.Vorname;
			Assert.AreEqual(100/3, mitarbeiterListe.Count, "Number of read objects is wrong #4");
		}

		[Test]
		public void TestDeleteHollow() 
		{
//			System.Diagnostics.Debug.WriteLine("TestDeleteHollow");
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #1");
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "Wrong state #2");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Wrong state #3");
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #4");
			pm.Delete(m);
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Wrong state #5");

			IList l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.AreEqual(0, l.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestDeletePersistent() 
		{
			pm.MakePersistent(m);
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "Wrong state #1");
			IList l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.AreEqual(1, l.Count, "Number of read objects is wrong");
			pm.Save();
			l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.AreEqual(0, l.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestDeletePersistentDirty() 
		{
			pm.MakePersistent(m);
			pm.Save();
			m.Nachname = "Test";
			Assert.AreEqual(NDOObjectState.PersistentDirty, m.NDOObjectState, "Wrong state #1");
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "Wrong state #1");
			pm.Abort();
			Assert.AreEqual("Kocher", m.Nachname, "Name shouldn't be changed");
			pm.Delete(m);
			pm.Save();
			IList l = pm.GetClassExtent(typeof(Mitarbeiter));
			Assert.AreEqual(0, l.Count, "Number of read objects is wrong");
		}

		[Test]
		public void TestPersistentDirty() 
		{
			pm.MakePersistent(m);
			pm.Save();
			m.Nachname = "Test";
			Assert.AreEqual(NDOObjectState.PersistentDirty, m.NDOObjectState, "Wrong state #1");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Wrong state #2");
			Assert.AreEqual("Kocher", m.Nachname, "Name shouldn't be changed");
		}

		[Test]
		public void TestMakeTransient() 
		{
			pm.MakePersistent(m);
			pm.Save();
			ObjectId id = m.NDOObjectId;
			pm.MakeTransient(m);
			Assert.NotNull(m.NDOObjectId, "MakeTransient doesn't remove the ID");			
			Assert.Null(m.NDOStateManager, "Transient object shouldn't have state manager");
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Wrong state #1");
			Assert.That(id.IsValid(), "Id should still be valid #1");
			m = (Mitarbeiter)pm.FindObject(id);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #2");
			pm.MakeTransient(m);
			Assert.Null(m.NDOStateManager, "Transient object shouldn't have state manager");
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Wrong state #3");
			Assert.That(id.IsValid(), "Id should still be valid #2");
			m = (Mitarbeiter)pm.FindObject(id);
			pm.Delete(m);
			pm.Save();
			Assert.That(id.IsValid(), "Id should be valid for the ChangeLog");
		}

		[Test]
		public void TestMakeHollow() 
		{
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #1");
			string access = m.Nachname;
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Wrong state #2");
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #3");
		}

		[Test]
		public void TestChangeHollow() 
		{
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			m.Nachname = "Test";
			pm.Abort();
			Assert.AreEqual("Kocher", m.Nachname, "Name shouldn't be changed");
			pm.MakeHollow(m);
			m.Nachname = "Test";
			pm.Save();
			Assert.AreEqual("Test", m.Nachname, "Name should be changed");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void TestRefreshFailed() 
		{
			pm.Refresh(m);
		}

		[Test]
		public void TestRefresh() 
		{
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			pm.Refresh(m);
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Wrong state #1");
			pm.Refresh(m);
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Wrong state #2");
			ObjectId id = m.NDOObjectId;

			PersistenceManager pm2 = PmFactory.NewPersistenceManager();
			Mitarbeiter m2 = (Mitarbeiter)pm2.FindObject(id);
			Assert.NotNull(m2, "Cannot load object");
			m2.Nachname = "Test";
			pm2.Save();
			pm2.Close();

			Assert.AreEqual("Kocher", m.Nachname, "Wrong name #1");
			Assert.AreEqual("Test", m2.Nachname, "Wrong name #2");
			pm.Refresh(m);
			Assert.AreEqual("Test", m.Nachname, "Wrong name #3");
		}

		[Test]
		public void TestHollowMode() 
		{
			pm.MakePersistent(m);
			pm.HollowMode = true;
			pm.Save();			
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #1");
			m.Nachname = "Test";
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #2");
			m.Nachname = "Test";
			pm.Save();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #3");
		}

		[Test]
		public void TestAggregateQuery()
		{
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
			Query q = pm.NewQuery(typeof(Mitarbeiter), null);
			Mitarbeiter.QueryHelper qh = new Mitarbeiter.QueryHelper();
			decimal newsum = (decimal)q.ExecuteAggregate(qh.position.Y, Query.AggregateType.Sum);
			Assert.AreEqual(sum, newsum, "Summe stimmt nicht");
			decimal count = (decimal)q.ExecuteAggregate(qh.position.X, Query.AggregateType.Count);
			Assert.AreEqual(20, count, "Count stimmt nicht");
		}


		[Test]
		public void TestPolymorphicQuery()
		{
			// If two classes are mapped to the same table, the query
			// should give us the class which is given as parameter

			pm.MakePersistent(m); // Manager with privilegstufe 0
			pm.Save();
			pm.UnloadCache();
			Query q = pm.NewQuery(typeof(Mitarbeiter), null);
			IList l = q.Execute();
			Assert.AreEqual(1, l.Count, "Only one object should be returned");
			Assert.AreEqual(typeof(Mitarbeiter), l[0].GetType(), "Object should be a Mitarbeiter");

			// Fetch the same object as manager
			q = pm.NewQuery(typeof(Manager), null);
			l = q.Execute();
			Assert.AreEqual(1, l.Count, "Only one object should be returned");
			Assert.AreEqual(typeof(Manager), l[0].GetType(), "Object should be a Manager");
		}

		[Test]
		public void TestStringLen()
		{
			string s = string.Empty;
			for (int i = 0; i < 255; i++)
			{
				s += "A";
			}
			m.Vorname = s;
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			Query q = pm.NewQuery(typeof(Mitarbeiter));
			Mitarbeiter m2 = (Mitarbeiter) q.ExecuteSingle(true);
			Assert.AreEqual(255, m2.Vorname.Length, "L�nge des Vornamens falsch");
		}


		[Test]
		public void TestHollowDelete()
		{
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			// Load hollow
			Query q = pm.NewQuery(typeof(Mitarbeiter), null, true);
			IList l = q.Execute();
			Assert.AreEqual(1, l.Count, "Anzahl Mitarbeiter falsch #1");
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			q = pm.NewQuery(typeof(Mitarbeiter));
			l = q.Execute();
			Assert.AreEqual(0, l.Count, "Anzahl Mitarbeiter falsch #2");

		}

		[Test]
		public void TestHollowDelete2Pm()
		{
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			// Load hollow
			Query q = pm.NewQuery(typeof(Mitarbeiter), null, true);
			IList l = q.Execute();
			Assert.AreEqual(1, l.Count, "Anzahl Mitarbeiter falsch #1");
			PersistenceManager pm2 = PmFactory.NewPersistenceManager();
			pm2.Delete(l);
			pm2.Save();
			q = pm.NewQuery(typeof(Mitarbeiter));
			l = q.Execute();
			Assert.AreEqual(0, l.Count, "Anzahl Mitarbeiter falsch #2");
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
//			Assert.AreEqual(m.NDOStateManager.PersistenceManager.LogPath, pm.LogPath, "LogPath falsch");
//			if (File.Exists(pm.LogFile))
//				....
//			m.NDOStateManager.PersistenceManager.VerboseMode = true;
//         
//			m.NDOStateManager.PersistenceManager.Save();	
//		}


		[Test]
		public void Angriff()
		{
//			pm.MakePersistent(m);
//			pm.Save();
			Mitarbeiter.QueryHelper qh = new Mitarbeiter.QueryHelper();
			Query q = pm.NewQuery(typeof(Mitarbeiter), qh.nachname + Query.Op.Like + "{0}");
			q.Parameters.Add(new Query.Parameter("dummy';SELECT * from Mitarbeiter; --"));
			IList l = q.Execute();
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
