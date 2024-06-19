using NDO.Logging;
using NUnit.Framework;
using Reisekosten.Personal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdoUnitTests
{
    [TestFixture]
    public class QueryCacheTests
    {
        void CreateObject()
        {
            var pm = PmFactory.NewPersistenceManager();
            Mitarbeiter m = new Mitarbeiter();
            m.Vorname = "Mirko";
            m.Nachname = "Matytschak";
            pm.MakePersistent( m );
            pm.Save();
        }

        [Test]
        public void TestSimpleQuery()
        {
            CreateObject();
            var pm = PmFactory.NewPersistenceManager();
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 0 ) );
            pm.UseQueryCache = true;
            var result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 1 ) );
            pm.VerboseMode = true;
            var testAdapter = new TestLogAdapter();
            pm.LogAdapter = testAdapter;
            result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( testAdapter.Text == "Getting results from QueryCache" );
        }
    }
}
