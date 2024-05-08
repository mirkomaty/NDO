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
using System.Diagnostics;
using System.Collections;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using Reisekosten;
using Reisekosten.Personal;
using PureBusinessClasses;
using NDO.Query;
using System.Linq;
using System.Collections.Generic;

namespace NdoUnitTests {
	/// <summary>
	/// Polymorphic tests.
	/// </summary>
	[TestFixture]
	public class PolymorphicWTable {
		public PolymorphicWTable() {
		}

		private PersistenceManager pm;
		private Reise r;
		private Kostenpunkt kp;
		private ICollection onSavingCollection = null;

		[SetUp]
		public void Setup() {
			pm = PmFactory.NewPersistenceManager();
			r = CreateReise("ADC");
			kp = CreateBeleg();
		}

		[TearDown]
		public void TearDown() {
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.None;

			//pm.UnloadCache();
			IList reiseliste = pm.GetClassExtent(typeof(Reise), true);
			pm.Delete(reiseliste);
			pm.Save();
			pm.UnloadCache();
			IQuery q;
			IList liste;

			// We can't delete orphan Beleg object with NDO, so use direct Sql statements
			using (var handler = pm.GetSqlPassThroughHandler())
			{
				var sql = "DELETE FROM " + pm.NDOMapping.FindClass( typeof( Beleg ) ).TableName;
				handler.Execute( sql );
			}

			q = new NDOQuery<Contact>(pm);
			liste = q.Execute();
			pm.Delete(liste);
			pm.Save();
			q = new NDOQuery<Adresse>(pm);
			liste = q.Execute();
			pm.Delete(liste);
			pm.Save();
			pm.Dispose();
		}

		[Test]
		public void TestCreateObjects() {
			pm.MakePersistent(r);
			r.AddKostenpunkt(kp);
			Assert.That(NDOObjectState.Created ==  kp.NDOObjectState, "Beleg should be Created: ");
		}


		[Test]
		public void TestCreateObjects2() {
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			Assert.That(NDOObjectState.Created ==  kp.NDOObjectState, "Beleg should be Created: ");
		}

		[Test]
		public void TestCreateObjectsSave() {
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.Save();

			pm.UnloadCache();
			r = (Reise)pm.FindObject(r.NDOObjectId);
			Assert.That(r != null, "Reise not found");
			Assert.That(1 ==  r.Kostenpunkte.Count );
			Assert.That(r.Kostenpunkte[0] != null, "Beleg not found");
		}

		[Test]
		public void TestCreateManyObjectsSave() {
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.Save();

			r.AddKostenpunkt(CreateBeleg(50));
			r.AddKostenpunkt(CreateBeleg(50));
			pm.Save();
			pm.UnloadCache();
			r = (Reise)pm.FindObject(r.NDOObjectId);
			Assert.That(r != null, "Reise not found");
			Assert.That(3 ==  r.Kostenpunkte.Count, "Anzahl Belege: " );
			Assert.That(r.Kostenpunkte[0] != null, "Beleg not found");
			Assert.That(300 ==  r.Gesamtkosten, "Gesamtkosten: ");
		}
		[Test]
		public void TestCreatePolymorphicObjectsSave() {
//			Debug.WriteLine("TestCreatePolymorphicObjectsSave");
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.Save();

			r.AddKostenpunkt(CreatePkw(100));
			r.AddKostenpunkt(CreateBeleg(50));
			pm.Save();
			pm.UnloadCache();
			r = (Reise)pm.FindObject(r.NDOObjectId);
			Assert.That(r != null, "Reise not found");
			Assert.That(r.Kostenpunkte[0] != null, "Beleg not found");
			Assert.That(3 ==  r.Kostenpunkte.Count, "Anzahl Belege: ");
			Assert.That(300 ==  r.Gesamtkosten, "Gesamtkosten: ");
			Assert.That(typeof(Beleg) ==  r.Kostenpunkte[0].GetType(), "Type");
			Assert.That(typeof(PKWFahrt) ==  r.Kostenpunkte[1].GetType(), "Type");
			Assert.That(typeof(Beleg) ==  r.Kostenpunkte[2].GetType(), "Type");

		}

