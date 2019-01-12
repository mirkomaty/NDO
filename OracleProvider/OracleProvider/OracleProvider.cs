//
// Copyright (c) 2002-2016 Mirko Matytschak 
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
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using NDO;
using System.Collections;
using System.Text.RegularExpressions;
using NDOInterfaces;

#pragma warning disable 618

namespace OracleProvider
{
	/// <summary>
	/// Sample adapter class to connect new ADO.NET Providers with NDO.
	/// This Adapter is based on the Oracle Provider in .NET Framework 1.1
	/// </summary>
	public class Provider : NDOAbstractProvider
	{
		// The following methods provide objects of provider classes 
		// which implement common interfaces in .NET:
		// IDbConnection, IDbCommand, DbDataAdapter and the Parameter objects
		#region Provide specialized type objects
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new OracleConnection(connectionString);
		}

		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			OracleCommand command = new OracleCommand();
			command.Connection = (OracleConnection)connection;
			return command;
		}

		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
		{
			OracleDataAdapter da = new OracleDataAdapter();
			da.SelectCommand = (OracleCommand)select;
			da.UpdateCommand = (OracleCommand)update;
			da.InsertCommand = (OracleCommand)insert;
			da.DeleteCommand = (OracleCommand)delete;
			return da;
		}

		
		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new OracleCommandBuilder((OracleDataAdapter)dataAdapter);
		}

		public override IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
			// Cast notwendig, damit der DbType richtig übersetzt wird
			OracleCommand cmd = (OracleCommand) command;
			return cmd.Parameters.Add(new OracleParameter(parameterName, (OracleDbType)dbType, size > -1 ? size : 0, columnName));			
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			// Cast notwendig, damit der DbType richtig übersetzt wird
			OracleCommand cmd = (OracleCommand) command;
			return cmd.Parameters.Add(new OracleParameter(parameterName, (OracleDbType)dbType, size > -1 ? size : 0, dir, isNullable, precision, scale, srcColumn, srcVersion, value));
		}
		#endregion


		// The following method convert System.Type objects to OracleType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide OracleType members
		public override object GetDbType(Type t) 
		{
			t = base.ConvertNullableType(t);
			if (t == typeof(bool))
				return OracleDbType.Byte;
			else if ( t == typeof(byte) )
				return OracleDbType.Byte;
			else if ( t == typeof(sbyte) )
				return OracleDbType.Byte;
			else if ( t == typeof(char) )
				return OracleDbType.Char;
			else if ( t == typeof(short))
				return OracleDbType.Single;
			else if ( t == typeof(ushort))
				return OracleDbType.Int16;
			else if ( t == typeof(int))
				return OracleDbType.Int32;
			else if ( t == typeof(uint))
				return OracleDbType.Int32;
			else if ( t == typeof(long))
				return OracleDbType.Int64;
			else if ( t == typeof(System.Guid))
				return OracleDbType.Char;
			else if ( t == typeof(ulong))
				return OracleDbType.Int64;
			else if ( t == typeof(float))
				return OracleDbType.Double;
			else if ( t == typeof(double))
				return OracleDbType.Double;
			else if ( t == typeof(string))
				return OracleDbType.NVarchar2;
			else if ( t == typeof(byte[]))
				return OracleDbType.Raw;
			else if ( t == typeof(decimal))
				return OracleDbType.Decimal;
			else if ( t == typeof(System.DateTime))
				return OracleDbType.TimeStamp;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return OracleDbType.Int32;
			else
				throw new NDOException(30, "NDOOracleProvider.GetDbType: Type " + t.Name + " can't be converted into an OracleDbType.");
		}

		// This method converts string representations of OracleType-Members
		// into OracleDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			OracleDbType result;
			if (!Enum.TryParse(typeName, out result))
				throw new NDOException(31, "NDOOracleProvider.GetDbType: Typ name " + typeName + " can't be converted into an OracleType");
			return result;
		}

		private string GetDateExpression(System.DateTime dt)
		{
			string ts = dt.ToString("dd-MMM-yyyy hh:mm:ss ");
			if (0 < dt.Hour && dt.Hour <= 12)
				ts += "AM";
			else
				ts += "PM";
			return "TO_DATE('" + ts + "', 'dd-Mon-yyyy HH:MI:SS AM')";
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string GetSqlLiteral(object o)
		{
			if (o is DateTime)
				return this.GetDateExpression((DateTime)o);
			return base.GetSqlLiteral(o);
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override int GetDefaultLength(System.Type t)
		{
			if ( t == typeof(bool) )
				return 3;
			else if ( t == typeof(byte) )
				return 3;
			else if ( t == typeof(sbyte) )
				return 3;
			else if ( t == typeof(char) )
				return 5;
			else if ( t == typeof(short))
				return 10;
			else if ( t == typeof(ushort))
				return 10;
			else if ( t == typeof(int))
				return 10;
			else if ( t == typeof(uint))
				return 10;
			else if ( t == typeof(long))
				return 20;
			else if ( t == typeof(System.Guid))
				return 36;
			else if ( t == typeof(ulong))
				return 20;
			else if ( t == typeof(float))
				return 15;
			else if ( t == typeof(double))
				return 30;
			else if ( t == typeof(string))
				return 255;
			else if ( t == typeof(byte[]))
				return 255;
			else if ( t == typeof(decimal))
				return 18;
			else if ( t == typeof(System.DateTime))
				return 16;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return 10;
			else
				return 0;		
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string Wildcard
		{
			get { return "%"; }
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
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
		
		//private Hashtable namedParameters = new Hashtable();
		public override string GetNamedParameter(string plainParameterName)
		{
			return ":p" + plainParameterName;
		}
	
		public override string GetQuotedName(string plainName)
		{
			return "\"" + plainName + "\"";
		}

	
		public override string[] GetTableNames(IDbConnection conn, string owner)
		{
			bool wasOpen = true;
			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
				wasOpen = false;
			}
			OracleCommand cmd = new OracleCommand("SELECT TABLE_NAME FROM ALL_TABLES where OWNER LIKE '" + owner + "'", (OracleConnection) conn);
			OracleDataReader dr = cmd.ExecuteReader();
			IList result = new ArrayList();
			while (dr.Read())
				result.Add(dr.GetString(0));
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
				return Enum.GetNames(typeof(OracleDbType));
			}
		}


		public override string Name { get { return "Oracle"; }  }

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override bool SupportsNativeGuidType { get { return false; } }

		#endregion

	}
}
