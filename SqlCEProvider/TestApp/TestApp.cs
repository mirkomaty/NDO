//
// Copyright (C) 2002-2015 Mirko Matytschak 
// (www.netdataobjects.de)
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
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
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
            NDOProviderFactory.Instance["SqlCe"] = new NDO.SqlCeProvider.Provider();
            if (!NDOProviderFactory.Instance.Generators.ContainsKey("SqlCe"))
				NDOProviderFactory.Instance.Generators.Add( "SqlCe", new NDO.SqlCeProvider.SqlCeGenerator() );
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

			// Uncomment the following two lines to view the generated queries.
			 //VirtualTable<DataContainer> vt = pm.Objects<DataContainer>().Where( dc => dc.DateTimeVar.Between(dt1, dt2) );
			 //Console.WriteLine(vt.QueryString);

			// uncomment the queries you like to test

			List<DataContainer> list = pm.Objects<DataContainer>();

			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.BoolVar == true select dc;
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.ByteVar == 0x55 select dc;
			
			//DateTime dt = DateTime.Today;
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.DateTimeVar == dt select dc;

			//DateTime dt1 = DateTime.Today.AddDays(-10);
			//DateTime dt2 = DateTime.Today.AddDays(1);
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.DateTimeVar.Between(dt1, dt2) select dc;

			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.DecVar > 0.34m select dc;
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.DoubleVar < 6.54 select dc;
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.FloatVar <= 10 select dc;
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where 10 >= dc.FloatVar select dc;
			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.Int64Var == 0x123456781234567 select dc;

			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.StringVar == "Test" select dc;

			//List<DataContainer> list = from dc in pm.Objects<DataContainer>() where dc.StringVar.Like( "T%" ) select dc;

			//List<DataContainer> list = new NDOQuery<DataContainer>( pm, "stringVar LIKE 'T%' ESCAPE '\\'" ).Execute();
			//List<DataContainer> list = new NDOQuery<DataContainer>( pm, "SELECT * FROM \"DataContainer\" WHERE \"StringVar\" LIKE 'T%'", false, Query.Language.Sql).Execute();

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
            // This code is used to generate a database while the generator is not available.
            // If the generator is available, NDO will generate the DDL code using the generator.
            PersistenceManager pm = new PersistenceManager();

			foreach ( string s in pm.BuildDatabase())
			{
				Console.WriteLine(s);
			} 
        }
	}
}


