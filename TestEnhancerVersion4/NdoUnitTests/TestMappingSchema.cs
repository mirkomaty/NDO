//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.IO;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
#if maskedOut
namespace NdoUnitTests
{
	internal static class RelationExtension
	{
		static public string GetRelationType(this Relation r)
		{
			string result;
			if ( r.Multiplicity == RelationMultiplicity.Element )
				result = "1";
			else
				result = "n";
			if ( r.ForeignRelation == null )
				return result;
			result += ':';
			if ( r.ForeignRelation.Multiplicity == RelationMultiplicity.Element )
				result += '1';
			else
				result += 'n';

			return result;
		}
	}

	/// <summary>
	/// Summary description for TestMappingSchema.
	/// </summary>
	[TestFixture]
	public class TestMappingSchema
	{
		string classMitarbeiter = "Reisekosten.Personal.Mitarbeiter";
		string assName = "MyAssembly";
		string testFile = "Test.Xml";

		[SetUp]
		public void Setup() 
		{
		}

		[TearDown]
		public void TearDown() 
		{
			if (File.Exists(testFile))
				File.Delete(testFile);
		}

		[Test]
		public void ListConnection()
		{
			PersistenceManager pm = PmFactory.NewPersistenceManager();
			Connection conn = (Connection) pm.NDOMapping.Connections[0];
			Console.WriteLine(String.Format("\r\nConnection '{0}', Type {1}\r\n", conn.Name, conn.Type));
		}

		[Test]
		public void AnlegenKlasse()
		{
			NDOMapping mapping = NDOMapping.Create(testFile);
			mapping.AddStandardClass(classMitarbeiter, assName, null);
			mapping.Save();
			mapping = new NDOMapping(testFile);

			Assert.AreEqual(1, mapping.Connections.Count, "Keine Connection");
			Assert.AreEqual(1, mapping.Classes.Count, "Keine Klasse");

			Class c = (Class) mapping.Classes[0];

			Assert.AreEqual("Mitarbeiter", c.TableName, "TableName falsch");
			Assert.NotNull(c.Oid, "Keine Oid");
			Assert.AreEqual("C0", mapping.FindConnection(c).ID, "Connection C0 nicht gefunden");

			c = mapping.FindClass(classMitarbeiter);

			Assert.NotNull(c, "FindClass fehlgeschlagen");

			File.Delete(testFile);
		}

		[Test]
		public void GleicherKlassenName()
		{
			string testFile = "Text.Xml";
			string newclassMitarbeiter = classMitarbeiter.Replace("Personal", "Test");
			NDOMapping mapping = NDOMapping.Create(testFile);
			mapping.AddStandardClass(classMitarbeiter, assName, null);
            mapping.AddStandardClass(newclassMitarbeiter, assName, null);
			mapping.Save();
			mapping = new NDOMapping(testFile);

			Class c = mapping.FindClass(classMitarbeiter);
			Assert.NotNull(c, classMitarbeiter + " nicht gefunden");

			Class c2 = mapping.FindClass(newclassMitarbeiter);
			Assert.NotNull(c, newclassMitarbeiter + " nicht gefunden");

			Assert.That(c.TableName != c2.TableName, "TableNames m�ssen ungleich sein");
		}

		[Test]
		public void AnlegenField()
		{
			string testFile = "Test.Xml";
			NDOMapping mapping = NDOMapping.Create(testFile);
            Class c = mapping.AddStandardClass(classMitarbeiter, assName, null);
			c.AddStandardField("vorname", false);
			mapping.Save();
			mapping = new NDOMapping(testFile);

			Assert.AreEqual(1, mapping.Connections.Count, "Keine Connection");
			Assert.AreEqual(1, mapping.Classes.Count, "Keine Klasse");

			c = (Class) mapping.Classes[0];

			Assert.AreEqual("Mitarbeiter", c.TableName, "TableName falsch");
			Assert.NotNull(c.Oid, "Keine Oid");
			Assert.AreEqual(1, c.Fields.Count, "Kein Field");

			Field f = (Field) c.Fields[0];

			Assert.AreEqual("Vorname", f.Column.Name, "ColumnName falsch");
		}