		[Test]
		public void TestPolymorphicQuery() 
		{
//			Debug.WriteLine("TestPolymorphicQuery");
			r.AddKostenpunkt(kp);
			r.AddKostenpunkt(CreatePkw(100));
			r.AddKostenpunkt(CreateBeleg(50));
			pm.MakePersistent(r);
			pm.Save();
			pm.UnloadCache();

			IQuery q = new NDOQuery<Kostenpunkt>(pm, null);
			IList l = q.Execute();
			Assert.That(3 ==  l.Count, "Anzahl Belege: ");
		}


		[Test]
		public void TestPolymorphicAggregateQuery() 
		{
//			Debug.WriteLine("TestPolymorphicAggregateQuery");
			r.AddKostenpunkt(kp);
			r.AddKostenpunkt(CreatePkw(100));
			r.AddKostenpunkt(CreateBeleg(50));
			pm.MakePersistent(r);
			if (r.NDOObjectId.Id[0] is Int32)
			{
				pm.Save();
				decimal sum = 0m;
				foreach(Kostenpunkt kp2 in r.Kostenpunkte)
				{
					sum += (Int32)kp2.NDOObjectId.Id[0];
				}

				IQuery q = new NDOQuery<Kostenpunkt>(pm, null);
				decimal newsum = (decimal)q.ExecuteAggregate("oid", AggregateType.Sum);
				Assert.That(sum ==  newsum, "Summe stimmt nicht: ");
				decimal newcount = (decimal)q.ExecuteAggregate("oid", AggregateType.Count);
				Assert.That(3 ==  newcount, "Summe stimmt nicht: ");
			}
		}

		[Test]
		public void TestOnSavingEvent() 
		{
//			Debug.WriteLine("TestOnSavingEvent");
			r.AddKostenpunkt(kp);
			r.AddKostenpunkt(CreatePkw(100));
			r.AddKostenpunkt(CreateBeleg(50));
			pm.MakePersistent(r);
			pm.OnSavingEvent += new OnSavingHandler(this.OnSavingListener);
			pm.Save();
			Assert.That(this.onSavingCollection != null, "onSavingCollection is null");
		}

		private void OnSavingListener(ICollection c)
		{
			this.onSavingCollection = c;
		}



		[Ignore("Future Feature")]
		[Test]
		public void TestPolymorphicQueryWithSql() 
		{
			r.AddKostenpunkt(kp);
			r.AddKostenpunkt(CreatePkw(100));
			r.AddKostenpunkt(CreateBeleg(50));
			pm.MakePersistent(r);
			pm.Save();
			pm.UnloadCache();

			string sql = @"SELECT DISTINCT Reise.ID FROM Reise, relBelegKostenpunkt, PKWFahrt, Beleg WHERE ((relBelegKostenpunkt.IDReise = Reise.ID AND relBelegKostenpunkt.TCBeleg = 4) AND PKWFahrt.ID != 0 ) OR  ((relBelegKostenpunkt.IDReise = Reise.ID AND relBelegKostenpunkt.TCBeleg = 3) AND Beleg.ID != 0)";
			//string sql = "select distinct Reise.ID from Reise";
			//string sql = "select * from Reise";

			NDOQuery<Reise> q = new NDOQuery<Reise>(pm, sql, true, QueryLanguage.Sql);
			//System.Diagnostics.Debug.WriteLine(q.GeneratedQuery);

			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Anzahl Reisen: ");
		}

		
		[Test]
		public void TestAddObjectSave() {
			pm.MakePersistent(r);
			pm.Save();
			r.AddKostenpunkt(kp);
			Assert.That(NDOObjectState.Created ==  kp.NDOObjectState, "1. Wrong state");
			pm.Save();
			r = (Reise)pm.FindObject(r.NDOObjectId);
			kp = (Kostenpunkt)pm.FindObject(kp.NDOObjectId);
			Assert.That(r != null, "1. Reise not found");
			Assert.That(kp != null, "2. Beleg not found");
			Assert.That(NDOObjectState.Persistent ==  kp.NDOObjectState, "3. Wrong state");
			Assert.That(DateTime.Now.Date ==  kp.Datum, "Wrong data");
		}
			
