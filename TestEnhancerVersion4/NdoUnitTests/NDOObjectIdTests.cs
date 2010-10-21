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
using System.Data;
using System.Reflection;
using System.Collections;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using NDOObjectIdTestClasses;
using NDOInterfaces;

namespace NdoUnitTests
{
	[TestFixture]
	public class NDOObjectIdTests
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
			Query q = pm.NewQuery(typeof(NDOoidAndHandler));
			testobj = null;
			testobj = (NDOoidAndHandler) q.ExecuteSingle(true);
			Assert.AreEqual(guid, testobj.MyId, "Wrong guid");
			Assert.AreEqual("Test", testobj.Text, "Wrong text");

			testobj.Text = "Neuer Text";
			pm.Save();
			testobj = null;
			testobj = (NDOoidAndHandler) q.ExecuteSingle(true);
			Assert.AreEqual(guid, testobj.MyId, "Wrong guid");
			Assert.AreEqual("Neuer Text", testobj.Text, "Wrong text");
			
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
			Query q = pm.NewQuery(typeof(ObjectOwner));
			owner = (ObjectOwner) q.ExecuteSingle(true);
			Assert.NotNull(owner.Element, "No element");
			Assert.AreEqual(guid, owner.Element.MyId, "Wrong guid");
			Assert.AreEqual("Text", owner.Element.Text, "Wrong text");
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
			Query q = pm.NewQuery(typeof(HintOwner));
			owner = (HintOwner) q.ExecuteSingle(true);
			Assert.NotNull(owner.Element, "No element");
			Assert.AreEqual(guid, owner.Element.NDOObjectId.Id[0], "Wrong guid");
			Assert.AreEqual("Text", owner.Element.Text, "Wrong text");
		}


		[Test]
		public void HintEnhancerTest()
		{
			Class cl = pm.NDOMapping.FindClass(typeof(ClassWithHint));
			IProvider provider = NDOProviderFactory.Instance[((Connection)pm.NDOMapping.Connections[0]).Type];
			Assert.NotNull(cl, "Class not found");
			Assert.AreEqual(typeof(Guid), ((OidColumn)cl.Oid.OidColumns[0]).SystemType, "Wrong type");
			Type t = pm.GetType();
			FieldInfo fi = t.GetField("ds", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.NotNull(fi, "Field 'ds' not found");
			DataSet ds = (DataSet) fi.GetValue(pm);
			Assert.NotNull(ds, "DataSet is null");
			DataTable dt = ds.Tables[cl.TableName];
			Assert.NotNull(dt, "DataTable is null");
            OidColumn oidColumn = (OidColumn)cl.Oid.OidColumns[0];
			DataColumn dc = dt.Columns[oidColumn.Name];
			Assert.NotNull(dc, "DataColumn is null");
			Assert.NotNull(provider, "Provider is null");
			if (provider.SupportsNativeGuidType)
				Assert.AreEqual(typeof(Guid), dc.DataType, "Wrong column type");
			else
				Assert.AreEqual(typeof(string), dc.DataType, "Wrong column type");
		}


		[Test]
		public void HintTest()
		{
			ClassWithHint testobj = new ClassWithHint();
			testobj.Text = "Test";
			pm.MakePersistent(testobj);
			pm.Save();
			pm.UnloadCache();
			Query q = pm.NewQuery(typeof(ClassWithHint));
			testobj = null;
			testobj = (ClassWithHint) q.ExecuteSingle(true);
			Assert.AreEqual(guid, testobj.NDOObjectId.Id[0], "Wrong guid #1");
			Assert.AreEqual("Test", testobj.Text, "Wrong text #1");

			testobj.Text = "Neuer Text";
			pm.Save();
			testobj = null;
			testobj = (ClassWithHint) q.ExecuteSingle(true);
			Assert.AreEqual(guid, testobj.NDOObjectId.Id[0], "Wrong guid #2");
			Assert.AreEqual("Neuer Text", testobj.Text, "Wrong text #2");
			
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
			Query q = pm.NewQuery(typeof(DerivedGuid));
			dg = (DerivedGuid) q.ExecuteSingle(true);
			Assert.AreEqual(new Guid(5,5,5,5,5,5,5,5,5,5,5), dg.Guid, "Guid wrong");
			Assert.AreEqual(new Guid(5,5,5,5,5,5,5,5,5,5,5), dg.NDOObjectId.Id[0], "Oid wrong");
			Assert.AreEqual("Test", dg.Myfield, "Text wrong");
			pm.Delete(dg);
			pm.Save();
		}

		private void OnIdGeneration(Type t, ObjectId oid)
		{
			if (!t.FullName.StartsWith ("NDOObjectIdTestClasses."))
				return;  // Use the other handler
            Class cl = pm.NDOMapping.FindClass(t);
            Assert.NotNull(cl);
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
