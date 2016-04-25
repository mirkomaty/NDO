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
using System.Text;
using System.IO;
using System.Data;
using System.Collections.Generic;
using NDO;
using NDO.Linq;
using NDO.Mapping;
using BusinessClasses;
using NDOInterfaces;
using FirebirdSql.Data.FirebirdClient;

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

			pm.IdGenerationEvent += new IdGenerationHandler(IdGenerator.Generate);
			IProvider p = NDO.NDOProviderFactory.Instance["Firebird"];
			NDO.NDOProviderFactory.Instance["Firebird"] = new NDO.FirebirdProvider.Provider();
            p = NDO.NDOProviderFactory.Instance["Firebird"];

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

			List<DataContainer> list = pm.Objects<DataContainer>();

			//VirtualTable<DataContainer> vt = pm.Objects<DataContainer>().Where( dc => dc.StringVar.Like( "T%" ) );
			//Console.WriteLine(vt.QueryString);

//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.BoolVar == true select dc;
//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.ByteVar == 0x55 select dc;
			
			//DateTime dt = DateTime.Today;
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.DateTimeVar == dt select dc;

            //DateTime dt1 = DateTime.Today - TimeSpan.FromDays(1);
            //DateTime dt2 = DateTime.Today + TimeSpan.FromDays(1);
            //Query q = pm.NewQuery(typeof (DataContainer), qh.dateTimeVar + " BETWEEN {0} AND {1}");
            //q.Parameters.Add(dt1);
            //q.Parameters.Add(dt2);
//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.DecVar > 0.34m select dc;
//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.DoubleVar < 6.54 select dc;
//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.FloatVar <= 10 select dc;
//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where 10 >= dc.FloatVar select dc;
//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.Int64Var == 0x123456781234567 select dc;

//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.StringVar == "Test" select dc;

//			List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.StringVar.Like( "T%" ) select dc;

//			List<DataContainer> list = new NDOQuery<DataContainer>( pm, "stringVar LIKE 'T%' ESCAPE '\\'" ).Execute();
//			List<DataContainer> list = new NDOQuery<DataContainer>( pm, "SELECT * FROM \"DataContainer\" WHERE \"StringVar\" LIKE 'T%'", false, Query.Language.Sql).Execute();

			if (list.Count == 0)
			{
				Console.WriteLine("Nothing found");
			}
			else
			{
				DataContainer dc = (DataContainer) list[0];
				//Console.WriteLine(dc.DoubleVar);
				Console.WriteLine(dc.ByteVar);
				Console.WriteLine(dc.GuidVar);
				//Console.WriteLine(dc.DecVar.ToString());
			}
			Console.WriteLine("Ready");		
			
#endif			
		}

        static void GenerateDatabase()
        {
            // This code is used to generate a database during development of a provider.
            // If the generator is available, NDO generates the DDL code using the generator.
            IProvider p = NDOProviderFactory.Instance["Firebird"];
            ISqlGenerator gen = (ISqlGenerator)NDOProviderFactory.Instance.Generators["Firebird"];
            PersistenceManager pm = new PersistenceManager();
            Type t = typeof(DataContainer);
            string myPath = typeof(Class1).Assembly.Location;
            myPath = Path.ChangeExtension(myPath, ".ndo.sql");
            TextWriter sw = new StreamWriter(myPath, false, Encoding.UTF8);
            //TextWriter sw = Console.Out;
            Type oidType = typeof(int);
            foreach (Class cl in pm.NDOMapping.Classes)
            {
                sw.WriteLine("DROP TABLE " + p.GetQuotedName(cl.TableName) + ";");
                sw.WriteLine("CREATE TABLE " + p.GetQuotedName(cl.TableName) + " (");
                sw.Write("\"ID\" INTEGER NOT NULL,");
                foreach (Field f in cl.Fields)
                {
                    FieldInfo fi = t.GetField(f.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                    sw.Write(p.GetQuotedName(f.Column.Name) + " " + gen.DbTypeFromType(fi.FieldType));
                    if (fi.FieldType == typeof(string))
                        sw.Write("(255)");
                    if (fi.FieldType == typeof(Guid))
                        sw.Write("(36)");
                    sw.Write(" DEFAULT NULL");
                        sw.WriteLine(",");
                }
                sw.WriteLine("CONSTRAINT \"PK_DataContainer\" PRIMARY KEY (\"ID\")");
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


