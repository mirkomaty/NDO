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


#if nix
using System;
using NDO;
using NDO.Mapping;

namespace NdoUnitTests
{
	/// <summary>
	/// Zusammenfassung f�r ConnectionGenerator.
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
