//
// Copyright (C) 2002-2011 Mirko Matytschak 
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
//  Hier wird die Anbindung am Beispiel der Oracle-Datenbank gezeigt. Beachten Sie:
//  NDO enthält bereits eine Oracle-Anbindung.
//  Für die Anbindung Ihres eigenen Providers benötigen Sie ein Kürzel, 
//  das Sie in der Mapping-Datei und in der Anmeldung des Providers bei
//  der Provider-Factory von NDO benötigen. In diesem Beispiel ist das Kürzel "Oracle". 
//	Zur Anmeldung des Providers benötigen Sie in Ihrem Code eine Zeile wie die folgende:
//  	NDOProviderFactory.Instance["Oracle"] = new OracleAdapter.Provider();
//	bevor Sie mit Persistenz-Funktionen arbeiten.
//	Hiermit wird Ihr Adapter unter dem Namen "Oracle" in die ProviderFactory eingetragen.
//	Den String "Oracle" (oder welchen String auch immer Sie zur Kennzeichnung auswählen)
//	müssen Sie in der NDOMapping.xml als Attribut "Type" des Connection-Eintrags 
//	angeben.

using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using NDO;
using System.Collections;
using System.Text.RegularExpressions;
using NDOInterfaces;
using System.Windows.Forms;

