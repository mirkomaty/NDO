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


using NDOInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using NDO.Mapping.Attributes;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a class mapping.
    /// </summary>
    /// <remarks>This class is equivalent to the Class element of the mapping file schema.</remarks>
    public class Class : MappingNode, IFieldInitializer, IComparable
    {
        #region State variables and accessors
        /// <summary>
        /// The Oid mapping of the class. See <see cref="NDO.Mapping.ClassOid"/>.
        /// </summary>
        [Browsable(false)]
        public ClassOid Oid
        {
            get { return oid; }
            set { oid = value; this.Changed = true; }
        }

        /// <summary>
        /// Factory method for constructing an ClassOid object.
        /// </summary>
        /// <returns>The ClasOid object.</returns>
        public ClassOid NewOid()
        {
            this.Oid = new ClassOid(this);
            return this.oid;
        }

        private ClassOid oid = null;
        /// <summary>
        /// All field mappings of the class. See <see cref="NDO.Mapping.Field"/>
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Field> Fields
        {
            get { return fields; }
        }
        private List<Field> fields = new List<Field>();
        /// <summary>
        /// All relation mappings of the class. See <see cref="NDO.Mapping.Relation"/>
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Relation> Relations
        {
            get { return relations; }
        }
        private List<Relation> relations = new List<Relation>();

        /// <summary>
        /// A list of <code>Class</code> objects for all subclasses of this class.
        /// This is for use of the NDO framework only. It will be initialized, if passed to an IPersistenceHandler.
        /// </summary>
        private List<Class> subclasses = new List<Class>();

		/// <summary>
		/// Gets the Subclasses of the current class
		/// </summary>
        [Browsable(false)]
        public IEnumerable<Class> Subclasses
        {
            get { return this.subclasses; }
        }

        /// <summary>
        /// Determines, whether the class has subclasses.
        /// </summary>
        [Browsable(false)]
        public bool HasSubclasses
        {
            get { return this.subclasses.Count > 0; }
        }

        /// <summary>
        /// Gets and sets the short Name of the assembly the class resides in.
        /// </summary>
        [ReadOnly(true), Description("Short Name of the assembly the class resides in.")]
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }
        private string assemblyName = "";
        /// <summary>
        /// Gets and sets the fully qualified name of the class.
        /// </summary>
        [ReadOnly(true), Description("Fully qualified name of the class.")]
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }
        private string fullName = "";
        /// <summary>
        /// Name of the table, the class is mapped to.
        /// </summary>
        [Description("Name of the table, the class is mapped to.")]
        public string TableName
        {
            get { return tableName; }
            set
            {
                tableName = value;
                this.Changed = true;
            }
        }
        private string tableName = "";
        /// <summary>
        /// Id of the connection to the data source the table resides in. See <see cref="NDO.Mapping.Connection"/>
        /// </summary>
        [Description("Id of the connection to the data source the mapped table resides in.")]
        public string ConnectionId
        {
            get { return connectionId; }
            set
            {
                connectionId = value;
                this.Changed = true;
            }
        }
        private string connectionId = "";

        /// <summary>
        /// Optional. Name of a column which holds a Guid - either as string or as native guid value. NDO will use this column for collision detection.
        /// </summary>
        [Description("NDO will use this column for collision detection.")]
        public string TimeStampColumn
        {
            get { return timeStampColumn; }
            set
            {
                timeStampColumn = value;
                this.Changed = true;
            }
        }
        private string timeStampColumn = null;

        /// <summary>
        /// For use by the NDO framework only. It will be initialized if the Class object is passed to the IPersistenceHandler.
        /// </summary>
        [Browsable(false)]
        public Type SystemType
        {
            get { return systemType; }
            set { systemType = value; }
        }
        private Type systemType;

        /// <summary>
        /// NDOMapping object, this Class object resides in
        /// </summary>
        [Browsable(false)]
        public NDOMapping Parent
        {
            get { return NodeParent as NDOMapping; }
        }

        /// <summary>
        /// Gets the NDO provider for the database the class is stored in.
        /// </summary>
        [Browsable(false)]
        public IProvider Provider
        {
            get { return this.Parent.GetProvider(this); }
        }
        /// <summary>
        ///  Used to set the parent if Class objects are moved to another mapping file.
        /// </summary>
        /// <param name="parent">An object of type NDOMapping.</param>
        internal void SetParent(NDOMapping parent)
        {
            this.NodeParent = parent;
        }

        /// <summary>
        /// For use by the NDO framework only. It will be initialized if the Class object is passed to the IPersistenceHandler.
        /// </summary>
        public bool IsAbstract = false;  // true for abstract classes and interfaces

        private string[]            myColumns;                   // wird in InitFields angelegt
        private IEnumerable<string> myEmbeddedTypes;            // wird in InitFields angelegt
        internal int                RelationOrdinalBase = -1;   // wird in InitFields angelegt
        internal IEnumerable<string>FKColumnNames;              // InitFields - collects all foreign key column names used in LoadState
		private bool?				hasEncryptedFields;

        /// <summary>
        /// Determines, if the class has encrypted fields.
        /// </summary>
		[Browsable(false)]
		
        public bool HasEncryptedFields
		{
			get 
			{
				if (!this.hasEncryptedFields.HasValue)
				{
					this.hasEncryptedFields = this.fields.Any( f => f.Encrypted );
				}

				return this.hasEncryptedFields.Value; 
			}
		}

        private Column typeNameColumn = null;

        /// <summary>
        /// Gets the TypeNameColumn, if one is present.
        /// </summary>
        [ReadOnly(true)]
        public Column TypeNameColumn
        {
            get { return typeNameColumn; }
        }

        private int typeCode;
        /// <summary>
        /// If the class is part of an inheritance chain, this property contains the unique type code of the class.
        /// </summary>
        /// <remarks>Note, that this information is part of the foreign keys and thus is stored in the database.</remarks>
        [ReadOnly(true)]
        public int TypeCode
        {
            get { return this.typeCode; }
            set { this.typeCode = value; }
        }

        #endregion

        #region FieldMap stuff
        /// <summary>
        /// For use by the NDO framework only. It will be initialized if the Class object is passed to the IPersistenceHandler.
        /// </summary>
        [Browsable(false)]
        public string[] ColumnNames
        {
            get { return myColumns; }
        }

        /// <summary>
        /// For use by the NDO framework only. The list be initialized if the Class object is passed to the IPersistenceHandler.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<string> EmbeddedTypes
        {
            get { return myEmbeddedTypes; }
        }

        /// <summary>
        /// Gets a dictionary, which contains all mappable fields. The key is the field name, the value is the MemberInfo structure.
        /// </summary>
        [Browsable(false)]
        public Dictionary<string,MemberInfo> PersistentFields
        {
            get { return new FieldMap(this, false).PersistentFields; }
        }

        /// <summary>
        /// Gets a list of FieldInfo entries, which reflects all relations of a class and its subclasses.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<FieldInfo> RelationInfos
        {
            get { return new FieldMap(this, false).Relations; }
        }
        #endregion

        #region Constructors and Save function

        /// <summary>
        /// Constructs a new Class object.
        /// </summary>
        public Class(NDOMapping parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Constructs a new Class object
        /// </summary>
        private Class()
            : base(null)
        {
        }


        /// <summary>
        /// Constructs a Class object according the information in the mapping file.
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="parent"></param>
        internal Class(XmlNode classNode, NDOMapping parent)
            : base(classNode, parent)
        {
            assemblyName = classNode.Attributes["AssemblyName"].Value;
            fullName = classNode.Attributes["FullName"].Value;
            tableName = classNode.Attributes["TableName"].Value;
            connectionId = classNode.Attributes["ConnectionId"].Value;
            this.typeCode = 0;  // undefined
            XmlAttribute attr = classNode.Attributes["TypeCode"];
            if (attr != null)
            {
                int.TryParse(attr.Value, out this.typeCode);
            }
            attr = classNode.Attributes["TimeStampColumn"];
            if (attr != null)
                this.timeStampColumn = attr.Value;

            XmlNodeList nl = classNode.SelectNodes(Parent.selectRelation, Parent.nsmgr);
            foreach (XmlNode relNode in nl)
            {
                Relation r = new Relation(relNode, this);
                relations.Add(r);
            }
            nl = classNode.SelectNodes(Parent.selectField, Parent.nsmgr);
            foreach (XmlNode fieldNode in nl)
            {
                fields.Add(new Field(fieldNode, this));
            }
            XmlNode oidNode = classNode.SelectSingleNode(Parent.selectOid, Parent.nsmgr);
            if (null != oidNode)
                this.oid = new ClassOid(oidNode, this);

            XmlNode typeColumnNode = classNode.SelectSingleNode(Parent.selectTypeNameColumn, Parent.nsmgr);
            if (null != typeColumnNode)
                this.typeNameColumn = new Column(typeColumnNode, this);
        }

        internal void Save(XmlNode parentNode)
        {
            XmlDocument doc = parentNode.OwnerDocument;
            XmlElement classNode = doc.CreateElement("Class");
            parentNode.AppendChild(classNode);
            base.SaveProperties(classNode);

            classNode.SetAttribute("AssemblyName", AssemblyName);
            classNode.SetAttribute("FullName", FullName);
            classNode.SetAttribute("TableName", TableName);
            classNode.SetAttribute("ConnectionId", ConnectionId);
            if (this.TimeStampColumn != null)
                classNode.SetAttribute("TimeStampColumn", this.TimeStampColumn);

            if (this.typeCode != 0)
                classNode.SetAttribute("TypeCode", this.typeCode.ToString());

            this.oid.Save(classNode);

            XmlElement newNode;

            if (this.typeNameColumn != null)
            {
                newNode = doc.CreateElement("TypeNameColumn");
                classNode.AppendChild(newNode);
                this.typeNameColumn.SaveOwnNode(newNode);
            }

            newNode = doc.CreateElement("Fields");
            classNode.AppendChild(newNode);

            this.fields.Sort();

            foreach (Field f in Fields)
                f.Save(newNode);

            newNode = doc.CreateElement("Relations");
            classNode.AppendChild(newNode);

            this.relations.Sort();

            foreach (Relation f in Relations)
                f.Save(newNode);
        }

        #endregion

        #region InitField stuff - initialisation by the persistence manager
        internal void ComputeRelationOrdinalBase()
        {
            this.RelationOrdinalBase = 0;
            Type baseType = SystemType.BaseType;
            Class cl;
            while ((cl = Parent.FindClass(baseType)) != null)
            {
                this.RelationOrdinalBase += cl.Relations.Count();
                baseType = cl.SystemType.BaseType;
            }
        }

		Type GetMemberType(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType == MemberTypes.Field)
				return ((FieldInfo) memberInfo).FieldType;
			if (memberInfo.MemberType == MemberTypes.Property)
				return ((PropertyInfo) memberInfo).PropertyType;
			throw new NDOException( 117, $"InitFields: MemberInfo of the persistent field {FullName}.{memberInfo.Name} should be a FieldInfo or a PropertyInfo");
		}

		void IFieldInitializer.InitFields()
        {
            this.systemType = Type.GetType(FullName + ", " + AssemblyName);
            if (this.systemType == null) throw new NDOException(22, "Can't load type: " + FullName + ", " + AssemblyName);
            IsAbstract = SystemType.IsAbstract;
            if (!IsAbstract)
                AddToSuperClass(this);
            FieldMap fieldMap = new FieldMap(this);
            myColumns = fieldMap.Fields;
            myEmbeddedTypes = fieldMap.EmbeddedTypes;
			var persistentFields = fieldMap.PersistentFields;
            ((IFieldInitializer)this.oid).InitFields();
			foreach (var field in this.fields)
			{
				var fieldName = field.Name;
				if (!persistentFields.ContainsKey( fieldName ))
					continue;  // The mapping file contains a needless field definition
				var type = GetMemberType( persistentFields[fieldName] );
				if (field.Encrypted && type != typeof( string ))
					throw new NDOException( 118, $"Field {fieldName} can't be encrypted. Encrypted fields must be of type string. Change the 'Encrypted' attribute in your mapping file." );
			}
        }

        internal void CollectForeignKeyNames()
        {
            if (this.oid.IsDependent)
            {
                string relationName = string.Empty;
                int j = 0;
                IEnumerator<ForeignKeyColumn> fkcEnumerator = null; ;
                new OidColumnIterator(this.oid).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
                {
                    Relation rel = null;
                    if (oidColumn.RelationName != relationName)
                    {
                        relationName = oidColumn.RelationName;
                        rel = oidColumn.Relation;
                        j = 0;
                        fkcEnumerator = rel.ForeignKeyColumns.GetEnumerator();
                        fkcEnumerator.MoveNext();  // index 0
                    }
                    oidColumn.SystemType = ((OidColumn)Parent.FindClass(rel.ReferencedType).Oid.OidColumns[j]).SystemType;
                    oidColumn.Size = this.Provider.GetDefaultLength(oidColumn.SystemType);
                    //TODO: This code needs desperately a review.
                    //The enumerator might be null at this point.
                    //It seems as if the condition oidColumn.RelationName != relationName always applies in the first iteration.
                    //Otherwise this code would give us a lot of trouble...
                    oidColumn.Name = fkcEnumerator.Current.Name;
                    j++;
                    fkcEnumerator.MoveNext();
                });


            }
            RelationCollector rc = new RelationCollector(this);
            rc.CollectRelations();
            this.FKColumnNames = rc.ForeignKeyColumns;
            if (this.FKColumnNames.Count() == 0)
                this.FKColumnNames = null;
        }


        private void AddSubClass(Class c)
        {
            if (!Subclasses.Contains(c))
            {
                this.subclasses.Add(c);
            }
            AddToSuperClass(c);
        }

        private void AddToSuperClass(Class c)
        {
            if (SystemType == null)
                systemType = Type.GetType(this.FullName + ", " + this.AssemblyName);
            Class baseClass = Parent.FindClass(SystemType.BaseType.FullName);
            if (baseClass != null)
            {
                baseClass.AddSubClass(c);
            }
            foreach (Type i in SystemType.GetInterfaces())
            {
                Class interfaceClass = Parent.FindClass(i.FullName);
                if (interfaceClass != null)
                {
                    interfaceClass.AddSubClass(c);
                }
            }
        }

        #endregion

        #region Find Functions

        /// <summary>
        /// Find a field mapping.
        /// </summary>
        /// <param name="fieldName">The name of the field or the accessor property for the field.</param>
        /// <returns>A <code>Field</code> object</returns>
        public Field FindField(string fieldName)
        {
            foreach (Field f in Fields)
            {
                if (f.Name == fieldName || f.AccessorName == fieldName)
                    return f;
            }
            return null;
        }

        /// <summary>
        /// Looks for a field, which is mapped to the given column.
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <returns>The field, or null, if no field is mapped to the given column.</returns>
        public Field FindColumn(string columnName)
        {
            // Column names are case insensitiv
            foreach (Field field in this.Fields)
            {
                if (string.Compare(field.Column.Name, columnName, true) == 0)
                    return field;
            }
            return null;
        }

        /// <summary>
        /// Search for a relation mapping, based on a field name
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>A <code>Relation</code> object</returns>
        public Relation FindRelation(string fieldName)
        {
            foreach (Relation r in Relations)
            {
                if (r.FieldName == fieldName || r.AccessorName == fieldName)
                    return r;
            }
            return null;
        }

        /// <summary>
        /// Gets the connection object
        /// </summary>
		[Browsable(false)]
		public Connection Connection
		{
			get
			{
				return Parent.FindConnection( this.connectionId );
			}
		}

        #endregion

        #region Enhancer Support
        /// <summary>
        /// Computes a column name which is unique in the given class mapping. 
        /// </summary>
        /// <param name="theFieldName">The field name, the column name is derived from.</param>
        /// <param name="isOidField">Specifies, if the the field is used as Oid for the given class.</param>
        /// <returns>A string representing a column name.</returns>
        public string ColumnNameFromFieldName(string theFieldName, bool isOidField)
        {
            if (theFieldName[0] == '<')
            {
                int q = theFieldName.IndexOf('>');
                if (q > -1)
                    return theFieldName.Substring(1, q - 1);
            }

            string[] strarr = theFieldName.Split('.');
            for (int i = 0; i < strarr.Length; i++)
            {
                if (strarr[i].StartsWith("_"))
                    strarr[i] = strarr[i].Substring(1);
                if (strarr[i].StartsWith("m_"))
                    strarr[i] = strarr[i].Substring(2);
                if (char.IsLower(strarr[i][0]))
                    strarr[i] = strarr[i].Substring(0, 1).ToUpper() + strarr[i].Substring(1);
            }
            string columnName = string.Empty;
            for (int i = 0; i < strarr.Length; i++)
            {
                if (i > 0)
                    columnName += "_";
                columnName = columnName + strarr[i];
            }

            string newColumnName = columnName;
            int index = 0;
            while (this.FindColumn(newColumnName) != null
                || !isOidField && Oid.FindColumn(newColumnName) != null)
            {
                index++;
                newColumnName = columnName + index.ToString();
            }
            return newColumnName;
        }

        /// <summary>
        /// This function is used by the enhancer. Adds a default field mapping.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="isOidField">Determines, if the field is used as Oid of the object.</param>
        /// <returns>A <code>Field</code> object</returns>
        public Field AddStandardField(string fieldName, bool isOidField)
        {
            this.Changed = true;
            Field f = new Field(this);
            f.Column.Name = ColumnNameFromFieldName(fieldName, isOidField);
            f.Name = fieldName;
            this.fields.Add(f);

            return f;
        }

        /// <summary>
        /// This function is used by the enhancer to add a column definition, to distinguish generic instance types.
        /// </summary>
        /// <returns></returns>
        public Column AddTypeNameColumn()
        {
            this.typeNameColumn = new Column(this);
            this.typeNameColumn.Name = "NDOTypeName";
            this.typeNameColumn.NetType = "System.String,mscorlib";
            return this.typeNameColumn;
        }
		
		/// <summary>
		/// This is for compatibility to enhancer versions &lt; 3.0.0.5
		/// </summary>
		/// <param name="fieldName">Name of the field</param>
		/// <param name="referencedTypeName">Type name of the referenced class</param>
		/// <param name="is1to1">True, if multiplicity is 1</param>
		/// <param name="relationName">Optional relation name</param>
		/// <param name="ownTypeIsPoly">True, if the class, containing the field, has a persistent base class</param>
		/// <param name="otherTypeIsPoly">True, if the related type has a persistent base class</param>
		/// <returns></returns>
		public Relation AddStandardRelation(string fieldName, string referencedTypeName, bool is1to1, string relationName, bool ownTypeIsPoly, bool otherTypeIsPoly)
		{
			return AddStandardRelation( fieldName, referencedTypeName, is1to1, relationName, ownTypeIsPoly, otherTypeIsPoly, null );
		}

        /// <summary>
        /// Adds a default relation mapping.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="referencedTypeName">Type name of the referenced class</param>
        /// <param name="isElement">True, if multiplicity is 1</param>
        /// <param name="relationName">Optional relation name</param>
        /// <param name="ownTypeIsPoly">True, if the class, containing the field, has a persistent base class</param>
        /// <param name="otherTypeIsPoly">True, if the related type has a persistent base class</param>
        /// <param name="mappingTableAttribute">If not null, the mapping information comes from this attribute.</param>
        /// <returns>A new constructed <code>Relation</code> object</returns>
        public Relation AddStandardRelation(string fieldName, string referencedTypeName, bool isElement, string relationName, bool ownTypeIsPoly, bool otherTypeIsPoly, MappingTableAttribute mappingTableAttribute)
        {
            //			if (null != Parent)
            //				Parent.this.Changed = true;

            Relation r = new Relation(this);
            r.FieldName = fieldName;
            r.ReferencedTypeName = referencedTypeName;
            //r.parent = this;
            r.RelationName = relationName;
            r.Multiplicity = isElement ? RelationMultiplicity.Element : RelationMultiplicity.List;

            int pos = referencedTypeName.LastIndexOf('.');
            string refShortName = referencedTypeName.Substring(pos + 1);
            refShortName = refShortName.Replace("`", string.Empty);

            pos = this.FullName.LastIndexOf('.');
            string myShortName = this.FullName.Substring(pos + 1);
            myShortName = myShortName.Replace("`", string.Empty);

            Relation foreignRelation = r.ForeignRelation;

            ForeignKeyColumn fkColumn = r.NewForeignKeyColumn();

            // Element->x AND no MappingTable
            if (isElement
				&& mappingTableAttribute == null
                && !(foreignRelation != null && foreignRelation.MappingTable != null))
            {
                r.MappingTable = null;

                // Foreign Key is in the own table and points to rows of the other table
                fkColumn.Name = "ID" + refShortName;

				if (otherTypeIsPoly)
                    r.ForeignKeyTypeColumnName = "TC" + refShortName;

                if (relationName != string.Empty)
                {
                    fkColumn.Name += "_" + relationName;
                    if (otherTypeIsPoly)
                        r.ForeignKeyTypeColumnName += "_" + relationName;
                }
            }
			else  // List or (Element with Mapping Table)
			{
				// These are the reasons for creating a mapping table:
				// 1. The MappingTableAttribute demands it
				// 2. We have a n:n relationship
				// 3. We have a 1:n relationship and the other type is poly
				// 4. The relation is bidirectional and the other side demands a mapping table

				bool needsMappingTable =
					mappingTableAttribute != null
					||
					null != foreignRelation && (foreignRelation.Multiplicity == RelationMultiplicity.List && r.Multiplicity == RelationMultiplicity.List)
					||
					otherTypeIsPoly && r.Multiplicity == RelationMultiplicity.List
					||
					null != foreignRelation && foreignRelation.MappingTable != null;
					
				
				// Foreign Key points to rows of our own table
				fkColumn.Name = "ID" + myShortName;

                if (ownTypeIsPoly && needsMappingTable)
                    r.ForeignKeyTypeColumnName = "TC" + myShortName;
				else if (otherTypeIsPoly && !needsMappingTable)
					r.ForeignKeyTypeColumnName = "TC" + refShortName;

				if (relationName != string.Empty)
                {
                    fkColumn.Name += "_" + relationName;
                    if (ownTypeIsPoly)
                        r.ForeignKeyTypeColumnName += "_" + relationName;
                }

                if (needsMappingTable)
                {
                    r.AddMappingTable(refShortName, myShortName, otherTypeIsPoly, mappingTableAttribute);
					r.RemapForeignMappingTable( myShortName, refShortName, ownTypeIsPoly, otherTypeIsPoly, mappingTableAttribute );
                }
                else r.MappingTable = null;
            }

            this.relations.Add(r);

            return r;
        }

        #endregion

        #region IComparable Member

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return this.FullName.CompareTo(((Class)obj).FullName);
        }

        #endregion

        /// <summary>
        /// This is used to optimize operations with type columns. We don't have to bother
        /// about type columns if the oid is a Guid.
        /// </summary>
        [Browsable(false)]
        internal bool HasGuidOid
        {
            get
            {
                if (this.oid.OidColumns.Count != 1)
                    return false;
                return ((OidColumn)this.oid.OidColumns[0]).SystemType == typeof(Guid);
            }
        }

        /// <summary>
        /// Removes a relation from the Class object.
        /// </summary>
        /// <param name="r"></param>
        public void RemoveRelation(Relation r)
        {
            this.relations.Remove(r);
			this.Changed = true;
        }

        /// <summary>
        /// Removes a relation from the Class object.
        /// </summary>
        /// <param name="r"></param>
        public void AddRelation(Relation r)
        {
			r.SetParent( this );
            this.relations.Add(r);
			this.Changed = true;
        }


        /// <summary>
        /// Removes a field from the Class object.
        /// </summary>
        /// <param name="f"></param>
        public void RemoveField(Field f)
        {
            this.fields.Remove(f);
			this.Changed = true;
        }

        /// <summary>
        /// String representation of the Class object
        /// </summary>
        /// <returns>FullName of the class</returns>
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// Removes the class mapping from the class mappings list in the parent object.
        /// </summary>
        public override void Remove()
        {
            Parent.RemoveClass(this);
        }

    }
}