		[Test]
		public void Anlegen1NRelation()
		{
			string testFile = "Test.Xml";
			NDOMapping mapping = NDOMapping.Create(testFile);
            Class c = mapping.AddStandardClass(classMitarbeiter, assName, null);
			c.AddStandardRelation("dieReisen", "Reise", false,"", false, false);
			mapping.Save();
			mapping = new NDOMapping(testFile);
			Assert.AreEqual(1, mapping.Connections.Count, "Keine Connection");
			Assert.AreEqual(1, mapping.Classes.Count, "Keine Klasse");
			c = (Class) mapping.Classes[0];
			Assert.AreEqual("Mitarbeiter", c.TableName, "TableName falsch");
			Assert.NotNull(c.Oid, "Keine Oid");
			Assert.AreEqual(1, c.Relations.Count, "Keine Relation");
			Relation r = (Relation) c.Relations[0];
			Assert.AreEqual("dieReisen", r.FieldName, "FieldName falsch");
			Assert.AreEqual("n", r.GetRelationType(), "Relationstyp falsch");
			Assert.AreEqual("IDMitarbeiter", ((ForeignKeyColumn)r.ForeignKeyColumns[0]).Name, "ForeignKeyColumnName falsch");
		}


		[Test]
		public void Anlegen1to1Relation()
		{
			string testFile = "Test.Xml";
			NDOMapping mapping = NDOMapping.Create(testFile);
            Class c = mapping.AddStandardClass(classMitarbeiter, assName, null);
			c.AddStandardRelation("dieReise", "Reise", true,"", false, false);
			mapping.Save();
			mapping = new NDOMapping(testFile);
			Assert.AreEqual(1, mapping.Connections.Count, "Keine Connection");
			Assert.AreEqual(1, mapping.Classes.Count, "Keine Klasse");
			c = (Class) mapping.Classes[0];
			Assert.AreEqual("Mitarbeiter", c.TableName, "TableName falsch");
			Assert.NotNull(c.Oid, "Keine Oid");
			Assert.AreEqual(1, c.Relations.Count, "Keine Relation");
			Relation r = (Relation) c.Relations[0];
			Assert.AreEqual("dieReise", r.FieldName, "FieldName falsch");
			Assert.AreEqual("1:1", r.GetRelationType(), "Relationstyp falsch");
			Assert.AreEqual("IDReise", ((ForeignKeyColumn)r.ForeignKeyColumns[0]).Name, "ForeignKeyColumnName falsch");
		}


		[Test]
		public void AnlegenNNRelation()
		{
			string testFile = "Test.Xml";
			NDOMapping mapping = NDOMapping.Create(testFile);
            Class c = mapping.AddStandardClass(classMitarbeiter, assName, null);
			c.AddStandardRelation("dieReisen", "Reisekosten.Reise", false,"", false, false);
            c = mapping.AddStandardClass("Reisekosten.Reise", assName, null);
			c.AddStandardRelation("dieMitarbeiter", classMitarbeiter, false,"", false, false);
			mapping.Save();
			mapping = new NDOMapping(testFile);
			Assert.AreEqual(1, mapping.Connections.Count, "Keine Connection");
			Assert.AreEqual(2, mapping.Classes.Count, "Keine Klasse");
			c = (Class) mapping.FindClass(classMitarbeiter);
			Assert.AreEqual("Mitarbeiter", c.TableName, "TableName falsch");
			Assert.NotNull(c.Oid, "Keine Oid");
			Assert.AreEqual(1, c.Relations.Count, "Keine Relation");
			Relation r = c.FindRelation("dieReisen");
			Assert.NotNull(r, "Relation dieReisen nicht gefunden");
			Assert.AreEqual("IDMitarbeiter", ((ForeignKeyColumn)r.ForeignKeyColumns[0]).Name, "ForeignKeyColumnName falsch");
			Assert.AreEqual( "n:n", r.GetRelationType(), "Relationstyp falsch" );
			Assert.AreEqual("IDReise", ((ForeignKeyColumn)r.MappingTable.ChildForeignKeyColumns[0]).Name, "ChildForeignKeyColumnName von MappingTable falsch");
			Assert.AreEqual("relMitarbeiterReise", r.MappingTable.TableName, "TableName von MappingTable falsch");
			c = mapping.FindClass("Reisekosten.Reise");
			r = c.FindRelation("dieMitarbeiter");
			Assert.NotNull(r, "Relation dieMitarbeiter nicht gefunden");
			Assert.AreEqual("relMitarbeiterReise", r.MappingTable.TableName, "TableName von MappingTable falsch");
			Assert.AreEqual( "n:n", r.GetRelationType(), "Relationstyp falsch" );
			Assert.AreEqual("IDReise", ((ForeignKeyColumn)r.ForeignKeyColumns[0]).Name, "ForeignKeyColumnName falsch");
			Assert.AreEqual("IDMitarbeiter", ((ForeignKeyColumn)r.MappingTable.ChildForeignKeyColumns[0]).Name, "ChildForeignKeyColumnName von MappingTable falsch");

		}


