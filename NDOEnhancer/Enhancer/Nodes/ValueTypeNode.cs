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

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für ValueTypeNode.
	/// </summary>
	public class ValueTypeNode
	{
		string name;
		public string Name
		{
			get { return name; }
		}

		bool isValid = false;
		public bool IsValid
		{
			get { return isValid; }
		}

		ArrayList fields = new ArrayList();
		public ArrayList Fields
		{
			get { return fields; }
		}

		Type type;
		public Type Type
		{
			get { return type; }
		}

		public ValueTypeNode(Type t)
		{
			this.type = t;
			ArrayList publicFields = new ArrayList();
			PropertyInfo[] mis = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach(PropertyInfo mi in mis)
			{
				if (mi.CanRead && mi.CanWrite && new ReflectedType(mi.PropertyType).IsStorable)
				{
					publicFields.Add(mi);
				}
			}
			FieldInfo[] fis = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach(FieldInfo fi in fis)
			{
				if (new ReflectedType(fi.FieldType).IsStorable)
				{
					publicFields.Add(fi);
				}
			}
			if (publicFields.Count > 0)
			{
				this.isValid = true;
				this.name = t.FullName;
				foreach (MemberInfo mi in publicFields)
				{
					fields.Add(new FieldNode(mi));
				}
			}
		}

        public override string ToString()
        {
            return this.name;
        }
	}
}
