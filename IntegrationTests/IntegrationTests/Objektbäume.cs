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
using Reisekosten.Personal;
using Reisekosten;
using NDO;
using NUnit.Framework;
using NDO.Query;

namespace NdoUnitTests
{
	[TestFixture]
	public class Objektbäume : NDOTest
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
			
			Assert.That(NDOObjectState.Transient ==  m.NDOObjectState, "Mitarbeiter muss Transient sein");
			Assert.That(NDOObjectState.Transient ==  r.NDOObjectState, "Reise muss Transient sein");
			Assert.That(NDOObjectState.Transient ==  b.NDOObjectState, "Beleg muss Transient sein");


			pm.MakePersistent(m);

			Assert.That(NDOObjectState.Created ==  m.NDOObjectState, "Mitarbeiter muss Created sein");
			Assert.That(NDOObjectState.Created ==  r.NDOObjectState, "Reise muss Created sein");
			Assert.That(NDOObjectState.Created ==  b.NDOObjectState, "Beleg muss Created sein");

			pm.Save();

			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "1 Mitarbeiter muss Persistent sein");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "1 Reise muss Persistent sein");
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "1 Beleg muss Persistent sein");

			pm.UnloadCache();

			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm, null);
			m = (Mitarbeiter) q.ExecuteSingle(true);
			Assert.That(1 ==  m.Reisen.Count, "Reise nicht gefunden");
			r = (Reise) m.Reisen[0];
			Assert.That(1 ==  r.Kostenpunkte.Count, "Kostenpunkt nicht gefunden");
			b = (Beleg) r.Kostenpunkte[0];
			double k = b.Kosten;

			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState, "2 Mitarbeiter muss Persistent sein");
			Assert.That(NDOObjectState.Persistent ==  r.NDOObjectState, "2 Reise muss Persistent sein");
			Assert.That(NDOObjectState.Persistent ==  b.NDOObjectState, "2 Beleg muss Persistent sein");

		}
	}
}
