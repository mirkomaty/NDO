using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOql;
using NDOql.Expressions;
using NDO.Mapping;
using System.Data;
using NDOInterfaces;
using System.Text.RegularExpressions;

namespace NDO.Query
{
	[Obsolete]
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
	public class NDOQuery<T> : IQuery //where T: IPersistenceCapable
	{
		private PersistenceManager pm;
		private string queryExpression;
		private bool hollowResults;
		private QueryLanguage queryLanguage;
		private List<QueryInfo> subQueries = new List<QueryInfo>();
		private Type resultType = typeof(T);
		private bool allowSubclasses = true;
		private OqlExpression expressionTree;
		private List<QueryOrder> orderings;

		private Mappings mappings;
		private List<object> parameters = new List<object>();
		private List<string> prefetches = new List<string>();
		public void Addstring(string s)
		{
			this.prefetches.Add(s);
		}
		public void Removestring(string s)
		{
			if (this.prefetches.Contains(s))
				this.prefetches.Remove(s);
		}
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
		/// <param name="field">The field, which should be aggregated</param>
		/// <param name="aggregateType">One of the <see cref="AggregateType">AggregateType</see> enum members.</param>
		/// <returns>A single value, which represents the aggregate</returns>
		/// <remarks>Before using this function, make shure, that your database product supports the aggregate function, as defined in aggregateType.
		/// Polymorphy: StDev and Var only return the aggregate for the given class. All others return the aggregate for all subclasses.
		/// Transactions: Please note, that the aggregate functions always work against the database.
		/// Unsaved changes in your objects are not recognized.</remarks>
		public object ExecuteAggregate(string field, AggregateType aggregateType)
		{
			if (aggregateType == AggregateType.StDev || aggregateType == AggregateType.Var)
				allowSubclasses = false;
			if (subQueries.Count == 0)
				GenerateQuery();

			object[] partResults = new object[subQueries.Count];

			AggregateFunction func = new AggregateFunction(aggregateType);

			int i = 0;
			foreach (QueryInfo qi in subQueries)
			{
				string query = qi.QueryString;
				Type t = qi.ResultType;
				Class cl = pm.GetClass(t);

                string column;
				if (field.ToLower().StartsWith("oid"))
				{
#warning !!!! Oid-Queries: Ändern !!!!!
                    Regex regex = new Regex(@"\(\s*(\d+)\s*\)");
                    Match match = regex.Match(field);
                    int index = 0;
                    if (match.Success)
                        index = int.Parse(match.Groups[1].Value);
                    column = ((OidColumn)cl.Oid.OidColumns[index]).Name;
				}
				else
				{
					column = pm.GetField(cl, field).Column.Name;
				}

				IProvider provider = mappings.GetProvider(cl);

				string table = QualifiedTableName.Get(cl.TableName, provider);
				column = table + "." + provider.GetQuotedName(column);

                query = query.Replace(FieldMarker.Instance, func.ToString() + "(" + column + ") as " + provider.GetQuotedName("AggrResult"));
                query = query.Replace(" DISTINCT", "");

				IPersistenceHandler persistenceHandler = mappings.GetPersistenceHandler(t, this.pm.HasOwnerCreatedIds);
				this.pm.CheckTransaction(persistenceHandler, t);
				// Note, that we can't execute all subQueries in one batch, because
				// the subqueries could be executed against different connections.
				System.Collections.IList l = persistenceHandler.ExecuteBatch(new string[]{query}, this.parameters);
				if (l.Count == 0)
					partResults[i] = null;
				else
				{
                    partResults[i] = ((System.Collections.Hashtable)l[0])["AggrResult"];
				}
				i++;					
			}
			this.pm.CheckEndTransaction(false);
			return func.ComputeResult(partResults);

		}



		/// <summary>
		/// Executes the query and returns a list of result objects.
		/// </summary>
		/// <returns></returns>
		public new List<T> Execute()
		{
			return GetResultList();
		}

		
		private List<T> GetResultList()
		{
			List<T> result = new List<T>();

			if (this.subQueries.Count == 0) // Query is not yet built
				GenerateQuery();

            // this.pm.CheckTransaction happens in ExecuteOrderedSubQuery or in ExecuteSubQuery
			if (this.subQueries.Count > 1 && this.orderings.Count > 0)
			{
				result = QueryOrderedPolymorphicList();
			}
			else
			{
				foreach (QueryInfo qi in subQueries)
				{
					foreach (var item in ExecuteSubQuery( qi.ResultType, qi.QueryString ))
					{
						result.Add( item );
					}
				}
			}
			GetPrefetches(result);
			this.pm.CheckEndTransaction(false);
			if (! this.pm.GetClass(resultType).Provider.SupportsFetchLimit)
			{
				List<T> fetchResult = new List<T>();
				for (int i = this.skip; i < Math.Min( result.Count, i + this.take ); i++ )
					fetchResult.Add( result[i] );
			}
			return result;
		}

