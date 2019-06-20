using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PureBusinessClasses;
using NDO;
using NDO.ShortId;
using Reisekosten.Personal;
using Reisekosten;

namespace NdoUnitTests
{
	[TestFixture]
	public class AuditTests
	{
		PersistenceManager pm;
		Mitarbeiter m;
		Reise r;
		int reiseTypeCode;

		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter( "Mirko", "Matytschak" );
			pm.MakePersistent( m );
			m.ErzeugeReise().Zweck = "ADC";
			pm.Save();
			pm.UnloadCache();
			this.reiseTypeCode = pm.NDOMapping.FindClass( typeof( Reise ) ).TypeCode;
		}

		[TearDown]
		public void TearDown()
		{
			pm.Abort();
			pm.UnloadCache();
			var mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
			pm.Delete( mitarbeiterListe );
			pm.Save();
			pm.UnloadCache();
			pm.GetSqlPassThroughHandler().Execute( $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Reise ) ).TableName}" );
			pm = null;
		}

		private Mitarbeiter CreateMitarbeiter( string vorname, string nachname )
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = vorname;
			m.Nachname = nachname;
			return m;
		}

		[Test]
		public void ChangeSetDetectsSimpleChange()
		{
			m = pm.Objects<Mitarbeiter>().Single();
			m.Vorname = "Hans";
			var changeObject = pm.GetChangeSet( m );
			var dict = (IDictionary<string,object>)changeObject;
			var original = (IDictionary<string, object>)dict["original"];
			var current = (IDictionary<string, object>)dict["current"];
			Assert.AreEqual( 1, original.Count );
			Assert.AreEqual( 1, current.Count );
			Assert.AreEqual( "Mirko", original["vorname"] );
			Assert.AreEqual( "Hans", current["vorname"] );
		}

		[Test]
		public void ChangeSetDetectsObjectAddition()
		{
			m = pm.Objects<Mitarbeiter>().Single();
			var r = m.ErzeugeReise();
			r.Zweck = "Test";
			var shortId = r.ShortId();
			shortId = shortId.Substring( 0, shortId.LastIndexOf( '-' ) + 1 ) + '?';
			var changeObject = pm.GetChangeSet( m );
			var dict = (IDictionary<string, object>)changeObject;
			var original = (IDictionary<string, object>)dict["original"];
			var current = (IDictionary<string, object>)dict["current"];
			Assert.AreEqual( 1, original.Count );
			Assert.AreEqual( 1, current.Count );
			Assert.That( original.ContainsKey("dieReisen") );
			Assert.That( current.ContainsKey( "dieReisen" ) );
			Assert.AreEqual(shortId, ((List<string>)current["dieReisen"])[1] );
		}
	}
}