namespace NDO
{
	/// <summary>
	/// Sample adapter class to connect new ADO.NET Providers with NDO.
	/// This Adapter is based on the Oracle Provider in .NET Framework 1.1
	/// </summary>
	public class NDOOracleProvider : NDOAbstractProvider
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
			return cmd.Parameters.Add(new OracleParameter(parameterName, (OracleType)dbType, size > -1 ? size : 0, columnName));			
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			// Cast notwendig, damit der DbType richtig übersetzt wird
			OracleCommand cmd = (OracleCommand) command;
			return cmd.Parameters.Add(new OracleParameter(parameterName, (OracleType)dbType, size > -1 ? size : 0, dir, isNullable, precision, scale, srcColumn, srcVersion, value));
		}
		#endregion


		const string OraString = "Provider=MSDAORA.1;";
		public override DialogResult ShowConnectionDialog(ref string connectionString)
		{
			string tempstr;
			if (connectionString == null || connectionString == string.Empty)
				tempstr = OraString;
			else
			{
				if (!(connectionString.IndexOf(OraString) > -1))
					tempstr = OraString + connectionString;
				else
					tempstr = connectionString;
			}
			OleDbConnectionDialog dlg = new OleDbConnectionDialog(tempstr);
			DialogResult result = dlg.Show();
			if (result != DialogResult.Cancel)
			{
				connectionString = dlg.ConnectionString.Replace(OraString, string.Empty);
			}
			return result;
		}


		// The following method convert System.Type objects to OracleType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide OracleType members
		public override object GetDbType(Type t) 
		{
			t = base.ConvertNullableType(t);
			if (t == typeof(bool))
				return OracleType.Number;
			else if ( t == typeof(byte) )
				return OracleType.Number;
			else if ( t == typeof(sbyte) )
				return OracleType.Number;
			else if ( t == typeof(char) )
				return OracleType.Char;
			else if ( t == typeof(short))
				return OracleType.Number;
			else if ( t == typeof(ushort))
				return OracleType.Number;
			else if ( t == typeof(int))
				return OracleType.Number;
			else if ( t == typeof(uint))
				return OracleType.Number;
			else if ( t == typeof(long))
				return OracleType.Number;
			else if ( t == typeof(System.Guid))
				return OracleType.Char;
			else if ( t == typeof(ulong))
				return OracleType.Number;
			else if ( t == typeof(float))
				return OracleType.Number;
			else if ( t == typeof(double))
				return OracleType.Number;
			else if ( t == typeof(string))
				return OracleType.VarChar;
			else if ( t == typeof(byte[]))
				return OracleType.Raw;
			else if ( t == typeof(decimal))
				return OracleType.Number;
			else if ( t == typeof(System.DateTime))
				return OracleType.DateTime;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return OracleType.Number;
			else
				throw new NDOException(30, "NDOOracleProvider.GetDbType: Type " + t.Name + " can't be converted into an OracleType.");
		}

		// This method converts string representations of OracleType-Members
		// into OracleType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			if (typeName == "BFile") return OracleType.BFile;
			if (typeName == "Blob") return OracleType.Blob;
			if (typeName == "Byte") return OracleType.Byte;
			if (typeName == "Char") return OracleType.Char;
			if (typeName == "Clob") return OracleType.Clob;
			if (typeName == "Cursor") return OracleType.Cursor;
			if (typeName == "DateTime") return OracleType.DateTime;
			if (typeName == "Double") return OracleType.Double;
			if (typeName == "Float") return OracleType.Float;
			if (typeName == "Int16") return OracleType.Int16;
			if (typeName == "Int32") return OracleType.Int32;
			if (typeName == "IntervalDayToSecond") return OracleType.IntervalDayToSecond;
			if (typeName == "IntervalYearToMonth") return OracleType.IntervalYearToMonth;
			if (typeName == "LongRaw") return OracleType.LongRaw;
			if (typeName == "LongVarChar") return OracleType.LongVarChar;
			if (typeName == "NChar") return OracleType.NChar;
			if (typeName == "NClob") return OracleType.NClob;
			if (typeName == "Number") return OracleType.Number;
			if (typeName == "NVarChar") return OracleType.NVarChar;
			if (typeName == "Raw") return OracleType.Raw;
			if (typeName == "RowId") return OracleType.RowId;
			if (typeName == "SByte") return OracleType.SByte;
			if (typeName == "Timestamp") return OracleType.Timestamp;
			if (typeName == "TimestampLocal") return OracleType.TimestampLocal;
			if (typeName == "TimestampWithTZ") return OracleType.TimestampWithTZ;
			if (typeName == "UInt16") return OracleType.UInt16;
			if (typeName == "UInt32") return OracleType.UInt32;
			if (typeName == "VarChar") return OracleType.VarChar;
			throw new NDOException(31, "NDOOracleProvider.GetDbType: Typ name " + typeName + " can't be converted into an OracleType");
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
		public override int GetDefaultLength(string typeName)
		{
			if (typeName == "BFile") return 0;
			if (typeName == "Blob") return 0;
			if (typeName == "Byte") return 3;
			if (typeName == "Char") return 5;
			if (typeName == "Clob") return 0;
			if (typeName == "Cursor") return 0;
			if (typeName == "DateTime") return 0;
			if (typeName == "Double") return 30;
			if (typeName == "Float") return 30;
			if (typeName == "Int16") return 5;
			if (typeName == "Int32") return 10;
			if (typeName == "IntervalDayToSecond") return 0;
			if (typeName == "IntervalYearToMonth") return 0;
			if (typeName == "LongRaw") return 0;
			if (typeName == "LongVarChar") return 255;
			if (typeName == "NChar") return 255;
			if (typeName == "NClob") return 1000;
			if (typeName == "Number") return 20;
			if (typeName == "NVarChar") return 255;
			if (typeName == "Raw") return 255;
			if (typeName == "RowId") return 0;
			if (typeName == "SByte") return 3;
			if (typeName == "Timestamp") return 16;
			if (typeName == "TimestampLocal") return 16;
			if (typeName == "TimestampWithTZ") return 16;
			if (typeName == "UInt16") return 5;
			if (typeName == "UInt32") return 10;
			if (typeName == "VarChar") return 255;
			return 0;		
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
				return 0;		}

		public override Type GetSystemType(string s)
		{
			System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame();
			throw new Exception("GetSystemType " + s + " aufgerufen. " + sf.ToString());
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
				return Enum.GetNames(typeof(OracleType));
			}
		}


		public override string Name { get { return "Oracle"; }  }

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override bool SupportsNativeGuidType { get { return false; } }


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		/// <remarks>Note, that Oracle doesn't allow to programmatically create a database.</remarks>
		public override string CreateDatabase(object necessaryData)
		{
			return string.Empty;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override DialogResult ShowCreateDbDialog(ref object necessaryData)
		{
			MessageBox.Show("Oracle doesn't allow to programmatically create a database.\nPlease use the Enterprise Manager Console.");
			return DialogResult.Cancel;
		}


		#endregion

	}
}
