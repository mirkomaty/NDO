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
using ForeignObjectManipulationClasses;
using NDO;
using NDO.Query;
using System.Collections;
using NUnit.Framework;
using System.Drawing;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung för ForeignObjectTests.
	/// </summary>
	[TestFixture]
	public class ForeignObjectTests
	{

		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();
			NDOQuery<Parent> q = new NDOQuery<Parent>(pm, null);
			IList l = q.Execute();
			pm.Delete(l);
			pm.Dispose();
		}

		[Test]
		public void TestString()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();			
			p1.ManipulateString(p2, "test");
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestPoint()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			p1.ManipulatePoint(p2, new Point(0, 0));
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestPoint2()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			p1.ManipulatePoint(p2);
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnPoint2()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			p1.ManipulateOwnPoint(p2);
			Assert.That(NDOObjectState.PersistentDirty ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.Persistent ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnPoint()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			pm.MakePersistent(p1);
			pm.Save();

			p1.Position = new Point(0,0);
			Assert.That(NDOObjectState.PersistentDirty ==  p1.NDOObjectState, "Falscher Status 1");
		}

	
		[Test]
		public void TestListElement()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			ChildList cl = new ChildList();
			pm.MakePersistent(cl);
			p1.ManipulateListElement(p2, cl);
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestList()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			ChildList cl = new ChildList();
			pm.MakePersistent(cl);
			IList l = new ArrayList();
			l.Add(cl);
			p1.ManipulateList(p2, l);
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestEmbedded()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			ChildEmbedded ce = new ChildEmbedded();
			p1.ManipulateEmbedded(p2, ce);
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnEmbedded()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			pm.MakePersistent(p1);
			pm.Save();

			ChildEmbedded ce = new ChildEmbedded();
			p1.ChildEmbedded = ce;
			Assert.That(NDOObjectState.PersistentDirty ==  p1.NDOObjectState, "Falscher Status 1");
		}


		[Test]
		public void TestElement()
		{
			var pm = PmFactory.NewPersistenceManager();
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			ChildElement ce = new ChildElement();
			pm.MakePersistent(ce);
			p1.ManipulateElement(p2, ce);
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

/*
		[Test]
		public void TestValueTypeMember()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			p1.ManipulateValueTypeMember(p2);
			Assert.That(NDOObjectState.Persistent ==  p1.NDOObjectState, "Falscher Status 1");
			Assert.That(NDOObjectState.PersistentDirty ==  p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnValueTypeMember()
		{
			Parent p1 = new Parent();
			pm.MakePersistent(p1);
			pm.Save();

			p1.ManipulateOwnValueTypeMember(p1);
			Assert.That(NDOObjectState.PersistentDirty ==  p1.NDOObjectState, "Falscher Status");
		}
*/

	}
}
