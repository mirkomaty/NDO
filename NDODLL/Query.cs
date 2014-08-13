//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
#if !NDO11
using System.Collections.Generic;
#endif
using System.Reflection;
using NDO.Mapping;
using NDOInterfaces;

namespace NDO
{

	/// <summary>
	/// Specialized Exception class to be used in the context of Queries
	/// </summary>
	public class QueryException : NDOException
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="errorNr">The unique NDO error number.</param>
		/// <param name="msg">Exception message.</param>
		/// <remarks>
		/// QueryException is derived from NDOException. The error numbers don't overlap 
		/// with the error numbers used in NDOException.
		/// </remarks>
		public QueryException(int errorNr, string msg) : base (errorNr, "Query Exception: " + msg)
		{
		}
	}

	internal class QueryInfo
	{
		public QueryInfo(Type resultType, string queryString)
		{
			this.QueryString = queryString;
			this.ResultType = resultType;
		}

		public string QueryString;
		public Type ResultType;
	}


#if !NDO11
	public class NDOQuery<T> : Query
	{
		/// <summary>
		/// Constructs a NDOQuery object
		/// </summary>
		/// <param name="pm">The PersistenceManager used to manage the objects.</param>
        /// <remarks>
        /// The query will return all available objects of the given type and it's subtypes. 
        /// The objects won't be hollow.
        /// </remarks>
        public NDOQuery(PersistenceManager pm)
			: base(typeof(T), null, false, (Mappings)pm.NDOMapping, Language.NDOql)
		{
			this.pm = pm;
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
        public NDOQuery(PersistenceManager pm, string queryExpression)
			: base(typeof(T), queryExpression, false, (Mappings)pm.NDOMapping, Language.NDOql)
		{
			this.pm = pm;
		}

        /// <summary>
        /// Constructs a NDOQuery object
        /// </summary>
        /// <param name="pm">The PersistenceManager used to manage the objects.</param>
        /// <param name="queryExpression">A NDOql expression.</param>
        /// <param name="hollowResults">Determines, if the objects should be fetched in the hollow state (true) or filled with data (false).</param>
        public NDOQuery(PersistenceManager pm, string queryExpression, bool hollowResults)
			: base(typeof(T), queryExpression, hollowResults, (Mappings)pm.NDOMapping, Language.NDOql)
		{
			this.pm = pm;
		}

        /// <summary>
        /// Constructs a NDOQuery object
        /// </summary>
        /// <param name="pm">The PersistenceManager used to manage the objects.</param>
        /// <param name="queryExpression">A NDOql or SQL expression.</param>
        /// <param name="hollowResults">Determines, if the objects should be fetched in the hollow state (true) or filled with data (false).</param>
        /// <param name="queryLanguage">Determines, if the query is a SQL pass-through query or a NDOql expression.</param>
		public NDOQuery(PersistenceManager pm, string queryExpression, bool hollowResults, Language queryLanguage)
			: base(typeof(T), queryExpression, hollowResults, (Mappings)pm.NDOMapping, queryLanguage)
		{
			this.pm = pm;
		}

		/// <summary>
		/// Executes the query and returns a list of result objects.
		/// </summary>
		/// <returns></returns>
        public new List<T> Execute()
		{
			IList result = base.Execute();
			List<T> genericResult = new List<T>(result.Count);
            for (int i = 0; i < result.Count; i++)
                genericResult.Add((T)result[i]);
			return genericResult;
		}

        /// <summary>
        /// Executes the query and returns a single object.
        /// </summary>
        /// <returns>The fetched object or null, if the object wasn't found. If the query has more than one result, the first of the results will be returned.</returns>
        public new T ExecuteSingle()
        {
            return (T)base.ExecuteSingle();
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
        public new T ExecuteSingle(bool throwIfResultCountIsWrong)
        {
            return (T)base.ExecuteSingle(throwIfResultCountIsWrong);
        }

	}
#endif

	/// <summary>
	/// This class is used to construct Queries in NDOql to retrieve defined sets of objects from the database.
	/// </summary>
	public class Query
	{

		public class Op
		{
			public const string Like = " LIKE ";
			public const string Eq = " = ";
			public new const string Equals = " = ";
			public const string Ne = " <> ";
			public const string NotEqual = " <> ";
			public const string And = " AND ";
			public const string Or = " OR ";
			public const string Not = " NOT ";
			public const string Gt = " > ";
			public const string GreaterThan = " > ";
			public const string Lt = " < ";
			public const string LowerThan = " < ";
			public const string Ge = " >= ";
			public const string GreaterEquals = " >= ";
			public const string Le = " <= ";
			public const string LowerEquals = " <= ";
			public const string IsNull = " IS NULL ";
			public const string IsNotNull = " IS NOT NULL ";
			public const string Between = " BETWEEN ";
			public const string True = " TRUE ";
			public const string Escape = " ESCAPE ";
			public const string Mult = " * ";
			public const string Div = " / ";
			public const string Add = " + ";
			public const string Sub = " - ";
			public const string BitOr = " | ";
			public const string BitAnd = " & ";
			public const string BitXor = " ^ ";
			public const string BitNot = " ~ ";
			public const string Mod = " % ";
			public const string Oid = "oid";
		}

		class ObjectRowPair : IComparable 
		{
			public ObjectRowPair(object o, DataRow r)
			{
				Obj = o;
				Row = r;
			}
			public object Obj;
			public DataRow Row;
			#region IComparable Member

			public int CompareTo(object obj)
			{
				if (!(obj is ObjectRowPair))
					throw new InternalException(96, "Query.cs");
				DataRow row1 = ((ObjectRowPair)obj).Row;
				foreach(DataColumn dc in row1.Table.Columns)
				{
					object own = Row[dc.ColumnName];
					object other = row1[dc];
					if (own == DBNull.Value && other == DBNull.Value)
						continue; // equal
					if (other == DBNull.Value)
						return (int) dc.AutoIncrementStep;  // null is always < other values
					if (own == DBNull.Value)
						return -((int) dc.AutoIncrementStep); // null is always < other values
					int cmp = ((IComparable)own).CompareTo(other);
					if (cmp != 0)
					{
						// In AutoIncrementStep is the Ordering.
						// 1=> Asc, -1 => Desc
						return cmp * (int) dc.AutoIncrementStep;
					}
				}
				return 0;  // Equal
			}

			#endregion
		}

		/// <summary>
		/// Constructs a Date/Time expression to be used in NDO queries.
		/// </summary>
		/// <param name="dt">A DateTime value.</param>
		/// <returns>A string expression, representing the DateTime value.</returns>
		public static string DateTime(DateTime dt)
		{
			return " FileTime(" + dt.ToFileTime().ToString() + ") ";
		}

		/// <summary>
		/// Constructs a Date/Time expression to be used in NDO queries.
		/// </summary>
		/// <param name="dt">A DateTime value, which will internally be converted into an UTC value.</param>
		/// <returns>A string expression, representing the DateTime value.</returns>
		public static string DateTimeUtc(DateTime dt)
		{
			return " FileTimeUtc(" + dt.ToFileTimeUtc().ToString() + ") ";
		}
		

		/// <summary>
		/// Constructs a string which can be used as parameter placeholder in a query.
		/// </summary>
		/// <param name="number">Number of the parameter.</param>
		/// <returns>Returns a string in the format {n}, where n is the given number.</returns>
		public static string Placeholder(int number)
		{
			return " {" + number.ToString() + "} ";
		}


		internal const string FieldMarker = "##FIELDS##";

		/// <summary>
		/// Used as parameter for ExecuteAggregate. 
		/// </summary>
		public enum AggregateType
		{
			/// <summary>
			/// Average value
			/// </summary>
			Avg,
			/// <summary>
			/// Minimum
			/// </summary>
			Min,
			/// <summary>
			/// Maximum
			/// </summary>
			Max,
			/// <summary>
			/// Count
			/// </summary>
			Count,
			/// <summary>
			/// Sum 
			/// </summary>
			Sum,
			/// <summary>
			/// Standard Deviation
			/// </summary>
			StDev,
			/// <summary>
			/// Variance
			/// </summary>
			Var
		}

		/// <summary>
		/// Type of query language used
		/// </summary>
		public enum Language
		{
			/// <summary>
			/// use NDOql
			/// </summary>
			NDOql,
			/// <summary>
			/// use SQL (Professional Edition only)
			/// </summary>
			Sql 
		}


		internal const int LoadRelations = 1024;

		IList orderings = new ArrayList();
		NDOParameterCollection parameters = new NDOParameterCollection();
		ArrayList prefetches = new ArrayList();
		string filter;
		ArrayList tokens = new ArrayList();
		ArrayList names = new ArrayList();
		Language queryLanguage;
		bool allowSubclasses = true;
		int subclassCount; // temporary var, to check, if manual ordering is necessary
		int skip;
		int take;

		/// <summary>
		/// Type of object to be retrieved
		/// </summary>
		protected Type resultType;
		/// <summary>
		/// Determines, whether only object ids will be fetched
		/// </summary>
		protected bool hollowMode;

		/// <summary>
		/// Each query can cause several SQL subqueries. The generated subqueries are
		/// in this ArrayList.
		/// </summary>
		IList subQueries = new ArrayList();

		/// <summary>
		/// Retrieves the SQL code of a NDOql Query.
		/// </summary>
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
				return generatedQuery.ToString().Replace(Query.FieldMarker, "*"); 
			}

			//		    set { generatedQuery = value; }
		}

		private Mappings mappings;

		/// <summary>
		/// Persistence manager, which this query object was constructed by
		/// </summary>
		protected PersistenceManager pm;

		/// <summary>
		/// Only used by the NDO framework
		/// </summary>
		internal PersistenceManager PersistenceManager
		{
			get { return pm; }
			set { pm = value; }
		}


		/// <summary>
		/// Constructor; only used by the framework. Use IPersistenceManager.NewQuery to instantiate new Queries.
		/// </summary>
		/// <param name="rt">Result type of the query</param>
		/// <param name="fi">Filter string in NDOql</param>
		/// <param name="hollow">Determines if only the Ids are fetched from the database</param>
		/// <param name="mappings">Mapping information</param>
		/// <param name="language">Determines if SQL or NDOql is used.</param>
		internal Query(Type rt, string fi, bool hollow, Mappings mappings, Query.Language language)
		{
			resultType = rt;
			//			this.provider = mappings.GetProvider(rt);
			//			if (provider == null)
			//				throw new QueryxxException("Kein Provider mit der ID " + conn.Type + " - prüfen Sie die Connection-Angaben für den Typ " + rt.FullName);
			this.filter = fi;
			this.hollowMode = hollow;
			this.mappings = mappings;
			this.queryLanguage = language;
		}

		
		/*
			SELECT Teilnehmer.* FROM Teilnehmer, Buchungen where Teilnehmer.Nummer = Buchungen.IDTeilnehmer and Buchungen.EarlyBird = true;			
			SELECT Buchungen.* FROM Teilnehmer, Buchungen where Teilnehmer.Nummer = Buchungen.IDTeilnehmer and Teilnehmer.TNachname='Zech';
			SELECT Reise.* FROM Mitarbeiter, Reise where Mitarbeiter.ID = Reise.IDMitarbeiter and Mitarbeiter.Nachname='Heege';

		SELECT Teilnehmer.*
		FROM Teilnehmer, refTeilnehmerHauptveranstaltungen, Hauptveranstaltungen
		WHERE (((refTeilnehmerHauptveranstaltungen.IDTeilnehmer)=[Teilnehmer].[Nummer]) AND ((Hauptveranstaltungen.Name)="ADC 2002") AND ((refTeilnehmerHauptveranstaltungen.IDVeranstaltung)=[Hauptveranstaltungen].[IDVeranstaltung]));			
		*/




		private void CreateQueryContexts(Type t)
		{
			ArrayList queryContexts = new QueryContextGenerator(this.pm.GetClass(t), this.names, this.mappings).GetContexts();
			if (queryContexts.Count == 0)
			{
				this.subQueries.Add(new QueryInfo(t, ConstructQueryString(t, new Hashtable())));
			}
			else
			{
				string queryString = string.Empty;
				int added = 0;
				for (int i = 0; i < queryContexts.Count; i++)
				{
					Hashtable queryContext = (Hashtable) queryContexts[i];
					bool found = false;
					foreach(DictionaryEntry de in queryContext)
					{
						if (((Class)de.Value).SystemType == t)
						{
							found = true;
							break;
						}
					}
					if (!found)
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
		private void AddSubQueryRecursive()
		{
			Hashtable usedTables = new Hashtable();
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
					if (usedTables.Contains(tn))
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
			this.subclassCount = usedTables.Count;
			// usedTables now contains all assignable classes of our result type
			foreach(DictionaryEntry de in usedTables)
			{
				Type t2 = (Type) de.Value;
				// Now we have to iterate through all mutations of
				// polymorphic relations, used in the filter expression
				CreateQueryContexts(t2);
			}
		}

		/// <summary>
		/// Get the ORDER BY clause of the SQL statement
		/// </summary>
		/// <returns>Order By clause string</returns>
		private string GetOrderPart(Class cl)
		{
			if (0 == orderings.Count)
				return string.Empty;

			string sql = " ORDER BY ";

			foreach (Order o in orderings)
			{
				sql += o.ToString(cl) + ",";
			}

			sql = sql.Substring(0, sql.Length - 1);

			if ((this.skip != 0 || this.take != 0) && cl.Provider.SupportsFetchLimit)
				sql += " " + cl.Provider.FetchLimit( skip, take );

			return sql;
		}


		private string ConstructQueryString(Type t, Hashtable queryContext)
		{
			IProvider provider = mappings.GetProvider(t);
			Class resultClass = pm.GetClass(t);

			string select = new SelectPartGenerator(provider, resultClass, names, mappings, queryContext).ToString() + " " 
				+ new WherePartGenerator(provider, resultClass, names, tokens, mappings, queryContext).ToString() + " " 
				+ GetOrderPart(resultClass); 
			// Generate the column names, if there are orderings
			if (this.hollowMode && this.orderings.Count > 0 && this.subclassCount > 1)
			{
				ColumnListGenerator gen = new ColumnListGenerator(provider, resultClass.TableName);
                new OidColumnIterator(resultClass).Iterate(delegate(OidColumn oidColumn, bool isLastEntry)
                    {
                        gen.Add(oidColumn.Name);
                    }
                );

                foreach(Order o in this.orderings)
				{
					string colname = resultClass.FindField(o.FieldName).Column.Name;
					gen.Add(colname);
				}
				select = select.Replace(FieldMarker, gen.Result);
			}
			return select;
		}


		private void GenerateQuery()
		{
			if ((int) this.queryLanguage == Query.LoadRelations)
				LoadRelatedTables();
			else if (this.queryLanguage == Language.NDOql)
			{
				if (filter != null)
				{
					Scanner sc = new Scanner(filter);
					Token t;
					while ((t = sc.NextToken()) != null)
					{
						tokens.Add(t);
						if ( t.TokenType == Token.Type.Name && !names.Contains( t.Value ) )
						{
							names.Add( t.Value );
							if ( string.Compare( "oid", t.Value.ToString(), true ) == 0 )
								this.allowSubclasses = false;
						}
						//Console.WriteLine(t.ToString());
					}
				}
				AddSubQueryRecursive();
			}
			else
			{
				subQueries.Add (new QueryInfo(resultType, this.filter));
			}
		}


		private IList ExecuteSubQuery(Type t, string generatedQuery)
		{
			IPersistenceHandler persistenceHandler = mappings.GetPersistenceHandler(t, this.PersistenceManager.HasOwnerCreatedIds);
			this.pm.CheckTransaction(persistenceHandler, t);

			bool dontTouch = false;
#if PRO
			dontTouch = this.queryLanguage == Query.Language.Sql;
#endif
			DataTable table = persistenceHandler.SqlQuery(generatedQuery, this.hollowMode, dontTouch, parameters);
			return pm.DataTableToIList(t, table.Rows, hollowMode);
		}

		private IList ExecuteOrderedSubQuery(Type t, string generatedQuery)
		{
#if !ExtractOrdersFromQueryString
			Class resultSubClass = this.pm.GetClass(t);
			DataTable comparismTable = new DataTable("ComparismTable");
			foreach(Query.Order order in this.orderings)
			{
				DataColumn col = comparismTable.Columns.Add(order.FieldName);
				if (order.IsAscending)
					col.AutoIncrementStep = 1;
				else
					col.AutoIncrementStep = -1;
			}
#endif

#if ExtractOrdersFromQueryString
			Regex regex = new Regex(@"ORDER\s*BY\s*(.*)");
			Match match = regex.Match(generatedQuery);
			string order;
			DataTable comparismTable = null;
			int i;
			if (match.Success)
			{
				order = match.Groups[1].Value;
				order = order.Replace(" ASC", string.Empty);
				order = order.Replace(" DESC", string.Empty);
				comparismTable = new DataTable();
				string[] cols = order.Split(',');
				i = 0;
				foreach(string s in cols)
				{
					DataColumn col = comparismTable.Columns.Add(s.Trim());
					if (this.orderings[i++] is AscendingOrder)
						col.AutoIncrementStep = 1;
					else
						col.AutoIncrementStep = -1;
				}
			}
			else
			{
				throw new InternalException(473, "Query.cs");
			}
#endif
			IPersistenceHandler persistenceHandler = mappings.GetPersistenceHandler(t, this.PersistenceManager.HasOwnerCreatedIds);
			this.pm.CheckTransaction(persistenceHandler, t);

#if PRO
			bool dontTouch = this.queryLanguage == Query.Language.Sql;
#else
			bool dontTouch = false;
#endif
			DataTable table = persistenceHandler.SqlQuery(generatedQuery, this.hollowMode, dontTouch, parameters);
			DataRow[] rows = table.Select();
			IList objects = pm.DataTableToIList(t, rows, hollowMode);
			ArrayList result = new ArrayList(objects.Count);
			int i = 0;
			IProvider provider = mappings.GetProvider(t);
#if !ExtractOrdersFromQueryString
			foreach(object o in objects)
			{
				DataRow row = rows[i++];
				DataRow newRow = comparismTable.NewRow();
				foreach(Order order in this.orderings)
				{
					string newColumnName = order.FieldName;
					if (!comparismTable.Columns.Contains(newColumnName))
						throw new InternalException(558, "Query.cs - Column not found.");
					string oldColumnName = resultSubClass.FindField(order.FieldName).Column.Name;
					if (!table.Columns.Contains(oldColumnName))
						throw new InternalException(561, "Query.cs - Column not found.");
					newRow[newColumnName] = row[oldColumnName];
				}
				result.Add(new ObjectRowPair(o, newRow));
			}
#endif
#if ExtractOrdersFromQueryString
			foreach(object o in objects)
			{
				DataRow row = rows[i++];
				DataRow newRow = comparismTable.NewRow();
				foreach(DataColumn dc in table.Columns)
				{
					string newColumnName = QualifiedTableName.Get(table.TableName, provider) + "." + provider.GetQuotedName(dc.ColumnName);
					if (comparismTable.Columns.Contains(newColumnName))
					{
						DataColumn dc2 = comparismTable.Columns[newColumnName];
						newRow[dc2] = row[dc.ColumnName];
					}
				}
				result.Add(new ObjectRowPair(o, newRow));
			}
#endif
			return result;
		}
	

		/// <summary>
		/// Execute the query with the given parameters
		/// </summary>
		/// <returns>List of retrieved objects</returns>
		public IList Execute()
		{
			return GetResultList();
		}

		/// <summary>
		/// Execute the query to retrieve a single object.
		/// </summary>
		/// <returns>The retrieved object or null, if the query hasn't a result.</returns>
		/// <remarks>If the query has more than one result, the first of the results will be returned.</remarks>
		public IPersistenceCapable ExecuteSingle()
		{
			return ExecuteSingle(false);
		}


		private IList QueryOrderedPolymorphicList()
		{
			ArrayList al = new ArrayList();
			foreach (QueryInfo qi in subQueries)
				al.AddRange(ExecuteOrderedSubQuery(qi.ResultType, qi.QueryString));
			al.Sort();
			ArrayList result = new ArrayList(al.Count);
			foreach(ObjectRowPair orp in al)
				result.Add(orp.Obj);
			return result;
		}
/*
		void FindPrefetch(Type t, Hashtable prefetches, string name, Stack relations)
		{
			string[] parts = name.Split('.');
			Class cl = this.pm.GetClass(this.resultType);
			Relation r = cl.FindRelation(parts[0]);
			relations.Push(r);
			if (!prefetches.Contains(name))
				prefetches.Add(name, relations.Clone());			
			if (parts.Length > 1)
			{
				int p = name.IndexOf('.');
				FindPrefetch(r.ReferencedType, prefetches, name.Substring(p + 1), relations);
			}
		}
*/

		Type GetPrefetchResultType(Type t, string relation)
		{
			string[] parts = relation.Split('.');
			Class cl = pm.GetClass(t);
			Relation rel = cl.FindRelation(parts[0]);
			if (parts.Length == 1)
				return rel.ReferencedType;
			else
				return GetPrefetchResultType(rel.ReferencedType, 
					relation.Substring(relation.IndexOf('.') + 1));
		}

		// This function is a real hack. This functionality should be
        // replaced with a more generic approach to prefetches.
        string ExtractTableNames(string condition, string exclude, string dummyName)
		{
            int p = dummyName.IndexOf("yyy");
            bool hasQuotes = p > 0 && p + 3 < dummyName.Length;
            if (!hasQuotes)
                throw new InternalException(706, "Unquoted name in ExtractTableNames");
            string q1 = dummyName.Substring(0, p);
            string q2 = dummyName.Substring(p + 3);
			Hashtable exclusions = new Hashtable();
			exclusions.Add(exclude, null);
            string regCond = @"(\" + q1 + @"[^\" + q2 + @"]+\" + q2 + @")\.\" + q1 + @"[^\" + q2 + @"]+\" + q2;
            Regex regex = new Regex(@"(\" + q1 + @"[^\" + q2 + @"]+\" + q2 + @")\.\" + q1 + @"[^\" + q2 + @"]+\" + q2);
			MatchCollection matches = regex.Matches(condition);
			string result = string.Empty;
			for(int i = 0; i < matches.Count; i++)
			{
				string val = matches[i].Groups[1].Value;
				if (!exclusions.Contains(val))
				{
					exclusions.Add(val, null);
					result += val;
					result += ", ";
				}
			}
			if (result != string.Empty)
				result = result.Substring(0, result.Length - 2);
			return result;
		}

		void MatchRelations(IList parents, IList childs, string relationName)
		{
			if (parents.Count == 0)
				return;
			if (childs.Count == 0)
				return;
			Class cl = pm.GetClass(resultType);
			Relation r = cl.FindRelation(relationName);
			RelationCollector rc = new RelationCollector(cl);
			rc.CollectRelations();
			IList parentColumns = rc.ForeignKeyColumns;
			cl = pm.GetClass(r.ReferencedType);
			rc = new RelationCollector(cl);
			rc.CollectRelations();
			IList childColumns = rc.ForeignKeyColumns;
            // Used to determine, if the relation has been collected
            string testColumnName = ((ForeignKeyColumn)r.ForeignKeyColumns[0]).Name;
            if (r.Multiplicity == RelationMultiplicity.Element && parentColumns.Contains(testColumnName))
			{
				foreach(IPersistenceCapable parent in parents)
				{
					foreach(IPersistenceCapable child in childs)
					{
                        if (parent.NDOLoadState.LostRowsEqualsOid(child.NDOObjectId.Id, r))
                            mappings.SetRelationField(parent, r.FieldName, child);
                        //KeyValuePair kvp = ((KeyValueList)parent.NDOLoadState.LostRowInfo)[r.ForeignKeyColumnName];
                        //if (kvp.Value.Equals(child.NDOObjectId.Id.Value))
                        //    mappings.SetRelationField(parent, r.FieldName, child);
					}
				}
			}
            else if (r.Multiplicity == RelationMultiplicity.List && childColumns.Contains(testColumnName))
			{
				foreach(IPersistenceCapable parent in parents)
				{
                    //object parentOid = parent.NDOObjectId.Id.Value;
                    //bool parentOidIsGuid = (parentOid is Guid);
					IList container = mappings.GetRelationContainer(parent, r);
					foreach(IPersistenceCapable child in childs)
					{
                        if (child.NDOLoadState.LostRowsEqualsOid(parent.NDOObjectId.Id, r))
                            container.Add(child);
                        //KeyValuePair kvp = ((KeyValueList)child.NDOLoadState.LostRowInfo)[r.ForeignKeyColumnName];
                        //object oidCompare;
                        //if (parentOidIsGuid && kvp.Value is string)
                        //    oidCompare = parentOid.ToString();
                        //else
                        //    oidCompare = parentOid;
                        //if (kvp.Value.Equals(oidCompare))
                        //{
                        //    container.Add(child);
                        //}
					}
					parent.NDOSetLoadState(r.Ordinal, true);
				}
			}
		}

		void GetPrefetches(IList parents)
		{
			if (this.prefetches.Count == 0)
				return;

			foreach(object o in this.prefetches)
			{
				string pf = o.ToString() + ".oid";
				ArrayList pfNames = new ArrayList(this.names);
				pfNames.Add(pf);
				WherePartGenerator gen = new WherePartGenerator(this.mappings.GetProvider(resultType), 
					pm.GetClass(resultType),
					pfNames, this.tokens,
					this.mappings, new Hashtable());
				string condition = gen.ToString();
				Type t = GetPrefetchResultType(this.resultType, o.ToString());
				IProvider provider = this.mappings.GetProvider(t);
				Class cl = pm.GetClass(t);
				string tableNames = ExtractTableNames(condition, QualifiedTableName.Get(cl.TableName, provider), provider.GetQuotedName("yyy"));
				SelectPartGenerator sgen = new SelectPartGenerator(provider, cl, new ArrayList(), this.mappings, new Hashtable());				
				string select = sgen.ToString();
                if (select.IndexOf("SELECT DISTINCT") < 0)
                    select = select.Replace("SELECT", "SELECT DISTINCT");
				select += "," + tableNames;
				this.hollowMode = false;
				IList result = ExecuteSubQuery(t, select + " " + condition);
				MatchRelations(parents, result, o.ToString());
			}
			

/*
			Hashtable serializedPrefetches = new Hashtable();
			Stack relations = new Stack();
			foreach(string prefetch in this.prefetches)
			{
				FindPrefetch(this.resultType, serializedPrefetches, prefetch, relations);
			}
			foreach(DictionaryEntry de in serializedPrefetches)
			{
				Stack stack = (Stack) de.Value;
				for (int i = 0; i < stack.Count; i++)
				{
					Relation r = (Relation) stack.Pop();
				}
			}
			*/
		}


		private IList GetResultList()
		{
			IList result = GenericListReflector.CreateList( resultType );

			if (subQueries.Count == 0) // Query is not yet built
				GenerateQuery();

#if PRO
			bool isPro = true;
#else
			bool isPro = false;
#endif
            // this.pm.CheckTransaction happens in ExecuteOrderedSubQuery or in ExecuteSubQuery
			if (isPro && subQueries.Count > 1 && this.orderings.Count > 0)
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
				IList fetchResult = GenericListReflector.CreateList( resultType );
				for (int i = this.skip; i < Math.Min( result.Count, i + take ); i++ )
					fetchResult.Add( result[i] );
			}
			return result;
		}

		/// <summary>
		/// Execute the query to retrieve a single object.
		/// </summary>
		/// <param name="throwIfResultCountIsWrong">If true, an exception wil be thrown, if the resultset doesn't contain excatly 1 object.</param>
		/// <returns>The retrieved object</returns>
		public IPersistenceCapable ExecuteSingle(bool throwIfResultCountIsWrong)
		{
			IList resultList = GetResultList();
			int count = resultList.Count;
			if (count == 1 || (!throwIfResultCountIsWrong && count > 0))
			{
				return (IPersistenceCapable) resultList[0];
			}
			else
			{
				if (throwIfResultCountIsWrong)
					throw new QueryException(10002, count.ToString() + " result objects in ExecuteSingle call");
				else
					return null;
			}
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

                query = query.Replace(Query.FieldMarker, func.ToString() + "(" + column + ") as " + provider.GetQuotedName("AggrResult"));
                query = query.Replace(" DISTINCT", "");

				IPersistenceHandler persistenceHandler = mappings.GetPersistenceHandler(t, this.PersistenceManager.HasOwnerCreatedIds);
				this.pm.CheckTransaction(persistenceHandler, t);
				// Note, that we can't execute all subQueries in one batch, because
				// the subqueries could be executed against different connections.
				IList l = persistenceHandler.ExecuteBatch(new string[]{query}, this.parameters);
				if (l.Count == 0)
					partResults[i] = null;
				else
				{
                    partResults[i] = ((Hashtable)l[0])["AggrResult"];
				}
				i++;					
			}
			this.pm.CheckEndTransaction(false);
			return func.ComputeResult(partResults);

		}


		/// <summary>
		/// Diese Abfrage wird generiert, wenn Relationen nachgeladen werden sollen.
		/// Bei Subclasses wird sie entsprechend oft vom pm aufgerufen.
		/// </summary>
		/// <returns></returns>
		private void LoadRelatedTables()
		{
#if MaskedOut
			Class resultClass = pm.GetClass(resultType);
			string[] arr = filter.Split('=');
			if (arr.Length != 2) 
				throw new QueryException(10007, "LoadRelations with wrong expression string: " + filter);
			StringBuilder sql = new StringBuilder();
			IProvider provider = mappings.GetProvider(resultClass);
			sql.Append("SELECT " + Query.FieldMarker + " from ");
			sql.Append(QualifiedTableName.Get(resultClass.TableName, provider));
			sql.Append(" WHERE ");
			sql.Append(provider.GetQuotedName(arr[0]));
			sql.Append(" = ");
			sql.Append(arr[1]);
			subQueries.Add(new QueryInfo(resultType, sql.ToString()));
#endif
            Class resultClass = pm.GetClass(resultType);
            StringBuilder sql = new StringBuilder();
            IProvider provider = mappings.GetProvider(resultClass);
            sql.Append("SELECT " + Query.FieldMarker + " FROM ");
            sql.Append(QualifiedTableName.Get(resultClass.TableName, provider));
            sql.Append(" WHERE ");
            sql.Append(filter);
            subQueries.Add(new QueryInfo(resultType, sql.ToString()));
		}

		/// <summary>
		/// Determines if a query should fetch objects of subclasses of a type. 
		/// This value is true by default.
		/// </summary>
		public bool AllowSubclasses
		{
			get { return allowSubclasses; }
			set { allowSubclasses = value; }
		}



		/// <summary>
		/// Helper class to define sorting of the resultset of a query
		/// </summary>
		public abstract class Order
		{
			string orderType;
			string fieldName;

			/// <summary>
			/// If true, the order object represents an ascending order, if false, the 
			/// order object represents an descending order.
			/// </summary>
			public abstract bool IsAscending
			{
				get;
			}

			/// <summary>
			/// Gets the field name of the order object.
			/// </summary>
			public string FieldName
			{
				get { return fieldName; }
			}
			
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="ot">Order type, which is ASC or DESC</param>
			/// <param name="fn">Field name to be used to sort the data</param>
			public Order(string ot, string fn)
			{
				orderType = ot;
				fieldName = fn;
			}
			
			/// <summary>
			/// Returns the Order By clause string
			/// </summary>
			/// <returns>Order By clause string</returns>
			public string ToString(Class cl)
			{
				IProvider provider = cl.Provider;

				Field field = cl.FindField(fieldName);
				if (field == null)
					throw new NDOException(7, "Can't find mapping information for field " + cl.FullName + "." + fieldName);
				if (provider == null)
					return cl.TableName + "." + field.Column.Name + " " + orderType;
				else
					return QualifiedTableName.Get(cl.TableName, provider) + "." + provider.GetQuotedName(field.Column.Name) + " " + orderType;

			}
		}


		/// <summary>
		/// This class is used to provide parameters for queries. If the parameter is a string, 
		/// it will appear in the resulting SQL statement enclosed in single quotes.
		/// </summary>
		public class Parameter
		{
			/// <summary>
			/// Stores the parameter object
			/// </summary>
			protected object par;
			public object Value
			{
				get { return par; }
				set { par = value; }
			}
			protected IProvider ndoprovider = null;

			public IProvider Provider
			{
			    get { return ndoprovider; }
			    set { ndoprovider = value; }
			}
			
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="o">The parameter object</param>
			public Parameter(object o)
			{
				par = o;
			}

			/// <summary>
			/// Representation of the parameter in the SQL statement of the query
			/// </summary>
			/// <returns>Parameter string</returns>
			public override string ToString()
			{
				if (par is string || par is Guid)
					return "'" + par.ToString() + "'";
				else if (par is DateTime)
					return ndoprovider.GetSqlLiteral((DateTime)par);
				return par.ToString();
			}
		}

		/// <summary>
		/// This class is used to enforce quoting of the parameter representation in the resulting SQL statement of a query,
		/// even if it is not a string.
		/// </summary>
		public class StringParameter : Parameter
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="o">The parameter object</param>
			public StringParameter(object o) : base (o)
			{
			}

			/// <summary>
			/// String expression
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return "'" + par.ToString() + "'";
			}
		}

		
		/// <summary>
		/// Specialized Class to support Ascending ordering
		/// </summary>
		public class AscendingOrder : Order
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="fn">Field name to be used in the ordering</param>
			public AscendingOrder(string fn) : base("ASC", fn)
			{
			}

			/// <summary>
			/// Returns always true.
			/// </summary>
			public override bool IsAscending
			{
				get { return true; }
			}
		}



		
		/// <summary>
		/// Specialized Class to support Descending ordering
		/// </summary>
		public class DescendingOrder : Order
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="fn">Field name to be used in the ordering</param>
			public DescendingOrder(string fn) : base("DESC", fn)
			{
			}

			public override bool IsAscending
			{
				get { return false; }
			}

		}

		/// <summary>
		/// The orderings collection of the Query. The calling context has full access to this member.
		/// </summary>
		public IList Orderings
		{
			get
			{ return orderings; }
		}

		/// <summary>
		/// The parameters collection of the Query. The calling context has full access to this member.
		/// </summary>
		public NDOParameterCollection Parameters
		{
			get { return parameters; }
		}

		/// <summary>
		/// The prefetches collection of the Query. Specifies, which relation members should be prefetched in a query. 
		/// </summary>
		public ArrayList Prefetches
		{
			get { return prefetches; }
			set { prefetches = value; }
		}

		/// <summary>
		/// This count of rows will be skipped.
		/// </summary>
		/// <remarks>If the database supports paging, this operation will be performed by the database.</remarks>
		public int Skip
		{
			get { return this.skip; }
			set 
			{ 
				this.skip = value;
				this.subQueries.Clear(); 
			}
		}

		/// <summary>
		/// This maximum count of rows will be taken from the result.
		/// </summary>
		/// <remarks>If the database supports paging, this operation will be performed by the database.</remarks>
		public int Take
		{
			get { return this.take; }
			set 
			{ 
				this.take = value; 
				this.subQueries.Clear(); 
			}
		}

		private class AggregateFunction
		{
			AggregateType type;
			public AggregateFunction(AggregateType type)
			{
				this.type = type;
			}

			public override string ToString()
			{
				switch(type)
				{
					case AggregateType.Avg:
						return "Avg";
					case AggregateType.Min:
						return "Min";
					case AggregateType.Max:
						return "Max";
					case AggregateType.Count:
						return "Count";
					case AggregateType.StDev:
						return "StDev";
					case AggregateType.Var:
						return "Var";
					case AggregateType.Sum:
						return "Sum";
					default:
						return string.Empty;
				}
			}

			public object ComputeResult(object[] parts)
			{
				if (parts.Length == 0)
					return 0m;
				if (parts.Length > 1 &&(type == AggregateType.StDev || type == AggregateType.Var))
					throw new NDOException(91, "Can't compute StDev or Var over more than one table.");
				if (type == AggregateType.StDev || type == AggregateType.Var)
					return parts[0];
				switch (type)
				{
					case AggregateType.Avg:
						if (parts.Length > 1)
							throw new NotImplementedException("Average over more than one table is not yet implemented");
						return parts[0];
					case AggregateType.Min:
						IComparable minVal = parts[0] as IComparable;
						for (int i = 1; i < parts.Length; i++)
							if (((IComparable)parts[i]).CompareTo(minVal) < 0)
								minVal = (IComparable)parts[i];
						return minVal;
					case AggregateType.Max:
                        IComparable maxVal = parts[0] as IComparable;
                        for (int i = 1; i < parts.Length; i++)
                            if (((IComparable)parts[i]).CompareTo(maxVal) > 0)
                                maxVal = (IComparable)parts[i];
                        return maxVal;
                    case AggregateType.Count:
					case AggregateType.Sum:
						decimal sum = 0m;
						for ( int i = 0; i < parts.Length; i++ )
						{
							if ( parts[i] == null )
								continue;
							IConvertible ic = (IConvertible) parts[i];
							sum += ic.ToDecimal( null );
						}
						return sum;
					default:
						return 0m;
				}
			}
		}



	}
}
