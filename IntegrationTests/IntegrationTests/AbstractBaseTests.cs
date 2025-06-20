﻿//
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
using AbstractBaseTestClasses;
using NDO;
using NUnit.Framework;

namespace NdoUnitTests
{

	[TestFixture]
	public class AbstractBaseTests : NDOTest
	{
		PersistenceManager pm;

		[SetUp]
		public void Setup()
		{
			this.pm = PmFactory.NewPersistenceManager();
		}


		[TearDown]
		public void TearDown()
		{			
			IList l = pm.GetClassExtent(typeof(ABA));
			pm.Delete(l);
			l = pm.GetClassExtent(typeof(ABB));
			pm.Delete(l);
			pm.Dispose();
		}


		[Test]
		public void TestConcreteRel()
		{
			ABA a = new ABA();
			a.MyString = "a";
			pm.MakePersistent(a);
			ABB b = new ABB();
			b.MyString = "b";
			pm.MakePersistent(b);
			pm.Save();
			b.AddA(a);
			pm.Save();
			pm.UnloadCache();
			b = (ABB) pm.FindObject(b.NDOObjectId);
			Assert.That(1 == b.MyA.Count, "Count wrong");
		}

		[Test]
		public void TestAbstractRel()
		{
			AbstractRelLeft left = new AbstractRelLeft();
			left.MyString = "Left";
			pm.MakePersistent(left);
			ABB b = new ABB();
			b.MyString = "b";
			pm.MakePersistent(b);
			pm.Save();
			left.AddABBase(b);
			pm.Save();
			pm.UnloadCache();
			left = (AbstractRelLeft) pm.FindObject(left.NDOObjectId);
			Assert.That(1 == left.TheBases.Count, "Count wrong");
		}


	}
}
