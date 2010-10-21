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
using NUnit.Framework;
using NDO;
using DeleteForeignRelation;

namespace NdoUnitTests
{
	[TestFixture]
	public class DeleteForeignRelationTests
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
			IList l = pm.GetClassExtent(typeof(DfrAddress));
			pm.Delete(l);
			l = pm.GetClassExtent(typeof(DfrContact));
			pm.Delete(l);
			l = pm.GetClassExtent(typeof(DfrAddressDescriptor));
			pm.Delete(l);
		}


		[Test]
		public void TestDfr()
		{

			DfrContact contact = new DfrContact();
			contact.Name = "Hans M�ller";
			pm.MakePersistent(contact as IPersistenceCapable);

			DfrAddress address = new DfrAddress();
			pm.MakePersistent(address as IPersistenceCapable);
			address.Ort = "Wahnweil";         
			address.OriginContact = contact;
      
			DfrAddressDescriptor descriptor = address.NewAddressDescriptor(contact);
			descriptor.IsAdopted = true;

			Assert.AreEqual(1, contact.Addresses.Count, "Count falsch");
			Assert.That((contact.Addresses[0] as DfrAddressDescriptor) == descriptor, "Descriptor falsch");
			pm.Save(); 

			pm.MakeHollow(contact as IPersistenceCapable);

			address.RemoveAddressDescriptor(descriptor);
			Assert.AreEqual(NDOObjectState.Deleted, descriptor.NDOObjectState, "Status falsch");
		}
	}
}
