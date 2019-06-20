//
// Copyright (c) 2002-2016 Mirko Matytschak 
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


using NDO;
using NDO.ShortId;
using Newtonsoft.Json;
using NUnit.Framework;
using PureBusinessClasses;
using Reisekosten;
using Reisekosten.Personal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NdoUnitTests
{
	[TestFixture]
	public class ShortIdTests
	{
		PersistenceManager pm;
		Mitarbeiter m;
		Reisebüro reiseBüro;
		Guid guid = new Guid( new byte[] { 1, 1, 1, 1,  1, 1, 1, 1,  1, 1, 1, 1,  1, 1, 1, 1 } );

		[SetUp]
		public void Setup()
		{
			pm = PmFactory.NewPersistenceManager();
			Type t = pm.NDOMapping.FindClass( typeof( Mitarbeiter ) ).Oid.OidColumns[0].SystemType;
			if (t == typeof( int ))
			{
				this.m = (Mitarbeiter)pm.FindObject( typeof( Mitarbeiter ), 1 );
				this.reiseBüro = (Reisebüro)pm.FindObject( typeof( Reisebüro ), 1 );
			}
			else
			{
				this.m = (Mitarbeiter)pm.FindObject( typeof( Mitarbeiter ), this.guid );
				this.reiseBüro = (Reisebüro)pm.FindObject( typeof( Reisebüro ), this.guid );
			}
		}

		[Test]
		public void TestConversionOhneUmlaute()
		{
			string shortId = ((IPersistenceCapable)m).ShortId();
			Console.WriteLine(shortId);
			string decodedShortId = shortId.Decode();
			Assert.AreEqual( decodedShortId, shortId, "ShortIds should be equal. #1" );
			string encodedShortId = decodedShortId.Encode();
			Assert.AreEqual( encodedShortId, decodedShortId, "ShortIds should be equal. #2" );
			Type t = shortId.GetObjectType(pm);
			Assert.AreEqual( t, typeof( Mitarbeiter ), "The Type should be Mitarbeiter." );
			Assert.AreEqual( shortId.GetEntityName(), "Mitarbeiter", "The Entity Name should be Mitarbeiter." );
		}

		[Test]
		public void TestConversionMitUmlauten()
		{
			string shortId = ((IPersistenceCapable)this.reiseBüro).ShortId();
			Console.WriteLine(shortId);
			string decodedShortId = shortId.Decode();
			Assert.AreNotEqual( decodedShortId, shortId, "The ShortIds should'nt be equal. #1" );
			string encodedShortId = decodedShortId.Encode();
			Assert.AreNotEqual( encodedShortId, decodedShortId, "The ShortIds should'nt be equal. #2" );
			Assert.AreEqual( encodedShortId, shortId, "The ShortId should be equal." );
			Type t = shortId.GetObjectType(pm);
			Assert.AreEqual( t, typeof( Reisebüro ), "The Type should be Reisebüro." );
			Assert.AreEqual( shortId.GetEntityName(), "Reisebüro", "The Entity Name should be Reisebüro." );
		}

		[Test]
		public void TestReadableShortIdMitUmlauten()
		{
			// Note: This test uses the old ShortId format, which is deprecated
			string shortId = ((IPersistenceCapable)this.reiseBüro).ShortId();
			Assert.That( shortId.IsShortId() );
			string[] arr = shortId.Split( '-' );
			arr[0] = typeof( Reisebüro ).FullName;
			arr[1] = typeof( Reisebüro ).Assembly.GetName().Name;
			shortId = string.Join( "-", arr );
			Assert.That( shortId.GetEntityName() == "Reisebüro" );
			Assert.That( shortId.GetObjectType( pm ) == typeof( Reisebüro ) );
			Reisebüro rb = (Reisebüro)pm.FindObject( shortId );
			Assert.NotNull( rb );
		}

		[Test]
		public void TestReadableShortIdOhneUmlaute()
		{
			// Note: This test uses the old ShortId format, which is deprecated
			string shortId = ((IPersistenceCapable)this.m).ShortId();
			Assert.That( shortId.IsShortId() );
			string[] arr = shortId.Split( '-' );
			arr[0] = typeof( Mitarbeiter ).FullName;
			arr[1] = typeof( Mitarbeiter ).Assembly.GetName().Name;
			shortId = string.Join( "-", arr );
			Assert.That( shortId.GetEntityName() == "Mitarbeiter" );
			Assert.That( shortId.GetObjectType( pm ) == typeof( Mitarbeiter ) );
			Mitarbeiter rb = (Mitarbeiter)pm.FindObject( shortId );
			Assert.NotNull( rb );
		}

		[Test]
		public void TestIfMultikeysCanBeConverted()
		{
			var guid = new Guid( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 );
			var guidString = guid.ToString().Replace( "-", "" );
			IPersistenceCapable orderDetail = pm.FindObject( typeof( OrderDetail ), new object[] { guid, guid } );
			string shortId = ((IPersistenceCapable)orderDetail).ShortId();
			Assert.That( shortId.IsShortId() );
			Assert.That( shortId.GetEntityName() == "OrderDetail" );
			Assert.That( shortId.GetObjectType( pm ) == typeof( OrderDetail ) );
			string[] arr = shortId.Split( '-' );
			Assert.AreEqual( $"{guidString}+{guidString}", arr[2] );
			var decodedShortId = shortId.Decode();
			arr = decodedShortId.Split( '-' );
			Assert.AreEqual( $"{guidString} {guidString}", arr[2] );

#if !USEGUIDS
			orderDetail = pm.FindObject( typeof(OrderDetail), new object[] { 1, 2 } );
			shortId = orderDetail.ShortId();
#endif
			orderDetail = pm.FindObject( shortId );
#if USEGUIDS
			Assert.AreEqual( guid, orderDetail.NDOObjectId.Id.Values[0] );
			Assert.AreEqual( guid, orderDetail.NDOObjectId.Id.Values[1] );
#else
			Assert.AreEqual( 1, orderDetail.NDOObjectId.Id.Values[0] );
			Assert.AreEqual( 2, orderDetail.NDOObjectId.Id.Values[1] );
#endif
		}

		public class ObjectIdConverter : JsonConverter<ObjectId>
		{
			public override void WriteJson( JsonWriter writer, ObjectId value, JsonSerializer serializer )
			{
				writer.WriteValue( value.ToString() );
			}

			public override ObjectId ReadJson( JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue, JsonSerializer serializer )
			{
				return null;
			}
		}

		[Test]
		public void OidArrayCanBeSerializedToJson()
		{
			var oid1 = ((IPersistenceCapable)pm.FindObject( typeof( Mitarbeiter ), 1 )).NDOObjectId;
			var oid2 = ((IPersistenceCapable)pm.FindObject( typeof( Mitarbeiter ), 2 )).NDOObjectId;
			ObjectIdList arr = new ObjectIdList { oid1, oid2 };
			var json = JsonConvert.SerializeObject( arr, new ObjectIdConverter() );
			Assert.AreEqual( "[\"Mitarbeiter-F33D0A6D-1\",\"Mitarbeiter-F33D0A6D-2\"]", json );
		}

		[Test]
		public void TestIfWeCanRetrieveAnObjectFromShortId()
		{
			string shortId = ((IPersistenceCapable)m).ShortId();
			Assert.That( shortId.IsShortId(), "The string should be a wellformed ShortId" );
			Mitarbeiter m2 = (Mitarbeiter)pm.FindObject( shortId );
			Assert.NotNull( m2 );
			Assert.AreEqual( this.m.NDOObjectId, m2.NDOObjectId );
		}

		[TearDown]
		public void TearDown()
		{
		}
	}
}
