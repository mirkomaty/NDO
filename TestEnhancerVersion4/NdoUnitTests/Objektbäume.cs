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
using Reisekosten.Personal;
using Reisekosten;
using NDO;
using NUnit.Framework;

namespace NdoUnitTests
{
	[TestFixture]
	public class Objektbäume
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
			if (null != pm)
			{
				IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), true);
				pm.Delete(mitarbeiterListe);
				pm.Save();
				pm.Close();
				pm = null;
			}
		}


		[Test]
		public void TestObjektbaum()
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = "Test";
			m.Nachname = "Objektbaum";
			Reise r = new Reise();
			r.Zweck ="Reisezweck";
			Beleg b = new Beleg("Taxi", 30.4);
			r.AddKostenpunkt(b);
			m.Hinzufuegen(r);
			
			Assert.AreEqual(NDOObjectState.Transient, m.NDOObjectState, "Mitarbeiter muss Transient sein");
			Assert.AreEqual(NDOObjectState.Transient, r.NDOObjectState, "Reise muss Transient sein");
			Assert.AreEqual(NDOObjectState.Transient, b.NDOObjectState, "Beleg muss Transient sein");


			pm.MakePersistent(m);

			Assert.AreEqual(NDOObjectState.Created, m.NDOObjectState, "Mitarbeiter muss Created sein");
			Assert.AreEqual(NDOObjectState.Created, r.NDOObjectState, "Reise muss Created sein");
			Assert.AreEqual(NDOObjectState.Created, b.NDOObjectState, "Beleg muss Created sein");

			pm.Save();

			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "1 Mitarbeiter muss Persistent sein");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "1 Reise muss Persistent sein");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "1 Beleg muss Persistent sein");

			pm.UnloadCache();

			Query q = pm.NewQuery(typeof(Mitarbeiter), null);
			m = (Mitarbeiter) q.ExecuteSingle(true);
			Assert.AreEqual(1, m.Reisen.Count, "Reise nicht gefunden");
			r = (Reise) m.Reisen[0];
			Assert.AreEqual(1, r.Kostenpunkte.Count, "Kostenpunkt nicht gefunden");
			b = (Beleg) r.Kostenpunkte[0];
			double k = b.Kosten;

			Assert.AreEqual(NDOObjectState.Persistent, m.NDOObjectState, "2 Mitarbeiter muss Persistent sein");
			Assert.AreEqual(NDOObjectState.Persistent, r.NDOObjectState, "2 Reise muss Persistent sein");
			Assert.AreEqual(NDOObjectState.Persistent, b.NDOObjectState, "2 Beleg muss Persistent sein");

		}
	}
}
