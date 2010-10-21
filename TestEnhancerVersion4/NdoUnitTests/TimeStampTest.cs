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
using TimeStampTestClasses;
using NDO;
using NUnit.Framework;

namespace NdoUnitTests
{
	[TestFixture]
	public class TimeStampTest
	{
		PersistenceManager pm1;
		PersistenceManager pm2;
		TimeStampContainer tsc;
		bool collisionHappened = false;

		[SetUp]
		public void Setup() 
		{
			tsc = new TimeStampContainer("Hans");
			pm1 = PmFactory.NewPersistenceManager();
			pm2 = PmFactory.NewPersistenceManager();
			pm1.MakePersistent(tsc);
			pm1.Save();
			pm1.UnloadCache();
		}

		[TearDown]
		public void TearDown() 
		{
			if (pm2 == null)
				pm2 = PmFactory.NewPersistenceManager();
			pm2.UnloadCache();
			IList l = pm2.GetClassExtent(typeof(TimeStampContainer), false);
			if (l.Count > 0)
			{
				pm2.Delete(l);
				pm2.Save();
			}
			pm2.Close();
			pm2 = null;
			if (pm1 != null)
			{
				pm1.Close();
				pm1 = null;
			}
		}

		[Test]
		public void TestTimeStampsNoException()
		{
			IList l1 = pm1.GetClassExtent(typeof(TimeStampContainer));
			Assert.AreEqual(1, l1.Count, "Count sollte 1 sein");
			IList l2 = pm2.GetClassExtent(typeof(TimeStampContainer));
			Assert.AreEqual(1, l2.Count, "Count sollte 1 sein");
			TimeStampContainer tsc1 = (TimeStampContainer) l1[0];
			TimeStampContainer tsc2 = (TimeStampContainer) l2[0];
			pm1.CollisionEvent += new CollisionHandler(OnCollisionEvent);
			tsc1.Name = "Friedrich";
			tsc2.Name = "Peter";
			pm2.Save();
			pm1.Save();
			Assert.AreEqual(true, collisionHappened, "Kollision ist nicht erkannt worden");
		}


//		[Test]
//		public void GetMappingFile()
//		{
//			PersistenceManager pm = PmFactory.NewPersistenceManager();
//			System.Diagnostics.Debug.WriteLine(pm.NDOMapping.FileName);
//		}

		[Test]
		public void TestDeleteHollow()
		{
			Query q = pm1.NewQuery(typeof(TimeStampContainer), null, true);
			tsc = (TimeStampContainer) q.ExecuteSingle(true);
			Assert.AreEqual(NDOObjectState.Hollow, tsc.NDOObjectState, "Object should be hollow");
			Assert.That(((IPersistenceCapable)tsc).NDOTimeStamp != Guid.Empty, "Time Stamp should be there");
			pm1.Delete(tsc);
			pm1.Save();
			pm1.UnloadCache();
			IList l = pm1.GetClassExtent(typeof(TimeStampContainer));
			Assert.AreEqual(0, l.Count, "No object should be there");
		}

		private void OnCollisionEvent(object pc)
		{
			this.collisionHappened = true;
		}
	}
}
