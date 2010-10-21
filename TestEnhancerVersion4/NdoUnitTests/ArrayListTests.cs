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
using System.Reflection;
using System.Collections;
using NUnit.Framework;
using NDO;
using Reisekosten;
using Reisekosten.Personal;

namespace NdoUnitTests
{
	[TestFixture]
	public class ArrayListTests
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
			pm.Close();
			pm = null;
		}


		private void AddRange()
		{
			ArrayList l = new ArrayList();
			l.Add(r);
			l.Add(r2);
			m.ReisenAddRange(l);
			Assert.That(GetLoadState(), "Relation not loaded");
			pm.Save();
			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "Wrong state #1");
			pm.MakeAllHollow();
			pm.UnloadCache();
			Assert.AreEqual(NDOObjectState.Hollow, m.NDOObjectState, "Wrong state #2");
			Assert.That(!GetLoadState(), "Relation should not be loaded");
		}


		[Test]
		public void TestAddRange()
		{
			AddRange();
			Query q = pm.NewQuery(typeof(Mitarbeiter));
			m = (Mitarbeiter) q.ExecuteSingle(true);
			Assert.AreEqual(2, m.Reisen.Count, "Count of Reisen wrong");
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

        //[Test]
        //public void ReisenClone()
        //{
        //    AddRange();
        //    IList l = m.ReisenClone;
	//Assert.That(GetLoadState(), "Relation not loaded");
	//Assert.AreEqual(2, l.Count, "Count is wrong");
        //}

		[Test]
		public void ReisenGetRange()
		{
			AddRange();
			IList l = m.ReisenGetRange(0, 2);
			Assert.That(GetLoadState(), "Relation not loaded");
			Assert.AreEqual(2, l.Count, "Count is wrong");
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
			Assert.AreEqual(m.Reisen[1], (Reise)range[0], "Wrong order");
			SaveAndReload();
			Assert.AreEqual(4, m.Reisen.Count, "Count of Reisen wrong");
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
			Assert.AreEqual(1, m.Reisen.Count, "Count of Reisen wrong #1");
			SaveAndReload();
			Assert.AreEqual(1, m.Reisen.Count, "Count of Reisen wrong #2");
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
	//Assert.AreEqual("foo", (m.Reisen[0] as Reise).Zweck, "Falsche Reise #1");
	//Assert.AreEqual("bar", (m.Reisen[1] as Reise).Zweck, "Falsche Reise #2");
	//    //    SaveAndReload();
	//Assert.AreEqual(2, m.Reisen.Count, "Count of Reisen wrong");
	//    //    if (((Reise)m.Reisen[0]).NDOObjectId.Id[0] is Int32)
	//    //    {
	//    //        m.ReisenSort();
	//    Assert.AreEqual("bar", (m.Reisen[0] as Reise).Zweck, "Falsche Reise #3");
	//    Assert.AreEqual("foo", (m.Reisen[1] as Reise).Zweck, "Falsche Reise #4");
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
			Assert.AreEqual(1, m.Reisen.Count, "Wrong count");
			Assert.AreEqual(zweck, ((Reise)m.Reisen[0]).Zweck, "Reise wrong");
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
			FieldInfo fi = typeof(NDO.Mapping.Relation).GetField("Ordinal", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.NotNull(fi);
			object o = fi.GetValue(r);
			Assert.NotNull(o);
			return m.NDOGetLoadState((int)o);
		}

		private void SaveAndReload()
		{
			pm.Save();
			pm.UnloadCache();
			Query q = pm.NewQuery(typeof(Mitarbeiter));
			m = (Mitarbeiter) q.ExecuteSingle(true);
		}


	}
}
