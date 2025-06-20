﻿//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using System.Reflection;
using System.Data;
using System.Data.Common;
using NDOInterfaces;

using System.Data.SQLite;
using System.IO;

namespace System.Data.SQLite
{
	public enum SQLiteDbType
	{
		Text,
		Numeric,
		Integer,
		Decimal,
		Int2,
		Real,
		Datetime,
		Blob,
		Guid
	}
}

namespace NDO.SqliteProvider
{
	/// <summary>
	/// Sample adapter class to connect new ADO.NET Providers with NDO.
	/// This Adapter is based on the SQLite SQLite Data Connector
	/// </summary>
	public class Provider : NDOAbstractProvider
	{
		public Provider()
		{
		}

		// The following methods provide objects of provider classes 
		// which implement common interfaces in .NET:
		// IDbConnection, IDbCommand, DbDataAdapter and the Parameter objects
		#region Provide specialized type objects
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new SQLiteConnection(connectionString);
		}

		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			SQLiteCommand command = new SQLiteCommand();
			command.Connection = (SQLiteConnection)connection;
			return command;
		}

		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
		{
			SQLiteDataAdapter da = new SQLiteDataAdapter();
			da.SelectCommand = (SQLiteCommand)select;
			da.UpdateCommand = (SQLiteCommand)update;
			da.InsertCommand = (SQLiteCommand)insert;
			da.DeleteCommand = (SQLiteCommand)delete;
			return da;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new SQLiteCommandBuilder((SQLiteDataAdapter)dataAdapter);
		}


		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object odbType, int size, string columnName) 
		{
			DbType dbType = GetDbTypeFromSqliteDbType( (SQLiteDbType) odbType );
			SQLiteParameter result = new SQLiteParameter( parameterName, dbType, size, columnName );
			((SQLiteCommand)command).Parameters.Add(result);
			return result;
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object odbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			DbType dbType = GetDbTypeFromSqliteDbType( (SQLiteDbType) odbType );
			SQLiteParameter result = new SQLiteParameter( parameterName, dbType, size, dir, isNullable, precision, scale, srcColumn, srcVersion, value );
			((SQLiteCommand)command).Parameters.Add(result);
			return result;
		}

		private DbType GetDbTypeFromSqliteDbType( SQLiteDbType sqliteDbType )
		{
			switch ( sqliteDbType )
			{
				case SQLiteDbType.Text:
					return DbType.String;
				case SQLiteDbType.Numeric:
					return DbType.VarNumeric;
				case SQLiteDbType.Integer:
					return DbType.Int32;
				case SQLiteDbType.Int2:
					return DbType.Int16;
				case SQLiteDbType.Decimal:
				    return DbType.Decimal;
				case SQLiteDbType.Real:
					return DbType.Double;
				case SQLiteDbType.Datetime:
					return DbType.DateTime;
				case SQLiteDbType.Blob:
					return DbType.Binary;
				case SQLiteDbType.Guid:
					return DbType.String;
			}
			return DbType.String;
		}

		#endregion

		// The following method convert System.Type objects to SQLiteDbType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide SQLiteDbType members
		public override object GetDbType(Type t) 
		{
            t = base.ConvertNullableType(t);
			return GetInternalDbType( t );
		}

		internal static SQLiteDbType GetInternalDbType( Type t )
		{
			if ( t == typeof( bool ) )
				return SQLiteDbType.Int2;
			else if ( t == typeof( byte ) )
				return SQLiteDbType.Int2;
			else if ( t == typeof( sbyte ) )
				return SQLiteDbType.Int2;
			else if ( t == typeof( char ) )
				return SQLiteDbType.Int2;
			else if ( t == typeof( short ) )
				return SQLiteDbType.Int2;
			else if ( t == typeof( ushort ) )
				return SQLiteDbType.Int2;
			else if ( t == typeof( int ) )
				return SQLiteDbType.Integer;
			else if ( t == typeof( uint ) )
				return SQLiteDbType.Integer;
			else if ( t == typeof( long ) )
				return SQLiteDbType.Numeric;
			else if ( t == typeof( System.Guid ) )
				return SQLiteDbType.Guid;
			else if ( t == typeof( ulong ) )
				return SQLiteDbType.Numeric;
			else if ( t == typeof( float ) )
				return SQLiteDbType.Real;
			else if ( t == typeof( double ) )
				return SQLiteDbType.Real;
			else if ( t == typeof( string ) )
				return SQLiteDbType.Text;
			else if ( t == typeof( byte[] ) )
				return SQLiteDbType.Blob;
			else if ( t == typeof( decimal ) )
				return SQLiteDbType.Decimal;
			else if ( t == typeof( System.DateTime ) )
				return SQLiteDbType.Datetime;
			else if ( t.IsSubclassOf( typeof( System.Enum ) ) )
				return SQLiteDbType.Integer;
			else
				throw new NDOException( 27, "NDO.SQLiteProvider.GetDbType: Type " + t.Name + " can't be converted into SQLiteDbType." );
		}

		// The following method converts string representations of SQLiteDbType-Members
		// into SQLiteDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			SQLiteDbType result;

			if (!Enum.TryParse<SQLiteDbType>(typeName, true, out result))
			throw new NDOException(27, "NDO.SQLiteProvider.Provider.GetDbType: Typname " + typeName + " cannot be converted into a DbType.");
			return result;
		}

		public override int GetDefaultLength(System.Type t)
		{
            t = base.ConvertNullableType(t);
            if (t == typeof(bool))
				return 2;
			else if ( t == typeof(byte) )
				return 2;
			else if ( t == typeof(sbyte) )
				return 2;
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
				return 255;
			else if ( t == typeof(decimal))
				return 10;
			else if ( t == typeof(System.DateTime))
				return 0;
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

/*
	    public override bool UseStoredProcedure
		{
			get { return false; }
		}
*/
		#endregion
		
		//private Hashtable namedParameters = new Hashtable();
		public override string GetNamedParameter(string plainParameterName)
		{
			return ":" + plainParameterName;
		}
	
		public override string GetQuotedName(string plainName)
		{
			return "\"" + plainName + '"';
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
			return "(SELECT last_insert_rowid())";
		}

		/// <summary>
		/// Determines whether a database supports bulk command strings.
		/// </summary>
		public override bool SupportsBulkCommands 
		{
			get { return false; }
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
            SQLiteDataAdapter a = new SQLiteDataAdapter("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;", (SQLiteConnection)conn);
            DataSet ds = new DataSet();
            a.Fill(ds);
            DataTable dt = ds.Tables[0];
			string[] strresult = new string[dt.Rows.Count];

			int i = 0;
			foreach (DataRow dr in dt.Rows)
            {
                strresult[i++] = (string)dr["name"];
            }

			return strresult;
		}
	
		public override string[] TypeNames
		{
			get
			{				
				return Enum.GetNames(typeof(SQLiteDbType));
			}
		}

		public override string Name { get { return "Sqlite"; }  }

		public override bool SupportsInsertBatch
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsNativeGuidType 
		{ 
			get { return true; } 
		}

		public override string FetchLimit( int skip, int take )
		{
			return "LIMIT " + skip + "," + take;
		}


		public override string CreateDatabase(string databaseName, string connectionString, object additionalData)
		{
			if (connectionString == null )
				throw new ArgumentNullException( nameof(connectionString) );

			string path = connectionString.Substring( connectionString.IndexOf( '=' ) + 1 );
			path = path.Trim();

			SQLiteConnection.CreateFile( path );

			return connectionString;
		}
	}
}
