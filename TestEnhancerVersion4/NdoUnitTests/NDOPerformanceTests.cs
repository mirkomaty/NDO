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
