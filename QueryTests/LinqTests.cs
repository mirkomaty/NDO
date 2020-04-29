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
		string mitarbeiterJoinFields;
		string belegFields;
		string pkwFahrtFields;
		string reiseFields;
		string reiseJoinFields;

		[SetUp]
		public void SetUp()
		{
			this.pm = NDOFactory.Instance.PersistenceManager;

			mitarbeiterFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Mitarbeiter ) ) ).SelectList;
			mitarbeiterJoinFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Mitarbeiter ) ) ).Result( false, false, true );
			belegFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Beleg ) ) ).SelectList;
			this.pkwFahrtFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( PKWFahrt ) ) ).SelectList;
			this.reiseFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Reise ) ) ).SelectList;
			this.reiseJoinFields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Reise ) ) ).Result( false, false, true );
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
		public void CheckIfOrderingsWork()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>();
			vt.OrderBy( m => m.Vorname ).OrderByDescending( m => m.Nachname );
			string qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] ORDER BY [Mitarbeiter].[Vorname] ASC, [Mitarbeiter].[Nachname] DESC", qs );

			vt = pm.Objects<Mitarbeiter>();
			vt.OrderBy( m => m.Vorname ).OrderByDescending( m => m.Nachname );
			vt.Take( 10 ).Skip( 12 );
			qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] ORDER BY [Mitarbeiter].[Vorname] ASC, [Mitarbeiter].[Nachname] DESC OFFSET 12 ROWS FETCH NEXT 10 ROWS ONLY", qs );
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
		public void LinqParameterChangesDontChangeTheQuery()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname == "Mirko" );

			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt.QueryString );

			vt.ReplaceParameters( new object[] { "Hans" } );

			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckIfWhereClauseWith1nRelationWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Zweck == "ADC" );
			string qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}}", qs );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid().Equals( 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid() == 5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].NDOObjectId.Equals( 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].NDOObjectId == 5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
		}

		[Test]
		public void LinqCheckIfWhereClauseWithAnyIn1nRelationWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen.Any(r=>r.Zweck == "ADC") );
			string qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}}", qs );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen.Any( r => r.Oid().Equals( 5 ) ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen.Any( r => r.Oid() == 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen.Any( r => r.NDOObjectId.Equals( 5 ) ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen.Any( r => r.NDOObjectId == 5 ) );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
		}


		[Test]
		public void LinqCheckIfWhereClauseWith11RelationWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.Lkz.Like( "D%" ) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckIfWhereClauseWithOidIn11RelationWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.Oid() == 5 );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[IDAdresse] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckIfMultipleRelationsWork()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.Lkz.Like( "D%" ) && m.Reisen[Any.Index].Länder[Any.Index].Name == "D" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Adresse].[Lkz] LIKE {{0}} AND [Land].[Name] = {{1}}", vt.QueryString );
		}

		[Test]
		public void LinqCheckOidWithTable()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Länder[Any.Index].Oid() == 55 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
		}

		[Test]
		public void LinqCheckThatOneJoinAppearsOnlyOneTime()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.Lkz.Like("D%") && m.Adresse.Ort != "Bad Tölz" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE {{0}} AND [Adresse].[Ort] <> {{1}}", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Zweck == "ADC" || m.Reisen[Any.Index].Zweck == "ADW" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}} OR [Reise].[Zweck] = {{1}}", vt.QueryString );
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
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = {{0}} OR NOT ([Reise].[Zweck] = {{1}})", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => !(m.Reisen[Any.Index].Zweck == "ADC" || m.Reisen[Any.Index].Zweck == "ADW") );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE NOT ([Reise].[Zweck] = {{0}} OR [Reise].[Zweck] = {{1}})", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => !(m.Reisen[Any.Index].Länder[Any.Index].IsInEu == true) );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE NOT ([Land].[IsInEu] = {{0}})", vt.QueryString );
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
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Position.X > 2 && m.Position.Y < 5);
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Position_X] > {{0}} AND [Mitarbeiter].[Position_Y] < {{1}}", vt.QueryString );
		}

		[Test]
		public void TestValueTypeRelation()
		{
			var vt = pm.Objects<Sozialversicherungsnummer>().Where( s => s.Angestellter.Position.X > 2 && s.Angestellter.Position.Y < 5 );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).Result( false, false, true );
			Assert.AreEqual( $"SELECT {fields} FROM [Sozialversicherungsnummer] INNER JOIN [Mitarbeiter] ON [Mitarbeiter].[ID] = [Sozialversicherungsnummer].[IDSozial] WHERE [Mitarbeiter].[Position_X] > {{0}} AND [Mitarbeiter].[Position_Y] < {{1}}", vt.QueryString );
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
#if ignored
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
			// This fails, because the parameter won't be checked by the parser. 
			// It isn't checked in the WherePart-Generator neither because it checks only, if the right side of the comparism is a parameter.
			// We need to check the oid mapping to detect this situation.
			// Or we might let it be, because we will get an exception anyway, if the query is executed.
			Assert.AreEqual( true, thrown );
