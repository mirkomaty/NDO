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


using System;
using System.IO;
using NDO.Mapping;
using System.Data;
using System.Data.Common;
using NDO;
using NDOInterfaces;

namespace ExecuteSqlBatch
{
	/// <summary>
	/// Zusammenfassung f�r Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Der Haupteinstiegspunkt f�r die Anwendung.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("ExecuteSqlBatch: usage: ExecuteSqlBatch batchfile mapping-file");
				return -1;
			}
			Console.WriteLine("ExecuteSqlBatch: " + args[0] + " " + args[1]);
			if (!File.Exists(args[0]))
			{
				Console.WriteLine("ExecuteSqlBatch: can't find file " + args[0]);
				return -2;
			}
			NDOMapping mapping = new NDOMapping(args[1]);
			if (mapping.Connections.Count != 1)
			{
				Console.WriteLine(String.Format("ExecuteSqlBatch: Wrong count of connections: {0} in mapping file {1}", mapping.Connections.Count, args[1]));
				return -3;
			}
			Connection conn = (Connection)mapping.Connections[0];
			IProvider provider = NDOProviderFactory.Instance[conn.Type];
			if (provider == null)
			{
				Console.WriteLine("ExecuteSqlBatch: NDO provider not found: " + conn.Type);
					return -4;
			}
			Console.WriteLine(String.Format("Executing batch '{0}' on connection '{2}', db type = '{1}'", args[0], conn.Type, conn.Name));
			StreamReader sr = new StreamReader(args[0], System.Text.Encoding.Default);
			string s = sr.ReadToEnd();
			sr.Close();
			string[] arr = s.Split(';');
			IDbConnection cn = provider.NewConnection(conn.Name);
			cn.Open();
			IDbCommand cmd = provider.NewSqlCommand(cn);
			int result = 0;
			foreach(string statement in arr)
			{
				if (statement.Trim() != string.Empty)
				{
					try
					{
						cmd.CommandText = statement;
						cmd.ExecuteNonQuery();
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			cn.Close();
			Console.WriteLine("ExecuteSqlBatch ready");

			return result;
		}
	}
}
