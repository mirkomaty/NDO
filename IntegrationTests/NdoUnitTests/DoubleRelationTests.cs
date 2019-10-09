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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NDO;
using DoubleRelationClasses;

namespace NdoUnitTests
{
    [TestFixture]
    public class DoubleRelationTests
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
			pm.Close();
			Assert.That( PmFactory.PoolCount <= 1, "Pool Count is " + PmFactory.PoolCount );
		}

		[Test]
        public void TestDoubleRelation()
        {
            DRPerson oPerson1 = new DRPerson();
            pm.MakePersistent(oPerson1);
            oPerson1.DefaultAddress = oPerson1.NewAddress();
            oPerson1.Name = "Hello World 1";

            DRPerson oPerson2 = new DRPerson();
            pm.MakePersistent(oPerson2);
            oPerson2.DefaultAddress = oPerson2.NewAddress();
            oPerson2.Name = "Hello World 2";

            pm.Delete(oPerson2);
            pm.Save();
        }
    }
}
