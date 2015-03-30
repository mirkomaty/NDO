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
//	In diesem Beispiel wird Ihr Adapter unter dem Namen "Firebird" in die ProviderFactory eingetragen.
//	Den String "Firebird" (oder welchen String auch immer Sie zur Kennzeichnung benötigen)
//	müssen Sie in der NDOMapping.xml als Attribut "Type" des Connection-Eintrags 
//	angeben.


using System;
using System.Text;
using System.Data;
using System.Data.Common;
using NDO;
using System.Collections;
using FirebirdSql.Data.FirebirdClient;
using System.Globalization;
using System.Threading;
using NDOInterfaces;
using System.Windows.Forms;

namespace NDO.FirebirdProvider
{
	/// <summary>
	/// Sample adapter class to connect new ADO.NET Providers with NDO.
	/// </summary>
	public class Provider : NDOAbstractProvider
	{
		// The following methods provide objects of provider classes 
		// which implement common interfaces in .NET:
		// IDbConnection, IDbCommand, DbDataAdapter and the Parameter objects
		#region Provide specialized type objects
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new FbConnection(connectionString);
		}

		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			FbCommand command = new FbCommand();
			command.Connection = (FbConnection)connection;
			return command;
		}

		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
		{
			FbDataAdapter da = new FbDataAdapter();
			da.SelectCommand = (FbCommand)select;
			da.UpdateCommand = (FbCommand)update;
			da.InsertCommand = (FbCommand)insert;
			da.DeleteCommand = (FbCommand)delete;
			return da;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new FbCommandBuilder((FbDataAdapter)dataAdapter);
		}


		public override IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
			return ((FbCommand)command).Parameters.Add(new FbParameter(parameterName, (FbDbType)dbType, size, columnName));			
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			return ((FbCommand)command).Parameters.Add(new FbParameter(parameterName, (FbDbType)dbType, size, dir, isNullable, precision, scale, srcColumn, srcVersion, value));
		}
		#endregion

		// The following method convert System.Type objects to FbDbType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide FbDbType members
		public override object GetDbType(Type t) 
		{
#if !NDO11
            t = base.ConvertNullableType(t);
#endif
			if ( t == typeof(bool) )
				return FbDbType.SmallInt;
			else if ( t == typeof(byte) )
				return FbDbType.SmallInt;
			else if ( t == typeof(sbyte) )
				return FbDbType.SmallInt;
			else if ( t == typeof(char) )
				return FbDbType.SmallInt;
			else if ( t == typeof(short))
				return FbDbType.SmallInt;
			else if ( t == typeof(ushort))
				return FbDbType.SmallInt;
			else if ( t == typeof(int))
				return FbDbType.Integer;
			else if ( t == typeof(uint))
				return FbDbType.Integer;
			else if ( t == typeof(long))
				return FbDbType.BigInt;
			else if ( t == typeof(System.Guid))
				return FbDbType.Char;
			else if ( t == typeof(ulong))
				return FbDbType.BigInt;
			else if ( t == typeof(float))
				return FbDbType.Numeric;
			else if ( t == typeof(double))
				return FbDbType.Numeric;
			else if ( t == typeof(string))
				return FbDbType.VarChar;
			else if ( t == typeof(byte[]))
				return FbDbType.Binary;
			else if ( t == typeof(decimal))
				return FbDbType.Decimal;
			else if ( t == typeof(System.DateTime))
				return FbDbType.Date;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return FbDbType.Integer;
			else
				throw new NDOException(27, "NDOFirebirdProvider.GetDbType: Typ " + t.Name + " kann nicht in FbDbType konvertiert werden");
		}

		// The following method converts string representations of FbDbType-Members
		// into FbDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			if (typeName == "Array")
				return FbDbType.Array;
			if (typeName == "BigInt")
				return FbDbType.BigInt;
			if (typeName == "Binary")
				return FbDbType.Binary;
			if (typeName == "Char")
				return FbDbType.Char;
			if (typeName == "Date")
				return FbDbType.Date;
			if (typeName == "Decimal")
				return FbDbType.Decimal;
			if (typeName == "Double")
				return FbDbType.Double;
			if (typeName == "Float")
				return FbDbType.Float;
			if (typeName == "Guid")
				return FbDbType.Guid;
			if (typeName == "Integer")
				return FbDbType.Integer;
			if (typeName == "Numeric")
				return FbDbType.Numeric;
			if (typeName == "SmallInt")
				return FbDbType.SmallInt;
			if (typeName == "Text")
				return FbDbType.Text;
			if (typeName == "Time")
				return FbDbType.Time;
			if (typeName == "TimeStamp")
				return FbDbType.TimeStamp;
			if (typeName == "VarChar")
				return FbDbType.VarChar;			
			throw new NDOException(27, "NDOFb.Provider.GetDbType: Typname " + typeName + " kann nicht in FbDbType konvertiert werden");
		}

		private string GetDateExpression(System.DateTime dt)
		{
			//'9999-12-31 23:59:59'
			//'31-Dec-9999'
			CultureInfo ci = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new CultureInfo(1033);
			string result = "'" + dt.ToString("dd-MMM-yyyy") + "'";
			Thread.CurrentThread.CurrentCulture = ci;
			return result;
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
			return "@" + plainParameterName;
		}
	
		public override string GetQuotedName(string plainName)
		{
			return "\"" + plainName + "\"";
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
			return string.Empty;
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
			FbCommand cmd = new FbCommand("SELECT RDB$RELATION_NAME FROM RDB$RELATIONS  WHERE RDB$SYSTEM_FLAG = 0", (FbConnection) conn);
			bool wasOpen = true;
			if (conn.State == ConnectionState.Closed)
			{
				wasOpen = false;
				conn.Open();
			}
			FbDataReader dr = cmd.ExecuteReader();
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
				return Enum.GetNames(typeof(FbDbType));
			}
		}

		public override string Name { get { return "Firebird"; }  }

		public override bool SupportsInsertBatch
		{
			get
			{
				return false;
			}
		}

		public override bool SupportsNativeGuidType 
		{ 
			get { return false; } 
		}

		public override System.Windows.Forms.DialogResult ShowCreateDbDialog(ref object necessaryData)
		{
			FbConnectionStringBuilder sb = necessaryData as FbConnectionStringBuilder;
			if (sb == null)
			{
				sb = new FbConnectionStringBuilder();
				sb.UserID = "SYSDBA";
				sb.Password = "masterkey";
				sb.DataSource = "localhost";
				sb.Database = string.Empty;
			}
			sb.Dialect = 3;
			ConnectionDialog dlg = new ConnectionDialog(sb);
			dlg.CreateDatabase = true;
			DialogResult result = dlg.ShowDialog();
			if (result == DialogResult.Cancel)
				return result;
			necessaryData = sb;
			return result;
		}

		public override string CreateDatabase(object necessaryData)
		{
			FbConnectionStringBuilder sb = necessaryData as FbConnectionStringBuilder;
			if (sb == null)
				throw new ArgumentException("FirebirdProvider: parameter type " + necessaryData.GetType().FullName + " is wrong.", "necessaryData");
			try
			{
				sb.Dialect = 3;
				FbConnection.CreateDatabase(sb.ConnectionString);
			}
			catch (Exception ex)
			{
				throw new NDOException(19, "Error while attempting to create a database: Exception Type: " + ex.GetType().Name + " Message: " + ex.Message + "\nConnection String: " + sb.ConnectionString);
			}
			return sb.ConnectionString;
		}

		public override DialogResult ShowConnectionDialog(ref string connectionString)
		{
			string cs;
			if (connectionString != null)
				cs = connectionString;
			else
				cs = string.Empty;
			FbConnectionStringBuilder sb = new FbConnectionStringBuilder(cs);
			sb.Dialect = 3;
			if (cs == string.Empty)
			{
				sb.UserID = "SYSDBA";
				sb.Password = "masterkey";
				sb.DataSource = "localhost";
				sb.Database = string.Empty;
			}
			ConnectionDialog dlg = new ConnectionDialog(sb);
			if (dlg.ShowDialog() == DialogResult.Cancel)
				return DialogResult.Cancel;
			connectionString = sb.ConnectionString;
			return DialogResult.OK;
		}

	}
}
