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
using ForeignObjectManipulationClasses;
using NDO;
using System.Collections;
using NUnit.Framework;
using System.Drawing;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung f�r ForeignObjectTests.
	/// </summary>
	[TestFixture]
	public class ForeignObjectTests
	{
		PersistenceManager pm; 

		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
		}

		[TearDown]
		public void TearDown()
		{
			Query q = pm.NewQuery(typeof(Parent), null);
			IList l = q.Execute();
			pm.Delete(l);
			pm = null;
		}

		[Test]
		public void TestString()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();			
			p1.ManipulateString(p2, "test");
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestPoint()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			p1.ManipulatePoint(p2, new Point(0, 0));
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestPoint2()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			p1.ManipulatePoint(p2);
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnPoint2()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			p1.ManipulateOwnPoint(p2);
			Assert.AreEqual(NDOObjectState.PersistentDirty, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.Persistent, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnPoint()
		{
			Parent p1 = new Parent();
			pm.MakePersistent(p1);
			pm.Save();

			p1.Position = new Point(0,0);
			Assert.AreEqual(NDOObjectState.PersistentDirty, p1.NDOObjectState, "Falscher Status 1");
		}

	
		[Test]
		public void TestListElement()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			ChildList cl = new ChildList();
			pm.MakePersistent(cl);
			p1.ManipulateListElement(p2, cl);
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestList()
		{
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
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestEmbedded()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			ChildEmbedded ce = new ChildEmbedded();
			p1.ManipulateEmbedded(p2, ce);
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnEmbedded()
		{
			Parent p1 = new Parent();
			pm.MakePersistent(p1);
			pm.Save();

			ChildEmbedded ce = new ChildEmbedded();
			p1.ChildEmbedded = ce;
			Assert.AreEqual(NDOObjectState.PersistentDirty, p1.NDOObjectState, "Falscher Status 1");
		}


		[Test]
		public void TestElement()
		{
			Parent p1 = new Parent();
			Parent p2 = new Parent();
			pm.MakePersistent(p1);
			pm.MakePersistent(p2);
			pm.Save();

			ChildElement ce = new ChildElement();
			pm.MakePersistent(ce);
			p1.ManipulateElement(p2, ce);
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
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
			Assert.AreEqual(NDOObjectState.Persistent, p1.NDOObjectState, "Falscher Status 1");
			Assert.AreEqual(NDOObjectState.PersistentDirty, p2.NDOObjectState, "Falscher Status 2");
		}

		[Test]
		public void TestOwnValueTypeMember()
		{
			Parent p1 = new Parent();
			pm.MakePersistent(p1);
			pm.Save();

			p1.ManipulateOwnValueTypeMember(p1);
			Assert.AreEqual(NDOObjectState.PersistentDirty, p1.NDOObjectState, "Falscher Status");
		}
*/

	}
}
