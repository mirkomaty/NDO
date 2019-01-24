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


#if nix
using System;
using NDO;
using NDO.Mapping;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung för ConnectionGenerator.
	/// </summary>
	public class ConnectionGenerator
	{
#if SQLSERVER
		public static string OnConnection(Connection conn)
		{
			return @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=NDOTest;Data Source=localhost";
		}
#endif
#if ORACLE
		public static string OnConnection(Connection conn)
		{
			return @"Password=manager;User ID=SYSTEM;Data Source=maty;Persist Security Info=True";
		}
#endif
#if MYSQL
		public static string OnConnection(Connection conn)
		{
			return @"Database=NDOTest;Data Source=localhost;User Id=root;";
		}
#endif
#if ACCESS
		public static string OnConnection(Connection conn)
		{
			return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Dokumente und Einstellungen\Mirko Matytschak\Eigene Dateien\db1.mdb;Persist Security Info=False";
		}
#endif

	}
}

#endif