#endif
		}

		[Test]
		public void LinqTestBooleanExpression()
		{
			var vt = pm.Objects<Land>().Where( l => l.IsInEu == true );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Land ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [Land] WHERE [Land].[IsInEu] = {{0}}", vt.QueryString );

			vt = pm.Objects<Land>().Where( l => l.IsInEu );
			Assert.AreEqual( $"SELECT {fields} FROM [Land] WHERE [Land].[IsInEu] = {{0}}", vt.QueryString );
		}


		[Test]
		public void LinqTestSuperclasses()
		{
			var vt = pm.Objects<Kostenpunkt>();
			Assert.AreEqual( $"SELECT {this.belegFields} FROM [Beleg];\r\nSELECT {this.pkwFahrtFields} FROM [PKWFahrt]", vt.QueryString );
		}

		[Test]
		public void CanAddPrefetches()
		{
			var vt = pm.Objects<Mitarbeiter>();
			vt.AddPrefetch( m => m.Reisen );
			vt.AddPrefetch( m => m.Reisen[Any.Index].Länder );
			var list = vt.Prefetches.ToList();
			Assert.AreEqual( 2, list.Count );
			Assert.AreEqual( "Reisen", list[0] );
			Assert.AreEqual( "Reisen.Länder", list[1] );
		}

		[Test]
		public void LinqSimplePrefetchWorks()
		{
			var vt = pm.Objects<Mitarbeiter>();
			vt.AddPrefetch( m => m.Reisen );
			var s = vt.QueryString;
		}

		[Test, Ignore("This is not implemented")]
		public void LinqPrefetchWithBidirectionalRelationWorks()
		{
			Assert.That( false, "Not implemented" );
			// With Bidirectional Relation (vorhandener JOIN)
		}

		[Test, Ignore( "This is not implemented" )]
		public void LinqPrefetchWithMonoRelationWorks()
		{
			Assert.That( false, "Not implemented" );
			// Monodirektional (neuer JOIN)
		}

		[Test, Ignore( "This is not implemented" )]
		public void LinqPrefetchWithDifferentRelationRolesWorks()
		{
			Assert.That( false, "Not implemented" );
			// Unterschiedliche Relationen werden auseinandergehalten
		}

		[Test]
		public void LinqTestPolymorphicRelationQueries()
		{
			// We have to patch the AccessorName here, since the Enhancer doesn't create the AccessorName automatically.
			// This will change as soon as we update the tests.
			pm.NDOMapping.FindClass( typeof( Kostenpunkt ) ).FindField( "datum" ).AccessorName = "Datum";
			pm.NDOMapping.FindClass( typeof( Beleg ) ).FindField( "datum" ).AccessorName = "Datum";
			pm.NDOMapping.FindClass( typeof( PKWFahrt ) ).FindField( "datum" ).AccessorName = "Datum";

			var vt = pm.Objects<Reise>().Where(r => r.Kostenpunkte[Any.Index].Datum == DateTime.Now.Date);

			Assert.AreEqual( $"SELECT {reiseJoinFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [Beleg] ON [Beleg].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 926149172 WHERE [Beleg].[Datum] = {{0}} UNION \r\nSELECT {reiseJoinFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [PKWFahrt] ON [PKWFahrt].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 734406058 WHERE [PKWFahrt].[Datum] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqTest1To1()
		{
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.Zimmer.Zimmer == "abc");
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Buero] ON [Buero].[ID] = [Mitarbeiter].[IDBuero] WHERE [Buero].[Zimmer] = {{0}}", vt.QueryString );
		}


		[Test]
		public void LinqTest1To1Bidirectional()
		{
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.SVN.SVN == 4711);
            NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "sn.nummer = 'abc'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Sozialversicherungsnummer] ON [Sozialversicherungsnummer].[ID] = [Mitarbeiter].[IDSozial] WHERE [Sozialversicherungsnummer].[Nummer] = 'abc'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
            var vt2 = pm.Objects<Sozialversicherungsnummer>().Where(s=>s.Angestellter.Vorname == "Mirko");
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).Result( false, false, true );
			Assert.AreEqual( $"SELECT {fields} FROM [Sozialversicherungsnummer] INNER JOIN [Mitarbeiter] ON [Mitarbeiter].[ID] = [Sozialversicherungsnummer].[IDSozial] WHERE [Mitarbeiter].[Vorname] = {{0}}", vt2.QueryString );
		}

		[Test]
		public void LinqTest1To1BiWithTable()
		{
            var vt1 = pm.Objects<Zertifikat>().Where(z=>z.SGN.Key == "abc");
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Zertifikat ) ) ).Result( false, false, true );
			Assert.AreEqual( $"SELECT {fields} FROM [Zertifikat] INNER JOIN [relSignaturZertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] INNER JOIN [Signatur] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] WHERE [Signatur].[Signature] = {{0}}", vt1.QueryString );
            var vt2 = pm.Objects<Signatur>().Where(sg=>sg.Owner.Key == -4);
			fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Signatur ) ) ).Result( false, false, true );
			Assert.AreEqual( $"SELECT {fields} FROM [Signatur] INNER JOIN [relSignaturZertifikat] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] INNER JOIN [Zertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] WHERE [Zertifikat].[Schlüssel] = {{0}}", vt2.QueryString);
		}

		[Test]
		public void LinqTest1ToNWithTable()
		{
			// We have to patch the AccessorName here, since the Enhancer doesn't create the AccessorName automatically.
			// This will change as soon as we update the tests.
			pm.NDOMapping.FindClass( typeof( Mitarbeiter ) ).FindRelation( "reiseBüros" ).AccessorName = "ReiseBüros";
            var vt = pm.Objects<Mitarbeiter>().Where(m => m.ReiseBüros.ElementAt(Any.Index).Name == "abc");
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [relMitarbeiterReisebuero] ON [Mitarbeiter].[ID] = [relMitarbeiterReisebuero].[IDMitarbeiter] INNER JOIN [Reisebuero] ON [Reisebuero].[ID] = [relMitarbeiterReisebuero].[IDReisebuero] WHERE [Reisebuero].[Name] = {{0}}", vt.QueryString );
		}

		[Test]
		public void LinqTestIfQueryForNonNullOidsWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Länder.Oid() != null );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] IS NOT NULL", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Länder[Any.Index].NDOObjectId != null );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] IS NOT NULL", vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse.NDOObjectId != null );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[IDAdresse] IS NOT NULL", vt.QueryString );
		}

		[Test]
		public void LinqTestIfQueryWithNonNullRelationWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse != null );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[IDAdresse] IS NOT NULL", vt.QueryString );
		}

		[Test]
		public void LinqTestIfQueryWithNullRelationWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Adresse == null );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[IDAdresse] IS NULL", vt.QueryString );
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
            Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IN (1, 2, 3, 4, 5)", vt.QueryString );
            vt = pm.Objects<Mitarbeiter>().Where(m => m.Reisen.Oid().In(new[] { 1, 2, 3, 4, 5 }));
            Assert.AreEqual($"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IN (1, 2, 3, 4, 5)", vt.QueryString);
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
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
			vt = pm.Objects<Mitarbeiter>().Where( m => (int)m.Reisen[Any.Index].NDOObjectId[0] == 5 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), vt.QueryString );
		}

		[Test]
		public void TestIfLinqQueryForNonNullOidsWorks()
		{
			VirtualTable<Mitarbeiter> vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Oid() != null );
			var qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IS NOT NULL", qs );
			vt = pm.Objects<Mitarbeiter>().Where( m => m.Reisen[Any.Index].Länder[Any.Index].Oid() != null );
			qs = vt.QueryString;
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] IS NOT NULL", qs );
		}

		[Test]
		public void ComparismWithStringWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname.GreaterEqual( "abc" ) && m.Vorname.LowerThan( "abcd") );
			var qs = vt.QueryString; 
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] >= {0}" ) > -1 );
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] < {1}" ) > -1 );
		}

		[Test]
		public void FlipParametersWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where(m => "abc" == m.Vorname );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] = {0}" ) > -1 );
		}

		[Test]
		public void FlipParametersInComplexExpressionsWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where(m => "abc" == m.Vorname && "def" == m.Nachname );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] = {0}" ) > -1 );
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Nachname] = {1}" ) > -1 );
		}

		[Test]
		public void FlipParametersWithGTWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => "abc".GreaterThan(m.Vorname) );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] < {0}" ) > -1 );
		}

		[Test]
		public void FlipParametersWithGEWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => "abc".GreaterEqual(m.Vorname) );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] <= {0}" ) > -1 );
		}
		[Test]
		public void FlipParametersWithLTWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => "abc".LowerThan(m.Vorname) );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] > {0}" ) > -1 );
		}
		[Test]
		public void FlipParametersWithLEWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => "abc".LowerEqual(m.Vorname) );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] >= {0}" ) > -1 );
		}

		[Test]
		public void GTWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname.GreaterThan( "abc" ) );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] > {0}" ) > -1 );
		}

		[Test]
		public void GEWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname.GreaterEqual( "abc" ) );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] >= {0}" ) > -1 );
		}
		[Test]
		public void LTWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname.LowerThan( "abc" ) );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] < {0}" ) > -1 );
		}
		[Test]
		public void LEWorks()
		{
			var vt = pm.Objects<Mitarbeiter>().Where( m => m.Vorname.LowerEqual("abc") );
			var qs = vt.QueryString;
			Assert.That( qs.IndexOf( "[Mitarbeiter].[Vorname] <= {0}" ) > -1 );
		}

	}
}
