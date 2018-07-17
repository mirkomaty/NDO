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
using System.Collections;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using NDOInterfaces;
using System.Windows.Forms;
using ADOX;

namespace NDO
{
	/// <summary>
	/// This is an Implementation of the IProvider interface for OleDb / Jet. 
	/// For more information see <see cref="IProvider"> IProvider interface </see>.
	/// </summary>
	public class NDOAccessProvider : NDOAbstractProvider 
	{
	
		#region Implementation of IProvider


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
		public override string GetQuotedName(string plainName)
		{
			if (plainName[0] == '[')
				return plainName;
			return "[" + plainName + "]";
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		private string GetDateExpression(DateTime dt)
		{
			return string.Format("#{0}/{1}/{2} {3}:{4}:{5}#", dt.Month, dt.Day, dt.Year, dt.Hour, dt.Minute, dt.Second);
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override System.Data.IDbConnection NewConnection(string connectionString) 
		{
			return new OleDbConnection(connectionString);
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object NewCommandBuilder(DbDataAdapter dataAdapter)
		{
			return new OleDbCommandBuilder((OleDbDataAdapter)dataAdapter);
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
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
			else if ( t == typeof(ulong))
				return 0;
			else if ( t == typeof(float))
				return 0;
			else if ( t == typeof(double))
				return 0;
			else if ( t == typeof(string))
				return 255;
			else if ( t == typeof(decimal))
				return 0;
			else if ( t == typeof(DateTime))
				return 0;
			else if ( t == typeof(Guid))
				return 0;
			else if ( t == typeof(Byte[]))
				return 0;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return 0;
			else
				return 0;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection) 
		{
			OleDbCommand command = new OleDbCommand();
			command.Connection = (OleDbConnection)connection;
			return command;
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete) 
		{
			OleDbDataAdapter da = new OleDbDataAdapter();
			da.SelectCommand = (OleDbCommand)select;
			da.UpdateCommand = (OleDbCommand)update;
			da.InsertCommand = (OleDbCommand)insert;
			da.DeleteCommand = (OleDbCommand)delete;
			return da;
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override IDataParameter AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName) 
		{
			return ((OleDbCommand)command).Parameters.Add(new OleDbParameter(parameterName, (OleDbType)dbType, size > -1 ? size : 0, columnName));
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override IDataParameter AddParameter(IDbCommand command, string parameterName,object dbType , int size, ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value) 
		{
			return ((OleDbCommand)command).Parameters.Add(new OleDbParameter(parameterName, (OleDbType)dbType, size > -1 ? size : 0, dir, isNullable, precision, scale, srcColumn, srcVersion, value));
		}


		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object GetDbType(System.Type t) 
		{
			t = base.ConvertNullableType
                (t);
			if ( t == typeof(bool) )
				return OleDbType.Boolean;
			else if ( t == typeof(byte) )
				return OleDbType.UnsignedTinyInt;
			else if ( t == typeof(sbyte) )
				return OleDbType.TinyInt;
			else if ( t == typeof(char) )
				return OleDbType.SmallInt;
			else if ( t == typeof(short))
				return OleDbType.SmallInt;
			else if ( t == typeof(ushort))
				return OleDbType.SmallInt;
			else if ( t == typeof(int))
				return OleDbType.Integer;
			else if ( t == typeof(uint))
				return OleDbType.Integer;
			else if ( t == typeof(long))
				return OleDbType.Decimal;
			else if ( t == typeof(ulong))
				return OleDbType.Decimal;
			else if ( t == typeof(float))
				return OleDbType.Single;
			else if ( t == typeof(double))
				return OleDbType.Double;
			else if ( t == typeof(string))
				return OleDbType.VarWChar;
			else if ( t == typeof(byte[]))
				return OleDbType.VarBinary;
			else if ( t == typeof(decimal))
				return OleDbType.Currency;
			else if ( t == typeof(DateTime))
				return OleDbType.Date;
			else if ( t == typeof(Guid))
				return OleDbType.Guid;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return OleDbType.Integer;
			else
				throw new NDOException(27, "Can't convert " + t.FullName + " into a DbType type.");
		}


		const string AccessString = "Provider=Microsoft.Jet.OLEDB.4.0;";
		public override DialogResult ShowConnectionDialog(ref string connectionString)
		{
			string tempstr;
			if (connectionString == null || connectionString == string.Empty)
				tempstr = AccessString;
			else
			{
				if (!(connectionString.IndexOf(AccessString) > -1))
					tempstr = AccessString;
				else
					tempstr = connectionString;
			}
			OleDbConnectionDialog dlg = new OleDbConnectionDialog(tempstr);
			DialogResult result = dlg.Show();
			if (result != DialogResult.Cancel)
				connectionString = dlg.ConnectionString;
			return result;
		}



		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override object GetDbType(string typeName) 
		{
			if (typeName == "BigInt") return OleDbType.BigInt;
			if (typeName == "Binary") return OleDbType.Binary;
			if (typeName == "Boolean") return OleDbType.Boolean;
			if (typeName == "BSTR") return OleDbType.BSTR;
			if (typeName == "Char") return OleDbType.Char;
			if (typeName == "Currency") return OleDbType.Currency;
			if (typeName == "Date") return OleDbType.Date;
			if (typeName == "DBDate") return OleDbType.DBDate;
			if (typeName == "DBTime") return OleDbType.DBTime;
			if (typeName == "DBTimeStamp") return OleDbType.DBTimeStamp;
			if (typeName == "Decimal") return OleDbType.Decimal;
			if (typeName == "Double") return OleDbType.Double;
			if (typeName == "Empty") return OleDbType.Empty;
			if (typeName == "Error") return OleDbType.Error;
			if (typeName == "Filetime") return OleDbType.Filetime;
			if (typeName == "Guid") return OleDbType.Guid;
			if (typeName == "IDispatch") return OleDbType.IDispatch;
			if (typeName == "Int") return OleDbType.Integer;
			if (typeName == "Integer") return OleDbType.Integer;
			if (typeName == "IUnknown") return OleDbType.IUnknown;
			if (typeName == "LongVarBinary") return OleDbType.LongVarBinary;
			if (typeName == "LongVarChar") return OleDbType.LongVarChar;
			if (typeName == "LongVarWChar") return OleDbType.LongVarWChar;
			if (typeName == "Numeric") return OleDbType.Numeric;
			if (typeName == "PropVariant") return OleDbType.PropVariant;
			if (typeName == "Single") return OleDbType.Single;
			if (typeName == "SmallInt") return OleDbType.SmallInt;
			if (typeName == "TinyInt") return OleDbType.TinyInt;
			if (typeName == "UnsignedBigInt") return OleDbType.UnsignedBigInt;
			if (typeName == "UnsignedInt") return OleDbType.UnsignedInt;
			if (typeName == "UnsignedSmallInt") return OleDbType.UnsignedSmallInt;
			if (typeName == "UnsignedTinyInt") return OleDbType.UnsignedTinyInt;
			if (typeName == "VarBinary") return OleDbType.VarBinary;
			if (typeName == "VarChar") return OleDbType.VarChar;
			if (typeName == "Variant") return OleDbType.Variant;
			if (typeName == "VarNumeric") return OleDbType.VarNumeric;
			if (typeName == "VarWChar") return OleDbType.VarWChar;
			if (typeName == "WChar") return OleDbType.WChar;
			if (typeName == "Memo") return OleDbType.VarWChar;
			throw new NDOException(28, "NDOOleDbProvider.GetDbType: Type " + typeName + " can't be converted into an OleDbType.");
		}

		public override string GetSqlLiteral(object o)
		{
			if (o is DateTime)
				return this.GetDateExpression((DateTime)o);
			if (o is Guid)
				return "{" + o.ToString() + "}";
			return base.GetSqlLiteral(o);
		}

	
		public override string[] GetTableNames(IDbConnection conn, string owner)
		{
			IList result = new ArrayList();
			bool wasOpen = true;
			if (conn.State == ConnectionState.Closed)
			{
				wasOpen = false;
				conn.Open();
			}
			DataTable dt = ((OleDbConnection) conn).GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {null, null, null, "TABLE"});
			foreach (System.Data.DataRow dr in dt.Rows)
			{
				result.Add(dr[2]);
			}
			if (!wasOpen)
				conn.Close();
			string[] strresult = new string[result.Count];
			for (int i = 0; i < result.Count; i++)
				strresult[i] = (string) result[i];
			return strresult;

		}



		public override DataSet GetDatabaseStructure( IDbConnection conn, string owner )
		{
			DataSet ds = base.GetDatabaseStructure(conn, owner);

			bool wasOpen = true;
			if (conn.State == ConnectionState.Closed)
			{
				wasOpen = false;
				conn.Open();
			}

			DataTable schemaTable = ((OleDbConnection)conn).GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, 
				new Object[] {null, null, null});

			foreach ( DataRow dr in schemaTable.Rows )
			{
				DataTable pk_dt = ds.Tables[(string)dr["PK_TABLE_NAME"]];
				DataTable fk_dt = ds.Tables[(string)dr["FK_TABLE_NAME"]];
				if (pk_dt == null || fk_dt == null)
					continue;
				DataColumn pk_dc = pk_dt.Columns[(string)dr["PK_COLUMN_NAME"]];
				DataColumn fk_dc = fk_dt.Columns[(string)dr["FK_COLUMN_NAME"]];
				if (fk_dc == null || pk_dc == null)
					continue;

				try
				{
					DataRelation rel = new DataRelation( (string) dr["FK_NAME"], pk_dc, fk_dc, true );
					ds.Relations.Add( rel );

					if ( ((string) dr["DELETE_RULE"]) == "CASCADE" )
						rel.ChildKeyConstraint.DeleteRule = Rule.Cascade;
					else
						rel.ChildKeyConstraint.DeleteRule = Rule.None;

				}
				catch ( Exception ex )
				{
					Exception newEx = (Exception) Activator.CreateInstance(ex.GetType(), "Analyzing Constraint " + pk_dt.TableName + "->" + fk_dt.TableName + ". " + ex.Message);
					throw newEx;
				}
			}
			
			if (!wasOpen)
				conn.Close();

			return ds;
		}
	
		public override string[] TypeNames
		{
			get
			{
				return Enum.GetNames(typeof(OleDbType));
			}
		}

        public override string GetLastInsertedId(string tableName, string columnName)
        {
            return "SELECT @@IDENTITY";
        }

		public override bool SupportsLastInsertedId
		{
			get
			{
				return true;
			}
		}

		Hashtable updateHandlers = new Hashtable();
		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override void RegisterRowUpdateHandler(IRowUpdateListener handler)
		{
			if (!updateHandlers.Contains(handler.DataAdapter))
			{
				OleDbDataAdapter da = handler.DataAdapter as OleDbDataAdapter;
				if (da == null)
					throw new NDOException(29, "Can't register OleDb update handler for data adapter of type " + handler.DataAdapter.GetType().FullName + ".");
				updateHandlers.Add(da, handler);
				da.RowUpdated += new OleDbRowUpdatedEventHandler(this.OnRowUpdated);
			}
		}

		private void OnRowUpdated(object sender, OleDbRowUpdatedEventArgs args)
		{
			IRowUpdateListener handler = (IRowUpdateListener) updateHandlers[sender];
			if (handler == null)
				throw new InternalException(314, "NDOOleDbProvider");
			handler.OnRowUpdate(args.Row);
		}

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override string Name { get { return "Access"; }  }

		/// <summary>
		/// See <see cref="IProvider"> IProvider interface </see>
		/// </summary>
		public override bool SupportsNativeGuidType { get { return true; } }

		public override string CreateDatabase(object necessaryData)
		{
			string connStr = necessaryData as string;
			if (connStr == null)
				throw new ArgumentException("CreateDatabase: wrong parameter type or null parameter", "necessaryData");

			try
			{
				Type t = Type.GetTypeFromCLSID(new Guid("{00000602-0000-0010-8000-00AA006D2EA4}"));
				object oc = Activator.CreateInstance(t);
				ADOX._Catalog cat = (ADOX._Catalog) oc;
				cat.Create(connStr + ";Jet OLEDB:Engine Type=5");
				return connStr;
			}
			catch (Exception ex)
			{
				throw new NDOException(19, "Error while attempting to create a database: Exception Type: " + ex.GetType().Name + " Message: " + ex.Message);
			}

		}

		public override bool SupportsFetchLimit
		{
			get { return false; }
		}

		public override DialogResult ShowCreateDbDialog(ref object necessaryData)
		{
			AccessCreateDbDlg dlg = new AccessCreateDbDlg();
			if (dlg.ShowDialog() == DialogResult.Cancel)
				return DialogResult.Cancel;
			necessaryData = dlg.ConnectionString;
			return DialogResult.OK;
		}

		#endregion

	}
}
