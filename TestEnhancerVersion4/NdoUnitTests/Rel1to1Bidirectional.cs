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
using System.IO;
using NUnit.Framework;
using Reisekosten.Personal;
using Reisekosten;
using NDO;

namespace NdoUnitTests {
	/// <summary>
	/// All tests for 1:1-Relations. Bidirectional Composition and Aggregation
	/// </summary>


	[TestFixture]
	public class Rel1to1Bidirectional {
		public Rel1to1Bidirectional() {
		}

		private static int count = 0;
		private PersistenceManager pm;
		private Mitarbeiter m;
		private Sozialversicherungsnummer svn;
		private Email e;
		private Zertifikat z;

		[SetUp]
		public void Setup() {
			count++;
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Boris", "Becker");
			svn = new Sozialversicherungsnummer();
			svn.SVN = 4711;
			e = new Email("hwk@cortex-brainware.de");
			z = new Zertifikat();
			z.Key = 42; // :-)
		}

		[TearDown]
		public void TearDown() {
			pm.Abort();
			pm.UnloadCache();
			/*
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
			 */
			string path = Path.GetDirectoryName(pm.NDOMapping.FileName);
			path = Path.Combine(path, "NDOUnitTests.ndo.sql");
			pm.BuildDatabase(path);

			pm.Close();
			pm = null;
		}



		#region Composition Tests

		[Test]
		public void CompTestCreateObjectsSave() {
			m.SVN = svn;
			pm.MakePersistent(m);
			pm.Save();
			Assert.That(!m.NDOObjectId.Equals(m.SVN.NDOObjectId), "Ids should be different");
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			svn = (Sozialversicherungsnummer)pm.FindObject(m.SVN.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(svn, "1. SVN not found");
			ObjectId moid = m.NDOObjectId;
			ObjectId soid = svn.NDOObjectId;
			m = null;
			svn = null;

			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Sozialversicherungsnummer s2 = m.SVN;
			svn = (Sozialversicherungsnummer)pm.FindObject(soid);
			Assert.NotNull(m, "2. Mitarbeiter not found");
			Assert.NotNull(svn, "2. SVN not found");
			Assert.AreSame(svn, s2, "SVN should match");
			Assert.AreSame(m, svn.Angestellter, "Mitarbeiter should match");
		}

		[Test]
		public void SimpleObjectSave() {
			pm.MakePersistent(svn);
			pm.Save();
			pm.UnloadCache();
			IList l = pm.GetClassExtent(typeof(Sozialversicherungsnummer));
			Assert.That(l.Count == 1, "Sozialversicherungsnummer sollte gespeichert sein");
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			l = pm.GetClassExtent(typeof(Sozialversicherungsnummer));
			Assert.That(l.Count == 0, "Sozialversicherungsnummer sollte gelöscht sein");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompChildAddFail() {
			pm.MakePersistent(svn);
			svn.Angestellter = m;
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompChildAddFail2() {
			pm.MakePersistent(svn);
			pm.Save();
			svn.Angestellter = m;
		}

		[Test] 
		[ExpectedException(typeof(NDOException))]
		public void CompChildAdd3() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			svn.Angestellter = m;  // Cannot change relation through child
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompChildAdd4() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			svn.Angestellter = null; // Cannot change relation through child
		}

		[Test]
		public void CompTestAddObjectSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			Assert.AreEqual(NDOObjectState.Created, svn.NDOObjectState, "1. Wrong state");
			Assert.AreSame(m, svn.Angestellter, "1. Backlink wrong");
			pm.Save();
			m = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			svn = (Sozialversicherungsnummer)pm.FindObject(svn.NDOObjectId);
			Assert.NotNull(m, "1. Mitarbeiter not found");
			Assert.NotNull(svn, "1. SVN not found");
			Assert.AreEqual(NDOObjectState.Persistent, svn.NDOObjectState, "2. Wrong state");
			Assert.AreSame(m, svn.Angestellter, "2. Backlink wrong");
		}
			
		[Test]
		public void CompTestAddObjectAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			Assert.AreEqual(NDOObjectState.Created, svn.NDOObjectState, "1. Wrong state");
			Assert.NotNull(m.SVN, "1. SVN not found");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "2. Wrong state");
			Assert.Null(m.SVN, "1. SVN should be null");
			Assert.Null(svn.Angestellter, "1. Mitarbeiter should be null");
		}


