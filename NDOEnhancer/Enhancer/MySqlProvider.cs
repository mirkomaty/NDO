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


#if nix
using System;
using System.Data;
using System.Data.Common;
using NDO;

namespace NDOEnhancer
{
	// Achtung, diese Typen entsprechen nicht den ByteFX-Typen.
	// Sie listen direkt die Column Types der MySql-Datenbank auf,
	// die benötigt werden, um die speicherbaren Datentypen von NDO
	// abzubilden.
	internal enum MySqlDbType
	{
		Tinyint,
		Blob,
		Smallint,
		Int,
		BigInt,
		TinyBlob,
		Float,
		Double,
		Varchar,
		Decimal,
		Datetime
	}
	/// <summary>
	/// Zusammenfassung für MySqlProvider.
	/// </summary>
	internal class MySqlProvider : NDO.NDOAbstractProvider
	{
		// The following methods provide objects of provider classes 
		// which implement common interfaces in .NET:
		// IDbConnection, IDbCommand, DbDataAdapter and the Parameter objects
		// Der Provider ist eine Teilimplementierung und wird ins Projekt hineingezogen 
		// um Abhängigkeiten von ByteFX zu vermeiden. Alle Verweise auf die ByteFX-Lib 
		// sind entfernt.
		#region IProvider-Funktionen, die nicht implementiert werden
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return null;
		}

		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			return null;
		}

		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
		{
			return null;
		}

		public override void AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
		}

		public override void AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
		}
		#endregion

		// The following method convert System.Type objects to MySqlDbType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide MySqlDbType members
		public override object GetDbType(Type t) 
		{
			if ( t == typeof(bool) )
				return MySqlDbType.Tinyint;
			else if ( t == typeof(byte) )
				return MySqlDbType.Tinyint;
			else if ( t == typeof(sbyte) )
				return MySqlDbType.Smallint;
			else if ( t == typeof(char) )
				return MySqlDbType.Smallint;
			else if ( t == typeof(short))
				return MySqlDbType.Smallint;
			else if ( t == typeof(ushort))
				return MySqlDbType.Smallint;
			else if ( t == typeof(int))
				return MySqlDbType.Int;
			else if ( t == typeof(uint))
				return MySqlDbType.Int;
			else if ( t == typeof(long))
				return MySqlDbType.BigInt;
			else if ( t == typeof(System.Guid))
				return MySqlDbType.TinyBlob;
			else if ( t == typeof(ulong))
				return MySqlDbType.BigInt;
			else if ( t == typeof(float))
				return MySqlDbType.Float;
			else if ( t == typeof(double))
				return MySqlDbType.Double;
			else if ( t == typeof(string))
				return MySqlDbType.Varchar;
			else if ( t == typeof(byte[]))
				return MySqlDbType.Blob;
			else if ( t == typeof(decimal))
				return MySqlDbType.Decimal;
			else if ( t == typeof(System.DateTime))
				return MySqlDbType.Datetime;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return MySqlDbType.Int;
			else
				throw new NDOException("NDOMySqlProvider.GetDbType: Typ " + t.Name + " kann nicht in MySqlDbType konvertiert werden");
		}

		// The following method converts string representations of MySqlDbType-Members
		// into MySqlDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			throw new NotImplementedException("GetDbType(string) im MySqlProvider");
			/*
			if (typeName == "BigInt")
				return MySqlDbType.BigInt;
			if (typeName == "Blob")
				return MySqlDbType.Blob;
			if (typeName == "Byte")
				return MySqlDbType.Byte;
			if (typeName == "Date")
				return MySqlDbType.Date;
			if (typeName == "Datetime")
				return MySqlDbType.Datetime;
			if (typeName == "Decimal")
				return MySqlDbType.Decimal;
			if (typeName == "Double")
				return MySqlDbType.Double;
			if (typeName == "Enum")
				return MySqlDbType.Enum;
			if (typeName == "Float")
				return MySqlDbType.Float;
			if (typeName == "Int")
				return MySqlDbType.Int;
			if (typeName == "Int24")
				return MySqlDbType.Int24;
			if (typeName == "Long")
				return MySqlDbType.BigInt;
			if (typeName == "LongBlob")
				return MySqlDbType.LongBlob;
			if (typeName == "LongLong")
				return MySqlDbType.BigInt;
			if (typeName == "MediumBlob")
				return MySqlDbType.MediumBlob;
			if (typeName == "Newdate")
				return MySqlDbType.Newdate;
			if (typeName == "Null")
				return MySqlDbType.Null;
			if (typeName == "Set")
				return MySqlDbType.Set;
			if (typeName == "Short")
				return MySqlDbType.Short;
			if (typeName == "String")
				return MySqlDbType.String;
			if (typeName == "Time")
				return MySqlDbType.Time;
			if (typeName == "Timestamp")
				return MySqlDbType.Timestamp;
			if (typeName == "TinyBlob")
				return MySqlDbType.TinyBlob;
			if (typeName == "VarChar")
				return MySqlDbType.VarChar;
			if (typeName == "Year")
				return MySqlDbType.Year;			
			throw new NDOException("NDOMySql.Provider.GetDbType: Typname " + typeName + " kann nicht in MySqlDbType konvertiert werden");
			*/
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return null;
		}

		private string GetDateExpression(System.DateTime dt)
		{
			//'9999-12-31 23:59:59'
			return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "'";
		}

		public override int GetDefaultLength(string typeName)
		{
			if (typeName == "BigInt")
				return 0;
			if (typeName == "Blob")
				return 0;
			if (typeName == "Byte")
				return 0;
			if (typeName == "Date")
				return 0;
			if (typeName == "Datetime")
				return 0;
			if (typeName == "Decimal")
				return 18;
			if (typeName == "Double")
				return 0;
			if (typeName == "Enum")
				return 0;
			if (typeName == "Float")
				return 0;
			if (typeName == "Int")
				return 0;
			if (typeName == "Int24")
				return 0;
			if (typeName == "Long")
				return 0;
			if (typeName == "LongBlob")
				return 0;
			if (typeName == "LongLong")
				return 0;
			if (typeName == "MediumBlob")
				return 0;
			if (typeName == "Newdate")
				return 0;
			if (typeName == "Null")
				return 0;
			if (typeName == "Set")
				return 0;
			if (typeName == "Short")
				return 0;
			if (typeName == "String")
				return 255;
			if (typeName == "Time")
				return 0;
			if (typeName == "Timestamp")
				return 0;
			if (typeName == "TinyBlob")
				return 0;
			if (typeName == "VarChar")
				return 255;
			if (typeName == "Year")
				return 0;
			throw new NDOException("NDO.MySql.Provider.GetDefaultLength: Typ " + typeName + " kann nicht in MySqlDbType konvertiert werden");		
		}
	

		public override int GetDefaultLength(System.Type t)
		{
			if ( t == typeof(bool) )
				return 0;
			else if ( t == typeof(byte) )
				return 0;
			else if ( t == typeof(sbyte) )
				return 0;
			else if ( t == typeof(char) )
				return 0;
			else if ( t == typeof(short))
				return 0;
			else if ( t == typeof(ushort))
				return 0;
			else if ( t == typeof(int))
				return 0;
			else if ( t == typeof(uint))
				return 0;
			else if ( t == typeof(long))
				return 0;
			else if ( t == typeof(System.Guid))
				return 0;
			else if ( t == typeof(ulong))
				return 0;
			else if ( t == typeof(float))
				return 0;
			else if ( t == typeof(double))
				return 0;
			else if ( t == typeof(string))
				return 255;
			else if ( t == typeof(byte[]))
				return 0;
			else if ( t == typeof(decimal))
				return 18;
			else if ( t == typeof(System.DateTime))
				return 0;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return 0;			
			else
				throw new NDOException("NDOMySqlProvider.GetDefaultLength: Typ " + t.Name + " kann nicht in MySqlDbType konvertiert werden");		}

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
			/*
						if (namedParameters.Contains(plainParameterName))
							return (string) namedParameters[plainParameterName];
						string paramName = ":PARAM" + namedParameters.Count.ToString();
						namedParameters.Add(plainParameterName, paramName);
						return paramName;
						*/
			return "@" + plainParameterName;
		}
	
		public override string GetQuotedName(string plainName)
		{
			return "`" + plainName + "`";
		}
	
		public override string[] GetTableNames(IDbConnection conn, string owner)
		{
			// TODO:  Implementierung von MySqlProvider.GetTableNames hinzufügen
			return null;
		}
	
		public override string Name
		{
			get
			{
				// TODO:  Getter-Implementierung für MySqlProvider.Name hinzufügen
				return null;
			}
		}
	
		public override string[] TypeNames
		{
			get
			{
				// TODO:  Getter-Implementierung für MySqlProvider.TypeNames hinzufügen
				return null;
			}
		}

		public override bool SupportsNativeGuidType
		{
			get
			{
				return true;
			}
		}

	}
}
#endif