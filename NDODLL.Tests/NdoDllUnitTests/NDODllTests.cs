using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NDO;
using NDO.Mapping;
using Moq;

namespace NdoDllUnitTests
{
	[TestFixture]
    public class UpdateRankTests
    {
		Dictionary<string, Mock<Type>> types = new Dictionary<string,Mock<Type>>();

		[Test]
		public void TestRank()
		{
			Type t1;
			Type t2;
			List<Class> classes = new List<Class>();


			Class cls1 = new Class(null);
			cls1.FullName = "ShopFachklassen.AllgemeinZugabeGutschein";			
			t1 = cls1.SystemType = CreateType( cls1.FullName ).Object;
			var oid = new ClassOid(cls1);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls1.Oid = oid;			

			Mock<Relation> rel1Mock = new Mock<Relation>(cls1);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( true );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls1.Relations;
			relations.Add( relation );
			relation.FieldName = "code";
			relation.ReferencedTypeName = "ShopFachklassen.GutscheinCode";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls1 );

			Class cls2 = new Class(null);
			cls2.FullName = "ShopFachklassen.GutscheinCode";
			t2 = cls2.SystemType = CreateType( cls2.FullName ).Object;
			cls2.Oid = oid;

			Mock<Relation>rel2Mock = new Mock<Relation>(cls1);
			rel2Mock.Setup( t => t.Bidirectional ).Returns( true );
			relation = rel2Mock.Object;
			relations = (List<Relation>)cls2.Relations;
			relations.Add( relation );
			relation.FieldName = "gutschein";
			relation.ReferencedTypeName = "ShopFachklassen.Gutschein";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = false;
			relation.HasSubclasses = false;

