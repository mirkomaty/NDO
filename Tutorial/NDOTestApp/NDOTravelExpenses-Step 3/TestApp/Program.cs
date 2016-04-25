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

			Read();
            
        }

		static void Read()
		{
			PersistenceManager pm = new PersistenceManager();
			Query q = pm.NewQuery( typeof( Employee ) );
			IList l = q.Execute();
			foreach (Employee e in l)
			{
				Console.WriteLine( e.FirstName + " " + e.LastName + ", " + e.Address.City );
				foreach (Travel t in e.Travels)
					Console.WriteLine( " Travel: " + t.Purpose );
			}
		}

		static void CheckBits()
		{
			Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
		}
    }
}
