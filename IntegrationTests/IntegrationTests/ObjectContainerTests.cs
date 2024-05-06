using NDO;
using NDOInterfaces;
using NUnit.Framework;
using Reisekosten;
using Reisekosten.Personal;
using System;

namespace NdoUnitTests
{
	[TestFixture]
	public class ObjectContainerTests
	{
		private PersistenceManager pm;
		private Mitarbeiter m;


		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			m = CreateMitarbeiter( "Mirko", "Matytschak" );
			m.Position = new System.Drawing.Point( 12, 34 );
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
		public void ObjectContainerContainsCreatedObject()
		{
			pm.MakePersistent( m );
			var oc = pm.GetObjectContainer();
			Assert.That( Object.ReferenceEquals( m, oc.RootObjects[0] ) );
			pm.Save();
			oc = pm.GetObjectContainer();
			Assert.That( Object.ReferenceEquals( m, oc.RootObjects[0] ) );
			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Single();
			oc = pm.GetObjectContainer();
			Assert.That( Object.ReferenceEquals( m, oc.RootObjects[0] ) );
		}

		INdoFormatter GetFormatter()
		{
			return new NDO.JsonFormatter.NdoJsonFormatter();
		}

		[Test]
		public void ObjectContainerIsSerializable()
		{
			var binaryFormatter = GetFormatter();

			pm.MakePersistent( m );
			pm.Save();
			var oc = pm.GetObjectContainer();
			Assert.That( Object.ReferenceEquals( m, oc.RootObjects[0] ) );

			oc.Formatter = binaryFormatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			var oc2 = pm.GetObjectContainer();
			Assert.That(0 ==  oc2.RootObjects.Count );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, binaryFormatter );
			pm.MergeObjectContainer( oc );

			oc2 = pm.GetObjectContainer();
			Assert.That(1 ==  oc2.RootObjects.Count );

			m = (Mitarbeiter)oc2.RootObjects[0];
			Assert.That("Mirko" ==  m.Vorname );
			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState );
		}

		[Test]
		public void CompleteTurnaroundWithChangeSetContainer()
		{
			// Create object and serialize it
			var binaryFormatter = GetFormatter();

			pm.MakePersistent( m );
			pm.Save();
			var oc = pm.GetObjectContainer();			

			oc.Formatter = binaryFormatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			// Merge object in to an OfflinePersistenceManager and change it
			OfflinePersistenceManager opm = new OfflinePersistenceManager( pm.NDOMapping.FileName );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, binaryFormatter );
			
			opm.MergeObjectContainer( oc );

			var m2 = (Mitarbeiter)oc.RootObjects[0];
			Assert.That(NDOObjectState.Persistent ==  m2.NDOObjectState );
			m2.Vorname = "Hans";
			Assert.That(NDOObjectState.PersistentDirty ==  m2.NDOObjectState );

			// Create a ChangeSetContainer and serialize it
			var csc = opm.GetChangeSet();
			csc.Formatter = binaryFormatter;
			string serializedChanges = csc.MarshalingString;

			// Merge the changes back to pm
			csc = new ChangeSetContainer();
			csc.Deserialize( serializedChanges, binaryFormatter );
			m = (Mitarbeiter)csc.ChangedObjects[0];
			pm.MergeObjectContainer( csc );
			m2 = (Mitarbeiter)pm.FindObject( m.NDOObjectId );
			Assert.That(NDOObjectState.PersistentDirty ==  m2.NDOObjectState );
			Assert.That("Hans" ==  m.Vorname );

			// Save and Reload
			pm.Save();
			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Single();
			Assert.That("Hans" ==  m.Vorname );
		}

		[Test]
		public void CanSetObjectsToHollow()
		{
			var binaryFormatter = GetFormatter();
			var serializationIterator = new NDO.SerializationIterator( r => r.ReferencedType == typeof( Reise ), pc => pc.NDOObjectState = NDOObjectState.Hollow );

			pm.MakePersistent( m );
			Reise reise;
			m.Hinzufuegen( reise = new Reise() { Zweck = "ADC" } );
			pm.Save();

			var oc = pm.GetObjectContainer();
			oc.SerialisationIterator = serializationIterator;

			bool found = false;
			foreach (object o in oc.RootObjects)
			{
				if (Object.ReferenceEquals( m, o ))
					found = true;
			}
			Assert.That( found );

			Assert.That(NDOObjectState.Persistent ==  m.NDOObjectState );

			oc.Formatter = binaryFormatter;
			string serialized = oc.MarshalingString;

			Assert.That(2 ==  oc.RootObjects.Count );
			Assert.That(NDOObjectState.Hollow ==  m.NDOObjectState );
			Assert.That(NDOObjectState.Hollow ==  reise.NDOObjectState );
		}

		[Test]
		public void ObjectContainerSerializesRelations()
		{
			var binaryFormatter = GetFormatter();
			var serializationIterator = new NDO.SerializationIterator( r => r.ReferencedType == typeof( Reise ) );

			pm.MakePersistent( m );
			m.Hinzufuegen( new Reise() {Zweck = "ADC" } );
			pm.Save();

			var oc = pm.GetObjectContainer();

			bool found = false;
			foreach (object o in oc.RootObjects)
			{
				if (Object.ReferenceEquals( m, o ))
					found = true;
			}
			Assert.That( found );

			oc.SerialisationIterator = serializationIterator;
			oc.Formatter = binaryFormatter;
			string serialized = oc.MarshalingString;

			Assert.That(2 ==  oc.RootObjects.Count );

			pm.UnloadCache();

			var oc2 = pm.GetObjectContainer();
			Assert.That(0 ==  oc2.RootObjects.Count );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, binaryFormatter );
			pm.MergeObjectContainer( oc );

			oc2 = pm.GetObjectContainer();
			Assert.That(2 ==  oc2.RootObjects.Count );

			Reise r2 = null;
			Mitarbeiter m2 = null;

			foreach(object o in oc2.RootObjects)
			{
				if (o is Reise)
					r2 = (Reise)o;
				if (o is Mitarbeiter)
					m2 = (Mitarbeiter)o;
			}

			Assert.That(r2  != null);
			Assert.That(m2  != null);

			Assert.That("Mirko" ==  m2.Vorname );
			Assert.That(NDOObjectState.Persistent ==  m2.NDOObjectState );

			Assert.That("ADC" ==  r2.Zweck );
			Assert.That(NDOObjectState.Persistent ==  r2.NDOObjectState );
		}

	}
}
