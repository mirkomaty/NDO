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
using System.Reflection;
using System.Collections;
using System.Text;

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

        /*
        bool ParameterInfoEquals(ParameterInfo[] arr1, ParameterInfo[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i].ParameterType != arr2[i].ParameterType)
                    return false;
            }
            return true;
        }


        MethodInfo GetMethod(Type t, string name, ParameterInfo[] parameterInfos)
        {
            MethodInfo[] infos = t.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo mi in infos)
            {
                if (mi.Name == name
                    && ParameterInfoEquals(parameterInfos, mi.GetParameters()))
                {
                    return mi;
                }
            }
            return null;
        }
        */
        readonly string[] funcNamesList = { "Add", "AddRange", "Clear", "Insert", "InsertRange", "Remove", "SetRange", "RemoveAt", "RemoveRange", "set_Item" };
        readonly string[] funcNamesIList = { "Add", "Clear", "Remove", "Insert", "RemoveAt", "set_Item" };

        public IList GetMethods()
        {
            string[] funcNames = funcNamesIList;

            ArrayList result = new ArrayList();
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
