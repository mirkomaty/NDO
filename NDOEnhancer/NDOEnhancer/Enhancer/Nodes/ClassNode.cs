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
using System.Collections.Generic;
using System.Linq;
using NDO;
using NDO.Mapping;
using NDO.Mapping.Attributes;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für ClassNode.
	/// </summary>
	internal class ClassNode
	{
		string name = null;
		List<FieldNode> fields = new List<FieldNode>();
		List<RelationNode> relations = new List<RelationNode>();
		List<EmbeddedTypeNode> embeddedTypes = new List<EmbeddedTypeNode>();
		Class myClass = null;
		Type classType;
		NDOMapping mappings;

		public ClassNode(Type t, NDOMapping mappings)
		{
			this.classType = t;
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                throw new InternalException(45, "ClassNode");
			this.mappings = mappings;
			this.name = new ReflectedType(t).ILNameWithoutPrefix;
			this.isInterface = t.IsInterface;
			this.isPersistent = IsPersistentType(t);

			Type basetype = t.BaseType;
			this.baseFullName = new ReflectedType(basetype).ILNameWithoutPrefix;
			this.isAbstract = t.IsAbstract;
			AnalyzeFields();
            CheckOidAttributes();
        }

		private bool IsPersistentType(Type t)
		{
			return t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length > 0;
		}

        private void CheckOidAttributes()
		{
            List<OidColumnAttribute> collectedAttributes = new List<OidColumnAttribute>();

            object[] attributes = this.classType.GetCustomAttributes(typeof(OidColumnAttribute), true);
            if (attributes.Length > 0)
            {
                foreach (OidColumnAttribute ca in attributes)
                    collectedAttributes.Add(ca);
            }
        
			// If no attribute is assigned to the class, look for attributes assigned to the assembly
			if (collectedAttributes.Count == 0)
            {
                attributes = this.classType.Assembly.GetCustomAttributes(typeof(OidColumnAttribute), false);
				foreach (OidColumnAttribute ca in attributes)
				{
					ca.IsAssemblyWideDefinition = true;
					collectedAttributes.Add( ca );
				}
            }
            if (collectedAttributes.Count > 0)
            {
                this.columnAttributes = new OidColumnAttribute[collectedAttributes.Count];
                collectedAttributes.CopyTo(this.columnAttributes, 0);
            }
		}

		private void AnalyzeFields()
		{
			var relations = new List<RelationNode>();
			var fis = this.classType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(fi=>!fi.IsInitOnly);
			foreach (FieldInfo fi in fis)
			{
				string fname = fi.Name;
				if (fname.StartsWith("_ndo"))
					continue;

				object[] attributes = fi.GetCustomAttributes(false);
				bool cont = false;
				foreach (Attribute attr in attributes)
				{
					if (attr is NDOTransientAttribute)
					{
						cont = true;
						break;
					}
					if (attr is NDORelationAttribute)
					{
						this.relations.Add(new RelationNode(fi, (NDORelationAttribute)attr, this));
						cont = true;
						break;
					}
				}
				if (cont) continue;

				if (fi.FieldType.IsSubclassOf( typeof( System.Delegate ) ))
					continue;

				// Field type is persistent - assume relation with element multiplicity.
				if (fi.FieldType.GetInterface( "IPersistenceCapable" ) != null)
				{
					NDORelationAttribute nra = new NDORelationAttribute(fi.FieldType, RelationInfo.Default);
					this.relations.Add(new RelationNode(fi, nra, this));
					continue;
				}

				// Field is a collection - assume that it is either a relation or a transient field.
				if (GenericIListReflector.IsGenericIList(fi.FieldType))
				{
					if (fi.FieldType.GetGenericArguments()[0].GetInterface( "IPersistenceCapable" ) != null)
					{
						NDORelationAttribute nra = new NDORelationAttribute(fi.FieldType.GetGenericArguments()[0], RelationInfo.Default);
						this.relations.Add(new RelationNode(fi, nra, this));
					}
					else
					{
						Console.WriteLine("Warning: The field " + this.name + "." + fi.Name + " is a generic collection to a nonpersistent type. NDO assumes, that this field is transient.");
					}
					continue;
				}

				if (!fi.FieldType.IsGenericParameter && fi.FieldType.IsClass && fi.FieldType != typeof(string) && fi.FieldType != typeof(byte[]))
				{
					this.embeddedTypes.Add(new EmbeddedTypeNode(fi));
				}
				else
				{
					FieldNode fn = new FieldNode(fi);
					this.fields.Add(fn);
					Type ft = fi.FieldType;
					if (ft.IsValueType && !ft.IsEnum && !StorableTypes.Contains(ft))
					{
						ValueTypes.Instance.Add(new ValueTypeNode(ft));
					}
				}
			}

			// If there is more than one relation to the same target type
			// without relation name, assign a relation name automatically
			foreach(var group in this.relations.GroupBy( r => r.RelatedType ))
			{
				if (group.Count() < 2)
					continue;
				int countWithoutName = 0;
				foreach (var rel in group)
				{
					if (string.IsNullOrEmpty( rel.RelationName ))
					{
						if (countWithoutName > 0)
						{
							string relname = rel.Name;
							if (relname[0]=='<')
							{
								int q = relname.IndexOf( '>' );
								if (q == -1)
									q = relname.Length;
								relname = relname.Substring( 1, q - 1 );
							}
							rel.RelationName = relname;
						}
						countWithoutName++;
					}
				}
			}
		}




		private string GetTypeFullName(Type t)
		{
			NDOAssemblyName ndoAssName = new NDOAssemblyName(t.Assembly.FullName);
			string type = t.FullName.Replace("+", "/");
			return "[" + ndoAssName.Name + "]" + type;
		}

		public Class Class
		{
			get 
			{ 
				// Mapping information isn't available to construction time.
				// So we have to search the class information on demand.
				if (myClass == null)
				{
					myClass = mappings.FindClass(this.classType.FullName);
					if (myClass == null)
						throw new Exception("Can't find class mapping for class " + this.Name);
				}
				return myClass; 
			}
		}

		public IEnumerable<RelationNode> Relations
		{
			get
			{
				return this.relations;
			}
		}

		public IEnumerable<FieldNode> Fields
		{
			get 
			{ 
				return this.fields;
			}
		}

		public IEnumerable<FieldNode> EmbeddedTypes
		{
			get 
			{ 
				return this.embeddedTypes.Cast<FieldNode>();
			}
		}

		public string Name
		{
			get 
			{

				return this.classType.FullName;
			}
		}

		public string GetILName(string assemblyName)
		{
			return new ReflectedType(this.classType, assemblyName).ILName;
		}

        public Type ClassType
        {
            get { return this.classType; }
        }


        OidColumnAttribute[] columnAttributes;
        public OidColumnAttribute[] ColumnAttributes
        {
            get { return columnAttributes; }
        }


		public string BaseName
		{
			get 
			{
                Type t = this.classType.BaseType;
                if (t.IsGenericType)
                    t = t.GetGenericTypeDefinition();
				return t.FullName;
			}
		}

		string baseFullName;
        // As long as we don't need this, we leave it in the comment
        //public string BaseFullName 
        //{
        //    get 
        //    { 
        //        return this.baseFullName; 
        //    }
        //}

		public string AssemblyName
		{
			get 
			{
				NDOAssemblyName an = new NDOAssemblyName(this.classType.Assembly.FullName);
				return an.Name;
			}
		}

		public override bool Equals(object obj)
		{
			if (! (obj is ClassNode))
				return false;
			return ((ClassNode) obj).Name == this.Name;
		}

		public override int GetHashCode()
		{
			// Fits to the definition of Equals
			return this.Name.GetHashCode();
		}


		public bool IsAbstractOrInterface
		{
			get
			{
				return isAbstract | isInterface;
			}
		}
		
		bool isAbstract;
		public bool IsAbstract
		{
			get
			{
				return isAbstract;
			}
		}
		bool isInterface;
		public bool IsInterface
		{
			get
			{
				return isInterface;
			}
		}

		bool isPoly;
		public bool IsPoly
		{
			get { return isPoly; }
			set { isPoly = value; }
		}

		bool isPersistent;
		public bool IsPersistent
		{
			// If A inherits from B inherits from C and
			// A and C are [NDOPersistent] and B is not [NDOPersistent]
			// then B is in the nodelist and has the attribute IsPersistent with
			// the value "false".
			get { return isPersistent; }
		}

        public override string ToString()
        {
            return this.name;
        }

	}
}
