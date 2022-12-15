using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NDO.Query;
using System.Globalization;
using System.Text.RegularExpressions;
using NDOInterfaces;
using System.Xml.Linq;

namespace NDO.SqlPersistenceHandling
{
	internal class BatchExecutor
	{
		private static Regex parameterRegex = new Regex( @"\{(\d+)\}", RegexOptions.Compiled );
		private readonly IProvider provider;
		private readonly DbConnection connection;
		private readonly DbTransaction transaction;

		private Action<DataRow[], DbCommand, IEnumerable<string>> Dump { get; }

		public BatchExecutor(IProvider provider, DbConnection connection, DbTransaction transaction, Action<DataRow[], DbCommand, IEnumerable<string>> dump)
		{
			this.provider = provider;
			this.connection = connection;
			this.transaction = transaction;
			Dump = dump;
		}

		/// <summary>
		/// Executes a batch of sql statements.
		/// </summary>
		/// <param name="inputStatements">Each element in the array is a sql statement.</param>
		/// <param name="parameters">A list of parameters (see remarks).</param>
		/// <param name="isCommandArray">Determines, if statements contains identical commands which all need parameters</param>
		/// <returns>An List of Hashtables, containing the Name/Value pairs of the results.</returns>
		/// <remarks>
		/// For emty resultsets an empty dictionary will be returned. 
		/// If we have no command array, the parameters in the collection are for 
		/// all subqueries. In case of a command array the statements will bei combined to a template. 
		/// parameters contains a list of lists, with one entry per repetition of the template.
		/// </remarks>
		public async Task<IList<Dictionary<string, object>>> ExecuteBatchAsync( IEnumerable<string> inputStatements, IList parameters, IEnumerable<DbParameterInfo> parameterInfos = null, bool isCommandArray = false )
		{
			/*
				These are examples of the two cases:

				-------------- Non Array

				INSERT INTO x (Col1, Col2) VALUES({0},{1})
				SELECT SCOPE_IDENTITY()

				Parameter 'abc', 4711

				=>

				Insert x values(@p0,@p1);
				Select SCOPE_IDENTITY();

				@p0 = 'abc'
				@p1 = 4711

				------------- Array

				DELETE FROM x WHERE id1 = {0} AND tstamp = {1}

				Parameter
				12, '927D81A1-07AA-477B-9EA9-73F7C19E754F'
				33, '9061A0EA-BB02-47DB-866F-04210546E493'
				44, '2B9D2573-226E-4486-AEE7-A3DEFCEB48FC'
				45, '3B9D2573-226E-4486-AEE7-A3DEFCEB48FD'
				46, '4B9D2573-226E-4486-AEE7-A3DEFCEB48FE'

				=>

				DELETE FROM x WHERE id1 = @p0 AND tstamp = @p1;
				DELETE FROM x WHERE id1 = @p2 AND tstamp = @p3;
				DELETE FROM x WHERE id1 = @p4 AND tstamp = @p5;
				DELETE FROM x WHERE id1 = @p6 AND tstamp = @p7;
				DELETE FROM x WHERE id1 = @p8 AND tstamp = @p9;

				@p0 = 12
				@p1 = '927D81A1-07AA-477B-9EA9-73F7C19E754F'
				@p2 = 33
				@p3 = '9061A0EA-BB02-47DB-866F-04210546E493'
				@p4 = 44, 
				@p5 = '2B9D2573-226E-4486-AEE7-A3DEFCEB48FC'
				@p6 = 45
				@p7 = '3B9D2573-226E-4486-AEE7-A3DEFCEB48FD'
				@p8 = 46
				@p9 = '4B9D2573-226E-4486-AEE7-A3DEFCEB48FE'

			*/
			List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
			bool closeIt = false;
			DbDataReader dr = null;
			int i;
			try
			{
				if (this.connection.State != ConnectionState.Open)
				{
					closeIt = true;
					this.connection.Open();
				}

				string sql = string.Empty;
				var rearrangedStatements = new List<string>();

				if (this.provider.SupportsBulkCommands)
				{
					// cast is necessary here to get access to async methods
					DbCommand cmd = (DbCommand)this.provider.NewSqlCommand( this.connection );

					if (this.transaction != null)
						cmd.Transaction = this.transaction;

					if (parameters != null && parameters.Count > 0)
					{
						CreateQueryParameters( cmd, inputStatements, rearrangedStatements, parameters, parameterInfos, isCommandArray );
					}
					else
					{
						rearrangedStatements = inputStatements.ToList();
					}

					sql = this.provider.GenerateBulkCommand( rearrangedStatements.ToArray() );
					cmd.CommandText = sql;

					// cmd.CommandText can be changed in CreateQueryParameters
					Dump( null, cmd, rearrangedStatements );

					using (dr = await cmd.ExecuteReaderAsync().ConfigureAwait( false ))
					{
						do
						{
							var dict = new Dictionary<string, object>();

							while (await dr.ReadAsync().ConfigureAwait( false ))
							{
								for (i = 0; i < dr.FieldCount; i++)
								{
									// GetFieldValueAsync uses just Task.FromResult(),
									// so we don't gain anything with an async call here.
									string name = dr.GetName( i );
									if (name == "NdoInsertedId") // The result is an autoincremented id
										dict.Add( name, (int)dr.GetDecimal( i ) );
									else
										dict.Add( name, dr.GetValue( i ) );
								}
							}

							if (dict.Count > 0)
								result.Add( dict );

						} while (await dr.NextResultAsync().ConfigureAwait( false ));
					}
				}
				else
				{
					if (isCommandArray)
					{
						// A command array always has parameter sets, otherwise it wouldn't make any sense
						// to work with a command array
						foreach (IList parameterSet in parameters)
						{
							result.AddRange( await ExecuteStatementSetAsync( inputStatements, rearrangedStatements, parameterSet, parameterInfos ).ConfigureAwait( false ) );
						}
					}
					else
					{
						result.AddRange( await ExecuteStatementSetAsync( inputStatements, rearrangedStatements, parameters, parameterInfos ).ConfigureAwait( false ) );
					}
				}
			}
			finally
			{
				if (dr != null && !dr.IsClosed)
					dr.Close();
				if (closeIt)
					this.connection.Close();
			}

			return result;
		}

