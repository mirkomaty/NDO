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
using NDOEnhancer.ILCode;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für ListAccessManipulator.
	/// </summary>
	internal class ListAccessManipulator
	{
		static Hashtable functions;
		string ownAssemblyName;

		public ListAccessManipulator(string ownAssemblyName)
		{
			this.ownAssemblyName = ownAssemblyName;
		}

		static ListAccessManipulator()
		{
			functions = new Hashtable(11);
			functions.Add("AddRange", $"AddRange(object,class {Corlib.Name}System.Collections.IEnumerable,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("InsertRange", $"InsertRange(object,int32,class {Corlib.Name}System.Collections.IEnumerable,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("RemoveRange", $"RemoveRange(object,int32,int32,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("SetRange", $"SetRange(object,int32,class {Corlib.Name}System.Collections.ICollection,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("Add", $"Add(object,object,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("Clear", $"Clear(object,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("Insert", $"Insert(object,int32,object,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("RemoveAt", $"RemoveAt(object,int32,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("Remove", $"Remove(object,object,class {Corlib.Name}System.Collections.Hashtable)");
			functions.Add("set_Item", $"SetItem(object,int32,object,class {Corlib.Name}System.Collections.Hashtable)");
            functions.Add("RemoveAll", $"RemoveAll(object,class {Corlib.Name}System.Delegate,class {Corlib.Name}System.Collections.Hashtable)");
		}

		public bool Manipulate(Hashtable reflectors, ILStatementElement statementElement)
		{
			string line = ILElement.StripLabel(statementElement.GetAllLines());
            string callvirtInstance = "callvirt   instance ";
            if (line.StartsWith(callvirtInstance))
                line = line.Substring(callvirtInstance.Length);
            else
                line = line.Substring(4).Trim();  // strip "call        "
			MethodInfo foundMethod = null;
			
			foreach (DictionaryEntry de in reflectors)
			{
				IListReflector reflector = de.Value as IListReflector;
				foreach(MethodInfo mi in reflector.GetMethods())
				{
                    string toCompare = null;
                    if (reflector.CallvirtNeedsClassPrefix)
                        toCompare = new ReflectedType(mi.ReturnType, ownAssemblyName).ILName + " " + new ReflectedType(mi.ReflectedType, ownAssemblyName).QuotedILName + "::" + mi.Name + "("; //RL 6-3-2008  QuotedIlName instead of ILName(Fix 'Relations with Umlaut')
                    else
                        toCompare = new ReflectedType(mi.ReturnType, ownAssemblyName).ILName + " " + new ReflectedType(mi.ReflectedType, ownAssemblyName).QuotedILNameWithoutPrefix + "::" + mi.Name + "(";  //RL 6-3-2008  QuotedIlNameWithoutPrefix instead of IlNameWithoutPrefix (Fix 'Relations mit Umlaut')

					if (line.StartsWith(toCompare))
					{
						foundMethod = mi;
						break;
					}
				}
			}
			if (foundMethod == null)
				return false;

            Type reflectedType = foundMethod.ReflectedType;
            string foundName = foundMethod.Name;

			string stackMethod = (string) functions[foundName];
			if (stackMethod == null)
				return false;
            // All list manipulation functions are void, with 3 Exceptions:
            // - IList.Add (non generic only)
            // - IList<T>.Remove (generic only)
            // - List<T>.RemoveAll (generic only)
            string retval = "void";

            if (foundName == "Add")
                retval = "int32";  // we use a return value anyway and pop it from the stack by generic containers
            else if (foundName == "Remove")
                retval = "bool";   // we use a return value anyway and pop it from the stack by non generic containers        
            else if (foundName == "RemoveAll")
                retval = "int32";
		
			statementElement.InsertBefore(new ILStatementElement("ldloc __ndocontainertable"));
			statementElement.InsertBefore(new ILStatementElement("call       " + retval + " [NDO]NDO._NDOContainerStack::" 
				+ stackMethod));

            // The non generic IList.Add function has an int32 result.
            // The generic IList<T>.Add function has a void result.
            // To give the result of the non generic function back, our Add function has also an int32 result.
            // If the call was to a generic version, we have to eliminate the result value with a pop instruction.
            if (foundName == "Add" && foundMethod.ReflectedType.IsGenericType)
                statementElement.InsertBefore(new ILStatementElement("pop"));
            if (foundName == "Remove" && !foundMethod.ReflectedType.IsGenericType)
                statementElement.InsertBefore(new ILStatementElement("pop"));

			statementElement.Remove();
			return true;
		}

	}
}