			rel1Mock.Setup( r => r.ForeignRelation ).Returns( rel2Mock.Object );
			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2 } );
			rel2Mock.Setup( r => r.ForeignRelation ).Returns( rel1Mock.Object );
			rel2Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls1 } );

			classes.Add( cls2 );
			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[t2] == 0 );
			Assert.That( updateRank[t1] == 1 );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[t2] == 0 );
			Assert.That( updateRank[t1] == 1 );
		}

        [Test]
        public void TestHttpUtil()
        {
            string s = "abc/d \\ üÜöÖäÄß";
            string encoded = NDO.HttpUtil.HttpUtility.UrlEncode( s );
            Assert.That( s == NDO.HttpUtil.HttpUtility.UrlDecode( encoded ) );
        }

		[Test]
		public void TestRanknDirNoPoly()
		{
			Type tMitarbeiter;
			Type tReise;
			List<Class> classes = new List<Class>();


			Class cls = new Class(null);
			cls.FullName = "Mitarbeiter";
			tMitarbeiter = cls.SystemType = CreateType( cls.FullName ).Object;
			var oid = new ClassOid(cls);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls.Oid = oid;

			Mock<Relation> rel1Mock = new Mock<Relation>(cls);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( false );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls.Relations;
			relations.Add( relation );
			relation.FieldName = "reisen";
			relation.ReferencedTypeName = "Reise";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.List;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls );

			cls = new Class( null );
			cls.FullName = "Reise";
			tReise = cls.SystemType = CreateType( cls.FullName ).Object;
			oid = new ClassOid( cls );
			cls.Oid = oid;
			classes.Add( cls );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { classes[1] } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( (Relation)null );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tReise] == 1 );
			Assert.That( updateRank[tMitarbeiter] == 0 );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tReise] == 1 );
			Assert.That( updateRank[tMitarbeiter] == 0 );
		}


		[Test]
		public void TestRank1nNoPoly()
		{
			Type tMitarbeiter;
			Type tReise;
			List<Class> classes = new List<Class>();

			Class cls1 = new Class(null);
			cls1.FullName = "Mitarbeiter";
			tMitarbeiter = cls1.SystemType = CreateType( cls1.FullName ).Object;
			var oid = new ClassOid(null);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls1.Oid = oid;

			Mock<Relation> rel1Mock = new Mock<Relation>(cls1);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( true );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls1.Relations;
			relations.Add( relation );
			relation.FieldName = "reisen";
			relation.ReferencedTypeName = "Reise";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.List;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls1 );

			Class cls2 = new Class( null );
			cls2.FullName = "Reise";
			tReise = cls2.SystemType = CreateType( cls2.FullName ).Object;
			oid = new ClassOid( cls2 );
			cls2.Oid = oid;

			Mock<Relation> rel2Mock = new Mock<Relation>(cls2);
			rel2Mock.Setup( t => t.Bidirectional ).Returns( true );
			relation = rel2Mock.Object;
			relations = (List<Relation>)cls2.Relations;
			relations.Add( relation );
			relation.FieldName = "mitarbeiter";
			relation.ReferencedTypeName = "Mitarbeiter";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = false;
			relation.HasSubclasses = false;

			classes.Add( cls2 );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2 } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( rel2Mock.Object );
			rel2Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls1 } );
			rel2Mock.Setup( r => r.ForeignRelation ).Returns( rel1Mock.Object );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tReise] == 1 );
			Assert.That( updateRank[tMitarbeiter] == 0 );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tReise] == 1 );
			Assert.That( updateRank[tMitarbeiter] == 0 );
		}

		[Test]
		public void TestRank1nNotAutoincremented()
		{
			Type tMitarbeiter;
			Type tReise;
			List<Class> classes = new List<Class>();

			Class cls1 = new Class(null);
			cls1.FullName = "Mitarbeiter";
			tMitarbeiter = cls1.SystemType = CreateType( cls1.FullName ).Object;
			var oid = new ClassOid(cls1);
			// Mitarbeiter ist nicht autoincremented
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = false } );
			cls1.Oid = oid;

			var oid2 = new ClassOid(null);
			oid2.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );

			Mock<Relation> rel1Mock = new Mock<Relation>(cls1);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( true );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls1.Relations;
			relations.Add( relation );
			relation.FieldName = "reisen";
			relation.ReferencedTypeName = "Reise";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.List;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls1 );

			Class cls2 = new Class( null );
			cls2.FullName = "Reise";
			tReise = cls2.SystemType = CreateType( cls2.FullName ).Object;			
			cls2.Oid = oid2;

			Mock<Relation> rel2Mock = new Mock<Relation>(cls2);
			rel2Mock.Setup( t => t.Bidirectional ).Returns( true );
			relation = rel2Mock.Object;
			relations = (List<Relation>) cls2.Relations;
			relations.Add( relation );
			relation.FieldName = "mitarbeiter";
			relation.ReferencedTypeName = "Mitarbeiter";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = false;
			relation.HasSubclasses = false;

			classes.Add( cls2 );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2 } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( rel2Mock.Object );
			rel2Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls1 } );
			rel2Mock.Setup( r => r.ForeignRelation ).Returns( rel1Mock.Object );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tReise] == 0 );
			Assert.That( updateRank[tMitarbeiter] == 1 );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tReise] == 0 );
			Assert.That( updateRank[tMitarbeiter] == 1 );
		}

		[Test]
		public void TestRank1nRightPoly()
		{
			Type tMitarbeiter;
			Type tReise;
			List<Class> classes = new List<Class>();

			Class cls1 = new Class(null);
			cls1.FullName = "Mitarbeiter";
			tMitarbeiter = cls1.SystemType = CreateType( cls1.FullName ).Object;
			var oid = new ClassOid(cls1);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls1.Oid = oid;

			Mock<Relation> rel1Mock = new Mock<Relation>(cls1);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( true );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls1.Relations;
			relations.Add( relation );
			relation.FieldName = "reisen";
			relation.ReferencedTypeName = "ReiseBase";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.List;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls1 );

			Class cls2 = new Class( null );
			cls2.FullName = "Reise";
			tReise = cls2.SystemType = CreateType( cls2.FullName ).Object;
			oid = new ClassOid( cls2 );
			cls2.Oid = oid;

			Class cls2Base = new Class(null);
			cls2Base.FullName = "ReiseBase";
			cls2Base.SystemType = CreateType( cls2Base.FullName ).Object;
			cls2Base.Oid = oid;
			classes.Add( cls2Base );

			Mock<Relation> rel2Mock = new Mock<Relation>(cls2);
			rel2Mock.Setup( t => t.Bidirectional ).Returns( true );
			relation = rel2Mock.Object;
			relations = (List<Relation>) cls2.Relations;
			relations.Add( relation );
			relation.FieldName = "mitarbeiter";
			relation.ReferencedTypeName = "Mitarbeiter";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = false;
			relation.HasSubclasses = false;

			classes.Add( cls2 );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2, cls2Base } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( rel2Mock.Object );
			rel2Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls1 } );
			rel2Mock.Setup( r => r.ForeignRelation ).Returns( rel1Mock.Object );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			int maRank = updateRank[tMitarbeiter];
			Assert.That( updateRank[tReise] > maRank );
			Assert.That( updateRank[cls2Base.SystemType] > maRank );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			maRank = updateRank[tMitarbeiter];
			Assert.That( updateRank[tReise] > maRank );
			Assert.That( updateRank[cls2Base.SystemType] > maRank );
		}

		[Test]
		public void TestRank1DirNoPoly()
		{
			Type tMitarbeiter;
			Type tAdresse;
			List<Class> classes = new List<Class>();

			Class cls1 = new Class(null);
			cls1.FullName = "Mitarbeiter";
			tMitarbeiter = cls1.SystemType = CreateType( cls1.FullName ).Object;
			var oid = new ClassOid(cls1);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls1.Oid = oid;

			Mock<Relation> rel1Mock = new Mock<Relation>(cls1);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( false );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls1.Relations;
			relations.Add( relation );
			relation.FieldName = "adresse";
			relation.ReferencedTypeName = "Adresse";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls1 );

			Class cls2 = new Class( null );
			cls2.FullName = "Adresse";
			tAdresse = cls2.SystemType = CreateType( cls2.FullName ).Object;			
			cls2.Oid = oid;  // We can use the same oid, since the code asks only for autoincremented columns
			classes.Add( cls2 );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2 } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( (Relation) null );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tMitarbeiter] == 1 );
			Assert.That( updateRank[tAdresse] == 0 );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tMitarbeiter] == 1 );
			Assert.That( updateRank[tAdresse] == 0 );
		}

		[Test]
		public void TestRank1DirNotAutoincremented()
		{
			Type tMitarbeiter;
			Type tAdresse;
			List<Class> classes = new List<Class>();

			Class cls1 = new Class(null);
			cls1.FullName = "Mitarbeiter";
			tMitarbeiter = cls1.SystemType = CreateType( cls1.FullName ).Object;
			var oid = new ClassOid(cls1);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls1.Oid = oid;

			Mock<Relation> rel1Mock = new Mock<Relation>(cls1);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( false );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls1.Relations;
			relations.Add( relation );
			relation.FieldName = "adresse";
			relation.ReferencedTypeName = "Adresse";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls1 );

			Class cls2 = new Class( null );
			cls2.FullName = "Adresse";
			tAdresse = cls2.SystemType = CreateType( cls2.FullName ).Object;
			var oid2 = new ClassOid(cls2);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = false } );
			cls2.Oid = oid2; 
			classes.Add( cls2 );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2 } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( (Relation) null );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tMitarbeiter] == 0 );
			Assert.That( updateRank[tAdresse] == 1 );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[tMitarbeiter] == 0 );
			Assert.That( updateRank[tAdresse] == 1 );
		}

		[Test]
		public void TestRank1DirRightPoly()
		{
			Type tMitarbeiter;
			Type tAdresse;
			List<Class> classes = new List<Class>();

			Class cls1 = new Class(null);
			cls1.FullName = "Mitarbeiter";
			tMitarbeiter = cls1.SystemType = CreateType( cls1.FullName ).Object;
			var oid = new ClassOid(cls1);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls1.Oid = oid;

			Mock<Relation> rel1Mock = new Mock<Relation>(cls1);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( false );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls1.Relations;
			relations.Add( relation );
			relation.FieldName = "adresse";
			relation.ReferencedTypeName = "AdresseBase";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.Element;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls1 );

			Class cls2Base = new Class(null);
			cls2Base.FullName = "AdresseBase";
			cls2Base.SystemType = CreateType( cls2Base.FullName ).Object;
			cls2Base.Oid = oid;
			classes.Add( cls2Base );

			Class cls2 = new Class( null );
			cls2.FullName = "Adresse";
			tAdresse = cls2.SystemType = CreateType( cls2.FullName ).Object;
			cls2.Oid = oid;  // We can use the same oid, since the code asks only for autoincremented columns
			classes.Add( cls2 );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2, cls2Base } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( (Relation) null );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			int maRank = updateRank[tMitarbeiter];
			Assert.That( updateRank[cls2Base.SystemType] < maRank );
			Assert.That( updateRank[tAdresse] < maRank );
			classes.Reverse();
			classRank = new ClassRank();
			updateRank = classRank.BuildUpdateRank( classes );
			maRank = updateRank[tMitarbeiter];
			Assert.That( updateRank[cls2Base.SystemType] < maRank );
			Assert.That( updateRank[tAdresse] < maRank );
		}

		[Test]
		public void RankDictionaryWorksWithTheGivenMocks()
		{
			Type t1;
			Type t2;
			List<Class> classes = new List<Class>();

			// Create two classes with an arbitrary relation
			// Class 1
			Class cls = new Class(null);
			cls.FullName = "Mitarbeiter";
			t1 = cls.SystemType = CreateType( cls.FullName ).Object;
			var oid = new ClassOid(cls);
			oid.OidColumns.Add( new OidColumn( oid ) { AutoIncremented = true } );
			cls.Oid = oid;

			Mock<Relation> rel1Mock = new Mock<Relation>(cls);
			rel1Mock.Setup( t => t.Bidirectional ).Returns( false );
			Relation relation = rel1Mock.Object;
			List<Relation> relations = (List<Relation>)cls.Relations;
			relations.Add( relation );
			relation.FieldName = "reisen";
			relation.ReferencedTypeName = "Reise";
			relation.RelationName = string.Empty;
			relation.Multiplicity = RelationMultiplicity.List;
			relation.Composition = true;
			relation.HasSubclasses = false;

			classes.Add( cls );

			// Class 2
			cls = new Class( null );
			cls.FullName = "Reise";
			t2 = cls.SystemType = CreateType( cls.FullName ).Object;
			cls.Oid = oid;

			rel1Mock.Setup( r => r.ForeignRelation ).Returns( (Relation) null );

			classes.Add( cls );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );

			// Make sure, that the types returned by the list
			// are equal to the types we assigned to the Class objects
			var list = updateRank.Keys.ToList();
			var tx = list[0];
			var ty = list[1];
			Assert.That( tx.GetHashCode() != ty.GetHashCode() );
			Assert.That( !tx.Equals( ty ) );
			Assert.That( tx.Equals(t1) && ty.Equals(t2) || tx.Equals( t2 ) && ty.Equals( t1 ) );
			Assert.That( !tx.Equals( t1 ) && !ty.Equals( t2 ) || !tx.Equals( t2 ) && !ty.Equals( t1 ) );

			// Make sure, that indexing the updateRank dictionary works
			int rank = updateRank[t1];				// Will throw an exception, if indexing doesn't work
			Assert.That( rank == 0 || rank == 1 );  // Expected result with two classes...
		}

		public Mock<Type> CreateType( string typeName )
		{
			if (types.ContainsKey( typeName ))
				return types[typeName];
			var type = MockType.CreateType( typeName );
			types.Add( typeName, type );
			return type;
		}

		[Test]
		public void TestRelationEquality()
		{

		}

		[Test]
		public void TypeMocksWorkAsExpected()
		{
			Mock<Type> type = CreateType( "ShopFachklassen.AllgemeinZugabeGutschein" );
			Assert.That( type.Object.FullName == "ShopFachklassen.AllgemeinZugabeGutschein" );
			Assert.That( type.Object.Name == "AllgemeinZugabeGutschein" );
			Mock<Type> type2 = CreateType( type.Object.FullName );
			Assert.That( type.Object.Equals( type2.Object ) );
			// This expression is used by the .NET Framework's implementation of Equals.
			Assert.That( object.ReferenceEquals( type.Object.UnderlyingSystemType, type2.Object.UnderlyingSystemType ) );
		}
	}
}
