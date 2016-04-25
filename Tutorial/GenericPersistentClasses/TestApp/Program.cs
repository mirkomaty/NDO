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
			GenerateData();
			QueryData();
			//GenericProperty<int>.MyQueryHelper qh = new GenericProperty<int>.MyQueryHelper();
			//Console.WriteLine(qh.name);
        }


        static void GenerateData()
        {
            PersistenceManager pm = new PersistenceManager();            
            pm.BuildDatabase();

            // All primitive types and the storable value types in .NET
            // support the IConvertible interface. You can just store them.
            GenericProperty<string> gp1 = new GenericProperty<string>("String");
            gp1.Value = "StringVal";
            pm.MakePersistent(gp1);
            GenericProperty<decimal> gp2 = new GenericProperty<decimal>("Decimal");
            gp2.Value = 1.23m;
            pm.MakePersistent(gp2);
            GenericProperty<DateTime> gp3 = new GenericProperty<DateTime>("DateTime");
            gp3.Value = DateTime.Now;
            pm.MakePersistent(gp3);
            GenericProperty<int> gp4 = new GenericProperty<int>("Integer");
            gp4.Value = 234;
            pm.MakePersistent(gp4);

            // Any type having a type converter, which can convert to/from string
            // can be stored using this technology.
            GenericProperty<UserDefinedType> gp5 = new GenericProperty<UserDefinedType>("User Defined Type");
            UserDefinedType udt = new UserDefinedType();
            udt.A = 234;
            udt.B = 33.3;
            gp5.Value = udt;
            pm.MakePersistent(gp5);

            pm.Save();            
        }

        static void QueryData()
        {
            PersistenceManager pm = new PersistenceManager();
            Query q = pm.NewQuery(typeof(GenericProperty<>));
            IList l = q.Execute();
            foreach (IProperty p in l)
                Console.WriteLine(p.Name + ": " + p.PolymorphicValue);
        }
         
    }
}
