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
using System.Collections;
using NDO;
using NUnit.Framework;
using Reisekosten;
using Reisekosten.Personal;

namespace NdoUnitTests {
	/// <summary>
	/// This class contains all tests for 1:n directed relations with an intermediate mapping table and composition.
	/// </summary>
	[TestFixture]
	public class Rel1toNDirectedWTableComposition {
		
		public Rel1toNDirectedWTableComposition() {
		}

		private PersistenceManager pm;
		private Mitarbeiter m;
		private Email e;

		[SetUp]
		public void Setup() {

			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Hartmut", "Kocher");
			e = CreateEmail("hwk@cortex-brainware.de");
		}

		[TearDown]
		public void TearDown() {
			pm.Abort();
			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), false);
			pm.Delete(mitarbeiterListe);
			IList eListe = pm.GetClassExtent(typeof(Email), false);
			pm.Delete(eListe);
			pm.Save();
			pm.Close();
			pm = null;
		}

		[Test]
		public void TestCreateObjects() {
			pm.MakePersistent(e);
			if (!pm.HasOwnerCreatedIds && e.NDOObjectId.Id[0] is Int32)
                Assert.AreEqual(-1, e.NDOObjectId.Id[0], "Email key wrong");
			Email r2 = (Email)pm.FindObject(e.NDOObjectId);
			Assert.AreSame(e, r2, "Emails should match");
		}

		[Test]
		public void TestCreateObjectsTransient1() {
			m.Hinzufuegen(e);  
			pm.MakePersistent(m);
		}

		[Test]
		public void TestCreateObjectsTransient2() {
			pm.MakePersistent(m);
			m.Hinzufuegen(e);  
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void TestCreateObjectsTransient3() {
			pm.MakePersistent(e);
			pm.MakePersistent(m);
			m.Hinzufuegen(e); // Cannot add pers. obj.
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void TestCreateObjectsTransient4() {
			pm.MakePersistent(e);
			m.Hinzufuegen(e); 
			pm.MakePersistent(m);
		}

		[Test]
		public void TestCreateObjectsSave() {
			m.Hinzufuegen(e);
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(e.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			e = (Email)pm.FindObject(e.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(e, "1. Email not found");

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			e = (Email)pm.FindObject(e.NDOObjectId);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(e, "2. Email not found");
		}

		[Test]
		public void TestAddObjectSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(e);
			Assert.AreEqual(NDOObjectState.Created, e.NDOObjectState, "1. Wrong state");
			pm.Save();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			e = (Email)pm.FindObject(e.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(e, "1. Email not found");
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void TestAddObjectAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(e);
			Assert.AreEqual(NDOObjectState.Created, e.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(1, m.Emails.Count, "1. Wrong number of objects");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "2. Wrong state");
			Assert.AreEqual(0, m.Emails.Count, "2. Wrong number of objects");
		}
		[Test]
		public void TestRemoveObjectSave() {
            m.Hinzufuegen(e);
			pm.MakePersistent(m);
			pm.Save();
            Assert.AreEqual(1, m.Emails.Count, "1. Wrong number of objects");
			m.Löschen(e);
            Assert.AreEqual(NDOObjectState.Deleted, e.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(0, m.Emails.Count, "2. Wrong number of objects");
            //pm.LogAdapter = new NDO.Logging.ConsoleLogAdapter();
            //pm.VerboseMode = true;
            pm.Save();
            Assert.AreEqual(0, m.Emails.Count, "3. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void TestRemoveObjectAbort() {
			pm.MakePersistent(m);
			m.Hinzufuegen(e);
			pm.Save();
			Assert.AreEqual(1, m.Emails.Count, "1. Wrong number of objects");
			m.Löschen(e);
			Assert.AreEqual(NDOObjectState.Deleted, e.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(0, m.Emails.Count, "2. Wrong number of objects");
			pm.Abort();
			Assert.AreEqual(1, m.Emails.Count, "3. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestDeleteSave() {
			pm.MakePersistent(m);
			m.Hinzufuegen(e);
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, e.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void TestDeleteAbort() {
			pm.MakePersistent(m);
			m.Hinzufuegen(e);
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, e.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(e);
			m.Löschen(e);
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "1. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "2. Wrong state");
			Assert.AreEqual(0, m.Emails.Count, "3. Wrong number of objects");
		}

		[Test]
		public void TestAddRemoveAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(e);
			m.Löschen(e);
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "2. Wrong state");
			Assert.AreEqual(0, m.Emails.Count, "3. Wrong number of objects");
		}

		[Test]
		public void TestClearRelatedObjectsSave() {
			for(int i = 0; i < 10; i++) {
				Email rb = CreateEmail(i.ToString());
				m.Hinzufuegen(rb);
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Emails);
			m.LöscheEmails();
			Assert.AreEqual(0, m.Emails.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Deleted, ((Email)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.AreEqual(0, m.Emails.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Transient, ((Email)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestClearRelatedObjectsAbort() {
			for(int i = 0; i < 10; i++) {
				Email rb = CreateEmail(i.ToString());
				m.Hinzufuegen(rb);
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Emails);
			m.LöscheEmails();
			Assert.AreEqual(0, m.Emails.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Deleted, ((Email)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.AreEqual(10, m.Emails.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.AreEqual(NDOObjectState.Persistent, ((Email)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}


		[Test]
		public void TestHollow() {
			m.Hinzufuegen(e);
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "1: Email should be persistent");
			IList Email = m.Emails;

			pm.MakeHollow(m, true);
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "2: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "2: Email should be hollow");

			Email = m.Emails;
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "3: Mitarbeiter should be persistent");
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "3: Email should be hollow");
			Assert.AreEqual("hwk@cortex-brainware.de", e.Adresse, "3: Email should have correct Adresse");
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "4: Email should be persistent");
		}

		[Test]
		public void  TestMakeAllHollow() {
			m.Hinzufuegen(e);
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "1: Email should be hollow");
		}

		[Test]
		public void  TestMakeAllHollowUnsaved() {
			m.Hinzufuegen(e);
			pm.MakePersistent(m);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "1: Mitarbeiter should be created");
			Assert.AreEqual(NDOObjectState.Created, e.NDOObjectState, "1: Email should be created");
		}

		[Test]
		public void TestLoadRelatedObjects() {
			for(int i = 0; i < 10; i++) {
				Email rb = CreateEmail(i.ToString());
				m.Hinzufuegen(rb);
			}
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m, true);

			IList Emails = new ArrayList(m.Emails);
			Assert.AreEqual(10, Emails.Count, "List size should be 10");

			for(int i = 0; i < 10; i++) 
			{
				Email rr = (Email)Emails[i];
				Assert.AreEqual(NDOObjectState.Hollow, rr.NDOObjectState, "1: Email should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), rr.Adresse, "2: Email should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Reisebüros2 = m.Emails;
			for(int i = 0; i < 10; i++) {
				Email r1 = (Email)Emails[i];
				Email r2 = (Email)Reisebüros2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), r1.Adresse, "3: Email should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestLoadRelatedObjectsSave() {
			pm.MakePersistent(m);
			pm.Save();
			for(int i = 0; i < 10; i++) {
				Email rb = CreateEmail(i.ToString());
				m.Hinzufuegen(rb);
			}
			pm.Save();
			pm.MakeHollow(m, true);

			IList Emails = new ArrayList(m.Emails);

			for(int i = 0; i < 10; i++) {
				Email rr = (Email)Emails[i];
				Assert.AreEqual(NDOObjectState.Hollow, rr.NDOObjectState, "1: Email should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), rr.Adresse, "2: Email should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Reisebüros2 = m.Emails;
			for(int i = 0; i < 10; i++) {
				Email r1 = (Email)Emails[i];
				Email r2 = (Email)Reisebüros2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.AreEqual(i.ToString(), r1.Adresse, "3: Email should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestExtentRelatedObjects() {
			m.Hinzufuegen(e);
			pm.MakePersistent(m);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.NotNull(m.Emails, "2. Relation is missing");
			Assert.AreEqual(1, m.Emails.Count, "3. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Persistent, ((Email)m.Emails[0]).NDOObjectState, "4.: Email should be persistent");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "5: Mitarbeiter should be hollow");
			Assert.NotNull(m.Emails, "6. Relation is missing");
			Assert.AreEqual(1, m.Emails.Count, "7. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Hollow, ((Email)m.Emails[0]).NDOObjectState, "8.: Email should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter), false);
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "9: Mitarbeiter should be persistent");
			Assert.NotNull(m.Emails, "10. Relation is missing");
			Assert.AreEqual(1, m.Emails.Count, "11. Wrong number of objects");
			Assert.AreEqual(NDOObjectState.Hollow, ((Email)m.Emails[0]).NDOObjectState, "12.: Email should be hollow");
		}

		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) {
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		private Email CreateEmail(string Adresse) {
			Email e = new Email();
			e.Adresse = Adresse;
			return e;
		}
	}
}
