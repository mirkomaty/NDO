﻿using System;
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
		}

		[Test]
		public void TestRanknDirNoPoly()
		{
			Type t1;
			Type t2;
			List<Class> classes = new List<Class>();


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

			cls = new Class( null );
			cls.FullName = "Reise";
			t2 = cls.SystemType = CreateType( cls.FullName ).Object;
			oid = new ClassOid( cls );
			cls.Oid = oid;
			classes.Add( cls );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { classes[1] } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( (Relation)null );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[t2] == 0 );
			Assert.That( updateRank[t1] == 1 );
		}

		[Test]
		public void TestRank1DirNoPoly()
		{
			Type t1;
			Type t2;
			List<Class> classes = new List<Class>();

			Class cls1 = new Class(null);
			cls1.FullName = "Mitarbeiter";
			t1 = cls1.SystemType = CreateType( cls1.FullName ).Object;
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
			t2 = cls2.SystemType = CreateType( cls2.FullName ).Object;			
			cls2.Oid = oid;  // We can use the same oid, since the code asks only for autoincremented columns
			classes.Add( cls2 );

			rel1Mock.Setup( r => r.ReferencedSubClasses ).Returns( new List<Class> { cls2 } );
			rel1Mock.Setup( r => r.ForeignRelation ).Returns( (Relation) null );

			ClassRank classRank = new ClassRank();
			var updateRank = classRank.BuildUpdateRank( classes );
			Assert.That( updateRank[t1] == 0 );
			Assert.That( updateRank[t2] == 1 );
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

		/// <summary>
		/// Creates a Type mock which can be used in updateRanks and other
		/// Lists and Dictionaries
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns>The type mock</returns>
		Mock<Type> CreateType(string typeName)
		{
			if (types.ContainsKey( typeName ))
				return types[typeName];
			Mock<Type> type = new Mock<Type>();
			type.Setup( t => t.FullName ).Returns( typeName );
			type.Setup( t => t.GetHashCode() ).Returns( typeName.GetHashCode() );
			type.Setup( t => t.Equals(It.IsAny<Type>()) ).Returns( (Type t)=>t.FullName == typeName );
			type.Setup( t => t.Equals( It.IsAny<object>() ) ).Returns( ( Type t ) => t.FullName == typeName );
			type.Setup( t => t.Name ).Returns( typeName.Substring( typeName.LastIndexOf( '.' ) + 1 ) );
			type.Setup( t => t.ToString() ).Returns( typeName );
			type.Setup( t => t.UnderlyingSystemType ).Returns( type.Object );
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
