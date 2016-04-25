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
using System.Reflection;
using System.Collections;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für ListCloner.
	/// </summary>
	internal class ListCloner
	{
		public static object CloneList(object l)
		{
			Type t = l.GetType();
			try
			{
				if (l is ICloneable)
					return (IList)((ICloneable)l).Clone();				
//				return t.Assembly.CreateInstance(t.FullName, false, BindingFlags.CreateInstance, null, new object[]{l}, null, null);
				return Activator.CreateInstance(t, new object[] {l});
			}
			catch (Exception ex)
			{
				throw new NDOException(74, "Can't clone List of type " + t.FullName + ". " + ex.Message);
			}
		}

		public static object CreateList(Type t)
		{
#if !NDO11
			if (t.FullName.StartsWith("System.Collections.Generic.IList`1"))
			{
				return GenericListReflector.CreateList(t.GetGenericArguments()[0]);
			}
#endif
			if (t == typeof(IList))
				return new ArrayList();
			try
			{
				return Activator.CreateInstance(t);
			}
			catch (Exception ex)
			{
				throw new NDOException(74, "Can't create List of type " + t.FullName + ". " + ex.Message);
			}
		}


	}
}
