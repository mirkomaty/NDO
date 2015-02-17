using NDO.Mapping.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a oid mapping
    /// </summary>
    /// <remarks>This class is equivalent to the Oid element of the mapping file schema.</remarks>
    public class ClassOid : MappingNode, IFieldInitializer
    {
        List<OidColumn> oidColumns = new List<OidColumn>();
        /// <summary>
        /// The column descriptions for the oid
        /// </summary>
        [Browsable(false)]
        public IEnumerable<OidColumn> OidColumns
        {
            get { return oidColumns; }
        }

        /// <summary>
        /// Creates a new OidColumn and adds it to the OidColumns list.
        /// </summary>
        /// <returns>The created OidColumn object.</returns>
        public OidColumn NewOidColumn()
        {
            OidColumn column = new OidColumn(this);
            this.oidColumns.Add(column);
            return column;
        }

        bool isInitialized;

        /// <summary>
        /// Checks, if the OidColumns collection matches the columnAttributes for a given class. 
        /// If the OidColums collection doesn't match, it will be completely rebuilt.
        /// </summary>
        /// <param name="columnAttributes">An array of OidColumnAttribute objects or null, if no attribute was set. If columnAttributes is null, the mappings are left as they are except there is no OidColumn defined. If no OidColumn is defined, a standard OidColumn will be generated. The standard OidColumn is an autoincremented integer column with the name "ID".</param>
        public void RemapOidColumns(OidColumnAttribute[] columnAttributes)
        {
            if (columnAttributes == null)  // No attrs defined
                return;

            bool remap = false;
            if (columnAttributes.Length == this.oidColumns.Count)
            {
                for (int i = 0; i < columnAttributes.Length; i++)
                {
                    OidColumn oidColumn = (OidColumn)this.oidColumns[i];
                    if (!columnAttributes[i].MatchesOidColumn(oidColumn))
                    {
                        remap = true;
                        break;
                    }
                }
            }
            else
            {
                remap = true;
            }

            if (!remap)
                return;

            // We assume, that we can use the same name, if a remapping of a single column is necessary.
            if (columnAttributes.Length == 1 && oidColumns.Count == 1 && string.IsNullOrEmpty(columnAttributes[0].Name))
                columnAttributes[0].Name = ((OidColumn)oidColumns[0]).Name;

            this.oidColumns.Clear();
            for (int i = 0; i < columnAttributes.Length; i++)
                this.oidColumns.Add(columnAttributes[i].CreateOidColumn(this));

        }

        /// <summary>
        /// Looks for a column with a given name.
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <returns>The column, or null, if no column with the given name exists.</returns>
        public Column FindColumn(string columnName)
        {
            // Column names are case insensitiv
            foreach (Column c in this.oidColumns)
            {
                if (string.Compare(c.Name, columnName, true) == 0)
                    return c;
            }
            return null;
        }


        public Class Parent
        {
            get { return base.nodeParent as Class; }
        }


        /// <summary>
        /// Removes the Oid mapping from the parent object. Note, that a class mapping 
        /// without Oid mapping will cause exceptions, if used by NDO.
        /// </summary>
        public override void Remove()
        {
            Parent.Oid = null;
        }

        /// <summary>
        /// Constructs a new Oid
        /// </summary>
        public ClassOid(Class parent)
            : base(parent)
        {
        }


        internal ClassOid(XmlNode oidNode, Class parent)
            : base(oidNode, parent)
        {
            if (oidNode.Attributes["ColumnName"] == null)  // new mapping
            {
                XmlNodeList nl = oidNode.SelectNodes(parent.Parent.selectOidColumns);
                foreach (XmlNode columnNode in nl)
                {
                    this.oidColumns.Add(new OidColumn(columnNode, this));
                }
            }
            else
            {
                OidColumn oidColumn = new OidColumn(this);
                this.oidColumns.Add(oidColumn);
                oidColumn.Name = oidNode.Attributes["ColumnName"].Value;
                if (null != oidNode.Attributes["FieldName"])
                    oidColumn.FieldName = oidNode.Attributes["FieldName"].Value;
                if (null != oidNode.Attributes["ChildRelation"])
                    oidColumn.RelationName = oidNode.Attributes["ChildRelation"].Value;
                if (null != oidNode.Attributes["ParentRelation"])
                {
                    OidColumn secondOid = new OidColumn(this);
                    secondOid.RelationName = oidNode.Attributes["ParentRelation"].Value;
                    secondOid.Name = secondOid.RelationName;
                    this.oidColumns.Add(secondOid);
                }
            }
        }

        internal void Save(XmlNode parentNode)
        {
            XmlElement oidNode = parentNode.OwnerDocument.CreateElement("Oid");
            parentNode.AppendChild(oidNode);
            base.SaveProperties(oidNode);             // Save properties
            foreach (OidColumn oidColumn in this.oidColumns)
            {
                oidColumn.Save(oidNode);
            }
        }

        /// <summary>
        /// Gets or sets a value, which determines, if the oid consists of one or
        /// more foreign keys.
        /// </summary>
        [Browsable(false)]
        public bool IsDependent
        {
            // Either all or no columns can have a RelationName
            get { return ((OidColumn)this.oidColumns[0]).RelationName != null; }
        }

        /// <summary>
        /// If the Oid is a dependent oid this property returns the list of the relations, the 
        /// class is dependent from.
        /// </summary>
        [Browsable(false)]
        public IList<Relation> Relations
        {
            get
            {
                List<Relation> result = new List<Relation>();
                if (!IsDependent) return result;
                string relationName = string.Empty;
                foreach (OidColumn oidc in this.oidColumns)
                {
                    if (relationName != oidc.RelationName)
                    {
                        relationName = oidc.RelationName;
                        Relation r = Parent.FindRelation(relationName);
                        System.Diagnostics.Debug.Assert(r != null);
                        result.Add(r);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets a value, which determines, if the Oid has an autoincremented column.
        /// </summary>
        [Browsable(false)]
        public bool HasAutoincrementedColumn
        {
            get
            {
                foreach (OidColumn oidColumn in this.OidColumns)
                {
                    if (oidColumn.AutoIncremented)
                        return true;
                }
                return false;
            }
        }

        Type oidType = null;

        [Browsable(false)]
        private Type OidTypeHint
        {
            get
            {
                if (oidType != null)
                    return oidType;
#pragma warning disable 618
                object[] attrs = Parent.SystemType.GetCustomAttributes(typeof(NDOOidTypeAttribute), true);
                if (attrs.Length == 0)
                    return null;
                return oidType = ((NDOOidTypeAttribute)attrs[0]).OidType;
#pragma warning restore 618
            }
        }


        void IFieldInitializer.InitFields()
        {
            if (this.isInitialized)
                return; // Avoid redundant calls, since InitFields is called in a recursive algorithm

            this.isInitialized = true;

            Type oidTypeHint = this.OidTypeHint;
            FieldMap fieldMap = new FieldMap(Parent);
            Dictionary<string,MemberInfo> myPersistentFields = fieldMap.PersistentFields;
            List<string> relationsReady = new List<string>();

            bool isDependent = ((OidColumn)oidColumns[0]).RelationName != null;
            foreach (OidColumn column in this.oidColumns)
            {
                if (isDependent && column.RelationName == null
                    || !isDependent && column.RelationName != null)
                    throw new NDOException(113, "Wrong Oid mapping for type " + Parent.FullName + ". You can't mix OidColumns with and without a RelationName.");
            }

            foreach (OidColumn column in this.oidColumns)
            {
                if (column.FieldName != null)
                {
                    FieldInfo fi = (FieldInfo)myPersistentFields[column.FieldName];
                    if (fi == null)
                        throw new NDOException(20, "Can't find field " + Parent.FullName + "." + column.FieldName + '.');
                    column.SystemType = fi.FieldType;
                    Field fieldMapping = Parent.FindField(fi.Name);
                    if (fieldMapping == null)
                        throw new NDOException(7, "Can't find mapping information for field " + Parent.FullName + "." + fi.Name);

                    // This information will be deleted by the enhancer after generating the schema
                    // to avoid redundant information in the mapping file.
                    column.Name = fieldMapping.Column.Name;
                }
                else if (column.RelationName != null)
                {
                    // Find and check the relation
                    Relation r = Parent.FindRelation(column.RelationName);
                    if (r == null || r.Multiplicity != RelationMultiplicity.Element || r.Composition)
                        throw new NDOException(109, "Wrong relation name " + column.RelationName + " in OidColumn definition of type " + Parent.FullName + '.' + " The RelationName must denote an existing assoziation with RelationMultiplicity.Element.");

                    bool found = false;
                    foreach (string s in relationsReady)
                    {
                        if (column.RelationName == s)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        relationsReady.Add(column.RelationName);
                        // find all oid columns with the same RelationName
                        IEnumerable<OidColumn> allOidColumns = this.oidColumns.Where(oidc => oidc.RelationName == column.RelationName);

                        // find all FkColumns of the relation
                        ArrayList fkColumns = new ArrayList();
                        foreach (ForeignKeyColumn fkc in r.ForeignKeyColumns)
                        {
                            fkColumns.Add(fkc);
                        }

                        // Both lists must have the same count
                        if (allOidColumns.Count != fkColumns.Count)
                            throw new NDOException(110, "Count of Oid columns with relation name " + column.RelationName + " is different from count of foreign key columns of the named relation. Expected: " + fkColumns.Count + ". Count of OidColumns was: " + allOidColumns.Count + '.' + " Type = " + Parent.FullName + '.');

                        // Now let's determine the oid types of the columns
                        Class relClass = Parent.Parent.FindClass(r.ReferencedTypeName);

                        // Resolve the Oid types of the related type. Endless recursion is not possible because of the test at the beginning of this method. 
                        if (allOidColumns.Count != relClass.Oid.OidColumns.Count)
                            throw new NDOException(110, "Count of Oid columns with relation name " + column.RelationName + " is different from count of oid columns of the related type. Expected: " + fkColumns.Count + ". Count of OidColumns was: " + allOidColumns.Count + '.' + " Type = " + Parent.FullName + '.');
                        ((IFieldInitializer)relClass).InitFields();  // Must initialize the system type and oid of the related class first.
                        for (int i = 0; i < relClass.Oid.OidColumns.Count; i++)
                        {
                            OidColumn relOidColumn = (OidColumn)relClass.Oid.OidColumns[i];
                            // The column has the same type as the oid column of the related type
                            ((OidColumn)allOidColumns[i]).SystemType = relOidColumn.SystemType;
                            // The column has the same name as the foreign key column of the relation defined 
                            // by RelationName.
                            // The enhancer will remove the name assigned here after generating the schema.
                            ((OidColumn)allOidColumns[i]).Name = ((Column)fkColumns[i]).Name;
                        }
                    }
                }
                else if (column.NetType != null)
                {
                    column.SystemType = Type.GetType(column.NetType);
                    if (column.SystemType == null)
                        throw new NDOException(11, "Type.GetType for the type name " + column.NetType + " failed; check your mapping File.");
                }
                else if (oidTypeHint != null && this.oidColumns.Count == 1)
                {
                    column.SystemType = oidTypeHint;
                }
                else  // Default case if nothing is defined in the mapping or in NDOOidType
                {
                    column.SystemType = typeof(Int32);
                    if (this.oidColumns.Count == 1)
                        column.AutoIncremented = true;
                }
            }

            if (!((IEnhancerSupport)Parent.Parent).IsEnhancing)
            {
                int autoIncCount = 0;
                foreach (OidColumn column in this.oidColumns)
                {
                    // The size is only set for internal use by NDO. 
                    // Don't save this information in the mapping file.
                    if (column.Size == 0)
                        column.Size = Parent.Parent.GetProvider(Parent).GetDefaultLength(column.SystemType);
                    if (column.AutoIncremented)
                        autoIncCount++;
                }
                if (autoIncCount > 1)
                    throw new NDOException(112, "Type " + Parent.FullName + " has more than one autoincremented column. This is not supported by NDO. Check your mapping file or the OidColumn attributes in the source code.");
            }
        }
    }
}
