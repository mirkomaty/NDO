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
using NUnit.Framework;
using NDO;
using DeleteForeignRelation;

namespace NdoUnitTests
{
	[TestFixture]
	public class DeleteForeignRelationTests : NDOTest
	{
		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();
			IList l = pm.GetClassExtent(typeof(DfrAddress));
			pm.Delete(l);
			l = pm.GetClassExtent(typeof(DfrContact));
			pm.Delete(l);
			l = pm.GetClassExtent(typeof(DfrAddressDescriptor));
			pm.Delete(l);
			pm.Dispose();
		}


		[Test]
		public void TestDfr()
		{
			var pm = PmFactory.NewPersistenceManager();

			DfrContact contact = new DfrContact();
			contact.Name = "Hans Möller";
			pm.MakePersistent(contact as IPersistenceCapable);

			DfrAddress address = new DfrAddress();
			pm.MakePersistent(address as IPersistenceCapable);
			address.Ort = "Wahnweil";         
			address.OriginContact = contact;
      
			DfrAddressDescriptor descriptor = address.NewAddressDescriptor(contact);
			descriptor.IsAdopted = true;

			Assert.That(1 ==  contact.Addresses.Count, "Count falsch");
			Assert.That((contact.Addresses[0] as DfrAddressDescriptor) == descriptor, "Descriptor falsch");
			pm.Save(); 

			pm.MakeHollow(contact as IPersistenceCapable);

			address.RemoveAddressDescriptor(descriptor);
			Assert.That(NDOObjectState.Deleted ==  descriptor.NDOObjectState, "Status falsch");
		}
	}
}
