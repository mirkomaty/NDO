//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using NDO;
using NDO.Linq;
using NDO.Mapping;
using NDOInterfaces;
using BusinessClasses;
using System.Collections.Generic;
using NDO.Query;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;

namespace TestApp
{
	/// <summary>
	/// Test app for the provider.
	/// </summary>
	class Class1
	{
		[STAThread]
		static void Main( string[] args )
		{
			NDOProviderFactory.Instance["Sqlite"] = new NDO.SqliteProvider.Provider();
			if (!NDOProviderFactory.Instance.Generators.ContainsKey( "Sqlite" ))
				NDOProviderFactory.Instance.Generators.Add( "Sqlite", new NDO.SqliteProvider.SqliteGenerator() );
			PersistenceManager pm = new PersistenceManager();

			pm.LogAdapter = new NDO.Logging.ConsoleLogAdapter();
			pm.VerboseMode = true;

			//Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			// Make this true to store the test instance. 
			// Make it false if you want to retrieve data.
#if true
            GenerateDatabase();

            DataContainer dc = new DataContainer();
            pm.MakePersistent(dc);
			pm.Save();
			Console.WriteLine("Fertig");
#else

			Dump( pm.Objects<DataContainer>().ResultTable, 1 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.BoolVar == true ).ResultTable, 1 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.ByteVar == 0x55 ).ResultTable, 3 );

			DateTime dt = new DataContainer().DateTimeVar;
			Dump( pm.Objects<DataContainer>().Where( dc => dc.DateTimeVar == dt ).ResultTable, 4 );
			DateTime dt1 = new DateTime( 2006, 12, 6, 0, 0, 0 );
			DateTime dt2 = new DateTime( 2006, 12, 8, 23, 0, 0 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.DateTimeVar.Between( dt1, dt2 ) ).ResultTable, 5 );

			Dump( pm.Objects<DataContainer>().Where( dc => dc.DecVar > 0.34m ).ResultTable, 6 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.DoubleVar < 6.54 ).ResultTable, 7 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.FloatVar <= 10 ).ResultTable, 8 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.Int64Var == 0x123456781234567 ).ResultTable, 9 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.StringVar == "Test" ).ResultTable, 10 );
			Dump( pm.Objects<DataContainer>().Where( dc => dc.StringVar.Like( "T%" ) ).ResultTable, 11 );
			Dump( new NDOQuery<DataContainer>( pm, "SELECT * FROM \"DataContainer\" WHERE \"StringVar\" LIKE 'T%'", false, QueryLanguage.Sql ).Execute(), 12 );


			pm.VerboseMode = true;
			pm.LogAdapter = new NDO.Logging.ConsoleLogAdapter();
			Console.WriteLine( "Fertig" );

#endif

		}

		private static void Dump( List<DataContainer> l, int nr )
		{
			Console.Write( $"{nr} " );
			if (l.Count == 0)
			{
				Console.WriteLine( "Nichts gefunden" );
			}
			else
			{
				//DataContainer dc = (DataContainer)l[0];
				//Console.WriteLine( dc.DecVar );
				//Console.WriteLine( dc.GuidVar );
				//Console.WriteLine( dc.NDOObjectId.Id.Value );

				//Console.WriteLine(dc.Uint64Var);
				//Console.WriteLine(dc.ByteVar);
				//foreach ( Byte b in dc.ByteArrVar )
				//	Console.Write( b.ToString() + " " );
				//            Console.WriteLine("");
				//Console.WriteLine(dc.GuidVar);
				//Console.WriteLine(dc.DecVar.ToString());				 
				Console.WriteLine("OK");
			}
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
                if (cl.Fields.Count() > 0)
                    sw.WriteLine(",");
				int i = 0;
                foreach ( Field f in cl.Fields )
                {
                    FieldInfo fi = t.GetField(f.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                    sw.Write(p.GetQuotedName(f.Column.Name) + " " + gen.DbTypeFromType(fi.FieldType));
                    if (fi.FieldType == typeof(string))
                        sw.Write("(255)");
                    if (fi.FieldType == typeof(Guid))
                        sw.Write("(36)");
                    sw.Write(" NULL");
                    if (i++ < cl.Fields.Count() - 1)
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


