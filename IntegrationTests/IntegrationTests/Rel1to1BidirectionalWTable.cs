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

namespace NdoUnitTests
{
	/// <summary>
	/// All tests for 1:1-Relations. Bidirectional Composition and Aggregation with mapping table.
	/// </summary>


	[TestFixture]
	public class Rel1to1BidirectionalWTable
	{
		public Rel1to1BidirectionalWTable()
		{
		}

		private PersistenceManager pm;
		private Zertifikat z;
		private Signatur sgn;

		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			z = CreateZertifikat( 0815 );
			sgn = new Signatur();
			sgn.Key = "Signiert";
		}

		[TearDown]
		public void TearDown()
		{
			pm.Abort();
			IList zertifikatListe = pm.GetClassExtent( typeof( Zertifikat ), true );
			pm.Delete( zertifikatListe );
			IList sListe = pm.GetClassExtent( typeof( Signatur ), true );
			pm.Delete( sListe );
			pm.Save();
			pm.Close();
			pm = null;
		}



		#region Composition Tests

		[Test]
		public void CompTestCreateObjectsSave()
		{
			z.SGN = sgn;
			pm.MakePersistent( z );
			pm.Save();
			Assert.That( !z.NDOObjectId.Equals( z.SGN.NDOObjectId ), "Ids should be different" );
			z = (Zertifikat)pm.FindObject( z.NDOObjectId );
			sgn = (Signatur)pm.FindObject( z.SGN.NDOObjectId );
			Assert.That(z != null, "1. Zertifikat not found" );
			Assert.That(sgn != null, "1. SGN not found" );
			ObjectId moid = z.NDOObjectId;
			ObjectId soid = sgn.NDOObjectId;
			z = null;
			sgn = null;

			pm.UnloadCache();
			z = (Zertifikat)pm.FindObject( moid );
			Signatur s2 = z.SGN;
			sgn = (Signatur)pm.FindObject( soid );
			Assert.That(z != null, "2. Zertifikat not found" );
			Assert.That(sgn != null, "2. SGN not found" );
			Assert.That(Object.ReferenceEquals(sgn, s2), "SGN should match" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "Zertifikat should match" );
		}

		[Test]
		public void SimpleObjectSave()
		{
			pm.MakePersistent( sgn );
			pm.Save();
		}

