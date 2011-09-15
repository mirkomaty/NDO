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
            pm.BuildDatabase(); // Make sure to build the database
              
            Country c1 = new Country(); 
            c1.Name = "Germany"; 
            pm.MakePersistent(c1); 
            Country c2 = new Country(); 
            c2.Name = "USA"; 
            pm.MakePersistent(c2); 
              
            Employee e = new Employee(); 
            e.FirstName = "John"; 
            e.LastName = "Doe";   
            Travel t = e.NewTravel(); 
            t.Purpose = "Inhouse-Training"; 
            t.AddCountry(c1); 
            t.AddCountry(c2); 
              
            t = e.NewTravel(); 
            t.Purpose = "TechEd 2006"; 
            t.AddCountry(c1); 
              
            Address a = new Address(); 
            a.CountryCode = "USA"; 
            a.Zip="CA 12345"; 
            a.Street = "10, Wounderway"; 
            a.City = "Dreamcity"; 
            e.Address = a; 
            pm.MakePersistent(e); 
            pm.Save();
            

            Query1();
            Query2();
            Query3();
            Query4();
        }

        static void Query1()
        {
            Console.WriteLine("--------- Query 1 --------------");
            PersistenceManager pm = new PersistenceManager();
            Query q = pm.NewQuery(typeof(Employee));
            IList l = q.Execute();
            foreach (Employee e in l)
            {
                Console.WriteLine(e.FirstName + " " + e.LastName + ", " + e.Address.City);
                foreach (Travel t in e.Travels)
                {
                    Console.WriteLine(" Travel: " + t.Purpose);
                    foreach (Country country in t.Countries)
                        Console.WriteLine("  Country:" + country.Name);
                }
            }
        }
        static void Query2()
        {
            Console.WriteLine("--------- Query 2 --------------");
            PersistenceManager pm = new PersistenceManager();
            Query q = pm.NewQuery(typeof(Country));
            IList l = q.Execute();
            foreach (Country country in l)
            {
                Console.WriteLine(country.Name);
                foreach (Travel t in country.Travels)
                    Console.WriteLine(" Travel: " + t.Purpose);
            }
        }
        static void Query3()
        {
            Console.WriteLine("--------- Query 3 --------------");
            PersistenceManager pm = new PersistenceManager();
            Query q = pm.NewQuery(typeof(Country));
            IList l = q.Execute();
            foreach (Country country in l)
            {
                Console.WriteLine(country.Name);
                foreach (Travel t in country.Travels)
                    Console.WriteLine(" Employee: " + t.Employee.LastName + ", Travel: " + t.Purpose);
            }
        }
        static void Query4()
        {
            Console.WriteLine("--------- Query 4 --------------");
            PersistenceManager pm = new PersistenceManager();
            Employee.QueryHelper qh = new Employee.QueryHelper();
            Query q = pm.NewQuery(typeof(Employee), qh.travels.countries.name + " LIKE 'USA'");
            Console.WriteLine(q.GeneratedQuery);
            IList l = q.Execute();
            foreach (Employee e in l)
                Console.WriteLine(e.FirstName + " " + e.LastName);
        }

		static void CheckBits()
		{
			Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
		}
    }
}
