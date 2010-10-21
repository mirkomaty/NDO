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
using System.IO;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using Reisekosten;
using Reisekosten.Personal;
using PureBusinessClasses;

namespace NdoUnitTests
{
	[TestFixture]
	public class SelfReference
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
				pm.UnloadCache();
				IList mitarbeiterListe = pm.GetClassExtent(typeof(Mitarbeiter), true);
				// Normal geht das mit Accessoren nicht, weil sie Read Only sind
				// aber hier spielts keine Rolle.
				foreach(Mitarbeiter m in mitarbeiterListe)
					m.Untergebene.Clear();
				pm.Delete(mitarbeiterListe);
				pm.Save();
				pm.Close();
				pm = null;
			}
		}

		[Test]
		public void TestMitarbeiterHierarchie()
		{
			Mitarbeiter m = new  Mitarbeiter();
			m.Vorname = "Mirko"; 
			m.Nachname = "Matytschak";
			pm.MakePersistent(m);
			pm.Save();
			Mitarbeiter m2;
			m2 = new Mitarbeiter();
			m2.Vorname = "Halli";
			m2.Nachname = "Galli";
			pm.MakePersistent(m2);
			m2.Vorgesetzter = m;
			Assert.AreEqual(NDOObjectState.Created, m2.NDOObjectState, "m2 m�sste Created sein");
			Assert.AreEqual(1, m.Untergebene.Count, "m m�sste einen Untergebenen in der Liste haben");
			Mitarbeiter m3 = new Mitarbeiter();
			m3.Vorname = "m3";
			m3.Nachname = "dummy";
			pm.MakePersistent(m3);
			m2.AddUntergebener(m3);
			Assert.NotNull(m3.Vorgesetzter, "m3 m�sste einen Chef haben");
			pm.Save();
			pm.UnloadCache();
			m = (Mitarbeiter) pm.FindObject(m.NDOObjectId);
			Assert.AreEqual(1, m.Untergebene.Count, "Anzahl untergebene falsch");
			m2 = (Mitarbeiter) m.Untergebene[0];
			Assert.AreEqual(m2.Vorgesetzter, m, "Chef falsch");
			Assert.AreEqual(1, m2.Untergebene.Count, "Anzahl untergebene falsch #2");
			m3 = (Mitarbeiter) m2.Untergebene[0];
			Assert.AreEqual(m3.Vorgesetzter, m2, "Chef falsch");

		}

		[Test]
		public void TestTopicHierarchie()
		{
			Topic t = new  Topic("Main Topic");
			pm.MakePersistent(t);
			pm.Save();

			Topic t2 = t.NewSubTopic("Middle Topic");
			Assert.AreEqual(NDOObjectState.Created, t2.NDOObjectState, "t2 m�sste Created sein");
			Assert.AreEqual(t, t2.Owner, "Owner stimmt nicht");
			Assert.AreEqual(1, t.Subtopics.Count, "t m�sste einen Untergebenen in der Liste haben");

			Topic t3 = t2.NewSubTopic("End Topic");
			Assert.AreEqual(NDOObjectState.Created, t3.NDOObjectState, "t3 m�sste Created sein");
			Assert.NotNull(t3.Owner, "t3 m�sste einen Owner haben");

			pm.Save();

			pm.UnloadCache();

			Query q = pm.NewQuery(typeof (Topic), "oid = {0}");
            q.Parameters.Add(t.NDOObjectId.Id[0]);
			t = (Topic) q.ExecuteSingle(true);
			Assert.AreEqual(1, t.Subtopics.Count, "t m�sste einen Untergebenen in der Liste haben");

			t2 = (Topic) t.Subtopics[0];
			Assert.NotNull(t2.Owner, "t2 m�sste einen Owner haben");
			Assert.AreEqual(1, t2.Subtopics.Count, "t2 m�sste einen Untergebenen in der Liste haben");

			t3 = (Topic) t2.Subtopics[0];
			
			t.RemoveSubTopic(t2);
			Assert.Null(t2.Owner, "Owner muss null sein");
			Assert.AreEqual(NDOObjectState.Deleted, t2.NDOObjectState, "t2 muss gel�scht sein");
			Assert.AreEqual(NDOObjectState.Deleted, t3.NDOObjectState, "t3 muss gel�scht sein");
			pm.Save();
		}

		[Test]
		public void TestTopicHierarchieOnce()
		{
			Topic t = new  Topic("Main Topic");

			Topic t2 = t.NewSubTopic("Middle Topic");
			pm.MakePersistent(t);

			Topic t3 = t2.NewSubTopic("End Topic");

			Assert.AreEqual(NDOObjectState.Created, t2.NDOObjectState, "t2 m�sste Created sein");
			Assert.AreEqual(t, t2.Owner, "Owner stimmt nicht");
			Assert.AreEqual(1, t.Subtopics.Count, "t m�sste einen Untergebenen in der Liste haben");
			Assert.AreEqual(NDOObjectState.Created, t3.NDOObjectState, "t3 m�sste Created sein");
			Assert.NotNull(t3.Owner, "t3 m�sste einen Owner haben");
			pm.Save();

			pm.UnloadCache();

			Query q = pm.NewQuery(typeof (Topic), "oid = {0}");
            q.Parameters.Add(t.NDOObjectId.Id[0]);
			t = (Topic) q.ExecuteSingle(true);
			Assert.AreEqual(1, t.Subtopics.Count, "t m�sste einen Untergebenen in der Liste haben");

			t2 = (Topic) t.Subtopics[0];
			Assert.NotNull(t2.Owner, "t2 m�sste einen Owner haben");
			Assert.AreEqual(1, t2.Subtopics.Count, "t2 m�sste einen Untergebenen in der Liste haben");

			t3 = (Topic) t2.Subtopics[0];
			
			t.RemoveSubTopic(t2);
			Assert.Null(t2.Owner, "Owner muss null sein");
			Assert.AreEqual(NDOObjectState.Deleted, t2.NDOObjectState, "t2 muss gel�scht sein");
			Assert.AreEqual(NDOObjectState.Deleted, t3.NDOObjectState, "t3 muss gel�scht sein");
			pm.Save();
		}


		[Test]
		public void TestTopicHierarchieWorkaround()
		{
			Topic t = new  Topic("Main Topic");
			pm.MakePersistent(t);
			pm.Save();

			Topic t2 = t.NewSubTopic("Middle Topic");
			Assert.AreEqual(NDOObjectState.Created, t2.NDOObjectState, "t2 m�sste Created sein");
			Assert.AreEqual(t, t2.Owner, "Owner stimmt nicht");
			Assert.AreEqual(1, t.Subtopics.Count, "t m�sste einen Untergebenen in der Liste haben");

			Topic t3 = t2.NewSubTopic("End Topic");
			Assert.AreEqual(NDOObjectState.Created, t3.NDOObjectState, "t3 m�sste Created sein");
			Assert.NotNull(t3.Owner, "t3 m�sste einen Owner haben");

			pm.Save();

			pm.UnloadCache();

			Query q = pm.NewQuery(typeof (Topic), "oid = {0}");
            q.Parameters.Add(t.NDOObjectId.Id[0]);
            t = (Topic)q.ExecuteSingle(true);
			Assert.AreEqual(1, t.Subtopics.Count, "t m�sste einen Untergebenen in der Liste haben");

			t2 = (Topic) t.Subtopics[0];
			ObjectId oid2 = t2.NDOObjectId;
			Assert.NotNull(t2.Owner, "t2 m�sste einen Owner haben");
			Assert.AreEqual(1, t2.Subtopics.Count, "t2 m�sste einen Untergebenen in der Liste haben");

			t3 = (Topic) t2.Subtopics[0];
			ObjectId oid3 = t3.NDOObjectId;
			
			t2.RemoveSubTopic(t3);
			t.RemoveSubTopic(t2);
			Assert.Null(t2.Owner, "Owner muss null sein");
			Assert.AreEqual(NDOObjectState.Deleted, t2.NDOObjectState, "t2 muss gel�scht sein");
			Assert.AreEqual(NDOObjectState.Deleted, t3.NDOObjectState, "t3 muss gel�scht sein");
			pm.Delete(t);
			pm.Save();
			pm.UnloadCache();
			IList l = q.Execute();
			Assert.AreEqual(0, l.Count, "Liste muss leer sein 1");
			q = pm.NewQuery(typeof (Topic), "oid = {0}");
            q.Parameters.Add(oid2.Id[0]);
			l = q.Execute();
			Assert.AreEqual(0, l.Count, "Liste muss leer sein 2");
			q = pm.NewQuery(typeof (Topic), "oid = {0}");
            q.Parameters.Add(oid3.Id[0]);
            l = q.Execute();
			Assert.AreEqual(0, l.Count, "Liste muss leer sein 3");
		}

	}
}
