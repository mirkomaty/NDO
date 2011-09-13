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
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace CheckCommandBuilder
{
	/// <summary>
	/// Zusammenfassung für Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// For this code to work you have to create a SQLite database and adjust the 
			// table and column names below.
			SQLiteConnection conn = new SQLiteConnection(@"Data Source=D:\Projekte\NDO\SqliteProvider\NDOUnitTests.db");
			SQLiteDataAdapter a = new SQLiteDataAdapter( "Select * from Mitarbeiter", conn );
			SQLiteCommandBuilder cb = new SQLiteCommandBuilder( a );
			conn.Open();
			/*
			DataSet ds = new DataSet();
			ds.Tables.Add( "Test" );
			DataTable dt = ds.Tables["Test"];
			dt.Columns.Add( "intCol", typeof( int ) );
			dt.Columns.Add( "stringCol", typeof( string ) );
			dt.Columns.Add( "boolCol", typeof( bool ) );
			DataRow row = dt.NewRow();
			 */
			SQLiteCommand cmd = cb.GetInsertCommand();
			string s = cmd.CommandText;
			foreach ( SQLiteParameter par in cmd.Parameters )
				Console.WriteLine( "   " + par.DbType + " " + par.Size );
			//Clipboard.SetDataObject(s);
			Console.WriteLine( s );
			conn.Close();
			Console.ReadLine();
		}
	}
}
