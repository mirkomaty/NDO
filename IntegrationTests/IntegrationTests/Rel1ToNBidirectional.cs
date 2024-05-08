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
using PureBusinessClasses;
using NDO;
using NUnit.Framework;
using NDO.Query;

namespace NdoUnitTests
{
	[TestFixture]
	public class Rel1ToNBidirectional : NDOTest
	{
		Peer p;
		PersistenceManager pm;

		[SetUp]
		public void Setup() 
		{
			pm = PmFactory.NewPersistenceManager();
			p = CreatePeer("Peer 1");
		}

		[TearDown]
		public void TearDown() 
		{
			pm.Abort();
			IList peerListe = pm.GetClassExtent(typeof(Peer), false);
			pm.Delete(peerListe);
			IList trListe = pm.GetClassExtent(typeof(Track), false);
			pm.Delete(trListe);
			pm.Save();
			pm.Close();
			pm = null;
		}

		[Test]
		public void TestDeleteHierarchy()
		{			
			Track t = this.p.NewTrack();
			t.Name = "Track 1";
			t = this.p.NewTrack();
			t.Name = "Track 2";
			pm.MakePersistent(p);

			pm.Save();

			pm.UnloadCache();

			IQuery q = new NDOQuery<Peer>(pm, null);
			IList l = q.Execute();
			pm.Delete(l);
			pm.Save();
			pm.UnloadCache();
			Assert.That(1 ==  l.Count, "1 Peer muss da sein");

			Peer p2 = (Peer) l[0];

			Assert.That(NDOObjectState.Transient ==  p2.NDOObjectState, "Peer muss transient sein");

			foreach(Track t2 in p2.Tracks)
			{
				Assert.That(NDOObjectState.Transient ==  t2.NDOObjectState, "Track muss transient sein");
			}
			Assert.That(NDOObjectState.Transient ==  p2.NDOObjectState, "Peer muss transient sein");

			pm.MakePersistent(p2);
		}
		
		Peer CreatePeer(string name)
		{
			Peer p = new Peer();
			p.Name = name;
			return p;
		}
	}
}
