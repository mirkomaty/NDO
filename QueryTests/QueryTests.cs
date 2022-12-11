using System;
using System.Collections.Generic;
using NUnit.Framework;
using NDO;
using NDO.Query;
using NDOql.Expressions;
using Reisekosten;
using Reisekosten.Personal;
using PureBusinessClasses;
using NDO.SqlPersistenceHandling;
using Moq;
using NDO.Configuration;
using NDO.Logging;
using NDO.Mapping;
using System.Collections;
using System.Data;
using System.Data.Common;
using DataTypeTestClasses;

namespace QueryTests
{
	[TestFixture]
	public class NDOQueryTests
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
			var pm = NDOFactory.Instance.PersistenceManager;
			pm.Delete( pm.Objects<Mitarbeiter>().ResultTable );
			pm.Save();
		}

		[Test]
		public void QueryWithEmptyGuidParameterSearchesForNull()
		{
			// The query will fetch for DataContainerDerived objects, too.
			// Hence we test with "StartsWith", because the query contains additional text, which doesn't matter here.
			var q = new NDOQuery<DataContainer>(pm, "guidVar = {0}");
			q.Parameters.Add( Guid.Empty );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( DataContainer ) ) ).SelectList;
			var sql = $"SELECT {fields} FROM [DataContainer] WHERE [DataContainer].[GuidVar] IS NULL";
			Assert.That( q.GeneratedQuery.StartsWith( sql ) );

			q = new NDOQuery<DataContainer>(pm, "guidVar <> {0}");
			q.Parameters.Add( Guid.Empty );
			sql = $"SELECT {fields} FROM [DataContainer] WHERE [DataContainer].[GuidVar] IS NOT NULL";
			Assert.That( q.GeneratedQuery.StartsWith( sql ) );
		}

		[Test]
		public void QueryWithDateTimeMinValueParameterSearchesForNull()
		{
			// The query will fetch for DataContainerDerived objects, too.
			// Hence we test with "StartsWith", because the query contains additional text, which doesn't matter here.
			var q = new NDOQuery<DataContainer>(pm, "dateTimeVar = {0}");
			q.Parameters.Add( DateTime.MinValue );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( DataContainer ) ) ).SelectList;
			var sql = $"SELECT {fields} FROM [DataContainer] WHERE [DataContainer].[DateTimeVar] IS NULL";
			Assert.That( q.GeneratedQuery.StartsWith( sql ) );

			q = new NDOQuery<DataContainer>(pm, "dateTimeVar <> {0}");
			q.Parameters.Add( DateTime.MinValue );
			sql = $"SELECT {fields} FROM [DataContainer] WHERE [DataContainer].[DateTimeVar] IS NOT NULL";
			Assert.That( q.GeneratedQuery.StartsWith( sql ) );

		}

		[Test]
		public void CheckIfQueryWithoutWhereClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter]", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfSimplePrefetchWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm );
			q.AddPrefetch( "dieReisen" );
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
			q = new NDOQuery<Mitarbeiter>( pm, "vorname LIKE 'M*'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] LIKE 'M*'", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "oid = 1" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = 1", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "oid(0) = 1" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] = 1", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfFunctionsWork()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = SqlFunction('Mirko')" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = SqlFunction('Mirko')", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "vorname = SqlFunction('Mirko', 42)" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = SqlFunction('Mirko', 42)", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "vorname = SqlFunction()" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = SqlFunction()", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "vorname = SqlFunction(nachname)" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = SqlFunction([Mitarbeiter].[Nachname])", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "SqlFunction('Mirko', 42) > 3124" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE SqlFunction('Mirko', 42) > 3124", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfGeneratedQueryCanBeCalledTwice()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = 'Mirko'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", this.mitarbeiterFields ), q.GeneratedQuery );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", this.mitarbeiterFields ), q.GeneratedQuery );
		}

		[Test]
		[TestCase( true )]
		[TestCase( false )]
		public void SkipTakeParametersDontChangeTheCoreQuery(bool asc)
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
			q.Parameters.Add( "Mirko" );
			if (asc)
				q.Orderings.Add( new AscendingOrder( "vorname" ) );
			else
				q.Orderings.Add( new DescendingOrder( "vorname" ) );
			q.Take = 10;
			var desc = asc ? "" : "DESC ";
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[Vorname] {desc}OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
			q.Skip = 10;
			q.Take = 10;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[Vorname] {desc}OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
		}

		[Test]
		public void MixedOrderingsWork()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
			q.Parameters.Add( "Mirko" );
			q.Orderings.Add( new AscendingOrder( "vorname" ) );
			q.Orderings.Add( new DescendingOrder( "nachname" ) );
			q.Take = 10;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[Vorname] ASC, [Mitarbeiter].[Nachname] DESC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
			q.Skip = 10;
			q.Take = 10;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[Vorname] ASC, [Mitarbeiter].[Nachname] DESC OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
		}

		[Test]
		public void OrderingByOidWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
			q.Parameters.Add( "Mirko" );
			q.Orderings.Add( new AscendingOrder( "oid" ) );
			q.Take = 10;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[ID] OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
			q.Skip = 10;
			q.Take = 10;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[ID] OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );

			q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
			q.Parameters.Add( "Mirko" );
			q.Orderings.Add( new DescendingOrder( "oid" ) );
			q.Take = 10;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[ID] DESC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
			q.Skip = 10;
			q.Take = 10;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[ID] DESC OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
		}

		[Test]
		[TestCase( true )]
		[TestCase( false )]
		public void ParametersChangesDontChangeTheCoreQuery(bool asc)
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
			if (asc)
				q.Orderings.Add( new AscendingOrder( "vorname" ) );
			else
				q.Orderings.Add( new DescendingOrder( "vorname" ) );
			q.Take = 10;
			q.Parameters.Add( "Mirko" );
			var desc = asc ? "" : "DESC ";
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[Vorname] {desc}OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
			q.Parameters.Add( "Hans" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = {{0}} ORDER BY [Mitarbeiter].[Vorname] {desc}OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", q.GeneratedQuery );
		}

		[Test]
		public void CheckIfWhereClauseWith1nRelationWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.zweck = 'ADC'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = 'ADC'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.oid = {0}" );
			q.Parameters.Add( 1 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] = {{0}}", this.mitarbeiterJoinFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfWhereClauseWith11RelationWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse.lkz LIKE 'D%'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE 'D%'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckIfMultipleRelationsWork()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse.lkz LIKE 'D%' AND dieReisen.dieLaender.name = 'D'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] INNER JOIN [Land] ON [Land].[ID] = [relLandReise].[IDLand] WHERE [Adresse].[Lkz] LIKE 'D%' AND [Land].[Name] = 'D'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckOidWithTable()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.dieLaender.oid = {0}" );
			q.Parameters.Add( 1 );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] = {{0}}", this.mitarbeiterJoinFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckThatOneJoinAppearsOnlyOneTime()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse.lkz LIKE 'D%' AND adresse.ort <> 'Bad Tölz'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Adresse] ON [Adresse].[ID] = [Mitarbeiter].[IDAdresse] WHERE [Adresse].[Lkz] LIKE 'D%' AND [Adresse].[Ort] <> 'Bad Tölz'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.zweck = 'ADC' OR dieReisen.zweck = 'ADW'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = 'ADC' OR [Reise].[Zweck] = 'ADW'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
		}

		[Test]
		public void CheckNotOperator()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "NOT (vorname LIKE 'M%')" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE NOT ([Mitarbeiter].[Vorname] LIKE 'M%')", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "vorname LIKE 'M%' AND NOT nachname = 'Matytschak'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] LIKE 'M%' AND NOT [Mitarbeiter].[Nachname] = 'Matytschak'", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "NOT (vorname LIKE 'M%' AND nachname = 'Matytschak')" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] WHERE NOT ([Mitarbeiter].[Vorname] LIKE 'M%' AND [Mitarbeiter].[Nachname] = 'Matytschak')", this.mitarbeiterFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.zweck = 'ADC' OR NOT dieReisen.zweck = 'ADW'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[Zweck] = 'ADC' OR NOT [Reise].[Zweck] = 'ADW'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "NOT (dieReisen.zweck = 'ADC' OR dieReisen.zweck = 'ADW')" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE NOT ([Reise].[Zweck] = 'ADC' OR [Reise].[Zweck] = 'ADW')", this.mitarbeiterJoinFields ), q.GeneratedQuery );
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
			q = new NDOQuery<Mitarbeiter>( pm, "NOT vorname BETWEEN 'A' AND 'B'" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE NOT [Mitarbeiter].[Vorname] BETWEEN 'A' AND 'B'", q.GeneratedQuery );
			q = new NDOQuery<Mitarbeiter>( pm, "NOT (vorname BETWEEN 'A' AND 'B')" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE NOT ([Mitarbeiter].[Vorname] BETWEEN 'A' AND 'B')", q.GeneratedQuery );
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
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).Result( false, false, true );
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
		public void CheckIfSyntaxErrorThrowsAQueryException()
		{
			bool qeThrown = false;
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname IN 'Mirko'" );
			try
			{
				var s = q.GeneratedQuery;
			}
			catch (OqlExpressionException)
			{
				qeThrown = true;
			}

			Assert.That( qeThrown, "Syntax Error should throw an OqlExpressionException" );
		}

		[Test]
		public void CheckIfMultiKeysWork()
		{
			NDOQuery<OrderDetail> q = new NDOQuery<OrderDetail>( pm, "oid = {0}" );
			var od = pm.FindObject(typeof(OrderDetail), new object[]{ 1, 2 } );
			q.Parameters.Add( od.NDOObjectId );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( OrderDetail ) ) ).SelectList;
			Assert.AreEqual( $"SELECT {fields} FROM [OrderDetail] WHERE [OrderDetail].[IDProduct] = {{0}} AND [OrderDetail].[IDOrder] = {{1}}", q.GeneratedQuery );
			bool thrown = false;
			try
			{
				q = new NDOQuery<OrderDetail>( pm, "oid = -4" );
				string s = q.GeneratedQuery;
			}
			catch (NDOException)
			{
				thrown = true;
			}
			Assert.AreEqual( true, thrown );
		}

		[Test]
		public void CheckIfMultiKeyObjectIdParametersAreProcessed()
		{
			IList generatedParameters = null;
			Mock<IPersistenceHandler> handlerMock = new Mock<IPersistenceHandler>();
			handlerMock.Setup( h => h.PerformQuery( It.IsAny<string>(), It.IsAny<IList>(), It.IsAny<DataSet>() ) ).Returns( new DataTable() ).Callback<string, IList, DataSet>( ( s, l, d ) => generatedParameters = l );

			pm.ConfigContainer.RegisterInstance<IPersistenceHandler>( handlerMock.Object );

			NDOQuery<OrderDetail> q = new NDOQuery<OrderDetail>( pm, "oid = {0}" );
			ObjectId oid = pm.FindObject( typeof( OrderDetail ), new object[] { 1, 2 } ).NDOObjectId;
			q.Parameters.Add( oid );

			q.Execute();

			Assert.NotNull( generatedParameters );
			Assert.AreEqual( 2, generatedParameters.Count );
			Assert.AreEqual( 1, generatedParameters[0] );
			Assert.AreEqual( 2, generatedParameters[1] );
		}

		[Test]
		public void CheckIfMultiKeyArrayParametersAreProcessed()
		{
			NDOQuery<OrderDetail> q = new NDOQuery<OrderDetail>( pm, "oid = {0}" );
			q.Parameters.Add( new object[] { 1, 2 } );

			IList generatedParameters = null;
			Mock<IPersistenceHandler> handlerMock = new Mock<IPersistenceHandler>();
			handlerMock.Setup( h => h.PerformQuery( It.IsAny<string>(), It.IsAny<IList>(), It.IsAny<DataSet>() ) ).Returns( new DataTable() ).Callback<string, IList, DataSet>( ( s, l, d ) => generatedParameters = l );
			var container = pm.ConfigContainer;
			container.RegisterInstance<IPersistenceHandler>( handlerMock.Object );
			q.Execute();
			Assert.NotNull( generatedParameters );
			Assert.AreEqual( 2, generatedParameters.Count );
			Assert.AreEqual( 1, generatedParameters[0] );
			Assert.AreEqual( 2, generatedParameters[1] );
			container.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
		}


		[Test]
		public void CheckIfSingleKeyOidParameterIsProcessed()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "oid = {0}" );
			q.Parameters.Add( 1 );

			IList generatedParameters = null;
			Mock<IPersistenceHandler> handlerMock = new Mock<IPersistenceHandler>();
			handlerMock.Setup( h => h.PerformQuery( It.IsAny<string>(), It.IsAny<IList>(), It.IsAny<DataSet>() ) ).Returns( new DataTable() ).Callback<string, IList, DataSet>( ( s, l, d ) => generatedParameters = l );
			var handler = handlerMock.Object;
			Mock<IPersistenceHandlerManager> phManagerMock = new Mock<IPersistenceHandlerManager>();
			phManagerMock.Setup( m => m.GetPersistenceHandler( It.IsAny<Type>() ) ).Returns( handler ).Callback<Type>( ( pc ) => { Console.WriteLine("Test"); });
			var container = pm.ConfigContainer;
			container.RegisterInstance<IPersistenceHandlerManager>( phManagerMock.Object );
			q.Execute();
			Assert.NotNull( generatedParameters );
			Assert.AreEqual( 1, generatedParameters.Count );
			Assert.AreEqual( 1, generatedParameters[0] );
			container.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
		}

		[Test]
		public void CheckIfSqlQueryIsProcessed()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "SELECT * FROM Mitarbeiter WHERE ID = {0}", false, QueryLanguage.Sql );
			q.Parameters.Add( 1 );

			IList generatedParameters = null;
			string expression = null;
			Mock<IPersistenceHandler> handlerMock = new Mock<IPersistenceHandler>();
			handlerMock.Setup( h => h.PerformQuery( It.IsAny<string>(), It.IsAny<IList>(), It.IsAny<DataSet>() ) ).Returns( new DataTable() ).Callback<string, IList, DataSet>( ( s, l, d ) => { generatedParameters = l; expression = s; } );
			var handler = handlerMock.Object;
			Mock<IPersistenceHandlerManager> phManagerMock = new Mock<IPersistenceHandlerManager>();
			phManagerMock.Setup( m => m.GetPersistenceHandler( It.IsAny<Type>() ) ).Returns( handler );
			var container = pm.ConfigContainer;
			container.RegisterInstance<IPersistenceHandlerManager>( phManagerMock.Object );
			q.Execute();
			Assert.NotNull( generatedParameters );
			Assert.AreEqual( 1, generatedParameters.Count );
			Assert.AreEqual( 1, generatedParameters[0] );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM Mitarbeiter WHERE ID = {{0}}", expression );
			container.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
		}

		[Test]
		public void CheckIfSingleKeyNDOObjectIdParameterIsProcessed()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "oid = {0}" );
			var dummy = (IPersistenceCapable) pm.FindObject( typeof( Mitarbeiter ), 121 );
			q.Parameters.Add( dummy.NDOObjectId );

			IList generatedParameters = null;
			Mock<IPersistenceHandler> handlerMock = new Mock<IPersistenceHandler>();
			handlerMock.Setup( h => h.PerformQuery( It.IsAny<string>(), It.IsAny<IList>(), It.IsAny<DataSet>() ) ).Returns( new DataTable() ).Callback<string, IList, DataSet>( ( s, l, d ) => generatedParameters = l );
			var handler = handlerMock.Object;
			Mock<IPersistenceHandlerManager> phManagerMock = new Mock<IPersistenceHandlerManager>();
			phManagerMock.Setup( m => m.GetPersistenceHandler( It.IsAny<Type>() ) ).Returns( handler ).Callback<Type>( ( pc ) => { Console.WriteLine( "Test" ); } );
			var container = pm.ConfigContainer;
			container.RegisterInstance<IPersistenceHandlerManager>( phManagerMock.Object );
			q.Execute();
			Assert.NotNull( generatedParameters );
			Assert.AreEqual( 1, generatedParameters.Count );
			Assert.AreEqual( 121, generatedParameters[0] );
			container.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
		}

		[Test]
		public void SimpleQueryWithHandler()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm );

			IList generatedParameters = null;
			Mock<IPersistenceHandler> handlerMock = new Mock<IPersistenceHandler>();
			handlerMock.Setup( h => h.PerformQuery( It.IsAny<string>(), It.IsAny<IList>(), It.IsAny<DataSet>() ) ).Returns( new DataTable() ).Callback<string, IList, DataSet>( ( s, l, d ) => generatedParameters = l );
			var handler = handlerMock.Object;
			Mock<IPersistenceHandlerManager> phManagerMock = new Mock<IPersistenceHandlerManager>();
			phManagerMock.Setup( m => m.GetPersistenceHandler( It.IsAny<Type>() ) ).Returns( handler ).Callback<Type>( ( pc ) => { Console.WriteLine( "Test" ); } );
			var container = pm.ConfigContainer;
			container.RegisterInstance<IPersistenceHandlerManager>( phManagerMock.Object );
			q.Execute();
			Assert.NotNull( generatedParameters );
			Assert.AreEqual( 0, generatedParameters.Count );
			container.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
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
			q.Parameters.Add( DateTime.Now );
			Assert.AreEqual( $"SELECT {reiseJoinFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [Beleg] ON [Beleg].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 926149172 WHERE [Beleg].[Datum] = {{0}} UNION \r\nSELECT {reiseJoinFields} FROM [Reise] INNER JOIN [relBelegKostenpunkt] ON [Reise].[ID] = [relBelegKostenpunkt].[IDReise] INNER JOIN [PKWFahrt] ON [PKWFahrt].[ID] = [relBelegKostenpunkt].[IDBeleg] AND [relBelegKostenpunkt].[TCBeleg] = 734406058 WHERE [PKWFahrt].[Datum] = {{0}}", q.GeneratedQuery );
		}

		[Test]
		public void Test1To1()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "meinBuero.zimmer = 'abc'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Buero] ON [Buero].[ID] = [Mitarbeiter].[IDBuero] WHERE [Buero].[Zimmer] = 'abc'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
		}


		[Test]
		public void Test1To1Bidirectional()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "sn.nummer = 4711" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [Sozialversicherungsnummer] ON [Sozialversicherungsnummer].[ID] = [Mitarbeiter].[IDSozial] WHERE [Sozialversicherungsnummer].[Nummer] = 4711", this.mitarbeiterJoinFields ), q.GeneratedQuery );
			NDOQuery<Sozialversicherungsnummer> qs = new NDOQuery<Sozialversicherungsnummer>( pm, "arbeiter.vorname = 'Mirko'" );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Sozialversicherungsnummer ) ) ).Result( false, false, true );
			Assert.AreEqual( $"SELECT {fields} FROM [Sozialversicherungsnummer] INNER JOIN [Mitarbeiter] ON [Mitarbeiter].[ID] = [Sozialversicherungsnummer].[IDSozial] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", qs.GeneratedQuery );
		}

		[Test]
		public void Test1To1BiWithTable()
		{
			NDOQuery<Zertifikat> qz = new NDOQuery<Zertifikat>( pm, "sgn.signature = 'abc'" );
			var fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Zertifikat ) ) ).Result( false, false, true );
			Assert.AreEqual( $"SELECT {fields} FROM [Zertifikat] INNER JOIN [relSignaturZertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] INNER JOIN [Signatur] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] WHERE [Signatur].[Signature] = 'abc'", qz.GeneratedQuery );
			NDOQuery<Signatur> qs = new NDOQuery<Signatur>( pm, "owner.schlüssel = -4" );
			string s = qs.GeneratedQuery;
			fields = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( Signatur ) ) ).Result( false, false, true );
			Assert.AreEqual( $"SELECT {fields} FROM [Signatur] INNER JOIN [relSignaturZertifikat] ON [Signatur].[ID] = [relSignaturZertifikat].[IDSignatur] INNER JOIN [Zertifikat] ON [Zertifikat].[ID] = [relSignaturZertifikat].[IDZertifikat] WHERE [Zertifikat].[Schlüssel] = -4", qs.GeneratedQuery );
		}

		[Test]
		public void Test1ToNWithTable()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "reiseBüros.name = 'abc'" );
			Assert.AreEqual( String.Format( "SELECT {0} FROM [Mitarbeiter] INNER JOIN [relMitarbeiterReisebuero] ON [Mitarbeiter].[ID] = [relMitarbeiterReisebuero].[IDMitarbeiter] INNER JOIN [Reisebuero] ON [Reisebuero].[ID] = [relMitarbeiterReisebuero].[IDReisebuero] WHERE [Reisebuero].[Name] = 'abc'", this.mitarbeiterJoinFields ), q.GeneratedQuery );
		}

		[Test]
		public void TestIfQueryForNonNullOidsWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.dieLaender.oid IS NOT NULL" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] INNER JOIN [relLandReise] ON [Reise].[ID] = [relLandReise].[IDReise] WHERE [relLandReise].[IDLand] IS NOT NULL", q.GeneratedQuery );
		}

		[Test]
		public void TestIfQueryWithNonNullRelationWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse IS NOT NULL" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[IDAdresse] IS NOT NULL", q.GeneratedQuery );
		}

		[Test]
		public void TestIfQueryWithNullRelationWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "adresse IS NULL" );
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[IDAdresse] IS NULL", q.GeneratedQuery );
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
		public void TestIfSingleQuotesAreForbidden()
		{
			// -- will be interpreted as NDOql comment
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname = 'lala'--SELECT * FROM Mitarbeiter'" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = 'lala'", s );

			// -SELECT * FROM is a valid expression, it would result in a sql error
			// Mitarbeiter is a syntax error, so the expression is recognized as incorrect from the 33rd character onwards
			q = new NDOQuery<Mitarbeiter>( pm, "vorname = 'lala' -SELECT * FROM Mitarbeiter'" );
			try
			{
				s = q.GeneratedQuery;
			}
			catch (OqlExpressionException)
			{
				// Expected outcome
			}
		}

		[Test]
		public void TestIfInClauseWithQuotesInStringsWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "vorname IN ('1''','2''3','''3','4','5')" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] IN ('1''', '2''3', '''3', '4', '5')", s );
		}

		[Test]
		public void TestIfRelationInInClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "dieReisen.oid IN (1,2,3,4,5)" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterJoinFields} FROM [Mitarbeiter] INNER JOIN [Reise] ON [Mitarbeiter].[ID] = [Reise].[IDMitarbeiter] WHERE [Reise].[ID] IN (1, 2, 3, 4, 5)", s );
		}

		[Test]
		public void TestIfOidWithInClauseWorks()
		{
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( pm, "oid IN (1,2,3,4,5)" );
			var s = q.GeneratedQuery;
			Assert.AreEqual( $"SELECT {this.mitarbeiterFields} FROM [Mitarbeiter] WHERE [Mitarbeiter].[ID] IN (1, 2, 3, 4, 5)", s );
		}

		[Test]
		public void TestIfSqlCodeWorks()
		{
			var expression = "SELECT * FROM Mitarbeiter WHERE Vorname = 'Mirko'";
			NDOQuery<Mitarbeiter> q = new NDOQuery<Mitarbeiter>( this.pm, expression, false, QueryLanguage.Sql );
			Assert.AreEqual( expression, q.GeneratedQuery );
		}

		[Test]
		public void GreaterEqualsWorks()
		{
			var query = new NDOQuery<Mitarbeiter>( NDOFactory.Instance.PersistenceManager, "vorname >= {0} AND vorname < {1}" );
			query.Parameters.Add( "ab" );
			query.Parameters.Add( "abc" );
			query.Orderings.Add( new NDO.Query.DescendingOrder( "vorname" ) );
			Assert.That(query.GeneratedQuery.IndexOf( "[Mitarbeiter].[Vorname] >= {0}" ) > -1);
			Assert.That( query.GeneratedQuery.IndexOf( "[Mitarbeiter].[Vorname] < {1}" ) > -1 );
		}

		[Test]
		public void BitwiseOperatorsWork()
		{
			var pm = NDOFactory.Instance.PersistenceManager;
			var query = new NDOQuery<Buero>( pm, "Nummer & 2 = 0" );
			Assert.That( query.GeneratedQuery.IndexOf( "[Buero].[Nummer] & 2 = 0" ) > -1 );
			query = new NDOQuery<Buero>( pm, "Nummer | 2 = 0" );
			Assert.That( query.GeneratedQuery.IndexOf( "[Buero].[Nummer] | 2 = 0" ) > -1 );
		}

		[Test]
		public void DirectDeleteQueryIsCorrect()
		{
			var q = new NDOQuery<Mitarbeiter>( NDOFactory.Instance.PersistenceManager, "vorname = 'Mirko'" );
			var sql = q.GetDirectDeleteQuery();
			Assert.AreEqual( "DELETE FROM [Mitarbeiter] WHERE [Mitarbeiter].[Vorname] = 'Mirko'", sql );
		}

		[Test]
		public void TransientDelete()
		{
			var pm = NDOFactory.Instance.PersistenceManager;
			var m = new Mitarbeiter();
			try
			{
				pm.DeleteAsync( m );
			}
			catch (NDOException ex)
			{
				Assert.AreEqual( 120, ex.ErrorNumber );
			}
		}
	}
}
