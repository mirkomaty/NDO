using NDO.Mapping.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a relation between classes
    /// </summary>
    /// <remarks>This class is equivalent to the Relation element of the mapping file schema.</remarks>
    public class Relation : MappingNode, IFieldInitializer, IComparable
    {
        #region State variables and accessors
        /// <summary>
        /// Parent class of relation
        /// </summary>
        [Browsable(false)]
        public Class Parent
        {
            get { return nodeParent as Class; }
        }

        /// <summary>
        /// Removes the Relation object from the Relation object list of the parent object.
        /// </summary>
        public override void Remove()
        {
            Parent.RemoveRelation(this);
        }

        /// <summary>
        /// Field name of relation.
        /// </summary>
        [ReadOnly(true), Description("Field name of relation.")]
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; this.Changed = true; }
        }
        private string fieldName;

        /// <summary>
        /// Field type of relation.
        /// </summary>
        [Browsable(false)]
        public Type FieldType
        {
            get { return fieldType; }
            set { fieldType = value; }
        }
        private Type fieldType;

        /// <summary>
        /// Name of the referenced type.
        /// </summary>
        [ReadOnly(true), Description("Name of the referenced type.")]
        public string ReferencedTypeName
        {
            get { return referencedTypeName; }
            set { referencedTypeName = value; this.Changed = true; }
        }
        private string referencedTypeName;

        /// <summary>
        /// Type of referenced class. For 1:1 relations is the same type as the field type.
        /// This field is initialized by NDO while constructing the PersistenceManger.
        /// </summary>
        [Browsable(false)]
        public Type ReferencedType
        {
            get { return referencedType; }
            set { referencedType = value; }
        }
        private Type referencedType;

        List<ForeignKeyColumn> foreignKeyColumns = new List<ForeignKeyColumn>();
        /// <summary>
        /// The foreign key columns of the relation.
        /// </summary>
        /// <remarks>
        /// Under normal circumstances this collection contains one Column
        /// definition. But if the related class has a MultiColumn oid the
        /// relation needs more than one column to store the information needed to
        /// denote the related column. The order of the foreignKeyColums must match
        /// the order of the according Oid colums in the related class.
        /// </remarks>
        [Browsable(false)]
        public IEnumerable<ForeignKeyColumn> ForeignKeyColumns
        {
            get { return this.foreignKeyColumns; }
        }

        /// <summary>
        /// Adds a new ForeignKeyColumn to the relation.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Used by the enhancer and the mapping tool.
        /// </remarks>
        public ForeignKeyColumn NewForeignKeyColumn()
        {
            ForeignKeyColumn fkColumn = new ForeignKeyColumn(this);
            this.foreignKeyColumns.Add(fkColumn);
            return fkColumn;
        }

        /// <summary>
        /// Removes a foreign key column from the relation.
        /// </summary>
        /// <param name="fkc"></param>
        public void RemoveForeignKeyColumn(ForeignKeyColumn fkc)
        {
            this.foreignKeyColumns.Remove(fkc);
        }

        /// <summary>
        /// Name of the foreign key type column in data row. The type of this column is always "int".
        /// </summary>
        [Description("Name of the foreign key type column.")]
        public string ForeignKeyTypeColumnName
        {
            get { return foreignKeyTypeColumnName; }
            set { foreignKeyTypeColumnName = value; this.Changed = true; }
        }
        private string foreignKeyTypeColumnName;


#if nix
		/// <summary>
		/// Name of the foreign key column.
		/// </summary>
		/// <remarks>
		/// In 1:1 relations the column resides in datarows representing objects of the own class, pointing to rows, representing the related class. 
		/// In 1:n relations the column resides in datarows representing objects of the related class, pointing to rows, representing the own class.
		/// If a mapping table is used, the column resides in the mapping table, pointing to rows representing the own class.
		/// </remarks>
		[Description("Name of the foreign key column.")]
		public string ForeignKeyColumnName
		{
			get { return foreignKeyColumnName; }
			set { foreignKeyColumnName = value; this.Changed = true; }
		}
		private string foreignKeyColumnName;

