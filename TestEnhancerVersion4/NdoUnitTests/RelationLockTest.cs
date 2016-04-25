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
using System.IO;
using NUnit.Framework;
using Reisekosten.Personal;
using Reisekosten;
using NDO;
using NDO.Mapping;
using System.Reflection;

namespace NdoUnitTests
{

	[TestFixture]
	public class RelationLockTest
	{
		PersistenceManager pm;
		private Mitarbeiter m;
		private Sozialversicherungsnummer svn;
		Assembly ass;
		Relation r;
		Relation fr;
		Type t;
		object theLock;

		[SetUp]
		public void Setup() 
		{
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Boris", "Becker");
			svn = new Sozialversicherungsnummer();
			svn.SVN = 4711;
			m.SVN = svn;
			pm.MakePersistent(m);
			ass = pm.GetType().Assembly;
			r = pm.NDOMapping.FindClass(typeof(Mitarbeiter)).FindRelation("sn");
			fr = r.ForeignRelation;
			t = ass.GetType("NDO.ObjectLock", true);
			if (t != null)
				theLock = Activator.CreateInstance(t);

			//			pm.Save();

		}

		[TearDown]
		public void TearDown() 
		{
		}

		[Test]
		public void TestLockingBi()
		{
			if (t != null)
			{
				bool b = GetLock(m, r, svn);
				Assert.AreEqual(true, b, "There shouldn't be a lock");
				Assert.AreEqual(true, IsLocked(m, r, svn), "Objects should be locked #1");
				Assert.AreEqual(true, IsLocked(svn, fr, m), "Objects should be locked #2");
				Assert.AreEqual(false, GetLock(svn, fr, m), "Objects should be locked #3");
				Unlock(m, r, svn);
				Assert.AreEqual(false, IsLocked(m, r, svn), "Objects shouldn't be locked #1");
				Assert.AreEqual(false, IsLocked(svn, fr, m), "Objects shouldn't be locked #2");
			}
		}

		[Test]
		public void TestRelationHash()
		{
			Class cl1 = pm.NDOMapping.FindClass(typeof(Reise));
			Class cl2 = pm.NDOMapping.FindClass(typeof(PKWFahrt));
			Relation r1 = cl1.FindRelation("belege");
			Relation r2 = cl2.FindRelation("reise");
			Assert.AreEqual(r1.GetHashCode(), r2.GetHashCode(), "Hash should be equal");
			Assert.That(r1.Equals(r2), "Relation should be the same");
		}


		[Test]
		public void TestLockingPolyBi()
		{
			if (t != null)
			{
				Reise reise = new Reise();
				reise.Zweck = "ADC";
				Kostenpunkt kp = new PKWFahrt(100);
				reise.AddKostenpunkt(kp);
				pm.MakePersistent(reise);
				Relation rel = pm.NDOMapping.FindClass(typeof(Reise)).FindRelation("belege");
				Relation rel2 = pm.NDOMapping.FindClass(typeof(Beleg)).FindRelation("reise");
				Assert.AreEqual(rel, rel2, "Relation should be considered the same");
				bool b = GetLock(reise, rel, kp);
				Assert.AreEqual(true, b, "There shouldn't be a lock");
				Assert.AreEqual(true, IsLocked(reise, rel, kp), "Objects should be locked #1");
				Assert.AreEqual(true, IsLocked(kp, rel, reise), "Objects should be locked #2");
				Assert.AreEqual(false, GetLock(kp, rel.ForeignRelation, reise), "Objects should be locked #3");
				Assert.AreEqual(false, GetLock(reise, rel.ForeignRelation, kp), "Objects should be locked #4");
				Assert.AreEqual(false, GetLock(kp, rel2, reise), "Objects should be locked #5");
				Assert.AreEqual(false, GetLock(reise, rel2, kp), "Objects should be locked #6");
				Assert.AreEqual(false, GetLock(kp, rel2.ForeignRelation, reise), "Objects should be locked #7");
				Assert.AreEqual(false, GetLock(reise, rel2.ForeignRelation, kp), "Objects should be locked #8");
				Unlock(reise, rel, kp);
				Assert.AreEqual(false, IsLocked(reise, rel, kp), "Objects should be locked #1");
				Assert.AreEqual(false, IsLocked(kp, rel, reise), "Objects should be locked #2");
				Assert.AreEqual(false, IsLocked(kp, rel.ForeignRelation, reise), "Objects should be locked #3");
				Assert.AreEqual(false, IsLocked(reise, rel.ForeignRelation, kp), "Objects should be locked #4");
				Assert.AreEqual(false, IsLocked(kp, rel2, reise), "Objects should be locked #5");
				Assert.AreEqual(false, IsLocked(reise, rel2, kp), "Objects should be locked #6");
				Assert.AreEqual(false, IsLocked(kp, rel2.ForeignRelation, reise), "Objects should be locked #7");
				Assert.AreEqual(false, IsLocked(reise, rel2.ForeignRelation, kp), "Objects should be locked #8");
			}
		}


		private bool GetLock(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
			MethodInfo mi = t.GetMethod("GetLock", new Type[]{typeof(IPersistenceCapable), typeof(Relation), typeof(IPersistenceCapable)});
			Assert.NotNull(mi, "Method not found");
			return (bool) mi.Invoke(theLock, new object[]{pc, r, child});
		}

		private bool IsLocked(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
			MethodInfo mi = t.GetMethod("IsLocked", new Type[]{typeof(IPersistenceCapable), typeof(Relation), typeof(IPersistenceCapable)});
			Assert.NotNull(mi, "Method not found");
			return (bool) mi.Invoke(theLock, new object[]{pc, r, child});
		}

		private void Unlock(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
			MethodInfo mi = t.GetMethod("Unlock", new Type[]{typeof(IPersistenceCapable), typeof(Relation), typeof(IPersistenceCapable)});
			Assert.NotNull(mi, "Method not found");
			mi.Invoke(theLock, new object[]{pc, r, child});
		}



		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) 
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

	}
}
