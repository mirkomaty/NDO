using System;
using System.Collections;
using System.Collections.Generic;
using BusinessClasses;
using NDO;
namespace TestApp
{
    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
			CheckBits();
            
            Employee e = new Employee();
            e.FirstName = "Scott";
            e.LastName = "Lion";
            Address a = new Address();
            a.CountryCode = "USA";
            a.Zip = "CA 94065";
            a.Street = "501 NDO Parkway";
            a.City = "Redwood Shores";
            // By the way, did you know, that NDO works with Oracle, too? ;-)
            e.Address = a;
            PersistenceManager pm = new PersistenceManager();
            pm.BuildDatabase();
            pm.MakePersistent(e);
            pm.Save();
             
            /* Query Code
            PersistenceManager pm = new PersistenceManager();
            NDOQuery<Employee> q = new NDOQuery<Employee>(pm);
            List<Employee> lstEmployee = q.Execute();
            Employee e = lstEmployee[0];
            Console.WriteLine(e.FirstName + " " + e.LastName);
            Console.WriteLine(e.Address.Street);
            Console.WriteLine(e.Address.City + ", " + e.Address.Zip);
            Console.WriteLine(e.Address.CountryCode);
            */
        }

		static void CheckBits()
		{
			Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
		}
    }
}
