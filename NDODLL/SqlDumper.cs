//
// Copyright (c) 2002-2023 Mirko Matytschak 
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


using System.IO;
using System.Data;
using NDOInterfaces;
using Microsoft.Extensions.Logging;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für SqlDumper.
	/// </summary>
	internal class SqlDumper
	{
		ILogger logger;
		IDbCommand insertCommand;
		IDbCommand selectCommand;
		IDbCommand updateCommand;
		IDbCommand deleteCommand;
		IProvider provider;

		public SqlDumper(ILoggerFactory loggerFactopry, IProvider provider, IDbCommand insertCommand, IDbCommand selectCommand, IDbCommand updateCommand, IDbCommand deleteCommand)
		{
			this.logger = loggerFactopry.CreateLogger<SqlDumper>();
			this.provider = provider;
			this.updateCommand = updateCommand;
			this.insertCommand = insertCommand;
			this.selectCommand = selectCommand;
			this.deleteCommand = deleteCommand;
		}

		internal void Dump(DataRow[] rows)
		{
			if (logger == null)
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
					if (this.logger != null && this.logger.IsEnabled( LogLevel.Debug ))
						this.logger.LogDebug( sw.ToString() );
				}
			}
		}

		private void DumpParameters(IDbCommand cmd, TextWriter sw)
		{
			int j = 1;
			foreach(IDbDataParameter dp in cmd.Parameters)
			{
				string parType = provider.GetDbTypeString(dp);
				sw.Write($"Parameter {j}: {dp.ToString()}({parType}) Source Column: {dp.SourceColumn} ");
				sw.Write($"Prec: {dp.Precision} Scale: {dp.Scale} Size: {dp.Size}");
				if (dp.Value != null)
					sw.WriteLine(" Value: " + dp.Value);
				else
					sw.WriteLine();

				j++;
			}
		}
	}
}
