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
using NDO.Query;
using NDO.Linq;

namespace NdoUnitTests
{
	/// <summary>
	/// All tests for directed 1:1-Relations. Composition and Assoziation
	/// </summary>


	[TestFixture]
	public class Rel1to1Directed : NDOTest
	{
		public Rel1to1Directed()
		{
		}

		private PersistenceManager pm;
		private Mitarbeiter m;
		private Adresse a;
		private Buero b;

		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter( "Mirko", "Matytschak" );
			a = CreateAdresse( "D", "83646", "Nockhergasse 7", "Bad Tölz" );
			b = CreateBuero( "3-0815" );
		}

		[TearDown]
		public void TearDown()
		{
			pm.Abort();
			IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
			pm.Delete( mitarbeiterListe );
			IList bueroListe = pm.GetClassExtent( typeof( Buero ), true );
			pm.Delete( bueroListe );
			IList adressListe = pm.GetClassExtent( typeof( Adresse ), true );
			pm.Delete( adressListe );
			pm.Save();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			pm.Dispose();
		}


		[Test]
		public void TestCreateObjects()
		{
			pm.MakePersistent( m );
			pm.MakePersistent( a );
			pm.MakePersistent( b );
			if (!pm.HasOwnerCreatedIds)
			{
				if (m.NDOObjectId.Id[0] is Int32)
					Assert.That(-1 == (int) m.NDOObjectId.Id[0], "Mitarbeiter key wrong" );
				if (a.NDOObjectId.Id[0] is Int32)
					Assert.That(-1 == (int) a.NDOObjectId.Id[0], "Adresse key wrong" );
				if (b.NDOObjectId.Id[0] is Int32)
					Assert.That(-1 == (int) b.NDOObjectId.Id[0], "Büro key wrong" );
			}
			Assert.That( !m.NDOObjectId.Equals( a.NDOObjectId ), "Ids should be different m-a" );
			Assert.That( !m.NDOObjectId.Equals( b.NDOObjectId ), "Ids should be different m-b" );
			Assert.That( !a.NDOObjectId.Equals( b.NDOObjectId ), "Ids should be different a-b" );
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			a = (Adresse)pm.FindObject( a.NDOObjectId );
			b = (Buero)pm.FindObject( b.NDOObjectId );
		}

		#region Composition Tests

