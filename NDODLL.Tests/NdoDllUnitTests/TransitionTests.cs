using NDO;
using NDO.ProviderFactory;
using NUnit.Framework;
using System;
using System.Linq;
using System.Xml.Linq;

namespace NdoDllUnitTests
{
    [TestFixture]

    public class TransitionTests
    {
        SchemaTransitionGenerator transitionGenerator;

        [SetUp]
        public void Setup()
        {
            transitionGenerator = new SchemaTransitionGenerator( NDOProviderFactory.Instance, "SqlServer", null );
        }

        XElement GetSimpleIndex()
        {
            XElement column = new XElement( "Column", new XAttribute( "name", "IndexedColumn" ) );
            XElement createIndex = new XElement( "CreateIndex", new XAttribute( "name", "ix_MyTable" ), new XAttribute( "onTable", "MyTable" ), column );
            return createIndex;
        }

        [Test]
        public void CreateSimpleIndex()
        {
            XElement createIndex = GetSimpleIndex();
            XElement schemaTransition = new XElement( "NdoSchemaTransition", createIndex );

            var sql = this.transitionGenerator.Generate( schemaTransition );
            Assert.That( sql, Is.EqualTo( "CREATE INDEX [ix_MyTable] ON [MyTable] ([IndexedColumn])" ) );
        }

        [Test]
        public void CreateIndexWith2Columns()
        {
            XElement createIndex = GetSimpleIndex();
            XElement column = new XElement( "Column", new XAttribute( "name", "SecondColumn" ) );
            createIndex.Add( column );

            XElement schemaTransition = new XElement( "NdoSchemaTransition", createIndex );

            var sql = this.transitionGenerator.Generate( schemaTransition );
            Assert.That( sql, Is.EqualTo( "CREATE INDEX [ix_MyTable] ON [MyTable] ([IndexedColumn], [SecondColumn])" ) );
        }

        [Test]
        public void CreateUniqueIndex()
        {
            XElement createIndex = GetSimpleIndex();
            createIndex.Add( new XAttribute( "unique", "true" ) );
            XElement schemaTransition = new XElement( "NdoSchemaTransition", createIndex );

            var sql = this.transitionGenerator.Generate( schemaTransition );
            Assert.That( sql, Is.EqualTo( "CREATE UNIQUE INDEX [ix_MyTable] ON [MyTable] ([IndexedColumn])" ) );
        }

        [Test]
        public void CreateUniqueFulltextIndex()
        {
            XElement createIndex = GetSimpleIndex();
            createIndex.Add( new XAttribute( "fulltext", "true" ) );
            XElement schemaTransition = new XElement( "NdoSchemaTransition", createIndex );

            var sql = this.transitionGenerator.Generate( schemaTransition );
            Assert.That( sql, Is.EqualTo( "CREATE FULLTEXT INDEX [ix_MyTable] ON [MyTable] ([IndexedColumn])" ) );
        }

        [Test]
        public void AscendingShouldBeIgnored()
        {
            XElement createIndex = GetSimpleIndex();
            createIndex.Element( "Column" ).Add( new XAttribute( "desc", "false" ) );
            XElement schemaTransition = new XElement( "NdoSchemaTransition", createIndex );

            var sql = this.transitionGenerator.Generate( schemaTransition );
            Assert.That( sql, Is.EqualTo( "CREATE INDEX [ix_MyTable] ON [MyTable] ([IndexedColumn])" ) );
        }

        [Test]
        public void CreateDescendingColumnIndex()
        {
            XElement createIndex = GetSimpleIndex();
            createIndex.Element( "Column" ).Add( new XAttribute( "desc", "true" ) );
            XElement schemaTransition = new XElement( "NdoSchemaTransition", createIndex );

            var sql = this.transitionGenerator.Generate( schemaTransition );
            Assert.That( sql, Is.EqualTo( "CREATE INDEX [ix_MyTable] ON [MyTable] ([IndexedColumn] DESC)" ) );
        }

    }
}
