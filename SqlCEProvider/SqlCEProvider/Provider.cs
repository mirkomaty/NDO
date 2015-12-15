//
// Copyright (C) 2002-2010 Mirko Matytschak 
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
//  Kopieren Sie die erstellte Dll in das NDO-Anwendungs-Verzeichnis. NDO findet dann
//  automatisch den Provider und reiht ihn unter dem Namen in die ProviderFactory ein, der 
//  vom Property "Name" zurückgegeben wird.
//	In diesem Beispiel wird Ihr Adapter unter dem Namen "SqlCe" in die ProviderFactory eingetragen.
//	Den String "SqlCe" (oder welchen String auch immer Sie zur Kennzeichnung benötigen)
//	müssen Sie in der NDOMapping.xml als Attribut "Type" des Connection-Eintrags 
//	angeben.

using System;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using NDOInterfaces;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Data.SqlServerCe;
using System.IO;


namespace NDO.SqlCeProvider
{
	/// <summary>
	/// Sample adapter class to connect new ADO.NET Providers with NDO.
	/// </summary>
	public class Provider : NDOAbstractProvider
	{
		public Provider()
		{
		}

        //Assembly OnAssemblyResolve( object sender, ResolveEventArgs args )
        //{
        //    if (args.Name.StartsWith("System.Data.SqlServerCe"))
        //    {
        //        string path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
        //        if ( path.IndexOf( "Temporary ASP.NET Files" ) > -1 )
        //            path = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "bin" );
        //        if ( IntPtr.Size == 8 )
        //            path = Path.Combine( path, "amd64" );
        //        else
        //            path = Path.Combine( path, "x86" );
        //        path = Path.Combine( path, "System.Data.SqlServerCe.dll" );				
        //        return Assembly.LoadFrom( path );
        //    }
        //    return null;
        //}


