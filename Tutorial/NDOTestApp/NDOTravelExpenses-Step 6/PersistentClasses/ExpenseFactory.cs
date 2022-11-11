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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BusinessClasses
{
    public partial class ExpenseFactory
    {
        Dictionary <string,Type> theTypes;

		/// <summary>
		/// Constructs an ExpenseFactory object and collects all types derived by Expense
		/// </summary>
		public ExpenseFactory()
		{
			theTypes = new Dictionary<string, Type>();
			foreach (Type t in GetType().Assembly.GetTypes())
			{
				if (t == typeof(Expense))
					continue;
				if (!typeof( Expense ).IsAssignableFrom( t ))
					continue;

				DisplayNameAttribute dna = (DisplayNameAttribute)t.GetCustomAttributes( typeof(DisplayNameAttribute), false ).FirstOrDefault();
				if (dna != null)
					theTypes.Add( dna.DisplayName, t );
				else
					theTypes.Add( t.Name, t );
			}
		}

		/// <summary>
		/// Get the names of the types we can instantiate in the system
		/// </summary>
        public string[] Types
        {
            get { return theTypes.Keys.ToArray(); }
        }

		/// <summary>
		/// Instantiate an Expense object which type is determined by the typeName
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
        public Expense NewExpense(string typeName)
        {
			if (!this.theTypes.ContainsKey(typeName))
                    throw new Exception (String.Format("Unknown Expense Type: {0}", typeName));

			return (Expense) Activator.CreateInstance( theTypes[typeName] );            
        }
    }
}
