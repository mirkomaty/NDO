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
					connName = @"Password=abundance;User ID=SYSTEM;Data Source=maty;Persist Security Info=True";
					type = "Oracle";
					break;
				case "SqlServer":
					connName = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=NDOTest;Data Source=.\sqlexpress";
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
                case "TurboDB":
                    connName = @"C:\Projekte\NDO\TurboDbProvider\TestDb.tdbd";
                    type = "TurboDB";
                    break;
				default:
					Console.WriteLine("ManipulateConnection: can't find connection type " + args[1]);
					return -1;
			}
			Console.WriteLine("ManipulateConnection: Setting Connection: " + Path.GetFullPath(args[0]) + " '" + connName + '\'');
			try
			{
				NDOMapping mapping = new NDOMapping(Path.GetFullPath(args[0]));
				if (mapping.Connections.Count != 1)
				{
					Console.WriteLine("Wrong connection count: " + mapping.Connections.Count.ToString());
					return -1;
				}
				((Connection)mapping.Connections[0]).Name = connName;
				((Connection)mapping.Connections[0]).Type = type;
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
