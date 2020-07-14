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
using System.Reflection;
using System.IO;
using System.Text;
using System.Data;
using NDO;
using System.Collections;
using NDO.Mapping;
using NDOInterfaces;
using BusinessClasses;

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
			PersistenceManager pm = new PersistenceManager();

			IProvider p = NDO.NDOProviderFactory.Instance["MySql"];
			NDO.NDOProviderFactory.Instance["MySql"] = new NDO.MySqlProvider.Provider();
			
			
			pm.VerboseMode = true;

			// Make this true to store the test instance. 
			// Make it false to retrieve the test instance from the database.
#if true
            GenerateDatabase();
			DataContainer dc = new DataContainer();
			pm.MakePersistent(dc);
			pm.Save();
			Console.WriteLine("Fertig");
#else			

			// uncomment the queries you like to test

			DataContainer.QueryHelper qh = new DataContainer.QueryHelper();
			Query q = pm.NewQuery(typeof (DataContainer), null);
//			Query q = pm.NewQuery(typeof (DataContainer), qh.boolVar + " = true");
//          Query q = pm.NewQuery(typeof(DataContainer), qh.boolVar + " = 1");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.byteVar + " = " + (0x55).ToString());
//          Query q = pm.NewQuery(typeof (DataContainer), qh.byteArrVar + " = {0}");
//          q.Parameters.Add(new byte[] { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16});
			
//			Query q = pm.NewQuery(typeof (DataContainer), qh.dateTimeVar + " = {0}");
//          q.Parameters.Add(DateTime.Now.Date);
//			Query q = pm.NewQuery(typeof (DataContainer), qh.dateTimeVar + " BETWEEN {0} AND {1}");
//          q.Parameters.Add(DateTime.Now.Date - TimeSpan.FromDays(1));
//          q.Parameters.Add(DateTime.Now.Date + TimeSpan.FromDays(1));

//			Query q = pm.NewQuery(typeof (DataContainer), qh.decVar + " > 0.34");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.doubleVar + " < 6.54");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.floatVar + " <= 10");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.int64Var + " = " + (0x123456781234567).ToString());
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " = \"Test\"");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " = \'Test\'");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " LIKE 'T*'", false);
//			Query q = pm.NewQuery(typeof (DataContainer), "SELECT * FROM `DataContainer` WHERE `StringVar` LIKE 'T%'", false, Query.Language.Sql);

			Console.WriteLine(q.GeneratedQuery);
			IList l = q.Execute();
			if (l.Count == 0)
			{
				Console.WriteLine("Nichts gefunden");
			}
			else
			{
				DataContainer dc = (DataContainer) l[0];
				//Console.WriteLine(dc.DoubleVar);
				Console.WriteLine(dc.ByteVar);
                foreach (Byte b in dc.ByteArrVar)
                    Console.Write(b.ToString() + " ");
                Console.WriteLine("");
				Console.WriteLine(dc.GuidVar);
				//Console.WriteLine(dc.DecVar.ToString());
			}
			Console.WriteLine("Fertig");
			
#endif			
		}

        public static void GenerateDatabase()
        {
            // This code is used to generate a database during development of a provider.
            // Once the generator is available, NDO generates the DDL code using the generator.
            IProvider p = NDOProviderFactory.Instance["MySql"];
            ISqlGenerator gen = new MySqlProvider.MySqlGenerator();
            PersistenceManager pm = new PersistenceManager();
            Type t = typeof(DataContainer);
            string myPath = typeof(Class1).Assembly.Location;
            myPath = Path.ChangeExtension(myPath, ".ndo.sql");
            TextWriter sw = new StreamWriter(myPath, false, Encoding.UTF8);
            //TextWriter sw = Console.Out;
            Type oidType = typeof(int);
            foreach (Class cl in pm.NDOMapping.Classes)
            {
                sw.WriteLine("DROP TABLE IF EXISTS" + p.GetQuotedName(cl.TableName) + ";");
                sw.WriteLine("CREATE TABLE " + p.GetQuotedName(cl.TableName) + " (");
                sw.Write("`ID` Int AUTO_INCREMENT NOT NULL,");
                foreach (Field f in cl.Fields)
                {
                    FieldInfo fi = t.GetField(f.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                    sw.Write(p.GetQuotedName(f.Column.Name) + " " + gen.DbTypeFromType(fi.FieldType));
                    if (fi.FieldType == typeof(string))
                        sw.Write("(255)");
                    if (fi.FieldType == typeof(Guid))
                        sw.Write("(36)");
                    sw.Write(" NULL");
                    sw.WriteLine(",");
                }
                sw.WriteLine("CONSTRAINT `PK_DataContainer` PRIMARY KEY (`ID`)");
                sw.WriteLine(");");
            }
            if (sw != Console.Out)
                sw.Close();
            string[] arr = pm.BuildDatabase();
            //foreach(string s in arr)
            //    Console.WriteLine(s); ;

        }
	}
}


