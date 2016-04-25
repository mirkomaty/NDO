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
using System.Linq;
using System.IO;
using NDO.Mapping;
using System.Data;
using System.Data.Common;
using NDO;
using NDOInterfaces;

namespace ExecuteSqlBatch
{
	/// <summary>
	/// Zusammenfassung fï¿½r Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Der Haupteinstiegspunkt fï¿½r die Anwendung.
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
			if (mapping.Connections.Count() != 1)
			{
				Console.WriteLine(String.Format("ExecuteSqlBatch: Wrong count of connections: {0} in mapping file {1}", mapping.Connections.Count(), args[1]));
				return -3;
			}
			Connection conn = (Connection)mapping.Connections.First();
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
