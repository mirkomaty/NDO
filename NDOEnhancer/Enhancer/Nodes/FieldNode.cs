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
using NDO;
using NDO.Mapping.Attributes;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für FieldNode.
	/// </summary>
	internal class FieldNode
	{
		string dataType;
		string name;
		IList fields = new ArrayList();
		bool isEnum;
        Type fieldType;
		string declaringType;
		bool isProperty;
		public bool IsProperty
		{
			get { return isProperty; }
		}

        ColumnAttribute columnAttribute;
        public ColumnAttribute ColumnAttribute
        {
            get { return columnAttribute; }
        }

		public FieldNode(MemberInfo mi)
		{		
			this.isProperty = (mi is PropertyInfo);
			
			this.fieldType = isProperty ? ((PropertyInfo)mi).PropertyType : ((FieldInfo)mi).FieldType;
			this.name = mi.Name;
			if (!isProperty)
				this.isOid = mi.GetCustomAttributes(typeof(NDOObjectIdAttribute), false).Length > 0;
			this.dataType = new ReflectedType(this.fieldType).ILName;
			if (!isProperty)
			{
				if (mi.DeclaringType != mi.ReflectedType)
					this.declaringType = new ReflectedType(mi.DeclaringType).ILName;
			}
            this.isEnum = this.fieldType.IsEnum;

            object[] attrs = mi.GetCustomAttributes(typeof(ColumnAttribute), true);
            if (attrs.Length > 0) // since ColumnAttribute.AllowMultiple == false, we can only have max 1 attr.
                this.columnAttribute = (ColumnAttribute)attrs[0];
		}

        public Type FieldType
        {
        	get 
            {
                return fieldType; 
            }
        }

		public bool IsEnum
		{
			get { return isEnum; }
		}

		bool isOid;
		public bool IsOid
		{
			get { return isOid; }
		}

		public IList Fields
		{
			get { return fields; }
		}

		public string DataType
		{
			get { return dataType; }
		}

		public string Name
		{
			get { return name; }
		}

		public Type OidType
		{
			get
			{
				if (this.fieldType == typeof(int) 
					|| this.fieldType == typeof(string) 
					|| this.fieldType == typeof(Guid) 
					|| this.fieldType == typeof(long))
					return this.fieldType;
				return null;
			}
		}
        public override string ToString()
        {
            return this.name;
        }
	}
}
