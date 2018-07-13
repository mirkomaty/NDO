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
using NDO;
using NDO.Query;
using NUnit.Framework;
using Reisekosten;
using Reisekosten.Personal;

namespace NdoUnitTests {
	/// <summary>
	/// This class contains all tests for 1:n directed relations with an intermediate mapping table.
	/// </summary>
	[TestFixture]
	public class Rel1toNDirectedWTable {
		
		public Rel1toNDirectedWTable() {
		}

		private PersistenceManager pm;
		private Mitarbeiter m;
		private Reisebüro r;
		private Reisebüro r2;

		[SetUp]
		public void Setup() {
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Hartmut", "Kocher");
			r = CreateReisebüro("Lufthansa City Center");
		}

		[TearDown]
		public void TearDown() {
			pm.Abort();
			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), false);
			pm.Delete(mitarbeiterListe);
			IList rbListe = pm.GetClassExtent(typeof(Reisebüro), false);
			pm.Delete(rbListe);
			pm.Save();
			pm.Close();
			pm = null;
		}

		[Test]
		public void TestCreateObjects() {
			pm.MakePersistent(r);
			if (r.NDOObjectId.Id[0] is Int32 && !pm.HasOwnerCreatedIds)
				Assert.AreEqual(-1, r.NDOObjectId.Id[0], "Reisebüro key wrong");
			Reisebüro r2 = (Reisebüro)pm.FindObject(r.NDOObjectId);
			Assert.AreSame(r, r2, "Reisebüros should match");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void TestCreateObjectsTransient1() {
			m.Hinzufuegen(r);  // Cannot add transient object
			pm.MakePersistent(m);
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void TestCreateObjectsTransient2() {
			pm.MakePersistent(m);
			m.Hinzufuegen(r);  // Cannot add transient object
		}

		[Test]
		public void TestCreateObjectsTransient3() {
			pm.MakePersistent(r);
			pm.MakePersistent(m);
			m.Hinzufuegen(r); 
		}

		[Test]
		public void TestCreateObjectsTransient4() {
			pm.MakePersistent(r);
			m.Hinzufuegen(r); 
			pm.MakePersistent(m);
		}

		[Test]
		public void TestCreateObjectsSave() {
			pm.MakePersistent(r);
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(r.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			r = (Reisebüro)pm.FindObject(r.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(r, "1. Reisebüro not found");

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			r = (Reisebüro)pm.FindObject(r.NDOObjectId);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(r, "2. Reisebüro not found");
		}

		[Test]
		public void TestQueryWithCondition() 
		{
			pm.MakePersistent(r);
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();

			pm.UnloadCache();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, "reiseBüros.name LIKE 'L*'");
			//System.Diagnostics.Debug.WriteLine(q.GeneratedQuery);
			IList l = q.Execute();
			Assert.AreEqual(1, l.Count, "Mitarbeiter not found");
		}


		[Test]
		public void TestAddObjectSave() {
			StandardSetup();
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1. Wrong state");
			pm.Save();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			r = (Reisebüro)pm.FindObject(r.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(r, "1. Reisebüro not found");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void TestAddObjectAbort() {
			StandardSetup();
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(1, m.Reisebüros.Count, "1. Wrong number of objects");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
			Assert.AreEqual(0, m.Reisebüros.Count, "2. Wrong number of objects");
		}
		[Test]
		public void TestRemoveObjectSave() {
			StandardSetup();
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			Assert.AreEqual(1, m.Reisebüros.Count, "1. Wrong number of objects");
			m.Löschen(r);
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(0, m.Reisebüros.Count, "2. Wrong number of objects");
			pm.Save();
			Assert.AreEqual(0, m.Reisebüros.Count, "3. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void TestRemoveObjectAbort() {
			StandardSetup();
			pm.MakePersistent(m);
			m.Hinzufuegen(r);
			pm.Save();
			Assert.AreEqual(1, m.Reisebüros.Count, "1. Wrong number of objects");
			m.Löschen(r);
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(0, m.Reisebüros.Count, "2. Wrong number of objects");
			pm.Abort();
			Assert.AreEqual(1, m.Reisebüros.Count, "3. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestDeleteSave() {
			StandardSetup();
			pm.MakePersistent(m);
			m.Hinzufuegen(r);
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void TestDeleteAbort() {
			StandardSetup();
			pm.MakePersistent(m);
			m.Hinzufuegen(r);
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveSave() {
			StandardSetup();
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			m.Löschen(r);
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
			Assert.AreEqual(0, m.Reisebüros.Count, "3. Wrong number of objects");
		}

		[Test]
		public void TestAddRemoveAbort() {
			StandardSetup();
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			m.Löschen(r);
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2. Wrong state");
			Assert.AreEqual(0, m.Reisebüros.Count, "3. Wrong number of objects");
		}

		[Test]
		public void TestClearRelatedObjectsSave() {
			for(int i = 0; i < 10; i++) {
				Reisebüro rb = CreateReisebüro(i.ToString());
				pm.MakePersistent(rb);
				m.Hinzufuegen(rb);
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Reisebüros);
			m.LöscheReisebüros();
			Assert.AreEqual(0, m.Reisebüros.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Persistent, ((Reisebüro)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.AreEqual(0, m.Reisebüros.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Persistent, ((Reisebüro)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestClearRelatedObjectsAbort() {
			for(int i = 0; i < 10; i++) {
				Reisebüro rb = CreateReisebüro(i.ToString());
				pm.MakePersistent(rb);
				m.Hinzufuegen(rb);
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Reisebüros);
			m.LöscheReisebüros();
			Assert.AreEqual(0, m.Reisebüros.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Persistent, ((Reisebüro)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.AreEqual(10, m.Reisebüros.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Persistent, ((Reisebüro)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}


		[Test]
		public void TestHollow() {
			StandardSetup();
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1: Reisebüro should be persistent");
			IList Reisebüro = m.Reisebüros;

			pm.MakeHollow(m, true);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "2: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, r.NDOObjectState, "2: Reisebüro should be hollow");

			Reisebüro = m.Reisebüros;
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "3: Mitarbeiter should be persistent");
			Assert.AreEqual(NDOObjectState.Hollow, r.NDOObjectState, "3: Reisebüro should be hollow");
			Assert.AreEqual("Lufthansa City Center", r.Name, "3: Reisebüro should have correct Name");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "4: Reisebüro should be persistent");
		}

		[Test]
		public void  TestMakeAllHollow() {
			StandardSetup();
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, r.NDOObjectState, "1: Reisebüro should be hollow");
		}

		[Test]
		public void  TestMakeAllHollowUnsaved() {
			StandardSetup();
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "1: Mitarbeiter should be created");
			Assert.AreEqual(NDOObjectState.Hollow, r.NDOObjectState, "1: Reisebüro should be Persistent");
		}

		[Test]
		public void TestLoadRelatedObjects() {
			for(int i = 0; i < 10; i++) {
				Reisebüro rb = CreateReisebüro(i.ToString());
				pm.MakePersistent(rb);
				m.Hinzufuegen(rb);
			}
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m, true);

			IList Reisebüros = new ArrayList(m.Reisebüros);
			Assert.AreEqual(10, Reisebüros.Count, "List size should be 10");

			for(int i = 0; i < 10; i++) {
				Reisebüro rr = (Reisebüro)Reisebüros[i];
				Assert.AreEqual(NDOObjectState.Hollow, rr.NDOObjectState, "1: Reisebüro should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), rr.Name, "2: Reisebüro should be in right order");
#endif
				}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Reisebüros2 = m.Reisebüros;
			for(int i = 0; i < 10; i++) {
				Reisebüro r1 = (Reisebüro)Reisebüros[i];
				Reisebüro r2 = (Reisebüro)Reisebüros2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), r1.Name, "3: Reisebüro should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestLoadRelatedObjectsSave() {
			pm.MakePersistent(m);
			pm.Save();
			for(int i = 0; i < 10; i++) {
				Reisebüro rb = CreateReisebüro(i.ToString());
				pm.MakePersistent(rb);
				m.Hinzufuegen(rb);
			}
			pm.Save();
			pm.MakeHollow(m, true);

			IList Reisebüros = new ArrayList(m.Reisebüros);

			for(int i = 0; i < 10; i++) {
				Reisebüro rr = (Reisebüro)Reisebüros[i];
				Assert.AreEqual(NDOObjectState.Hollow, rr.NDOObjectState, "1: Reisebüro should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), rr.Name, "2: Reisebüro should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Reisebüros2 = m.Reisebüros;
			for(int i = 0; i < 10; i++) {
				Reisebüro r1 = (Reisebüro)Reisebüros[i];
				Reisebüro r2 = (Reisebüro)Reisebüros2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), r1.Name, "3: Reisebüro should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestExtentRelatedObjects() {
			StandardSetup();
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.NotNull(m.Reisebüros, "2. Relation is missing");
			Assert.AreEqual(1, m.Reisebüros.Count, "3. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Persistent, ((Reisebüro)m.Reisebüros[0]).NDOObjectState, "4.: Reisebüro should be persistent");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "5: Mitarbeiter should be hollow");
			Assert.NotNull(m.Reisebüros, "6. Relation is missing");
			Assert.AreEqual(1, m.Reisebüros.Count, "7. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Hollow, ((Reisebüro)m.Reisebüros[0]).NDOObjectState, "8.: Reisebüro should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter), false);
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "9: Mitarbeiter should be persistent");
			Assert.NotNull(m.Reisebüros, "10. Relation is missing");
			Assert.AreEqual(1, m.Reisebüros.Count, "11. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Hollow, ((Reisebüro)m.Reisebüros[0]).NDOObjectState, "12.: Reisebüro should be hollow");
		}

		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) {
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		private Reisebüro CreateReisebüro(string name) {
			Reisebüro r = new Reisebüro();
			r.Name = name;
			return r;
		}

		public void StandardSetup() {
			pm.MakePersistent(r);
			pm.Save();
		}
	}
}
