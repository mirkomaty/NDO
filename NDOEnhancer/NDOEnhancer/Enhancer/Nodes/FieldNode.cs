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
using NDO;
using NDO.Mapping.Attributes;
using System.Collections.Generic;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für FieldNode.
	/// </summary>
	public class FieldNode
	{
		string dataType;
		string name;
		List<FieldNode> fields = new List<FieldNode>();
		bool isEnum;
        Type fieldType;
		string declaringType;
		bool isProperty;
		public bool IsProperty
		{
			get { return isProperty; }
		}

        ColumnAttribute columnAttribute;
        public ColumnAttribute ColumnAttribute => columnAttribute;

		FieldAttribute fieldAttribute;
		public FieldAttribute FieldAttribute => this.fieldAttribute; 

		public FieldNode(MemberInfo mi)
		{		
			this.isProperty = (mi is PropertyInfo);
			
			this.fieldType = isProperty ? ((PropertyInfo)mi).PropertyType : ((FieldInfo)mi).FieldType;
			this.name = mi.Name;

#warning	this.isOid should be set by the information provided by the class' attribute OidColumn(fieldName)

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

			attrs = mi.GetCustomAttributes( typeof(FieldAttribute), true );
			if (attrs.Length > 0)
				this.fieldAttribute = (FieldAttribute) attrs[0];
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

		public IEnumerable<FieldNode> Fields
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

		/// <summary>
		/// Add a subfield in case of embedded fields
		/// </summary>
		/// <param name="fieldNode"></param>
		public void AddField(FieldNode fieldNode)
		{
			this.fields.Add( fieldNode );
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
