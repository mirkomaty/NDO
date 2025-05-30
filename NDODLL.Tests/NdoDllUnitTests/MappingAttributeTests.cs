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
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var relation = cls.AddStandardRelation( "relField", "RefTypeName", false, "", false, false );
			Assert.That( "IDTestClass", Is.EqualTo( relation.ForeignKeyColumns.First().Name ) );
			var attr = new MappingTableAttribute();
			relation.RemapMappingTable( false, false, attr );
			Assert.That( relation.MappingTable != null );
			Assert.That( "relRefTypeNameTestClass", Is.EqualTo(relation.MappingTable.TableName) );
			Assert.That( "IDTestClass", Is.EqualTo( relation.ForeignKeyColumns.First().Name ) );
			Assert.That( "IDRefTypeName", Is.EqualTo( relation.MappingTable.ChildForeignKeyColumns.First().Name ) );

			attr.TableName = "newTableName";
			relation.RemapMappingTable( false, false, attr );
			Assert.That( "newTableName", Is.EqualTo(relation.MappingTable.TableName) );
		}

		[Test]
		public void RemapForeignKeyColumnsWorks()
		{
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			mapping.AddStandardClass( "RefTypeName", "TestAssembly", null );
			var relation = cls.AddStandardRelation( "relField", "RefTypeName", false, "", false, false );
			Assert.That( "IDTestClass", Is.EqualTo( relation.ForeignKeyColumns.First().Name ) );
			var attr = new ForeignKeyColumnAttribute() { Name = "newColumnName" };
			relation.RemapForeignKeyColumns( new[] { attr }, new ChildForeignKeyColumnAttribute[] { } );
			Assert.That( "newColumnName", Is.EqualTo( relation.ForeignKeyColumns.First().Name ) );
		}

		[Test]
		public void RemapOidColumnsWorks()
		{
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			Assert.That( "ID", Is.EqualTo( cls.Oid.OidColumns.First().Name ) );
			var attr = new OidColumnAttribute() { Name = "newColumnName" };
			cls.Oid.RemapOidColumns( new[] { attr } );
			Assert.That( "newColumnName", Is.EqualTo( cls.Oid.OidColumns.First().Name ) );
		}

		[Test]
		public void RemapColumnsWorks()
		{
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var field = cls.AddStandardField( "test", false );
			Assert.That( "Test", Is.EqualTo(field.Column.Name) );
			var attr = new ColumnAttribute() { Size = -1, Name = "XTest" };
			attr.RemapColumn( field.Column );
			Assert.That( "XTest", Is.EqualTo(field.Column.Name) );
			Assert.That( field.Column.Size == -1 );
		}

		[Test]
		public void RemapFieldsWorks()
		{
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var field = cls.AddStandardField( "test", false );
			Assert.That( !field.Encrypted );
			var attr = new FieldAttribute() { Encrypted = true };
			attr.RemapField( field );
			Assert.That( field.Encrypted );
			attr = new FieldAttribute() { Encrypted = false };
			attr.RemapField( field );
			// Encrypted can only be rewritten, if the value is false
			Assert.That( field.Encrypted );
		}

		[Test]
		public void SetFieldValuesWorks()
		{
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			var field = cls.AddStandardField( "test", false );
			Assert.That( !field.Encrypted );
			var attr = new FieldAttribute() { Encrypted = true };
			attr.SetFieldValues( field );
			Assert.That( field.Encrypted );
			attr = new FieldAttribute() { Encrypted = false };
			attr.SetFieldValues( field );
			// Encrypted can be set to false
			Assert.That( !field.Encrypted );
		}

		[Test]
		public void OidColumnShouldntBeRemappedWithAssembyWideColumnAttribute()
		{
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass( "TestClass", "TestAssembly", null );
			Assert.That( "ID", Is.EqualTo( cls.Oid.OidColumns.First().Name ) );
			var attr = new OidColumnAttribute() { Name = "newColumnName", IsAssemblyWideDefinition=true };
			cls.Oid.RemapOidColumns( new[] { attr } );
			Assert.That( "ID", Is.EqualTo( cls.Oid.OidColumns.First().Name ) );
		}

        [Test]
        public void AssembyWideColumnAttributeOverwritesDefaultValues()
        {
			NDOMapping mapping = new NDOMapping(null,null); //NDOMapping.Create( null );
			var cls = mapping.AddStandardClass("TestClass", "TestAssembly", null);
            Assert.That(1 == cls.Oid.OidColumns.First().AutoIncrementStart);
            var attr = new OidColumnAttribute() { Name = "newColumnName", AutoIncrementStart = 2, IsAssemblyWideDefinition = true };
            cls.Oid.RemapOidColumns(new[] { attr });
            Assert.That(2 == cls.Oid.OidColumns.First().AutoIncrementStart);
        }
    }
}