		[Test]
		public void MergeEqual()
		{
			string file1 = "File1.Xml";
			string file2 = "File2.Xml";

			NDOMapping map1 = NDOMapping.Create(file1);
            map1.AddStandardClass(classMitarbeiter, assName, null);
			map1.Save();

			NDOMapping map2 = NDOMapping.Create(file2);
            map2.AddStandardClass(classMitarbeiter, assName, null);
			map2.Save();

			map1 = new NDOMapping(file1);
			map2 = new NDOMapping(file2);

			map1.MergeMapping(map2);
			Assert.AreEqual(1, map1.Classes.Count, "Falsche Anzahl Klassen");
			Assert.AreEqual(1, map1.Connections.Count, "Falsche Anzahl Connections");
			
			File.Delete(file1);
			File.Delete(file2);
		}

		[Test]
		public void MergeEqualNewConnName()
		{
			string file1 = "File1.Xml";
			string file2 = "File2.Xml";

			NDOMapping map1 = NDOMapping.Create(file1);
            map1.AddStandardClass(classMitarbeiter, assName, null);

			map1.Save();

			NDOMapping map2 = NDOMapping.Create(file2);
            map2.AddStandardClass(classMitarbeiter, assName, null);
			((Connection) map2.Connections[0]).ID = "C1";
			map2.Save();

			map1 = new NDOMapping(file1);
			map2 = new NDOMapping(file2);

			map1.MergeMapping(map2);
			Assert.AreEqual(1, map1.Classes.Count, "Falsche Anzahl Klassen");
			Assert.AreEqual(1, map1.Connections.Count, "Falsche Anzahl Connections");
			
			File.Delete(file1);
			File.Delete(file2);
		}

		[Test]
		public void MergeNewConnString()
		{
			string file1 = "File1.Xml";
			string file2 = "File2.Xml";

			NDOMapping map1 = NDOMapping.Create(file1);
            map1.AddStandardClass(classMitarbeiter, assName, null);
			((Connection) map1.Connections[0]).Name = "Was anderes als Dummy";
			map1.Save();

			NDOMapping map2 = NDOMapping.Create(file2);
            map2.AddStandardClass(classMitarbeiter, assName, null);
			((Connection) map2.Connections[0]).Name = "Neuer ConnectionString";
			map2.Save();

			map1 = new NDOMapping(file1);
			map2 = new NDOMapping(file2);

			map1.MergeMapping(map2);
			Assert.AreEqual(2, map1.Connections.Count, "Falsche Anzahl Connections");
			Assert.AreEqual(1, map1.Classes.Count, "Falsche Anzahl Klassen");
			Class c = map1.FindClass(classMitarbeiter);
			Assert.NotNull(c, "Mitarbeiter nicht gefunden");
			Assert.AreEqual("C0", c.ConnectionId, "Connection falsch");

			File.Delete(file1);
			File.Delete(file2);
		}


		[Test]
		public void MergeNewConnStringFromDummy1()
		{
			string file1 = "File1.Xml";
			string file2 = "File2.Xml";

			NDOMapping map1 = NDOMapping.Create(file1);
            map1.AddStandardClass(classMitarbeiter, assName, null);
			map1.Save();

			NDOMapping map2 = NDOMapping.Create(file2);
            map2.AddStandardClass(classMitarbeiter, assName, null);
			map2.Save();

			map1 = new NDOMapping(file1);
			map2 = new NDOMapping(file2);

			Assert.AreEqual(Connection.DummyConnectionString, ((Connection)map1.Connections[0]).Name, "Must be dummy connection");
			Assert.AreEqual(Connection.DummyConnectionString, ((Connection)map2.Connections[0]).Name, "Must be dummy connection");
			((Connection)map2.Connections[0]).Name = "Some new string";

			map1.MergeMapping(map2);
			Assert.AreEqual(1, map1.Connections.Count, "Falsche Anzahl Connections");
			Class c = map1.FindClass(classMitarbeiter);
			Assert.NotNull(c, "Mitarbeiter nicht gefunden");
			Assert.AreEqual("C0", c.ConnectionId, "Connection falsch");

			File.Delete(file1);
			File.Delete(file2);
		}



