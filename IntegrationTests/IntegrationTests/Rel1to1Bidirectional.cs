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
using Reisekosten;
using NDO;
using NDO.Linq;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace NdoUnitTests
{
	/// <summary>
	/// All tests for 1:1-Relations. Bidirectional Composition and Aggregation
	/// </summary>


	[TestFixture]
	public class Rel1to1Bidirectional : NDOTest
	{
		public Rel1to1Bidirectional()
		{
		}

		private static int count = 0;
		private Mitarbeiter m;
		private Sozialversicherungsnummer svn;
		private Email e;
		private Zertifikat z0;

		[SetUp]
		public void Setup()
		{
			count++;
			m = CreateMitarbeiter( "Boris", "Becker" );
			svn = new Sozialversicherungsnummer();
			svn.SVN = 4711;
			e = new Email( "hwk@cortex-brainware.de" );
			z0 = new Zertifikat();
			z0.Key = 42; // :-)
		}

		public void DeleteAll()
		{
			var pm = PmFactory.NewPersistenceManager();
			IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
			pm.Delete( mitarbeiterListe );
			pm.Save();

			// Delete unbound Soz.V. objects
			IList sozListe = pm.GetClassExtent( typeof( Sozialversicherungsnummer ) );
			pm.Delete( sozListe );
			pm.Save();

			IList eListe = pm.GetClassExtent( typeof( Email ) );
			pm.Delete( eListe );
			pm.Save();

			IList zListe = pm.GetClassExtent( typeof( Zertifikat ) );
			pm.Delete( zListe );
			pm.Save();
		}

		[TearDown]
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();
			DeleteAll();
			pm.Close();
			pm = null;
		}



		#region Composition Tests

		[Test]
		public void CompTestCreateObjectsSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			m.SVN = svn;
			pm.MakePersistent( m );
			pm.Save();
			Assert.That( !m.NDOObjectId.Equals( m.SVN.NDOObjectId ), "Ids should be different" );
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			svn = (Sozialversicherungsnummer)pm.FindObject( m.SVN.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(svn != null, "1. SVN not found" );
			ObjectId moid = m.NDOObjectId;
			ObjectId soid = svn.NDOObjectId;
			m = null;
			svn = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Sozialversicherungsnummer s2 = m.SVN;
			svn = (Sozialversicherungsnummer)pm.FindObject( soid );
			Assert.That(m != null, "2. Mitarbeiter not found" );
			Assert.That(svn != null, "2. SVN not found" );
			Assert.That(Object.ReferenceEquals(svn, s2), "SVN should match" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "Mitarbeiter should match" );
		}

		[Test]
		public void SimpleObjectSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( svn );
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent( typeof( Sozialversicherungsnummer ) );
			Assert.That( l.Count == 1, "Sozialversicherungsnummer sollte gespeichert sein" );
			pm.Delete( l );
			pm.Save();
			pm.UnloadCache();
			l = pm.GetClassExtent( typeof( Sozialversicherungsnummer ) );
			Assert.That( l.Count == 0, "Sozialversicherungsnummer sollte gelöscht sein" );
		}

		[Test]
		public void CompChildAddFail()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( svn );
			var thrown = false;
			try
			{
				svn.Angestellter = m;
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void CompChildAddFail2()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( svn );
			pm.Save();
			var thrown = false;
			try
			{
				svn.Angestellter = m;
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void CompChildAdd3()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			var thrown = false;
			try
			{
				svn.Angestellter = m;
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void CompChildAdd4()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			var thrown = false;
			try
			{
				svn.Angestellter = null; // Cannot change relation through child
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void CompTestAddObjectSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			Assert.That(NDOObjectState.Created ==  svn.NDOObjectState, "1. Wrong state" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "1. Backlink wrong" );
			pm.Save();
			m = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			svn = (Sozialversicherungsnummer)pm.FindObject( svn.NDOObjectId );
			Assert.That(m != null, "1. Mitarbeiter not found" );
			Assert.That(svn != null, "1. SVN not found" );
			Assert.That(NDOObjectState.Persistent ==  svn.NDOObjectState, "2. Wrong state" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestAddObjectAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			Assert.That(NDOObjectState.Created ==  svn.NDOObjectState, "1. Wrong state" );
			Assert.That(m.SVN != null, "1. SVN not found" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "2. Wrong state" );
			Assert.That(m.SVN == null, "1. SVN should be null" );
			Assert.That(svn.Angestellter == null, "1. Mitarbeiter should be null" );
		}


		[Test]
		public void CompTestReplaceChildSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			m.SVN = svn;
			pm.Save();
			Assert.That(m.SVN != null, "1. SVN not found" );
			Sozialversicherungsnummer svn2 = new Sozialversicherungsnummer();
			svn2.SVN = 0815;
			m.SVN = svn2;
			Assert.That(NDOObjectState.Deleted ==  svn.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Created ==  svn2.NDOObjectState, "2. Wrong state" );
			Assert.That(svn.Angestellter == null, "3. No relation to Mitarbeiter" );
			Assert.That(Object.ReferenceEquals(svn2.Angestellter, m), "4. Mitarbeiter should be same" );
			Assert.That(Object.ReferenceEquals(m.SVN, svn2), "5. SVN should be same" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "6. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  svn2.NDOObjectState, "7. Wrong state" );
			Assert.That(svn.Angestellter == null, "8. No relation to Mitarbeiter" );
			Assert.That(Object.ReferenceEquals(svn2.Angestellter, m), "9. Mitarbeiter should be same" );
			Assert.That(Object.ReferenceEquals(m.SVN, svn2), "10. SVN should be same" );
		}

		[Test]
		public void CompTestReplaceChildAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			m.SVN = svn;
			Assert.That(Object.ReferenceEquals(svn.Angestellter, m), "1. Mitarbeiter should be same" );
			pm.Save();
			Assert.That(m.SVN != null, "1. SVN not found" );
			Sozialversicherungsnummer svn2 = new Sozialversicherungsnummer();
			svn2.SVN = 0815;
			m.SVN = svn2;
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  svn2.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  svn.NDOObjectState, "2. Wrong state" );
			Assert.That(svn2.Angestellter == null, "3. No relation to Mitarbeiter" );
			Assert.That(Object.ReferenceEquals(svn.Angestellter, m), "4. Mitarbeiter should be same" );
			Assert.That(Object.ReferenceEquals(m.SVN, svn), "5. SVN should be same" );
		}

		[Test]
		public void CompTestReplaceParent()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			m.SVN = svn;
			pm.Save();
			Assert.That(m.SVN != null, "1. SVN not found" );
			Mitarbeiter m2 = CreateMitarbeiter( "Andre", "Agassi" );
			var thrown = false;
			try
			{
				svn.Angestellter = m2;
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}


		[Test]
		public void CompTestRemoveObjectSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			m.SVN = svn;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			Assert.That(m.SVN != null, "1. SVN not found" );
			m.SVN = null;
			Assert.That(NDOObjectState.Deleted ==  svn.NDOObjectState, "1. Wrong state" );
			Assert.That(m.SVN == null, "1. SVN should be null" );
			Assert.That(svn.Angestellter == null, "1. Mitarbeiter should be null" );
			pm.Save();
			Assert.That(m.SVN == null, "2. SVN should be null" );
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "2. Wrong state" );
			moid = m.NDOObjectId;
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject( moid );
			Assert.That(m != null, "3. Mitarbeiter not found" );
			Assert.That(m.SVN == null, "3. SVN should be null" );
		}

		[Test]
		public void CompTestRemoveObjectAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			m.SVN = svn;
			pm.Save();
			Assert.That(m.SVN != null, "1. SVN not found" );
			m.SVN = null;
			Assert.That(NDOObjectState.Deleted ==  svn.NDOObjectState, "1. Wrong state" );
			Assert.That(m.SVN == null, "2. SVN should be null" );
			pm.Abort();
			Assert.That(m.SVN != null, "2. SVN not found" );
			Assert.That(NDOObjectState.Persistent ==  svn.NDOObjectState, "2. Wrong state" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestRemoveParentSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			m.SVN = svn;
			pm.Save();
			Assert.That(svn.Angestellter != null, "1. Mitarbeiter not found" );
			var thrown = false;
			try
			{
				svn.Angestellter = null;  // Cannot manipulate composition through child object.
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}


		[Test]
		public void CompTestDeleteSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  svn.NDOObjectState, "2. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "2. Wrong state" );
			// Objects should retain relations in memory
			Assert.That(m.SVN != null, "3. SVN shouldn't be null" );
			Assert.That(svn.Angestellter == null, "3. Mitarbeiter should be null" );
		}

		[Test]
		public void CompTestDeleteChildSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			bool thrown = false;
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			pm.Save();
			try
			{
				pm.Delete( svn ); // Should throw an Exception
			}
			catch (NDOException e)
			{
				thrown = true;
			}
			Assert.That( thrown, "Exception mösste ausgelöst worden sein" );
		}


		[Test]
		public void CompTestDeleteAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			pm.Save();
			pm.Delete( m );
			Assert.That(NDOObjectState.Deleted ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  svn.NDOObjectState, "2. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  svn.NDOObjectState, "2. Wrong state" );
			Assert.That(m.SVN != null, "2. SVN not found" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestCreateDelete()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			m.SVN = svn;
			pm.Delete( m );
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "2. Wrong state" );
			// Objects should retain relations in memory
			Assert.That(m.SVN != null, "3. SVN shouldn't be null" );
			Assert.That(svn.Angestellter == null, "3. Mitarbeiter should be null" );
		}

		[Test]
		public void CompTestAddRemoveSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			m.SVN = null;
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "1. Wrong state" );
			Assert.That(m.SVN == null, "1. SVN should be null" );
			Assert.That(svn.Angestellter == null, "1. Mitarbeiter should be null" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "2. Wrong state" );
			Assert.That(m.SVN == null, "2. SVN should be null" );
			Assert.That(svn.Angestellter == null, "3. Mitarbeiter should be null" );
		}

		[Test]
		public void CompTestAddRemoveAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( m );
			pm.Save();
			m.SVN = svn;
			m.SVN = null;
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "1. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  svn.NDOObjectState, "2. Wrong state" );
			Assert.That(m.SVN == null, "2. SVN should be null" );
			Assert.That(svn.Angestellter == null, "3. Mitarbeiter should be null" );
		}



		[Test]
		public void CompTestHollow()
		{
			var pm = PmFactory.NewPersistenceManager();
			m.SVN = svn;
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeHollow( m ); // setzt m.svn auf null

			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Persistent ==  svn.NDOObjectState, "1: SVN should be persistent" );

			svn = m.SVN; // ruft LoadData för m auf. m.svm liegt auf dem Cache und ist Persistent
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent" );
			Assert.That(NDOObjectState.Persistent ==  svn.NDOObjectState, "2: SVN should be persistent" );
			ObjectId id = m.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			m = (Mitarbeiter)pm.FindObject( id );
			Assert.That(m != null, "Mitarbeiter not found" );
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "2: Mitarbeiter should be hollow" );
			svn = m.SVN;
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "2: Mitarbeiter should be persistent" );
			Assert.That(svn != null, "SVN not found" );
			Assert.That(NDOObjectState.Hollow ==  svn.NDOObjectState, "1: SVN should be hollow" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "2. Backlink wrong" );
		}


		[Test]
		public void CompTestMakeAllHollow()
		{
			var pm = PmFactory.NewPersistenceManager();
			m.SVN = svn;
			pm.MakePersistent( m );
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "1: Mitarbeiter should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  svn.NDOObjectState, "1: SVN should be hollow" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestMakeAllHollowUnsaved()
		{
			var pm = PmFactory.NewPersistenceManager();
			m.SVN = svn;
			pm.MakePersistent( m );
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "1: Mitarbeiter should be created" );
			Assert.That(NDOObjectState.Created ==  svn.NDOObjectState, "1: SVN should be created" );
		}

		[Test]
		public void CompTestLockRelations()
		{
			var pm = PmFactory.NewPersistenceManager();
			m.SVN = svn;
			pm.MakePersistent( m );
			m.Adresse = new Adresse();
			m.Adresse.Lkz = "Bla";
			pm.Save();
			pm.Delete( m );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "1: mitarbeiter should be transient" );
			Assert.That(NDOObjectState.Transient ==  m.SVN.NDOObjectState, "1: SVN should be transient" );
			Assert.That(NDOObjectState.Transient ==  m.Adresse.NDOObjectState, "1: Adresse should be transient" );
		}

		[Test]
		public void CompTestExtentRelatedObjects()
		{
			var pm = PmFactory.NewPersistenceManager();
			m.SVN = svn;
			pm.MakePersistent( m );
			pm.Save();
			IList liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1: Mitarbeiter should be persistent" );
			Assert.That(m.SVN != null, "2. Relation is missing" );
			Assert.That(NDOObjectState.Persistent ==  m.SVN.NDOObjectState, "2.: SVN should be hollow" );
			Assert.That(Object.ReferenceEquals(m, svn.Angestellter), "2. Backlink wrong" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ) );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "5: Mitarbeiter should be hollow" );
			Assert.That(m.SVN != null, "6. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  m.SVN.NDOObjectState, "8.: SVN should be hollow" );
			Assert.That( m != svn.Angestellter, "8a. Should be different objects" );
			Assert.That(Object.ReferenceEquals(m, m.SVN.Angestellter), "8b. Mitarbeiter should match" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Mitarbeiter ), false );
			m = (Mitarbeiter)liste[0];
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "9: Mitarbeiter should be persistent" );
			Assert.That(m.SVN != null, "10. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  m.SVN.NDOObjectState, "12.: SVN should be hollow" );
			Assert.That( m != svn.Angestellter, "12a. Should be different objects" );
			Assert.That(Object.ReferenceEquals(m, m.SVN.Angestellter), "12b. Mitarbeiter should match" );
		}

		#endregion

		#region Aggregation Tests

		[Test]
		public void AggrTestCreateObjectsTransient()
		{
			var pm = PmFactory.NewPersistenceManager();
			e.Schlüssel = z0;
			var thrown = false;
			try
			{
				pm.MakePersistent( e );
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void AggrTestCreateObjectsTransient2()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( e );
			var thrown = false;
			try
			{
				e.Schlüssel = z0;
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void AggrTestCreateObjectsSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			e.Schlüssel = z0;
			pm.MakePersistent( e );
			pm.Save();
			Assert.That( !e.NDOObjectId.Equals( e.Schlüssel.NDOObjectId ), "Ids should be different" );
			e = (Email)pm.FindObject( e.NDOObjectId );
			z0 = (Zertifikat)pm.FindObject( e.Schlüssel.NDOObjectId );
			Assert.That(e != null, "1. Email not found" );
			Assert.That(z0 != null, "1. Zertifikat not found" );
			ObjectId moid = e.NDOObjectId;
			ObjectId soid = z0.NDOObjectId;
			e = null;
			z0 = null;

			pm.UnloadCache();
			e = (Email)pm.FindObject( moid );
			var z1 = pm.Objects<Zertifikat>().Where(z=>z.Oid() == soid.Id.Value).SingleOrDefault();
			Assert.That( z1 != null );
			Zertifikat z2 = e.Schlüssel;
			Assert.That(Object.ReferenceEquals(z1, z2));
			z0 = (Zertifikat)pm.FindObject( soid );
			Assert.That(e != null, "2. Email not found" );
			Assert.That(z0 != null, "2. Zertifikat not found" );
			Assert.That(Object.ReferenceEquals(z0, z2), "Zertifikat should match" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "Email should match" );
		}

		[Test]
		public void AggrTestCreateObjectsWithTransaction()
		{
			var pm = PmFactory.NewPersistenceManager();
			var tm = pm.TransactionMode;
			try
			{
				// See Issue #1145
				// The test would fail, if we didn't alter the update rank
				// according to the actual situation, that e has a valid database id.
				// The test would fail, because the update rank would cause e to be saved first
				// at the 2nd Save() call.
				pm.TransactionMode = TransactionMode.Optimistic;
				pm.MakePersistent( e );
				pm.Save( true );
				pm.MakePersistent( z0 );
				e.Schlüssel = z0;
				pm.Save();
				pm.UnloadCache();
				pm.TransactionMode = TransactionMode.None;
				var email = pm.Objects<Email>().Single();
				Assert.That(email.Schlüssel  != null);
				Assert.That(42 ==  email.Schlüssel.Key );
				pm.UnloadCache();
				var zertifikat = pm.Objects<Zertifikat>().Single();
				Assert.That(zertifikat.Adresse  != null);
			}
			finally
			{
				pm.TransactionMode = tm;
			}
		}


		[Test]
		public void AggrSimpleObjectSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.Save();
		}

		[Test]
		public void AggrChildAddFail()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			z0.Adresse = e;
		}

		[Test]
		public void AggrChildAddFail2()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( e );
			pm.MakePersistent( z0 );
			pm.Save();
			z0.Adresse = e;
		}

		[Test]
		public void AggrChildAdd3()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( e );
			pm.MakePersistent( z0 );
			pm.Save();
			e.Schlüssel = z0;
			z0.Adresse = e;
		}

		[Test]
		public void AggrChildAdd4()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			pm.Save();
			e.Schlüssel = z0;
			z0.Adresse = null;
		}

		[Test]
		public void AggrTestAddObjectSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			pm.Save();
			e.Schlüssel = z0;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "1. Backlink wrong" );
			pm.Save();
			e = (Email)pm.FindObject( e.NDOObjectId );
			z0 = (Zertifikat)pm.FindObject( z0.NDOObjectId );
			Assert.That(e != null, "1. Email not found" );
			Assert.That(z0 != null, "1. Zertifikat not found" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "2. Backlink wrong" );
		}

		[Test]
		public void AggrTestAddObjectAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			pm.Save();
			e.Schlüssel = z0;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(e.Schlüssel != null, "1. Zertifikat not found" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			Assert.That(e.Schlüssel == null, "1. Zertifikat should be null" );
			Assert.That(z0.Adresse == null, "1. Email should be null" );
		}


		[Test]
		public void AggrTestReplaceChildSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			pm.Save();
			Assert.That(e.Schlüssel != null, "1. Zertifikat not found" );
			Zertifikat z2 = new Zertifikat();
			z2.Key = 0815;
			pm.MakePersistent( z2 );
			e.Schlüssel = z2;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Created ==  z2.NDOObjectState, "2. Wrong state" );
			Assert.That(z0.Adresse == null, "3. No relation to Email" );
			Assert.That(Object.ReferenceEquals(z2.Adresse, e), "4. Email should be same" );
			Assert.That(Object.ReferenceEquals(e.Schlüssel, z2), "5. Zertifikat should be same" );
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "6. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  z2.NDOObjectState, "7. Wrong state" );
			Assert.That(z0.Adresse == null, "8. No relation to Email" );
			Assert.That(Object.ReferenceEquals(z2.Adresse, e), "9. Email should be same" );
			Assert.That(Object.ReferenceEquals(e.Schlüssel, z2), "10. Zertifikat should be same" );
		}

		[Test]
		public void AggrTestReplaceChildAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			pm.Save();
			Assert.That(e.Schlüssel != null, "1. Zertifikat not found" );
			Zertifikat z2 = new Zertifikat();
			z2.Key = 0815;
			pm.MakePersistent( z2 );
			e.Schlüssel = z2;
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  z2.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			Assert.That(z2.Adresse == null, "3. No relation to Email" );
			Assert.That(Object.ReferenceEquals(z0.Adresse, e), "4. Email should be same" );
			Assert.That(Object.ReferenceEquals(e.Schlüssel, z0), "5. Zertifikat should be same" );
		}

		[Test]
		public void AggrTestReplaceParentSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			pm.Save();
			Assert.That(e.Schlüssel != null, "1. Zertifikat not found" );
			Email m2 = new Email( "db@cortex-brainware.de" );
			pm.MakePersistent( m2 );
			z0.Adresse = m2;
		}


		[Test]
		public void AggrTestRemoveObjectSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			pm.Save();
			Assert.That(e.Schlüssel != null, "1. Zertifikat not found" );
			ObjectId aoid = z0.NDOObjectId;
			e.Schlüssel = null;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(e.Schlüssel == null, "1. Zertifikat should be null" );
			Assert.That(z0.Adresse == null, "1. Email should be null" );
			pm.Save();
			Assert.That(e.Schlüssel == null, "2. Zertifikat should be null" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			ObjectId moid = e.NDOObjectId;
			Assert.That( aoid.IsValid(), "Still valid Zertifikat" );
			pm.UnloadCache();
			e = (Email)pm.FindObject( moid );
			Assert.That(e != null, "3. Email not found" );
			Assert.That(e.Schlüssel == null, "3. Zertifikat should be null" );
		}

		[Test]
		public void AggrTestRemoveObjectAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			pm.Save();
			Assert.That(e.Schlüssel != null, "1. Zertifikat not found" );
			e.Schlüssel = null;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(e.Schlüssel == null, "2. Zertifikat should be null" );
			pm.Abort();
			Assert.That(e.Schlüssel != null, "2. Zertifikat not found" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "2. Backlink wrong" );
		}

		[Test]
		public void AggrTestRemoveParentSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			pm.Save();
			Assert.That(z0.Adresse != null, "1. Email not found" );
			ObjectId aoid = z0.NDOObjectId;
			z0.Adresse = null;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(e.Schlüssel == null, "1. Zertifikat should be null" );
			Assert.That(z0.Adresse == null, "1. Email should be null" );
			pm.Save();
			Assert.That(e.Schlüssel == null, "2. Zertifikat should be null" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			ObjectId moid = e.NDOObjectId;
			Assert.That( aoid.IsValid(), "Zertifikat still valid" );
			pm.UnloadCache();
			e = (Email)pm.FindObject( moid );
			Assert.That(e != null, "3. Email not found" );
			Assert.That(e.Schlüssel == null, "3. Zertifikat should be null" );
		}

		[Test]
		public void AggrTestRemoveParentAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			pm.Save();
			Assert.That(e.Schlüssel != null, "1. Zertifikat not found" );
			z0.Adresse = null;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(e.Schlüssel == null, "2. Zertifikat should be null" );
			pm.Abort();
			Assert.That(e.Schlüssel != null, "2. Zertifikat not found" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "2. Backlink wrong" );
		}