		private async Task<List<Dictionary<string, object>>> ExecuteStatementSetAsync( IEnumerable<string> inputStatements, List<string> rearrangedStatements, IList parameterSet, IEnumerable<DbParameterInfo> parameterInfos )
		{
			var result = new List<Dictionary<string, object>>();

			// cast is necessary here to get access to async methods
			DbCommand cmd = (DbCommand)this.provider.NewSqlCommand( this.connection );

			if (this.transaction != null)
				cmd.Transaction = this.transaction;

			rearrangedStatements.Clear();
			// we repeat rearranging for each parameterSet, just as if it were an ordinary Bulk statement set
			CreateQueryParameters( cmd, inputStatements, rearrangedStatements, parameterSet, parameterInfos, false );
			foreach (var statement in rearrangedStatements)
			{
				Dictionary<string,object> dict = new Dictionary<string, object>();				
				cmd.CommandText = statement;

				using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait( false ))
				{

					while (await dr.ReadAsync().ConfigureAwait( false ))
					{
						for (int i = 0; i < dr.FieldCount; i++)
						{
							var name = dr.GetName(i);
							if (name == "NdoInsertedId") // The result is an autoincremented id
								dict.Add( name, (int) dr.GetDecimal( i ) );
							else
								dict.Add( name, dr.GetValue( i ) );
						}
					}
				}

				result.Add( dict );
			}

			Dump( null, cmd, rearrangedStatements );
			return result;
		}



