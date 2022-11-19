//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NDOEnhancer
{
    internal class GenericIListReflector : IListReflector
    {
        Type t;
        public GenericIListReflector(Type t)
        {
            this.t = t;
        }

        readonly string[] funcNames = { "Add", "Clear", "Remove", "Insert", "RemoveAt", "set_Item"};

		public Type ReflectedType
		{
			get { return this.t; }
		}

		public static bool IsGenericIList(Type t)
		{
			bool isGenericIList = false;
			if (t.IsGenericType)
			{
				Type[] genericArgs = t.GetGenericArguments();
				if (genericArgs.Length != 1)
					genericArgs = new Type[] { genericArgs[0] };
				
				Type template = typeof(IList<object>).GetGenericTypeDefinition();
				Type concrete = template.MakeGenericType(genericArgs);
				isGenericIList = concrete.IsAssignableFrom(t);				
			}
			return isGenericIList;
		}

        public IList GetMethods()
        {
            ArrayList result = new ArrayList();
            Type[] parameters = t.GetGenericArguments();
            Type t2 = typeof(IList<object>).GetGenericTypeDefinition().MakeGenericType(parameters);
            if (!t2.IsAssignableFrom(t))
                throw new Exception("Invalid generic relation field type " + t.FullName + ". The field type must be assignable to IList<T> and IList.");

            MethodInfo[] mis = t2.GetMethods();
            foreach (MethodInfo mi in mis)
            {
                foreach (string fname in funcNames)
                {
                    if (mi.Name == fname)
                    {
                        result.Add(mi);
                        break;
                    }
                }
            }

            t2 = typeof(ICollection<object>).GetGenericTypeDefinition().MakeGenericType(parameters);

            mis = t2.GetMethods();
            foreach (MethodInfo mi in mis)
            {
                foreach (string fname in funcNames)
                {
                    if (mi.Name == fname)
                    {
                        result.Add(mi);
                        break;
                    }
                }
            }
            return result;
        }

        public bool CallvirtNeedsClassPrefix { get { return true; } }
    }
}
