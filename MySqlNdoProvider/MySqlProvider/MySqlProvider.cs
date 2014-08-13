//
// Copyright (C) 2002-2014 HoT - Mirko Matytschak 
// (www.netdataobjects.de)
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
//	In diesem Beispiel wird Ihr Adapter unter dem Namen "MySql" in die ProviderFactory eingetragen.
//	Den String "MySql" (oder welchen String auch immer Sie zur Kennzeichnung benötigen)
//	müssen Sie in der NDOMapping.xml als Attribut "Type" des Connection-Eintrags 
//	angeben.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using NDOInterfaces;
using System.Collections;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Windows.Forms;

namespace NDO.MySqlProvider
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
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new MySqlConnection(connectionString);
		}

		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			MySqlCommand command = new MySqlCommand();
			command.Connection = (MySqlConnection)connection;
			return command;
		}

		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
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


		public override IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
			return ((MySqlCommand)command).Parameters.Add(new MySqlParameter(parameterName, (MySql.Data.MySqlClient.MySqlDbType)dbType, size, columnName));			
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			return ((MySqlCommand)command).Parameters.Add(new MySqlParameter(parameterName, (MySql.Data.MySqlClient.MySqlDbType)dbType, size, dir, isNullable, precision, scale, srcColumn, srcVersion, value));
		}
		#endregion

		// The following method convert System.Type objects to MySql.Data.MySqlClient.MySqlDbType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide MySql.Data.MySqlClient.MySqlDbType members
		public override object GetDbType(Type t) 
		{
            t = base.ConvertNullableType(t);
			if ( t == typeof(bool) )
				return MySql.Data.MySqlClient.MySqlDbType.Byte;
			else if ( t == typeof(byte) )
				return MySql.Data.MySqlClient.MySqlDbType.Byte;
			else if ( t == typeof(sbyte) )
				return MySql.Data.MySqlClient.MySqlDbType.Byte;
			else if ( t == typeof(char) )
				return MySql.Data.MySqlClient.MySqlDbType.Int16;
			else if ( t == typeof(short))
				return MySql.Data.MySqlClient.MySqlDbType.Int16;
			else if ( t == typeof(ushort))
				return MySql.Data.MySqlClient.MySqlDbType.Int16;
			else if ( t == typeof(int))
				return MySql.Data.MySqlClient.MySqlDbType.Int32;
			else if ( t == typeof(uint))
				return MySql.Data.MySqlClient.MySqlDbType.Int32;
			else if ( t == typeof(long))
				return MySql.Data.MySqlClient.MySqlDbType.Int64;
			else if ( t == typeof(System.Guid))
				return MySql.Data.MySqlClient.MySqlDbType.VarChar;
			else if ( t == typeof(ulong))
				return MySql.Data.MySqlClient.MySqlDbType.Int64;
			else if ( t == typeof(float))
				return MySql.Data.MySqlClient.MySqlDbType.Float;
			else if ( t == typeof(double))
				return MySql.Data.MySqlClient.MySqlDbType.Double;
			else if ( t == typeof(string))
				return MySql.Data.MySqlClient.MySqlDbType.VarChar;
			else if ( t == typeof(byte[]))
				return MySql.Data.MySqlClient.MySqlDbType.MediumBlob;
			else if ( t == typeof(decimal))
				return MySql.Data.MySqlClient.MySqlDbType.Decimal;
			else if ( t == typeof(System.DateTime))
				return MySql.Data.MySqlClient.MySqlDbType.Datetime;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return MySql.Data.MySqlClient.MySqlDbType.Int32;
			else
				throw new NDOException(27, "NDO.MySqlProvider.GetDbType: Typ " + t.Name + " kann nicht in MySql.Data.MySqlClient.MySqlDbType konvertiert werden");
		}

		// The following method converts string representations of MySql.Data.MySqlClient.MySqlDbType-Members
		// into MySql.Data.MySqlClient.MySqlDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			if (typeName == "BigInt")
				return MySql.Data.MySqlClient.MySqlDbType.Int64;
			if (typeName == "Blob")
				return MySql.Data.MySqlClient.MySqlDbType.Blob;
			if (typeName == "Byte")
				return MySql.Data.MySqlClient.MySqlDbType.Byte;
			if (typeName == "Date")
				return MySql.Data.MySqlClient.MySqlDbType.Date;
			if (typeName == "Datetime")
				return MySql.Data.MySqlClient.MySqlDbType.Datetime;
			if (typeName == "Decimal")
				return MySql.Data.MySqlClient.MySqlDbType.Decimal;
			if (typeName == "Double")
				return MySql.Data.MySqlClient.MySqlDbType.Double;
			if (typeName == "Enum")
				return MySql.Data.MySqlClient.MySqlDbType.Enum;
			if (typeName == "Float")
				return MySql.Data.MySqlClient.MySqlDbType.Float;
			if (typeName == "Int32")
				return MySql.Data.MySqlClient.MySqlDbType.Int32;
			if (typeName == "Int24")
				return MySql.Data.MySqlClient.MySqlDbType.Int24;
			if (typeName == "Long")
				return MySql.Data.MySqlClient.MySqlDbType.Int64;
			if (typeName == "LongBlob")
				return MySql.Data.MySqlClient.MySqlDbType.LongBlob;
			if (typeName == "LongLong")
				return MySql.Data.MySqlClient.MySqlDbType.Int64;
			if (typeName == "MediumBlob")
				return MySql.Data.MySqlClient.MySqlDbType.MediumBlob;
			if (typeName == "Newdate")
				return MySql.Data.MySqlClient.MySqlDbType.Newdate;
			if (typeName == "Set")
				return MySql.Data.MySqlClient.MySqlDbType.Set;
			if (typeName == "Int16")
				return MySql.Data.MySqlClient.MySqlDbType.Int16;
			if (typeName == "String")
				return MySql.Data.MySqlClient.MySqlDbType.String;
			if (typeName == "Time")
				return MySql.Data.MySqlClient.MySqlDbType.Time;
			if (typeName == "Timestamp")
				return MySql.Data.MySqlClient.MySqlDbType.Timestamp;
			if (typeName == "TinyBlob")
				return MySql.Data.MySqlClient.MySqlDbType.TinyBlob;
			if (typeName == "VarChar")
				return MySql.Data.MySqlClient.MySqlDbType.VarChar;
			if (typeName == "Year")
				return MySql.Data.MySqlClient.MySqlDbType.Year;			
			throw new NDOException(27, "NDOMySql.Provider.GetDbType: Typname " + typeName + " kann nicht in MySql.Data.MySqlClient.MySqlDbType konvertiert werden");
		}

		private string GetDateExpression(System.DateTime dt)
		{
			//'9999-12-31 23:59:59'
			return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
		}

		public override int GetDefaultLength(string typeName)
		{
			if (typeName == "BigInt")
				return 8;
			if (typeName == "Blob")
				return 1024;
			if (typeName == "Byte")
				return 0;
			if (typeName == "Date")
				return 0;
			if (typeName == "Datetime")
				return 16;
			if (typeName == "Decimal")
				return 18;
			if (typeName == "Double")
				return 8;
			if (typeName == "Enum")
				return 0;
			if (typeName == "Float")
				return 4;
			if (typeName == "Int")
				return 4;
			if (typeName == "Int24")
				return 3;
			if (typeName == "Long")
				return 8;
			if (typeName == "LongBlob")
				return 2048;
			if (typeName == "LongLong")
				return 1000000;
			if (typeName == "MediumBlob")
				return 100000;
			if (typeName == "Newdate")
				return 0;
			if (typeName == "Null")
				return 0;
			if (typeName == "Set")
				return 0;
			if (typeName == "Short")
				return 2;
			if (typeName == "String")
				return 255;
			if (typeName == "Time")
				return 0;
			if (typeName == "Timestamp")
				return 0;
			if (typeName == "TinyBlob")
				return 1024;
			if (typeName == "VarChar")
				return 255;
			if (typeName == "Year")
				return 0;
			return 0;		
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
				return 0;		}

		public override Type GetSystemType(string s)
		{
			System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame();
			throw new Exception("GetSystemType " + s + " aufgerufen. " + sf.ToString());
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
				return Enum.GetNames(typeof(MySql.Data.MySqlClient.MySqlDbType));
			}
		}

		public override string Name { get { return "MySql"; }  }

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

		public override System.Windows.Forms.DialogResult ShowConnectionDialog(ref string connectionString)
		{
			ConnectionDialog dlg = new ConnectionDialog(connectionString, false);
			if (dlg.ShowDialog() == DialogResult.Cancel)
				return DialogResult.Cancel;
			connectionString = dlg.ConnectionString;
			return DialogResult.OK;
		}

		public override string CreateDatabase(object necessaryData)
		{
			base.CreateDatabase (necessaryData);
			// Don't need to check, if type is OK, since that happens in CreateDatabase
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

		public override DialogResult ShowCreateDbDialog(ref object necessaryData)
		{
			NDOCreateDbParameter par;
			if (necessaryData == null)
				par = new NDOCreateDbParameter(string.Empty, "Data Source=localhost;User Id=root;");
			else
				par = necessaryData as NDOCreateDbParameter;
			if (par == null)
				throw new ArgumentException("MySql provider: parameter type " + necessaryData.GetType().FullName + " is wrong.", "necessaryData");
			if (par.Connection == null || par.Connection == string.Empty)
				par.Connection = "Data Source=localhost;User Id=root";
			ConnectionDialog dlg = new ConnectionDialog(par.Connection, true);
			if (dlg.ShowDialog() == DialogResult.Cancel)
				return DialogResult.Cancel;
			par.Connection = dlg.ConnectionString;
			par.DatabaseName = dlg.Database;
			necessaryData = par;
			return DialogResult.OK;
		}



	}
}
