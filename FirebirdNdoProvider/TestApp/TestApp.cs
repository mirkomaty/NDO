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
using System.Reflection;
using System.Text;
using System.IO;
using System.Data;
using System.Collections;
using NDO;
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
//			Query q = pm.NewQuery(typeof (DataContainer), qh.boolVar + " = 1");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.byteVar + " = " + (0x55).ToString());
			
            //DateTime dt = DateTime.Today;
            //Query q = pm.NewQuery(typeof (DataContainer), qh.dateTimeVar + " = {0}");
            //q.Parameters.Add(dt);

            //DateTime dt1 = DateTime.Today - TimeSpan.FromDays(1);
            //DateTime dt2 = DateTime.Today + TimeSpan.FromDays(1);
            //Query q = pm.NewQuery(typeof (DataContainer), qh.dateTimeVar + " BETWEEN {0} AND {1}");
            //q.Parameters.Add(dt1);
            //q.Parameters.Add(dt2);

//			Query q = pm.NewQuery(typeof (DataContainer), qh.decVar + " > 0.34");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.doubleVar + " < 6.54");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.floatVar + " <= 10");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.int64Var + " = " + (0x123456781234567).ToString());
//			The following line throws an exception. Only single quotes are allowed.
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " = \"Test\"");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " = \'Test\'");
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " LIKE 'T*'", false);
//			Query q = pm.NewQuery(typeof (DataContainer), qh.stringVar + " LIKE 'T%' ESCAPE '\\'");
//			Query q = pm.NewQuery(typeof (DataContainer), "SELECT * FROM \"DataContainer\" WHERE \"StringVar\" LIKE 'T%'", false, Query.Language.Sql);

			Console.WriteLine(q.GeneratedQuery);
			IList l = q.Execute();
			if (l.Count == 0)
			{
				Console.WriteLine("Nothing found");
			}
			else
			{
				DataContainer dc = (DataContainer) l[0];
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
                for (int i = 0; i < cl.Fields.Count; i++)
                {
                    Field f = (Field)cl.Fields[i];
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


