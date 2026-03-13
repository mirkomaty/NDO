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
using System.Reflection;
using System.Collections;
using NUnit.Framework;
using NDO;
using Reisekosten;
using Reisekosten.Personal;
using NDO.Query;
using NDO.Mapping;

namespace NdoUnitTests
{
	[TestFixture]
	public class ArrayListTests : NDOTest
	{
		private PersistenceManager pm;
		private Mitarbeiter m;
		private Reise r;
		private Reise r2;

		[SetUp]
		public void Setup() 
		{
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter("Hartmut", "Kocher");
			pm.MakePersistent(m);
			r = CreateReise("ADC");
			r2 = CreateReise("ADW");
		}

		[TearDown]
		public void TearDown() 
		{
			pm.Abort();
			IList mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
			pm.Delete( mitarbeiterListe );
			pm.Save();
			pm.Dispose();
		}


		private void AddRange()
		{
			ArrayList l = new ArrayList();
			l.Add(r);
			l.Add(r2);
			m.ReisenAddRange(l);
			Assert.That(GetLoadState(), "Relation not loaded");
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "Wrong state #1");
			pm.MakeAllHollow();
			pm.UnloadCache();
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState, "Wrong state #2");
			Assert.That(!GetLoadState(), "Relation should not be loaded");
		}


		[Test]
		public void TestAddRange()
		{
			AddRange();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm);
			m = q.ExecuteSingle(true);
			Assert.That(2 ==  m.Reisen.Count, "Count of Reisen wrong");
		}

		[Test]
		public void TestBinarySearch()
		{
			AddRange();
			int index = m.ReisenBinarySearch(r);
			// Don't care about the result, the object won't be fount
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenCapacity()
		{
			AddRange();
			int result = m.ReisenCapacity;
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenGetRange()
		{
			AddRange();
			IList l = m.ReisenGetRange(0, 2);
			Assert.That(GetLoadState(), "Relation not loaded");
			Assert.That(2 ==  l.Count, "Count is wrong");
		}

		[Test]
		public void ReisenInsertRange()
		{
			AddRange();
			ArrayList range = new ArrayList();
			range.Add(CreateReise("foo"));
			range.Add(CreateReise("bar"));
			m.ReisenInsertRange(1, range);
			Assert.That(GetLoadState(), "Relation not loaded");
			Assert.That(m.Reisen[1] ==  (Reise)range[0], "Wrong order");
			SaveAndReload();
			Assert.That(4 ==  m.Reisen.Count, "Count of Reisen wrong");
		}

		[Test]
		public void ReisenLastIndexOf()
		{
			AddRange();
			int i = m.ReisenLastIndexOf(r, 1, 2);
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenRemoveRange()
		{
			AddRange();
			m.ReisenRemoveRange(0, 1);
			Assert.That(GetLoadState(), "Relation not loaded");
			Assert.That(1 ==  m.Reisen.Count, "Count of Reisen wrong #1");
			SaveAndReload();
			Assert.That(1 ==  m.Reisen.Count, "Count of Reisen wrong #2");
		}

		[Test]
		public void ReisenReverse()
		{
			AddRange();
			m.ReisenReverse();
			Assert.That(GetLoadState(), "Relation not loaded");
		}

        //[Test]
        //public void ReisenSetRange()
        //{
        //    AddRange();
        //    ArrayList l = new ArrayList();
        //    l.Add(CreateReise("foo"));
        //    l.Add(CreateReise("bar"));
        //    m.ReisenSetRange(0, l);
	//Assert.That(GetLoadState(), "Relation not loaded");
	//Assert.That("foo" ==  (m.Reisen[0] as Reise).Zweck, "Falsche Reise #1");
	//Assert.That("bar" ==  (m.Reisen[1] as Reise).Zweck, "Falsche Reise #2");
	//    //    SaveAndReload();
	//Assert.That(2 ==  m.Reisen.Count, "Count of Reisen wrong");
	//    //    if (((Reise)m.Reisen[0]).NDOObjectId.Id[0] is Int32)
	//    //    {
	//    //        m.ReisenSort();
	//    Assert.That("bar" ==  (m.Reisen[0] as Reise).Zweck, "Falsche Reise #3");
	//    Assert.That("foo" ==  (m.Reisen[1] as Reise).Zweck, "Falsche Reise #4");
        //    }
        //}

		[Test]
		public void ReisenSort()
		{
			AddRange();
			m.ReisenSort();
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenToArray()
		{
			AddRange();
			object o = m.ReisenToArray(typeof(Reise));
			Assert.That(GetLoadState(), "Relation not loaded");
		}

        //[Test]
        //public void ReisenTrimToSize()
        //{
        //    AddRange();
        //    m.ReisenTrimToSize();
		//Assert.That(GetLoadState(), "Relation not loaded");
        //}
		
		[Test]
		public void ReisenCount()
		{
			AddRange();
			int x = m.ReisenCount; 
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenContains()
		{
			AddRange();
			bool test = m.ReisenContains(r);
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenCopyTo()
		{
			AddRange();
			Reise[] arr = m.ReisenCopyTo;
			Assert.That(GetLoadState(), "Relation not loaded");
			Assert.That((arr[0].Zweck == r.Zweck || arr[0].Zweck == r2.Zweck) && (arr[1].Zweck == r.Zweck || arr[1].Zweck == r2.Zweck), "Reisen not found");
		}

		[Test]
		public void ReisenEquals()
		{
			Object o = r;
			AddRange();
			bool dummy = m.ReisenEquals(o);
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenEnumerator()
		{
			AddRange();
			IEnumerator ie = m.ReisenEnumerator; 
			Assert.That(GetLoadState(), "Relation not loaded");
			ie.MoveNext();
			Assert.That(((Reise)ie.Current).Zweck != r.Zweck ||  ((Reise)ie.Current).Zweck != r2.Zweck, "Reise wrong #1");
			ie.MoveNext();
			Assert.That(((Reise)ie.Current).Zweck != r.Zweck ||  ((Reise)ie.Current).Zweck != r2.Zweck, "Reise wrong #2");
		}

		[Test]
		public void ReisenHashCode()
		{
			AddRange();
			int result = m.ReisenHashCode();
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenGetType()
		{
			AddRange();
			Type t = m.ReisenGetType();
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		[Test]
		public void ReisenIndexOf()
		{
			AddRange();
			int result = m.ReisenIndexOf(r);
			Assert.That(GetLoadState(), "Relation not loaded");
		}

        //[Test]
        //public void ReisenIsFixedSize()
        //{
        //    AddRange();
        //    bool result = m.ReisenIsFixedSize; 
//    Assert.That(GetLoadState(), "Relation not loaded");
        //}

        //[Test]
        //public void ReisenIsReadOnly()
        //{
        //    AddRange();
        //    bool result = m.ReisenIsReadOnly; 
 //   Assert.That(GetLoadState(), "Relation not loaded");
        //}


        //[Test]
        //public void ReisenIsSynchronized()
        //{
        //    AddRange();
        //    bool b = m.ReisenIsSynchronized; 
//    Assert.That(GetLoadState(), "Relation not loaded");
        //}


        //[Test]
        //public void ReisenSyncRoot()
        //{
        //    AddRange();
        //    object o = m.ReisenSyncRoot; 
//    Assert.That(GetLoadState(), "Relation not loaded");
        //}

		[Test]
		public void ReisenRemoveAt() 
		{
			AddRange();
			m.ReisenRemoveAt(1);
			string zweck = ((Reise)m.Reisen[0]).Zweck;
			Assert.That(GetLoadState(), "Relation not loaded");
			SaveAndReload();
			Assert.That(1 ==  m.Reisen.Count, "Wrong count");
			Assert.That(zweck ==  ((Reise)m.Reisen[0]).Zweck, "Reise wrong");
		}

		[Test]
		public void ReisenToString()
		{
			AddRange();
			string s = m.ReisenToString; 
			Assert.That(GetLoadState(), "Relation not loaded");
		}

		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) 
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		private Reise CreateReise(string zweck) 
		{
			Reise r = new Reise();
			r.Zweck = zweck;
			return r;
		}

		private bool GetLoadState()
		{
			NDO.Mapping.Relation r = pm.NDOMapping.FindClass(typeof(Mitarbeiter)).FindRelation("dieReisen");
			var lss = (ILoadStateSupport) r;
			return ((IPersistenceCapable)m).NDOGetLoadState(lss.Ordinal);
		}

		private void SaveAndReload()
		{
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm);
			m = (Mitarbeiter) q.ExecuteSingle(true);
		}


	}
}