		[Test]
		public void TestAddObjectAbort() {
			pm.MakePersistent(r);
			pm.Save();
			r.AddKostenpunkt(kp);
			Assert.That(NDOObjectState.Created ==  kp.NDOObjectState, "1. Wrong state");
			Assert.That(1 ==  r.Kostenpunkte.Count, "1. Wrong number of objects");
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  kp.NDOObjectState, "2. Wrong state");
			Assert.That(0 ==  r.Kostenpunkte.Count, "2. Wrong number of objects");
		}
		[Test]
		public void TestRemoveObjectSave() {
			pm.MakePersistent(r);
			r.AddKostenpunkt(kp);
			pm.Save();
			Assert.That(1 ==  r.Kostenpunkte.Count, "1. Wrong number of objects");
			r.Löschen(kp);
			Assert.That(NDOObjectState.Deleted ==  kp.NDOObjectState, "1. Wrong state");
			Assert.That(0 ==  r.Kostenpunkte.Count, "2. Wrong number of objects");
			pm.Save();
			Assert.That(0 ==  r.Kostenpunkte.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Transient ==  kp.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void TestRemoveObjectAbort() {
			pm.MakePersistent(r);
			r.AddKostenpunkt(kp);
			pm.Save();
			Assert.That(1 ==  r.Kostenpunkte.Count, "1. Wrong number of objects");
			r.Löschen(kp);
			Assert.That(NDOObjectState.Deleted ==  kp.NDOObjectState, "1. Wrong state");
			Assert.That(0 ==  r.Kostenpunkte.Count, "2. Wrong number of objects");
			pm.Abort();
			Assert.That(1 ==  r.Kostenpunkte.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Persistent ==  kp.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestDeleteSave() {
			pm.MakePersistent(r);
			r.AddKostenpunkt(kp);
			pm.Save();
			pm.Delete(r);
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Deleted ==  kp.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Transient ==  kp.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void TestDeleteAbort() {
			pm.MakePersistent(r);
			r.AddKostenpunkt(kp);
			pm.Save();
			pm.Delete(r);
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Deleted ==  kp.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Persistent ==  kp.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveSave() {
			pm.MakePersistent(r);
			pm.Save();
			r.AddKostenpunkt(kp);
			r.Löschen(kp);
			Assert.That(NDOObjectState.Transient ==  kp.NDOObjectState, "1. Wrong state");
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  kp.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveAbort() {
			pm.MakePersistent(r);
			pm.Save();
			r.AddKostenpunkt(kp);
			r.Löschen(kp);
			Assert.That(NDOObjectState.Transient ==  kp.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  kp.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestClearRelatedObjectsSave() {
			for(int i = 0; i < 10; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.MakePersistent(r);
			pm.Save();
			IList rr = r.Kostenpunkte.ToList();
			r.LöscheKostenpunkte();
			Assert.That(0 ==  r.Kostenpunkte.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Kostenpunkt)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.That(0 ==  r.Kostenpunkte.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Transient ==  ((Kostenpunkt)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestClearRelatedObjectsAbort() {
			for(int i = 0; i < 10; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.MakePersistent(r);
			pm.Save();
            IList rr = r.Kostenpunkte.ToList();
            r.LöscheKostenpunkte();
			Assert.That(0 ==  r.Kostenpunkte.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Kostenpunkt)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.That(10 ==  r.Kostenpunkte.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Persistent ==  ((Kostenpunkt)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsNullSave() {
			for(int i = 0; i < 3; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.MakePersistent(r);
			pm.Save();
            IList rr = r.Kostenpunkte.ToList();
            r.ErsetzeKostenpunkte();
			Assert.That(r.Kostenpunkte == null, "No objects should be there");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Kostenpunkt)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.That(r.Kostenpunkte == null, "No objects should be there");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Transient ==  ((Kostenpunkt)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsNullAbort() {
			for(int i = 0; i < 3; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.MakePersistent(r);
			pm.Save();
            IList rr = r.Kostenpunkte.ToList();
            r.ErsetzeKostenpunkte();
			Assert.That(r.Kostenpunkte == null, "No objects should be there");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Kostenpunkt)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.That(3 ==  r.Kostenpunkte.Count, "3. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Persistent ==  ((Kostenpunkt)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsSave() {
			for(int i = 0; i < 3; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.MakePersistent(r);
			pm.Save();
			List<Kostenpunkt> neueKostenpunkte = new List<Kostenpunkt>();
			Kostenpunkt nr = CreateBeleg(50);
			neueKostenpunkte.Add(nr);

            IList rr = r.Kostenpunkte.ToList();
            r.ErsetzeKostenpunkte(neueKostenpunkte);
			Assert.That(1 ==  r.Kostenpunkte.Count, "1. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Kostenpunkt)rr[i]).NDOObjectState, "2. Wrong state");
			}
			Assert.That(NDOObjectState.Created ==  nr.NDOObjectState, "3. Wrong state");

			pm.Save();
			Assert.That(1 ==  r.Kostenpunkte.Count, "4. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Transient ==  ((Kostenpunkt)rr[i]).NDOObjectState, "5. Wrong state");
			}
			Assert.That(NDOObjectState.Persistent ==  nr.NDOObjectState, "6. Wrong state");
		}

		[Test]
		public void TestAssignRelatedObjectsAbort() {
			for(int i = 0; i < 3; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.MakePersistent(r);
			pm.Save();
            List<Kostenpunkt> neueKostenpunkte = new List<Kostenpunkt>();
            Kostenpunkt nr = CreateBeleg(200);
            neueKostenpunkte.Add(nr);

            IList rr = r.Kostenpunkte.ToList();
			r.ErsetzeKostenpunkte(neueKostenpunkte);
			Assert.That(1 ==  r.Kostenpunkte.Count, "1. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Kostenpunkt)rr[i]).NDOObjectState, "2. Wrong state");
			}
			Assert.That(NDOObjectState.Created ==  nr.NDOObjectState, "3. Wrong state");

			pm.Abort();
			Assert.That(3 ==  r.Kostenpunkte.Count, "4. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Persistent ==  ((Kostenpunkt)rr[i]).NDOObjectState, "5. Wrong state");
			}
			Assert.That(NDOObjectState.Transient ==  nr.NDOObjectState, "6. Wrong state");
		}


		[Test]
		public void TestHollow() {
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.Save();
			pm.MakeHollow(r);
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "1: Reise should be hollow");
			Assert.That(NDOObjectState.Persistent ==  kp.NDOObjectState, "1: Kostenpunkt should be persistent");
			var kostenpunkte = r.Kostenpunkte;

			pm.MakeHollow(r, true);
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "2: Reise should be hollow");
			Assert.That(NDOObjectState.Hollow ==  kp.NDOObjectState, "2: Kostenpunkt should be hollow");

			kostenpunkte = r.Kostenpunkte;
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "3: Reise should be persistent");
			Assert.That(NDOObjectState.Hollow ==  kp.NDOObjectState, "3: Kostenpunkt should be hollow");
			Assert.That(200 ==  kp.Kosten, "3: Kostenpunkt should have correct Kosten");
			Assert.That(NDOObjectState.Persistent ==  kp.NDOObjectState, "4: Kostenpunkt should be persistent");
		}

		[Test]
		public void  TestMakeAllHollow() {
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "1: Reise should be hollow");
			Assert.That(NDOObjectState.Hollow ==  kp.NDOObjectState, "1: Kostenpunkt should be hollow");
		}

		[Test]
		public void  TestMakeAllHollowUnsaved() {
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  r.NDOObjectState, "1: Reise should be created");
			Assert.That(NDOObjectState.Created ==  kp.NDOObjectState, "1: Kostenpunkt should be created");
		}

		[Test]
		public void TestLoadRelatedObjects() {
			for(int i = 0; i < 10; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.MakePersistent(r);
			pm.Save();
			pm.MakeHollow(r, true);

			var kpunkte = r.Kostenpunkte.ToList();
			Assert.That(10 ==  kpunkte.Count, "Array size should be 10");

			for(int i = 0; i < 10; i++) {
				Kostenpunkt rr = (Kostenpunkt)kpunkte[i];
				Assert.That(NDOObjectState.Hollow ==  rr.NDOObjectState, "1: Kostenpunkt should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i*10+100 ==  rr.Kosten, "2: Kostenpunkt should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			var kpunkte2 = r.Kostenpunkte.ToList();
			for(int i = 0; i < 10; i++) {
				Kostenpunkt r1 = (Kostenpunkt)kpunkte[i];
				Kostenpunkt r2 = (Kostenpunkt)kpunkte2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i*10+100 ==  r1.Kosten, "3: Kostenpunkt should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestLoadRelatedObjectsSave() {
			pm.MakePersistent(r);
			pm.Save();
			for(int i = 0; i < 10; i++) {
				r.AddKostenpunkt(CreateBeleg(i*10+100));
			}
			pm.Save();
			pm.MakeHollow(r, true);

			IList reisen = r.Kostenpunkte.ToList();

			for(int i = 0; i < 10; i++) {
				Kostenpunkt rr = (Kostenpunkt)reisen[i];
				Assert.That(NDOObjectState.Hollow ==  rr.NDOObjectState, "1: Kostenpunkt should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i*10+100 ==  rr.Kosten, "2: Kostenpunkt should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList reisen2 = r.Kostenpunkte.ToList();
			for(int i = 0; i < 10; i++) {
				Kostenpunkt r1 = (Kostenpunkt)reisen[i];
				Kostenpunkt r2 = (Kostenpunkt)reisen2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i*10+100 ==  r1.Kosten, "3: Kostenpunkt should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestExtentRelatedObjects() {
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "0: Reise should be persistent");
			IList liste = pm.GetClassExtent(typeof(Reise));
			r = (Reise)liste[0];
			Assert.That(1 ==  liste.Count, "1: Number of Reise objects is wrong");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "1: Reise should be persistent");
			Assert.That(r.Kostenpunkte != null, "2. Relation is missing");
			Assert.That(1 ==  r.Kostenpunkte.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Persistent ==  ((Kostenpunkt)r.Kostenpunkte[0]).NDOObjectState, "4.: Kostenpunkt should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Reise));
			r = (Reise)liste[0];
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "5: Reise should be hollow");
			Assert.That(r.Kostenpunkte != null, "6. Relation is missing");
			Assert.That(1 ==  r.Kostenpunkte.Count, "7. Wrong number of objects");
			Assert.That(NDOObjectState.Hollow ==  ((Kostenpunkt)r.Kostenpunkte[0]).NDOObjectState, "8.: Kostenpunkt should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Reise), false);
			r = (Reise)liste[0];
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "9: Reise should be persistent");
			Assert.That(r.Kostenpunkte != null, "10. Relation is missing");
			Assert.That(1 ==  r.Kostenpunkte.Count, "11. Wrong number of objects");
			Assert.That(NDOObjectState.Hollow ==  ((Kostenpunkt)r.Kostenpunkte[0]).NDOObjectState, "12.: Kostenpunkt should be hollow");
		}

		[Test]
		public void TestJoinQuery()
		{
			Person p = new Person();
			p.FirstName = "Mirko";
			p.LastName = "Matytschak";
			p.Moniker = "Hallo";
			Adresse a1 = new Adresse();
			a1.Lkz = "D";
			a1.Plz = "83646";
			a1.Straße = "Nockhergasse 7";
			a1.Ort = "Bad Tölz";

			Adresse a2 = new Adresse();
			a2.Ort = "Mönchen";
			a2.Plz = "80331";
			a2.Straße = "Isartorplatz 5";
			a2.Lkz = "D";

			Institution i = new Institution();
			i.FirstName = "Schnulli";
			i.LastName = "Wauwau";
			i.Hausmeister = "Möller";

			pm.MakePersistent(a1);
			pm.MakePersistent(a2);
			pm.MakePersistent(p);
			pm.MakePersistent(i);
			i.AddAdresse(a1);
			p.AddAdresse(a1);
			p.AddAdresse(a2);
			pm.Save();
			pm.UnloadCache();

			IQuery q = new NDOQuery<Contact>(pm, $"addresses.plz = '{a2.Plz}'");
			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Wrong number of contacts");
		}

		[Test]
		public void TestPolymorphicCondition() 
		{
			r.AddKostenpunkt(kp);
			pm.MakePersistent(r);
			pm.Save();

			pm.UnloadCache();
			NDOQuery<Reise> q = new NDOQuery<Reise>(pm, "belege.datum" + " = " + "{0}");
			q.Parameters.Add(DateTime.Now.Date);
			r = (Reise)q.ExecuteSingle(true);
			Assert.That(r != null, "Reise not found");
			Assert.That(r.Kostenpunkte.Count > 0, "No Beleg" );
			Assert.That(r.Kostenpunkte[0] != null, "Beleg not found" );
		}

		[Test]
		public void TestSortedQueryAsc() 
		{
			r.AddKostenpunkt(kp);
			DateTime yesterday = DateTime.Today - new TimeSpan(1, 0, 0, 0);
			kp.Datum = yesterday;
			r.AddKostenpunkt(kp = CreatePkw());
			kp.Datum = DateTime.Today;
			pm.MakePersistent(r);
			pm.Save();

			pm.UnloadCache();
			IQuery q = new NDOQuery<Kostenpunkt>(pm);
			q.Orderings.Add(new AscendingOrder("datum"));
			IList l = q.Execute();
			Assert.That(2 ==  l.Count, "Count wrong");
			Assert.That(yesterday == ((Kostenpunkt)l[0]).Datum, "Order wrong");
			Assert.That(DateTime.Today == ((Kostenpunkt)l[1]).Datum, "Wrong object");
		}

		[Test]
		public void TestSortedQueryDesc() 
		{
			r.AddKostenpunkt(kp);
			DateTime yesterday = DateTime.Today - new TimeSpan(1, 0, 0, 0);
			kp.Datum = yesterday;
			r.AddKostenpunkt(kp = CreatePkw());
			kp.Datum = DateTime.Today;
			pm.MakePersistent(r);
			pm.Save();

			pm.UnloadCache();
			IQuery q = new NDOQuery<Kostenpunkt>(pm);
			q.Orderings.Add(new DescendingOrder("datum"));
			IList l = q.Execute();
			//pm.VerboseMode = false;
			Assert.That(2 ==  l.Count, "Count wrong");
			Assert.That(DateTime.Today == ((Kostenpunkt)l[0]).Datum, "Order wrong");
			Assert.That(yesterday == ((Kostenpunkt)l[1]).Datum, "Wrong object");
		}

		[Test]
		public void TestSortedQueryAscOneNull() 
		{
			r.AddKostenpunkt(kp);
			kp.Datum = DateTime.MinValue;  // DbNull
			r.AddKostenpunkt(kp = CreatePkw());
			kp.Datum = DateTime.Today;
			pm.MakePersistent(r);
			pm.Save();

			pm.UnloadCache();
			IQuery q = new NDOQuery<Kostenpunkt>(pm);
			q.Orderings.Add(new AscendingOrder("datum"));
			IList l = q.Execute();
			//pm.VerboseMode = false;
			Assert.That(2 ==  l.Count, "Count wrong");
			Assert.That(DateTime.MinValue == ((Kostenpunkt)l[0]).Datum, "Order wrong");
			Assert.That(DateTime.Today == ((Kostenpunkt)l[1]).Datum, "Wrong object");
		}

		[Test]
		public void TestSortedQueryBothNull() 
		{
			r.AddKostenpunkt(kp);
			kp.Datum = DateTime.MinValue;  // DbNull
			r.AddKostenpunkt(kp = CreatePkw());
			kp.Datum = DateTime.MinValue;  // DbNull
			pm.MakePersistent(r);
			pm.Save();

			pm.UnloadCache();
			IQuery q = new NDOQuery<Kostenpunkt>(pm);
			q.Orderings.Add(new AscendingOrder("datum"));
			IList l = q.Execute();
			//pm.VerboseMode = false;
			Assert.That(2 ==  l.Count, "Count wrong");
			Assert.That(DateTime.MinValue == ((Kostenpunkt)l[0]).Datum, "Wrong Date #1");
			Assert.That(DateTime.MinValue == ((Kostenpunkt)l[1]).Datum, "Wrong Date #2");
		}

		[Test]
		public void TestSortedQueryDescOneNull() 
		{
			r.AddKostenpunkt(kp);
			kp.Datum = DateTime.MinValue;
			r.AddKostenpunkt(kp = CreatePkw());
			kp.Datum = DateTime.Today;
			pm.MakePersistent(r);
			pm.Save();

			pm.UnloadCache();
			IQuery q = new NDOQuery<Kostenpunkt>(pm);
			q.Orderings.Add(new DescendingOrder("datum"));
			IList l = q.Execute();
			//pm.VerboseMode = false;
			Assert.That(2 ==  l.Count, "Count wrong");
			Assert.That(DateTime.Today == ((Kostenpunkt)l[0]).Datum, "Order wrong");
			Assert.That(DateTime.MinValue == ((Kostenpunkt)l[1]).Datum, "Wrong object");
		}


		[Test]
		public void TestQueryForSingleObject()
		{
			Kostenpunkt kp2 = CreatePkw(100);
			r.AddKostenpunkt(kp);
			r.AddKostenpunkt(kp2);
			pm.MakePersistent(r);
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Reise> q = new NDOQuery<Reise>(pm, "belege.datum" + " = " + "{0}");
			q.Parameters.Add(DateTime.Now.Date);
			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Count wrong");
		}


		[Test]
		public void TestPolyDelete()
		{
			Kostenpunkt kp2 = CreatePkw(100);
			r.AddKostenpunkt(kp);
			r.AddKostenpunkt(kp2);
			pm.MakePersistent(r);
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Reise> q = new NDOQuery<Reise>(pm, "belege.datum" + " = " + "{0}");
			q.Parameters.Add(DateTime.Now.Date);
			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Count wrong");
			r = (Reise) l[0];
			pm.Delete(r);
			pm.Save();
		}


		[Test]
		public void RelationTest()
		{
			NDOMapping mapping = pm.NDOMapping;
			Class reiseClass = mapping.FindClass(typeof(Reise));
			Class pkwClass = mapping.FindClass(typeof(PKWFahrt));
			Assert.That(reiseClass != null, "Mapping for Reise not found");
			Assert.That(pkwClass != null, "Mapping for pkw not found");
			Relation r1 = reiseClass.FindRelation("belege");
			Assert.That(r1 != null, "Relation not found #1");
			Assert.That(r1.ForeignRelation != null, "ForeignRelation of Reise not found");
			Relation r2 = pkwClass.FindRelation("reise");
			Assert.That(r2 != null, "Relation not found #2");
			Assert.That(r2.ForeignRelation != null, "ForeignRelation of PKWFahrt not found");
			Assert.That(r1 == r2.ForeignRelation, "Back relation wrong");
//			Debug.WriteLine(r1.Parent.FullName + "->" + r1.ReferencedTypeName);
//			Debug.WriteLine(r1.ForeignRelation.Parent.FullName + "->" + r1.ForeignRelation.ReferencedTypeName);
//			Debug.WriteLine(r2.Parent.FullName + "->" + r2.ReferencedTypeName);
//			Debug.WriteLine(r2.ForeignRelation.Parent.FullName + "->" + r2.ForeignRelation.ReferencedTypeName);
		}


		private Kostenpunkt CreateBeleg(double betrag) 
		{
			return new Beleg("Taxi", betrag);
		}

		private Kostenpunkt CreateBeleg() {
			return new Beleg("Flug", 200);
		}

		private Kostenpunkt CreatePkw() {
			return CreatePkw(100);
		}

		private Kostenpunkt CreatePkw(double km) {
			return new PKWFahrt(km);
		}

		private Reise CreateReise(string zweck) {
			Reise r = new Reise();
			r.Zweck = zweck;
			return r;
		}
	}
}
