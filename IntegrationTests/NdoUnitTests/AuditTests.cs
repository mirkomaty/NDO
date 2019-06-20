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
			var handler = pm.GetSqlPassThroughHandler();
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Reise ) ).TableName}" );
			handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Adresse ) ).TableName}" );
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
		public void ChangeSetDetectsObjectAddition1_to_n()
		{
			m = pm.Objects<Mitarbeiter>().Single();
			var r = m.ErzeugeReise();
			r.Zweck = "Test";
			var changeObject = pm.GetChangeSet( m );
			var dict = (IDictionary<string, object>)changeObject;
			var original = (IDictionary<string, object>)dict["original"];
			var current = (IDictionary<string, object>)dict["current"];
			Assert.AreEqual( 1, original.Count );
			Assert.AreEqual( 1, current.Count );
			Assert.That( original.ContainsKey("dieReisen") );
			Assert.That( current.ContainsKey( "dieReisen" ) );
			Assert.AreEqual( 1, ((List<ObjectId>)original["dieReisen"]).Count );
			Assert.AreEqual( 2, ((List<ObjectId>)current["dieReisen"]).Count );
			Assert.AreEqual(r.NDOObjectId, ((List<ObjectId>)current["dieReisen"])[1] );
			// At this point it doesn't make any sense to serialize the changeObject,
			// since the id of r is not yet determined.
			Assert.That( (int)r.NDOObjectId.Id[0] < 0 );
			pm.Save();
			// Now the id of r is determined. Let's assert, that the list in current reflects the change.
			Assert.That( (int)r.NDOObjectId.Id[0] > 0 );
			Assert.AreEqual( r.NDOObjectId, ((List<ObjectId>)current["dieReisen"])[1] );
		}

		[Test]
		public void ChangeSetDetectsObjectDeletion1_to_n()
		{
			m = pm.Objects<Mitarbeiter>().Single();
			m.Löschen( m.Reisen[0] );
			var changeObject = pm.GetChangeSet( m );
			var dict = (IDictionary<string, object>)changeObject;
			var original = (IDictionary<string, object>)dict["original"];
			var current = (IDictionary<string, object>)dict["current"];
			Assert.AreEqual( 1, original.Count );
			Assert.AreEqual( 1, current.Count );
			Assert.That( original.ContainsKey( "dieReisen" ) );
			Assert.That( current.ContainsKey( "dieReisen" ) );
			Assert.AreEqual( 1, ((List<ObjectId>)original["dieReisen"]).Count );
			Assert.AreEqual( 0, ((List<ObjectId>)current["dieReisen"]).Count );
		}

		[Test]
		public void ChangeSetDetectsObjectAddition1_to_1()
		{
			m = pm.Objects<Mitarbeiter>().Single();
			Adresse a = new Adresse() { Ort = "München", Straße = "Teststr", Plz = "80133" };
			m.Adresse = a;
			var changeObject = pm.GetChangeSet( m );
			var dict = (IDictionary<string, object>)changeObject;
			var original = (IDictionary<string, object>)dict["original"];
			var current = (IDictionary<string, object>)dict["current"];
			Assert.AreEqual( 1, original.Count );
			Assert.AreEqual( 1, current.Count );
			Assert.That( original.ContainsKey( "adresse" ) );
			Assert.That( current.ContainsKey( "adresse" ) );
			Assert.AreEqual( a.NDOObjectId, ((List<ObjectId>)current["adresse"])[0] );
			// At this point it doesn't make any sense to serialize the changeObject,
			// since the id of r is not yet determined.
			Assert.That( (int)a.NDOObjectId.Id[0] < 0 );
			pm.Save();
			// Now the id of r is determined. Let's assert, that the list in current reflects the change.
			Assert.That( (int)a.NDOObjectId.Id[0] > 0 );
			Assert.AreEqual( a.NDOObjectId, ((List<ObjectId>)current["adresse"])[0] );
		}

		[Test]
		public void ChangeSetDetectsObjectDeletion1_to_1()
		{
			m = pm.Objects<Mitarbeiter>().Single();
			Adresse a = new Adresse() { Ort = "München", Straße = "Teststr", Plz = "80133" };
			m.Adresse = a;
			pm.Save();
			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Single();
			m.Adresse = null;
			var changeObject = pm.GetChangeSet( m );
			var dict = (IDictionary<string, object>)changeObject;
			var original = (IDictionary<string, object>)dict["original"];
			var current = (IDictionary<string, object>)dict["current"];
			Assert.AreEqual( 1, original.Count );
			Assert.AreEqual( 1, current.Count );
			Assert.That( original.ContainsKey( "adresse" ) );
			Assert.That( current.ContainsKey( "adresse" ) );
			Assert.AreEqual( 1, ((List<ObjectId>)original["adresse"]).Count );
			Assert.AreEqual( 0, ((List<ObjectId>)current["adresse"]).Count );
		}
	}
}
