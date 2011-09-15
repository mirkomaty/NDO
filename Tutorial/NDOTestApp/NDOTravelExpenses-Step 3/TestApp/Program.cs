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
            
            PersistenceManager pm = new PersistenceManager();
            pm.BuildDatabase();
            Employee e = new Employee();
            e.FirstName = "John";
            e.LastName = "Doe";

            Travel t = e.NewTravel();
            t.Purpose = "Inhouse Training";
            t = e.NewTravel();
            t.Purpose = "TechEd 2006";

            Address a = new Address();
            a.CountryCode = "USA";
            a.Zip = "CA 12345";
            a.Street = "10, Wonderway";
            a.City = "Dreamcity";
            e.Address = a;
            pm.MakePersistent(e);
            pm.Save();
            
            /* Query code
            PersistenceManager pm = new PersistenceManager();
            Query q = pm.NewQuery(typeof(Employee));
            IList l = q.Execute();
            foreach (Employee e in l)
            {
                Console.WriteLine(e.FirstName + " " + e.LastName + ", " + e.Address.City);
                foreach (Travel t in e.Travels)
                    Console.WriteLine(" Travel: " + t.Purpose);
            }
            */
            
        }

		static void CheckBits()
		{
			Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
		}
    }
}
