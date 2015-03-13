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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BusinessClasses;
using NDO;
using NDO.Logging;
using NDODev.Linq;
using NDOInterfaces;

namespace TestApp
{
    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
#if GenerateData
            PersistenceManager pm = new PersistenceManager();
            pm.BuildDatabase();
            Employee e = new Employee();
            e.FirstName = "Mirko";
            e.LastName = "Matytschak";
            e.GeburtsJahr = 1958;

            Travel t = e.NewTravel();
            t.Purpose = "Inhouse Training";
            t = e.NewTravel();
            t.Purpose = "ADC 2006";

            Address a = new Address();
            a.CountryCode = "D";
            a.Zip = "83646";
            a.Street = "Brauneckstr. 9";
            a.City = "Bad Tölz";
            e.Address = a;
            pm.MakePersistent(e);
            pm.Save();
#endif


            PersistenceManager pm = new PersistenceManager();
            int year = 1957;
            List<Employee> l = 
                from e in pm.Objects<Employee>() where 
                  (e.FirstName == "Mirko" && e.GeburtsJahr > year) 
                  && e.Travels[Any.Index].Purpose.Like( "ADC 2006" )
				  && e.Position=="Manager"
				orderby e.GeburtsJahr descending
                select e;
 
            foreach (Employee e in l)
            {
                Console.WriteLine(e.FirstName + " " + e.LastName + ", " + e.Address.City);
                foreach (Travel t in e.Travels)
                    Console.WriteLine(" Travel: " + t.Purpose);
            }
        }
    }
}
