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
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NDO;
using NDO.Query;
using Reisekosten;
using Reisekosten.Personal;
using NDO.Linq;
using NDO.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace NdoUnitTests
{
	/// <summary>
	/// Combined Tests of Mitarbeiter and Reise to test uniqueness of object ids.
	/// </summary>
	[TestFixture]
	public class NDOMitarbeiterReiseTests : NDOTest
	{
		public NDOMitarbeiterReiseTests()
		{
		}

		private PersistenceManager pm;
		private Mitarbeiter m;
		private Reise r;

		[SetUp]
		public void Setup() {
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Hartmut", "Kocher");
			r = CreateReise("ADC");
		}

		[TearDown]
		public void TearDown() {
			pm.Abort();
			pm.UnloadCache();
			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), true);
			pm.Delete(mitarbeiterListe);
			pm.Save();
			pm.Close();
			pm.GetSqlPassThroughHandler().Execute( "DELETE FROM " + pm.NDOMapping.FindClass( typeof( Reise ) ).TableName );
			pm.Dispose();
		}

		[Test]
		public void TestCreateObjects() {
			pm.MakePersistent(m);
			pm.MakePersistent(r);
			Assert.That(!m.NDOObjectId.Equals(r.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			r = (Reise)pm.FindObject(r.NDOObjectId);
		}

		[Test]
		public void TestCreateObjectsSave() {
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(r.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			r = (Reise)pm.FindObject(r.NDOObjectId);
			Assert.That(m != null, "1. Mitarbeiter not found");
			Assert.That(r != null, "1. Reise not found");

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			r = (Reise)pm.FindObject(r.NDOObjectId);
			Assert.That(m != null, "2. Mitarbeiter not found");
			Assert.That(r != null, "2. Reise not found");
		}

		[Test]
		public void TestAddObjectSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			Assert.That(NDOObjectState.Created ==  r.NDOObjectState, "1. Wrong state");
			pm.Save();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			r = (Reise)pm.FindObject(r.NDOObjectId);
			Assert.That(m != null, "1. Mitarbeiter not found");
			Assert.That(r != null, "1. Reise not found");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "2. Wrong state");
		}

		int GetOrdinal(Relation r)
		{
			var t = r.Parent.SystemType;
			Type mcType = t.GetNestedType( "MetaClass", BindingFlags.NonPublic | BindingFlags.Public );
			if (null == mcType)
				throw new NDOException( 13, "Missing nested class 'MetaClass' for type '" + t.Name + "'; the type doesn't seem to be enhanced." );
			var mc = (IMetaClass2)Activator.CreateInstance( mcType, t );
			return mc.GetRelationOrdinal( r.FieldName );
		}

		bool IsLoaded(IPersistentObject pc, Relation r)
		{
			return ((IPersistenceCapable)pc).NDOLoadState.RelationLoadState[GetOrdinal( r )];
		}

		[Test]
		public void RestorePreservesRelationLists()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Hinzufuegen( r );
			pm.Save();
			Assert.That(1 ==  m.Reisen.Count );

			pm.UnloadCache();
			var m2 = pm.Objects<Mitarbeiter>().Single();

			var relation = pm.NDOMapping.FindClass( typeof( Mitarbeiter ) ).FindRelation( "dieReisen" );

			Assert.That( !IsLoaded( m2, relation ) );
			var list = m2.Reisen;
			m2.Vorname = "Testxxxx";

			Assert.That( IsLoaded( m2, relation ) );

			pm.Restore( m2 );
			m2.Nachname = "Testyyyy";
			pm.Save();


			Assert.That(1 ==  m2.Reisen.Count );
		}

		[Test]
		public void TestAddObjectAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			Assert.That(NDOObjectState.Created ==  r.NDOObjectState, "1. Wrong state");
			Assert.That(1 ==  m.Reisen.Count, "1. Wrong number of objects");
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "2. Wrong state");
			Assert.That(0 ==  m.Reisen.Count, "2. Wrong number of objects");
		}
		[Test]
		public void TestRemoveObjectSave() {
			pm.MakePersistent(m);
			m.Hinzufuegen(r);
			pm.Save();
			Assert.That(1 ==  m.Reisen.Count, "1. Wrong number of objects");
			m.Löschen(r);
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "1. Wrong state");
			Assert.That(0 ==  m.Reisen.Count, "2. Wrong number of objects");
			pm.Save();
			Assert.That(0 ==  m.Reisen.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void TestRemoveObjectAbort() {
			pm.MakePersistent(m);
			m.Hinzufuegen(r);
			pm.Save();
			Assert.That(1 ==  m.Reisen.Count, "1. Wrong number of objects");
			m.Löschen(r);
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "1. Wrong state");
			Assert.That(0 ==  m.Reisen.Count, "2. Wrong number of objects");
			pm.Abort();
			Assert.That(1 ==  m.Reisen.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestDeleteSave() {
			pm.MakePersistent(m);
			m.Hinzufuegen(r);
			pm.Save();
			pm.Delete(m);
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void TestDeleteAbort() {
			pm.MakePersistent(m);
			m.Hinzufuegen(r);
			pm.Save();
			pm.Delete(m);
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Deleted ==  r.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			m.Löschen(r);
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "1. Wrong state");
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.Hinzufuegen(r);
			m.Löschen(r);
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestClearRelatedObjectsSave() {
			for(int i = 0; i < 10; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Reisen);
			m.LöscheReisen();
			Assert.That(0 ==  m.Reisen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Reise)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.That(0 ==  m.Reisen.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Transient ==  ((Reise)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestClearRelatedObjectsAbort() {
			for(int i = 0; i < 10; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Reisen);
			m.LöscheReisen();
			Assert.That(0 ==  m.Reisen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Reise)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.That(10 ==  m.Reisen.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) {
				Assert.That(NDOObjectState.Persistent ==  ((Reise)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsNullSave() {
			for(int i = 0; i < 3; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Reisen);
			m.ErsetzeReisen(null);
			Assert.That(m.Reisen == null, "No objects should be there");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Reise)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.That(m.Reisen == null, "No objects should be there");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Transient ==  ((Reise)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsNullAbort() {
			for(int i = 0; i < 3; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.MakePersistent(m);
			pm.Save();
			IList rr = new ArrayList(m.Reisen);
			m.ErsetzeReisen(null);
			Assert.That(m.Reisen == null, "No objects should be there");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Reise)rr[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.That(3 ==  m.Reisen.Count, "3. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Persistent ==  ((Reise)rr[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsSave() {
			for(int i = 0; i < 3; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.MakePersistent(m);
			pm.Save();
			IList neueReisen = new ArrayList();
			Reise nr = CreateReise("Test");
			neueReisen.Add(nr);

			IList rr = new ArrayList(m.Reisen);
			m.ErsetzeReisen(neueReisen);
			Assert.That(1 ==  m.Reisen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Reise)rr[i]).NDOObjectState, "2. Wrong state");
			}
			Assert.That(NDOObjectState.Created ==  nr.NDOObjectState, "3. Wrong state");

			pm.Save();
			Assert.That(1 ==  m.Reisen.Count, "4. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Transient ==  ((Reise)rr[i]).NDOObjectState, "5. Wrong state");
			}
			Assert.That(NDOObjectState.Persistent ==  nr.NDOObjectState, "6. Wrong state");
		}

		[Test]
		public void TestAssignRelatedObjectsAbort() {
			for(int i = 0; i < 3; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.MakePersistent(m);
			pm.Save();
			IList neueReisen = new ArrayList();
			Reise nr = CreateReise("Test");
			neueReisen.Add(nr);

			IList rr = new ArrayList(m.Reisen);
			m.ErsetzeReisen(neueReisen);
			Assert.That(1 ==  m.Reisen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Deleted ==  ((Reise)rr[i]).NDOObjectState, "2. Wrong state");
			}
			Assert.That(NDOObjectState.Created ==  nr.NDOObjectState, "3. Wrong state");

			pm.Abort();
			Assert.That(3 ==  m.Reisen.Count, "4. Wrong number of objects");
			for(int i = 0; i < 3; i++) {
				Assert.That(NDOObjectState.Persistent ==  ((Reise)rr[i]).NDOObjectState, "5. Wrong state");
			}
			Assert.That(NDOObjectState.Transient ==  nr.NDOObjectState, "6. Wrong state");
		}

		[Test]
		public void TestAddNewReise()
		{
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm);
			m = q.ExecuteSingle(true);
			Reise r2 = CreateReise("Schnulli");
			m.Hinzufuegen(r2);
			pm.Save();
			m = q.ExecuteSingle(true);
			Assert.That(2 ==  m.Reisen.Count, "Count wrong");
		}

		[Test]
		public void RefreshReloadsRelation()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			PersistenceManager pm2 = PmFactory.NewPersistenceManager();
			Mitarbeiter m2 = pm2.Objects<Mitarbeiter>().ResultTable.First();
			Assert.That(m.Reisen.Count ==  m2.Reisen.Count );
			Assert.That(1 ==  m.Reisen.Count );
			Reise r2 = new Reise() { Zweck = "Test" };
			m2.Hinzufuegen( r2 );
			pm2.Save();
			Assert.That(1 ==  m.Reisen.Count );
			pm.Refresh( m );
			Assert.That(2 ==  m.Reisen.Count );
		}

		[Test]
		public void CanIterateThrougRelatedObjectsWithLinq()
		{
			m.Hinzufuegen( r );
			r = new Reise() { Zweck = "Test" };
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			Assert.That(2 ==  m.Reisen.Count );
			ObjectId oid = m.NDOObjectId;
			pm.UnloadCache();
			pm = PmFactory.NewPersistenceManager();
			m = (Mitarbeiter) pm.FindObject( oid );
			Assert.That( m.NDOObjectState == NDOObjectState.Hollow );
			Reise tr = (from reise in m.Reisen where reise.Zweck == "Test" select reise).SingleOrDefault();
			Assert.That(tr  != null);
			Assert.That("Test" ==  tr.Zweck );
		}

		[Test]
		public void TestHollow() {
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m);
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "1: Reise should be persistent");
			IList reise = m.Reisen;

			pm.MakeHollow(m, true);
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "2: Mitarbeiter should be hollow");
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "2: Reise should be hollow");

			reise = m.Reisen;
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "3: Mitarbeiter should be persistent");
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "3: Reise should be hollow");
			Assert.That("ADC" ==  r.Zweck, "3: Reise should have correct Zweck");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "4: Reise should be persistent");
		}

		[Test]
		public void  TestMakeAllHollow() {
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.That(NDOObjectState.Hollow ==  r.NDOObjectState, "1: Reise should be hollow");
			pm.UnloadCache();
		}

		[Test]
		public void  TestMakeAllHollowUnsaved() {
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "1: Mitarbeiter should be created");
			Assert.That(NDOObjectState.Created ==  r.NDOObjectState, "1: Reise should be created");
		}

		[Test]
		public void TestLoadRelatedObjects() {
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null);
			IList dellist = q.Execute();
			pm.Delete(dellist);
			pm.Save();
			pm.UnloadCache();

			for(int i = 0; i < 10; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m, true);

			IList reisen = new ArrayList(m.Reisen);
			Assert.That(10 ==  reisen.Count, "Array size should be 10");

			for(int i = 0; i < 10; i++) {
				Reise rr = (Reise)reisen[i];
				Assert.That(NDOObjectState.Hollow ==  rr.NDOObjectState, "1: Reise should be hollow");
//				if (i.ToString()!= rr.Zweck)
//				{
//					for (int j = 0; j < 10; j++)
//						System.Diagnostics.Debug.WriteLine("Reise: " + ((Reise)reisen[j]).Zweck);
//				}
#if !ORACLE && !MYSQL && !FIREBIRD
				if (rr.NDOObjectId.Id[0] is Int32)
					Assert.That(i.ToString() ==  rr.Zweck, "2: Reise should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList reisen2 = m.Reisen;
			for(int i = 0; i < 10; i++) {
				Reise r1 = (Reise)reisen[i];
				Reise r2 = (Reise)reisen2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				if (r1.NDOObjectId.Id[0] is Int32)
				Assert.That(i.ToString() ==  r1.Zweck, "3: Reise should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestLoadRelatedObjectsSave() {
			pm.MakePersistent(m);
			pm.Save();
			for(int i = 0; i < 10; i++) {
				m.Hinzufuegen(CreateReise(i.ToString()));
			}
			pm.Save();
			pm.MakeHollow(m, true);

			IList reisen = new ArrayList(m.Reisen);

			for(int i = 0; i < 10; i++) {
				Reise rr = (Reise)reisen[i];
				Assert.That(NDOObjectState.Hollow ==  rr.NDOObjectState, "1: Reise should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				if (rr.NDOObjectId.Id[0] is Int32)
					Assert.That(i.ToString() ==  rr.Zweck, "2: Reise should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList reisen2 = m.Reisen;
			for(int i = 0; i < 10; i++) {
				Reise r1 = (Reise)reisen[i];
				Reise r2 = (Reise)reisen2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				if (r1.NDOObjectId.Id[0] is Int32)
				Assert.That(i.ToString() ==  r1.Zweck, "3: Reise should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestExtentRelatedObjects() {
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.That(m.Reisen != null, "2. Relation is missing");
			Assert.That(1 ==  m.Reisen.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Persistent ==  ((Reise)m.Reisen[0]).NDOObjectState, "4.: Reise should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "5: Mitarbeiter should be hollow");
			Assert.That(m.Reisen != null, "6. Relation is missing");
			Assert.That(1 ==  m.Reisen.Count, "7. Wrong number of objects");
			Assert.That(NDOObjectState.Hollow ==  ((Reise)m.Reisen[0]).NDOObjectState, "8.: Reise should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter), false);
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "9: Mitarbeiter should be persistent");
			Assert.That(m.Reisen != null, "10. Relation is missing");
			Assert.That(1 ==  m.Reisen.Count, "11. Wrong number of objects");
			Assert.That(NDOObjectState.Hollow ==  ((Reise)m.Reisen[0]).NDOObjectState, "12.: Reise should be hollow");
		}

		[Test]
		public void TestReiseChangeReload()
		{
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			var provider = pm.NDOMapping.FindClass( typeof( Reise ) ).Provider;
			NDOQuery<Reise> q = new NDOQuery<Reise>(pm, $"zweck LIKE 'A{provider.Wildcard}'");
			r = (Reise) q.ExecuteSingle(true);
			r.Zweck = "NewPurpose";
			pm.Save();
			NDOQuery<Mitarbeiter> qm = new NDOQuery<Mitarbeiter>(pm);
			m = qm.ExecuteSingle(true);
			Assert.That(1 ==  m.Reisen.Count, "Count wrong");
			Assert.That("NewPurpose" ==  r.Zweck, "Reise wrong");
		}

		[Test]
		public void RelationEmptyAssignment()
		{
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			m = (Mitarbeiter) pm.FindObject(m.NDOObjectId);
			//Assert.That(m.Reisen.Count == 1);
			m.ErsetzeReisen(new ArrayList());
			pm.Save();
			pm.UnloadCache();
//			// This code gets the same result
//			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, "oid = {0}");
//			q.Parameters.Add(m.NDOObjectId.Id.Value);
//			m = (Mitarbeiter) q.ExecuteSingle(true);

			m = (Mitarbeiter) pm.FindObject(m.NDOObjectId);
			Assert.That(m.Reisen.Count == 0, "Reisen should be empty");
		}

		[Test]
		public void RelationNullAssignment()
		{
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			m = (Mitarbeiter) pm.FindObject(m.NDOObjectId);
			//Assert.That(m.Reisen.Count == 1);
			m.ErsetzeReisen(null);
			pm.Save();
			pm.UnloadCache();
			m = (Mitarbeiter) pm.FindObject(m.NDOObjectId);
			Assert.That(m.Reisen.Count == 0, "Reisen should be empty");
		}

		[Test]
//		[Ignore("Erzeugt Exception in TearDown")]
		public void AbortedTransaction()
		{
			m.Hinzufuegen( r );
			pm.MakePersistent( m );
			pm.Save();
			pm.UnloadCache();

			pm.TransactionMode = TransactionMode.Optimistic;

			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, null );
			m = (Mitarbeiter)q.ExecuteSingle( true );
			m.Vorname = "Hans";
			((Reise)m.Reisen[0]).Zweck = "Neuer Zweck";

			FieldInfo fi = pm.NDOMapping.GetType().GetField( "persistenceHandler", BindingFlags.Instance | BindingFlags.NonPublic );

			Dictionary<Type, IPersistenceHandler> ht = (Dictionary<Type, IPersistenceHandler>)fi.GetValue( pm.NDOMapping );
			ht.Clear();
			pm.NDOMapping.FindClass( typeof( Reise ) ).FindField( "zweck" ).Column.Name = "nix";

			try
			{
				pm.Save();
			}
			catch
			{
				ht.Clear();
				pm.NDOMapping.FindClass( typeof( Reise ) ).FindField( "zweck" ).Column.Name = "Zweck";
				pm.Abort();
			}

			pm.UnloadCache();
			m = (Mitarbeiter)q.ExecuteSingle( true );
			Assert.That("Hartmut" ==  m.Vorname, "Vorname falsch" );
			Assert.That("ADC" ==  ((Reise)m.Reisen[0]).Zweck, "Vorname falsch" );
		}

		[Test]
		public void ResolveTest()
		{
			// This makes sure, that each resolve delivers a new PersistenceHandler.
			var h1 = Host.Services.GetRequiredService<IPersistenceHandler>();
			var h2 = Host.Services.GetRequiredService<IPersistenceHandler>();
			Assert.That( !object.ReferenceEquals( h1, h2 ) );
		}

		[Test]
		public void TestListEnumerator()
		{
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null);
			m = (Mitarbeiter) q.ExecuteSingle(true);
			IEnumerator ie = m.Reisen.GetEnumerator();
			bool result = ie.MoveNext();
			Assert.That(result, "Enumerator should give a result");
			Assert.That(ie.Current != null);
		}

		[Test]
		public void TestListCount()
		{
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null);
			m = (Mitarbeiter) q.ExecuteSingle(true);
			Assert.That(1 ==  m.Reisen.Count, "Count should be 1");
		}

		[Test]
		public void TestLinqQuery()
		{
			m.Hinzufuegen(r);
			pm.MakePersistent(m);
			pm.Save();
			pm.UnloadCache();
			m = (from mi in pm.Objects<Mitarbeiter>() where mi.Reisen[Any.Index].Zweck == "ADC" select mi).FirstOrDefault();
			Assert.That(m != null, "There should be 1 object");
			Assert.That(1 ==  m.Reisen.Count, "Count should be 1");
		}


		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) 
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		private Reise CreateReise(string zweck) {
			Reise r = new Reise();
			r.Zweck = zweck;
			return r;
		}
	}
}
