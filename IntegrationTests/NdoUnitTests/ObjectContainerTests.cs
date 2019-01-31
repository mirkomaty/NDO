using NDO;
using NUnit.Framework;
using Reisekosten;
using Reisekosten.Personal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

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
				pm = null;
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

		[Test]
		public void ObjectContainerIsSerializable()
		{
			var binaryFormatter = new BinaryFormatter();

			pm.MakePersistent( m );
			pm.Save();
			var oc = pm.GetObjectContainer();
			Assert.That( Object.ReferenceEquals( m, oc.RootObjects[0] ) );

			oc.Formatter = binaryFormatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			var oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 0, oc2.RootObjects.Count );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, binaryFormatter );
			pm.MergeObjectContainer( oc );

			oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 1, oc2.RootObjects.Count );

			m = (Mitarbeiter)oc2.RootObjects[0];
			Assert.AreEqual( "Mirko", m.Vorname );
			Assert.AreEqual( NDOObjectState.Persistent, m.NDOObjectState );
		}

		[Test]
		public void CompleteTurnaroundWithChangeSetContainer()
		{
			// Create object and serialize it
			var binaryFormatter = new BinaryFormatter();

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
			Assert.AreEqual( NDOObjectState.Persistent, m2.NDOObjectState );
			m2.Vorname = "Hans";
			Assert.AreEqual( NDOObjectState.PersistentDirty, m2.NDOObjectState );

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
			Assert.AreEqual( NDOObjectState.PersistentDirty, m2.NDOObjectState );
			Assert.AreEqual( "Hans", m.Vorname );

			// Save and Reload
			pm.Save();
			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Single();
			Assert.AreEqual( "Hans", m.Vorname );
		}

		[Test]
		public void CanSetObjectsToHollow()
		{
			var binaryFormatter = new BinaryFormatter();
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

			Assert.AreEqual( NDOObjectState.Persistent, m.NDOObjectState );

			oc.Formatter = binaryFormatter;
			string serialized = oc.MarshalingString;

			Assert.AreEqual( 2, oc.RootObjects.Count );
			Assert.AreEqual( NDOObjectState.Hollow, m.NDOObjectState );
			Assert.AreEqual( NDOObjectState.Hollow, reise.NDOObjectState );
		}

		[Test]
		public void ObjectContainerSerializesRelations()
		{
			var binaryFormatter = new BinaryFormatter();
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

			Assert.AreEqual( 2, oc.RootObjects.Count );

			pm.UnloadCache();

			var oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 0, oc2.RootObjects.Count );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, binaryFormatter );
			pm.MergeObjectContainer( oc );

			oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 2, oc2.RootObjects.Count );

			Reise r2 = null;
			Mitarbeiter m2 = null;

			foreach(object o in oc2.RootObjects)
			{
				if (o is Reise)
					r2 = (Reise)o;
				if (o is Mitarbeiter)
					m2 = (Mitarbeiter)o;
			}

			Assert.NotNull( r2 );
			Assert.NotNull( m2 );

			Assert.AreEqual( "Mirko", m2.Vorname );
			Assert.AreEqual( NDOObjectState.Persistent, m2.NDOObjectState );

			Assert.AreEqual( "ADC", r2.Zweck );
			Assert.AreEqual( NDOObjectState.Persistent, r2.NDOObjectState );
		}

	}
}
