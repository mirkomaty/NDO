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
using System.IO;
using NUnit.Framework;
using Reisekosten.Personal;
using NDO;

namespace NdoUnitTests {
	/// <summary>
	/// All tests for directed 1:1-Relations. Composition and Assoziation
	/// </summary>


	[TestFixture]
	public class Rel1to1Directed {
		public Rel1to1Directed() {
		}

		private PersistenceManager pm;
		private Mitarbeiter m;
		private Adresse a;
		private Buero b;

		[SetUp]
		public void Setup() {
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Mirko", "Matytschak");
			a = CreateAdresse("D", "83646", "Nockhergasse 7", "Bad TÃ¶lz");
			b = CreateBuero("3-0815");
		}

		[TearDown]
		public void TearDown() {

			pm.Abort();
			IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
			pm.Delete( mitarbeiterListe );
			IList bueroListe = pm.GetClassExtent( typeof( Buero ), true );
			pm.Delete( bueroListe );
			IList adressListe = pm.GetClassExtent( typeof( Adresse ), true );
			pm.Delete( adressListe );
			pm.Save();

			pm.Close();
			pm = null;
		}


		[Test]
		public void TestCreateObjects() {
			pm.MakePersistent(m);
			pm.MakePersistent(a);
			pm.MakePersistent(b);
			if (!pm.HasOwnerCreatedIds)
			{
				if (m.NDOObjectId.Id[0] is Int32)
                    Assert.AreEqual(-1, m.NDOObjectId.Id[0], "Mitarbeiter key wrong");
				if (a.NDOObjectId.Id[0] is Int32)
                    Assert.AreEqual(-1, a.NDOObjectId.Id[0], "Adresse key wrong");
                if (b.NDOObjectId.Id[0] is Int32)
                    Assert.AreEqual(-1, b.NDOObjectId.Id[0], "BÃ¼ro key wrong");
			}
			Assert.That(!m.NDOObjectId.Equals(a.NDOObjectId), "Ids should be different m-a");
			Assert.That(!m.NDOObjectId.Equals(b.NDOObjectId), "Ids should be different m-b");
			Assert.That(!a.NDOObjectId.Equals(b.NDOObjectId), "Ids should be different a-b");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			a = (Adresse)pm.FindObject(a.NDOObjectId);
			b = (Buero)pm.FindObject(b.NDOObjectId);
		}

		#region Composition Tests

