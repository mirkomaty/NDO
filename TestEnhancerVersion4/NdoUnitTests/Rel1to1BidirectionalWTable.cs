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
	/// All tests for 1:1-Relations. Bidirectional Composition and Aggregation with mapping table.
	/// </summary>


	[TestFixture]
	public class Rel1to1BidirectionalWTable {
		public Rel1to1BidirectionalWTable() {
		}

		private PersistenceManager pm;
		private Zertifikat z;
		private Signatur sgn;

		[SetUp]
		public void Setup() {
			pm = PmFactory.NewPersistenceManager();
			z = CreateZertifikat(0815);
			sgn = new Signatur();
			sgn.Key = "Signiert";
		}

		[TearDown]
		public void TearDown() {
			pm.Abort();
			IList zertifikatListe = pm.GetClassExtent(typeof(Zertifikat), true);
			pm.Delete(zertifikatListe);
			IList sListe = pm.GetClassExtent(typeof(Signatur), true);
			pm.Delete(sListe);
			pm.Save();
			pm.Close();
			pm = null;
		}



		#region Composition Tests

		[Test]
		public void CompTestCreateObjectsSave() {
			z.SGN = sgn;
			pm.MakePersistent(z);
			pm.Save();
			Assert.That(!z.NDOObjectId.Equals(z.SGN.NDOObjectId), "Ids should be different");
			z = (Zertifikat)pm.FindObject(z.NDOObjectId);
			sgn = (Signatur)pm.FindObject(z.SGN.NDOObjectId);
			Assert.NotNull(z, "1. Zertifikat not found");
			Assert.NotNull(sgn, "1. SGN not found");
			ObjectId moid = z.NDOObjectId;
			ObjectId soid = sgn.NDOObjectId;
			z = null;
			sgn = null;

			pm.UnloadCache();
			z = (Zertifikat)pm.FindObject(moid);
			Signatur s2 = z.SGN;
			sgn = (Signatur)pm.FindObject(soid);
			Assert.NotNull(z, "2. Zertifikat not found");
			Assert.NotNull(sgn, "2. SGN not found");
			Assert.AreSame(sgn, s2, "SGN should match");
			Assert.AreSame(z, sgn.Owner, "Zertifikat should match");
		}

		[Test]
		public void SimpleObjectSave() {
			pm.MakePersistent(sgn);
			pm.Save();
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompChildAddFail() {
			pm.MakePersistent(sgn);
			sgn.Owner = z;
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompChildAddFail2() {
			pm.MakePersistent(sgn);
			pm.Save();
			sgn.Owner = z;
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompChildAdd3() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			sgn.Owner = z;  // Cannot manipulate composition relation through child
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompChildAdd4() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			sgn.Owner = null;  // Cannot manipulate composition relation through child
		}

		[Test]
		public void CompTestAddObjectSave() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			Assert.AreEqual(NDOObjectState.Created, sgn.NDOObjectState, "1. Wrong state");
			Assert.AreSame(z, sgn.Owner, "1. Backlink wrong");
			pm.Save();
			z = (Zertifikat)pm.FindObject(z.NDOObjectId);
			sgn = (Signatur)pm.FindObject(sgn.NDOObjectId);
			Assert.NotNull(z, "1. Zertifikat not found");
			Assert.NotNull(sgn, "1. SGN not found");
			Assert.AreEqual(NDOObjectState.Persistent, sgn.NDOObjectState, "2. Wrong state");
			Assert.AreSame(z, sgn.Owner, "2. Backlink wrong");
		}
			
		[Test]
		public void CompTestAddObjectAbort() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			Assert.AreEqual(NDOObjectState.Created, sgn.NDOObjectState, "1. Wrong state");
			Assert.NotNull(z.SGN, "1. SGN not found");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "2. Wrong state");
			Assert.Null(z.SGN, "1. SGN should be null");
			Assert.Null(sgn.Owner, "1. Zertifikat should be null");
		}


		[Test]
		public void CompTestReplaceChildSave() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			pm.Save();
			Assert.NotNull(z.SGN, "1. SGN not found");
			Signatur svn2 = new Signatur();
			svn2.Key = "VeriSign";
			z.SGN = svn2;
			Assert.AreEqual(NDOObjectState.Deleted, sgn.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Created, svn2.NDOObjectState, "2. Wrong state");
			Assert.Null(sgn.Owner, "3. No relation to Zertifikat");
			Assert.AreSame(svn2.Owner, z, "4. Zertifikat should be same");
			Assert.AreSame(z.SGN, svn2, "5. SGN should be same");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "6. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, svn2.NDOObjectState, "7. Wrong state");
			Assert.Null(sgn.Owner, "8. No relation to Zertifikat");
			Assert.AreSame(svn2.Owner, z, "9. Zertifikat should be same");
			Assert.AreSame(z.SGN, svn2, "10. SGN should be same");
		}

		[Test]
		public void CompTestReplaceChildAbort() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			pm.Save();
			Assert.NotNull(z.SGN, "1. SGN not found");
			Signatur svn2 = new Signatur();
			svn2.Key = "VeriSign";
			z.SGN = svn2;
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, svn2.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, sgn.NDOObjectState, "2. Wrong state");
			Assert.Null(svn2.Owner, "3. No relation to Zertifikat");
			Assert.AreSame(sgn.Owner, z, "4. Zertifikat should be same");
			Assert.AreSame(z.SGN, sgn, "5. SGN should be same");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompTestReplaceParentSave() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			pm.Save();
			Assert.NotNull(z.SGN, "1. SGN not found");
			Zertifikat m2 = CreateZertifikat(3345);
			sgn.Owner = m2;
		}


		[Test]
		public void CompTestRemoveObjectSave() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			pm.Save();
			Assert.NotNull(z.SGN, "1. SGN not found");
			ObjectId aoid = sgn.NDOObjectId;
			z.SGN = null;
			Assert.AreEqual(NDOObjectState.Deleted, sgn.NDOObjectState, "1. Wrong state");
			Assert.Null(z.SGN, "1. SGN should be null");
			Assert.Null(sgn.Owner, "1. Zertifikat should be null");
			pm.Save();
			Assert.Null(z.SGN, "2. SGN should be null");
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "2. Wrong state");
			ObjectId moid = z.NDOObjectId;
			Assert.That(!aoid.IsValid());
			pm.UnloadCache();
			z = (Zertifikat)pm.FindObject(moid);
			Assert.NotNull(z, "3. Zertifikat not found");
			Assert.Null(z.SGN, "3. SGN should be null");
		}
		
		[Test]
		public void CompTestRemoveObjectAbort() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			pm.Save();
			Assert.NotNull(z.SGN, "1. SGN not found");
			z.SGN = null;
			Assert.AreEqual(NDOObjectState.Deleted, sgn.NDOObjectState, "1. Wrong state");
			Assert.Null(z.SGN, "2. SGN should be null");
			pm.Abort();
			Assert.NotNull(z.SGN, "2. SGN not found");
			Assert.AreEqual(NDOObjectState.Persistent, sgn.NDOObjectState, "2. Wrong state");
			Assert.AreSame(z, sgn.Owner, "2. Backlink wrong");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompTestRemoveParent() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			pm.Save();
			Assert.NotNull(sgn.Owner, "1. Zertifikat not found");
			ObjectId aoid = sgn.NDOObjectId;
			sgn.Owner = null;  	// Cannot manipulate composition relation through child
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompTestRemoveParentBeforeSave() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			sgn.Owner = null;  	// Cannot manipulate composition relation through child
		}


		[Test]
		public void CompTestDeleteSave() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			pm.Save();
			pm.Delete(z);
			Assert.AreEqual(NDOObjectState.Deleted, z.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, sgn.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, z.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "2. Wrong state");
			// Objects should retain relations in memory
			Assert.NotNull(z.SGN, "3. SGN shouldn't be null");
			Assert.Null(sgn.Owner, "3. Zertifikat should be null");
		}

		[Test]
		[ExpectedException(typeof(NDOException))]
		public void CompTestDeleteChildSave() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			pm.Save();
			pm.Delete(sgn);
			pm.Save();
		}


		[Test]
		public void CompTestDeleteAbort() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			pm.Save();
			pm.Delete(z);
			Assert.AreEqual(NDOObjectState.Deleted, z.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Deleted, sgn.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Persistent, sgn.NDOObjectState, "2. Wrong state");
			Assert.NotNull(z.SGN, "2. SGN not found");
			Assert.AreSame(z, sgn.Owner, "2. Backlink wrong");
		}

		[Test]
		public void CompTestCreateDelete() {
			pm.MakePersistent(z);
			z.SGN = sgn;
			pm.Delete(z);
			Assert.AreEqual(NDOObjectState.Transient, z.NDOObjectState, "1. Wrong state");
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "2. Wrong state");
			// Objects should retain relations in memory
			Assert.NotNull(z.SGN, "3. SGN shouldn't be null");
			Assert.Null(sgn.Owner, "3. Zertifikat should be null");
		}

		[Test]
		public void CompTestAddRemoveSave() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			z.SGN = null;
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "1. Wrong state");
			Assert.Null(z.SGN, "1. SGN should be null");
			Assert.Null(sgn.Owner, "1. Zertifikat should be null");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "2. Wrong state");
			Assert.Null(z.SGN, "2. SGN should be null");
			Assert.Null(sgn.Owner, "3. Zertifikat should be null");
		}

		[Test]
		public void CompTestAddRemoveAbort() {
			pm.MakePersistent(z);
			pm.Save();
			z.SGN = sgn;
			z.SGN = null;
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.AreEqual(NDOObjectState.Transient, sgn.NDOObjectState, "2. Wrong state");
			Assert.Null(z.SGN, "2. SGN should be null");
			Assert.Null(sgn.Owner, "3. Zertifikat should be null");
		}



		[Test]
		public void CompTestHollow() {
			z.SGN = sgn;
			pm.MakePersistent(z);
			pm.Save();
			pm.MakeHollow(z); // setzt z.sgn auf null
			
			Assert.AreEqual(NDOObjectState.Hollow, z.NDOObjectState, "1: Zertifikat should be hollow");
			Assert.AreEqual(NDOObjectState.Persistent, sgn.NDOObjectState, "1: SGN should be persistent");

			sgn = z.SGN; // ruft LoadData f�r z auf. z.svm liegt auf dem Cache und ist Persistent
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "1: Zertifikat should be persistent");
			Assert.AreEqual(NDOObjectState.Persistent, sgn.NDOObjectState, "2: SGN should be persistent");
			ObjectId id = z.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			z = (Zertifikat) pm.FindObject(id);
			Assert.NotNull(z, "Zertifikat not found");
			Assert.AreEqual(NDOObjectState.Hollow, z.NDOObjectState, "2: Zertifikat should be hollow");
			sgn = z.SGN;
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "2: Zertifikat should be persistent");
			Assert.NotNull(sgn, "SGN not found");
			Assert.AreEqual(NDOObjectState.Hollow, sgn.NDOObjectState, "1: SGN should be hollow");
			Assert.AreSame(z, sgn.Owner, "2. Backlink wrong");
		}


		[Test]
		public void  CompTestMakeAllHollow() {
			z.SGN = sgn;
			pm.MakePersistent(z);
			pm.Save();
			pm.MakeAllHollow();
			Assert.AreEqual(NDOObjectState.Hollow, z.NDOObjectState, "1: Zertifikat should be hollow");
			Assert.AreEqual(NDOObjectState.Hollow, sgn.NDOObjectState, "1: SGN should be hollow");
			Assert.AreSame(z, sgn.Owner, "2. Backlink wrong");
		}

		[Test]
		public void  CompTestMakeAllHollowUnsaved() {
			z.SGN = sgn;
			pm.MakePersistent(z);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.AreEqual(NDOObjectState.Created, z.NDOObjectState, "1: Zertifikat should be created");
			Assert.AreEqual(NDOObjectState.Created, sgn.NDOObjectState, "1: SGN should be created");
		}


		[Test]
		public void CompTestExtentRelatedObjects() {
			z.SGN = sgn;
			pm.MakePersistent(z);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Zertifikat));
			z = (Zertifikat)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "1: Zertifikat should be persistent");
			Assert.NotNull(z.SGN, "2. Relation is missing");
			Assert.AreEqual(NDOObjectState.Persistent, z.SGN.NDOObjectState, "2.: SGN should be hollow");
			Assert.AreSame(z, sgn.Owner, "2. Backlink wrong");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Zertifikat));
			z = (Zertifikat)liste[0];
			Assert.AreEqual(NDOObjectState.Hollow, z.NDOObjectState, "5: Zertifikat should be hollow");
			Assert.NotNull(z.SGN, "6. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, z.SGN.NDOObjectState, "8.: Key should be hollow");
			Assert.That(z != sgn.Owner, "8a. Should be different objects");
			Assert.AreSame(z, z.SGN.Owner, "8b. Zertifikat should match");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Zertifikat), false);
			z = (Zertifikat)liste[0];
			Assert.AreEqual(NDOObjectState.Persistent, z.NDOObjectState, "9: Zertifikat should be persistent");
			Assert.NotNull(z.SGN, "10. Relation is missing");
			Assert.AreEqual(NDOObjectState.Hollow, z.SGN.NDOObjectState, "12.: Key should be hollow");
			Assert.That(z != sgn.Owner, "12a. Should be different objects");
			Assert.AreSame(z, z.SGN.Owner, "12b. Zertifikat should match");
		}

		#endregion

				

		private Zertifikat CreateZertifikat(int k) {
			Zertifikat z = new Zertifikat();
			z.Key = k;
			return z;
		}
	}
}
