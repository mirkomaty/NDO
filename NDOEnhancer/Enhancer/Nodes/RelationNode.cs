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
	/// Zusammenfassung für RelationNode.
	/// </summary>
	internal class RelationNode
	{
        Type fieldType;
        public Type FieldType
        {
        	get { return fieldType; }
        }


		ClassNode parent;
		public ClassNode Parent
		{
			get { return parent; }
			set { parent = value; }
		}

        ForeignKeyColumnAttribute[] foreignKeyColumnAttributes;
        public ForeignKeyColumnAttribute[] ForeignKeyColumnAttributes
        {
            get { return foreignKeyColumnAttributes; }
        }

        ChildForeignKeyColumnAttribute[] childForeignKeyColumnAttributes;
        public ChildForeignKeyColumnAttribute[] ChildForeignKeyColumnAttributes
        {
            get { return childForeignKeyColumnAttributes; }
        }


		public RelationNode(FieldInfo fi, NDORelationAttribute attr, ClassNode parent)
		{
			this.parent = parent;
			this.fieldType = fi.FieldType;
			Type parentType = fi.ReflectedType;
			NDOAssemblyName an = new NDOAssemblyName(parentType.Assembly.FullName);
			string assShortName = an.Name;
#if NDO11
			this.isElement = !(fi.FieldType == typeof(IList) || fi.FieldType.GetInterface("IList") != null);
#else
			this.isElement = !(fi.FieldType == typeof(IList) 
				|| fi.FieldType.GetInterface("IList") != null 
				|| GenericIListReflector.IsGenericIList(fi.FieldType));
#endif
			// dataType & declaringType haben
			// - immer ein class/valutype prefix
			// - immer ein [AssName] prefix
			this.dataType = new ReflectedType(fi.FieldType).ILName;
			if (fi.DeclaringType != fi.ReflectedType)
				this.declaringType = new ReflectedType(fi.DeclaringType).ILName;
			else
				this.declaringType = null;
			this.name = fi.Name;

			Type attrType = attr.GetType();
			this.relationName = attr.RelationName;
			this.relationInfo = attr.Info;
			Type relType;
            if (isElement)
            {
                relType = fi.FieldType;
            }
            else
            {
#if !NET11
                if (attr.RelationType == null && fi.FieldType.IsGenericType)
                    relType = fi.FieldType.GetGenericArguments()[0];
                else
                    relType = attr.RelationType;
#else
                relType = attr.RelationType;
#endif
                if (relType == null)
                    throw new Exception("Can't determine referenced type in relation " + parent.Name + "." + this.name + ". Provide a type parameter for the [NDORelation] attribute.");
                                    
            }

            if (relType.IsGenericType && !relType.IsGenericTypeDefinition)
                relType = relType.GetGenericTypeDefinition();

            // Related Type hat kein "class " Prefix, hat [assName] nur bei fremden Assemblies.
			this.relatedType = new ReflectedType(relType, assShortName).ILNameWithoutPrefix;
            if (relType.IsGenericType)
                this.relatedType = this.relatedType.Substring(0, this.relatedType.IndexOf('<'));

            object[] attrs = fi.GetCustomAttributes(typeof(ForeignKeyColumnAttribute), true);
            if (attrs.Length > 0)
            {
                this.foreignKeyColumnAttributes = new ForeignKeyColumnAttribute[attrs.Length];
                attrs.CopyTo(this.foreignKeyColumnAttributes, 0);
            }
            attrs = fi.GetCustomAttributes(typeof(ChildForeignKeyColumnAttribute), true);
            if (attrs.Length > 0)
            {
                this.childForeignKeyColumnAttributes = new ChildForeignKeyColumnAttribute[attrs.Length];
                attrs.CopyTo(this.childForeignKeyColumnAttributes, 0);
            }
        }

		string relationName;
		public string RelationName
		{
			get { return relationName; }
		}

		string name;
		public string Name
		{
			get { return name; }
		}

		RelationInfo relationInfo;
		public RelationInfo RelationInfo
		{
			get { return relationInfo; }
		}

		string declaringType;
		public string DeclaringType
		{
			get { return declaringType; }
		}

		bool isElement;
		public bool IsElement
		{
			get { return isElement; }
		}

		string relatedType;
		public string RelatedType
		{
			get { return relatedType; }
		}

		string dataType;
		public string DataType
		{
			get { return dataType; }
		}

        public override string ToString()
        {
            return this.name;
        }
	}
}
