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
using System.Collections.Generic;
using System.Collections;

namespace NDOEnhancer
{
    internal class StandardListReflector : IListReflector
    {
        Type t;
        public StandardListReflector(Type t)
        {
            this.t = t;
        }

        public bool CallvirtNeedsClassPrefix { get { return false; } }

        readonly string[] funcNamesList = { "Add", "AddRange", "Clear", "Insert", "InsertRange", "Remove", "SetRange", "RemoveAt", "RemoveRange", "set_Item" };
        readonly string[] funcNamesIList = { "Add", "Clear", "Remove", "Insert", "RemoveAt", "set_Item" };

        public List<MethodInfo> GetMethods()
        {
            string[] funcNames = funcNamesIList;

            var result = new List<MethodInfo>();
            Type t2 = t;
            if (typeof(ArrayList).IsAssignableFrom(t))
                funcNames = funcNamesList;
            else if (t.IsInterface && typeof(IList).IsAssignableFrom(t))
                t2 = typeof(IList);
            else if (!(t.IsClass && typeof(IList).IsAssignableFrom(t)))
                throw new Exception("Invalid relation field type " + t.FullName + ". The field type must be assignable to IList.");


            MethodInfo[] mis = t2.GetMethods();
            foreach(MethodInfo mi in mis)
            {
                foreach (string fname in funcNames)
                {
                    if (mi.Name == fname)
                    {
                        MethodInfo baseMi = mi.GetBaseDefinition();
                        result.Add(baseMi);
                        break;
                    }
                }
            }
            return result;
        }

		public Type ReflectedType
		{
			get { return this.t; }
		}
    }
}
