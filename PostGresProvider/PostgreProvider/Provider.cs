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


//	Achtung: Diese Klasse ist als Beispiel gedacht, wie ADO.NET Provider mit NDO 
//	verbunden werden können. Es wird keine Garantie für die Funktionsfähigkeit
//	des hier veröffentlichten Codes übernommen.
//  Kopieren Sie die erstellte Dll in das NDO-Anwendungs-Verzeichnis. NDO findet dann
//  automatisch den Provider und reiht ihn unter dem Namen in die ProviderFactory ein, der 
//  vom Property "Name" zurückgegeben wird.
//	In diesem Beispiel wird Ihr Adapter unter dem Namen "Npgsql" in die ProviderFactory eingetragen.
//	Den String "Npgsql" (oder welchen String auch immer Sie zur Kennzeichnung benötigen)
//	müssen Sie in der NDOMapping.xml als Attribut "Type" des Connection-Eintrags 
//	angeben.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using NDOInterfaces;
using System.Collections;
using System.Windows.Forms;
using Npgsql;
using NpgsqlTypes;

namespace NDO.PostGreProvider
{
	/// <summary>
	/// Sample adapter class to connect new ADO.NET Providers with NDO.
	/// This Adapter is based on the Npgsql Npgsql Data Connector
	/// </summary>
	public class Provider : NDOAbstractProvider
	{
		// The following methods provide objects of provider classes 
		// which implement common interfaces in .NET:
		// IDbConnection, IDbCommand, DbDataAdapter and the Parameter objects
		#region Provide specialized type objects
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new NpgsqlConnection(connectionString);
		}

		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			NpgsqlCommand command = new NpgsqlCommand();
			command.Connection = (NpgsqlConnection)connection;
			return command;
		}

		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
		{
			NpgsqlDataAdapter da = new NpgsqlDataAdapter();
			da.SelectCommand = (NpgsqlCommand)select;
			da.UpdateCommand = (NpgsqlCommand)update;
			da.InsertCommand = (NpgsqlCommand)insert;
			da.DeleteCommand = (NpgsqlCommand)delete;
			return da;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new NpgsqlCommandBuilder((NpgsqlDataAdapter)dataAdapter);
		}


		public override IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
			return ((NpgsqlCommand)command).Parameters.Add(new NpgsqlParameter(parameterName, (NpgsqlDbType)dbType, size, columnName));			
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			return ((NpgsqlCommand)command).Parameters.Add(new NpgsqlParameter(parameterName, (NpgsqlDbType)dbType, size, srcColumn, dir, isNullable, precision, scale, srcVersion, value));
		}
		#endregion

		// The following method convert System.Type objects to NpgsqlDbType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide NpgsqlDbType members
		public override object GetDbType(Type t) 
		{
            t = base.ConvertNullableType(t);
            if (t == typeof(bool))
				return NpgsqlDbType.Boolean;
			else if ( t == typeof(byte) )
				return NpgsqlDbType.Smallint;
			else if ( t == typeof(sbyte) )
				return NpgsqlDbType.Smallint;
			else if ( t == typeof(char) )
				return NpgsqlDbType.Smallint;
			else if ( t == typeof(short))
				return NpgsqlDbType.Smallint;
			else if ( t == typeof(ushort))
				return NpgsqlDbType.Smallint;
			else if ( t == typeof(int))
				return NpgsqlDbType.Integer;
			else if ( t == typeof(uint))
				return NpgsqlDbType.Integer;
			else if ( t == typeof(long))
				return NpgsqlDbType.Bigint;
			else if ( t == typeof(System.Guid))
				return NpgsqlDbType.Char;
			else if ( t == typeof(ulong))
				return NpgsqlDbType.Bigint;
			else if ( t == typeof(float))
				return NpgsqlDbType.Real;
			else if ( t == typeof(double))
				return NpgsqlDbType.Double;
			else if ( t == typeof(string))
				return NpgsqlDbType.Varchar;
			else if ( t == typeof(byte[]))
				return NpgsqlDbType.Bytea;
			else if ( t == typeof(decimal))
				return NpgsqlDbType.Numeric;
			else if ( t == typeof(System.DateTime))
				return NpgsqlDbType.Date;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return NpgsqlDbType.Integer;
			else
				throw new NDOException(27, "NDO.NpgsqlProvider.GetDbType: Typ " + t.Name + " kann nicht in NpgsqlDbType konvertiert werden");
		}

		// The following method converts string representations of NpgsqlDbType-Members
		// into NpgsqlDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			if (String.Compare(typeName, "bool", true) == 0)
				return NpgsqlDbType.Boolean;
			if (String.Compare(typeName, "int8", true) == 0)
				return NpgsqlDbType.Bigint;
			if (String.Compare(typeName, "bytea", true) == 0)
				return NpgsqlDbType.Bytea;
			if (String.Compare(typeName, "date", true) == 0)
				return NpgsqlDbType.Date;
			if (String.Compare(typeName, "float8", true) == 0)
				return NpgsqlDbType.Double;
			if (String.Compare(typeName, "int4", true) == 0)
				return NpgsqlDbType.Integer;
			if (String.Compare(typeName, "money", true) == 0)
				return NpgsqlDbType.Money;
			if (String.Compare(typeName, "numeric", true) == 0)
				return NpgsqlDbType.Numeric;
			if (String.Compare(typeName, "float4", true) == 0)
				return NpgsqlDbType.Real;
			if (String.Compare(typeName, "int2", true) == 0)
				return NpgsqlDbType.Smallint;
			if (String.Compare(typeName, "text", true) == 0)
				return NpgsqlDbType.Text;
			if (String.Compare(typeName, "time", true) == 0)
				return NpgsqlDbType.Time;
			if (String.Compare(typeName, "timestamp", true) == 0)
				return NpgsqlDbType.Timestamp;
            if (String.Compare(typeName, "timetz", true) == 0)
                return NpgsqlDbType.Time;
            if (String.Compare(typeName, "timestamptz", true) == 0)
                return NpgsqlDbType.Timestamp;
            if (String.Compare(typeName, "varchar", true) == 0)
				return NpgsqlDbType.Varchar;
			throw new NDOException(27, "NDONpgsql.Provider.GetDbType: Typname " + typeName + " kann nicht in NpgsqlDbType konvertiert werden");
		}

		private string GetDateExpression(System.DateTime dt)
		{
			//'9999-12-31 23:59:59'
			return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
		}

		public override int GetDefaultLength(string typeName)
		{
            if (String.Compare(typeName, "bool", true) == 0)
				return 1;
			if (String.Compare(typeName, "int8", true) == 0)
				return 8;
			if (String.Compare(typeName, "bytea", true) == 0)
				return 1024;
			if (String.Compare(typeName, "date", true) == 0)
				return 16;
			if (String.Compare(typeName, "float8", true) == 0)
				return 8;
			if (String.Compare(typeName, "int4", true) == 0)
				return 4;
			if (String.Compare(typeName, "money", true) == 0)
				return 10;
			if (String.Compare(typeName, "numeric", true) == 0)
				return 10;
			if (String.Compare(typeName, "float4", true) == 0)
				return 4;
			if (String.Compare(typeName, "int2", true) == 0)
				return 2;
			if (String.Compare(typeName, "text", true) == 0)
				return 255;
			if (String.Compare(typeName, "time", true) == 0)
				return 16;
			if (String.Compare(typeName, "timestamp", true) == 0)
				return 16;
            if (String.Compare(typeName, "timetz", true) == 0)
                return 16;
            if (String.Compare(typeName, "timestamptz", true) == 0)
                return 16;
            if (String.Compare(typeName, "varchar", true) == 0)
				return 255;
			return 0;		
		}
	

		public override int GetDefaultLength(System.Type t)
		{
            t = base.ConvertNullableType(t);
            if (t == typeof(bool))
				return 1;
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
				return 18;
			else if ( t == typeof(System.DateTime))
				return 16;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return 4;
			else
				return 0;		}

        [Obsolete]
		public override Type GetSystemType(string s)
		{
			System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame();
			throw new Exception("Obsolete method GetSystemType " + s + " called.\n" + sf.ToString());
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
			get { return false; }
		}


		/// <summary>
		/// Gets an expression in the SQL dialect of the database, which retrieves the ID of the last
		/// inserted row, if the ID is automatically generated by the database.
		/// </summary>
		public override string GetLastInsertedId(string tableName, string columnName)
		{
            return null;
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
            NpgsqlDataAdapter a = new NpgsqlDataAdapter("Select * from pg_tables", (NpgsqlConnection)conn);
            DataSet ds = new DataSet();
            a.Fill(ds);
            DataTable dt = ds.Tables[0];
            ArrayList al = new ArrayList();
            bool hasOwner = (owner != null || owner != "");
            foreach (DataRow dr in dt.Rows)
            {
                string sname = (string) dr["schemaname"];
                if (sname == "information_schema")
                    continue;
                if (sname == "pg_catalog")
                    continue;
                if (hasOwner && sname != owner)
                    continue;
                    
                al.Add(dr["tablename"]);
            }
            string[] strresult = new string[al.Count];
			for (int i = 0; i < al.Count; i++)
				strresult[i] = (string) al[i];
			return strresult;
		}
	
		public override string[] TypeNames
		{
			get
			{				
				return Enum.GetNames(typeof(NpgsqlDbType));
			}
		}

		public override string Name { get { return "Postgre"; }  }

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

		public override System.Windows.Forms.DialogResult ShowConnectionDialog(ref string connectionString)
		{
            Npgsql.Design.ConnectionStringEditorForm csef = new Npgsql.Design.ConnectionStringEditorForm(connectionString);
            if (csef.ShowDialog() == DialogResult.Cancel)
				return DialogResult.Cancel;
			connectionString = csef.ConnectionString;
			return DialogResult.OK;
		}

		public override string CreateDatabase(object necessaryData)
		{
			base.CreateDatabase (necessaryData);
			// Don't need to check wether the type of necessaryData is OK, since that happens in CreateDatabase
			NDOCreateDbParameter par = necessaryData as NDOCreateDbParameter;

			string dbName = par.DatabaseName;
			Regex regex = new Regex(@"Database\s*=([^\;]*)");
			Match match = regex.Match(par.Connection);
			if (match.Success)
			{
				return par.Connection.Substring(0, match.Groups[1].Index) + dbName + par.Connection.Substring(match.Index + match.Length);
			}
			
			if (!par.Connection.EndsWith(";"))
				par.Connection = par.Connection + ";";
			
			return par.Connection + "Database=" + dbName;
		}


	}
}
