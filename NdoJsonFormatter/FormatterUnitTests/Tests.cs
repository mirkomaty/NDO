//
// Copyright (c) 2002-2020 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

using NDO.JsonFormatter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reisekosten;
using Reisekosten.Personal;
using NDO;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace FormatterUnitTests
{
	[TestFixture]
    public class Tests
    {
		PersistenceManager pm;

		[SetUp]
		public void SetUp()
		{
			this.pm = new PersistenceManager();
		}

		[TearDown]
		public void TearDown()
		{
			var ml = this.pm.Objects<Mitarbeiter>().ResultTable;
			this.pm.Delete( ml );
			this.pm.Save();

			var rl = this.pm.Objects<Reise>().ResultTable;
			this.pm.Delete(rl);
			this.pm.Save();

			var ll = this.pm.Objects<Land>().ResultTable;
			this.pm.Delete(ll);
			this.pm.Save();

		}

		Mitarbeiter CreatePersistentMitarbeiter()
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = "Mirko";
			m.Nachname = "Matytschak";
			pm.MakePersistent( m );
			pm.Save();
			var objectId = m.NDOObjectId;
			pm.UnloadCache();
			m = (Mitarbeiter)this.pm.FindObject( objectId );
			return m;
		}

		Mitarbeiter CreateMitarbeiterWithTravelAndAddress()
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = "Mirko";
			m.Nachname = "Matytschak";
			pm.MakePersistent( m );
			m.Hinzufuegen( new Reise { Zweck = "Vortrag" } );
			m.Adresse = new Adresse { Lkz = "D", Plz = "84149", Straße = "Ahornstr. 5", Ort="Eberspoint" };
			pm.Save();
			var objectId = m.NDOObjectId;
			pm.UnloadCache();
			m = (Mitarbeiter) this.pm.FindObject( objectId );
			return m;
		}

		private Mitarbeiter CreateMitarbeiter()
		{
			Mitarbeiter m = new Mitarbeiter();
			m.Vorname = "Mirko";
			m.Nachname = "Matytschak";
			m.Position = new System.Drawing.Point( 111, 222 );
			return m;
		}

		[Test]
		public void CanSerializeObject()
		{
			NdoJsonFormatter formatter = new NdoJsonFormatter(this.pm);
			var m = CreatePersistentMitarbeiter();
			Assert.IsNotNull( m );
			var ms = new MemoryStream();
			formatter.Serialize( ms, m );
			ms.Seek( 0L, SeekOrigin.Begin );
			string json = null;
			using (StreamReader sr = new StreamReader( ms ))
			{
				json = sr.ReadToEnd();
			}

			Assert.IsNotNull( json );
		}

		[Test]
		public void CanGetObjectContainer()
		{
			CreateObjectContainer();
		}

		private string CreateObjectContainer(bool serializeAddress = false)
		{
			NdoJsonFormatter formatter = new NdoJsonFormatter();
			Mitarbeiter user = CreateMitarbeiterWithTravelAndAddress();
			Assert.IsNotNull( user );
			var relations = serializeAddress ? new string[] { "dieReisen", "adresse" } : new string[]{"dieReisen"};
			ObjectContainer container = new ObjectContainer(r=>relations.Contains( r.FieldName ));
			container.AddObject( user );

			var ms = new MemoryStream();
			container.Serialize( ms, formatter );
			ms.Seek( 0L, SeekOrigin.Begin );
			string json = null;
			using (StreamReader sr = new StreamReader( ms ))
			{
				json = sr.ReadToEnd();
			}

			Assert.IsNotNull( json );
			var jObj = JsonConvert.DeserializeObject<JObject>(json);
			var rootObjects = (JArray)jObj["rootObjects"];
			var additionalObjects = (JArray)jObj["additionalObjects"];
			Assert.NotNull( rootObjects );
			Assert.NotNull( additionalObjects );
			Assert.AreEqual( 1, rootObjects.Count() );
			Assert.AreEqual( serializeAddress ? 2 : 1, additionalObjects.Count() );
			var rootObject = rootObjects[0];
			Assert.That( ( (string) rootObject["_oid"] ).StartsWith( "Mitarbeiter-F33D0A6D" ) );
			return json;
		}

		[Test]
        [TestCase(true)]
        [TestCase( false )]
        public void CanDeserializeObjectContainer(bool serializeAddress)
		{
			ObjectContainer oc = new ObjectContainer();
			string json = CreateObjectContainer(serializeAddress);
			this.pm = new PersistenceManager();
			oc.Deserialize( json, new NdoJsonFormatter(this.pm) );
			Assert.AreEqual( 1, oc.RootObjects.Count );
			Assert.That( oc.RootObjects[0] is Mitarbeiter );
			Mitarbeiter m = (Mitarbeiter)oc.RootObjects[0];
			Assert.AreEqual( "Mirko", m.Vorname );
			Assert.AreEqual( 1, m.Reisen.Count );
            if (serializeAddress)
            {
                Assert.NotNull( m.Adresse );
                Assert.AreEqual( "D", m.Adresse.Lkz );
            }
            else
            {
                Assert.IsNull( m.Adresse );
            }
		}

		[Test]
		[TestCase(typeof(BinaryFormatter))]
		[TestCase(typeof(NdoJsonFormatter))]
		public void ObjectContainerIsSerializable(Type formatterType)
		{
			IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);

			var m = CreateMitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			var oc = pm.GetObjectContainer();
			Assert.That( Object.ReferenceEquals( m, oc.RootObjects[0] ) );

			oc.Formatter = formatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			var oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 0, oc2.RootObjects.Count );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, formatter );
			pm.MergeObjectContainer( oc );

			oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 1, oc2.RootObjects.Count );

			m = (Mitarbeiter) oc2.RootObjects[0];
			Assert.AreEqual( "Mirko", m.Vorname );
			Assert.AreEqual( NDOObjectState.Persistent, m.NDOObjectState );
		}


		[Test]
		[TestCase(typeof(BinaryFormatter))]
		[TestCase(typeof(NdoJsonFormatter))]
		public void ChangeRelationWithExistingObject(Type formatterType)
		{
			// Create object and serialize it
			IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);

			var r = new Reise { Zweck = "NDO" };
			pm.MakePersistent(r);
			var l = new Land("Germany");
			pm.MakePersistent(l);
			pm.Save();
			var oc = pm.GetObjectContainer();

			oc.Formatter = formatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			// Merge object in to an OfflinePersistenceManager and change it
			OfflinePersistenceManager opm = new OfflinePersistenceManager(pm.NDOMapping.FileName);

			oc = new ObjectContainer();
			oc.Deserialize(serialized, formatter);

			opm.MergeObjectContainer(oc);

			var newOc = oc.RootObjects.Cast<IPersistenceCapable>();
			var r2 = (Reise)newOc.FirstOrDefault(pc=>pc is Reise);
			var l2 = (Land)newOc.FirstOrDefault(pc => pc is Land); ;
			Assert.AreEqual(NDOObjectState.Persistent, r2.NDOObjectState);
			Assert.AreEqual(NDOObjectState.Persistent, l2.NDOObjectState);
			r2.LandHinzufügen(l2);
			Assert.AreEqual(NDOObjectState.PersistentDirty, r2.NDOObjectState);

			// Create a ChangeSetContainer and serialize it
			var csc = opm.GetChangeSet();
			csc.Formatter = formatter;
			string serializedChanges = csc.MarshalingString;

			// Merge the changes back to pm
			csc = new ChangeSetContainer();
			csc.Deserialize(serializedChanges, formatter);
			r = (Reise)csc.ChangedObjects.Cast<IPersistenceCapable>().FirstOrDefault(pc => pc is Reise);
			pm.MergeObjectContainer(csc);
			r2 = (Reise)pm.FindObject(r.NDOObjectId);
			Assert.AreEqual(NDOObjectState.PersistentDirty, r2.NDOObjectState);
			Assert.AreEqual(1, r2.Länder.Count);

			// Save and Reload
			pm.Save();
			pm.UnloadCache();
			r = pm.Objects<Reise>().Single();
			Assert.AreEqual(1, r2.Länder.Count);
		}


		[Test]
		[TestCase(typeof(BinaryFormatter))]
		[TestCase(typeof(NdoJsonFormatter))]
		public void ChangeRelationWithNewObject(Type formatterType)
		{
			// Create object and serialize it
			IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);

			var m = CreateMitarbeiter();
			pm.MakePersistent(m);
			pm.Save();
			var oc = pm.GetObjectContainer();

			oc.Formatter = formatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			// Merge object in to an OfflinePersistenceManager and change it
			OfflinePersistenceManager opm = new OfflinePersistenceManager(pm.NDOMapping.FileName);

			oc = new ObjectContainer();
			oc.Deserialize(serialized, formatter);

			opm.MergeObjectContainer(oc);

			var m2 = (Mitarbeiter)oc.RootObjects[0];
			Assert.AreEqual(NDOObjectState.Persistent, m2.NDOObjectState);
			m2.Hinzufuegen(new Reise() { Zweck = "NDO" });			
			Assert.AreEqual(NDOObjectState.PersistentDirty, m2.NDOObjectState);

			// Create a ChangeSetContainer and serialize it
			var csc = opm.GetChangeSet();
			csc.Formatter = formatter;
			string serializedChanges = csc.MarshalingString;

			// Merge the changes back to pm
			csc = new ChangeSetContainer();
			csc.Deserialize(serializedChanges, formatter);
			m = (Mitarbeiter)csc.ChangedObjects[0];
			pm.MergeObjectContainer(csc);
			m2 = (Mitarbeiter)pm.FindObject(m.NDOObjectId);
			Assert.AreEqual(NDOObjectState.PersistentDirty, m2.NDOObjectState);
			Assert.AreEqual(1, m2.Reisen.Count);

			// Save and Reload
			pm.Save();
			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Single();
			Assert.AreEqual(1, m2.Reisen.Count);
		}

		[Test]
		[TestCase(typeof(BinaryFormatter))]
		[TestCase(typeof(NdoJsonFormatter))]
		public void CompleteTurnaroundWithChangeSetContainer(Type formatterType)
		{
			// Create object and serialize it
			IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);

			var m = CreateMitarbeiter();
			pm.MakePersistent( m );
			pm.Save();
			var oc = pm.GetObjectContainer();

			oc.Formatter = formatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			// Merge object in to an OfflinePersistenceManager and change it
			OfflinePersistenceManager opm = new OfflinePersistenceManager( pm.NDOMapping.FileName );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, formatter );

			opm.MergeObjectContainer( oc );

			var m2 = (Mitarbeiter)oc.RootObjects[0];
			Assert.AreEqual( NDOObjectState.Persistent, m2.NDOObjectState );
			m2.Vorname = "Hans";
			Assert.AreEqual( NDOObjectState.PersistentDirty, m2.NDOObjectState );

			// Create a ChangeSetContainer and serialize it
			var csc = opm.GetChangeSet();
			csc.Formatter = formatter;
			string serializedChanges = csc.MarshalingString;

			// Merge the changes back to pm
			csc = new ChangeSetContainer();
			csc.Deserialize( serializedChanges, formatter );
			m = (Mitarbeiter) csc.ChangedObjects[0];
			pm.MergeObjectContainer( csc );
			m2 = (Mitarbeiter) pm.FindObject( m.NDOObjectId );
			Assert.AreEqual( NDOObjectState.PersistentDirty, m2.NDOObjectState );
			Assert.AreEqual( "Hans", m.Vorname );

			// Save and Reload
			pm.Save();
			pm.UnloadCache();
			m = pm.Objects<Mitarbeiter>().Single();
			Assert.AreEqual( "Hans", m.Vorname );
		}

		[Test]
		[TestCase(typeof(BinaryFormatter))]
		[TestCase(typeof(NdoJsonFormatter))]
		public void CompleteTurnaroundWithAddedObject(Type formatterType)
		{
			// Create object and serialize it
			IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);

			var m = CreateMitarbeiter();
			pm.MakePersistent(m);
			pm.Save();
			var oc = pm.GetObjectContainer();

			oc.Formatter = formatter;
			string serialized = oc.MarshalingString;

			pm.UnloadCache();

			// Merge object into an OfflinePersistenceManager and change it
			OfflinePersistenceManager opm = new OfflinePersistenceManager(pm.NDOMapping.FileName);

			oc = new ObjectContainer();
			oc.Deserialize(serialized, formatter);

			opm.MergeObjectContainer(oc);

			var m2 = new Mitarbeiter() { Vorname = "Hans", Nachname = "Müller" };
			opm.MakePersistent(m2);

			// Create a ChangeSetContainer and serialize it
			var csc = opm.GetChangeSet();
			csc.Formatter = formatter;
			string serializedChanges = csc.MarshalingString;

			// Merge the changes back to pm
			csc = new ChangeSetContainer();
			csc.Deserialize(serializedChanges, formatter);

			Assert.That(csc.AddedObjects.Count == 1);

			pm = new PersistenceManager();  // we need a new pm here which get's us id's beginning from -1
			pm.MergeObjectContainer(csc);
			// Now we should have a created object in the cache
			Assert.AreEqual(true, pm.HasChanges);
			m = (Mitarbeiter)pm.FindObject(typeof(Mitarbeiter), -1);
			Assert.That(m.NDOObjectState == NDOObjectState.Created);

			// Save and Reload
			pm.Save();
			Assert.That(m.NDOObjectState == NDOObjectState.Persistent);
			pm.UnloadCache();
			var l = pm.Objects<Mitarbeiter>().ResultTable;
			Assert.AreEqual(2, l.Count);
			Assert.That(l.Any(m1 => m1.Vorname == "Mirko" && m1.Nachname == "Matytschak"));
			Assert.That(l.Any(m1 => m1.Vorname == "Hans" && m1.Nachname == "Müller"));
		}

		[Test]
		public void CanSetObjectsToHollow()
		{
			var formatter = new NdoJsonFormatter();
			var serializationIterator = new NDO.SerializationIterator( r => r.ReferencedType == typeof( Reise ), pc => pc.NDOObjectState = NDOObjectState.Hollow );

			var m = CreateMitarbeiter();
			pm.MakePersistent( m );
			Reise reise;
			m.Hinzufuegen( reise = new Reise() { Zweck = "NDO" } );
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

			oc.Formatter = formatter;
			string serialized = oc.MarshalingString;

			Assert.AreEqual( 2, oc.RootObjects.Count );
			Assert.AreEqual( NDOObjectState.Hollow, m.NDOObjectState );
			Assert.AreEqual( NDOObjectState.Hollow, reise.NDOObjectState );
		}

		[Test]
		[TestCase(typeof(BinaryFormatter))]
		[TestCase(typeof(NdoJsonFormatter))]
		public void ObjectContainerSerializesRelations(Type formatterType)
		{
			IFormatter formatter = (IFormatter)Activator.CreateInstance(formatterType);
			var serializationIterator = new NDO.SerializationIterator( r => r.ReferencedType == typeof( Reise ) );

			var m = CreateMitarbeiter();
			pm.MakePersistent( m );
			m.Hinzufuegen( new Reise() { Zweck = "NDO" } );
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
			oc.Formatter = formatter;
			string serialized = oc.MarshalingString;

			Assert.AreEqual( 2, oc.RootObjects.Count );

			pm.UnloadCache();

			var oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 0, oc2.RootObjects.Count );

			oc = new ObjectContainer();
			oc.Deserialize( serialized, formatter );
			pm.MergeObjectContainer( oc );

			oc2 = pm.GetObjectContainer();
			Assert.AreEqual( 2, oc2.RootObjects.Count );

			Reise r2 = null;
			Mitarbeiter m2 = null;

			foreach (object o in oc2.RootObjects)
			{
				if (o is Reise)
					r2 = (Reise) o;
				if (o is Mitarbeiter)
					m2 = (Mitarbeiter) o;
			}

			Assert.NotNull( r2 );
			Assert.NotNull( m2 );

			Assert.AreEqual( "Mirko", m2.Vorname );
			Assert.AreEqual( NDOObjectState.Persistent, m2.NDOObjectState );

			Assert.AreEqual( "NDO", r2.Zweck );
			Assert.AreEqual( NDOObjectState.Persistent, r2.NDOObjectState );
		}
		[Test]
		public void TestJsonNull()
		{
			var formatter = new NdoJsonFormatter(this.pm);
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write("null");
			ms.Position = 0;

			object obj = formatter.Deserialize(ms);
			Assert.IsNull(obj);
		}

	}
}
