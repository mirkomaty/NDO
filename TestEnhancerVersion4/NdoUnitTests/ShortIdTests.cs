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
			string decodedShortId = shortId.Decode();
			Assert.AreEqual( decodedShortId, shortId, "Die ShortIds sollten gleich sein. #1" );
			string encodedShortId = decodedShortId.Encode();
			Assert.AreEqual( encodedShortId, decodedShortId, "Die ShortIds sollten gleich sein. #2" );
			Type t = shortId.GetObjectType();
			Assert.AreEqual( t, typeof( Mitarbeiter ), "Der Typ der ShortId sollte Mitarbeiter sein." );
		}

		[Test]
		public void TestConversionMitUmlauten()
		{
			string shortId = ((IPersistenceCapable)this.reiseBüro).ShortId();
			string decodedShortId = shortId.Decode();
			Assert.AreNotEqual( decodedShortId, shortId, "Die ShortIds sollten ungleich sein. #1" );
			string encodedShortId = decodedShortId.Encode();
			Assert.AreNotEqual( encodedShortId, decodedShortId, "Die ShortIds sollten ungleich sein. #2" );
			Assert.AreEqual( encodedShortId, shortId, "Die ShortId sollte gleich sein." );
			Type t = shortId.GetObjectType();
			Assert.AreEqual( t, typeof( Reisebüro ), "Der Typ der ShortId sollte Reisebüro sein." );
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