		public void CreateQueryParameters( DbCommand command, IEnumerable<string> inputStatements, List<string> rearrangedStatements, IList parameters, IEnumerable<DbParameterInfo> parameterInfos, bool isCommandArray )
		{
#warning TODO We need test cases for this method

			var maxindex = -1;

			foreach (var statement in inputStatements)
			{
				var matches = parameterRegex.Matches(statement);
				foreach (Match match in matches)
				{
					var i = int.Parse( match.Groups[1].Value );
					maxindex = Math.Max( i, maxindex );
				}
			}

			// No parameters in the statements? Just return the statements
			if (maxindex == -1)
			{
				rearrangedStatements.AddRange( inputStatements );
				return;
			}

			if (isCommandArray)
			{
				var index = 0;

				foreach (IList parameterSet in parameters)
				{
					if (parameterSet.Count <= maxindex)
						throw new QueryException( 10009, $"Parameter-Reference {maxindex} has no matching parameter." );

					// inputStatements contains kind-of a template
					// which will be repeated for each parameterSet
					foreach (var statement in inputStatements)
					{
						var rearrangedStatement = statement;
						var matches = parameterRegex.Matches(statement);
						foreach (Match match in matches)
						{
							var i = int.Parse( match.Groups[1].Value );
							var parName = this.provider.GetNamedParameter( $"p{(i + index)}" );
							rearrangedStatement = rearrangedStatement.Replace( match.Value, parName );
						}

						rearrangedStatements.Add( rearrangedStatement );
					}

					var parameterInfoEnum = parameterInfos?.GetEnumerator();
					foreach (var par in parameterSet)
					{
						parameterInfoEnum?.MoveNext();
						var parameterInfo = parameterInfoEnum?.Current;
						AddParameter( command, index++, par, parameterInfo );
					}
				}
			}
			else
			{
				foreach (var statement in inputStatements)
				{
					var rearrangedStatement = statement;

					MatchCollection matches = parameterRegex.Matches( statement );
					Dictionary<string, object> tcValues = new Dictionary<string, object>();
					if (maxindex > parameters.Count - 1)
						throw new QueryException( 10009, $"Parameter-Reference {maxindex} has no matching parameter." );

					foreach (Match match in matches)
					{
						var i = int.Parse( match.Groups[1].Value );
						var parName = this.provider.GetNamedParameter( $"p{i}" );
						rearrangedStatement = rearrangedStatement.Replace( match.Value, parName );
					}

					rearrangedStatements.Add( rearrangedStatement );
				}

				var parameterInfoEnum = parameterInfos?.GetEnumerator();
				int index = 0;
				foreach (var par in parameters)
				{
					parameterInfoEnum?.MoveNext();
					var parameterInfo = parameterInfoEnum?.Current;
					AddParameter( command, index++, par, parameterInfo );
				}
			}
		}

		private void AddParameter( DbCommand command, int index, object p, DbParameterInfo parameterInfo )
		{
			string name = "p" + index.ToString();

			if (parameterInfo != null)
			{
				provider.AddParameter
				(
					command,
					this.provider.GetNamedParameter( name ),
					parameterInfo.DbType,
					parameterInfo.Size,
					parameterInfo.Direction,
					parameterInfo.IsNullable,
					parameterInfo.Precision,
					parameterInfo.Scale,
					this.provider.GetQuotedName( name ),
					DataRowVersion.Current,
					p
				);
			}
			else
			{
				if (p == null)
					p = DBNull.Value;
				Type type = p.GetType();
				if (type.FullName.StartsWith( "System.Nullable`1" ))
					type = type.GetGenericArguments()[0];
				if (type == typeof( Guid ) && Guid.Empty.Equals( p ) || type == typeof( DateTime ) && DateTime.MinValue.Equals( p ))
				{
					p = DBNull.Value;
				}
				if (type.IsEnum)
				{
					type = Enum.GetUnderlyingType( type );
					p = ( (IConvertible) p ).ToType( type, CultureInfo.CurrentCulture );
				}
				else if (type == typeof( Guid ) && !provider.SupportsNativeGuidType)
				{
					type = typeof( string );
					if (p != DBNull.Value)
						p = p.ToString();
				}
				int length = this.provider.GetDefaultLength(type);
				if (type == typeof( string ))
				{
					length = ( (string) p ).Length;
					if (provider.GetType().Name.IndexOf( "Oracle" ) > -1)
					{
						if (length == 0)
							throw new QueryException( 10001, "Empty string parameters are not allowed in Oracle. Use IS NULL instead." );
					}
				}
				else if (type == typeof( byte[] ))
				{
					length = ( (byte[]) p ).Length;
				}
				IDataParameter par = provider.AddParameter(
					command,
					this.provider.GetNamedParameter(name),
					this.provider.GetDbType( type == typeof( DBNull ) ? typeof( string ) : type ),
					length,
					this.provider.GetQuotedName(name));
				par.Value = p;
				par.Direction = ParameterDirection.Input;
			}
		}
	}
}
