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