#endif
        /// <summary>
        /// Optional name of relation. Used to distinguish between several relations to the same type.
        /// </summary>
        [Description("Used to distinguish between several relations to the same type.")]
        public string RelationName
        {
            get { return relationName; }
            set { relationName = value; this.Changed = true; }
        }
        private string relationName;

        /// <summary>
        /// Optional mapping table
        /// </summary>
        [Browsable(false)]
        public MappingTable MappingTable
        {
            get { return mappingTable; }
            set { mappingTable = value; this.Changed = true; }
        }
        private MappingTable mappingTable;

        /// <summary>
        /// Relation type: Assoziation == false, Composition == true
        /// This field is only initialized, if used by the NDO framework.
        /// </summary>
        [Browsable(false)]
        public bool Composition
        {
            get { return composition; }
            set { composition = value; }
        }
        private bool composition;

        /// <summary>
        /// True, if we have a polymorphic relation
        /// This field is only initialized, if used by the NDO framework.
        /// </summary>
        [Browsable(false)]
        public bool HasSubclasses
        {
            get { return hasSubclasses; }
            set { hasSubclasses = value; }
        }
        private bool hasSubclasses;


        /// <summary>
        /// Relation type: field or element.
        /// This field is only initialized, if used by the NDO framework.
        /// </summary>
        [Browsable(false)]
        public RelationMultiplicity Multiplicity
        {
            get { return multiplicity; }
            set { multiplicity = value; }
        }
        private RelationMultiplicity multiplicity;

        /// <summary>
        /// For accessing the relation load state in the objects
        /// </summary>
        internal int Ordinal;// If you ever change the name of this variable, change it in dotfuscator.xml too.

        private bool foreignRelationValid;
        private Relation foreignRelation;
        private Class definingClass;


        #endregion

        /// <summary>
        /// Returns a list of the target class and all subclasses of the target class of the relation. This field is initialized by the NDO Framework.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Class> ReferencedSubClasses
        {
            get
            {
                List<Class> result = new List<Class>();
                Class cl;
                result.Add(cl = this.RelatedClass);
                result.AddRange(cl.Subclasses);
                return result;
            }
        }

        /// <summary>
        /// Checks, if all foreign key mapping entries match the oid columns of the target types
        /// </summary>
        public void RemapForeignKeyColumns(ForeignKeyColumnAttribute[] fkAttributes, ChildForeignKeyColumnAttribute[] childFkAttributes)
        {
#if ShouldBeImplemented
            bool remap = false;
            Class cl = this.RelatedClass;
            if (this.mappingTable == null)
            {
                if (cl.Oid.OidColumns.Count != this.foreignKeyColumns.Count)
                {
                    remap = true;
                }
                else
                {
                    int i = 0;
                    new ForeignKeyIterator(this).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                    {

                    });


                }
            }
