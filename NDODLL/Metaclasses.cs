//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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
using System.Collections;
using System.Reflection;

namespace NDO
{
	/// <summary>
	/// Summary description for Metaclasses.
	/// </summary>
	internal class Metaclasses
	{
		private static Hashtable theClasses = new Hashtable();

		internal static IMetaClass GetClass(Type t) 
		{
            if (t.IsGenericTypeDefinition)
                return null;

			IMetaClass mc;

			lock(theClasses)
			{				
				if (null == (mc = (IMetaClass) theClasses[t])) 
				{
					Type mcType = t.GetNestedType("MetaClass", BindingFlags.NonPublic | BindingFlags.Public);
					if (null == mcType) 
						throw new NDOException(13, "Missing nested class 'MetaClass' for type '" + t.Name + "'; the type doesn't seem to be enhanced.");
					Type t2 = mcType;
					if (t2.IsGenericTypeDefinition)
						t2 = t2.MakeGenericType(t.GetGenericArguments());
					mc = (IMetaClass) Activator.CreateInstance(t2);
					theClasses.Add(t, mc);
				}
			}

			return mc;
		}
	}
}
