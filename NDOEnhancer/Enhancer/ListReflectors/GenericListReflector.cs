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


#if !NET11
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NDOEnhancer
{
    internal class GenericListReflector : IListReflector
    {
        Type t;
        public GenericListReflector(Type t)
        {
            this.t = t;
        }

        readonly string[] funcNamesList = { "Add", "AddRange", "Clear", "Insert", "InsertRange", "Remove", "RemoveAll", "RemoveAt", "RemoveRange", "set_Item" };
        readonly string[] funcNamesIList = { "Add", "Clear", "Remove", "Insert", "RemoveAt", "set_Item" };

		public Type ReflectedType
		{
			get { return this.t; }
		}

        public IList GetMethods()
        {
            string[] funcNames = funcNamesIList;

            // Check, if t is derived from List<T>
            Type[] parameters = t.GetGenericArguments();
            Type template = typeof(List<object>).GetGenericTypeDefinition();
            Type concrete = template.MakeGenericType(parameters);
            if (concrete.IsAssignableFrom(t))
                funcNames = funcNamesList;

            ArrayList result = new ArrayList();
            Type t2 = t;
            while (t2 != null && t2 != typeof(object))
            {
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
                t2 = t2.BaseType;
            }
            return result;
        }
        public bool CallvirtNeedsClassPrefix { get { return true; } }

    }
}
#endif