		[Test]
		public void MergeNewConnStringFromDummy2()
		{
			string file1 = "File1.Xml";
			string file2 = "File2.Xml";

			NDOMapping map1 = NDOMapping.Create(file1);
            map1.AddStandardClass(classMitarbeiter, assName, null);
			map1.Save();

			NDOMapping map2 = NDOMapping.Create(file2);
            map2.AddStandardClass(classMitarbeiter, assName, null);
			map2.Save();

			map1 = new NDOMapping(file1);
			map2 = new NDOMapping(file2);

			Assert.AreEqual(Connection.DummyConnectionString, ((Connection)map1.Connections[0]).Name, "Must be dummy connection");
			Assert.AreEqual(Connection.DummyConnectionString, ((Connection)map2.Connections[0]).Name, "Must be dummy connection");
			((Connection)map1.Connections[0]).Name = "Some new string";

			map1.MergeMapping(map2);
			Assert.AreEqual(1, map1.Connections.Count, "Falsche Anzahl Connections");
			Class c = map1.FindClass(classMitarbeiter);
			Assert.NotNull(c, "Mitarbeiter nicht gefunden");
			Assert.AreEqual("C0", c.ConnectionId, "Connection falsch");

			File.Delete(file1);
			File.Delete(file2);
		}



		[Test]
		public void MergeNewConnStringClassChanged()
		{
			string file1 = "File1.Xml";
			string file2 = "File2.Xml";

			NDOMapping map1 = NDOMapping.Create(file1);
            map1.AddStandardClass(classMitarbeiter, assName, null);
			((Connection) map1.Connections[0]).Name = "Alter ConnectionString";
			map1.Save();

			NDOMapping map2 = NDOMapping.Create(file2);
            map2.AddStandardClass(classMitarbeiter, assName, null);
			((Connection) map2.Connections[0]).Name = "Neuer ConnectionString";
			((Class) map2.Classes[0]).AssemblyName = "New Assembly-Name";
			map2.Save();

			map1 = new NDOMapping(file1);
			map2 = new NDOMapping(file2);

			map1.MergeMapping(map2);
			Assert.AreEqual(2, map1.Connections.Count, "Falsche Anzahl Connections");
			Assert.AreEqual(1, map1.Classes.Count, "Falsche Anzahl Klassen");
			Class c = map1.FindClass(classMitarbeiter);
			Assert.NotNull(c, "Mitarbeiter nicht gefunden");
			Assert.AreEqual("C1", c.ConnectionId, "Connection falsch");

			File.Delete(file1);
			File.Delete(file2);
		}

		[Test]
		public void MergeNewConnNewIDNewClass()
		{
			string file1 = "File1.Xml";
			string file2 = "File2.Xml";

			NDOMapping map1 = NDOMapping.Create(file1);
            map1.AddStandardClass(classMitarbeiter, assName, null);
			((Connection) map1.Connections[0]).Name = "Alter ConnectionString";
			map1.Save();

			NDOMapping map2 = NDOMapping.Create(file2);
            map2.AddStandardClass(classMitarbeiter, assName, null);
			((Connection) map2.Connections[0]).Name = "Neuer ConnectionString";
			((Class) map2.Classes[0]).FullName = "Reisekosten.Reise";
			map2.Save();

			map1 = new NDOMapping(file1);
			map2 = new NDOMapping(file2);

			map1.MergeMapping(map2);
			Assert.AreEqual(2, map1.Connections.Count, "Falsche Anzahl Connections");
			Assert.AreEqual(2, map1.Classes.Count, "Falsche Anzahl Klassen");
			Class c = map1.FindClass("Reisekosten.Reise");
			Assert.NotNull(c, "Reise nicht gefunden");
			Assert.AreEqual("C1", c.ConnectionId, "Connection falsch");

			c = map1.FindClass(classMitarbeiter);
			Assert.NotNull(c, "Mitarbeiter nicht gefunden");
			Assert.AreEqual("C0", c.ConnectionId, "Connection falsch");

			File.Delete(file1);
			File.Delete(file2);
		}



	}
}
#endif