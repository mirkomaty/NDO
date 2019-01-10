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
using System.Text;
using System.Collections;
using NDOInterfaces;
using NDO;
using System.Windows.Forms;

namespace NDOInterfaces
{
	/// <summary>
	/// Contains standard implementation for providers.
	/// </summary>
	public abstract class NDOAbstractProvider : IProvider 
	{
		
	
		#region Implementation of IProvider


		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract System.Data.IDbConnection NewConnection(string parameters);

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection);

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract System.Data.Common.DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete);

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract object NewCommandBuilder(System.Data.Common.DbDataAdapter dataAdapter);

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName);

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, System.Data.ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, System.Data.DataRowVersion srcVersion, object value);
		
		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract object GetDbType(System.Type t);

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract object GetDbType(string dbTypeName);

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual int GetDefaultLength(System.Type t) 
		{
			if ( t == typeof(bool) )
				return 4;
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
			else if ( t == typeof(ulong))
				return 8;
			else if ( t == typeof(float))
				return 8;
			else if ( t == typeof(double))
				return 8;
			else if ( t == typeof(string))
				return 255;
			else if ( t == typeof(decimal))
				return 11;
			else if ( t == typeof(DateTime))
				return 4;
			else if ( t == typeof(Guid))
				return 16;
			else if ( t == typeof(Byte[]))
				return 1024;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return 4;
			else
				return 0;
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string Wildcard
		{
			get { return "%"; }
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool UseNamedParams
		{
			get { return false; }
		}


		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string NamedParamPrefix
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool UseStoredProcedure
		{
			get { return false; }
		}

	
		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GetNamedParameter(string plainParameterName)
		{
			return plainParameterName;
		}
	
		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GetQuotedName(string plainName)
		{
			return plainName;
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GetSqlLiteral(object o)
		{
			if (o == null)
				return "NULL";
			if (o is string || o.GetType().IsSubclassOf(typeof(string)) || o is Guid)
				return "'" + o.ToString() + "'";
			if (o is byte[])
			{
				StringBuilder sb = new StringBuilder(((byte[])o).Length * 2 + 2);
				sb.Append('\'');
				foreach (byte b in (byte[])o)
				{
					sb.Append(b.ToString("X2"));
				}
				sb.Append('\'');
				return sb.ToString();
			}
			return o.ToString();
		}


		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool SupportsLastInsertedId 
		{
			get {return false;}
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
#if NDO20
        public virtual string GetLastInsertedId(string tableName, string columnName)
		{
			return null;
		}
#else
        public virtual string GetLastInsertedId
        {
            get { return null; }
        }
#endif

        /// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool SupportsBulkCommands 
		{
			get { return false; }
		}
		

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GenerateBulkCommand(string[] commands)
		{
			return null;
		}


		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual int MaxBulkCommandLength 
		{
			get { return 0; }
		}


		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract string[] TypeNames { get; }

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public string[] GetTableNames (IDbConnection conn)
		{
			return GetTableNames(conn, null);
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract string[] GetTableNames(IDbConnection conn, string owner);


		/// <summary>
		/// Generates a DataSet with exactly the same structure elements as the database. 
		/// It maps all tables, columns, primary keys and foreign key constraints of the database
		/// to the DataSet.
		/// </summary>
		/// <remarks>
		/// This implementation fetches only the tables and columns.
		/// </remarks>
		public virtual DataSet GetDatabaseStructure( IDbConnection conn, string ownerName )
		{
			bool wasOpen = false;
			if (conn.State == ConnectionState.Open)
				wasOpen = true;
			else
				conn.Open();

			DataSet ds = new DataSet();
			foreach(string tableName in this.GetTableNames(conn, ownerName))
			{
				string sql;

				if ( ownerName != null && ownerName.Trim() != "" )
				{
					sql = "SELECT * FROM " + this.GetQuotedName( ownerName ) + "." + this.GetQuotedName( tableName );
				}
				else
				{
					sql = "SELECT * FROM " + this.GetQuotedName( tableName );
				}

				IDbCommand cmd = this.NewSqlCommand( conn );
				cmd.CommandText = sql;
				IDataAdapter da = this.NewDataAdapter( cmd, null, null, null );

				da.FillSchema( ds, SchemaType.Source );
				ds.Tables[ds.Tables.Count - 1].TableName = tableName;
			}

			if (!wasOpen)
				conn.Close();
			return ds;
		}


		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool SupportsInsertBatch
		{
			get { return false; }
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual void RegisterRowUpdateHandler(IRowUpdateListener handler)
		{
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public abstract bool SupportsNativeGuidType { get; }

		/// <summary>
		/// Fetch limits are used for paging. If the database supports paging, it provides an addition to OrderBy which
		/// allows to specify a count of rows to skip and a maximum count of rows to take.
		/// </summary>
		public virtual bool SupportsFetchLimit
		{
			get { return true; }
		}

		/// <summary>
		/// Fetch limits are used for paging. If the database supports paging, it provides an addition to OrderBy which
		/// allows to specify a count of rows to skip and a maximum count of rows to take.
		/// </summary>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns>A string, which is concatenated to the OrderBy clause</returns>
		public virtual string FetchLimit( int skip, int take )
		{
			if (skip == 0 && take == 0)
				return string.Empty;
			// Standard Sql implementation
			string fetch = String.Empty;
			if (take != 0)
				fetch = String.Format( " FETCH NEXT {0} ROWS ONLY", take );
			return String.Format("OFFSET {0} ROWS{1}", skip, fetch);
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual DialogResult ShowConnectionDialog(ref string connectionString)
		{
			GenericConnectionDialog dlg = new GenericConnectionDialog(connectionString);			
			DialogResult result = dlg.ShowDialog();			
			if (result != DialogResult.Cancel)
				connectionString = dlg.ConnectionString;
			return result;
		}


		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual DialogResult ShowCreateDbDialog(ref object necessaryData)
		{
			DefaultCreateDbDialog dlg = new DefaultCreateDbDialog(this, necessaryData as NDOCreateDbParameter);
			DialogResult result = dlg.ShowDialog();
			if (result != DialogResult.Cancel)
				necessaryData = dlg.NecessaryData;
			return result;
		}

		/// <summary>
		/// See <see cref="IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string CreateDatabase(object necessaryData)
		{
			NDOCreateDbParameter par = necessaryData as NDOCreateDbParameter;
			if (par == null)
				throw new ArgumentException("NDOAbstractProvider: parameter type " + necessaryData.GetType().FullName + " is wrong.", "necessaryData");
			string dbName = this.GetQuotedName(par.DatabaseName);
			try
			{
				IDbConnection conn = this.NewConnection(par.Connection);
				IDbCommand cmd = this.NewSqlCommand(conn);
				cmd.CommandText = "CREATE DATABASE " + dbName;
				bool wasOpen = true;
				if (conn.State == ConnectionState.Closed)
				{
					conn.Open();
					wasOpen = false;
				}
				cmd.ExecuteNonQuery();
				if (!wasOpen)
					conn.Close();
			}
			catch (Exception ex)
			{
				throw new NDOException(19, "Error while attempting to create a database: Exception Type: " + ex.GetType().Name + " Message: " + ex.Message);
			}
			return string.Empty;
		}


		#endregion

		/// <summary>
		/// Called by the providers to get the generic argument type of a Nullable.
		/// </summary>
		/// <param name="t">The type to convert.</param>
		/// <returns>If t is a Nullable, the generic argument type is returned. Else t is returned.</returns>
		public Type ConvertNullableType(Type t)
		{
            if (t.IsGenericParameter)
                return typeof(string);
			if (t.FullName.StartsWith("System.Nullable`1"))
				t = t.GetGenericArguments()[0];
            if (t.IsEnum)
			    return Enum.GetUnderlyingType(t);
			return t;
        }

	}
}