		// The following methods provide objects of provider classes 
		// which implement common interfaces in .NET:
		// IDbConnection, IDbCommand, DbDataAdapter and the Parameter objects
		#region Provide specialized type objects
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new SqlCeConnection(connectionString);
		}

		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			SqlCeCommand command = new SqlCeCommand();
			command.Connection = (SqlCeConnection)connection;
			return command;
		}

		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
		{
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			da.SelectCommand = (SqlCeCommand)select;
			da.UpdateCommand = (SqlCeCommand)update;
			da.InsertCommand = (SqlCeCommand)insert;
			da.DeleteCommand = (SqlCeCommand)delete;
			return da;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new SqlCeCommandBuilder((SqlCeDataAdapter)dataAdapter);
		}


		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object odbType, int size, string columnName) 
		{
            SqlCeParameter result = new SqlCeParameter(parameterName, (SqlDbType)odbType, size, columnName);
			((SqlCeCommand)command).Parameters.Add(result);
			return result;
		}

		public override IDataParameter AddParameter(IDbCommand command, string parameterName, object odbType, int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			SqlCeParameter result = new SqlCeParameter(parameterName, (SqlDbType)odbType, size, dir, isNullable, precision, scale, srcColumn, srcVersion, value);
			((SqlCeCommand)command).Parameters.Add(result);
			return result;
		}


        internal static SqlDbType GetInternalDbType(Type t)
        {
            if (t == typeof(bool))
                return SqlDbType.Bit;
            else if (t == typeof(byte))
                return SqlDbType.TinyInt;
            else if (t == typeof(sbyte))
                return SqlDbType.TinyInt;
            else if (t == typeof(char))
                return SqlDbType.Char;
            else if (t == typeof(short))
                return SqlDbType.SmallInt;
            else if (t == typeof(ushort))
                return SqlDbType.SmallInt;
            else if (t == typeof(int))
                return SqlDbType.Int;
            else if (t == typeof(uint))
                return SqlDbType.Int;
            else if (t == typeof(long))
                return SqlDbType.BigInt;
            else if (t == typeof(System.Guid))
                return SqlDbType.UniqueIdentifier;
            else if (t == typeof(ulong))
                return SqlDbType.BigInt;
            else if (t == typeof(float))
                return SqlDbType.Real;
            else if (t == typeof(double))
                return SqlDbType.Float;
            else if (t == typeof(string))
                return SqlDbType.NVarChar;
            else if (t == typeof(byte[]))
                return SqlDbType.Image;
            else if (t == typeof(decimal))
                return SqlDbType.Decimal;
            else if (t == typeof(System.DateTime))
                return SqlDbType.DateTime;
            else if (t.IsSubclassOf(typeof(System.Enum)))
                return SqlDbType.Int;
            else if (t == typeof(System.Data.SqlTypes.SqlXml))
                return SqlDbType.Xml;
            else
                throw new NDOException(41, "NDOSqlProvider.GetDbType: Type " + t.Name + " can't be converted into a SqlDbType.");
        }



		#endregion

		// The following method convert System.Type objects to SqlDbType-Members
		// For your own adapter use members of the database type emumeration 
		// of your ADO.NET provider
		#region Provide SqlDbType members
		public override object GetDbType(Type t) 
		{
            t = base.ConvertNullableType(t);
			return GetInternalDbType( t );
		}


		// The following method converts string representations of SqlDbType-Members
		// into SqlDbType.
		// For your own adapter use members of the database type emumeration of your 
		// ADO.NET provider and convert it to the respective enumeration type		
		public override object GetDbType(string typeName) 
		{
			SqlDbType result;

			if (!Enum.TryParse<SqlDbType>(typeName, true, out result))
			throw new NDOException(27, this.GetType().FullName + ".GetDbType: Typname " + typeName + " cannot be converted into a DbType.");
			return result;
		}

        //private string GetDateExpression(System.DateTime dt)
        //{
        //    //'9999-12-31 23:59:59'
        //    return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "." + dt.Millisecond + "'";
        //}

        public override int GetDefaultLength(Type t)
        {
            if (t == typeof(byte[]))
                return 65535;
            return base.GetDefaultLength(t);
        }


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
				
		public override string GetNamedParameter(string plainParameterName)
		{
			return "@" + plainParameterName;
		}
	
		public override string GetQuotedName(string plainName)
		{
			return "[" + plainName + ']';
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

        public override bool SupportsInsertBatch
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Gets an expression in the SQL dialect of the database, which retrieves the ID of the last
		/// inserted row, if the ID is automatically generated by the database.
		/// </summary>
		public override string GetLastInsertedId(string tableName, string columnName)
		{
            return "SELECT @@IDENTITY";
		}

        Dictionary<DbDataAdapter, IRowUpdateListener> updateHandlers = new Dictionary<DbDataAdapter, IRowUpdateListener>();

        /// <summary>
        /// See <see cref="IProvider"> IProvider interface </see>
        /// </summary>
        public override void RegisterRowUpdateHandler(IRowUpdateListener handler)
        {
            if (!updateHandlers.ContainsKey(handler.DataAdapter))
            {
                SqlCeDataAdapter da = handler.DataAdapter as SqlCeDataAdapter;
                if (da == null)
                    throw new NDOException(29, "Can't register SqlCe update handler for data adapter of type " + handler.DataAdapter.GetType().FullName + ".");
				if (handler == null)
					throw new ArgumentNullException( "handler", "RegisterRowUpdateHandler" );
				updateHandlers.Add( da, handler );
                da.RowUpdated += new SqlCeRowUpdatedEventHandler(this.OnRowUpdated);
            }
        }

        private void OnRowUpdated(object sender, SqlCeRowUpdatedEventArgs args)
        {
            DbDataAdapter da = (DbDataAdapter)sender;
            if (!updateHandlers.ContainsKey(da))
                throw new NDOException(314, "OnRowUpdated event can't be scheduled to a DataAdapter.");
            IRowUpdateListener handler = updateHandlers[da];
            handler.OnRowUpdate(args.Row);
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
            IList result = new ArrayList();
            string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES";
            SqlCeCommand cmd = new SqlCeCommand(sql, (SqlCeConnection)conn);
            bool wasOpen = true;
            if (conn.State == ConnectionState.Closed)
            {
                wasOpen = false;
                conn.Open();
            }
            SqlCeDataReader dr = cmd.ExecuteReader();
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
                strresult[i] = (string)result[i];
            return strresult;
        }
	
		public override string[] TypeNames
		{
			get
			{				
				return Enum.GetNames(typeof(SqlDbType));
			}
		}

		public override string Name { get { return "SqlCe"; }  }

		public override bool SupportsNativeGuidType 
		{ 
			get { return true; } 
		}

		public override DialogResult ShowConnectionDialog(ref string connectionString)
		{
            ConnectionDialog cd = new ConnectionDialog();
            DialogResult result = cd.ShowDialog();
            if (result == DialogResult.Cancel)
                return result;

            connectionString = cd.Connection;
            return DialogResult.OK;
		}

		public override DialogResult ShowCreateDbDialog( ref object necessaryData )
		{
			string connectionString = string.Empty;
			DialogResult result = ShowConnectionDialog( ref connectionString );
			if ( result == DialogResult.OK )
			{
				necessaryData = connectionString;
			}

			return result;
		}

		public override string CreateDatabase(object necessaryData)
		{
			string s = necessaryData as string;
			if ( s == null )
				throw new ArgumentException( "NDO.SqlCEProvider.Provider.CreateDatabase: wrong parameter type for 'necessaryData'. Expected: string." );

            SqlCeEngine en = new SqlCeEngine(s);
            en.CreateDatabase();

			return s;
		}
	}
}
