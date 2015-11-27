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
using System.Text;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using PureBusinessClasses;

namespace NdoUnitTests
{
    [TestFixture]
    public class KeyTests
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
#if maskedOut
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
#endif
        }



		[Ignore("This tests must be reworked")]
        [Test]
        public void IntKeyEquality()
        {
#if maskedOut
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
#endif
        }
    }
}