#endif
        }

        #region Constructors and Save function
        /// <summary>
        /// Constructs a new Relation object
        /// </summary>
        public Relation(Class cl)
            : base(cl)
        {
            // fields initialized with null are optional
            // fields initialized with "" are required
            fieldName = "";
            mappingTable = null;
            referencedTypeName = "";
            relationName = "";
        }

        internal Relation(XmlNode relNode, Class parent)
            : base(relNode, parent)
        {
            fieldName = relNode.Attributes["FieldName"].Value;
            referencedTypeName = relNode.Attributes["ReferencedTypeName"].Value;

            if (relNode.Attributes["ForeignKeyTypeColumnName"] != null && relNode.Attributes["ForeignKeyTypeColumnName"].Value != string.Empty)
                this.foreignKeyTypeColumnName = relNode.Attributes["ForeignKeyTypeColumnName"].Value;

            if (relNode.Attributes["ForeignKeyColumnName"] != null) // Old mapping
            {
                ForeignKeyColumn fkColumn = new ForeignKeyColumn(this);
                fkColumn.Name = relNode.Attributes["ForeignKeyColumnName"].Value;
                this.foreignKeyColumns.Add(fkColumn);
            }
            else
            {
                XmlNodeList nl = relNode.SelectNodes(parent.Parent.selectForeignKeyColumns);
                foreach (XmlNode fkcNode in nl)
                    this.foreignKeyColumns.Add(new ForeignKeyColumn(fkcNode, this));
            }

            XmlNode mtNode = relNode.SelectSingleNode(Parent.Parent.selectMappingTable, Parent.Parent.nsmgr);
            if (null != mtNode)
                mappingTable = new MappingTable(mtNode, this);
            if (null != relNode.Attributes["RelationName"])
                relationName = relNode.Attributes["RelationName"].Value;
            else
                relationName = string.Empty;
        }

        internal void Save(XmlNode parentNode)
        {
            XmlElement relNode = parentNode.OwnerDocument.CreateElement("Relation");
            parentNode.AppendChild(relNode);
            base.SaveProperties(relNode);
            relNode.SetAttribute("FieldName", fieldName);

            if (!string.IsNullOrEmpty(foreignKeyTypeColumnName))
                relNode.SetAttribute("ForeignKeyTypeColumnName", foreignKeyTypeColumnName);

#if nix
			relNode.SetAttribute("ForeignKeyColumnName", foreignKeyColumnName);
#endif
            foreach (ForeignKeyColumn fkColumn in this.foreignKeyColumns)
                fkColumn.Save(relNode);

            relNode.SetAttribute("ReferencedTypeName", referencedTypeName);
            relNode.SetAttribute("RelationName", relationName);

            if (null != mappingTable)
                mappingTable.Save(relNode);
        }

        #endregion

        /// <summary>
        /// Determines, if a relation is bidirectional
        /// </summary>
        public bool Bidirectional
        {
            get { return ForeignRelation != null; }
        }

        internal void GetOrdinal()
        {
            System.Diagnostics.Debug.Assert(this.Parent.RelationOrdinalBase > -1, "RelationOrdinalBase for type " + Parent.FullName + " not computed");
            Type t = Type.GetType(this.Parent.FullName + ", " + this.Parent.AssemblyName);
            IMetaClass mc = Metaclasses.GetClass(t);
            Ordinal = this.Parent.RelationOrdinalBase + mc.GetRelationOrdinal(this.FieldName);
            if (Ordinal > 63)
                throw new NDOException(18, "Class " + Parent.FullName + " has too much relations.");
        }

        Class RelatedClass
        {
            get
            {
                Class relatedClass = Parent.Parent.FindClass(this.ReferencedTypeName);
                if (relatedClass == null)
                    throw new NDOException(17, "Can't find mapping information for class " + this.ReferencedTypeName);
                return relatedClass;
            }
        }

        void IFieldInitializer.InitFields()
        {
            bool isEnhancing = ((IEnhancerSupport)Parent.Parent).IsEnhancing;

            if (!isEnhancing)
                GetOrdinal();

            Class relatedClass = this.RelatedClass;

            Type t = Parent.SystemType;

            if (t == null)
                throw new InternalException(1155, "Relation.InitFields");

            FieldInfo fi = null;

            while (fi == null && t != typeof(object))
            {
                fi = t.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi == null)
                    t = t.BaseType;
            }
            if (fi == null)
                throw new NDOException(20, "Can't find field " + Parent.SystemType.Name + "." + FieldName);

            FieldType = fi.FieldType;

            System.Attribute a = System.Attribute.GetCustomAttribute(fi, typeof(NDORelationAttribute), false);
            NDORelationAttribute ra = (NDORelationAttribute)a;

            this.composition = ra != null && (ra.Info & RelationInfo.Composite) != 0;

            if (fi.FieldType == typeof(System.Collections.IList) || fi.FieldType.GetInterface("IList") != null || fi.FieldType.FullName.StartsWith("System.Collections.Generic.IList`1"))
            {
                this.multiplicity = RelationMultiplicity.List;
            }
            else if (fi.FieldType.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length > 0)
            {
                this.multiplicity = RelationMultiplicity.Element;
            }
            else
            {
                throw new NDOException(111, "Invalid field type for relation " + t.FullName + "." + FieldName + ": Type = " + fi.FieldType.Name);
            }


#if PRO
            // This could be easier, if we hadn't the choice whether to use
            // polymorphy or not.
            bool cond1 = this.Multiplicity == RelationMultiplicity.Element
                && this.ForeignKeyTypeColumnName != null;
            bool cond2 = this.Multiplicity == RelationMultiplicity.List
                && this.MappingTable != null && this.MappingTable.ChildForeignKeyTypeColumnName != null;
            hasSubclasses = (relatedClass.HasSubclasses)
                && (cond1 || cond2);
#else
            hasSubclasses = false;
