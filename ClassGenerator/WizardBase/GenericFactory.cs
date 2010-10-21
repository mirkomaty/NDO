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
using System.Collections;
using System.Reflection;

namespace WizardBase
{
	/// <summary>
	/// Zusammenfassung für GenericFactory.
	/// </summary>
	public class GenericFactory
	{
		Hashtable typeHash = new Hashtable();
		Assembly assembly;
		public Assembly Assembly
		{
			get { return assembly; }
			set { assembly = value; }
		}

		public virtual string[] AvailableTypes
		{
			get 
			{ 
				string[] result = new string[typeHash.Count];
				int i = 0;
				foreach(DictionaryEntry de in typeHash)
					result[i++] = (string) de.Key;
				return result;	
			}
		}

		public bool Contains(string name)
		{
			foreach(string s in AvailableTypes)
			{
				if (s == name)
					return true;
			}
			return false;
		}

		protected virtual object Create(string name)
		{
			return Activator.CreateInstance((Type)typeHash[name]);
		}

		protected virtual object Create(string name, object[] parameters)
		{
			return Activator.CreateInstance((Type)typeHash[name], parameters);
		}

		public GenericFactory(Assembly ass, Type assignableType, string nameSpace)
		{
			this.assembly = ass;
			Type at = typeof(object);
			foreach (Type t in ass.GetTypes())
			{
				if (((nameSpace != null && t.FullName.StartsWith(nameSpace)) 
					|| nameSpace == null) 
					&& assignableType.IsAssignableFrom(t))
				{
					object[] attrs = t.GetCustomAttributes(typeof(DisplayNameAttribute), false);
					if (attrs.Length > 0)
					{
						DisplayNameAttribute dna = (DisplayNameAttribute) attrs[0];
						typeHash.Add(dna.Name, t);
					}
					else
						typeHash.Add(t.Name, t);
				}
			}
		}

		public GenericFactory(Assembly ass, Type assignableType) : this(ass, assignableType, null)
		{			
		}
	}
}
