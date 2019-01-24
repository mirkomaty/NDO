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
using System.Data;
using System.Data.SqlServerCe;
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
            string connectionString = @"Data Source=""D:\Projekte\NDO\SqlCeProvider\NDOUnitTests.sdf"";Password='ndo'";
            SqlCeEngine en = new SqlCeEngine(connectionString);
//          en.CreateDatabase();

			// For this code to work you have to create a SqlCe database and adjust the 
			// table and column names below.
			SqlCeConnection conn = new SqlCeConnection(connectionString);
            conn.Open();
            //SqlCeCommand cmd = new SqlCeCommand("CREATE TABLE Mitarbeiter (ID int IDENTITY (100,1) PRIMARY KEY, Name nvarchar (50))", conn);
            //cmd.ExecuteNonQuery();

			SqlCeDataAdapter a = new SqlCeDataAdapter( "Select * from Mitarbeiter", conn );
			SqlCeCommandBuilder cb = new SqlCeCommandBuilder( a );

            SqlCeCommand cmd = cb.GetUpdateCommand();
			string s = cmd.CommandText;
			foreach ( SqlCeParameter par in cmd.Parameters )
				Console.WriteLine( "   " + par.DbType + " " + par.Size );
			//Clipboard.SetDataObject(s);
			Console.WriteLine( s );
			conn.Close();
			Console.ReadLine();
		}
	}
}
