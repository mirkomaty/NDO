using NDO;
using NDO.Query;
using NUnit.Framework;
using Reisekosten;
using Reisekosten.Personal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdoUnitTests
{
	[TestFixture]
	public class PrefetchTests : NDOTest
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
				var mitarbeiterListe = pm.Objects<Mitarbeiter>().ResultTable;
				if (mitarbeiterListe.Count > 0)
				{
					pm.Delete( mitarbeiterListe );
					pm.Save();
				}
				pm.Close();
				pm = null;
			}
		}

		[Test]
		public void PrefetchOccurs()
		{
			Reise r = new Reise() { Zweck = "NDO" };
			Mitarbeiter m = new Mitarbeiter() { Vorname = "Mirko", Nachname = "Matytschak" };
			pm.MakePersistent( m );
			m.Hinzufuegen( r );
			pm.Save();

			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>(pm);
			q.AddPrefetch( nameof( Mitarbeiter.Reisen ) );

			var m2 = q.ExecuteSingle();

			var oc = pm.GetObjectContainer();
			Assert.AreEqual( 2, oc.RootObjects.Count );
		}
	}
}
