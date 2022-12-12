﻿using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlServerProvider
{
	/// <summary>
	/// This is an Implementation of the IProvider interface for SqlServer. 
	/// For more information see <see cref="IProvider"> IProvider interface </see>.
	/// </summary>
	public class Provider : NDOAbstractProvider
	{

		#region Implementation of IProvider


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override bool UseNamedParams
		{
			get { return true; }
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string GetNamedParameter( string plainParameterName )
		{
			return "@" + plainParameterName;
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string NamedParamPrefix
		{
			get { return "@"; }
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		private string GetDateExpression( DateTime dt )
		{
			string dtstr = dt.ToString();
			dtstr = dtstr.Replace( ".", "/" );
			dtstr = dtstr.Replace( "-", "/" );
			return "'" + dtstr + "'";
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string GetSqlLiteral( object o )
		{
			if (o is DateTime)
				return this.GetDateExpression( (DateTime)o );
			return base.GetSqlLiteral( o );
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override System.Data.IDbConnection NewConnection( string connectionString )
		{
			return new SqlConnection( connectionString );
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override System.Data.IDbCommand NewSqlCommand( System.Data.IDbConnection connection )
		{
			SqlCommand command = new SqlCommand();
			command.Connection = (SqlConnection)connection;
			return command;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override DbDataAdapter NewDataAdapter( System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete )
		{
			SqlDataAdapter da = new SqlDataAdapter();
			da.SelectCommand = (SqlCommand)select;
			da.UpdateCommand = (SqlCommand)update;
			da.InsertCommand = (SqlCommand)insert;
			da.DeleteCommand = (SqlCommand)delete;
			return da;
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder( DbDataAdapter dataAdapter )
		{
			return new SqlCommandBuilder( (SqlDataAdapter)dataAdapter );
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override IDataParameter AddParameter( System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName )
		{
			return ((SqlCommand)command).Parameters.Add( new SqlParameter( parameterName, (SqlDbType)dbType, size > -1 ? size : 0, columnName ) );
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override IDataParameter AddParameter( IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value )
		{
			return ((SqlCommand)command).Parameters.Add( new SqlParameter( parameterName, (SqlDbType)dbType, size > -1 ? size : 0, dir, isNullable, precision, scale, srcColumn, srcVersion, value ) );
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object GetDbType( Type t )
		{
			t = base.ConvertNullableType( t );
			if (t == typeof( bool ))
				return SqlDbType.Bit;
			else if (t == typeof( byte ))
				return SqlDbType.TinyInt;
			else if (t == typeof( sbyte ))
				return SqlDbType.TinyInt;
			else if (t == typeof( char ))
				return SqlDbType.Char;
			else if (t == typeof( short ))
				return SqlDbType.SmallInt;
			else if (t == typeof( ushort ))
				return SqlDbType.SmallInt;
			else if (t == typeof( int ))
				return SqlDbType.Int;
			else if (t == typeof( uint ))
				return SqlDbType.Int;
			else if (t == typeof( long ))
				return SqlDbType.BigInt;
			else if (t == typeof( System.Guid ))
				return SqlDbType.UniqueIdentifier;
			else if (t == typeof( ulong ))
				return SqlDbType.BigInt;
			else if (t == typeof( float ))
				return SqlDbType.Real;
			else if (t == typeof( double ))
				return SqlDbType.Float;
			else if (t == typeof( string ))
				return SqlDbType.NVarChar;
			else if (t == typeof( byte[] ))
				return SqlDbType.Image;
			else if (t == typeof( decimal ))
				return SqlDbType.Decimal;
			else if (t == typeof( System.DateTime ))
				return SqlDbType.DateTime;
			else if (t.IsSubclassOf( typeof( System.Enum ) ))
				return SqlDbType.Int;
			else if (t == typeof( System.Data.SqlTypes.SqlXml ))
				return SqlDbType.Xml;
			else
				throw new Exception( $"NDOSqlProvider.GetDbType: Type {t.Name} can't be converted into a SqlDbType." );
		}

		public override string GetDbTypeString( IDbDataParameter parameter )
		{
			return (((SqlParameter)parameter).SqlDbType).ToString();
		}

		public override int GetDefaultLength( Type t )
		{
			if (t == typeof( byte[] ))
				return 65535;
			return base.GetDefaultLength( t );
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object GetDbType( string typeName )
		{
			string typeNameLower = typeName.ToLower();
			if (typeNameLower.StartsWith( "nvarchar" )) return SqlDbType.NVarChar;
			if (String.Compare( typeName, "VarChar", true ) == 0) return SqlDbType.VarChar;
			if (String.Compare( typeName, "NChar", true ) == 0) return SqlDbType.NChar;
			if (String.Compare( typeName, "Int", true ) == 0) return SqlDbType.Int;
			if (String.Compare( typeName, "Char", true ) == 0) return SqlDbType.Char;
			if (String.Compare( typeName, "UniqueIdentifier", true ) == 0) return SqlDbType.UniqueIdentifier;
			if (String.Compare( typeName, "DateTime", true ) == 0) return SqlDbType.DateTime;
			if (String.Compare( typeName, "Decimal", true ) == 0) return SqlDbType.Decimal;
			if (String.Compare( typeName, "Binary", true ) == 0) return SqlDbType.Binary;
			if (String.Compare( typeName, "Bit", true ) == 0) return SqlDbType.Bit;
			if (String.Compare( typeName, "Real", true ) == 0) return SqlDbType.Real;
			if (String.Compare( typeName, "Float", true ) == 0) return SqlDbType.Float;
			if (String.Compare( typeName, "Image", true ) == 0) return SqlDbType.Image;
			if (String.Compare( typeName, "Money", true ) == 0) return SqlDbType.Money;
			if (String.Compare( typeName, "NText", true ) == 0) return SqlDbType.NText;
			if (String.Compare( typeName, "SmallDateTime", true ) == 0) return SqlDbType.SmallDateTime;
			if (String.Compare( typeName, "SmallInt", true ) == 0) return SqlDbType.SmallInt;
			if (String.Compare( typeName, "SmallMoney", true ) == 0) return SqlDbType.SmallMoney;
			if (String.Compare( typeName, "Text", true ) == 0) return SqlDbType.Text;
			if (String.Compare( typeName, "Timestamp", true ) == 0) return SqlDbType.Timestamp;
			if (String.Compare( typeName, "TinyInt", true ) == 0) return SqlDbType.TinyInt;
			if (String.Compare( typeName, "VarBinary", true ) == 0) return SqlDbType.VarBinary;
			if (String.Compare( typeName, "Variant", true ) == 0) return SqlDbType.Variant;
			if (String.Compare( typeName, "BigInt", true ) == 0) return SqlDbType.BigInt;
			if (typeNameLower.StartsWith( "xml" )) return SqlDbType.Xml;
			if (String.Compare( typeName, "RowVersion", true ) == 0) return SqlDbType.Timestamp;
			SqlDbType result;
			if (Enum.TryParse( typeName, out result ))
				return result;
			throw new Exception( $"NDOSqlProvider.GetDbType: Type name {typeName} can't be converted into a SqlDbType." );
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string GetQuotedName( string plainName )
		{
			if (plainName[0] == '[')
				return plainName;
			return "[" + plainName + "]";
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
		/// <param name="tableName">The table name, the insert happens into.</param>
		/// <param name="columnName">The column name of the autoincremented primary key column.</param>
		/// <returns></returns>        
		public override string GetLastInsertedId( string tableName, string columnName )
		{
			return "SCOPE_IDENTITY()";
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
		public override string GenerateBulkCommand( string[] commands )
		{
			StringBuilder sb = new StringBuilder( commands.Length * 100 );
			foreach (string s in commands)
			{
				sb.Append( s );
				sb.Append( ';' );
			}
			return sb.ToString();
		}

		/// <summary>
		/// See <see cref="NDOInterfaces.IProvider">IProvider interface</see>.
		/// </summary>
		public override int MaxBulkCommandLength
		{
			// Der Wert ist willkürlich gesetzt. Die Query in der Größe dauert ungefähr 15s.
			get { return 1280000; }
		}



		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string[] GetTableNames( IDbConnection conn, string owner )
		{
			List<string> result = new List<string>();
			string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
			SqlCommand cmd = new SqlCommand( sql, (SqlConnection)conn );
			bool wasOpen = true;

			if (conn.State == ConnectionState.Closed)
			{
				wasOpen = false;
				conn.Open();
			}

			SqlDataReader dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				string tbl = dr.GetString( 2 );
				if (tbl != "dtproperties")
					result.Add( tbl );
			}

			dr.Close();

			if (!wasOpen)
				conn.Close();

			return result.ToArray();
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string[] TypeNames
		{
			get
			{
				return Enum.GetNames( typeof( SqlDbType ) );
			}
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string Name { get { return "SqlServer"; } }

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override bool SupportsNativeGuidType { get { return true; } }

		public override bool SupportsInsertBatch
		{
			get
			{
				return true;
			}
		}

		public override string CreateDatabase( string databaseName, string connectionString, object additionalData )
		{
			base.CreateDatabase( databaseName, connectionString, additionalData );
			Regex regex = new Regex( @"Initial\sCatalog=([^\;]*)" );
			Match match = regex.Match( connectionString );
			if (match.Success)
			{
				return connectionString.Substring( 0, match.Groups[1].Index ) + GetQuotedName( databaseName ) + connectionString.Substring( match.Index + match.Length );
			}

			if (connectionString.EndsWith( ";" ))
				return connectionString + "Initial Catalog=" + databaseName;
			else
				return connectionString + ";Initial Catalog=" + databaseName;
		}

		#endregion

	}
}
