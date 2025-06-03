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
using System.Data;
using System.Reflection;
using System.Collections;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using NDOObjectIdTestClasses;
using NDOInterfaces;
using NDO.Query;
using NDO.ProviderFactory;

namespace NdoUnitTests
{
	[TestFixture]
	public class NDOObjectIdTests : NDOTest
	{
		PersistenceManager pm;
		Guid guid;

		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			pm.IdGenerationEvent += new IdGenerationHandler(OnIdGeneration);
			guid = new Guid(1,2,3,4,5,6,7,8,9,10,11);
		}

		[TearDown]
		public void TearDown()
		{			
			IList l = pm.GetClassExtent(typeof(ObjectOwner));
			pm.Delete(l);
			pm.Save();
			l = pm.GetClassExtent(typeof(NDOoidAndHandler));
			pm.Delete(l);
			pm.Save();
			l = pm.GetClassExtent(typeof(HintOwner));
			pm.Delete(l);
			pm.Save();
			l = pm.GetClassExtent(typeof(ClassWithHint));
			pm.Delete(l);
			pm.Save();
			l = pm.GetClassExtent(typeof(DerivedGuid));
			pm.Delete(l);
			pm.Save();
			pm.Dispose();
		}

		[Test]
		public void OidTest()
		{
			NDOoidAndHandler testobj = new NDOoidAndHandler();
			testobj.MyId = guid;
			testobj.Text = "Test";
			pm.MakePersistent(testobj);
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<NDOoidAndHandler>(pm);
			testobj = null;
			testobj = (NDOoidAndHandler) q.ExecuteSingle(true);
			Assert.That(guid ==  testobj.MyId, "Wrong guid");
			Assert.That("Test" ==  testobj.Text, "Wrong text");

			testobj.Text = "Neuer Text";
			pm.Save();
			testobj = null;
			testobj = (NDOoidAndHandler) q.ExecuteSingle(true);
			Assert.That(guid ==  testobj.MyId, "Wrong guid");
			Assert.That("Neuer Text" ==  testobj.Text, "Wrong text");
			
			pm.Delete(testobj);
			pm.Save();
		}

		[Test]
		public void TestForeignKey()
		{
			ObjectOwner owner = new ObjectOwner();
			pm.MakePersistent(owner);
			owner.Element = new NDOoidAndHandler();
			owner.Element.Text = "Text";
			pm.Save();
			owner = null;
			pm.UnloadCache();
			IQuery q = new NDOQuery<ObjectOwner>(pm);
			owner = (ObjectOwner) q.ExecuteSingle(true);
			Assert.That(owner.Element != null, "No element");
			Assert.That(guid ==  owner.Element.MyId, "Wrong guid");
			Assert.That("Text" ==  owner.Element.Text, "Wrong text");
		}


		[Test]
		public void TestHintForeignKey()
		{
			HintOwner owner = new HintOwner();
			pm.MakePersistent(owner);
			owner.Element = new ClassWithHint();
			owner.Element.Text = "Text";
			pm.Save();
			owner = null;
			pm.UnloadCache();
			IQuery q = new NDOQuery<HintOwner>(pm);
			owner = (HintOwner) q.ExecuteSingle(true);
			Assert.That(owner.Element != null, "No element");
			Assert.That(guid == (Guid) owner.Element.NDOObjectId.Id[0], "Wrong guid");
			Assert.That("Text" == owner.Element.Text, "Wrong text");
		}


		[Test]
		public void HintEnhancerTest()
		{
			Class cl = pm.NDOMapping.FindClass(typeof(ClassWithHint));
			IProvider provider = NDOProviderFactory.Instance[((Connection)pm.NDOMapping.Connections.First()).Type];
			Assert.That(cl != null, "Class not found");
			Assert.That(typeof(Guid) ==  ((OidColumn)cl.Oid.OidColumns[0]).SystemType, "Wrong type");
			Type t = pm.GetType();
			FieldInfo fi = t.GetField("ds", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(fi != null, "Field 'ds' not found");
			DataSet ds = (DataSet) fi.GetValue(pm);
			Assert.That(ds != null, "DataSet is null");
			DataTable dt = ds.Tables[cl.TableName];
			Assert.That(dt != null, "DataTable is null");
            OidColumn oidColumn = (OidColumn)cl.Oid.OidColumns[0];
			DataColumn dc = dt.Columns[oidColumn.Name];
			Assert.That(dc != null, "DataColumn is null");
			Assert.That(provider != null, "Provider is null");
			if (provider.SupportsNativeGuidType)
				Assert.That(typeof(Guid) ==  dc.DataType, "Wrong column type");
			else
				Assert.That(typeof(string) ==  dc.DataType, "Wrong column type");
		}


		[Test]
		public void HintTest()
		{
			ClassWithHint testobj = new ClassWithHint();
			testobj.Text = "Test";
			pm.MakePersistent(testobj);
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<ClassWithHint>(pm);
			testobj = null;
			testobj = (ClassWithHint) q.ExecuteSingle(true);
			Assert.That(guid == (Guid) testobj.NDOObjectId.Id[0], "Wrong guid #1");
			Assert.That("Test" ==  testobj.Text, "Wrong text #1");

			testobj.Text = "Neuer Text";
			pm.Save();
			testobj = null;
			testobj = (ClassWithHint) q.ExecuteSingle(true);
			Assert.That(guid == (Guid) testobj.NDOObjectId.Id[0], "Wrong guid #2");
			Assert.That("Neuer Text" ==  testobj.Text, "Wrong text #2");
			
			pm.Delete(testobj);
			pm.Save();
		}

		[Test]
		public void DerivedGuidTest()
		{
			PersistenceManager pm = PmFactory.NewPersistenceManager();
			DerivedGuid dg = new DerivedGuid();
			dg.Guid = new Guid(5,5,5,5,5,5,5,5,5,5,5);
			dg.Myfield = "Test";
			pm.MakePersistent(dg);
			pm.Save();
			dg = null;
			pm.UnloadCache();
			IQuery q = new NDOQuery<DerivedGuid>(pm);
			dg = (DerivedGuid) q.ExecuteSingle(true);
			Assert.That(new Guid(5,5,5,5,5,5,5,5,5,5,5) == dg.Guid, "Guid wrong");
			Assert.That(new Guid(5,5,5,5,5,5,5,5,5,5,5) == (Guid) dg.NDOObjectId.Id[0], "Oid wrong");
			Assert.That("Test" ==  dg.Myfield, "Text wrong");
			pm.Delete(dg);
			pm.Save();
		}

		private void OnIdGeneration(Type t, ObjectId oid)
		{
			if (!t.FullName.StartsWith ("NDOObjectIdTestClasses."))
				return;  // Use the other handler
            Class cl = pm.NDOMapping.FindClass(t);
            Assert.That(cl != null);
            OidColumn oidColumn = (OidColumn)cl.Oid.OidColumns[0];
            if (t == typeof(NDOoidAndHandler) || t == typeof(ClassWithHint))
            {
                if (oidColumn.SystemType == typeof(string))
                    oid.Id[0] = this.guid.ToString();
                else
                    oid.Id[0] = this.guid;
            }
            else
            {
                oid.Id[0] = 1;
            }
		}
	}
}
