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
using DataTypeTestClasses;
using NUnit.Framework;
using NDO;
using NDO.Query;

namespace NdoUnitTests
{
	[TestFixture]
	public class TestEmbedded
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
			if (null != pm)
			{
				IList l = pm.GetClassExtent(typeof(ParentWithEmbedded), true);
				pm.Delete(l);
				pm.Save();
				pm.Close();
				pm = null;
			}
		}

		[Test]
		public void Test()
		{
			ParentWithEmbedded pwe = new ParentWithEmbedded();
			pwe.Ewom = new EmbeddedWithOneMember("MyEwom");
			pwe.Test = "Test";
			pm.MakePersistent(pwe);
			pm.Save();

			Assert.That(NDOObjectState.Persistent ==  pwe.NDOObjectState, "Status falsch #1");
			pwe.Ewom = new EmbeddedWithOneMember("SecondEwom");
			Assert.That(NDOObjectState.PersistentDirty ==  pwe.NDOObjectState, "Status falsch #2");
			pm.Save();
			pm.UnloadCache();

			IQuery q = new NDOQuery<ParentWithEmbedded>(pm, null);
			pwe = (ParentWithEmbedded) q.ExecuteSingle(true);

			Assert.That("SecondEwom" ==  pwe.Ewom.Member, "Inhalt falsch ");
			
		}
	}
}
