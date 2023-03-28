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
using System.Text;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using PureBusinessClasses;

namespace NdoUnitTests
{
    [TestFixture]
    public class KeyTests : NDOTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

		[Ignore("This tests must be reworked")]
        [Test]
        public void GuidKeyEquality()
        {
            PersistenceManager pm = PmFactory.NewPersistenceManager();
            Class cl = pm.NDOMapping.FindClass(typeof(OrderDetail));
			IPersistenceCapable testObj1 = pm.FindObject( typeof( OrderDetail ), new object[] { new Guid( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ), new Guid( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ) } );
			IPersistenceCapable testObj2 = pm.FindObject( typeof( OrderDetail ), new object[] { new Guid( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ), new Guid( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ) } );
            ObjectId oid1 = testObj1.NDOObjectId;
            ObjectId oid2 = testObj2.NDOObjectId;

            Assert.That(oid1.GetHashCode() == oid2.GetHashCode(), "HashCodes should be equal");
            Assert.That(oid1 == oid2, "Keys should be equal #1");
            Assert.That(oid1.Equals(oid2), "Keys should be equal #2");
            Assert.That(oid2.Equals(oid1), "Keys should be equal #3");

            Hashtable ht = new Hashtable();
            ht.Add(oid1, null);
            Assert.That(ht.Contains(oid2));

            Key key1 = oid1.Id;
            Key key2 = oid2.Id;
            Assert.That(key1.GetHashCode() == key2.GetHashCode(), "HashCodes should be equal");
            Assert.That(key1 == key2, "Keys should be equal #4");
            Assert.That(key1.Equals(key2), "Keys should be equal #5");
            Assert.That(key2.Equals(key1), "Keys should be equal #6");

            ht.Clear();
            ht.Add(key1, null);
            Assert.That(ht.Contains(key2));

            oid1 = new ObjectId(key1);
            oid2 = new ObjectId(key2);

            Assert.That(oid1.GetHashCode() == oid2.GetHashCode(), "HashCodes should be equal");
            Assert.That(oid1 == oid2, "Keys should be equal #7");
            Assert.That(oid1.Equals(oid2), "Keys should be equal #8");
            Assert.That(oid2.Equals(oid1), "Keys should be equal #9");
        }



		[Ignore("This tests must be reworked")]
        [Test]
        public void IntKeyEquality()
        {
            PersistenceManager pm = PmFactory.NewPersistenceManager();
            Class cl = pm.NDOMapping.FindClass(typeof(OrderDetail));
			IPersistenceCapable testObj1 = pm.FindObject( typeof( OrderDetail ), new object[] { 4711,4712 });
			IPersistenceCapable testObj2 = pm.FindObject( typeof( OrderDetail ), new object[] { 4711,4712 });
            ObjectId oid1 = testObj1.NDOObjectId;
            ObjectId oid2 = testObj2.NDOObjectId;

            Assert.That(oid1.GetHashCode() == oid2.GetHashCode(), "HashCodes should be equal");
            Assert.That(oid1 == oid2, "Keys should be equal #1");
            Assert.That(oid1.Equals(oid2), "Keys should be equal #2");
            Assert.That(oid2.Equals(oid1), "Keys should be equal #3");

            Hashtable ht = new Hashtable();
            ht.Add(oid1, null);
            Assert.That(ht.Contains(oid2));

            Key key1 = oid1.Id;
            Key key2 = oid2.Id;
            Assert.That(key1.GetHashCode() == key2.GetHashCode(), "HashCodes should be equal");
            Assert.That(key1 == key2, "Keys should be equal #4");
            Assert.That(key1.Equals(key2), "Keys should be equal #5");
            Assert.That(key2.Equals(key1), "Keys should be equal #6");

            ht.Clear();
            ht.Add(key1, null);
            Assert.That(ht.Contains(key2));

            oid1 = new ObjectId(key1);
            oid2 = new ObjectId(key2);

            Assert.That(oid1.GetHashCode() == oid2.GetHashCode(), "HashCodes should be equal");
            Assert.That(oid1 == oid2, "Keys should be equal #7");
            Assert.That(oid1.Equals(oid2), "Keys should be equal #8");
            Assert.That(oid2.Equals(oid1), "Keys should be equal #9");
        }
    }
}