		[Test]
		public void CompTestCreateObjectsSave()
		{
			m.Adresse = a;
			pm.MakePersistent( m );
			pm.Save();
			Assert.That( !m.NDOObjectId.Equals( a.NDOObjectId ), "Ids should be different" );
			m = pm.Objects<Mitarbeiter>().Where(e=>e.Oid() == m.NDOObjectId.Id.Value ).SingleOrDefault();
			a = (Adresse)pm.FindObject( a.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(a != null, "1. Adresse not found" );
			ObjectId moid = m.NDOObjectId;
			ObjectId aoid = a.NDOObjectId;
			m = null;
			a = null;

			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Where( e=>e.NDOObjectId == moid ).SingleOrDefault();
			Adresse a2 = m.Adresse;
			a = pm.Objects<Adresse>().Where( x=>x.NDOObjectId == aoid ).SingleOrDefault();
			Assert.That(m != null, "2. Mitarbeiter not found" );
			Assert.That(a != null, "2. Adresse not found" );
			Assert.That(Object.ReferenceEquals(a, a2), "Address should match" );
		}

		[Test]
		public void CompTestAddObjectSave()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Adresse = a;
			Assert.That(NDOObjectState.Created ==  a.NDOObjectState, "1. Wrong state" );
			pm.Save();
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			a = (Adresse)pm.FindObject( a.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(a != null, "1. Adresse not found" );
			Assert.That(NDOObjectState.Persistent ==  a.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void CompTestAddObjectAbort()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Adresse = a;
			Assert.That(NDOObjectState.Created ==  a.NDOObjectState, "1. Wrong state" );
			Assert.That(m.Adresse != null, "1. Adress not found" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  a.NDOObjectState, "2. Wrong state" );
			Assert.That(m.Adresse == null, "1. Adress should be null" );
		}

		[Test]
		public void CompTestRemoveObjectSave()
		{
			pm.MakePersistent( m );
			m.Adresse = a;
			pm.Save();
			Assert.That(m.Adresse != null, "1. Adress not found" );
			m.Adresse = null;
			Assert.That(NDOObjectState.Deleted ==  a.NDOObjectState, "1. Wrong state" );
			Assert.That(m.Adresse == null, "1. Adress should be null" );
			pm.Save();
			Assert.That(m.Adresse == null, "2. Adress should be null" );
			Assert.That(NDOObjectState.Transient ==  a.NDOObjectState, "2. Wrong state" );
			ObjectId moid = m.NDOObjectId;
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "3. Mitarbeiter not found" );
			Assert.That(m.Adresse == null, "3. Adresse should be null" );
		}

		[Test]
		public void CompTestRemoveObjectAbort()
		{
			pm.MakePersistent( m );
			m.Adresse = a;
			pm.Save();
			Assert.That(m.Adresse != null, "1. Adress not found" );
			m.Adresse = null;
			Assert.That(NDOObjectState.Deleted ==  a.NDOObjectState, "1. Wrong state" );
			Assert.That(m.Adresse == null, "2. Adress should be null" );
			pm.Abort();
			Assert.That(m.Adresse != null, "2. Adress not found" );
			Assert.That(NDOObjectState.Persistent ==  a.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void CompTestDeleteSave()
		{
			pm.MakePersistent( m );
			m.Adresse = a;
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  a.NDOObjectState, "2. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Transient ==  a.NDOObjectState, "2. Wrong state" );
		}



		[Test]
		public void CompTestDeleteAbort()
		{
			pm.MakePersistent( m );
			m.Adresse = a;
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  a.NDOObjectState, "2. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  a.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void CompTestAddRemoveSave()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Adresse = a;
			m.Adresse = null;
			Assert.That(NDOObjectState.Transient ==  a.NDOObjectState, "1. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  a.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void CompTestAddRemoveAbort()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Adresse = a;
			m.Adresse = null;
			Assert.That(NDOObjectState.Transient ==  a.NDOObjectState, "1. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  a.NDOObjectState, "2. Wrong state" );
		}



		[Test]
		public void CompTestHollow()
		{
			m.Adresse = a;
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( m ); // setzt m.adresse auf null

			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Persistent ==  a.NDOObjectState, "1: Adresse should be persistent" );

			a = m.Adresse; // ruft LoadData för m auf. m.adresse liegt auf dem Cache und ist Persistent
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent" );
			Assert.That(NDOObjectState.Persistent ==  a.NDOObjectState, "2: Adresse should be persistent" );
			ObjectId id = m.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			m = (Mitarbeiter)pm.FindObject( id );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "2: Mitarbeiter should be hollow" );
			a = m.Adresse;
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "2: Mitarbeiter should be persistent" );
			Assert.That(a != null, "Adress not found" );
			Assert.That(NDOObjectState.Hollow ==  a.NDOObjectState, "1: Adresse should be hollow" );
		}


		[Test]
		public void CompTestMakeAllHollow()
		{
			m.Adresse = a;
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  a.NDOObjectState, "1: Adresse should be hollow" );
		}

		[Test]
		public void CompTestMakeAllHollowUnsaved()
		{
			m.Adresse = a;
			pm.MakePersistent( m );
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "1: Mitarbeiter should be created" );
			Assert.That(NDOObjectState.Created ==  a.NDOObjectState, "1: Adresse should be created" );
		}


