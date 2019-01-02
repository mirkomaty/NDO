using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOql;
using NDOql.Expressions;
using NDO.Mapping;
using System.Data;
using NDOInterfaces;
using System.Text.RegularExpressions;
using NDO.Linq;
using LE=System.Linq.Expressions;
using Unity;
using NDO.SqlPersistenceHandling;

namespace NDO.Query
{
	internal class FieldMarker
	{
		public static string Instance
		{
			get { return "##FIELDS##"; }
		}
	}

	/// <summary>
	/// This class manages string based queries in NDOql and Sql
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NDOQuery<T> : IQuery
	{
		private PersistenceManager pm;
		private string queryExpression;
		private bool hollowResults;
		private QueryLanguage queryLanguage;
		private Type resultType = typeof( T );
		private bool allowSubclasses = true;
		private OqlExpression expressionTree;
		private List<QueryOrder> orderings = new List<QueryOrder>();
		private List<QueryContextsEntry> queryContextsForTypes = null;
		private Mappings mappings;
		private List<object> parameters = new List<object>();
		private List<string> prefetches = new List<string>();
		private int skip;
		private int take;

		/// <summary>
		/// Constructs a NDOQuery object
		/// </summary>
		/// <param name="pm">The PersistenceManager used to manage the objects.</param>
		/// <remarks>
		/// The query will return all available objects of the given type and it's subtypes. 
		/// The objects won't be hollow.
		/// </remarks>
		public NDOQuery( PersistenceManager pm )
			: this( pm, null )
		{
		}

		/// <summary>
		/// Constructs a NDOQuery object
		/// </summary>
		/// <param name="pm">The PersistenceManager used to manage the objects.</param>
		/// <param name="queryExpression">A NDOql expression.</param>
		/// <remarks>
		/// The query will return all available objects of the given 
		/// type and it's subtypes where the expression applies. 
		/// The objects won't be hollow.
		/// </remarks>
		public NDOQuery( PersistenceManager pm, string queryExpression )
			: this( pm, queryExpression, false )
		{
		}

		/// <summary>
		/// Constructs a NDOQuery object
		/// </summary>
		/// <param name="pm">The PersistenceManager used to manage the objects.</param>
		/// <param name="queryExpression">A NDOql expression.</param>
		/// <param name="hollowResults">Determines, if the objects should be fetched in the hollow state (true) or filled with data (false).</param>
		public NDOQuery( PersistenceManager pm, string queryExpression, bool hollowResults )
			: this( pm, queryExpression, hollowResults, QueryLanguage.NDOql )
		{
		}

		/// <summary>
		/// Constructs a NDOQuery object
		/// </summary>
		/// <param name="pm">The PersistenceManager used to manage the objects.</param>
		/// <param name="queryExpression">A NDOql or SQL expression.</param>
		/// <param name="hollowResults">Determines, if the objects should be fetched in the hollow state (true) or filled with data (false).</param>
		/// <param name="queryLanguage">Determines, if the query is a SQL pass-through query or a NDOql expression.</param>
		public NDOQuery( PersistenceManager pm, string queryExpression, bool hollowResults, QueryLanguage queryLanguage )
		{
			this.pm = pm;
			if (pm == null)
				throw new ArgumentException( "Parameter is null", "pm" );
			this.mappings = pm.mappings;
			this.queryExpression = queryExpression;
			this.hollowResults = hollowResults;
			this.queryLanguage = queryLanguage;
		}

		/// <summary>
		/// Execute an aggregation query.
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <param name="keySelector">A Lambda expression which represents an accessor property of the field which shoule be aggregated.</param>
		/// <param name="aggregateType">One of the <see cref="AggregateType">AggregateType</see> enum members.</param>
		/// <returns>A single value, which represents the aggregate</returns>
		/// <remarks>Before using this function, make shure, that your database product supports the aggregate function, as defined in aggregateType.
		/// Polymorphy: StDev and Var only return the aggregate for the given class. All others return the aggregate for all subclasses.
		/// Transactions: Please note, that the aggregate functions always work against the database.
		/// Unsaved changes in your objects are not recognized.</remarks>
		public object ExecuteAggregate<K>( LE.Expression<Func<T, K>> keySelector, AggregateType aggregateType )
		{
			ExpressionTreeTransformer transformer =
				new ExpressionTreeTransformer( keySelector );
			string field = transformer.Transform();
			return ExecuteAggregate( field, aggregateType );
		}

		/// <summary>
		/// Execute an aggregation query.
		/// </summary>
		/// <param name="aggregateType">One of the <see cref="AggregateType">AggregateType</see> enum members.</param>
		/// <returns>A single value, which represents the aggregate</returns>
		/// <remarks>Before using this function, make sure, that your database product supports the aggregate function, as defined in aggregateType.
		/// Polymorphy: StDev and Var only return the aggregate for the given class. All others return the aggregate for all subclasses.
		/// Transactions: Please note, that the aggregate functions always work against the database.
		/// Unsaved changes in your objects are not recognized.</remarks>
		public object ExecuteAggregate( AggregateType aggregateType )
		{
			return ExecuteAggregate( "*", aggregateType );
		}

		/// <summary>
		/// Execute an aggregation query.
		/// </summary>
		/// <param name="field">The field, which should be aggregated</param>
		/// <param name="aggregateType">One of the <see cref="AggregateType">AggregateType</see> enum members.</param>
		/// <returns>A single value, which represents the aggregate</returns>
		/// <remarks>Before using this function, make sure, that your database product supports the aggregate function, as defined in aggregateType.
		/// Polymorphy: StDev and Var only return the aggregate for the given class. All others return the aggregate for all subclasses.
		/// Transactions: Please note, that the aggregate functions always work against the database.
		/// Unsaved changes in your objects are not recognized.</remarks>
		public object ExecuteAggregate( string field, AggregateType aggregateType )
		{
			if (aggregateType == AggregateType.StDev || aggregateType == AggregateType.Var)
				this.allowSubclasses = false;
			if (this.queryContextsForTypes == null)
				GenerateQueryContexts();

			object[] partResults = new object[this.queryContextsForTypes.Count];

			AggregateFunction func = new AggregateFunction( aggregateType );

			int i = 0;
			foreach (var queryContextsEntry in this.queryContextsForTypes)
			{
				partResults[i++] = ExecuteAggregateQuery( queryContextsEntry, field, aggregateType );
			}
			this.pm.CheckEndTransaction( false );
			return func.ComputeResult( partResults );
		}

		/// <summary>
		/// Executes the query and returns a list of result objects.
		/// </summary>
		/// <returns></returns>
		public new List<T> Execute()
		{
			return GetResultList();
		}

		/// <summary>
		/// Retrieves the SQL code of a NDOql Query.
		/// </summary>
		public string GeneratedQuery
		{
			get
			{
				if (this.queryLanguage == QueryLanguage.Sql)
				{
					return this.queryExpression;
				}

				if (this.queryContextsForTypes == null)
					GenerateQueryContexts();

				IQueryGenerator queryGenerator = ConfigContainer.Resolve<IQueryGenerator>();
				return queryGenerator.GenerateQueryStringForAllTypes( this.queryContextsForTypes, this.expressionTree, this.hollowResults, this.orderings, this.skip, this.take );
			}
		}

		private List<T> GetResultList()
		{
			List<T> result = new List<T>();

			if (this.queryContextsForTypes == null)
				GenerateQueryContexts();

			// this.pm.CheckTransaction happens in ExecuteOrderedSubQuery or in ExecuteSubQuery

			if (this.queryContextsForTypes.Count > 1 && this.orderings.Count > 0)
			{
				result = QueryOrderedPolymorphicList();
			}
			else
			{
				foreach (var queryContextsEntry in this.queryContextsForTypes)
				{
					foreach (var item in ExecuteSubQuery( queryContextsEntry ))
					{
						result.Add( item );
					}
				}
			}

			GetPrefetches( result );
			this.pm.CheckEndTransaction( false );
			if (!this.pm.GetClass( resultType ).Provider.SupportsFetchLimit)
			{
				List<T> fetchResult = new List<T>();
				for (int i = this.skip; i < Math.Min( result.Count, i + this.take ); i++)
					fetchResult.Add( result[i] );
			}
			return result;
		}

		Type GetPrefetchResultType( Type t, string relation )
		{
			string[] parts = relation.Split( '.' );
			Class cl = pm.GetClass( t );
			Relation rel = cl.FindRelation( parts[0] );
			if (parts.Length == 1)
				return rel.ReferencedType;
			else
				return GetPrefetchResultType( rel.ReferencedType,
					relation.Substring( relation.IndexOf( '.' ) + 1 ) );
		}

		void GetPrefetches( List<T> parents )
		{
			var queryGenerator = this.ConfigContainer.Resolve<IQueryGenerator>();
			if (parents.Count == 0)
				return;

			var mustUseInClause = false;

			if (this.expressionTree != null)
				mustUseInClause = this.expressionTree.GetAll( n => n.Operator == "IN" ).Any();

			foreach (string prefetch in this.Prefetches)
			{
				Type t = GetPrefetchResultType( this.resultType, prefetch );
				IPersistenceHandler persistenceHandler = this.pm.PersistenceHandlerManager.GetPersistenceHandler( t, this.pm.HasOwnerCreatedIds );
				persistenceHandler.VerboseMode = this.pm.VerboseMode;
				persistenceHandler.LogAdapter = this.pm.LogAdapter;

				foreach (var queryContextsEntry in this.queryContextsForTypes)
				{
					Type parentType = queryContextsEntry.Type;
					var parentCls = mappings.FindClass( parentType );
					var isMultiple = parentCls.Oid.OidColumns.Count > 1;
					var selectedParents = parents.Where( p => p.GetType() == parentType ).Select(p=>(IPersistenceCapable)p).ToList();
					if (selectedParents.Count == 0)
						continue;

					string generatedQuery;
					if (!mustUseInClause && (parents.Count > 100 || isMultiple) && this.skip == 0 && this.take == 0)
					{
						generatedQuery = queryGenerator.GenerateQueryString( queryContextsEntry, this.expressionTree, false, true, new List<QueryOrder>(), 0, 0, prefetch );
					}
					else
					{
						if (isMultiple)
							throw new QueryException( 10050, "Can't process a prefetch with skip, take & multiple oid columns" );
						generatedQuery = queryGenerator.GeneratePrefetchQuery( parentType, selectedParents, prefetch );
					}
#warning Überprüfen, ob das in der normalen Transaktion mitläuft
					//					this.pm.CheckTransaction( persistenceHandler, t );

					DataTable table = persistenceHandler.PerformQuery( generatedQuery, this.parameters );
					var result = pm.DataTableToIList( t, table.Rows, false );
					MatchRelations( parents, result, prefetch );
				}
			}
		}

		void MatchRelations( List<T> parents, IList childs, string relationName )
		{
			if (parents.Count == 0)
				return;
			if (childs.Count == 0)
				return;
			Class cl = pm.GetClass( resultType );
			Relation r = cl.FindRelation( relationName );
			RelationCollector rc = new RelationCollector( cl );
			rc.CollectRelations();
			string[] parentColumns = rc.ForeignKeyColumns.ToArray();
			cl = pm.GetClass( r.ReferencedType );
			rc = new RelationCollector( cl );
			rc.CollectRelations();
			string[] childColumns = rc.ForeignKeyColumns.ToArray();
			// Used to determine, if the relation has been collected
			string testColumnName = r.ForeignKeyColumns.First().Name;
			if (r.Multiplicity == RelationMultiplicity.Element && parentColumns.Contains( testColumnName ))
			{
				foreach (IPersistenceCapable parent in parents)
				{
					foreach (IPersistenceCapable child in childs)
					{
						if (parent.NDOLoadState.LostRowsEqualsOid( child.NDOObjectId.Id, r ))
							mappings.SetRelationField( parent, r.FieldName, child );
						//KeyValuePair kvp = ((KeyValueList)parent.NDOLoadState.LostRowInfo)[r.ForeignKeyColumnName];
						//if (kvp.Value.Equals(child.NDOObjectId.Id.Value))
						//    mappings.SetRelationField(parent, r.FieldName, child);
					}
				}
			}
			else if (r.Multiplicity == RelationMultiplicity.List && childColumns.Contains( testColumnName ))
			{
				foreach (IPersistenceCapable parent in parents)
				{
					IList container = mappings.GetRelationContainer( parent, r );
					foreach (IPersistenceCapable child in childs)
					{
						if (child.NDOLoadState.LostRowsEqualsOid( parent.NDOObjectId.Id, r ))
							container.Add( child );
					}

					parent.NDOSetLoadState( r.Ordinal, true );
				}
			}
		}


		private object ExecuteAggregateQuery( QueryContextsEntry queryContextsEntry, string field, AggregateType aggregateType )
		{
			Type t = queryContextsEntry.Type;
			IQueryGenerator queryGenerator = ConfigContainer.Resolve<IQueryGenerator>();
			string generatedQuery = queryGenerator.GenerateAggregateQueryString( field, queryContextsEntry, this.expressionTree, this.queryContextsForTypes.Count > 1, aggregateType );

			IPersistenceHandler persistenceHandler = this.pm.PersistenceHandlerManager.GetPersistenceHandler( t, this.pm.HasOwnerCreatedIds );
			persistenceHandler.VerboseMode = this.pm.VerboseMode;
			persistenceHandler.LogAdapter = this.pm.LogAdapter;
			this.pm.CheckTransaction( persistenceHandler, t );

			// Note, that we can't execute all subQueries in one batch, because
			// the subqueries could be executed against different connections.
			// TODO: This could be optimized, if we made clear whether the involved tables
			// can be reached with the same connection.
			var l = persistenceHandler.ExecuteBatch( new string[] { generatedQuery }, this.parameters );
			if (l.Count == 0)
				return null;

			return (l[0])["AggrResult"];
		}

		private List<T> ExecuteSqlQuery()
		{
			Type t = this.resultType;
			IPersistenceHandler persistenceHandler = this.pm.PersistenceHandlerManager.GetPersistenceHandler( t, this.pm.HasOwnerCreatedIds );
			persistenceHandler.VerboseMode = this.pm.VerboseMode;
			persistenceHandler.LogAdapter = this.pm.LogAdapter;
			this.pm.CheckTransaction( persistenceHandler, t );
			DataTable table = persistenceHandler.PerformQuery( this.queryExpression, this.parameters );
			return (List<T>)pm.DataTableToIList( t, table.Rows, this.hollowResults );
		}

		private bool PrepareParameters()
		{
			if (this.expressionTree == null)
				return false;
			var expressions = this.expressionTree.GetAll( e => e is ParameterExpression ).Select( e => (ParameterExpression)e ).ToList();
			if (expressions.Count == 0)
				return true;
			if (expressions[0].ParameterValue != null)
				return false;
			foreach (ParameterExpression item in expressions)
			{
				item.ParameterValue = this.parameters[item.Ordinal];
			}

			return true;
		}

		private void WriteBackParameters()
		{
			if (this.expressionTree == null)
				return;
			var expressions = this.expressionTree.GetAll( e => e is ParameterExpression ).Select( e => (ParameterExpression)e ).ToList();
			if (expressions.Count == 0)
				return;
			var count = (from e in expressions select e.Ordinal).Max();
			this.parameters = new List<object>( count );
			for (int i = 0; i < count + 1; i++)
				this.parameters.Add( null );
			foreach (ParameterExpression item in expressions)
			{
				this.parameters[item.Ordinal] = item.ParameterValue;
			}
		}

		private IList ExecuteSubQuery( Type t, QueryContextsEntry queryContextsEntry )
		{
			IQueryGenerator queryGenerator = ConfigContainer.Resolve<IQueryGenerator>();
			bool hasBeenPrepared = PrepareParameters();
			string generatedQuery;

			if (this.queryLanguage == QueryLanguage.NDOql)
				generatedQuery = queryGenerator.GenerateQueryString( queryContextsEntry, this.expressionTree, this.hollowResults, this.queryContextsForTypes.Count > 1, this.orderings, this.skip, this.take );
			else
				generatedQuery = (string)this.expressionTree.Value;

			if (hasBeenPrepared)
			{
				WriteBackParameters();
			}

			IPersistenceHandler persistenceHandler = this.pm.PersistenceHandlerManager.GetPersistenceHandler( t, this.pm.HasOwnerCreatedIds );
			persistenceHandler.VerboseMode = this.pm.VerboseMode;
			persistenceHandler.LogAdapter = this.pm.LogAdapter;
			this.pm.CheckTransaction( persistenceHandler, t );

			DataTable table = persistenceHandler.PerformQuery( generatedQuery, this.parameters );
			return pm.DataTableToIList( t, table.Rows, this.hollowResults );
		}

		private IEnumerable<T> ExecuteSubQuery( QueryContextsEntry queryContextsEntry )
		{
			var subResult = ExecuteSubQuery( queryContextsEntry.Type, queryContextsEntry );
			foreach (var item in subResult)
			{
				yield return (T)item;
			}
		}

		private List<T> QueryOrderedPolymorphicList()
		{
			List<ObjectRowPair<T>> rowPairList = new List<ObjectRowPair<T>>();
			foreach (var queryContextsEntry in this.queryContextsForTypes)
				rowPairList.AddRange( ExecuteOrderedSubQuery( queryContextsEntry ) );
			rowPairList.Sort();
			List<T> result = new List<T>( rowPairList.Count );
			foreach (ObjectRowPair<T> orp in rowPairList)
				result.Add( (T)orp.Obj );
			return result;
		}

		private List<ObjectRowPair<T>> ExecuteOrderedSubQuery( QueryContextsEntry queryContextsEntry )
		{
			Type t = queryContextsEntry.Type;
			Class resultSubClass = this.pm.GetClass( t );
			DataTable comparismTable = new DataTable( "ComparismTable" );
			foreach (QueryOrder order in this.orderings)
			{
				DataColumn col = comparismTable.Columns.Add( order.FieldName );
				if (order.IsAscending)
					col.AutoIncrementStep = 1;
				else
					col.AutoIncrementStep = -1;
			}

			IPersistenceHandler persistenceHandler = this.pm.PersistenceHandlerManager.GetPersistenceHandler( t, this.pm.HasOwnerCreatedIds );
			persistenceHandler.VerboseMode = this.pm.VerboseMode;
			persistenceHandler.LogAdapter = this.pm.LogAdapter;
			this.pm.CheckTransaction( persistenceHandler, t );

			bool hasBeenPrepared = PrepareParameters();
			IQueryGenerator queryGenerator = ConfigContainer.Resolve<IQueryGenerator>();
			string generatedQuery = queryGenerator.GenerateQueryString( queryContextsEntry, this.expressionTree, this.hollowResults, this.queryContextsForTypes.Count > 1, this.orderings, this.skip, this.take );

			if (hasBeenPrepared)
			{
				WriteBackParameters();
			}

			DataTable table = persistenceHandler.PerformQuery( generatedQuery, this.parameters );
			DataRow[] rows = table.Select();
			List<T> objects = (List<T>)pm.DataTableToIList( typeof( T ), rows, this.hollowResults );
			List<ObjectRowPair<T>> result = new List<ObjectRowPair<T>>( objects.Count );
			int i = 0;
			IProvider provider = mappings.GetProvider( t );
			foreach (T obj in objects)
			{
				DataRow row = rows[i++];
				DataRow newRow = comparismTable.NewRow();
				foreach (QueryOrder order in this.orderings)
				{
					string newColumnName = order.FieldName;
					if (!comparismTable.Columns.Contains( newColumnName ))
						throw new InternalException( 558, "Query.cs - Column not found." );
					string oldColumnName = resultSubClass.FindField( order.FieldName ).Column.Name;
					if (!table.Columns.Contains( oldColumnName ))
						throw new InternalException( 561, "Query.cs - Column not found." );
					newRow[newColumnName] = row[oldColumnName];
				}
				result.Add( new ObjectRowPair<T>( obj, newRow ) );
			}

			return result;
		}


		/// <summary>
		/// Executes the query and returns a single object.
		/// </summary>
		/// <returns>The fetched object or null, if the object wasn't found. If the query has more than one result, the first of the results will be returned.</returns>
		public new T ExecuteSingle()
		{
			return ExecuteSingle( false );
		}

		/// <summary>
		/// Executes the query and returns a single object.
		/// </summary>
		/// <param name="throwIfResultCountIsWrong"></param>
		/// <returns>The fetched object or null, if the object wasn't found and throwIfResultCountIsWrong is false.</returns>
		/// <remarks>
		/// If throwIfResultCountIsWrong is true, an Exception will be throwed, if the result count isn't exactly 1. 
		/// If throwIfResultCountIsWrong is false and the query has more than one result, the first of the results will be returned.
		/// </remarks>
		public new T ExecuteSingle( bool throwIfResultCountIsWrong )
		{
			var resultList = GetResultList();
			int count = resultList.Count;
			if (count == 1 || (!throwIfResultCountIsWrong && count > 0))
			{
				return resultList[0];
			}
			else
			{
				if (throwIfResultCountIsWrong)
					throw new QueryException( 10002, count.ToString() + " result objects in ExecuteSingle call" );
				else
					return default( T );
			}
		}

		/// <summary>
		/// Constructs the subqueries necessary to fetch all objects of a 
		/// class and its subclasses.
		/// </summary>
		/// <remarks>
		/// The function isn't actually recursive. The subclasses have been
		/// recursively collected in NDOMapping.
		/// </remarks>
		private void CreateQueryContextsForTypes()
		{
			Dictionary<string, Type> usedTables = new Dictionary<string, Type>();
			if (!resultType.IsAbstract)
			{
				Class cl = pm.GetClass( this.resultType );
				usedTables.Add( cl.TableName, cl.SystemType );
			}

			if (this.allowSubclasses)
			{
				// Check if subclasses are mapped to the same table.
				// Always fetch for the base class in the table
				foreach (Class cl in pm.GetClass( resultType ).Subclasses)
				{
					string tn = cl.TableName;
					if (usedTables.ContainsKey( tn ))
					{
						Type t = (Type)usedTables[tn];
						if (t.IsSubclassOf( cl.SystemType ))
						{
							usedTables.Remove( tn );
							usedTables.Add( tn, cl.SystemType );
						}
						break;
					}
					usedTables.Add( tn, cl.SystemType );
				}
			}

			var contextGenerator = ConfigContainer.Resolve<RelationContextGenerator>();
			this.queryContextsForTypes = new List<QueryContextsEntry>();
			// usedTables now contains all assignable classes of our result type
			foreach (var de in usedTables)
			{
				Type t2 = (Type)de.Value;
				// Now we have to iterate through all mutations of
				// polymorphic relations, used in the filter expression
				var queryContexts = contextGenerator.GetContexts( this.pm.GetClass( t2 ), this.expressionTree );
				this.queryContextsForTypes.Add( new QueryContextsEntry() { Type = t2, QueryContexts = queryContexts } );
			}
		}

		private void GenerateQueryContexts()
		{
			//if ((int) this.queryLanguage == OldQuery.LoadRelations)
			//	LoadRelatedTables();
			//else if (this.queryLanguage == Language.NDOql)
			if (this.queryLanguage == QueryLanguage.Sql)
			{
				var selectList = new SqlColumnListGenerator( pm.NDOMapping.FindClass( typeof( T ) ) ).SelectList;
				var sql = Regex.Replace( this.queryExpression, @"SELECT\s+\*", "SELECT " + selectList );
				this.expressionTree = new RawIdentifierExpression( sql, 0, 0 );
			}
			else
			{ 
				NDOql.OqlParser parser = new NDOql.OqlParser();
				var parsedTree = parser.Parse( this.queryExpression );
				if (parsedTree != null)
				{
					// The root expression tree might get exchanged.
					// To make this possible we make it the child of a dummy expression.
					this.expressionTree = new OqlExpression( 0, 0 );
					this.expressionTree.Add( parsedTree );
					((IManageExpression)parsedTree).SetParent( this.expressionTree );
				}
			}

			CreateQueryContextsForTypes();
		}

		IUnityContainer ConfigContainer
		{
			get { return this.pm.ConfigContainer; }
		}

		public ICollection<object> Parameters
		{
			get { return this.parameters; }
		}

		public bool AllowSubclasses
		{
			get { return this.allowSubclasses; }
			set { this.allowSubclasses = value; }
		}

		public IEnumerable<string> Prefetches
		{
			get { return this.prefetches; }
			set { this.prefetches = value.ToList(); }
		}

		public List<QueryOrder> Orderings
		{
			get { return this.orderings; }
			set { this.orderings = value.ToList(); }
		}

		/// <summary>
		/// Gets or sets the amount of elements to be skipped in an ordered query
		/// </summary>
		/// <remarks>This is for paging support.</remarks>
		public int Skip
		{
			get { return this.skip; }
			set { this.skip = value; }
		}

		/// <summary>
		/// Gets or sets the amount of elements to be taken in an ordered query
		/// </summary>
		/// <remarks>This is for paging support.</remarks>
		public int Take
		{
			get { return this.take; }
			set { this.take = value; }
		}

		/// <summary>
		/// Adds a prefetch to the query
		/// </summary>
		/// <param name="s"></param>
		public void AddPrefetch( string s )
		{
			this.prefetches.Add( s );
		}

		/// <summary>
		/// Removes a prefetch from the query
		/// </summary>
		/// <param name="s"></param>
		public void RemovePrefetch( string s )
		{
			if (this.prefetches.Contains( s ))
				this.prefetches.Remove( s );
		}

		#region IQuery Interface

		System.Collections.IList IQuery.Execute()
		{
			return this.Execute();
		}

		IPersistenceCapable IQuery.ExecuteSingle()
		{
			return (IPersistenceCapable)this.ExecuteSingle( false );
		}

		IPersistenceCapable IQuery.ExecuteSingle( bool throwIfResultCountIsWrong )
		{
			return (IPersistenceCapable)this.ExecuteSingle( throwIfResultCountIsWrong );
		}

		ICollection<object> IQuery.Parameters => this.parameters;
		ICollection<QueryOrder> IQuery.Orderings => this.orderings;
		object IQuery.ExecuteAggregate( string field, AggregateType aggregateType ) => ExecuteAggregate( field, aggregateType );
		bool IQuery.AllowSubclasses { get => this.allowSubclasses; set => this.allowSubclasses = value; }
		int IQuery.Skip { get => this.skip; set => this.skip = value; }
		int IQuery.Take { get => this.take; set => this.take = value; }
		string IQuery.GeneratedQuery { get { return this.GeneratedQuery; } }

		#endregion

	}
}