		[Test]
		public void CompTestCreateObjectsSave() {
			m.Adresse = a;
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(a.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			a = (Adresse)pm.FindObject(a.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(a, "1. Adresse not found");
			ObjectId moid = m.NDOObjectId;
			ObjectId aoid = a.NDOObjectId;
			m = null;
			a = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Adresse a2 = m.Adresse;
			a = (Adresse)pm.FindObject(aoid);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(a, "2. Adresse not found");
			Assert.AreSame(a, a2, "Address should match");
		}

		[Test]
		public void CompTestAddObjectSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.Adresse = a;
			Assert.AreEqual(NDOObjectState.Created, a.NDOObjectState, "1. Wrong state");
			pm.Save();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			a = (Adresse)pm.FindObject(a.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(a, "1. Adresse not found");
			Assert.AreEqual(NDOObjectState.Persistent, a.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void CompTestAddObjectAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.Adresse = a;
			Assert.AreEqual(NDOObjectState.Created, a.NDOObjectState, "1. Wrong state");
			Assert.NotNull(m.Adresse, "1. Adress not found");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, a.NDOObjectState, "2. Wrong state");
			Assert.Null(m.Adresse, "1. Adress should be null");
		}

		[Test]
		public void CompTestRemoveObjectSave() {
			pm.MakePersistent(m);
			m.Adresse = a;
			pm.Save();
			Assert.NotNull(m.Adresse, "1. Adress not found");
			m.Adresse = null;
			Assert.AreEqual(NDOObjectState.Deleted, a.NDOObjectState, "1. Wrong state");
			Assert.Null(m.Adresse, "1. Adress should be null");
			pm.Save();
			Assert.Null(m.Adresse, "2. Adress should be null");
			Assert.AreEqual(NDOObjectState.Transient, a.NDOObjectState, "2. Wrong state");
			ObjectId moid = m.NDOObjectId;
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "3. Mitarbeiter not found");
			Assert.Null(m.Adresse, "3. Adresse should be null");
		}
	
		[Test]
		public void CompTestRemoveObjectAbort() {
			pm.MakePersistent(m);
			m.Adresse = a;
			pm.Save();
			Assert.NotNull(m.Adresse, "1. Adress not found");
			m.Adresse = null;
			Assert.AreEqual(NDOObjectState.Deleted, a.NDOObjectState, "1. Wrong state");
			Assert.Null(m.Adresse, "2. Adress should be null");
			pm.Abort();
			Assert.NotNull(m.Adresse, "2. Adress not found");
			Assert.AreEqual(NDOObjectState.Persistent, a.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void CompTestDeleteSave() {
			pm.MakePersistent(m);
			m.Adresse = a;
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, a.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Transient, a.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void CompTestDeleteAbort() {
			pm.MakePersistent(m);
			m.Adresse = a;
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, a.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, a.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void CompTestAddRemoveSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.Adresse = a;
			m.Adresse = null;
			Assert.AreEqual(NDOObjectState.Transient, a.NDOObjectState, "1. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, a.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void CompTestAddRemoveAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.Adresse = a;
			m.Adresse = null;
			Assert.AreEqual(NDOObjectState.Transient, a.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, a.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void CompTestHollow() {
			m.Adresse = a;
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m); // setzt m.adresse auf null
			
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Persistent, a.NDOObjectState, "1: Adresse should be persistent");

			a = m.Adresse; // ruft LoadData fï¿½r m auf. m.adresse liegt auf dem Cache und ist Persistent
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.AreEqual(NDOObjectState.Persistent, a.NDOObjectState, "2: Adresse should be persistent");
			ObjectId id = m.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			m = (Mitarbeiter) pm.FindObject(id);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "2: Mitarbeiter should be hollow");
			a = m.Adresse;
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "2: Mitarbeiter should be persistent");
			Assert.NotNull(a, "Adress not found");
			Assert.AreEqual(NDOObjectState.Hollow, a.NDOObjectState, "1: Adresse should be hollow");
		}


		[Test]
		public void  CompTestMakeAllHollow() {
			m.Adresse = a;
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, a.NDOObjectState, "1: Adresse should be hollow");
		}

		[Test]
		public void  CompTestMakeAllHollowUnsaved() {
			m.Adresse = a;
			pm.MakePersistent(m);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "1: Mitarbeiter should be created");
			Assert.AreEqual(NDOObjectState.Created, a.NDOObjectState, "1: Adresse should be created");
		}


		[Test]
		public void CompTestExtentRelatedObjects() {
			m.Adresse = a;
			pm.MakePersistent(m);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.NotNull(m.Adresse, "2. Relation is missing");
			Assert.AreEqual(NDOObjectState.Persistent, m.Adresse.NDOObjectState, "4.: Adresse should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "5: Mitarbeiter should be hollow");
			Assert.NotNull(m.Adresse, "6. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, m.Adresse.NDOObjectState, "8.: Adresse should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter), false);
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "9: Mitarbeiter should be persistent");
			Assert.NotNull(m.Adresse, "10. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, m.Adresse.NDOObjectState, "12.: Adresse should be hollow");
		}

		#endregion

		#region Assoziation Tests

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void AssoTestCreateObjectsTransient() {
			m.Zimmer = b;
			pm.MakePersistent(m);
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void AssoTestCreateObjectsTransient2() {
			pm.MakePersistent(m);
			m.Zimmer = b;
		}

		[Test]
		public void AssoTestCreateObjectsSave() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(b.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			b = (Buero)pm.FindObject(b.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(b, "1. BÃ¼ro not found");
			ObjectId moid = m.NDOObjectId;
			ObjectId boid = b.NDOObjectId;
			m = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Buero b2 = m.Zimmer;
			b = (Buero)pm.FindObject(boid);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(b, "2. Adresse not found");
			Assert.AreSame(b, b2, "BÃ¼ro should match");
		}

		[Test]
		public void AssoTestCreateObjectsSave2() {
			pm.MakePersistent(b);
			pm.Save();
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(b.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			b = (Buero)pm.FindObject(b.NDOObjectId);
		}

		[Test]
		public void AssoTestAddObjectSave() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			pm.Save();
			m.Zimmer = b;
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.PersistentDirty, m.NDOObjectState, "2. Wrong state");
			pm.Save();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			b = (Buero)pm.FindObject(b.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(b, "1. BÃ¼ro not found");
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void AssoTestAggregateFunction()
		{
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			m.Zimmer = b;
			pm.Save();
			ObjectId oid = b.NDOObjectId;
			pm.UnloadCache();
			Mitarbeiter.QueryHelper qh = new Mitarbeiter.QueryHelper();
			Query q = pm.NewQuery(typeof(Mitarbeiter), qh.meinBuero.oid + Query.Op.Eq + Query.Placeholder(0));
			q.Parameters.Add(new Query.Parameter(oid.Id[0]));
			decimal count = (decimal) q.ExecuteAggregate(qh.nachname, Query.AggregateType.Count);
			Assert.That(count > 0m, "Count should be > 0");
			m.Zimmer = null;
			pm.Save();
			pm.UnloadCache();
			count = (decimal) q.ExecuteAggregate(qh.nachname, Query.AggregateType.Count);
			Assert.AreEqual(0m, count, "Count should be 0");
		}
			
		[Test]
		public void AssoTestAddObjectAbort() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			pm.Save();
			m.Zimmer = b;
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1. Wrong state");
			Assert.NotNull(m.Zimmer, "1. BÃ¼ro not found");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
			Assert.Null(m.Zimmer, "1. BÃ¼ro should be null");
		}

		[Test]
		public void AssoTestRemoveObjectSave() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			m.Zimmer = b;
			pm.Save();
			Assert.NotNull(m.Zimmer, "1. BÃ¼ro not found");
			m.Zimmer = null;
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1. Wrong state");
			Assert.Null(m.Zimmer, "1. BÃ¼ro should be null");
			pm.Save();
			Assert.Null(m.Zimmer, "2. BÃ¼ro should be null");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
			ObjectId moid = m.NDOObjectId;
			ObjectId boid = b.NDOObjectId;
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "3. Mitarbeiter not found");
			Assert.Null(m.Zimmer, "3. BÃ¼ro should be null");
			b = (Buero)pm.FindObject(boid);
			Assert.NotNull(b, "3. Buero not found");
		}
	
		[Test]
		public void AssoTestRemoveObjectAbort() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			m.Zimmer = b;
			pm.Save();
			Assert.NotNull(m.Zimmer, "1. BÃ¼ro not found");
			m.Zimmer = null;
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1. Wrong state");
			Assert.Null(m.Zimmer, "2. BÃ¼ro should be null");
			pm.Abort();
			Assert.NotNull(m.Zimmer, "2. BÃ¼ro not found");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void AssoTestDeleteSave() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			m.Zimmer = b;
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void AssoTestDeleteAbort() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			m.Zimmer = b;
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void AssoTestAddRemoveSave() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			pm.Save();
			m.Zimmer = b;
			m.Zimmer = null;
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void AssoTestAddRemoveAbort() {
			pm.MakePersistent(b);
			pm.MakePersistent(m);
			pm.Save();
			m.Zimmer = b;
			m.Zimmer = null;
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void AssoTestHollow() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m); // setzt m.Zimmer auf null
			
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1: BÃ¼ro should be persistent");

			b = m.Zimmer; // ruft LoadData fï¿½r m auf. m.Zimmer liegt im Cache und ist Persistent
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2: Adresse should be persistent");
			ObjectId id = m.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			m = (Mitarbeiter) pm.FindObject(id);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "2: Mitarbeiter should be hollow");
			b = m.Zimmer;
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "2: Mitarbeiter should be persistent");
			Assert.NotNull(b, "BÃ¼ro not found");
			Assert.AreEqual(NDOObjectState.Hollow, b.NDOObjectState, "1: BÃ¼ro should be hollow");
		}


		[Test]
		public void  AssoTestMakeAllHollow() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, b.NDOObjectState, "1: BÃ¼ro should be hollow");
		}

		[Test]
		public void  AssoTestMakeAllHollowUnsaved() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "1: Mitarbeiter should be created");
			Assert.AreEqual(NDOObjectState.Created, b.NDOObjectState, "1: BÃ¼ro should be created");
		}

		[Test]
		public void  AssoTestMakeAllHollowUnsaved2() {
			pm.MakePersistent(b);
			pm.Save();
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "1: Mitarbeiter should be created");
			Assert.AreEqual(NDOObjectState.Hollow, b.NDOObjectState, "1: BÃ¼ro should be hollow");
		}

		[Test]
		public void AssoTestExtentRelatedObjects() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.NotNull(m.Zimmer, "2. Relation is missing");
			Assert.AreEqual(NDOObjectState.Persistent, m.Zimmer.NDOObjectState, "4.: BÃ¼ro should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "5: Mitarbeiter should be hollow");
			Assert.NotNull(m.Zimmer, "6. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, m.Zimmer.NDOObjectState, "8.: BÃ¼ro should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter), false);
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "9: Mitarbeiter should be persistent");
			Assert.NotNull(m.Zimmer, "10. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, m.Zimmer.NDOObjectState, "12.: BÃ¼ro should be hollow");
		}
		#endregion

		#region Combined Tests
		[Test]
		public void CombinedTestCreateObjectsSave() {
			pm.MakePersistent(b);
			m.Adresse = a;
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(a.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			a = (Adresse)pm.FindObject(a.NDOObjectId);
			b = (Buero)pm.FindObject(b.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(a, "1. Adresse not found");
			Assert.NotNull(b, "1. BÃ¼ro not found");
			ObjectId moid = m.NDOObjectId;
			ObjectId aoid = a.NDOObjectId;
			ObjectId boid = b.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(m.Adresse, "2. Adresse not found");
			Assert.NotNull(m.Zimmer, "2. BÃ¼ro not found");
		}
		[Test]
		public void TestForeignKeyConstraint()
		{
			pm.MakePersistent(b);
			pm.Save();
			pm.MakePersistent(m);			
			m.Zimmer = b;
			pm.Save();
		}

		[Test]
		public void CombinedTestAddAdresse() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			m.Adresse = a;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(m.Adresse, "2. Adresse not found");
			Assert.NotNull(m.Zimmer, "2. BÃ¼ro not found");
		}

		[Test]
		public void CombinedTestAddAdresseRemoveBÃ¼ro() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			pm.MakePersistent(m);
			pm.Save();
			m.Adresse = a;
			m.Zimmer = null;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			ObjectId boid = b.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.NotNull(m.Adresse, "Adresse not found");
			Assert.Null(m.Zimmer, "Unexpected BÃ¼ro");
			b = (Buero)pm.FindObject(boid);
			Assert.NotNull(b, "BÃ¼ro not found");
		}

		[Test]
		public void CombinedTestAddBÃ¼roRemoveAdresse() {
			pm.MakePersistent(b);
			m.Adresse = a;
			pm.MakePersistent(m);
			pm.Save();
			Adresse adr = a;
			m.Zimmer = b;
			m.Adresse = null;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.NotNull(m.Zimmer, "BÃ¼ro not found");
			Assert.Null(m.Adresse, "Unexpected Adresse");
			Assert.That(adr.NDOObjectState == NDOObjectState.Transient, "Adresse should be deleted");
		}

		[Test]
		public void CombinedTestCreateAddRemoveAdresse() {
			pm.MakePersistent(b);
			m.Adresse = a;
			m.Zimmer = b;
			m.Adresse = null;
			pm.MakePersistent(m);
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.NotNull(m.Zimmer, "BÃ¼ro not found");
			Assert.Null(m.Adresse, "Unexpected Adresse");
		}

		[Test]
		public void CombinedTestCreateAddRemoveBÃ¼ro() {
			pm.MakePersistent(b);
			m.Zimmer = b;
			m.Adresse = a;
			m.Zimmer = null;
			pm.MakePersistent(m);
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.NotNull(m.Adresse, "Adresse not found");
			Assert.Null(m.Zimmer, "Unexpected BÃ¼ro");
		}

		[Test]
		public void CombinedTestAddRemoveBÃ¼ro() {
			pm.MakePersistent(b);
			m.Adresse = a;
			m.Zimmer = null;
			pm.MakePersistent(m);
			pm.Save();
			m.Zimmer = b;
			m.Zimmer = null;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.NotNull(m.Adresse, "Adresse not found");
			Assert.Null(m.Zimmer, "Unexpected BÃ¼ro");
		}

		[Test]
		public void CombinedTestAddBÃ¼roRemoveAdresseAbort() {
			pm.MakePersistent(b);
			m.Adresse = a;
			pm.MakePersistent(m);
			pm.Save();
			ObjectId aoid = a.NDOObjectId;
			m.Zimmer = b;
			m.Adresse = null;
			pm.Abort();
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(m.Adresse, "1. Adresse not found");
			Assert.Null(m.Zimmer, "1. Unexpected BÃ¼ro");

			
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(m.Adresse, "2. Adresse not found");
			Assert.Null(m.Zimmer, "2. Unexpected BÃ¼ro");
		}
		#endregion

		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) {
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		private Adresse CreateAdresse(string lkz, string plz, string straÃŸe, string ort) {
			Adresse a = new Adresse();
			a.Lkz = lkz;
			a.Plz = plz;
			a.StraÃŸe = straÃŸe;
			a.Ort = ort;
			return a;
		}

		private Buero CreateBuero(string zimmer) {
			return new Buero(zimmer);
		}
	}
}
