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
using System.Reflection;
using System.IO;
using System.Data;
using System.Data.Common;
using NDO.Logging;
using NDOInterfaces;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für SqlDumper.
	/// </summary>
	internal class SqlDumper
	{
		ILogAdapter logAdapter;
		IDbCommand insertCommand;
		IDbCommand selectCommand;
		IDbCommand updateCommand;
		IDbCommand deleteCommand;
		IProvider provider;

		public SqlDumper(ILogAdapter logAdapter, IProvider provider, IDbCommand insertCommand, IDbCommand selectCommand, IDbCommand updateCommand, IDbCommand deleteCommand)
		{
			this.logAdapter = logAdapter;
			this.provider = provider;
			this.updateCommand = updateCommand;
			this.insertCommand = insertCommand;
			this.selectCommand = selectCommand;
			this.deleteCommand = deleteCommand;
		}

		internal void Dump(DataRow[] rows)
		{
			if (logAdapter == null)
				return;

			bool hasSelect = false;
			bool hasInsert = false;
			bool hasDelete = false;
			bool hasUpdate = false;

            if (rows == null || rows.Length == 0)
            {
                hasSelect = true;
            }
            else
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    DataRow row = rows[i];
                    if (row.RowState == System.Data.DataRowState.Added)
                        hasInsert = true;
                    if (row.RowState == System.Data.DataRowState.Modified)
                        hasUpdate = true;
                    if (row.RowState == System.Data.DataRowState.Deleted)
                        hasDelete = true;
                }
            }

			StringWriter sw = null;

			try
			{
				sw = new StringWriter();
				if (hasSelect)
				{
					sw.WriteLine("Select Command:");
					sw.WriteLine(this.selectCommand.CommandText);
					DumpParameters(selectCommand, sw);
				}

				if (hasInsert)
				{
					sw.WriteLine("Insert Command:");
					sw.WriteLine(this.insertCommand.CommandText);
					DumpParameters(insertCommand, sw);			
				}
				if (hasDelete)
				{
					sw.WriteLine("Delete Command:");
					sw.WriteLine(this.deleteCommand.CommandText);
					DumpParameters(deleteCommand, sw);
					sw.WriteLine(rows.Length.ToString() + " Zeilen zu löschen");
				}
				if (hasUpdate)
				{
					if (updateCommand != null)
					{
						sw.WriteLine("Update Command:");
						sw.WriteLine(this.updateCommand.CommandText);
						DumpParameters(updateCommand, sw);
					}
					else
						sw.WriteLine("No Update Command");
				}
			
				if (rows != null) 
				{
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow r = rows[i];
                        sw.WriteLine("----");
                        sw.WriteLine("Row: " + r.RowState.ToString());
						DataRowVersion drv = r.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current;
						foreach (DataColumn c in r.Table.Columns)
						{
							sw.WriteLine(c.ColumnName + " = " + r[c, drv].ToString());
						}
					}
				
				}
			}
			finally
			{
				if (sw != null)
				{					
					sw.Close();
					this.logAdapter.Info(sw.ToString());
				}
			}
		}

		private void DumpParameters(IDbCommand cmd, TextWriter sw)
		{
			int j = 1;
			foreach(System.Data.IDbDataParameter dp in cmd.Parameters)
			{
				string parType;
				if (provider is NDOSqlProvider)
					parType = (((System.Data.SqlClient.SqlParameter)dp).SqlDbType).ToString();
				else if (provider is NDOAccessProvider)
					parType = (((System.Data.OleDb.OleDbParameter)dp).OleDbType).ToString();
				else if (dp.GetType().Name == "MySqlParameter")
				{
					Type t = dp.GetType();
					FieldInfo fi = t.GetField("dbType", BindingFlags.NonPublic | BindingFlags.Instance);
					parType = fi.GetValue(dp).ToString();	
				}
				else 
					parType = dp.DbType.ToString();
				sw.Write("Parameter " + j.ToString() + ": " + dp.ToString() + '(' + parType + ") Source Column: " + dp.SourceColumn + " ");
				sw.Write("Prec: " + dp.Precision + " Scale: " + dp.Scale + " Size: " + dp.Size);
				if (dp.Value != null)
					sw.WriteLine(" Value: " + dp.Value);
				else
					sw.WriteLine();

				j++;
			}
		}

	}
}
