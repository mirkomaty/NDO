//
// Copyright (c) 2002-2023 Mirko Matytschak 
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


using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using NDOInterfaces;
using System.Collections;
using MySqlConnector;

namespace NDO.MySqlConnectorProvider
{
	/// <summary>
	/// Sample adapter class to connect new ADO.NET Providers with NDO.
	/// This Adapter is based on the MySql MySql Data Connector
	/// </summary>
	public class Provider : NDOAbstractProvider
	{
		// The following methods provide objects of provider classes 
		// which implement common interfaces in .NET:
		// IDbConnection, IDbCommand, DbDataAdapter and the Parameter objects
		#region Provide specialized type objects
		public override IDbConnection NewConnection(string connectionString) 
		{
			return new MySqlConnection(connectionString);
		}

		public override IDbCommand NewSqlCommand(IDbConnection connection) 
		{
			MySqlCommand command = new MySqlCommand();
			command.Connection = (MySqlConnection)connection;
			return command;
		}

		public override DbDataAdapter NewDataAdapter(IDbCommand select, IDbCommand update, IDbCommand insert, IDbCommand delete) 
		{
			MySqlDataAdapter da = new MySqlDataAdapter();
			da.SelectCommand = (MySqlCommand)select;
			da.UpdateCommand = (MySqlCommand)update;
			da.InsertCommand = (MySqlCommand)insert;
			da.DeleteCommand = (MySqlCommand)delete;
			return da;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new MySqlCommandBuilder((MySqlDataAdapter)dataAdapter);
		}


		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
			return ((MySqlCommand)command).Parameters.Add(new MySqlParameter(parameterName, (MySqlDbType)dbType, size, columnName));			
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			return ((MySqlCommand)command).Parameters.Add(new MySqlParameter(parameterName, (MySqlDbType)dbType, size, dir, isNullable, precision, scale, srcColumn, srcVersion, value));
		}
		#endregion

		// The following method convert System.Type objects to MySqlDbType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide MySqlDbType members
		public override object GetDbType(Type t) 
		{
            t = base.ConvertNullableType(t);
			if ( t == typeof(bool) )
				return MySqlDbType.Byte;
			else if ( t == typeof(byte) )
				return MySqlDbType.Byte;
			else if ( t == typeof(sbyte) )
				return MySqlDbType.Byte;
			else if ( t == typeof(char) )
				return MySqlDbType.Int16;
			else if ( t == typeof(short))
				return MySqlDbType.Int16;
			else if ( t == typeof(ushort))
				return MySqlDbType.Int16;
			else if ( t == typeof(int))
				return MySqlDbType.Int32;
			else if ( t == typeof(uint))
				return MySqlDbType.Int32;
			else if ( t == typeof(long))
				return MySqlDbType.Int64;
			else if ( t == typeof(System.Guid))
				return MySqlDbType.VarChar;
			else if ( t == typeof(ulong))
				return MySqlDbType.Int64;
			else if ( t == typeof(float))
				return MySqlDbType.Float;
			else if ( t == typeof(double))
				return MySqlDbType.Double;
			else if ( t == typeof(string))
				return MySqlDbType.VarChar;
			else if ( t == typeof(byte[]))
				return MySqlDbType.MediumBlob;
			else if ( t == typeof(decimal))
				return MySqlDbType.Decimal;
			else if ( t == typeof(System.DateTime))
				return MySqlDbType.DateTime;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return MySqlDbType.Int32;
			else
				throw new NDOException(27, "NDO.MySqlProvider.GetDbType: Typ " + t.Name + " kann nicht in MySqlDbType konvertiert werden");
		}

		// The following method converts string representations of MySqlDbType-Members
		// into MySqlDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType( string typeName )
		{
			if (Enum.TryParse<MySqlDbType>( typeName, true, out var dbtype ))
				return dbtype;
			if (typeName.Equals( "BigInt", StringComparison.InvariantCultureIgnoreCase ))
				return MySqlDbType.Int64;
			if (typeName.Equals( "Datetime", StringComparison.InvariantCultureIgnoreCase ))
				return MySqlDbType.DateTime;
			if (typeName.Equals( "Long", StringComparison.InvariantCultureIgnoreCase ))
				return MySqlDbType.Int64;
			if (typeName.Equals( "LongLong", StringComparison.InvariantCultureIgnoreCase ))
				return MySqlDbType.Int64;
			throw new NDOException( 27, "MySqlConnector.Provider.GetDbType: Typname " + typeName + " kann nicht in MySqlDbType konvertiert werden" );
		}

		public override string GetDbTypeString( IDbDataParameter parameter )
		{
			return (((MySqlParameter)parameter).MySqlDbType).ToString();
		}

