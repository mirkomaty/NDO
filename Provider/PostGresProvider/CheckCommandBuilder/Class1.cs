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
using Npgsql;
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
            //Npgsql.Design.ConnectionStringEditorForm frm = new Npgsql.Design.ConnectionStringEditorForm("Server=127.0.0.1;Port=5432;User Id=postgres;Password=abundance;Database=prio;");
            //frm.ShowDialog();
			NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=abundance;Database=prio;");
            NpgsqlCommand cmd = new NpgsqlCommand("Insert into datacontainer values(2, 'abc'); select * from datacontainer", conn);
            conn.Open();
            NpgsqlDataReader r = cmd.ExecuteReader();
            r.NextResult();
            r.Read();
            Console.WriteLine(r.GetInt32(0));
            conn.Close();
            //NpgsqlCommandBuilder cb = new NpgsqlCommandBuilder(a);
            //conn.Open();
            //DataSet ds = new DataSet();
            //ds.Tables.Add("Test");
            //DataTable dt = ds.Tables["Test"];
            //dt.Columns.Add("intCol", typeof(int));
            //dt.Columns.Add("stringCol", typeof(string));
            //dt.Columns.Add("boolCol", typeof(bool));
            //DataRow row = dt.NewRow();
            //NpgsqlCommand cmd = cb.GetInsertCommand(row);
            //string s = cmd.CommandText;// cb.GetInsertCommand(row).CommandText;
            //foreach(NpgsqlParameter par in cmd.Parameters)
            //    Console.WriteLine("   " + par.NpgsqlDbType + " " + par.Size);
            ////Clipboard.SetDataObject(s);
            //Console.WriteLine(s);
            //conn.Close();
            //Console.ReadLine();
		}
	}
}