		[Test]
		public void CompTestExtentRelatedObjects()
		{
			m.Adresse = a;
			pm.MakePersistent( m );
			pm.Save();
			IList liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent" );
			Assert.That(m.Adresse != null, "2. Relation is missing" );
			Assert.That(NDOObjectState.Persistent ==  m.Adresse.NDOObjectState, "4.: Adresse should be hollow" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "5: Mitarbeiter should be hollow" );
			Assert.That(m.Adresse != null, "6. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  m.Adresse.NDOObjectState, "8.: Adresse should be hollow" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ), false );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "9: Mitarbeiter should be persistent" );
			Assert.That(m.Adresse != null, "10. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  m.Adresse.NDOObjectState, "12.: Adresse should be hollow" );
		}

		#endregion

		#region Assoziation Tests

		[Test]
		public void AssoTestCreateObjectsTransient()
		{
			m.Zimmer = b;
			var thrown = false;
			try
			{
				pm.MakePersistent( m );
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void AssoTestCreateObjectsTransient2()
		{
			pm.MakePersistent( m );
			var thrown = false;
			try
			{
				m.Zimmer = b;
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void AssoTestCreateObjectsSave()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.Save();
			Assert.That( !m.NDOObjectId.Equals( b.NDOObjectId ), "Ids should be different" );
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			b = (Buero)pm.FindObject( b.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(b != null, "1. Büro not found" );
			ObjectId moid = m.NDOObjectId;
			ObjectId boid = b.NDOObjectId;
			m = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Buero b2 = m.Zimmer;
			b = (Buero)pm.FindObject( boid );
			Assert.That(m != null, "2. Mitarbeiter not found" );
			Assert.That(b != null, "2. Adresse not found" );
			Assert.That(Object.ReferenceEquals(b, b2), "Büro should match" );
		}

		[Test]
		public void AssoTestCreateObjectsSave2()
		{
			pm.MakePersistent( b );
			pm.Save();
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.Save();
			Assert.That( !m.NDOObjectId.Equals( b.NDOObjectId ), "Ids should be different" );
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			b = (Buero)pm.FindObject( b.NDOObjectId );
		}

		[Test]
		public void AssoTestAddObjectSave()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			pm.Save();
			m.Zimmer = b;
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.PersistentDirty ==  m.NDOObjectState, "2. Wrong state" );
			pm.Save();
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			b = (Buero)pm.FindObject( b.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(b != null, "1. Büro not found" );
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "2. Wrong state" );
		}



		[Test]
		public void AssoTestAggregateFunction()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			m.Zimmer = b;
			pm.Save();
			ObjectId oid = b.NDOObjectId;
			pm.UnloadCache();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "meinBuero.oid" + " = " + "{0}" );
			q.Parameters.Add( oid.Id[0] );
			decimal count = (decimal)q.ExecuteAggregate( m => m.Nachname, AggregateType.Count );
			Assert.That( count > 0m, "Count should be > 0" );
			m.Zimmer = null;
			pm.Save();
			pm.UnloadCache();
			count = (decimal)q.ExecuteAggregate( m => m.Nachname, AggregateType.Count );
			Assert.That(0m ==  count, "Count should be 0" );
		}

		[Test]
		public void AssoTestAddObjectAbort()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			pm.Save();
			m.Zimmer = b;
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1. Wrong state" );
			Assert.That(m.Zimmer != null, "1. Büro not found" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
			Assert.That(m.Zimmer == null, "1. Büro should be null" );
		}

		[Test]
		public void AssoTestRemoveObjectSave()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			m.Zimmer = b;
			pm.Save();
			Assert.That(m.Zimmer != null, "1. Büro not found" );
			m.Zimmer = null;
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1. Wrong state" );
			Assert.That(m.Zimmer == null, "1. Büro should be null" );
			pm.Save();
			Assert.That(m.Zimmer == null, "2. Büro should be null" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
			ObjectId moid = m.NDOObjectId;
			ObjectId boid = b.NDOObjectId;
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "3. Mitarbeiter not found" );
			Assert.That(m.Zimmer == null, "3. Büro should be null" );
			b = (Buero)pm.FindObject( boid );
			Assert.That(b != null, "3. Buero not found" );
		}

		[Test]
		public void AssoTestRemoveObjectAbort()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			m.Zimmer = b;
			pm.Save();
			Assert.That(m.Zimmer != null, "1. Büro not found" );
			m.Zimmer = null;
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1. Wrong state" );
			Assert.That(m.Zimmer == null, "2. Büro should be null" );
			pm.Abort();
			Assert.That(m.Zimmer != null, "2. Büro not found" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void AssoTestDeleteSave()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			m.Zimmer = b;
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
		}



		[Test]
		public void AssoTestDeleteAbort()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			m.Zimmer = b;
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void AssoTestAddRemoveSave()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			pm.Save();
			m.Zimmer = b;
			m.Zimmer = null;
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void AssoTestAddRemoveAbort()
		{
			pm.MakePersistent( b );
			pm.MakePersistent( m );
			pm.Save();
			m.Zimmer = b;
			m.Zimmer = null;
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2. Wrong state" );
		}



		[Test]
		public void AssoTestHollow()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( m ); // setzt m.Zimmer auf null

			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1: Büro should be persistent" );

			b = m.Zimmer; // ruft LoadData för m auf. m.Zimmer liegt im Cache und ist Persistent
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent" );
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2: Adresse should be persistent" );
			ObjectId id = m.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			m = (Mitarbeiter)pm.FindObject( id );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "2: Mitarbeiter should be hollow" );
			b = m.Zimmer;
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "2: Mitarbeiter should be persistent" );
			Assert.That(b != null, "Büro not found" );
			Assert.That(NDOObjectState.Hollow ==  b.NDOObjectState, "1: Büro should be hollow" );
		}


		[Test]
		public void AssoTestMakeAllHollow()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  b.NDOObjectState, "1: Büro should be hollow" );
		}

		[Test]
		public void AssoTestMakeAllHollowUnsaved()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "1: Mitarbeiter should be created" );
			Assert.That(NDOObjectState.Created ==  b.NDOObjectState, "1: Büro should be created" );
		}

		[Test]
		public void AssoTestMakeAllHollowUnsaved2()
		{
			pm.MakePersistent( b );
			pm.Save();
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "1: Mitarbeiter should be created" );
			Assert.That(NDOObjectState.Hollow ==  b.NDOObjectState, "1: Büro should be hollow" );
		}

		[Test]
		public void AssoTestExtentRelatedObjects()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.Save();
			IList liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent" );
			Assert.That(m.Zimmer != null, "2. Relation is missing" );
			Assert.That(NDOObjectState.Persistent ==  m.Zimmer.NDOObjectState, "4.: Büro should be hollow" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "5: Mitarbeiter should be hollow" );
			Assert.That(m.Zimmer != null, "6. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  m.Zimmer.NDOObjectState, "8.: Büro should be hollow" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ), false );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "9: Mitarbeiter should be persistent" );
			Assert.That(m.Zimmer != null, "10. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  m.Zimmer.NDOObjectState, "12.: Büro should be hollow" );
		}
		#endregion

		#region Combined Tests
		[Test]
		public void CombinedTestCreateObjectsSave()
		{
			pm.MakePersistent( b );
			m.Adresse = a;
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.Save();
			Assert.That( !m.NDOObjectId.Equals( a.NDOObjectId ), "Ids should be different" );
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			a = (Adresse)pm.FindObject( a.NDOObjectId );
			b = (Buero)pm.FindObject( b.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(a != null, "1. Adresse not found" );
			Assert.That(b != null, "1. Büro not found" );
			ObjectId moid = m.NDOObjectId;
			ObjectId aoid = a.NDOObjectId;
			ObjectId boid = b.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "2. Mitarbeiter not found" );
			Assert.That(m.Adresse != null, "2. Adresse not found" );
			Assert.That(m.Zimmer != null, "2. Büro not found" );
		}
		[Test]
		public void TestForeignKeyConstraint()
		{
			pm.MakePersistent( b );
			pm.Save();
			pm.MakePersistent( m );
			m.Zimmer = b;
			pm.Save();
		}

		[Test]
		public void CombinedTestAddAdresse()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			pm.MakePersistent( m );
			pm.Save();
			m.Adresse = a;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "2. Mitarbeiter not found" );
			Assert.That(m.Adresse != null, "2. Adresse not found" );
			Assert.That(m.Zimmer != null, "2. Büro not found" );
		}

		[Test]
		public void CombinedTestAddAdresseRemoveBüro()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			pm.MakePersistent( m );
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
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(m.Adresse != null, "Adresse not found" );
			Assert.That(m.Zimmer == null, "Unexpected Büro" );
			b = (Buero)pm.FindObject( boid );
			Assert.That(b != null, "Büro not found" );
		}

		[Test]
		public void CombinedTestAddBüroRemoveAdresse()
		{
			pm.MakePersistent( b );
			m.Adresse = a;
			pm.MakePersistent( m );
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
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(m.Zimmer != null, "Büro not found" );
			Assert.That(m.Adresse == null, "Unexpected Adresse" );
			Assert.That( adr.NDOObjectState == NDOObjectState.Transient, "Adresse should be deleted" );
		}

		[Test]
		public void CombinedTestCreateAddRemoveAdresse()
		{
			pm.MakePersistent( b );
			m.Adresse = a;
			m.Zimmer = b;
			m.Adresse = null;
			pm.MakePersistent( m );
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(m.Zimmer != null, "Büro not found" );
			Assert.That(m.Adresse == null, "Unexpected Adresse" );
		}

		[Test]
		public void CombinedTestCreateAddRemoveBüro()
		{
			pm.MakePersistent( b );
			m.Zimmer = b;
			m.Adresse = a;
			m.Zimmer = null;
			pm.MakePersistent( m );
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(m.Adresse != null, "Adresse not found" );
			Assert.That(m.Zimmer == null, "Unexpected Büro" );
		}

		[Test]
		public void CombinedTestAddRemoveBüro()
		{
			pm.MakePersistent( b );
			m.Adresse = a;
			m.Zimmer = null;
			pm.MakePersistent( m );
			pm.Save();
			m.Zimmer = b;
			m.Zimmer = null;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(m.Adresse != null, "Adresse not found" );
			Assert.That(m.Zimmer == null, "Unexpected Büro" );
		}

		[Test]
		public void CombinedTestAddBüroRemoveAdresseAbort()
		{
			pm.MakePersistent( b );
			m.Adresse = a;
			pm.MakePersistent( m );
			pm.Save();
			ObjectId aoid = a.NDOObjectId;
			m.Zimmer = b;
			m.Adresse = null;
			pm.Abort();
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(m.Adresse != null, "1. Adresse not found" );
			Assert.That(m.Zimmer == null, "1. Unexpected Büro" );


			ObjectId moid = m.NDOObjectId;
			m = null;
			a = null;
			b = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "2. Mitarbeiter not found" );
			Assert.That(m.Adresse != null, "2. Adresse not found" );
			Assert.That(m.Zimmer == null, "2. Unexpected Büro" );
		}
		#endregion

		private Mitarbeiter CreateMitarbeiter( string vorname, string nachname )
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		private Adresse CreateAdresse( string lkz, string plz, string straße, string ort )
		{
			Adresse a = new Adresse();
			a.Lkz = lkz;
			a.Plz = plz;
			a.Straße = straße;
			a.Ort = ort;
			return a;
		}

		private Buero CreateBuero( string zimmer )
		{
			return new Buero( zimmer );
		}
	}
}