		private string GetDateExpression(System.DateTime dt)
		{
			//'9999-12-31 23:59:59'
			return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
		}
	

		public override int GetDefaultLength(System.Type t)
		{
            t = base.ConvertNullableType(t);
			if ( t == typeof(bool) )
				return 1;
			else if ( t == typeof(byte) )
				return 1;
			else if ( t == typeof(sbyte) )
				return 1;
			else if ( t == typeof(char) )
				return 2;
			else if ( t == typeof(short))
				return 2;
			else if ( t == typeof(ushort))
				return 2;
			else if ( t == typeof(int))
				return 4;
			else if ( t == typeof(uint))
				return 4;
			else if ( t == typeof(long))
				return 8;
			else if ( t == typeof(System.Guid))
				return 36;
			else if ( t == typeof(ulong))
				return 8;
			else if ( t == typeof(float))
				return 4;
			else if ( t == typeof(double))
				return 8;
			else if ( t == typeof(string))
				return 255;
			else if ( t == typeof(byte[]))
				return 100000;
			else if ( t == typeof(decimal))
				return 18;
			else if ( t == typeof(System.DateTime))
				return 16;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return 4;
			else
				return 0;		
		}


		public override string Wildcard
		{
			get { return "%"; }
		}

	    public override bool UseNamedParams
		{
			get { return true; }
		}


		#endregion
		
		//private Hashtable namedParameters = new Hashtable();
		public override string GetNamedParameter(string plainParameterName)
		{
			return "?" + plainParameterName;
		}
	
		public override string GetQuotedName(string plainName)
		{
			return "`" + plainName + "`";
		}
	
		public override string GetSqlLiteral(object o)
		{
			if (o is DateTime)
				return this.GetDateExpression((DateTime)o);
			return base.GetSqlLiteral (o);
		}
		

		/// <summary>
		/// Indicates whether the last automatically generated ID can be retrieved. 
		/// Returns true if a database provides automatically incremented IDs and its syntax has an expression 
		/// which retrieves the last generated ID; otherwise false.
		/// </summary>
		public override bool SupportsLastInsertedId 
		{
			get { return true; }
		}


		/// <summary>
		/// Gets an expression in the SQL dialect of the database, which retrieves the ID of the last
		/// inserted row, if the ID is automatically generated by the database.
		/// </summary>
		public override string GetLastInsertedId(string tableName, string columnName) 
		{
			return "LAST_INSERT_ID()";
		}

		/// <summary>
		/// Determines whether a database supports bulk command strings.
		/// </summary>
		public override bool SupportsBulkCommands 
		{
			get { return true; }
		}
		

		/// <summary>
		/// Generate one big command string out of several partial commands to save roundtrips
		/// to the server.
		/// </summary>
		/// <param name="commands"></param>
		/// <returns></returns>
		public override string GenerateBulkCommand(string[] commands)
		{
			StringBuilder sb = new StringBuilder(commands.Length * 100);
			foreach (string s in commands)
			{
				sb.Append(s);
				sb.Append(';');
			}
			return sb.ToString();
		}

			
		public override string[] GetTableNames(IDbConnection conn, string owner)
		{
			MySqlCommand cmd = new MySqlCommand("show tables", (MySqlConnection) conn);
			bool wasOpen = true;
			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
				wasOpen = false;
			}
			MySqlDataReader dr = cmd.ExecuteReader();
			IList result = new ArrayList();
			while (dr.Read())
				result.Add(dr.GetString(0));
			dr.Close();
			if (!wasOpen)
				conn.Close();
			string[] strresult = new string[result.Count];
			for (int i = 0; i < result.Count; i++)
				strresult[i] = (string) result[i];
			return strresult;
		}
	
		public override string[] TypeNames
		{
			get
			{				
				return Enum.GetNames(typeof(MySqlDbType));
			}
		}

		public override string Name { get { return "MySqlConnector"; }  }

		public override bool SupportsInsertBatch
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsNativeGuidType 
		{ 
			get { return false; } 
		}

		public override string FetchLimit( int skip, int take )
		{
			return "LIMIT " + take + " OFFSET " + skip;
		}


		public override string CreateDatabase(string databaseName, string connectionString, object additionalData)
		{
			base.CreateDatabase( databaseName, connectionString, additionalData );

			Regex regex = new Regex(@"Database\s*=([^\;]*)");
			Match match = regex.Match( connectionString );
			if (match.Success)
			{
				return connectionString.Substring(0, match.Groups[1].Index) + databaseName + connectionString.Substring(match.Index + match.Length);
			}
			
			if (!connectionString.EndsWith(";"))
				connectionString = connectionString + ";";
			
			return connectionString + "Database=" + databaseName;
		}
	}
}
