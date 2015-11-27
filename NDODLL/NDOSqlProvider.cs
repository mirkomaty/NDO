//
// Copyright (C) 2002-2014 Mirko Matytschak 
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
// there is a commercial licence available at www.netdataobjects.de.
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


using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NDOInterfaces;
using System.Windows.Forms;


namespace NDO
{
	/// <summary>
	/// This is an Implementation of the IProvider interface for SqlServer. 
	/// For more information see <see cref="IProvider"> IProvider interface </see>.
	/// </summary>
	public class NDOSqlProvider : NDOAbstractProvider 
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
		public override string GetNamedParameter(string plainParameterName)
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
		private string GetDateExpression(DateTime dt)
		{
			string dtstr  = dt.ToString();
			dtstr = dtstr.Replace(".", "/");
			dtstr = dtstr.Replace("-", "/");
			return "'" + dtstr + "'";
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
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new SqlConnection(connectionString);
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			SqlCommand command = new SqlCommand();
			command.Connection = (SqlConnection)connection;
			return command;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
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
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new SqlCommandBuilder((SqlDataAdapter)dataAdapter);
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
			return ((SqlCommand)command).Parameters.Add(new SqlParameter(parameterName, (SqlDbType)dbType, size > -1 ? size : 0, columnName));			
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object dbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			return ((SqlCommand)command).Parameters.Add(new SqlParameter(parameterName, (SqlDbType)dbType, size > -1 ? size : 0, dir, isNullable, precision, scale, srcColumn, srcVersion, value));
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object GetDbType(Type t) 
		{
			t = base.ConvertNullableType(t);
			if (t == typeof(bool))
				return SqlDbType.Bit;
			else if ( t == typeof(byte) )
				return SqlDbType.TinyInt;
			else if ( t == typeof(sbyte) )
				return SqlDbType.TinyInt;
			else if ( t == typeof(char) )
				return SqlDbType.Char;
			else if ( t == typeof(short))
				return SqlDbType.SmallInt;
			else if ( t == typeof(ushort))
				return SqlDbType.SmallInt;
			else if ( t == typeof(int))
				return SqlDbType.Int;
			else if ( t == typeof(uint))
				return SqlDbType.Int;
			else if ( t == typeof(long))
				return SqlDbType.BigInt;
			else if ( t == typeof(System.Guid))
				return SqlDbType.UniqueIdentifier;
			else if ( t == typeof(ulong))
				return SqlDbType.BigInt;
			else if ( t == typeof(float))
				return SqlDbType.Real;
			else if ( t == typeof(double))
				return SqlDbType.Float;
			else if ( t == typeof(string))
				return SqlDbType.NVarChar;
			else if ( t == typeof(byte[]))
				return SqlDbType.Image;
			else if ( t == typeof(decimal))
				return SqlDbType.Decimal;
			else if ( t == typeof(System.DateTime))
				return SqlDbType.DateTime;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return SqlDbType.Int;
			else if ( t == typeof(System.Data.SqlTypes.SqlXml))
				return SqlDbType.Xml;
			else
				throw new NDOException(41, "NDOSqlProvider.GetDbType: Type " + t.Name + " can't be converted into a SqlDbType.");
		}

		public override int GetDefaultLength(Type t)
		{
			if (t == typeof(byte[]))
				return 65535;
			return base.GetDefaultLength (t);
		}

		const string SqlString = "Provider=SQLOLEDB.1;";
		public override System.Windows.Forms.DialogResult ShowConnectionDialog(ref string connectionString)
		{
			string tempstr;
			if (connectionString == null || connectionString == string.Empty)
				tempstr = SqlString;
			else
			{
				if (!(connectionString.IndexOf(SqlString) > -1))
					tempstr = SqlString + connectionString;
				else
					tempstr = connectionString;
			}
			OleDbConnectionDialog dlg = new OleDbConnectionDialog(tempstr);
			DialogResult result = dlg.Show();
			if (result != DialogResult.Cancel)
				connectionString = dlg.ConnectionString.Replace(SqlString, string.Empty);
			return result;
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object GetDbType(string typeName) 
		{
			string typeNameLower = typeName.ToLower();
            if (typeNameLower.StartsWith("nvarchar")) return SqlDbType.NVarChar;
            if (String.Compare(typeName, "VarChar", true) == 0) return SqlDbType.VarChar;
            if (String.Compare(typeName, "NChar", true) == 0) return SqlDbType.NChar;
            if (String.Compare(typeName, "Int", true) == 0) return SqlDbType.Int;
            if (String.Compare(typeName, "Char", true) == 0) return SqlDbType.Char;
            if (String.Compare(typeName, "UniqueIdentifier", true) == 0) return SqlDbType.UniqueIdentifier;
            if (String.Compare(typeName, "DateTime", true) == 0) return SqlDbType.DateTime;
			if (String.Compare(typeName, "Decimal", true) == 0) return SqlDbType.Decimal;
            if (String.Compare(typeName, "Binary", true) == 0) return SqlDbType.Binary;
            if (String.Compare(typeName, "Bit", true) == 0) return SqlDbType.Bit;
            if (String.Compare(typeName, "Real", true) == 0) return SqlDbType.Real;
            if (String.Compare(typeName, "Float", true) == 0) return SqlDbType.Float;
			if (String.Compare(typeName, "Image", true) == 0) return SqlDbType.Image;
			if (String.Compare(typeName, "Money", true) == 0) return SqlDbType.Money;
			if (String.Compare(typeName, "NText", true) == 0) return SqlDbType.NText;
			if (String.Compare(typeName, "SmallDateTime", true) == 0) return SqlDbType.SmallDateTime;
			if (String.Compare(typeName, "SmallInt", true) == 0) return SqlDbType.SmallInt;
			if (String.Compare(typeName, "SmallMoney", true) == 0) return SqlDbType.SmallMoney;
			if (String.Compare(typeName, "Text", true) == 0) return SqlDbType.Text;
			if (String.Compare(typeName, "Timestamp", true) == 0) return SqlDbType.Timestamp;
			if (String.Compare(typeName, "TinyInt", true) == 0) return SqlDbType.TinyInt;
			if (String.Compare(typeName, "VarBinary", true) == 0) return SqlDbType.VarBinary;
			if (String.Compare(typeName, "Variant", true) == 0) return SqlDbType.Variant;
            if (String.Compare(typeName, "BigInt", true) == 0) return SqlDbType.BigInt;
            if (typeNameLower.StartsWith("xml")) return SqlDbType.Xml;
            throw new NDOException(42, "NDOSqlProvider.GetDbType: Type name " + typeName + " can't be converted into a SqlDbType.");
		}
	
		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string GetQuotedName(string plainName)
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


#if NDO20
        /// <summary>
        /// Gets an expression in the SQL dialect of the database, which retrieves the ID of the last
        /// inserted row, if the ID is automatically generated by the database.
        /// </summary>
        /// <param name="tableName">The table name, the insert happens into.</param>
        /// <param name="columnName">The column name of the autoincremented primary key column.</param>
        /// <returns></returns>        
        public override string GetLastInsertedId(string tableName, string columnName)
        {
            return "@@IDENTITY";
        }
#else
        /// <summary>
        /// Gets an expression in the SQL dialect of the database, which retrieves the ID of the last
        /// inserted row, if the ID is automatically generated by the database.
        /// </summary>
		public override string GetLastInsertedId
		{
			get
			{
				return "@@IDENTITY";
			}
		}
#endif



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
		public override string[] GetTableNames(IDbConnection conn, string owner)
		{
			IList result  = new ArrayList();
			string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
			SqlCommand cmd = new SqlCommand(sql, (SqlConnection) conn);
			bool wasOpen = true;
			if (conn.State == ConnectionState.Closed)
			{
				wasOpen = false;
				conn.Open();
			}
			SqlDataReader dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				string tbl = dr.GetString(2);
				if (tbl != "dtproperties")
					result.Add(tbl);
			}
			dr.Close();
			if (!wasOpen)
				conn.Close();
			string[] strresult = new string[result.Count];
			for (int i = 0; i < result.Count; i++)
				strresult[i] = (string) result[i];
			return strresult;
		}
	
		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string[] TypeNames
		{
			get
			{
				return Enum.GetNames(typeof(SqlDbType));
			}
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string Name { get { return "SqlServer"; }  }

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

		public override string CreateDatabase(object necessaryData)
		{
			base.CreateDatabase(necessaryData);
			// Don't need to check, if type is OK, since that happens in base.CreateDatabase
			NDOCreateDbParameter par = necessaryData as NDOCreateDbParameter;

			string dbName = par.DatabaseName;

			Regex regex = new Regex(@"Initial\sCatalog=([^\;]*)");
			Match match = regex.Match(par.Connection);
			if (match.Success)
			{
				return par.Connection.Substring(0, match.Groups[1].Index) + dbName + par.Connection.Substring(match.Index + match.Length);
			}
			if (par.Connection.EndsWith(";"))
				return par.Connection + "Initial Catalog=" + dbName;
			else
				return par.Connection + ";Initial Catalog=" + dbName;
		}


		#endregion

	}
}
