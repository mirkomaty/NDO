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

        [TearDown]
        public void TearDown()
        {
            var pm = PmFactory.NewPersistenceManager();
            pm.Objects<Mitarbeiter>().DeleteDirectly();
        }

        [Test]
        public void TestSimpleQuery()
        {
            CreateObject();
            var pm = PmFactory.NewPersistenceManager();
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 0 ) );
            pm.UseQueryCache = true;
            var result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 1 ) );
            pm.VerboseMode = true;
            var testAdapter = new TestLogAdapter();
            pm.LogAdapter = testAdapter;
            result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( testAdapter.Text.Trim() == "Getting results from QueryCache" );
        }

        [Test]
        public void QueryCacheCanBeSwitchedOff()
        {
            CreateObject();
            var pm = PmFactory.NewPersistenceManager();
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 0 ) );
            pm.UseQueryCache = false;
            var result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 0 ) );
            pm.VerboseMode = true;
            var testAdapter = new TestLogAdapter();
            pm.LogAdapter = testAdapter;
            result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( testAdapter.Text.IndexOf( "QueryCache" ) == -1 );
        }

        [Test]
        public void DifferentQueryDoesntUseTheCache()
        {
            CreateObject();
            var pm = PmFactory.NewPersistenceManager();
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 0 ) );
            pm.UseQueryCache = true;
            var result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 1 ) );
            pm.VerboseMode = true;
            var testAdapter = new TestLogAdapter();
            pm.LogAdapter = testAdapter;
            result = pm.Objects<Mitarbeiter>().Where(m=>m.Vorname == "Mirko").ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( testAdapter.Text.IndexOf( "QueryCache" ) == -1 );
        }

        [Test]
        public void TakeAndSkipDontUseTheSameCacheResult()
        {
            CreateObject();
            var pm = PmFactory.NewPersistenceManager();
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 0 ) );
            pm.UseQueryCache = true;
            var result = pm.Objects<Mitarbeiter>().OrderBy(m=>m.Nachname).Skip(0).Take(1).ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( pm.QueryCache.Count, Is.EqualTo( 1 ) );
            pm.VerboseMode = true;
            var testAdapter = new TestLogAdapter();
            pm.LogAdapter = testAdapter;
            result = pm.Objects<Mitarbeiter>().OrderBy(m=>m.Nachname).Skip(0).Take(2).ResultTable;
            Assert.That( result.Count == 1 );
            Assert.That( testAdapter.Text.IndexOf( "QueryCache" ) == -1 );
        }
    }
}
