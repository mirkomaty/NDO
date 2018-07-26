using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NDO;
using NDO.Query;
using NDOql.Expressions;
using NDO.Linq;
using Reisekosten;
using Reisekosten.Personal;
using PureBusinessClasses;
using NDO.SqlPersistenceHandling;
using System.Diagnostics;

namespace QueryTests
{
	[TestFixture]
	public class NDOQueryTests
	{
		PersistenceManager pm;
		string mitarbeiterFields;
		string belegFields;
		string pkwFahrtFields;
		string reiseFields;

		[SetUp]
		public void SetUp()
		{
			this.pm = NDOFactory.Instance.PersistenceManager;

			mitarbeiterFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Mitarbeiter ) ) ).SelectList;
			belegFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Beleg ) ) ).SelectList;
			this.pkwFahrtFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( PKWFahrt ) ) ).SelectList;
			this.reiseFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Reise ) ) ).SelectList;
			Mitarbeiter m = new Mitarbeiter() { Vorname = "Mirko", Nachname = "Matytschak" };
			pm.MakePersistent( m );
			m = new Mitarbeiter() { Vorname = "Hans", Nachname = "Huber" };
			pm.MakePersistent( m );
			pm.Save();
		}

		[TearDown]
		public void TearDown()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm );
			this.pm.Delete( q.Execute() );
			this.pm.Save();
		}

		[Test]
		public void CheckIfQueryWithoutWhereClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter]", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		public void CheckMitarbeiterQuery()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm );
			List<Mitarbeiter> l = q.Execute();
			Assert.AreEqual( 2, l.Count );
		}

		[Test]
		public void CheckIfSimpleWhereClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = 'Mirko'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "oid = 1" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = 1", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfGeneratedQueryCanBeCalledTwice()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = 'Mirko'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", this.mitarbeiterFields ), q.GeneratedQuery );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfWhereClauseWith1nRelationWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.zweck = 'ADC'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = 'ADC'", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.oid = {0}" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfWhereClauseWith11RelationWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse.lkz LIKE 'D%'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE 'D%'", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfMultipleRelationsWork()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse.lkz LIKE 'D%' AND dieReisen.dieLaender.name = 'D'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Adresse].[Lkz] LIKE 'D%' AND [Land].[Name] = 'D'", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckOidWithTable()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.dieLaender.oid = {0}" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Land].[ID] = {{0}}", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckThatOneJoinAppearsOnlyOneTime()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse.lkz LIKE 'D%' AND adresse.ort <> 'Bad Tölz'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE 'D%' AND [Adresse].[Ort] <> 'Bad Tölz'", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.zweck = 'ADC' OR dieReisen.zweck = 'ADW'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = 'ADC' OR [Reise].[Zweck] = 'ADW'", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckNotOperator()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname LIKE 'M%' AND NOT nachname = 'Matytschak'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] LIKE 'M%' AND NOT [Mitarbeiter].[Nachname] = 'Matytschak'", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "NOT (vorname LIKE 'M%' AND nachname = 'Matytschak')" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE NOT ([Mitarbeiter].[Vorname] LIKE 'M%' AND [Mitarbeiter].[Nachname] = 'Matytschak')", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.zweck = 'ADC' OR NOT dieReisen.zweck = 'ADW'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = 'ADC' OR NOT [Reise].[Zweck] = 'ADW'", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "NOT (dieReisen.zweck = 'ADC' OR dieReisen.zweck = 'ADW')" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE NOT ([Reise].[Zweck] = 'ADC' OR [Reise].[Zweck] = 'ADW')", this.mitarbeiterFields ), q.GeneratedQuery );
			bool thrown = false;
			try
			{
				q = new NDOQuery<Mitarbeiter>( pm, "vorname LIKE 'M%' AND nachname = NOT 'Matytschak'" );
				string s = q.GeneratedQuery;
			}
			catch (OqlExpressionException)
			{
				thrown = true;
			}
			Assert.AreEqual( true, thrown );

			// TODO: This is a wrong expression which passes the syntax check.
			// Mysql allows WHERE NOT True but disallows nachname = NOT True
			// Sql Server doesn't know the symbol True
			q = new NDOQuery<Mitarbeiter>( pm, "vorname LIKE 'M%' AND nachname = NOT True" );
			string t = q.GeneratedQuery; // Make sure, GeneratedQuery ist called twice.
			Console.WriteLine(t);
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] LIKE 'M%' AND [Mitarbeiter].[Nachname] = NOT TRUE", q.GeneratedQuery );
		}

		[Test]
		public void CheckBetween()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname BETWEEN 'A' AND 'B'" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] BETWEEN 'A' AND 'B'", q.GeneratedQuery );
		}

		[Test]
		public void TestValueType()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "position.X > 2 AND position.Y < 5" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Position_X] > 2 AND [Mitarbeiter].[Position_Y] < 5", q.GeneratedQuery );
		}

		[Test]
		public void TestValueTypeRelation()
		{
			NDOQuery<Sozialversicherungsnummer> q = new NDOQuery<Sozialversicherungsnummer>( pm, "arbeiter.position.X > 2 AND arbeiter.position.Y < 5" );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).SelectList;
			Assert.AreEqual( String.Format( $"SELECT {fields} FROM [Sozialversicherungsnummer] INNER JOIN [Mitarbeiter] ON [Mitarbeiter].[ID] = [Sozialversicherungsnummer].[IDSozial] WHERE [Mitarbeiter].[Position_X] > 2 AND [Mitarbeiter].[Position_Y] < 5", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckFetchGroupInitializationWithExpressions()
		{
			FetchGroup<Mitarbeiter> fg = new FetchGroup<Mitarbeiter>( m => m.Vorname, m => m.Nachname );
			Assert.AreEqual( fg.Count, 2, "Count should be 2" );
			Assert.AreEqual( "Vorname", fg[0], "Name wrong #1" );
			Assert.AreEqual( "Nachname", fg[1], "Name wrong #2" );
		}

		[Test]
		public void CheckIfMultiKeysWork()
		{
			NDOQuery<OrderDetail> q = new NDOQuery<OrderDetail>( pm, "oid = {0}" );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( OrderDetail ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [OrderDetail] WHERE [OrderDetail].[IDProduct] = {{0}} AND [OrderDetail].[IDOrder] = {{1}}", q.GeneratedQuery );
			bool thrown = false;
			try
			{
				q = new NDOQuery<OrderDetail>( pm, "oid = -4" );
				string s = q.GeneratedQuery;
			}
			catch (Exception)
			{
				thrown = true;
			}
			Assert.AreEqual( true, thrown );
		}

		[Test]
		public void TestSuperclasses()
		{
			NDOQuery<Kostenpunkt> qk = new NDOQuery<Kostenpunkt>( pm );
			Assert.AreEqual( $"SELECT {this.belegFields} FROM [Beleg];\r\nSELECT {this.pkwFahrtFields} FROM [PKWFahrt]", qk.GeneratedQuery );
		}

		[Test]
		public void TestPolymorphicRelationQueries()
		{
			NDOQuery<Reise> q = new NDOQuery<Reise>( pm, "belege.datum = {0}" );
			Assert.AreEqual( $"SELECT {reiseFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [Beleg] ON [Beleg].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 926149172 WHERE [Beleg].[Datum] = {{0}} UNION \r\nSELECT {reiseFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [PKWFahrt] ON [PKWFahrt].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 734406058 WHERE [PKWFahrt].[Datum] = {{0}}", q.GeneratedQuery );
		}

		[Test]
		public void Test1To1()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "meinBuero.zimmer = 'abc'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Buero] ON [Buero].[ID] = [Mitarbeiter].[IDBuero] WHERE [Buero].[Zimmer] = 'abc'", this.mitarbeiterFields ), q.GeneratedQuery );
		}


		[Test]
		public void Test1To1Bidirectional()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "sn.nummer = 'abc'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Sozialversicherungsnummer] ON [Sozialversicherungsnummer].[ID] = [Mitarbeiter].[IDSozial] WHERE [Sozialversicherungsnummer].[Nummer] = 'abc'", this.mitarbeiterFields ), q.GeneratedQuery );
			NDOQuery<Sozialversicherungsnummer> qs = new NDOQuery<Sozialversicherungsnummer>( pm, "arbeiter.vorname = 'Mirko'" );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [Sozialversicherungsnummer] INNER JOIN [Mitarbeiter] ON [Mitarbeiter].[ID] = [Sozialversicherungsnummer].[IDSozial] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", qs.GeneratedQuery );
		}

		[Test]
		public void Test1To1BiWithTable()
		{
			NDOQuery<Zertifikat> qz = new NDOQuery<Zertifikat>( pm, "sgn.signature = 'abc'" );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Zertifikat ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [Zertifikat] INNER JOIN [relSignaturZertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] INNER JOIN [Signatur] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] WHERE [Signatur].[Signature] = 'abc'", qz.GeneratedQuery );
			NDOQuery<Signatur> qs = new NDOQuery<Signatur>( pm, "owner.schlüssel = -4" );
			string s = qs.GeneratedQuery;
			fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Signatur ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [Signatur] INNER JOIN [relSignaturZertifikat] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] INNER JOIN [Zertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] WHERE [Zertifikat].[Schlüssel] = -4", qs.GeneratedQuery );
		}

		[Test]
		public void Test1ToNWithTable()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "reiseBüros.name = 'abc'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [relMitarbeiterReisebuero] ON [Mitarbeiter].[ID] = [relMitarbeiterReisebuero].[IDMitarbeiter] INNER JOIN [Reisebuero] ON [Reisebuero].[ID] = [relMitarbeiterReisebuero].[IDReisebuero] WHERE [Reisebuero].[Name] = 'abc'", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void TestIfQueryForNonNullOidsWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.dieLaender.oid IS NOT NULL" );
			String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Land].[ID] IS NOT NULL", this.mitarbeiterFields );
		}

		[Test]
		public void TestIfInClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname IN (1,2,3,4,5)" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] IN (1, 2, 3, 4, 5)", s );
		}

		[Test]
		public void TestIfInClauseWithStringsWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname IN ('1','2','3','4','5')" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] IN ('1', '2', '3', '4', '5')", s );
		}

		[Test]
		public void TestIfRelationInInClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.oid IN (1,2,3,4,5)" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IN (1, 2, 3, 4, 5)", s );
		}

		[Test]
		public void TestIfOidWithInClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "oid IN (1,2,3,4,5)" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] IN (1, 2, 3, 4, 5)", s );
		}

		[Test]
		public void TestIfSimpleLinqQueryWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>();
			string qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter]", qs );
		}

		[Test]
		public void TestIfSimpleLinqQueryWithExpressionWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where(m=>m.Vorname == "Mirko");
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt.QueryString );
			// Query for Oid values
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Oid() == 5 );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.NDOObjectId == 5 );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Oid().Equals(5) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.NDOObjectId.Equals(5) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
		}

		[Test]
		public void TestIfLinqQueryWithJoinWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Zweck == "ADC" );
			string qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}}", qs );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid().Equals( 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid() ==  5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].NDOObjectId.Equals( 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].NDOObjectId == 5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
		}

		[Test]
		public void TestIfLinqQueryWithOidParameterWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid(0).Equals( 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => (int)m.Reisen[Any.Index].NDOObjectId[0] == 5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
		}

		[Test]
		public void TestIfLinqQueryForNonNullOidsWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid() != null );
			var qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IS NOT NULL", qs );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Länder[Any.Index].Oid() != null );
			qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Land].[ID] IS NOT NULL", qs );
		}

		[Test]
		public void TestSimpleWriteAndReadOp()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( this.pm, "vorname = {0}" );
			q.Parameters.Add( "Mirko" );
			var list = q.Execute();
			Assert.AreEqual( 1, list.Count );
		}
	}
}
