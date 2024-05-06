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
using NUnit.Framework;
using Reisekosten;
using Reisekosten.Personal;

namespace NdoUnitTests
{
	/// <summary>
	/// This class contains all tests for 1:n directed relations with an intermediate mapping table and composition.
	/// </summary>
	[TestFixture]
	public class Rel1toNDirectedWTableComposition
	{

		public Rel1toNDirectedWTableComposition()
		{
		}

		private PersistenceManager pm;
		private Mitarbeiter m;
		private Email e;

		[SetUp]
		public void Setup()
		{

			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter( "Hartmut", "Kocher" );
			e = CreateEmail( "hwk@cortex-brainware.de" );
		}

		[TearDown]
		public void TearDown()
		{
			pm.Abort();
			IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), false );
			pm.Delete( mitarbeiterListe );
			IList eListe = pm.GetClassExtent( typeof( Email ), false );
			pm.Delete( eListe );
			pm.Save();
			pm.Close();
			pm = null;
		}

		[Test]
		public void TestCreateObjects()
		{
			pm.MakePersistent( e );
			if (!pm.HasOwnerCreatedIds && e.NDOObjectId.Id[0] is Int32)
				Assert.That(-1 == (int) e.NDOObjectId.Id[0], "Email key wrong" );
			Email r2 = (Email)pm.FindObject( e.NDOObjectId );
			Assert.That(Object.ReferenceEquals(e, r2), "Emails should match" );
		}

		[Test]
		public void TestCreateObjectsTransient1()
		{
			m.Hinzufuegen( e );
			pm.MakePersistent( m );
		}

		[Test]
		public void TestCreateObjectsTransient2()
		{
			pm.MakePersistent( m );
			m.Hinzufuegen( e );
		}

		[Test]
		public void TestCreateObjectsTransient3()
		{
			pm.MakePersistent( e );
			pm.MakePersistent( m );
			var thrown = false;
			try
			{
				m.Hinzufuegen( e ); // Cannot add pers. obj.
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void TestCreateObjectsTransient4()
		{
			pm.MakePersistent( e );
			m.Hinzufuegen( e );
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
		public void TestCreateObjectsSave()
		{
			m.Hinzufuegen( e );
			pm.MakePersistent( m );
			pm.Save();
			Assert.That( !m.NDOObjectId.Equals( e.NDOObjectId ), "Ids should be different" );
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			e = (Email)pm.FindObject( e.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(e != null, "1. Email not found" );

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			e = (Email)pm.FindObject( e.NDOObjectId );
			Assert.That(m != null, "2. Mitarbeiter not found" );
			Assert.That(e != null, "2. Email not found" );
		}

		[Test]
		public void TestAddObjectSave()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Hinzufuegen( e );
			Assert.That(NDOObjectState.Created ==  e.NDOObjectState, "1. Wrong state" );
			pm.Save();
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			e = (Email)pm.FindObject( e.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(e != null, "1. Email not found" );
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void TestAddObjectAbort()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Hinzufuegen( e );
			Assert.That(NDOObjectState.Created ==  e.NDOObjectState, "1. Wrong state" );
			Assert.That(1 ==  m.Emails.Count, "1. Wrong number of objects" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "2. Wrong state" );
			Assert.That(0 ==  m.Emails.Count, "2. Wrong number of objects" );
		}
		[Test]
		public void TestRemoveObjectSave()
		{
			m.Hinzufuegen( e );
			pm.MakePersistent( m );
			pm.Save();
			Assert.That(1 ==  m.Emails.Count, "1. Wrong number of objects" );
			m.Löschen( e );
			Assert.That(NDOObjectState.Deleted ==  e.NDOObjectState, "1. Wrong state" );
			Assert.That(0 ==  m.Emails.Count, "2. Wrong number of objects" );
			//pm.LogAdapter = new NDO.Logging.ConsoleLogAdapter();
			//pm.VerboseMode = true;
			pm.Save();
			Assert.That(0 ==  m.Emails.Count, "3. Wrong number of objects" );
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void TestRemoveObjectAbort()
		{
			pm.MakePersistent( m );
			m.Hinzufuegen( e );
			pm.Save();
			Assert.That(1 ==  m.Emails.Count, "1. Wrong number of objects" );
			m.Löschen( e );
			Assert.That(NDOObjectState.Deleted ==  e.NDOObjectState, "1. Wrong state" );
			Assert.That(0 ==  m.Emails.Count, "2. Wrong number of objects" );
			pm.Abort();
			Assert.That(1 ==  m.Emails.Count, "3. Wrong number of objects" );
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void TestDeleteSave()
		{
			pm.MakePersistent( m );
			m.Hinzufuegen( e );
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  e.NDOObjectState, "2. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "2. Wrong state" );
		}



		[Test]
		public void TestDeleteAbort()
		{
			pm.MakePersistent( m );
			m.Hinzufuegen( e );
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  e.NDOObjectState, "2. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "2. Wrong state" );
		}

		[Test]
		public void TestAddRemoveSave()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Hinzufuegen( e );
			m.Löschen( e );
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "1. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "2. Wrong state" );
			Assert.That(0 ==  m.Emails.Count, "3. Wrong number of objects" );
		}

		[Test]
		public void TestAddRemoveAbort()
		{
			pm.MakePersistent( m );
			pm.Save();
			m.Hinzufuegen( e );
			m.Löschen( e );
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "1. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "2. Wrong state" );
			Assert.That(0 ==  m.Emails.Count, "3. Wrong number of objects" );
		}

		[Test]
		public void TestClearRelatedObjectsSave()
		{
			for (int i = 0; i < 10; i++)
			{
				Email rb = CreateEmail( i.ToString() );
				m.Hinzufuegen( rb );
			}
			pm.MakePersistent( m );
			pm.Save();
			IList rr = new ArrayList( m.Emails );
			m.LöscheEmails();
			Assert.That(0 ==  m.Emails.Count, "1. Wrong number of objects" );
			for (int i = 0; i < 10; i++)
			{
				Assert.That(NDOObjectState.Deleted ==  ((Email)rr[i]).NDOObjectState, "2. Wrong state" );
			}
			pm.Save();
			Assert.That(0 ==  m.Emails.Count, "3. Wrong number of objects" );
			for (int i = 0; i < 10; i++)
			{
				Assert.That(NDOObjectState.Transient ==  ((Email)rr[i]).NDOObjectState, "4. Wrong state" );
			}
		}

		[Test]
		public void TestClearRelatedObjectsAbort()
		{
			for (int i = 0; i < 10; i++)
			{
				Email rb = CreateEmail( i.ToString() );
				m.Hinzufuegen( rb );
			}
			pm.MakePersistent( m );
			pm.Save();
			IList rr = new ArrayList( m.Emails );
			m.LöscheEmails();
			Assert.That(0 ==  m.Emails.Count, "1. Wrong number of objects" );
			for (int i = 0; i < 10; i++)
			{
				Assert.That(NDOObjectState.Deleted ==  ((Email)rr[i]).NDOObjectState, "2. Wrong state" );
			}
			pm.Abort();
			Assert.That(10 ==  m.Emails.Count, "3. Wrong number of objects" );
			for (int i = 0; i < 10; i++)
			{
				Assert.That(NDOObjectState.Persistent ==  ((Email)rr[i]).NDOObjectState, "4. Wrong state" );
			}
		}


		[Test]
		public void TestHollow()
		{
			m.Hinzufuegen( e );
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( m );
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "1: Email should be persistent" );
			IList Email = m.Emails;

			pm.MakeHollow( m, true );
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "2: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "2: Email should be hollow" );

			Email = m.Emails;
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "3: Mitarbeiter should be persistent" );
			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "3: Email should be hollow" );
			Assert.That("hwk@cortex-brainware.de" ==  e.Adresse, "3: Email should have correct Adresse" );
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "4: Email should be persistent" );
		}

		[Test]
		public void TestMakeAllHollow()
		{
			m.Hinzufuegen( e );
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "1: Email should be hollow" );
		}

		[Test]
		public void TestMakeAllHollowUnsaved()
		{
			m.Hinzufuegen( e );
			pm.MakePersistent( m );
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "1: Mitarbeiter should be created" );
			Assert.That(NDOObjectState.Created ==  e.NDOObjectState, "1: Email should be created" );
		}

		[Test]
		public void TestLoadRelatedObjects()
		{
			for (int i = 0; i < 10; i++)
			{
				Email rb = CreateEmail( i.ToString() );
				m.Hinzufuegen( rb );
			}
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( m, true );

			IList Emails = new ArrayList( m.Emails );
			Assert.That(10 ==  Emails.Count, "List size should be 10" );

			for (int i = 0; i < 10; i++)
			{
				Email rr = (Email)Emails[i];
				Assert.That(NDOObjectState.Hollow ==  rr.NDOObjectState, "1: Email should be hollow" );
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i.ToString() ==  rr.Adresse, "2: Email should be in right order" );
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Reisebüros2 = m.Emails;
			for (int i = 0; i < 10; i++)
			{
				Email r1 = (Email)Emails[i];
				Email r2 = (Email)Reisebüros2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i.ToString() ==  r1.Adresse, "3: Email should be in right order" );
#endif
				Assert.That( r1 != r2, "Objects should be different" );
			}
		}

		[Test]
		public void TestLoadRelatedObjectsSave()
		{
			pm.MakePersistent( m );
			pm.Save();
			for (int i = 0; i < 10; i++)
			{
				Email rb = CreateEmail( i.ToString() );
				m.Hinzufuegen( rb );
			}
			pm.Save();
			pm.MakeHollow( m, true );

			IList Emails = new ArrayList( m.Emails );

			for (int i = 0; i < 10; i++)
			{
				Email rr = (Email)Emails[i];
				Assert.That(NDOObjectState.Hollow ==  rr.NDOObjectState, "1: Email should be hollow" );
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i.ToString() ==  rr.Adresse, "2: Email should be in right order" );
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Reisebüros2 = m.Emails;
			for (int i = 0; i < 10; i++)
			{
				Email r1 = (Email)Emails[i];
				Email r2 = (Email)Reisebüros2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				Assert.That(i.ToString() ==  r1.Adresse, "3: Email should be in right order" );
#endif
				Assert.That( r1 != r2, "Objects should be different" );
			}
		}

		[Test]
		public void TestExtentRelatedObjects()
		{
			m.Hinzufuegen( e );
			pm.MakePersistent( m );
			pm.Save();
			IList liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent" );
			Assert.That(m.Emails != null, "2. Relation is missing" );
			Assert.That(1 ==  m.Emails.Count, "3. Wrong number of objects" );
			Assert.That(NDOObjectState.Persistent ==  ((Email)m.Emails[0]).NDOObjectState, "4.: Email should be persistent" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "5: Mitarbeiter should be hollow" );
			Assert.That(m.Emails != null, "6. Relation is missing" );
			Assert.That(1 ==  m.Emails.Count, "7. Wrong number of objects" );
			Assert.That(NDOObjectState.Hollow ==  ((Email)m.Emails[0]).NDOObjectState, "8.: Email should be hollow" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ), false );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "9: Mitarbeiter should be persistent" );
			Assert.That(m.Emails != null, "10. Relation is missing" );
			Assert.That(1 ==  m.Emails.Count, "11. Wrong number of objects" );
			Assert.That(NDOObjectState.Hollow ==  ((Email)m.Emails[0]).NDOObjectState, "12.: Email should be hollow" );
		}

		private Mitarbeiter CreateMitarbeiter( string vorname, string nachname )
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		private Email CreateEmail( string Adresse )
		{
			Email e = new Email();
			e.Adresse = Adresse;
			return e;
		}
	}
}
