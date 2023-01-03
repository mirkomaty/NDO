using NDO.Mapping;
using NDO.Mapping.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdoDllUnitTests
{
	class MappingAttributeTests
	{
		[Test]
		public void MappingTableAttributeWorks()
		{
			NDOMapping mapping = NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var relation = cls.AddStandardRelation( "relField", "RefTypeName", false, "", false, false );
			Assert.AreEqual( "IDTestClass", relation.ForeignKeyColumns.First().Name );
			var attr = new MappingTableAttribute();
			relation.RemapMappingTable( false, false, attr );
			Assert.That( relation.MappingTable != null );
			Assert.AreEqual( "relRefTypeNameTestClass", relation.MappingTable.TableName );
			Assert.AreEqual( "IDTestClass", relation.ForeignKeyColumns.First().Name );
			Assert.AreEqual( "IDRefTypeName", relation.MappingTable.ChildForeignKeyColumns.First().Name );

			attr.TableName = "newTableName";
			relation.RemapMappingTable( false, false, attr );
			Assert.AreEqual( "newTableName", relation.MappingTable.TableName );
		}

		[Test]
		public void RemapForeignKeyColumnsWorks()
		{
			NDOMapping mapping = NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			mapping.AddStandardClass( "RefTypeName", "TestAssembly", null );
			var relation = cls.AddStandardRelation( "relField", "RefTypeName", false, "", false, false );
			Assert.AreEqual( "IDTestClass", relation.ForeignKeyColumns.First().Name );
			var attr = new ForeignKeyColumnAttribute() { Name = "newColumnName" };
			relation.RemapForeignKeyColumns( new[] { attr }, new ChildForeignKeyColumnAttribute[] { } );
			Assert.AreEqual( "newColumnName", relation.ForeignKeyColumns.First().Name );
		}

		[Test]
		public void RemapOidColumnsWorks()
		{
			NDOMapping mapping = NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			Assert.AreEqual( "ID", cls.Oid.OidColumns.First().Name );
			var attr = new OidColumnAttribute() { Name = "newColumnName" };
			cls.Oid.RemapOidColumns( new[] { attr } );
			Assert.AreEqual( "newColumnName", cls.Oid.OidColumns.First().Name );
		}

		[Test]
		public void RemapColumnsWorks()
		{
			NDOMapping mapping = NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var field = cls.AddStandardField( "test", false );
			Assert.AreEqual( "Test", field.Column.Name );
			var attr = new ColumnAttribute() { Size = -1, Name = "XTest" };
			attr.RemapColumn( field.Column );
			Assert.AreEqual( "XTest", field.Column.Name );
			Assert.AreEqual( -1, field.Column.Size );
		}

		[Test]
		public void RemapFieldsWorks()
		{
			NDOMapping mapping = NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var field = cls.AddStandardField( "test", false );
			Assert.AreEqual( false, field.Encrypted );
			var attr = new FieldAttribute() { Encrypted = true };
			attr.RemapField( field );
			Assert.AreEqual( true, field.Encrypted );
			attr = new FieldAttribute() { Encrypted = false };
			attr.RemapField( field );
			// Encrypted can only be rewritten, if the value is false
			Assert.AreEqual( true, field.Encrypted );
		}

		[Test]
		public void SetFieldValuesWorks()
		{
			NDOMapping mapping = NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var field = cls.AddStandardField( "test", false );
			Assert.AreEqual( false, field.Encrypted );
			var attr = new FieldAttribute() { Encrypted = true };
			attr.SetFieldValues( field );
			Assert.AreEqual( true, field.Encrypted );
			attr = new FieldAttribute() { Encrypted = false };
			attr.SetFieldValues( field );
			// Encrypted can be set to false
			Assert.AreEqual( false, field.Encrypted );
		}

		[Test]
		public void OidColumnShouldntBeRemappedWithAssembyWideColumnAttribute()
		{
			NDOMapping mapping = NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			Assert.AreEqual( "ID", cls.Oid.OidColumns.First().Name );
			var attr = new OidColumnAttribute() { Name = "newColumnName", IsAssemblyWideDefinition=true };
			cls.Oid.RemapOidColumns( new[] { attr } );
			Assert.AreEqual( "ID", cls.Oid.OidColumns.First().Name );
		}

        [Test]
        public void AssembyWideColumnAttributeOverwritesDefaultValues()
        {
            NDOMapping mapping = NDOMapping.Create(null);
            var cls = mapping.AddStandardClass("TestClass", "TestAssembly", null);
            Assert.AreEqual(1, cls.Oid.OidColumns.First().AutoIncrementStart);
            var attr = new OidColumnAttribute() { Name = "newColumnName", AutoIncrementStart = 2, IsAssemblyWideDefinition = true };
            cls.Oid.RemapOidColumns(new[] { attr });
            Assert.AreEqual(2, cls.Oid.OidColumns.First().AutoIncrementStart);
        }
    }
}
