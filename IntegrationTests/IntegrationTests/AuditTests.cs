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
using Newtonsoft.Json;

namespace NdoUnitTests
{
	[TestFixture]
	public class AuditTests
	{
		Mitarbeiter m;
		Reise r;

		[SetUp]
		public void Setup()
		{
			var pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter( "Mirko", "Matytschak" );
			pm.MakePersistent( m );
			m.ErzeugeReise().Zweck = "ADC";
			pm.Save();
			pm.UnloadCache();
		}

		[TearDown]
		public void TearDown()
		{
			var pm = PmFactory.NewPersistenceManager();
			pm.TransactionMode = TransactionMode.None;
			var mitarbeiterListe = pm.GetClassExtent( typeof( Mitarbeiter ), true );
			pm.Delete( mitarbeiterListe );
			pm.Save();
			pm.UnloadCache();
			using (var handler = pm.GetSqlPassThroughHandler())
			{
				handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Reise ) ).TableName}" );
				handler.Execute( $"DELETE FROM {pm.NDOMapping.FindClass( typeof( Adresse ) ).TableName}" );
			}
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
			var pm = PmFactory.NewPersistenceManager();
			m = pm.Objects<Mitarbeiter>().Single();
			m.Vorname = "Hans";
			var changeObject = pm.GetChangeSet( m );
			Assert.That(1 ==  changeObject.original.Count );
			Assert.That(1 ==  changeObject.current.Count );
			Assert.That("Mirko", Is.EqualTo( changeObject.original["vorname"] ) );
			Assert.That("Hans", Is.EqualTo( changeObject.current["vorname"] ) );
		}

		[Test]
		public void ChangeSetDetectsObjectAddition1_to_n()
		{
			var pm = PmFactory.NewPersistenceManager();
			m = pm.Objects<Mitarbeiter>().Single();
			var r = m.ErzeugeReise();
			r.Zweck = "Test";
			var changeObject = pm.GetChangeSet( m );
			var original = changeObject.original;
			var current = changeObject.current;
			Assert.That(1 ==  original.Count );
			Assert.That(1 ==  current.Count );
			Assert.That( original.ContainsKey("dieReisen") );
			Assert.That( current.ContainsKey( "dieReisen" ) );
			Assert.That(1 ==  ((List<ObjectId>)original["dieReisen"]).Count );
			Assert.That(2 ==  ((List<ObjectId>)current["dieReisen"]).Count );
			Assert.That(r.NDOObjectId ==  ((List<ObjectId>)current["dieReisen"])[1] );
			// At this point it doesn't make any sense to serialize the changeObject,
			// since the id of r is not yet determined.
			Assert.That( (int)r.NDOObjectId.Id[0] < 0 );
			pm.Save();
			// Now the id of r is determined. Let's assert, that the list in current reflects the change.
			Assert.That( (int)r.NDOObjectId.Id[0] > 0 );
			Assert.That(r.NDOObjectId ==  ((List<ObjectId>)current["dieReisen"])[1] );

			changeObject = changeObject.SerializableClone();
			original = changeObject.original;
			current = changeObject.current;
			Assert.That(1 ==  original.Count );
			Assert.That(1 ==  current.Count );
			Assert.That( original.ContainsKey( "dieReisen" ) );
			Assert.That( current.ContainsKey( "dieReisen" ) );
			Assert.That(1 ==  ( (List<string>) original["dieReisen"] ).Count );
			Assert.That(2 ==  ( (List<string>) current["dieReisen"] ).Count );
			Assert.That(r.NDOObjectId.ToShortId() ==  ( (List<string>) current["dieReisen"] )[1] );
			string json = JsonConvert.SerializeObject(changeObject);
		}

		[Test]
		public void ChangeSetDetectsObjectDeletion1_to_n()
		{
			var pm = PmFactory.NewPersistenceManager();
			m = pm.Objects<Mitarbeiter>().Single();
			m.Löschen( m.Reisen[0] );
			var changeObject = pm.GetChangeSet( m );
			var original = changeObject.original;
			var current = changeObject.current;
			Assert.That(1 ==  original.Count );
			Assert.That(1 ==  current.Count );
			Assert.That( original.ContainsKey( "dieReisen" ) );
			Assert.That( current.ContainsKey( "dieReisen" ) );
			Assert.That(1 ==  ((List<ObjectId>)original["dieReisen"]).Count );
			Assert.That(0 ==  ((List<ObjectId>)current["dieReisen"]).Count );
		}

		[Test]
		public void ChangeSetDetectsObjectAddition1_to_1()
		{
			var pm = PmFactory.NewPersistenceManager();
			m = pm.Objects<Mitarbeiter>().Single();
			Adresse a = new Adresse() { Ort = "München", Straße = "Teststr", Plz = "80133" };
			m.Adresse = a;
			var changeObject = pm.GetChangeSet( m );
			var original = changeObject.original;
			var current = changeObject.current;
			Assert.That(1 ==  original.Count );
			Assert.That(1 ==  current.Count );
			Assert.That( original.ContainsKey( "adresse" ) );
			Assert.That( current.ContainsKey( "adresse" ) );
			Assert.That(a.NDOObjectId ==  ((List<ObjectId>) current["adresse"])[0] );
			// At this point it doesn't make any sense to serialize the changeObject,
			// since the id of a is not yet determined.
			Assert.That( (int)a.NDOObjectId.Id[0] < 0 );
			pm.Save();
			var newChangeObject = changeObject.SerializableClone();
			// Now the id of r is determined. Let's assert, that the list in current reflects the change.
			Assert.That( (int)a.NDOObjectId.Id[0] > 0 );
			Assert.That(a.NDOObjectId.ToShortId() ==  ((List<string>)newChangeObject.current["adresse"])[0] );
		}

		[Test]
		public void ChangeSetDetectsObjectDeletion1_to_1()
		{
			var pm = PmFactory.NewPersistenceManager();
			m = pm.Objects<Mitarbeiter>().Single();
			Adresse a = new Adresse() { Ort = "München", Straße = "Teststr", Plz = "80133" };
			m.Adresse = a;
			pm.Save();
			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Single();
			m.Adresse = null;
			var changeObject = pm.GetChangeSet( m );
			var original = changeObject.original;
			var current = changeObject.current;
			Assert.That(1 ==  original.Count );
			Assert.That(1 ==  current.Count );
			Assert.That( original.ContainsKey( "adresse" ) );
			Assert.That( current.ContainsKey( "adresse" ) );
			Assert.That(1 ==  ((List<ObjectId>)original["adresse"]).Count );
			Assert.That(0 ==  ((List<ObjectId>)current["adresse"]).Count );
		}
	}
}
