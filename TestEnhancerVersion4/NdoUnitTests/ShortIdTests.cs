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
			Assert.AreEqual( decodedShortId, shortId, "Die ShortIds sollten gleich sein. #1" );
			string encodedShortId = decodedShortId.Encode();
			Assert.AreEqual( encodedShortId, decodedShortId, "Die ShortIds sollten gleich sein. #2" );
			Type t = shortId.GetObjectType(pm);
			Assert.AreEqual( t, typeof( Mitarbeiter ), "Der Typ der ShortId sollte Mitarbeiter sein." );
			Assert.AreEqual( shortId.GetEntityName(), "Mitarbeiter", "Der Entity-Name der ShortId sollte Mitarbeiter sein." );
		}

		[Test]
		public void TestConversionMitUmlauten()
		{
			string shortId = ((IPersistenceCapable)this.reiseBüro).ShortId();
			Console.WriteLine(shortId);
			string decodedShortId = shortId.Decode();
			Assert.AreNotEqual( decodedShortId, shortId, "Die ShortIds sollten ungleich sein. #1" );
			string encodedShortId = decodedShortId.Encode();
			Assert.AreNotEqual( encodedShortId, decodedShortId, "Die ShortIds sollten ungleich sein. #2" );
			Assert.AreEqual( encodedShortId, shortId, "Die ShortId sollte gleich sein." );
			Type t = shortId.GetObjectType(pm);
			Assert.AreEqual( t, typeof( Reisebüro ), "Der Typ der ShortId sollte Reisebüro sein." );
			Assert.AreEqual( shortId.GetEntityName(), "Reisebüro", "Der Entity-Name der ShortId sollte Reisebüro sein." );
		}

		[Test]
		public void TestReadableShortIdMitUmlauten()
		{
			string shortId = ((IPersistenceCapable)this.reiseBüro).ShortId();
			Assert.That( shortId.IsShortId() );
			string[] arr = shortId.Split( '~' );
			arr[0] = typeof( Reisebüro ).FullName;
			arr[1] = typeof( Reisebüro ).Assembly.GetName().Name;
			shortId = string.Join( "~", arr );
			Assert.That( shortId.GetEntityName() == "Reisebüro" );
			Assert.That( shortId.GetObjectType( pm ) == typeof( Reisebüro ) );
			Reisebüro rb = (Reisebüro)pm.FindObject( shortId );
			Assert.NotNull( rb );
		}

		[Test]
		public void TestReadableShortIdOhneUmlaute()
		{
			string shortId = ((IPersistenceCapable)this.m).ShortId();
			Assert.That( shortId.IsShortId() );
			string[] arr = shortId.Split( '~' );
			arr[0] = typeof( Mitarbeiter ).FullName;
			arr[1] = typeof( Mitarbeiter ).Assembly.GetName().Name;
			shortId = string.Join( "~", arr );
			Assert.That( shortId.GetEntityName() == "Mitarbeiter" );
			Assert.That( shortId.GetObjectType( pm ) == typeof( Mitarbeiter ) );
			Mitarbeiter rb = (Mitarbeiter)pm.FindObject( shortId );
			Assert.NotNull( rb );
		}

		[Test]
		public void TestIfMultikeyGivesAnException()
		{
			IPersistenceCapable orderDetail = pm.FindObject( typeof( OrderDetail ), new object[] { new Guid( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ), new Guid( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ) } );
			bool exceptionOccured = false;
			try
			{
				string shortId = orderDetail.ShortId();
			}
			catch
			{
				exceptionOccured = true;
			}
			Assert.That( exceptionOccured, "It shouldn't be possible to get a ShortId from an object with mulitple keys." );
			Type t = typeof(OrderDetail);
			string sid = t.FullName + "~" + new AssemblyName( t.Assembly.FullName ).Name + "~4711";
			Assert.That( sid.IsShortId(), "The string should be a wellformed ShortId" );
			exceptionOccured = false;
			try
			{
				orderDetail = pm.FindObject( sid );
			}
			catch
			{
				exceptionOccured = true;
			}
			Assert.That( exceptionOccured, "It shouldn't be possible to retrieve an object with mulitple keys using a ShortId." );
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
