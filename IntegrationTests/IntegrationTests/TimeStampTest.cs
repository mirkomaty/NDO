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
using TimeStampTestClasses;
using NDO;
using NUnit.Framework;
using NDO.Query;
using System.Linq;

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
			pm2.Delete( pm2.Objects<RowVersionClass>().ResultTable );
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
			Assert.That(1 ==  l1.Count, "Count sollte 1 sein");
			IList l2 = pm2.GetClassExtent(typeof(TimeStampContainer));
			Assert.That(1 ==  l2.Count, "Count sollte 1 sein");
			TimeStampContainer tsc1 = (TimeStampContainer) l1[0];
			TimeStampContainer tsc2 = (TimeStampContainer) l2[0];
			pm1.CollisionEvent += new CollisionHandler(OnCollisionEvent);
			tsc1.Name = "Friedrich";
			tsc2.Name = "Peter";
			pm2.Save();
			pm1.Save();
			Assert.That(true ==  collisionHappened, "Kollision ist nicht erkannt worden");
		}

		[Test]
		public void TestDeleteHollow()
		{
			IQuery q = new NDOQuery<TimeStampContainer>(pm1, null, true);
			tsc = (TimeStampContainer) q.ExecuteSingle(true);
			Assert.That(NDOObjectState.Hollow ==  tsc.NDOObjectState, "Object should be hollow");
			Assert.That(((IPersistenceCapable)tsc).NDOTimeStamp != Guid.Empty, "Time Stamp should be there");
			pm1.Delete(tsc);
			pm1.Save();
			pm1.UnloadCache();
			IList l = pm1.GetClassExtent(typeof(TimeStampContainer));
			Assert.That(0 ==  l.Count, "No object should be there");
		}

		[Test]
		public void CanWorkWithRowversions()
		{
			RowVersionClass rwc = new RowVersionClass();
			rwc.MyData = "Testdata";
			pm1.MakePersistent( rwc );
			pm1.Save();
			rwc = pm1.Objects<RowVersionClass>().Single();
			var oldVersion = rwc.RowVersion;
			Assert.That( oldVersion != (ulong) 0 );
			var changesSince = RowVersionClass.GetNewerThan( pm1, oldVersion );
			Assert.That( !changesSince.Any() );
			rwc.MyData = "Testdata";	// Makes object dirty
			pm1.Save();					// This creates a new version
			changesSince = RowVersionClass.GetNewerThan( pm1, oldVersion );
			Assert.That( changesSince.Any() );
		}

		private void OnCollisionEvent(object pc)
		{
			this.collisionHappened = true;
		}
	}
}