		[Test]
		public void CompChildAddFail()
		{
			bool thrown = false;
			pm.MakePersistent( sgn );
			try
			{
				sgn.Owner = z;
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
			bool thrown = false;
			pm.MakePersistent( sgn );
			pm.Save();
			try
			{
				sgn.Owner = z;
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
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			var thrown = false;
			try
			{
				sgn.Owner = z;  // Cannot manipulate composition relation through child
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
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			var thrown = false;
			try
			{
				sgn.Owner = null;  // Cannot manipulate composition relation through child
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
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			Assert.That(NDOObjectState.Created ==  sgn.NDOObjectState, "1. Wrong state" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "1. Backlink wrong" );
			pm.Save();
			z = (Zertifikat)pm.FindObject( z.NDOObjectId );
			sgn = (Signatur)pm.FindObject( sgn.NDOObjectId );
			Assert.That(z != null, "1. Zertifikat not found" );
			Assert.That(sgn != null, "1. SGN not found" );
			Assert.That(NDOObjectState.Persistent ==  sgn.NDOObjectState, "2. Wrong state" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestAddObjectAbort()
		{
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			Assert.That(NDOObjectState.Created ==  sgn.NDOObjectState, "1. Wrong state" );
			Assert.That(z.SGN != null, "1. SGN not found" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "2. Wrong state" );
			Assert.That(z.SGN == null, "1. SGN should be null" );
			Assert.That(sgn.Owner == null, "1. Zertifikat should be null" );
		}


		[Test]
		public void CompTestReplaceChildSave()
		{
			pm.MakePersistent( z );
			z.SGN = sgn;
			pm.Save();
			Assert.That(z.SGN != null, "1. SGN not found" );
			Signatur svn2 = new Signatur();
			svn2.Key = "VeriSign";
			z.SGN = svn2;
			Assert.That(NDOObjectState.Deleted ==  sgn.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Created ==  svn2.NDOObjectState, "2. Wrong state" );
			Assert.That(sgn.Owner == null, "3. No relation to Zertifikat" );
			Assert.That(Object.ReferenceEquals(svn2.Owner, z), "4. Zertifikat should be same" );
			Assert.That(Object.ReferenceEquals(z.SGN, svn2), "5. SGN should be same" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "6. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  svn2.NDOObjectState, "7. Wrong state" );
			Assert.That(sgn.Owner == null, "8. No relation to Zertifikat" );
			Assert.That(Object.ReferenceEquals(svn2.Owner, z), "9. Zertifikat should be same" );
			Assert.That(Object.ReferenceEquals(z.SGN, svn2), "10. SGN should be same" );
		}

		[Test]
		public void CompTestReplaceChildAbort()
		{
			pm.MakePersistent( z );
			z.SGN = sgn;
			pm.Save();
			Assert.That(z.SGN != null, "1. SGN not found" );
			Signatur svn2 = new Signatur();
			svn2.Key = "VeriSign";
			z.SGN = svn2;
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  svn2.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  sgn.NDOObjectState, "2. Wrong state" );
			Assert.That(svn2.Owner == null, "3. No relation to Zertifikat" );
			Assert.That(Object.ReferenceEquals(sgn.Owner, z), "4. Zertifikat should be same" );
			Assert.That(Object.ReferenceEquals(z.SGN, sgn), "5. SGN should be same" );
		}

		[Test]
		public void CompTestReplaceParentSave()
		{
			pm.MakePersistent( z );
			z.SGN = sgn;
			pm.Save();
			Assert.That(z.SGN != null, "1. SGN not found" );
			Zertifikat m2 = CreateZertifikat( 3345 );
			var thrown = false;
			try
			{
				sgn.Owner = m2;
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
			pm.MakePersistent( z );
			z.SGN = sgn;
			pm.Save();
			Assert.That(z.SGN != null, "1. SGN not found" );
			z.SGN = null;
			Assert.That(NDOObjectState.Deleted ==  sgn.NDOObjectState, "1. Wrong state" );
			Assert.That(z.SGN == null, "1. SGN should be null" );
			Assert.That(sgn.Owner == null, "1. Zertifikat should be null" );
			pm.Save();
			Assert.That(z.SGN == null, "2. SGN should be null" );
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "2. Wrong state" );
			ObjectId moid = z.NDOObjectId;
			pm.UnloadCache();
			z = (Zertifikat)pm.FindObject( moid );
			Assert.That(z != null, "3. Zertifikat not found" );
			Assert.That(z.SGN == null, "3. SGN should be null" );
		}

		[Test]
		public void CompTestRemoveObjectAbort()
		{
			pm.MakePersistent( z );
			z.SGN = sgn;
			pm.Save();
			Assert.That(z.SGN != null, "1. SGN not found" );
			z.SGN = null;
			Assert.That(NDOObjectState.Deleted ==  sgn.NDOObjectState, "1. Wrong state" );
			Assert.That(z.SGN == null, "2. SGN should be null" );
			pm.Abort();
			Assert.That(z.SGN != null, "2. SGN not found" );
			Assert.That(NDOObjectState.Persistent ==  sgn.NDOObjectState, "2. Wrong state" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestRemoveParent()
		{
			pm.MakePersistent( z );
			z.SGN = sgn;
			pm.Save();
			Assert.That(sgn.Owner != null, "1. Zertifikat not found" );
			ObjectId aoid = sgn.NDOObjectId;
			var thrown = false;
			try
			{
				sgn.Owner = null;   // Cannot manipulate composition relation through child
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}

		[Test]
		public void CompTestRemoveParentBeforeSave()
		{
			pm.MakePersistent( z );
			z.SGN = sgn;
			var thrown = false;
			try
			{
				sgn.Owner = null;   // Cannot manipulate composition relation through child
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
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			pm.Save();
			pm.Delete( z );
			Assert.That(NDOObjectState.Deleted ==  z.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  sgn.NDOObjectState, "2. Wrong state" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  z.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "2. Wrong state" );
			// Objects should retain relations in memory
			Assert.That(z.SGN != null, "3. SGN shouldn't be null" );
			Assert.That(sgn.Owner == null, "3. Zertifikat should be null" );
		}

		[Test]
		public void CompTestDeleteChildSave()
		{
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			pm.Save();
			var thrown = false;
			try
			{
				pm.Delete( sgn );
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.That(true ==  thrown );
		}


		[Test]
		public void CompTestDeleteAbort()
		{
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			pm.Save();
			pm.Delete( z );
			Assert.That(NDOObjectState.Deleted ==  z.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Deleted ==  sgn.NDOObjectState, "2. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  z.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Persistent ==  sgn.NDOObjectState, "2. Wrong state" );
			Assert.That(z.SGN != null, "2. SGN not found" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestCreateDelete()
		{
			pm.MakePersistent( z );
			z.SGN = sgn;
			pm.Delete( z );
			Assert.That(NDOObjectState.Transient ==  z.NDOObjectState, "1. Wrong state" );
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "2. Wrong state" );
			// Objects should retain relations in memory
			Assert.That(z.SGN != null, "3. SGN shouldn't be null" );
			Assert.That(sgn.Owner == null, "3. Zertifikat should be null" );
		}

		[Test]
		public void CompTestAddRemoveSave()
		{
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			z.SGN = null;
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "1. Wrong state" );
			Assert.That(z.SGN == null, "1. SGN should be null" );
			Assert.That(sgn.Owner == null, "1. Zertifikat should be null" );
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "2. Wrong state" );
			Assert.That(z.SGN == null, "2. SGN should be null" );
			Assert.That(sgn.Owner == null, "3. Zertifikat should be null" );
		}

		[Test]
		public void CompTestAddRemoveAbort()
		{
			pm.MakePersistent( z );
			pm.Save();
			z.SGN = sgn;
			z.SGN = null;
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "1. Wrong state" );
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  sgn.NDOObjectState, "2. Wrong state" );
			Assert.That(z.SGN == null, "2. SGN should be null" );
			Assert.That(sgn.Owner == null, "3. Zertifikat should be null" );
		}



		[Test]
		public void CompTestHollow()
		{
			z.SGN = sgn;
			pm.MakePersistent( z );
			pm.Save();
			pm.MakeHollow( z ); // setzt z.sgn auf null

			Assert.That(NDOObjectState.Hollow ==  z.NDOObjectState, "1: Zertifikat should be hollow" );
			Assert.That(NDOObjectState.Persistent ==  sgn.NDOObjectState, "1: SGN should be persistent" );

			sgn = z.SGN; // ruft LoadData för z auf. z.svm liegt auf dem Cache und ist Persistent
			Assert.That(NDOObjectState.Persistent ==  z.NDOObjectState, "1: Zertifikat should be persistent" );
			Assert.That(NDOObjectState.Persistent ==  sgn.NDOObjectState, "2: SGN should be persistent" );
			ObjectId id = z.NDOObjectId;
			pm.Close();
			pm = PmFactory.NewPersistenceManager();
			z = (Zertifikat)pm.FindObject( id );
			Assert.That(z != null, "Zertifikat not found" );
			Assert.That(NDOObjectState.Hollow ==  z.NDOObjectState, "2: Zertifikat should be hollow" );
			sgn = z.SGN;
			Assert.That(NDOObjectState.Persistent ==  z.NDOObjectState, "2: Zertifikat should be persistent" );
			Assert.That(sgn != null, "SGN not found" );
			Assert.That(NDOObjectState.Hollow ==  sgn.NDOObjectState, "1: SGN should be hollow" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "2. Backlink wrong" );
		}


		[Test]
		public void CompTestMakeAllHollow()
		{
			z.SGN = sgn;
			pm.MakePersistent( z );
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  z.NDOObjectState, "1: Zertifikat should be hollow" );
			Assert.That(NDOObjectState.Hollow ==  sgn.NDOObjectState, "1: SGN should be hollow" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "2. Backlink wrong" );
		}

		[Test]
		public void CompTestMakeAllHollowUnsaved()
		{
			z.SGN = sgn;
			pm.MakePersistent( z );
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  z.NDOObjectState, "1: Zertifikat should be created" );
			Assert.That(NDOObjectState.Created ==  sgn.NDOObjectState, "1: SGN should be created" );
		}


		[Test]
		public void CompTestExtentRelatedObjects()
		{
			z.SGN = sgn;
			pm.MakePersistent( z );
			pm.Save();
			IList liste = pm.GetClassExtent( typeof( Zertifikat ) );
			z = (Zertifikat)liste[0];
			Assert.That(NDOObjectState.Persistent ==  z.NDOObjectState, "1: Zertifikat should be persistent" );
			Assert.That(z.SGN != null, "2. Relation is missing" );
			Assert.That(NDOObjectState.Persistent ==  z.SGN.NDOObjectState, "2.: SGN should be hollow" );
			Assert.That(Object.ReferenceEquals(z, sgn.Owner), "2. Backlink wrong" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Zertifikat ) );
			z = (Zertifikat)liste[0];
			Assert.That(NDOObjectState.Hollow ==  z.NDOObjectState, "5: Zertifikat should be hollow" );
			Assert.That(z.SGN != null, "6. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  z.SGN.NDOObjectState, "8.: Key should be hollow" );
			Assert.That( z != sgn.Owner, "8a. Should be different objects" );
			Assert.That(Object.ReferenceEquals(z, z.SGN.Owner), "8b. Zertifikat should match" );

			pm.UnloadCache();
			liste = pm.GetClassExtent( typeof( Zertifikat ), false );
			z = (Zertifikat)liste[0];
			Assert.That(NDOObjectState.Persistent ==  z.NDOObjectState, "9: Zertifikat should be persistent" );
			Assert.That(z.SGN != null, "10. Relation is missing" );
			Assert.That(NDOObjectState.Hollow ==  z.SGN.NDOObjectState, "12.: Key should be hollow" );
			Assert.That( z != sgn.Owner, "12a. Should be different objects" );
			Assert.That(Object.ReferenceEquals(z, z.SGN.Owner), "12b. Zertifikat should match" );
		}

		#endregion



		private Zertifikat CreateZertifikat( int k )
		{
			Zertifikat z = new Zertifikat();
			z.Key = k;
			return z;
		}
	}
}
