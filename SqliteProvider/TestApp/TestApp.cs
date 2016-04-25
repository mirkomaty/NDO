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
using System.Reflection;
using System.IO;
using System.Data;
using System.Collections;
using System.Text;
using NDO;
using NDO.Mapping;
using NDOInterfaces;
using BusinessClasses;
using System.Threading;
using System.Globalization;

namespace TestApp
{
	/// <summary>
	/// Test app for the provider.
	/// </summary>
	class Class1
	{
		[STAThread]
		static void Main(string[] args)
		{
            NDOProviderFactory.Instance["Sqlite"] = new NDO.SqliteProvider.Provider();
            if (!NDOProviderFactory.Instance.Generators.ContainsKey("Sqlite"))
				NDOProviderFactory.Instance.Generators.Add( "Sqlite", new NDO.SqliteProvider.SqliteGenerator() );
			PersistenceManager pm = new PersistenceManager();

            pm.LogAdapter = new NDO.Logging.ConsoleLogAdapter();
            pm.VerboseMode = true;

			//Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			// Make this true to store the test instance. 
			// Make it false if you want to retrieve data.
#if false
            GenerateDatabase();

            DataContainer dc = new DataContainer();
            pm.MakePersistent(dc);
			pm.Save();
			Console.WriteLine("Fertig");
#else			

			// uncomment the queries you like to test

			DataContainer.QueryHelper qh = new DataContainer.QueryHelper();
			Query q = pm.NewQuery(typeof (DataContainer), null);
//			Query q = pm.NewQuery(typeof (DataContainer), qh.boolVar + " = {0}");
//			q.Parameters.Add( true );
//			Query q = pm.NewQuery(typeof (DataContainer), qh.byteVar + " = " + (0x55).ToString());
//          Query q = pm.NewQuery(typeof(DataContainer), qh.byteVar + " = {0}");
//          q.Parameters.Add(0x55);
			
//          DateTime dt = new DataContainer().DateTimeVar;
//          Query q = pm.NewQuery(typeof (DataContainer), qh.dateTimeVar + " = {0}");
//          q.Parameters.Add(dt);
//          DateTime dt1 = new DateTime(2006, 12, 6, 0, 0, 0);
//          DateTime dt2 = new DateTime(2006, 12, 8, 23, 0, 0);
//          Query q = pm.NewQuery(typeof (DataContainer), qh.dateTimeVar + " BETWEEN {0} AND {1}");
//          q.Parameters.Add(dt1);
//          q.Parameters.Add(dt2);

//          Query q = pm.NewQuery(typeof (DataContainer), qh.decVar + " > 0.34");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.doubleVar + " < 6.54");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.floatVar + " <= 10");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.int64Var + " = " + (0x123456781234567).ToString());
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " = \'Test\'");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " LIKE 'T*'", false);
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " LIKE 'T%'", false);
//			Query q = pm.NewQuery(typeof (DataContainer), "SELECT * FROM \"DataContainer\" WHERE \"StringVar\" LIKE 'T%'", false, Query.Language.Sql);

			
			pm.VerboseMode = true;
			pm.LogAdapter = new NDO.Logging.ConsoleLogAdapter();
			IList l = q.Execute();
			if (l.Count == 0)
			{
				Console.WriteLine("Nichts gefunden");
			}
			else
			{
				DataContainer dc = (DataContainer) l[0];
				Console.WriteLine(dc.DecVar);
				Console.WriteLine( dc.GuidVar );
				Console.WriteLine(dc.NDOObjectId.Id.Value);
				/*
				Console.WriteLine(dc.Uint64Var);
				//Console.WriteLine(dc.ByteVar);
				foreach ( Byte b in dc.ByteArrVar )
					Console.Write( b.ToString() + " " );
                Console.WriteLine("");
				Console.WriteLine(dc.GuidVar);
				//Console.WriteLine(dc.DecVar.ToString());
				 */
			}
			Console.WriteLine("Fertig");
			
#endif		
			
		}

        static void GenerateDatabase()
        {
            // This code is used to generate a database while the generator is not available.
            // If the generator is available, NDO will generate the DDL code using the generator.
            PersistenceManager pm = new PersistenceManager();
#if maskedout
            IProvider p = NDOProviderFactory.Instance["Sqlite"];
            ISqlGenerator gen = (ISqlGenerator)NDOProviderFactory.Instance.Generators[p.Name];
            Type t = typeof(DataContainer);
            string myPath = typeof(Class1).Assembly.Location;
            myPath = Path.ChangeExtension(myPath, ".ndo.sql");
            TextWriter sw = new StreamWriter(myPath, false, Encoding.UTF8);
            //TextWriter sw = Console.Out;
            foreach (Class cl in pm.NDOMapping.Classes)
            {
                sw.WriteLine("DROP TABLE " + p.GetQuotedName(cl.TableName) + ";");
                sw.WriteLine("CREATE TABLE " + p.GetQuotedName(cl.TableName) + " (");
                OidColumn oidColumn = (OidColumn)cl.Oid.OidColumns[0];
                sw.Write(gen.PrimaryKeyColumn(p.GetQuotedName(oidColumn.Name), typeof(Guid), gen.DbTypeFromType(typeof(Guid)), p.GetDefaultLength(typeof(Guid)).ToString()));
                sw.Write(" NOT NULL");
                if (cl.Fields.Count > 0)
                    sw.WriteLine(",");
                for (int i = 0; i < cl.Fields.Count; i++)
                {
                    Field f = (Field)cl.Fields[i];
                    FieldInfo fi = t.GetField(f.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                    sw.Write(p.GetQuotedName(f.Column.Name) + " " + gen.DbTypeFromType(fi.FieldType));
                    if (fi.FieldType == typeof(string))
                        sw.Write("(255)");
                    if (fi.FieldType == typeof(Guid))
                        sw.Write("(36)");
                    sw.Write(" NULL");
                    if (i < cl.Fields.Count - 1)
                        sw.WriteLine(",");
                    else
                        sw.WriteLine();
                }
                sw.WriteLine(");");
            }
            if (sw != Console.Out)
                sw.Close();
#endif
			foreach ( string s in pm.BuildDatabase())
			{
				Console.WriteLine(s);
			} 
        }
	}
}


