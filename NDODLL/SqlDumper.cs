//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.IO;
using System.Data;
using NDO.Logging;
using NDOInterfaces;
using System.Collections.Generic;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für SqlDumper.
	/// </summary>
	internal class SqlDumper
	{
		private readonly ILogAdapter logAdapter;
		private readonly IProvider provider;

		public SqlDumper(ILogAdapter logAdapter, IProvider provider)
		{
			this.logAdapter = logAdapter;
			this.provider = provider;
		}

		internal void Dump(DataRow[] rows, IDbCommand cmd, IEnumerable<string> batch)
		{
			if (logAdapter == null)
				return;

			StringWriter sw = null;

			try
			{
				sw = new StringWriter();
				if (batch != null)
				{
					sw.WriteLine( String.Join( ";\r\n", batch ) );
				}
				if (cmd != null)
				{
					DumpParameters(cmd, sw);
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
