﻿//
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

namespace ManipulateConnection
{
	class Class1
	{
		[STAThread]
		static int Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("ManipulateConnection: usage: ManipulateConnection mapping-file conn-type [-t]");
				return -1;
			}

			Console.WriteLine("ManipulateConnection: args: " + args[0] + " " + args[1]);
			if (!File.Exists(args[0]))
			{
                if (args.Length == 2)  // return quiet if -t is not specified
                    return 0;
				Console.WriteLine("ManipulateConnection: can't find file " + args[0]);
				return -2;
			}
            


			string fileName;
			string connName = null;
			string type = null;
			switch(args[1])
			{
				case "Oracle":
					connName = @"Password=NDOTester;User ID=NDOTester;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.178.29)(PORT=1521)) (CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XE)));";
					type = "Oracle";
					break;
				case "SqlServer":
					connName = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=NDOTest;Data Source=localhost";
					type = "SqlServer";
					break;
				case "Access":
					connName = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Dokumente und Einstellungen\Mirko Matytschak\Eigene Dateien\db1.mdb;Persist Security Info=False";
					type = "Access";
					break;
				case "MySql":
					connName = @"Database=NDOTest;Data Source=localhost;User Id=root;";
					type = "MySql";
					break;
				case "Firebird":
					fileName = @"C:\Projekte\NDO\Firebird\TEST.FBD";
					if (!File.Exists(fileName))
					{
						Console.WriteLine("Error: File doesn't exist: " + fileName);
						return -1;
					}
					connName = @"Dialect=3;User Id=SYSDBA;Database=localhost:" + fileName + @";Data Source=localhost;Password=masterkey";
					type = "Firebird";
					break;
                case "Postgre":
                    connName = "SERVER=localhost;ENCODING=UNICODE;DATABASE=prio;USER ID=postgres;PASSWORD=abundance;";
                    type = "Postgre";
                    break;
                case "Sqlite":
                    connName = @"Data Source=D:\Projekte\NDO\SqliteProvider\NdoUnitTests.db";
                    type = "Sqlite";
                    break;
                case "SqlCe":
                    connName = @"Data Source=""D:\Projekte\NDO\SqlCeProvider\NDOUnitTests.sdf"";Password='ndo'";
                    type = "SqlCe";
                    break;
                default:
					Console.WriteLine("ManipulateConnection: can't find connection type " + args[1]);
					return -1;
			}
			Console.WriteLine("ManipulateConnection: Setting Connection: " + Path.GetFullPath(args[0]) + " '" + connName + '\'');
			try
			{
				NDOMapping mapping = new NDOMapping(Path.GetFullPath(args[0]));
				if (mapping.Connections.Count() != 1)
				{
					Console.WriteLine("Wrong connection count: " + mapping.Connections.Count().ToString());
					return -1;
				}
				((Connection)mapping.Connections.First()).Name = connName;
				((Connection)mapping.Connections.First()).Type = type;
				mapping.Save();
			}
			catch(Exception ex)
			{
				Console.WriteLine("ManipulateConnection: " + ex.Message);
				return -3;
			}

			return 0;
		}
	}
}
