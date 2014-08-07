using System;
using System.Collections;
using System.Collections.Generic;
using BusinessClasses;
using NDO;
using NDO.DataExtensions;
using Newtonsoft.Json;

namespace Client
{
	class Class1
	{
		static void Main( string[] args )
		{			
			CheckBits();
//			Write();
			Read();			
		}

		static void Write()
		{
			Console.WriteLine( "Writing Data" );
			Employee e = new Employee();
			e.FirstName = "Mirko";
			e.LastName = "Matytschak";
			e.SetPosition( 10, 20 );
			PersistenceManager pm = new PersistenceManager();
			foreach (string s in  pm.BuildDatabase())
			{
				Console.WriteLine(s);
			}
			pm.MakePersistent( e );
			object o = pm.GetChangeSet( e );
			Console.WriteLine( JsonConvert.SerializeObject( o ) );
			pm.Save();
		}

		static void Read()
		{
			Console.WriteLine("Reading Data");
			PersistenceManager pm = new PersistenceManager();
			NDOQuery<Employee> q = new NDOQuery<Employee>(pm);
			Employee e = q.ExecuteSingle();
			e.FirstName = "Neu";
			object o = pm.GetChangeSet( e );
			Console.WriteLine( JsonConvert.SerializeObject( o ) );
		}

		static void CheckBits()
		{
			Console.WriteLine("Running as " + (IntPtr.Size * 8) + " bit app.");
		}
	}

}