#if false
		[Test]
		[ExpectedException(typeof(NDOException))]
		public void AggrTestDelete () {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			pm.Save();
			e.Schlüssel = z;
			pm.Save();
			pm.Delete(e);  // Cannot delete object within aggregation
		}


		[Test]
		[ExpectedException(typeof(NDOException))]
		public void AggrTestCreateDelete() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Delete(e);  // Cannot delete object within aggregation
		}
#endif

		[Test]
		public void AggrTestCreateDeleteAllowed()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			pm.MakePersistent( e );
			e.Schlüssel = z0;
			e.Schlüssel = null;
			pm.Delete( e );
			Assert.That(NDOObjectState.Created ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Transient ==  e.NDOObjectState, "2. Wrong state" );
			Assert.That(e.Schlüssel == null, "3. Zertifikat should be null" );
			Assert.That(z0.Adresse == null, "4. Email should be null" );
		}


		[Test]
		public void AggrTestAddRemoveSave()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( e );
			pm.MakePersistent( z0 );
			pm.Save();
			e.Schlüssel = z0;
			e.Schlüssel = null;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			Assert.That(e.Schlüssel == null, "1. Zertifikat should be null" );
			Assert.That(z0.Adresse == null, "1. Email should be null" );
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			Assert.That(e.Schlüssel == null, "2. Zertifikat should be null" );
			Assert.That(z0.Adresse == null, "3. Email should be null" );
		}

		[Test]
		public void AggrTestAddRemoveAbort()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( e );
			pm.MakePersistent( z0 );
			pm.Save();
			e.Schlüssel = z0;
			e.Schlüssel = null;
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "1. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2. Wrong state" );
			Assert.That(e.Schlüssel == null, "2. Zertifikat should be null" );
			Assert.That(z0.Adresse == null, "3. Email should be null" );
		}



		[Test]
		public void AggrTestHollow()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			e.Schlüssel = z0;
			pm.MakePersistent( e );
			pm.Save();
			pm.MakeHollow( e ); // setzt e.z auf null

			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "1: Email should be hollow" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "1: Zertifikat should be persistent" );

			z0 = e.Schlüssel; // ruft LoadData för e auf. e.svm liegt auf dem Cache und ist Persistent
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "1: Email should be persistent" );
			Assert.That(NDOObjectState.Persistent ==  z0.NDOObjectState, "2: Zertifikat should be persistent" );
			ObjectId id = e.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			e = (Email)pm.FindObject( id );
			Assert.That(e != null, "Email not found" );
			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "2: Email should be hollow" );
			z0 = e.Schlüssel;
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "2: Email should be persistent" );
			Assert.That(z0 != null, "Zertifikat not found" );
			Assert.That(NDOObjectState.Hollow ==  z0.NDOObjectState, "1: Zertifikat should be hollow" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "2. Backlink wrong" );
		}


		[Test]
		public void AggrTestMakeAllHollowDelete()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			e.Schlüssel = z0;
			pm.MakePersistent( e );
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "1: Email should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  z0.NDOObjectState, "1: Zertifikat should be hollow" );
			Zertifikat zz = e.Schlüssel;  // Das lödt das Objekt, deshalb geht es hier!
			e.Schlüssel = null;
			Assert.That(NDOObjectState.PersistentDirty ==  e.NDOObjectState, "2: Email should be persistent dirty" );
			Assert.That(NDOObjectState.PersistentDirty ==  z0.NDOObjectState, "2: Zertifikat should be persistent dirty" );
			Assert.That(e.Schlüssel == null, "3. Email should have no Zertifikat" );
			Assert.That(z0.Adresse == null, "3. Zertifikat should have no Email" );
			pm.Delete( e );
		}

		[Test]
		public void AggrTestMakeAllHollow()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			e.Schlüssel = z0;
			pm.MakePersistent( e );
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "1: Email should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  z0.NDOObjectState, "1: Zertifikat should be hollow" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "2. Backlink wrong" );
		}

		[Test]
		public void AggrTestMakeAllHollowUnsaved()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			e.Schlüssel = z0;
			pm.MakePersistent( e );
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  e.NDOObjectState, "1: Email should be created" );
			Assert.That(NDOObjectState.Created ==  z0.NDOObjectState, "1: Zertifikat should be created" );
		}


		[Test]
		public void AggrTestExtentRelatedObjects()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( z0 );
			e.Schlüssel = z0;
			pm.MakePersistent( e );
			pm.Save();
			IList liste = pm.GetClassExtent( typeof( Email ) );
			e = (Email)liste[0];
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "1: Email should be persistent" );
			Assert.That(e.Schlüssel != null, "2. Relation is missing" );
			Assert.That(NDOObjectState.Persistent ==  e.Schlüssel.NDOObjectState, "2.: Zertifikat should be hollow" );
			Assert.That(Object.ReferenceEquals(e, z0.Adresse), "2. Backlink wrong" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Email ) );
			e = (Email)liste[0];
			Assert.That(NDOObjectState.Hollow ==  e.NDOObjectState, "5: Email should be hollow" );
			Assert.That(e.Schlüssel != null, "6. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  e.Schlüssel.NDOObjectState, "8.: Zertifikat should be hollow" );
			Assert.That( e != z0.Adresse, "8a. Should be different objects" );
			Assert.That(Object.ReferenceEquals(e, e.Schlüssel.Adresse), "8b. Email should match" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Email ), false );
			e = (Email)liste[0];
			Assert.That(NDOObjectState.Persistent ==  e.NDOObjectState, "9: Email should be persistent" );
			Assert.That(e.Schlüssel != null, "10. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  e.Schlüssel.NDOObjectState, "12.: Zertifikat should be hollow" );
			Assert.That( e != z0.Adresse, "12a. Should be different objects" );
			Assert.That(Object.ReferenceEquals(e, e.Schlüssel.Adresse), "12b. Email should match" );
		}

		[Test]
		public void BiTest()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( e );
			pm.MakePersistent( z0 );
			//			System.Diagnostics.Debug.WriteLine("e:" + e.NDOObjectId.ExpressionString);
			//			System.Diagnostics.Debug.WriteLine("z:" + z.NDOObjectId.ExpressionString);
			e.Schlüssel = this.z0;
			pm.Save();
			//			System.Diagnostics.Debug.WriteLine("e:" + e.NDOObjectId.ExpressionString);
			//			System.Diagnostics.Debug.WriteLine("z:" + z.NDOObjectId.ExpressionString);

		}


		#endregion

		private void SetupEmail()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.MakePersistent( e );
			pm.Save();
		}

		private Mitarbeiter CreateMitarbeiter( string vorname, string nachname )
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}
	}
}