		[Test]
		public void CompTestReplaceChildSave() {
			pm.MakePersistent(m);
			m.SVN = svn;
			pm.Save();
			Assert.NotNull(m.SVN, "1. SVN not found");
			Sozialversicherungsnummer svn2 = new Sozialversicherungsnummer();
			svn2.SVN = 0815;
			m.SVN = svn2;
			Assert.AreEqual(NDOObjectState.Deleted, svn.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Created, svn2.NDOObjectState, "2. Wrong state");
			Assert.Null(svn.Angestellter, "3. No relation to Mitarbeiter");
			Assert.AreSame(svn2.Angestellter, m, "4. Mitarbeiter should be same");
			Assert.AreSame(m.SVN, svn2, "5. SVN should be same");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "6. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, svn2.NDOObjectState, "7. Wrong state");
			Assert.Null(svn.Angestellter, "8. No relation to Mitarbeiter");
			Assert.AreSame(svn2.Angestellter, m, "9. Mitarbeiter should be same");
			Assert.AreSame(m.SVN, svn2, "10. SVN should be same");
		}

		[Test]
		public void CompTestReplaceChildAbort() {
			pm.MakePersistent(m);
			m.SVN = svn;
			Assert.AreSame(svn.Angestellter, m, "1. Mitarbeiter should be same");
			pm.Save();
			Assert.NotNull(m.SVN, "1. SVN not found");
			Sozialversicherungsnummer svn2 = new Sozialversicherungsnummer();
			svn2.SVN = 0815;
			m.SVN = svn2;
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, svn2.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, svn.NDOObjectState, "2. Wrong state");
			Assert.Null(svn2.Angestellter, "3. No relation to Mitarbeiter");
			Assert.AreSame(svn.Angestellter, m, "4. Mitarbeiter should be same");
			Assert.AreSame(m.SVN, svn, "5. SVN should be same");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompTestReplaceParent() {
			pm.MakePersistent(m);
			m.SVN = svn;
			pm.Save();
			Assert.NotNull(m.SVN, "1. SVN not found");
			Mitarbeiter m2 = CreateMitarbeiter("Andre", "Agassi");
			svn.Angestellter = m2;
		}


