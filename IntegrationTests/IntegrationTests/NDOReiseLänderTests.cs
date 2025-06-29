﻿//
// Copyright (c) 2002-2023 Mirko Matytschak 
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
using NDO.Query;
using Reisekosten;
using Reisekosten.Personal;

namespace NdoUnitTests {
	[TestFixture] 
	public class NDOReiseLänderTests : NDOTest
	{

		public NDOReiseLänderTests() {
		}

		private PersistenceManager pm;
		private Reise r;
		private Mitarbeiter m;
		private Land usa;
		private Land de;

		[SetUp]
		public void Setup() {
			pm = PmFactory.NewPersistenceManager();
			r = CreateReise("ADC");
			m = CreateMitarbeiter();
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
		}

		[TearDown]
		public void TearDown() {
			try {
				pm = PmFactory.NewPersistenceManager();
				pm.TransactionMode = TransactionMode.None;

				IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), false );
				pm.Delete( mitarbeiterListe );
				pm.Save();
				using (var handler = pm.GetSqlPassThroughHandler())
				{
					var sql = $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Reise ) ).TableName}";
					handler.Execute( sql );
					sql = $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Mitarbeiter ) ).TableName}";
					handler.Execute( sql );
					sql = $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Land ) ).TableName}";
					handler.Execute( sql );
				}
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine("Exception in TearDown: " + ex);
			}
		}


		[Test]
		public void EmptyDB() {
            IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), false);
            pm.Delete(mitarbeiterListe);
            pm.Save();
            /*
			IList reiseListe = pm.GetClassExtent(typeof(Reise), true);
			pm.Delete(reiseListe);
			pm.Save();
			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), true);
			pm.Delete(mitarbeiterListe);
			pm.Save();
             * */
		}

		[Test]
		public void TestObjectCreationTransient()
		{
			bool thrown = false;
			try
			{
				r.LandHinzufügen( new Land( "USA" ) );
			}
			catch (Exception ex)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void TestObjectCreation() {
			CreateLänder();
			r.LandHinzufügen(de);
			Assert.That(1 ==  r.Länder.Count, "Number of Länder");
			Assert.That(NDOObjectState.Created ==  r.NDOObjectState, "Status wrong");
			Land l = (Land)r.Länder[0];
			Assert.That(NDOObjectState.Created ==  l.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestObjectCreationSave() {
			CreateLänder();
			r.LandHinzufügen(de);
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Status wrong");
			Assert.That(NDOObjectState.Persistent ==  de.NDOObjectState, "Status wrong");
			Assert.That(NDOObjectState.Persistent ==  usa.NDOObjectState, "Status wrong");
			Assert.That(1 ==  r.Länder.Count, "Number of Länder");
		}

		[Test]
		public void TestObjectCreationSaveMultiple() {
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandHinzufügen(usa);
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Status wrong");
			Assert.That(NDOObjectState.Persistent ==  de.NDOObjectState, "Status wrong");
			Assert.That(NDOObjectState.Persistent ==  usa.NDOObjectState, "Status wrong");
			Assert.That(2 ==  r.Länder.Count, "Number of Länder");
		}


		[Test]
		public void AssoTestAggregateFunction()
		{
			CreateLänder();
			r.LandHinzufügen(usa);
			pm.Save();
			ObjectId oid = usa.NDOObjectId;
			pm.UnloadCache();
			NDOQuery<Reise> q = new NDOQuery<Reise>(pm, "dieLaender.oid = {0}");
			q.Parameters.Add(oid.Id[0]);
			decimal count = (decimal) q.ExecuteAggregate("*", AggregateType.Count);
			Assert.That(count > 0m, "Count should be > 0");
			usa = (Land) pm.FindObject(oid);
			Assert.That(usa != null, "USA nicht gefunden");
			r.LandLöschen(usa.Name);
			pm.Save();
			pm.UnloadCache();
			count = (decimal) q.ExecuteAggregate("*", AggregateType.Count);
			Assert.That(0m ==  count, "Count should be 0");
		}

		[Test]
		public void TestObjectCreationSaveChanged() {
			CreateLänder();
			r.LandHinzufügen(de);
			de.Name = "Deutschland";
			pm.Save();
			Assert.That("Deutschland" ==  de.Name, "Name wrong");
		}

		[Test]
		public void TestObjectCreationLandSavedFirst() {
			CreateLänder();
			pm.Save();
			r.LandHinzufügen(de);
			de.Name = "Deutschland";
			pm.Save();
			Assert.That("Deutschland" ==  de.Name, "Name wrong");
			Assert.That(1 ==  r.Länder.Count, "Number of Länder");
		}

		[Test]
		public void TestObjectCreationAbort() {
			CreateLänder();
			r.LandHinzufügen(de);
			pm.Abort();
			Assert.That(de.NDOObjectId == null, "Transient object shouldn't have ID");
			Assert.That(((IPersistenceCapable)de).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  de.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestCreateDeleteTransitionSave() {
			CreateLänder();
			r.LandHinzufügen(de);
			m.LöscheReisen();
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Reise should be transient");
			Assert.That(r.NDOObjectId == null, "Transient object shouldn't have ID");
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Status wrong");
			pm.Save();
			Assert.That(r.NDOObjectId == null, "Transient object shouldn't have ID");
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Status wrong");
		}

		[Test]
		public void TestRelatedDelete()
		{
			CreateLänder();
			r.LandHinzufügen(de);
			pm.Save();
			Assert.That(1 ==  de.DieReisen.Count, "The Reise should still be in the container");
			pm.Delete(m);
			pm.Save();
//			Assert.That(1 ==  de.DieReisen.Count, "The Reise should still be in the container");
//			Assert.That(NDOObjectState.Transient ==  ((IPersistenceCapable)de.DieReisen[0]).NDOObjectState, "Wrong object state");

			//pm.MakeHollow(de);
			IList l = pm.GetClassExtent(typeof(Reise), true);
			Assert.That(0 ==  l.Count, "l should be empty");
			l = pm.GetClassExtent(typeof(Land), true);
			foreach(Land land in l)
			{
				string z;
				if (land.DieReisen.Count > 0)
					z = ((Reise)land.DieReisen[0]).Zweck;
				Assert.That(0 ==  land.DieReisen.Count, "DieReisen should be empty");
			}
		}

		[Test]
		public void TestUnloadCache() {
			CreateLänder();
			r.LandHinzufügen(de);
			//pm.UnloadCache();
			Assert.That(Object.ReferenceEquals(de, pm.FindObject(((Land)r.Länder[0]).NDOObjectId)), "Getting same object twice should return same object");
			pm.Save();
			pm.UnloadCache();
			pm.MakeHollow(r);
			Assert.That(1 ==  r.Länder.Count, "Number of Länder");
			Land l = (Land)r.Länder[0];
			Assert.That(l != de, "Getting same object twice should return different objects");
			Assert.That(de.Name ==  l.Name, "Name should be same");
			ObjectId id = l.NDOObjectId;
			l = null;
			pm.UnloadCache();
			Assert.That(pm.FindObject(id) != null, "Should find object");
			pm.UnloadCache();
			Assert.That(1 ==  r.Länder.Count, "Number of Länder");
		}

		[Test]
		public void TestMakeTransient() {
			CreateLänder();
			r.LandHinzufügen(de);
			pm.Save();
			ObjectId id = r.NDOObjectId;
			pm.MakeTransient(r);
			Assert.That(((IPersistenceCapable)r).NDOStateManager == null, "Transient object shouldn't have state manager");
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Wrong state #1");
			Assert.That(id.IsValid(), "Id should still be valid #1");
			pm.Save();
			Land l = (Land)r.Länder[0];
			Assert.That(de.Name ==  l.Name, "Name is wrong");
			Assert.That(Object.ReferenceEquals(de, l), "Land is wrong");
			pm.MakeHollow(m);  // make sure, Reise is loaded fresh during TearDown
		}

		[Test]
		public void TestMakeTransientMitarbeiterDeleteFails() {
			CreateLänder();
			r.LandHinzufügen(de);
			pm.Save();
			pm.MakeTransient(r);
			try {
				pm.Delete(m);
				Assert.Fail("Mitarbeiter shouldn't be deletable because Reise is Transient");
			} catch (NDOException) {
			}
			//((Reise)m.Reisen[0]).NDOObjectState = NDOObjectState.Persistent;
		}


		[Test]
		public void TestDeleteLand() {
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandLöschen(de.Name);
			pm.Save();
			Assert.That(0 ==  r.Länder.Count, "#1 Number of Länder");
			pm.MakeHollow(r);
			Assert.That(0 ==  r.Länder.Count, "#2 Number of Länder");
		}

		[Test]
		public void TestDeleteLänder() {
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandHinzufügen(usa);
			r.LandLöschen(de.Name);
			Assert.That(1 ==  r.Länder.Count, "#1 Number of Länder");
			pm.Save();
			Assert.That(Object.ReferenceEquals(usa, r.Länder[0]), "Land is wrong");
			Assert.That(1 ==  r.Länder.Count, "#2 Number of Länder");
			pm.MakeHollow(r);
			Assert.That(1 ==  r.Länder.Count, "#3 Number of Länder");
		}

		[Test]
		public void TestDeleteReise() {
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandHinzufügen(usa);
			m.LöscheReisen();
			Assert.That(2 ==  r.Länder.Count, "#1 Number of Länder");
			pm.Save();
			Assert.That(2 ==  r.Länder.Count, "#2 Number of Länder");
		}

		[Test]
		public void TestDeleteReiseSave() 
		{
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandHinzufügen(usa);
			Assert.That(1 ==  usa.DieReisen.Count, "#1 Number of Reisen");
			Assert.That(1 ==  de.DieReisen.Count, "#2 Number of Reisen");
			pm.Save();
			pm.UnloadCache();
			m.LöscheReisen();
			Assert.That(2 ==  r.Länder.Count, "#1 Number of Länder");
			pm.Save();
			Assert.That(2 ==  r.Länder.Count, "#2 Number of Länder");
			pm.UnloadCache();
			IQuery q = new NDOQuery<Land>(pm, "name LIKE 'D%'");
			IList list = q.Execute();
			Assert.That(1 ==  list.Count, "#3 Number of Länder");
			de = (Land) list[0];
			Assert.That(0 ==  de.DieReisen.Count, "#3 Number of Reisen");
		}


		[Test]
		public void TestDeleteLänderSave() {
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandHinzufügen(usa);
			pm.Save();
			r.LandLöschen(de.Name);
			pm.Save();
			Assert.That(1 ==  r.Länder.Count, "#1 Number of Länder");
			pm.MakeHollow(r);
			Assert.That(1 ==  r.Länder.Count, "#2 Number of Länder");
		}

		[Test]
		public void TestDeleteLänderAbort() {
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandHinzufügen(usa);
			pm.Save();
			r.LandLöschen(de.Name);
			pm.Abort();
			Assert.That(2 ==  r.Länder.Count, "#1 Number of Länder");
			pm.MakeHollow(r);
			Assert.That(2 ==  r.Länder.Count, "#2 Number of Länder");
		}

		[Test]
		public void TestMakeHollow() {
			CreateLänder();
			r.LandHinzufügen(de);
			pm.Save();
			pm.MakeHollow(r);
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "Wrong state #1");
			Land l = (Land)r.Länder[0];
			// Should be in Cache
			Assert.That(NDOObjectState.Persistent ==  l.NDOObjectState, "Wrong state #2");
			Assert.That(de.Name ==  l.Name, "Name is wrong");
			Assert.That(Object.ReferenceEquals(de, l), "Land is wrong");
			Assert.That(1 ==  r.Länder.Count, "#1 Number of Länder");
		}


		[Test]
		public void TestRefresh() {
			CreateLänder();
			r.LandHinzufügen(de);
			pm.Save();
			pm.MakeHollow(r);
			pm.Refresh(r);
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "Wrong state #1");
		}

		[Test]
		public void TestJoinQuery()
		{
			CreateLänder();
			r.LandHinzufügen(de);
			r.LandHinzufügen(usa);

			Reise r2 = CreateReise("ADW");
			r2.LandHinzufügen(de);
			m.Hinzufuegen(r2);
			

			pm.Save();

			NDOQuery<Reise> q = new NDOQuery<Reise>(pm, "dieLaender.name = {0}");
			q.Parameters.Add( usa.Name );
			IList l = q.Execute();
			Assert.That(1 ==  l.Count, "Wrong number of travels");
		}

        [Test]
        public void TestIntermediateTableDelete()
        {
            CreateLänder();
            pm.Save();
            r.LandHinzufügen(de);
            r.LandHinzufügen(usa);
            pm.Save();
            r.LandLöschen(de);
            pm.Delete(de);
            pm.Save();
        }

		private Reise CreateReise(string zweck) 
		{
			Reise r = new Reise();	
			r.Zweck = zweck;
			return r;
		}

		private Mitarbeiter CreateMitarbeiter() {
			Mitarbeiter m = new Mitarbeiter();
			m.Nachname  = "Kocher";
			m.Vorname = "Hartmut";
			return m;
		}

		private void CreateLänder() {
			usa = new Land("US");
			pm.MakePersistent(usa);
			de = new Land("DE");
			pm.MakePersistent(de);
		}
	}
}

