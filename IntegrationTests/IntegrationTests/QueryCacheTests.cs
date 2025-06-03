using Formfakten.TestLogger;
using NUnit.Framework;
using Reisekosten.Personal;

namespace NdoUnitTests
{
    [TestFixture]
    public class QueryCacheTests : NDOTest
    {
        public QueryCacheTests()
        {
		}
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
            Logger.ClearTestLogs();
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
            result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( result.Count == 1 );
            var logs = Logger.FindLogsWith( "Getting results from QueryCache" );
            Assert.That( logs.Count > 0 );
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
            result = pm.Objects<Mitarbeiter>().ResultTable;
            Assert.That( result.Count == 1 );
			var logs = Logger.FindLogsWith( "QueryCache" );
			Assert.That( logs.Count == 0 );
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
            result = pm.Objects<Mitarbeiter>().Where(m=>m.Vorname == "Mirko").ResultTable;
            Assert.That( result.Count == 1 );
			var logs = Logger.FindLogsWith( "QueryCache" );
			Assert.That( logs.Count == 0 );
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
            result = pm.Objects<Mitarbeiter>().OrderBy(m=>m.Nachname).Skip(0).Take(2).ResultTable;
            Assert.That( result.Count == 1 );
			var logs = Logger.FindLogsWith( "QueryCache" );
			Assert.That( logs.Count == 0 );
		}
    }
}