		[Test]
		public void CompTestRemoveObjectSave() {
			pm.MakePersistent(m);
			m.SVN = svn;
			pm.Save();
			ObjectId moid = m.NDOObjectId;
			Assert.NotNull(m.SVN, "1. SVN not found");
			m.SVN = null;
			Assert.AreEqual(NDOObjectState.Deleted, svn.NDOObjectState, "1. Wrong state");
			Assert.Null(m.SVN, "1. SVN should be null");
			Assert.Null(svn.Angestellter, "1. Mitarbeiter should be null");
			pm.Save();
			Assert.Null(m.SVN, "2. SVN should be null");
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "2. Wrong state");
			pm.UnloadCache();
			m = (Mitarbeiter)pm.FindObject(moid);
			Assert.NotNull(m, "3. Mitarbeiter not found");
			Assert.Null(m.SVN, "3. SVN should be null");
		}
		
		[Test]
		public void CompTestRemoveObjectAbort() {
			pm.MakePersistent(m);
			m.SVN = svn;
			pm.Save();
			Assert.NotNull(m.SVN, "1. SVN not found");
			m.SVN = null;
			Assert.AreEqual(NDOObjectState.Deleted, svn.NDOObjectState, "1. Wrong state");
			Assert.Null(m.SVN, "2. SVN should be null");
			pm.Abort();
			Assert.NotNull(m.SVN, "2. SVN not found");
			Assert.AreEqual(NDOObjectState.Persistent, svn.NDOObjectState, "2. Wrong state");
			Assert.AreSame(m, svn.Angestellter, "2. Backlink wrong");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompTestRemoveParentSave() {
			pm.MakePersistent(m);
			m.SVN = svn;
			pm.Save();
			Assert.NotNull(svn.Angestellter, "1. Mitarbeiter not found");
			ObjectId aoid = svn.NDOObjectId;
			svn.Angestellter = null;  // Cannot manipulate composition through child object.
		}


		[Test]
		public void CompTestDeleteSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, svn.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "2. Wrong state");
			// Objects should retain relations in memory
			Assert.NotNull(m.SVN, "3. SVN shouldn't be null");
			Assert.Null(svn.Angestellter, "3. Mitarbeiter should be null");
		}

		[Test]
		public void CompTestDeleteChildSave() 
		{			
			bool thrown = false;
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			pm.Save();
			try 
			{
				pm.Delete(svn); // Should throw an Exception
			}
			catch (NDOException e)
			{
				thrown = true;
			}
			Assert.That(thrown, "Exception m�sste ausgel�st worden sein");
		}


		[Test]
		public void CompTestDeleteAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			pm.Save();
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Deleted, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, svn.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, svn.NDOObjectState, "2. Wrong state");
			Assert.NotNull(m.SVN, "2. SVN not found");
			Assert.AreSame(m, svn.Angestellter, "2. Backlink wrong");
		}

		[Test]
		public void CompTestCreateDelete() {
			pm.MakePersistent(m);
			m.SVN = svn;
			pm.Delete(m);
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "2. Wrong state");
			// Objects should retain relations in memory
			Assert.NotNull(m.SVN, "3. SVN shouldn't be null");
			Assert.Null(svn.Angestellter, "3. Mitarbeiter should be null");
		}

		[Test]
		public void CompTestAddRemoveSave() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			m.SVN = null;
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "1. Wrong state");
			Assert.Null(m.SVN, "1. SVN should be null");
			Assert.Null(svn.Angestellter, "1. Mitarbeiter should be null");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "2. Wrong state");
			Assert.Null(m.SVN, "2. SVN should be null");
			Assert.Null(svn.Angestellter, "3. Mitarbeiter should be null");
		}

		[Test]
		public void CompTestAddRemoveAbort() {
			pm.MakePersistent(m);
			pm.Save();
			m.SVN = svn;
			m.SVN = null;
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, svn.NDOObjectState, "2. Wrong state");
			Assert.Null(m.SVN, "2. SVN should be null");
			Assert.Null(svn.Angestellter, "3. Mitarbeiter should be null");
		}



		[Test]
		public void CompTestHollow() {
			m.SVN = svn;
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeHollow(m); // setzt m.svn auf null
			
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Persistent, svn.NDOObjectState, "1: SVN should be persistent");

			svn = m.SVN; // ruft LoadData f�r m auf. m.svm liegt auf dem Cache und ist Persistent
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.AreEqual(NDOObjectState.Persistent, svn.NDOObjectState, "2: SVN should be persistent");
			ObjectId id = m.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			m = (Mitarbeiter) pm.FindObject(id);
			Assert.NotNull(m, "Mitarbeiter not found");
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "2: Mitarbeiter should be hollow");
			svn = m.SVN;
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "2: Mitarbeiter should be persistent");
			Assert.NotNull(svn, "SVN not found");
			Assert.AreEqual(NDOObjectState.Hollow, svn.NDOObjectState, "1: SVN should be hollow");
			Assert.AreSame(m, svn.Angestellter, "2. Backlink wrong");
		}


		[Test]
		public void  CompTestMakeAllHollow() {
			m.SVN = svn;
			pm.MakePersistent(m);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "1: Mitarbeiter should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, svn.NDOObjectState, "1: SVN should be hollow");
			Assert.AreSame(m, svn.Angestellter, "2. Backlink wrong");
		}

		[Test]
		public void  CompTestMakeAllHollowUnsaved() {
			m.SVN = svn;
			pm.MakePersistent(m);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "1: Mitarbeiter should be created");
			Assert.AreEqual(NDOObjectState.Created, svn.NDOObjectState, "1: SVN should be created");
		}

		[Test]
		public void CompTestLockRelations() {
			m.SVN = svn;
			pm.MakePersistent(m);
			m.Adresse = new Adresse();
			m.Adresse.Lkz = "Bla";
			pm.Save();
			pm.Delete(m);
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "1: mitarbeiter should be transient");
			Assert.AreEqual(NDOObjectState.Transient, m.SVN.NDOObjectState, "1: SVN should be transient");
			Assert.AreEqual(NDOObjectState.Transient, m.Adresse.NDOObjectState, "1: Adresse should be transient");
		}

		[Test]
		public void CompTestExtentRelatedObjects() {
			m.SVN = svn;
			pm.MakePersistent(m);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1: Mitarbeiter should be persistent");
			Assert.NotNull(m.SVN, "2. Relation is missing");
			Assert.AreEqual(NDOObjectState.Persistent, m.SVN.NDOObjectState, "2.: SVN should be hollow");
			Assert.AreSame(m, svn.Angestellter, "2. Backlink wrong");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter));
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "5: Mitarbeiter should be hollow");
			Assert.NotNull(m.SVN, "6. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, m.SVN.NDOObjectState, "8.: SVN should be hollow");
			Assert.That(m != svn.Angestellter, "8a. Should be different objects");
			Assert.AreSame(m, m.SVN.Angestellter, "8b. Mitarbeiter should match");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Mitarbeiter), false);
			m = (Mitarbeiter)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "9: Mitarbeiter should be persistent");
			Assert.NotNull(m.SVN, "10. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, m.SVN.NDOObjectState, "12.: SVN should be hollow");
			Assert.That(m != svn.Angestellter, "12a. Should be different objects");
			Assert.AreSame(m, m.SVN.Angestellter, "12b. Mitarbeiter should match");
		}

		#endregion

		#region Aggregation Tests

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void AggrTestCreateObjectsTransient() {
			e.Schlüssel = z;
			pm.MakePersistent(e);
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void AggrTestCreateObjectsTransient2() {
			pm.MakePersistent(e);
			e.Schlüssel = z;
		}

		[Test]
		public void AggrTestCreateObjectsSave() {
			pm.MakePersistent(z);
			e.Schlüssel = z;
			pm.MakePersistent(e);
			pm.Save();
			Assert.That(!e.NDOObjectId.Equals(e.Schlüssel.NDOObjectId), "Ids should be different");
			e = (Email)pm.FindObject(e.NDOObjectId);
			z = (Zertifikat)pm.FindObject(e.Schlüssel.NDOObjectId);
			Assert.NotNull(e, "1. Email not found");
			Assert.NotNull(z, "1. Zertifikat not found");
			ObjectId moid = e.NDOObjectId;
			ObjectId soid = z.NDOObjectId;
			e = null;
			z = null;

			pm.UnloadCache();
			e = (Email)pm.FindObject(moid);
			Zertifikat s2 = e.Schlüssel;
			z = (Zertifikat)pm.FindObject(soid);
			Assert.NotNull(e, "2. Email not found");
			Assert.NotNull(z, "2. Zertifikat not found");
			Assert.AreSame(z, s2, "Zertifikat should match");
			Assert.AreSame(e, z.Adresse, "Email should match");
		}

		[Test]
		public void AggrSimpleObjectSave() {
			pm.MakePersistent(z);
			pm.Save();
		}

		[Test]
		public void AggrChildAddFail() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			z.Adresse = e;
		}

		[Test]
		public void AggrChildAddFail2() {
			pm.MakePersistent(e);
			pm.MakePersistent(z);
			pm.Save();
			z.Adresse = e;
		}

		[Test]
		public void AggrChildAdd3() {
			pm.MakePersistent(e);
			pm.MakePersistent(z);
			pm.Save();
			e.Schlüssel = z;
			z.Adresse = e;
		}

		[Test]
		public void AggrChildAdd4() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			pm.Save();
			e.Schlüssel = z;
			z.Adresse = null;
		}

		[Test]
		public void AggrTestAddObjectSave() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			pm.Save();
			e.Schlüssel = z;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.AreSame(e, z.Adresse, "1. Backlink wrong");
			pm.Save();
			e = (Email)pm.FindObject(e.NDOObjectId);
			z = (Zertifikat)pm.FindObject(z.NDOObjectId);
			Assert.NotNull(e, "1. Email not found");
			Assert.NotNull(z, "1. Zertifikat not found");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			Assert.AreSame(e, z.Adresse, "2. Backlink wrong");
		}
			
		[Test]
		public void AggrTestAddObjectAbort() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			pm.Save();
			e.Schlüssel = z;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.NotNull(e.Schlüssel, "1. Zertifikat not found");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			Assert.Null(e.Schlüssel, "1. Zertifikat should be null");
			Assert.Null(z.Adresse, "1. Email should be null");
		}


		[Test]
		public void AggrTestReplaceChildSave() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Save();
			Assert.NotNull(e.Schlüssel, "1. Zertifikat not found");
			Zertifikat z2 = new Zertifikat();
			z2.Key = 0815;
			pm.MakePersistent(z2);
			e.Schlüssel = z2;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Created, z2.NDOObjectState, "2. Wrong state");
			Assert.Null(z.Adresse, "3. No relation to Email");
			Assert.AreSame(z2.Adresse, e, "4. Email should be same");
			Assert.AreSame(e.Schlüssel, z2, "5. Zertifikat should be same");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "6. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, z2.NDOObjectState, "7. Wrong state");
			Assert.Null(z.Adresse, "8. No relation to Email");
			Assert.AreSame(z2.Adresse, e, "9. Email should be same");
			Assert.AreSame(e.Schlüssel, z2, "10. Zertifikat should be same");
		}

		[Test]
		public void AggrTestReplaceChildAbort() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Save();
			Assert.NotNull(e.Schlüssel, "1. Zertifikat not found");
			Zertifikat z2 = new Zertifikat();
			z2.Key = 0815;
			pm.MakePersistent(z2);
			e.Schlüssel = z2;
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, z2.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			Assert.Null(z2.Adresse, "3. No relation to Email");
			Assert.AreSame(z.Adresse, e, "4. Email should be same");
			Assert.AreSame(e.Schlüssel, z, "5. Zertifikat should be same");
		}

		[Test]
		public void AggrTestReplaceParentSave() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Save();
			Assert.NotNull(e.Schlüssel, "1. Zertifikat not found");
			Email m2 = new Email("db@cortex-brainware.de");
			pm.MakePersistent(m2);
			z.Adresse = m2;
		}


		[Test]
		public void AggrTestRemoveObjectSave() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Save();
			Assert.NotNull(e.Schlüssel, "1. Zertifikat not found");
			ObjectId aoid = z.NDOObjectId;
			e.Schlüssel = null;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.Null(e.Schlüssel, "1. Zertifikat should be null");
			Assert.Null(z.Adresse, "1. Email should be null");
			pm.Save();
			Assert.Null(e.Schlüssel, "2. Zertifikat should be null");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			ObjectId moid = e.NDOObjectId;
			Assert.That(aoid.IsValid(), "Still valid Zertifikat");
			pm.UnloadCache();
			e = (Email)pm.FindObject(moid);
			Assert.NotNull(e, "3. Email not found");
			Assert.Null(e.Schlüssel, "3. Zertifikat should be null");
		}
		
		[Test]
		public void AggrTestRemoveObjectAbort() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Save();
			Assert.NotNull(e.Schlüssel, "1. Zertifikat not found");
			e.Schlüssel = null;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.Null(e.Schlüssel, "2. Zertifikat should be null");
			pm.Abort();
			Assert.NotNull(e.Schlüssel, "2. Zertifikat not found");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			Assert.AreSame(e, z.Adresse, "2. Backlink wrong");
		}

		[Test]
		public void AggrTestRemoveParentSave() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Save();
			Assert.NotNull(z.Adresse, "1. Email not found");
			ObjectId aoid = z.NDOObjectId;
			z.Adresse = null;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.Null(e.Schlüssel, "1. Zertifikat should be null");
			Assert.Null(z.Adresse, "1. Email should be null");
			pm.Save();
			Assert.Null(e.Schlüssel, "2. Zertifikat should be null");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			ObjectId moid = e.NDOObjectId;
			Assert.That(aoid.IsValid(), "Zertifikat still valid");
			pm.UnloadCache();
			e = (Email)pm.FindObject(moid);
			Assert.NotNull(e, "3. Email not found");
			Assert.Null(e.Schlüssel, "3. Zertifikat should be null");
		}

		[Test]
		public void AggrTestRemoveParentAbort() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			pm.Save();
			Assert.NotNull(e.Schlüssel, "1. Zertifikat not found");
			z.Adresse = null;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.Null(e.Schlüssel, "2. Zertifikat should be null");
			pm.Abort();
			Assert.NotNull(e.Schlüssel, "2. Zertifikat not found");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			Assert.AreSame(e, z.Adresse, "2. Backlink wrong");
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
		public void AggrTestCreateDeleteAllowed() {
			pm.MakePersistent(z);
			pm.MakePersistent(e);
			e.Schlüssel = z;
			e.Schlüssel = null;
			pm.Delete(e); 
			Assert.AreEqual(NDOObjectState.Created, z.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Transient, e.NDOObjectState, "2. Wrong state");
			Assert.Null(e.Schlüssel, "3. Zertifikat should be null");
			Assert.Null(z.Adresse, "4. Email should be null");
		}


		[Test]
		public void AggrTestAddRemoveSave() {
			pm.MakePersistent(e);
			pm.MakePersistent(z);
			pm.Save();
			e.Schlüssel = z;
			e.Schlüssel = null;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			Assert.Null(e.Schlüssel, "1. Zertifikat should be null");
			Assert.Null(z.Adresse, "1. Email should be null");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			Assert.Null(e.Schlüssel, "2. Zertifikat should be null");
			Assert.Null(z.Adresse, "3. Email should be null");
		}

		[Test]
		public void AggrTestAddRemoveAbort() {
			pm.MakePersistent(e);
			pm.MakePersistent(z);
			pm.Save();
			e.Schlüssel = z;
			e.Schlüssel = null;
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2. Wrong state");
			Assert.Null(e.Schlüssel, "2. Zertifikat should be null");
			Assert.Null(z.Adresse, "3. Email should be null");
		}



		[Test]
		public void AggrTestHollow() {
			pm.MakePersistent(z);
			e.Schlüssel = z;
			pm.MakePersistent(e);
			pm.Save();
			pm.MakeHollow(e); // setzt e.z auf null
			
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "1: Email should be hollow");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "1: Zertifikat should be persistent");

			z = e.Schlüssel; // ruft LoadData f�r e auf. e.svm liegt auf dem Cache und ist Persistent
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "1: Email should be persistent");
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2: Zertifikat should be persistent");
			ObjectId id = e.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			e = (Email) pm.FindObject(id);
			Assert.NotNull(e, "Email not found");
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "2: Email should be hollow");
			z = e.Schlüssel;
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "2: Email should be persistent");
			Assert.NotNull(z, "Zertifikat not found");
			Assert.AreEqual(NDOObjectState.Hollow, z.NDOObjectState, "1: Zertifikat should be hollow");
			Assert.AreSame(e, z.Adresse, "2. Backlink wrong");
		}


		[Test]
		public void  AggrTestMakeAllHollowDelete() {
			pm.MakePersistent(z);
			e.Schlüssel = z;
			pm.MakePersistent(e);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "1: Email should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, z.NDOObjectState, "1: Zertifikat should be hollow");
			Zertifikat zz = e.Schlüssel;  // Das l�dt das Objekt, deshalb geht es hier!
			e.Schlüssel = null;
			Assert.AreEqual(NDOObjectState.PersistentDirty, e.NDOObjectState, "2: Email should be persistent dirty");
			Assert.AreEqual(NDOObjectState.PersistentDirty, z.NDOObjectState, "2: Zertifikat should be persistent dirty");
			Assert.Null(e.Schlüssel, "3. Email should have no Zertifikat");
			Assert.Null(z.Adresse, "3. Zertifikat should have no Email");
			pm.Delete(e);
		}

		[Test]
		public void  AggrTestMakeAllHollow() {
			pm.MakePersistent(z);
			e.Schlüssel = z;
			pm.MakePersistent(e);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "1: Email should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, z.NDOObjectState, "1: Zertifikat should be hollow");
			Assert.AreSame(e, z.Adresse, "2. Backlink wrong");
		}

		[Test]
		public void  AggrTestMakeAllHollowUnsaved() {
			pm.MakePersistent(z);
			e.Schlüssel = z;
			pm.MakePersistent(e);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, e.NDOObjectState, "1: Email should be created");
			Assert.AreEqual(NDOObjectState.Created, z.NDOObjectState, "1: Zertifikat should be created");
		}


		[Test]
		public void AggrTestExtentRelatedObjects() {
			pm.MakePersistent(z);
			e.Schlüssel = z;
			pm.MakePersistent(e);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Email));
			e = (Email)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "1: Email should be persistent");
			Assert.NotNull(e.Schlüssel, "2. Relation is missing");
			Assert.AreEqual(NDOObjectState.Persistent, e.Schlüssel.NDOObjectState, "2.: Zertifikat should be hollow");
			Assert.AreSame(e, z.Adresse, "2. Backlink wrong");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Email));
			e = (Email)liste[0];
			Assert.AreEqual(NDOObjectState.Hollow, e.NDOObjectState, "5: Email should be hollow");
			Assert.NotNull(e.Schlüssel, "6. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, e.Schlüssel.NDOObjectState, "8.: Zertifikat should be hollow");
			Assert.That(e != z.Adresse, "8a. Should be different objects");
			Assert.AreSame(e, e.Schlüssel.Adresse, "8b. Email should match");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Email), false);
			e = (Email)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, e.NDOObjectState, "9: Email should be persistent");
			Assert.NotNull(e.Schlüssel, "10. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, e.Schlüssel.NDOObjectState, "12.: Zertifikat should be hollow");
			Assert.That(e != z.Adresse, "12a. Should be different objects");
			Assert.AreSame(e, e.Schlüssel.Adresse, "12b. Email should match");
		}

		[Test]
		public void BiTest()
		{
			pm.MakePersistent(e);
			pm.MakePersistent(z);
//			System.Diagnostics.Debug.WriteLine("e:" + e.NDOObjectId.ExpressionString);
//			System.Diagnostics.Debug.WriteLine("z:" + z.NDOObjectId.ExpressionString);
			e.Schlüssel = this.z;
			pm.Save();
//			System.Diagnostics.Debug.WriteLine("e:" + e.NDOObjectId.ExpressionString);
//			System.Diagnostics.Debug.WriteLine("z:" + z.NDOObjectId.ExpressionString);
			
		}


		#endregion

		private void SetupEmail() {
			pm.MakePersistent(e);
			pm.Save();
		}

		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) {
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}
	}
}
