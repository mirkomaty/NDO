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
	public class NDOLinqTests
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

		[Test]
		public void LinqCheckMitarbeiterQuery()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>();
			string qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter]", qs );
		}

		[Test]
		public void LinqTestIfSimpleWhereClauseWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname == "Mirko" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt.QueryString );
			// Query for Oid values
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Oid() == 5 );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.NDOObjectId == 5 );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Oid().Equals( 5 ) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.NDOObjectId.Equals( 5 ) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckIfGeneratedQueryCanBeCalledTwice()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname == "Mirko" );

			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt.QueryString );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckIfWhereClauseWith1nRelationWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Zweck == "ADC" );
			string qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}}", qs );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid().Equals( 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid() == 5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].NDOObjectId.Equals( 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].NDOObjectId == 5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
		}

		[Test]
		public void LinqCheckIfWhereClauseWith11RelationWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.Lkz.Like( "D%" ) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckIfMultipleRelationsWork()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.Lkz.Like( "D%" ) && m.Reisen[Any.Index].Länder[Any.Index].Name == "D" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Adresse].[Lkz] LIKE {{0}} AND [Land].[Name] = {{1}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckOidWithTable()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Länder[Any.Index].Oid() == 55 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Land].[ID] = {{0}}", this.mitarbeiterFields ), vt.QueryString );
		}

		[Test]
		public void LinqCheckThatOneJoinAppearsOnlyOneTime()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.Lkz.Like("D%") && m.Adresse.Ort != "Bad Tölz" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE {{0}} AND [Adresse].[Ort] <> {{1}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Zweck == "ADC" || m.Reisen[Any.Index].Zweck == "ADW" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}} OR [Reise].[Zweck] = {{1}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckNotOperator()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where(m => !(m.Nachname == "Matytschak") );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE NOT ([Mitarbeiter].[Nachname] = {{0}})", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname.Like( "M%" ) && !(m.Nachname == "Matytschak") );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] LIKE {{0}} AND NOT ([Mitarbeiter].[Nachname] = {{1}})", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => !(m.Vorname.Like( "M%" ) && m.Nachname == "Matytschak") );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE NOT ([Mitarbeiter].[Vorname] LIKE {{0}} AND [Mitarbeiter].[Nachname] = {{1}})", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Zweck == "ADC" || !(m.Reisen[Any.Index].Zweck == "ADW") );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}} OR NOT ([Reise].[Zweck] = {{1}})", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => !(m.Reisen[Any.Index].Zweck == "ADC" || m.Reisen[Any.Index].Zweck == "ADW") );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE NOT ([Reise].[Zweck] = {{0}} OR [Reise].[Zweck] = {{1}})", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => !(m.Reisen[Any.Index].Länder[Any.Index].IsInEu == true) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE NOT ([Land].[IsInEu] = {{0}})", vt.QueryString );
		}

		[Test]
		public void LinqCheckBetween()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname.Between( "A", "B" ) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] BETWEEN {{0}} AND {{1}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => !m.Vorname.Between( "A", "B" ) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE NOT [Mitarbeiter].[Vorname] BETWEEN {{0}} AND {{1}}", vt.QueryString );
		}

		[Test]
		public void LinqTestValueType()
		{
			// Failure: No Accessor for Position.X and Position.Y possible
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Position.X > 2 && m.Position.Y < 5);
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Position_X] > {{0}} AND [Mitarbeiter].[Position_Y] < {{1}}", vt.QueryString );
		}

		[Test]
		public void TestValueTypeRelation()
		{
			// Failure: No Accessor for Position.X and Position.Y possible
			//NDOQuery<Sozialversicherungsnummer> q = new NDOQuery<Sozialversicherungsnummer>( pm, "arbeiter.position.X > 2 AND arbeiter.position.Y < 5" );
			//var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).SelectList;
			//Assert.AreEqual( String.Format( $"SELECT {fields} FROM [Sozialversicherungsnummer] INNER JOIN [Mitarbeiter] ON [Mitarbeiter].[ID] = [Sozialversicherungsnummer].[IDSozial] WHERE [Mitarbeiter].[Position_X] > 2 AND [Mitarbeiter].[Position_Y] < 5", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void LinqCheckFetchGroupInitializationWithExpressions()
		{
			//TODO: Clarify how this should be implemented in Linq
			//FetchGroup<Mitarbeiter> fg = new FetchGroup<Mitarbeiter>( m => m.Vorname, m => m.Nachname );
			//Assert.AreEqual( fg.Count, 2, "Count should be 2" );
			//Assert.AreEqual( "Vorname", fg[0], "Name wrong #1" );
			//Assert.AreEqual( "Nachname", fg[1], "Name wrong #2" );
		}

		[Test]
		public void LinqCheckIfMultiKeysWork()
		{
			var orderDetail = new OrderDetail();
			var vt = pm.Objects<OrderDetail>().Where( od => od.Oid() == orderDetail.NDOObjectId );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( OrderDetail ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [OrderDetail] WHERE [OrderDetail].[IDProduct] = {{0}} AND [OrderDetail].[IDOrder] = {{1}}", vt.QueryString );
			bool thrown = false;
			try
			{
				vt = pm.Objects<OrderDetail>().Where( od => od.Oid() == -4 );
				string s = vt.QueryString;
			}
			catch (Exception)
			{
				thrown = true;
			}
			// This fails, because the parameter won't be checked by the parser. But it should, at least in the WherePart-Generator.
			Assert.AreEqual( true, thrown );
		}

		[Test]
		public void LinqTestSuperclasses()
		{
			var vt = pm.Objects<Kostenpunkt>();
			Assert.AreEqual( $"SELECT {this.belegFields} FROM [Beleg];\r\nSELECT {this.pkwFahrtFields} FROM [PKWFahrt]", vt.QueryString );
		}

		[Test]
		public void LinqTestPolymorphicRelationQueries()
		{
            var vt = pm.Objects<Reise>().Where(r => r.Kostenpunkte[Any.Index].Datum == DateTime.Now.Date);

			Assert.AreEqual( $"SELECT {reiseFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [Beleg] ON [Beleg].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 926149172 WHERE [Beleg].[Datum] = {{0}} UNION \r\nSELECT {reiseFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [PKWFahrt] ON [PKWFahrt].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 734406058 WHERE [PKWFahrt].[Datum] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqTest1To1()
		{
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.Zimmer.Zimmer == "abc");
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Buero] ON [Buero].[ID] = [Mitarbeiter].[IDBuero] WHERE [Buero].[Zimmer] = {{0}}", vt.QueryString );
		}


		[Test]
		public void LinqTest1To1Bidirectional()
		{
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.SVN.SVN == 4711);
            NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "sn.nummer = 'abc'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Sozialversicherungsnummer] ON [Sozialversicherungsnummer].[ID] = [Mitarbeiter].[IDSozial] WHERE [Sozialversicherungsnummer].[Nummer] = 'abc'", this.mitarbeiterFields ), q.GeneratedQuery );
            var vt2 = pm.Objects<Sozialversicherungsnummer>().Where(s=>s.Angestellter.Vorname == "Mirko");
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [Sozialversicherungsnummer] INNER JOIN [Mitarbeiter] ON [Mitarbeiter].[ID] = [Sozialversicherungsnummer].[IDSozial] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt2.QueryString );
		}

		[Test]
		public void LinqTest1To1BiWithTable()
		{
            var vt1 = pm.Objects<Zertifikat>().Where(z=>z.SGN.Key == "abc");
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Zertifikat ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [Zertifikat] INNER JOIN [relSignaturZertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] INNER JOIN [Signatur] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] WHERE [Signatur].[Signature] = {{0}}", vt1.QueryString );
            var vt2 = pm.Objects<Signatur>().Where(sg=>sg.Owner.Key == -4);
			fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Signatur ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [Signatur] INNER JOIN [relSignaturZertifikat] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] INNER JOIN [Zertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] WHERE [Zertifikat].[Schlüssel] = {{0}}", vt2.QueryString);
		}

		[Test]
		public void LinqTest1ToNWithTable()
		{
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.ReiseBüros.ElementAt(Any.Index).Name == "abc");
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [relMitarbeiterReisebuero] ON [Mitarbeiter].[ID] = [relMitarbeiterReisebuero].[IDMitarbeiter] INNER JOIN [Reisebuero] ON [Reisebuero].[ID] = [relMitarbeiterReisebuero].[IDReisebuero] WHERE [Reisebuero].[Name] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqTestIfQueryForNonNullOidsWorks()
		{
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.Reisen[Any.Index].Länder.Oid() != null);
			Assert.AreEqual($"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] IS NOT NULL", vt.QueryString );
            vt = pm.Objects<Mitarbeiter>().Where(m => m.Reisen[Any.Index].Länder[Any.Index].NDOObjectId != null);
            Assert.AreEqual($"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] IS NOT NULL", vt.QueryString);
        }

        [Test]
		public void LinqTestIfInClauseWorks()
		{
            var arr = new[] { "Mirko", "Hans" };
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.Vorname.In(arr));            
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] IN ('Mirko', 'Hans')", vt.QueryString );
            vt = pm.Objects<Mitarbeiter>().Where(m => m.Vorname.In(new[] { "Mirko", "Hans" }));
            Assert.AreEqual($"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] IN ('Mirko', 'Hans')", vt.QueryString);
        }

        [Test]
		public void LinqTestIfInClauseWithNumbersWorks()
		{
            // NDO won't check, if the types match
            var arr = new[] { 1,2,3,4,5 };
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.Vorname.In(arr));
            Assert.AreEqual($"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] IN (1, 2, 3, 4, 5)", vt.QueryString);
        }

        [Test]
		public void LinqTestIfRelationInInClauseWorks()
		{
            var arr = new[] { 1, 2, 3, 4, 5 };
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.Reisen.Oid().In(arr));
            Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IN (1, 2, 3, 4, 5)", vt.QueryString );
            vt = pm.Objects<Mitarbeiter>().Where(m => m.Reisen.Oid().In(new[] { 1, 2, 3, 4, 5 }));
            Assert.AreEqual($"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IN (1, 2, 3, 4, 5)", vt.QueryString);
        }

        [Test]
		public void TestIfOidWithInClauseWorks()
		{
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.Oid().In(new[] { 1, 2, 3, 4, 5 }));
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] IN (1, 2, 3, 4, 5)", vt.QueryString );
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
	}
}