		void GetPrefetches(List<T> parents)
		{
			//TODO: Das müssen wir überarbeiten

			//if (this.prefetches.Count == 0)
			//	return;

			//foreach(object o in this.prefetches)
			//{
			//	string pf = o.ToString() + ".oid";
			//	ArrayList pfNames = new ArrayList(this.names);
			//	pfNames.Add(pf);
			//	WherePartGenerator gen = new WherePartGenerator(this.mappings.GetProvider(resultType), 
			//		pm.GetClass(resultType),
			//		pfNames, this.tokens,
			//		this.mappings, new Hashtable(), this.pm.TypeManager);
			//	string condition = gen.ToString();
			//	Type t = GetPrefetchResultType(this.resultType, o.ToString());
			//	IProvider provider = this.mappings.GetProvider(t);
			//	Class cl = pm.GetClass(t);
			//	string tableNames = ExtractTableNames(condition, QualifiedTableName.Get(cl.TableName, provider), provider.GetQuotedName("yyy"));
			//	SelectPartGenerator sgen = new SelectPartGenerator(provider, cl, new ArrayList(), this.mappings, new Hashtable());				
			//	string select = sgen.ToString();
			//	if (select.IndexOf("SELECT DISTINCT") < 0)
			//		select = select.Replace("SELECT", "SELECT DISTINCT");
			//	select += "," + tableNames;
			//	this.hollowMode = false;
			//	IList result = ExecuteSubQuery(t, select + " " + condition);
			//	MatchRelations(parents, result, o.ToString());
			//}			
		}


		private List<T> ExecuteSubQuery(Type t, string generatedQuery)
		{
			IPersistenceHandler persistenceHandler = mappings.GetPersistenceHandler(t, this.pm.HasOwnerCreatedIds);
			this.pm.CheckTransaction(persistenceHandler, t);
			//TODO: eliminieren von dontTouch
			bool dontTouch = this.queryLanguage == QueryLanguage.Sql;
			DataTable table = persistenceHandler.SqlQuery(generatedQuery, this.hollowResults, dontTouch, parameters);
			return pm.DataTableToIList(t, table.Rows, this.hollowResults).Cast<T>().ToList();
		}


		private List<T> QueryOrderedPolymorphicList()
		{
			List<ObjectRowPair<T>> rowPairList = new List<ObjectRowPair<T>>();
			foreach (QueryInfo qi in subQueries)
				rowPairList.AddRange(ExecuteOrderedSubQuery(qi.ResultType, qi.QueryString));
			rowPairList.Sort();
			List<T> result = new List<T>(rowPairList.Count);
			foreach(ObjectRowPair<T> orp in rowPairList)
				result.Add((T)orp.Obj);
			return result;
		}

