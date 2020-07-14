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
