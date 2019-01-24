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
using Reisekosten.Personal;

namespace NdoUnitTests
{
	/// <summary>
	/// Do some performance tests.
	/// </summary>
	[TestFixture]
	public class NDOPerformanceTests
	{
		public NDOPerformanceTests()
		{
		}

		private int ticks;
		private const int count = 1000;

		private void Start() {
			ticks = System.Environment.TickCount;
		}

		private void Time(string msg) {
			string time = string.Format("{0:#,##0} ms", (System.Environment.TickCount  - ticks));
			Console.WriteLine(msg + ": " + time);
		}

		private PersistenceManager pm;

		[SetUp]
		public void Setup() {
			pm = PmFactory.NewPersistenceManager();
		}

		[TearDown]
		public void TearDown() {
			IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), true);
			pm.Delete(mitarbeiterListe);
			pm.Save();
			pm.Close();
			pm = null;
		}

		[Test]
		[Ignore("Dauert zu lange")]
		public void TestCreation() {
			Start();
			for(int i = 0; i < count; i++) {
				pm.MakePersistent(CreateMitarbeiter("Hartmut", i.ToString()));
			}
			Time("Objects created");
			pm.Abort();
			Time("Aborted");
			Start();
			for(int i = 0; i < count; i++) {
				pm.MakePersistent(CreateMitarbeiter("Hartmut", i.ToString()));
			}
			pm.Save();
			Time("Created and Saved");
			Start();
			pm.GetClassExtent(typeof(Mitarbeiter));
			Time("Extent with cache");
			Start();
			pm.UnloadCache();
			Time("UnloadCache");
			Start();
			IList l = pm.GetClassExtent(typeof(Mitarbeiter));
			Time("Extent w/o cache");
			Start();
			pm.Refresh(l);
			Time("Refresh");
		}

		private Mitarbeiter CreateMitarbeiter(string vorname, string nachname) {
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

	}
}