		private List<ObjectRowPair<T>> ExecuteOrderedSubQuery(Type t, string generatedQuery)
		{
			Class resultSubClass = this.pm.GetClass(t);
			DataTable comparismTable = new DataTable("ComparismTable");
			foreach(QueryOrder order in this.orderings)
			{
				DataColumn col = comparismTable.Columns.Add(order.FieldName);
				if (order.IsAscending)
					col.AutoIncrementStep = 1;
				else
					col.AutoIncrementStep = -1;
			}

			IPersistenceHandler persistenceHandler = this.mappings.GetPersistenceHandler(t, this.pm.HasOwnerCreatedIds);
			this.pm.CheckTransaction(persistenceHandler, t);

			bool dontTouch = this.queryLanguage == QueryLanguage.Sql;
			DataTable table = persistenceHandler.SqlQuery(generatedQuery, this.hollowResults, dontTouch, this.parameters);
			DataRow[] rows = table.Select();
			List<IPersistenceCapable> objects = pm.DataTableToIList(t, rows, this.hollowResults);
			List<ObjectRowPair<T>> result = new List<ObjectRowPair<T>>(objects.Count);
			int i = 0;
			IProvider provider = mappings.GetProvider(t);
			foreach(T obj in objects)
			{
				DataRow row = rows[i++];
				DataRow newRow = comparismTable.NewRow();
				foreach(QueryOrder order in this.orderings)
				{
					string newColumnName = order.FieldName;
					if (!comparismTable.Columns.Contains(newColumnName))
						throw new InternalException(558, "Query.cs - Column not found.");
					string oldColumnName = resultSubClass.FindField(order.FieldName).Column.Name;
					if (!table.Columns.Contains(oldColumnName))
						throw new InternalException(561, "Query.cs - Column not found.");
					newRow[newColumnName] = row[oldColumnName];
				}
				result.Add(new ObjectRowPair<T>(obj, newRow));
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
			return default( T );
		}

		/// <summary>
		/// Retrieves the SQL code of a NDOql Query.
		/// </summary>
		/// <remarks>Note that the query string doesn't show the actually used field names. They are built in the PersistenceHandler.</remarks>
		public string GeneratedQuery
		{
			get
			{
				StringBuilder generatedQuery = new StringBuilder();
				if (subQueries.Count == 0)
					GenerateQuery();
				foreach (QueryInfo qi in subQueries)
				{
					generatedQuery.Append(qi.QueryString);
					generatedQuery.Append(";\n");
				}

				generatedQuery.Length--; // the trailing \n

				if (subQueries.Count == 1)
					generatedQuery.Length--; // the semicolon

				return generatedQuery.ToString(); 
			}
		}

		private void CreateQueryContexts(Type t)
		{
			List<Dictionary<Relation, Class>> queryContexts = new RelationContextGenerator( this.pm.GetClass( t ), this.expressionTree, this.pm.NDOMapping ).GetContexts();
			if (queryContexts.Count == 0)
			{
				this.subQueries.Add( new QueryInfo( t, ConstructQueryString( t, new Dictionary<Relation, Class>() ) ) );
			}				
			else
			{
				string queryString = string.Empty;
				int added = 0;
				for (int i = 0; i < queryContexts.Count; i++)
				{
					Dictionary<Relation, Class> queryContext = (Dictionary<Relation, Class>) queryContexts[i];

					if (!queryContext.Any(kvp=>kvp.Value.SystemType == t))
					{
						if (added > 0)
							queryString += " UNION \r\n";
						queryString += ConstructQueryString(t, queryContext);
						added++;
					}
				}
				this.subQueries.Add(new QueryInfo(t, queryString));
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
		private void AddSubQueries()
		{
			Dictionary<string, Type> usedTables = new Dictionary<string,Type>();
			if (!resultType.IsAbstract)
			{
				Class cl = pm.GetClass(this.resultType);
				usedTables.Add(cl.TableName, cl.SystemType);				
			}

			if (this.allowSubclasses)
			{
				// Check if subclasses are mapped to the same table.
				// Always fetch for the base class in the table
				foreach(Class cl in pm.GetClass(resultType).Subclasses)
				{
					string tn = cl.TableName;
					if (usedTables.ContainsKey(tn))
					{
						Type t = (Type) usedTables[tn];
						if (t.IsSubclassOf(cl.SystemType))
						{
							usedTables.Remove(tn);
							usedTables.Add(tn, cl.SystemType);
						}
						break;
					}
					usedTables.Add(tn, cl.SystemType);
				}
			}

			// usedTables now contains all assignable classes of our result type
			foreach(var de in usedTables)
			{
				Type t2 = (Type) de.Value;
				// Now we have to iterate through all mutations of
				// polymorphic relations, used in the filter expression
				CreateQueryContexts(t2);
			}
		}

		private void GenerateQuery()
		{
			//if ((int) this.queryLanguage == OldQuery.LoadRelations)
			//	LoadRelatedTables();
			//else if (this.queryLanguage == Language.NDOql)
			if (this.queryLanguage == QueryLanguage.NDOql)
			{
				NDOql.OqlParser parser = new NDOql.OqlParser();
				this.expressionTree = parser.Parse( this.queryExpression );

				AddSubQueries();
			}
			else
			{
				subQueries.Add (new QueryInfo(typeof(T), this.queryExpression));
			}
		}

		string ConstructQueryString(Type resultType, Dictionary<Relation, Class> relationContext)
		{
			if (this.queryLanguage == QueryLanguage.Sql)
				return this.queryExpression;

			Class cls = this.pm.mappings.FindClass( resultType );

			// We need a string like the following:
			// SELECT <column_list> 
			//		FROM <table_name> { INNER JOIN <table_name> ON { <fk_column_name> = <oid_column_name> [AND] } }
			//		WHERE <where_clause>
			StringBuilder sb = new StringBuilder( "SELECT " );
			sb.Append( new ColumnListGenerator( cls, this.hollowResults ).GenerateColumnList( this.expressionTree ) );
			sb.Append( ' ' );
			sb.Append( new FromGenerator( cls, relationContext ).GenerateFromExpression( this.expressionTree ) );
			string where = new WhereGenerator( cls, relationContext ).GenerateWhereClause( this.expressionTree );
			if (where != string.Empty)
			{
				sb.Append( ' ' );
				sb.Append( where );
			}

			return sb.ToString();
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

		public int Skip
		{
			get { return this.skip; }
			set { this.skip = value; }
		}

		public int Take
		{
			get { return this.take; }
			set { this.take = value; }
		}

		System.Collections.IList IQuery.Execute()
		{
			return this.Execute();
		}

        IPersistenceCapable IQuery.ExecuteSingle()
        {
            return (IPersistenceCapable) this.ExecuteSingle();
        }

        IPersistenceCapable IQuery.ExecuteSingle(bool throwIfResultCountIsWrong)
        {
            return (IPersistenceCapable) this.ExecuteSingle(throwIfResultCountIsWrong);
        }
    }
}
