using System;
using System.Collections;
using System.Collections.Generic;
using BusinessClasses;
using NDO;
namespace Client
{
	class Class1
	{
		static void Main( string[] args )
		{			
			CheckBits();
			Write();
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
			pm.BuildDatabase();
			pm.MakePersistent( e );
			pm.Save();
		}

		static void Read()
		{
			Console.WriteLine("Reading Data");
			PersistenceManager pm = new PersistenceManager();
			Query q = pm.NewQuery( typeof( Employee ), null );
			IList lstEmployee = q.Execute();
			foreach (Employee e in lstEmployee)
			{
				Console.WriteLine( e.FirstName + " " + e.LastName );
			}
		}

		static void CheckBits()
		{
			Console.WriteLine("Running as " + (IntPtr.Size * 8) + " bit app.");
		}
	}

}