#endif


            if (this.multiplicity == RelationMultiplicity.List)
            {
                if (ra == null)
                    throw new Exception("Kann Relationstyp für Feld " + Parent.FullName + "." + fi.Name + " nicht bestimmen.");
#if !NDO11
                if (ra.RelationType == null && fi.FieldType.IsGenericType)
                    this.referencedType = fi.FieldType.GetGenericArguments()[0];
                else
#endif
                    this.referencedType = ra.RelationType;
                if (referencedType == null)
                    throw new NDOException(101, "Can't determine referenced type in relation " + this.Parent.FullName + "." + this.fieldName + ". Provide a type parameter for the [NDORelation] attribute.");
            }
            else
            {
                this.referencedType = FieldType;
            }

            if (HasSubclasses && Multiplicity == RelationMultiplicity.List && MappingTable == null)
            {
                //throw new NDOException(21, "Polymorphic 1:n-relation w/o mapping table is not supported");
                Debug.WriteLine("NDO Warning: Polymorphic 1:n-relation " + Parent.FullName + "." + this.FieldName + " w/o mapping table");
            }

            this.definingClass = this.Parent;
            Type bt = this.Parent.SystemType.BaseType;
            while (typeof(IPersistenceCapable).IsAssignableFrom(bt))
            {
                Class pcl = this.Parent.Parent.FindClass(bt);
                if (pcl.FindRelation(this.fieldName) != null)
                    this.definingClass = pcl;
                else
                    break;
                bt = bt.BaseType;
            }

            // Do not set fkColumn.Size to a value != 0 in during enhancing, 
            // because that value would be hard coded into the mapping file.
            // Use (!isEnhancing) to make sure, that the code wasn't called  from the enhancer.
            if (this.MappingTable != null)
            {
                // r.ForeignKeyColumns points to the own table.
                if (Parent.Oid.OidColumns.Count != this.foreignKeyColumns.Count)
                    throw new NDOException(115, "Column count between relation and Oid doesn't match. Type " + Parent.FullName + " has an oid column count of " + Parent.Oid.OidColumns.Count + ". The Relation " + Parent.FullName + "." + this.fieldName + " has a foreign key column count of " + this.foreignKeyColumns.Count + '.');
                int i = 0;
                new ForeignKeyIterator(this).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastIndex)
                {
                    OidColumn oidColumn = (OidColumn)Parent.Oid.OidColumns[i];
                    if (!isEnhancing && fkColumn.Size == 0)
                        fkColumn.Size = oidColumn.Size;
                    fkColumn.SystemType = oidColumn.SystemType;
                    i++;
                }
                );

                // r.MappingTable.ChildForeignKeyColumns points to the table of the related class.
                if (relatedClass.Oid.OidColumns.Count != this.mappingTable.ChildForeignKeyColumns.Count())
                    throw new NDOException(115, "Column count between relation and Oid doesn't match. Type " + relatedClass.FullName + " has an oid column count of " + relatedClass.Oid.OidColumns.Count + ". The Relation " + this.Parent.FullName + "." + this.fieldName + " has a foreign key column count of " + this.mappingTable.ChildForeignKeyColumns.Count() + '.');
                i = 0;
                new ForeignKeyIterator(this.mappingTable).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastIndex)
                {
                    OidColumn oidColumn = (OidColumn)relatedClass.Oid.OidColumns[i];
                    if (!isEnhancing && fkColumn.Size == 0)
                        fkColumn.Size = oidColumn.Size;
                    fkColumn.SystemType = oidColumn.SystemType;
                    i++;
                }
                );
            }
            else if (this.multiplicity == RelationMultiplicity.Element)  // The foreign key points to the tabel of the related class.
            {
                if (relatedClass.Oid.OidColumns.Count != this.foreignKeyColumns.Count)
                    throw new NDOException(115, "Column count between relation and Oid doesn't match. Type " + relatedClass.FullName + " has an oid column count of " + relatedClass.Oid.OidColumns.Count + ". The Relation " + Parent.FullName + "." + this.fieldName + " has a foreign key column count of " + this.foreignKeyColumns.Count + '.');
                int i = 0;
                new ForeignKeyIterator(this).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastIndex)
                {
                    OidColumn oidColumn = (OidColumn)relatedClass.Oid.OidColumns[i];
                    if (!isEnhancing && fkColumn.Size == 0)
                        fkColumn.Size = oidColumn.Size;
                    fkColumn.SystemType = oidColumn.SystemType;
                    i++;
                }
                );
            }
            else  // List relation. The foreign key points to the own table.
            {
                if (Parent.Oid.OidColumns.Count != this.foreignKeyColumns.Count)
                    throw new NDOException(115, "Column count between relation and Oid doesn't match. Type " + Parent.FullName + " has an oid column count of " + Parent.Oid.OidColumns.Count + ". The Relation " + Parent.FullName + "." + this.fieldName + " has a foreign key column count of " + this.foreignKeyColumns.Count + '.');
                int i = 0;
                new ForeignKeyIterator(this).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastIndex)
                {
                    OidColumn oidColumn = (OidColumn)Parent.Oid.OidColumns[i];
                    if (!isEnhancing && fkColumn.Size == 0)
                        fkColumn.Size = oidColumn.Size;
                    fkColumn.SystemType = oidColumn.SystemType;
                    i++;
                }
                );
            }
        }

        /// <summary>
        /// If a relation is bidirectional, this property gets the opposite relation
        /// </summary>
        [Browsable(false)]
        public Relation ForeignRelation
        {
            get
            {
                int status = 0;
                try
                {
                    if (definingClass == null)
                        definingClass = this.Parent;
                    status = 1;
                    if (!foreignRelationValid)   // null is a valid Value for foreignRelation
                    {
                        foreignRelation = null;
                        Class referencedClass = Parent.Parent.FindClass(this.referencedTypeName);
                        status = 2;
                        if (null == referencedClass)
                        {
                            foreignRelation = null;
                        }
                        else
                        {
                            status = 3;
                            // first check for a relation directing to our class
                            foreach (Relation fr in referencedClass.Relations)
                            {
                                string frdefiningClass = fr.definingClass == null ? fr.Parent.FullName : fr.definingClass.FullName;
                                if (null != fr.referencedTypeName
                                    && fr.referencedTypeName == definingClass.FullName
                                    && fr.relationName == this.relationName
                                    && frdefiningClass == this.referencedTypeName)
                                {
                                    // Bei der Selbstbeziehung muss der FieldName unterschiedlich sein
                                    // sonst kommt die gleiche Seite der Beziehung zurück.
                                    if (referencedClass != definingClass || fr.FieldName != this.FieldName)
                                    {
                                        foreignRelation = fr;
                                        break;
                                    }
                                }
                            }
                            status = 4;
                            // now check, if a relation targets our base class
                            if (foreignRelation == null && definingClass != nodeParent)
                            {
                                foreach (Relation fr in referencedClass.Relations)
                                {
                                    if (null != fr.referencedTypeName
                                        && fr.referencedTypeName == definingClass.FullName
                                        && fr.relationName == this.relationName)
                                    {
                                        // Bei der Selbstbeziehung muss der FieldName unterschiedlich sein
                                        // sonst kommt die gleiche Seite der Beziehung zurück.
                                        if (referencedClass != definingClass || fr.FieldName != this.FieldName)
                                        {
                                            foreignRelation = fr;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        status = 5;
                        foreignRelationValid = true;
                    }
                    return foreignRelation;
                }
                catch (Exception ex)
                {
                    throw new InternalException(1379, "Relation.ForeignRelation:" + ex.Message + " Status: " + status.ToString());
                }
            }
        }

        /// <summary>
        /// String representation of the relation for debugging and tracing purposes
        /// </summary>
        /// <returns>A string representation of the Relation object</returns>
        public override string ToString()
        {
            return "Relation " + this.RelationName + " for field " + this.FieldName + " of class " + Parent.FullName + ":\n" +
                "    Type: " + FieldType + " [" + Multiplicity + "] RelationType: " + (Composition ? "Composition" : "Assoziation") +
                ", " + (Bidirectional ? "Bidirectional" : "Directed to class " + ReferencedType);
        }

        int hashCode = 0;
        public override int GetHashCode()
        {
            // This is a hack, because data binding to a property grid
            // asks for the hash code. Since the binding occurs in the mapping tool
            // with uninitialized definingClass and SystemType members
            // we just return the hash code of System.Object.
            if (definingClass == null || definingClass.SystemType == null)
                return base.GetHashCode();

            if (this.hashCode == 0)
            {
                int v1 = definingClass.SystemType.GetHashCode();
                int v2 = this.referencedType.GetHashCode();
                hashCode = (v1 ^ v2) ^ this.relationName.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (definingClass == null)
                return base.Equals(obj);

            Relation r = obj as Relation;
            if ((object)r == null)
                return false;
            if (r.GetHashCode() == this.GetHashCode()
                && r.relationName == this.relationName)
            {
                if (r.definingClass == this.definingClass
                    && r.referencedType == this.referencedType)
                    return true;
                if (this.Bidirectional && r.Bidirectional)
                {
                    if (this.ForeignRelation.definingClass == r.definingClass
                        && this.ForeignRelation.referencedType == r.referencedType)
                        return true;
                    if (r.ForeignRelation.definingClass == this.definingClass
                        && r.ForeignRelation.referencedType == this.referencedType)
                        return true;
                }
            }
            return false;
        }


        #region IComparable Member

        public int CompareTo(object obj)
        {
            return this.FieldName.CompareTo(((Relation)obj).FieldName);
        }

        #endregion
    }
}
