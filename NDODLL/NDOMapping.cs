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
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using System.IO;
using System.Reflection;
using NDOInterfaces;
using NDO.Mapping.Attributes;


namespace NDO.Mapping
{
    public abstract class MappingNode
    {
        protected MappingNode nodeParent;
        Hashtable properties = new Hashtable();

        /// <summary>
        /// Gets a collection of DictionaryEntry elements, representing the 
        /// properties of the mapping tree element.
        /// </summary>
        /// <remarks>
        /// dictionaryEntry.Key denotes the name of a property,
        /// dictionaryEntry.Value is a NDO.Mapping.Property object.
        /// </remarks>
        [Browsable(false)]
        public IEnumerable Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Adds a property to the properties collection.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">A string representaion of the type of the property value.</param>
        /// <param name="value">The value object of the property.</param>
        /// <remarks>
        /// The value might be of any type which is eiter an IConvertible or has a 
        /// TypeConverter, which is specified in the TypeConverterAttribute.
        /// Use the NDOProperty class to retrieve the binary representation of the
        /// property value in the given type.
        /// If a property with the same name already exists, this property will be 
        /// removed before inserting the new property.
        /// </remarks>
        public void AddProperty(string name, string type, string value)
        {
            Property prop = new Property(this, name, type, value);
            AddProperty(prop);
        }

        /// <summary>
        /// Adds a property to the properties collection.
        /// </summary>
        /// <param name="prop">The property object to add.</param>
        /// <remarks>
        /// If a property with the same name already exists, this property will be 
        /// removed before inserting the new property.
        /// </remarks>
        public void AddProperty(Property prop)
        {
            prop.SetParent(this);
            string name = prop.Name;
            properties.Remove(name); // Make sure, there is no element with the same key.
            properties.Add(name, prop);
            this.Changed = true;
        }

        /// <summary>
        /// Removes a property from the properties collection.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <remarks>
        /// If the property doesn't exist, the properties collection remains unchanged.
        /// </remarks>
        public void RemoveProperty(string name)
        {
            properties.Remove(name);
            this.Changed = true;
        }

        /// <summary>
        /// Gets or sets a property with the given name.
        /// </summary>
        /// <param name="name">The name of the property to retrieve or to set.</param>
        /// <returns>The property value. If the property doesn't exist, null is returned.</returns>
        /// <remarks>
        /// If an existing property is set, the value of the property will be changed.
        /// </remarks>
        [Browsable(false)]
        public Property this[string name]
        {
            get
            {
                return (Property)properties[name];
            }
            set
            {
                properties.Remove(name);
                value.Name = name;
                properties.Add(name, value);
                this.Changed = true;
            }
        }

        //public MappingNode()
        //{
        //}

        internal void SetParent(MappingNode newParent)
        {
            this.nodeParent = newParent;
        }

        public MappingNode(MappingNode parent)
        {
            this.nodeParent = parent;
        }

        public MappingNode(XmlNode node, MappingNode parent)
        {
            this.nodeParent = parent;
            LoadProperties(node);
        }

        protected void LoadProperties(XmlNode node)
        {
            foreach (XmlNode propNode in node.SelectNodes("Property"))
            {
                Property prop = new Property(propNode, this);
                properties.Add(prop.Name, prop);
            }
        }

        /// <summary>
        /// Saves the properties of the mapping element as subelements in the given XmlNode.
        /// </summary>
        /// <param name="node">The parent XmlNode.</param>
        internal virtual void SaveProperties(XmlNode node)
        {
            foreach (DictionaryEntry de in properties)
            {
                Property prop = (Property)de.Value;
                prop.Save(node);
            }
        }

        // This is overridden by NDOMapping to set
        // the changed variable at the root of the tree.
        [Browsable(false)]
        internal virtual bool Changed
        {
            set
            {
                if (nodeParent != null)
                    nodeParent.Changed = value;  // changed should never be set to false from the childs
            }
            get
            {
                if (nodeParent != null)
                    return nodeParent.Changed;
                else
                    return false;
            }
        }

        /// <summary>
        /// Removes the object from the object tree.
        /// </summary>
        public abstract void Remove();

    }

    /// <summary>
    /// This class is used by the NDO Framework. It encapsulates information which is stored in the <a href="MappingFile.html">NDOMapping.xml file</a>.
    /// </summary>
    /// <remarks>
    /// If you use this class to manipulate Mapping files, please note, that not all members of the classes contained in the namespace NDO.Mapping will be initialized. If you write your own persistence handler, you'll get an instance of this class
    /// as a parameter of the <see cref="NDO.IPersistenceHandler.Initialize">Initialize function</see>. In this case all fields are properly initialized.
    /// </remarks>
    public class NDOMapping : MappingNode, IEnhancerSupport
    {
        private string mappingFile;
        private string schemaVersion = string.Empty;  // Version always has a value
        bool changed;
        bool isEnhancing;


        [Browsable(false)]
        internal override bool Changed
        {
            get
            {
                return this.changed;
            }
            set
            {
                this.changed = value;
            }
        }

        [Browsable(false)]
        public bool HasChanges
        {
            get { return this.changed; }
        }

        /// <summary>
        /// Mapping schmema version. This value can only be set in the NDO configuration dialog.
        /// </summary>
        [Description("Mapping schema version. Use the NDO configuration dialog to alter this value.")]
        public string SchemaVersion
        {
            get { return schemaVersion; }
            set
            {
                schemaVersion = value;
                this.changed = true;
            }
        }

        /// <summary>
        /// Collection of all connections defined in the mapping file.
        /// </summary>
        [Browsable(false)]
        public IList Connections
        {
            get { return connections; }
            set { connections = value; }
        }
        private IList connections = new ArrayList();
        /// <summary>
        /// Collection of all class mappings in the mapping file.
        /// </summary>
        [Browsable(false)]
        public IList Classes
        {
            get { return ArrayList.ReadOnly(classes); }
        }
#if !PRO
        private IList classes = AlCreator.Create(NumberStrings.v10);
#else
		private ClassHashView classes = new ClassHashView();
#endif


        /// <summary>
        /// For NDO internal use.
        /// </summary>
        const string NDONamespace = "http://www.netdataobjects.de/NDOMapping";
        const string OldNamespace = "http://www.advanced-developers.de/NDOMapping";
        private string NDOShort = "ndo:"; // Not const to avoid overhead for internalizing
        private string unused = "Unused";
        internal string selectNdoMapping = "//ndo:NDOMapping";
        internal string selectConn = "//ndo:NDOMapping/ndo:Connections/ndo:Connection";
        internal string selectClass = "//ndo:NDOMapping/ndo:Classes/ndo:Class";
        internal string selectField = "ndo:Fields/ndo:Field";
        internal string selectRelation = "ndo:Relations/ndo:Relation";
        internal string selectOid = "ndo:Oid";
        internal string selectTypeNameColumn = "ndo:TypeNameColumn";
        internal string selectForeignKeyColumns = "ndo:ForeignKeyColumn";
        internal string selectOidColumns = "ndo:OidColumn";
        internal string selectColumns = "ndo:Columns/ndo:Column";
        internal string selectColumn = "ndo:Column";
        internal string selectMappingTable = "ndo:MappingTable";
        private string standardDbOwner = string.Empty;


        internal XmlNamespaceManager nsmgr = null;

        /// <summary>
        /// Constructor for opening a NDO Mapping file.
        /// </summary>
        /// <param name="mappingFile">Mapping file to open. This file must be existent.</param>
        /// <remarks>To construct a new Mapping file, use the static function <see cref="Create">Create</see>.</remarks>
        public NDOMapping(string mappingFile)
            : base(null)
        {
            this.mappingFile = mappingFile;

            if (!File.Exists(mappingFile))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(mappingFile);
            string ns = doc.DocumentElement.GetAttribute("xmlns");
            if (null != ns && ns == OldNamespace)
            {
                throw new Exception("Obsolete namespace in NDO mapping file. Please use the namespace " + NDONamespace + ".");
            }
            if (null != ns && ns == NDONamespace)
            {
                nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("ndo", NDONamespace);
            }
            else
            {
                this.selectConn = this.selectConn.Replace(NDOShort, string.Empty);
                this.selectClass = this.selectClass.Replace(NDOShort, string.Empty);
                this.selectField = this.selectField.Replace(NDOShort, string.Empty);
                this.selectRelation = this.selectRelation.Replace(NDOShort, string.Empty);
                this.selectOid = this.selectOid.Replace(NDOShort, string.Empty);
                this.selectMappingTable = this.selectMappingTable.Replace(NDOShort, string.Empty);
                this.selectNdoMapping = this.selectNdoMapping.Replace(NDOShort, string.Empty);
                this.selectTypeNameColumn = this.selectTypeNameColumn.Replace(NDOShort, string.Empty);
                this.selectForeignKeyColumns = this.selectForeignKeyColumns.Replace(NDOShort, string.Empty);
                this.selectOidColumns = this.selectOidColumns.Replace(NDOShort, string.Empty);
                this.selectColumns = this.selectColumns.Replace(NDOShort, string.Empty);
                this.selectColumn = this.selectColumn.Replace(NDOShort, string.Empty);
            }

            XmlNode node = doc.SelectSingleNode(selectNdoMapping, nsmgr);
            if (node.Attributes["SchemaVersion"] != null)
                this.schemaVersion = node.Attributes["SchemaVersion"].Value;

            LoadProperties(node);

            XmlNodeList nl = doc.SelectNodes(selectConn, nsmgr);
            foreach (XmlNode connNode in nl)
            {
                connections.Add(new Connection(connNode, this));
            }
            nl = doc.SelectNodes(selectClass, nsmgr);
            foreach (XmlNode classNode in nl)
            {
                classes.Add(new Class(classNode, this));
            }
            changed = false;
        }

        private NDOMapping()
            : base(null)
        {
        }

        /// <summary>
        /// Create a new mapping file. 
        /// </summary>
        /// <param name="mappingFile">Path of the file to create.</param>
        /// <returns>NDOMapping object, which represents the new file.</returns>
        public static NDOMapping Create(string mappingFile)
        {
            NDOMapping m = new NDOMapping();
            m.mappingFile = mappingFile;
            return m;
        }


        /// <summary>
        /// Gets the file name of the mapping file.
        /// </summary>
        public string FileName
        {
            get { return mappingFile; }
        }

        /// <summary>
        /// Changes the file name of the mapping file to the given file name and saves it.
        /// </summary>
        /// <param name="fileName">The new file name</param>
        public void SaveAs(string fileName)
        {
            mappingFile = fileName;
            Save();
        }

        /// <summary>
        /// Saves the mapping file.
        /// </summary>
        public void Save()
        {
            XmlDocument doc = new XmlDocument();

            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement docNode = doc.CreateElement("NDOMapping");
            doc.AppendChild(docNode);
            docNode.SetAttribute("SchemaVersion", this.schemaVersion);

            base.SaveProperties(docNode);

            XmlElement node = doc.CreateElement("Connections");
            docNode.AppendChild(node);
            foreach (Connection c in connections)
            {
                c.Save(node);
            }

            node = doc.CreateElement("Classes");
            docNode.AppendChild(node);

            // !STD uses an own ArrayList class and > PRO uses a ClassHashView
            // which isn't sortable
            ArrayList al = new ArrayList(classes);
            al.Sort();

            foreach (Class cl in al)
            {
                cl.Save(node);
            }
            doc.Save(mappingFile);

            changed = false;
        }

        /// <summary>
        /// Calls to this method will be ignored.
        /// </summary>
        public override void Remove()
        {
        }

        /// <summary>
        /// Add a connection with a dummy connection string.
        /// </summary>
        private void AddStandardConnection()
        {
            if (Connections.Count > 0)
                return;

            Connection c = new Connection(this);
            c.ID = "C0";
            c.FromStandardConnection();
            Connections.Add(c);
        }

        /// <summary>
        /// Used by NDO, if Data Tables should be prefixed with a owner or schema name.
        /// </summary>
        [Browsable(false)]
        public string StandardDbOwner
        {
            get { return standardDbOwner; }
            set { standardDbOwner = value; }
        }

        /// <summary>
        /// Add an abstract class mapping.
        /// </summary>
        /// <param name="fullName">Fully qualified name of the class.</param>
        /// <param name="AssemblyName">Assembly name of the assembly the class resides in.</param>
        /// <returns>A new Class object.</returns>
        public Class AddAbstractClass(string fullName, string AssemblyName, OidColumnAttribute[] columnAttributes)
        {
            Class c = AddStandardClass(fullName, AssemblyName, columnAttributes);
            c.TableName = unused;
            c.IsAbstract = true;
            ((OidColumn)c.Oid.OidColumns[0]).Name = unused;
            return c;
        }

        /// <summary>
        /// Adds a class mapping.
        /// </summary>
        /// <param name="fullName">Fully qualified name of the class.</param>
        /// <param name="AssemblyName">Name of the assembly, the class resides in.</param>
        /// <returns>A new Class object.</returns>
        public Class AddStandardClass(string fullName, string AssemblyName, OidColumnAttribute[] columnAttributes)
        {
            this.Changed = true;
            Class c = new Class(this);
            c.FullName = fullName;
            c.AssemblyName = AssemblyName;

            string tableName = fullName.Substring(fullName.LastIndexOf('.') + 1);
            int p = tableName.LastIndexOf('`');

            if (p > -1)
                tableName = tableName.Substring(0, p);

            if (this.standardDbOwner != string.Empty)
                tableName = standardDbOwner + "." + tableName;

            bool tableNameIsFree = false;
            int i = 0;
            while (!tableNameIsFree)
            {
                tableNameIsFree = true;
                foreach (Class c2 in classes)
                {
                    if (c2.TableName == tableName)
                    {
                        tableNameIsFree = false;
                        break;
                    }
                }
                if (!tableNameIsFree)
                {
                    i++;
                    tableName = fullName.Substring(fullName.LastIndexOf('.') + 1) + i.ToString();
                }
            }

            c.TableName = tableName;

            if (Connections.Count == 0)
                AddStandardConnection();
            c.ConnectionId = ((Connection)Connections[0]).ID;
            ClassOid oid = c.NewOid();
            if (columnAttributes == null)
            {
                OidColumn column = oid.NewOidColumn();
                column.Name = "ID";
            }
            else
            {
                oid.RemapOidColumns(columnAttributes);
                int count = oid.OidColumns.Count;
                i = 1;
                new OidColumnIterator(oid).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
                {
                    if (string.IsNullOrEmpty(oidColumn.Name))
                    {
                        if (count == 1)
                            oidColumn.Name = "ID";
                        else
                            oidColumn.Name = "ID" + i;
                    }
                    i++;
                });
            }

            AddClass(c);
            return c;
        }

        /// <summary>
        /// Find a Class object for the specified class.
        /// </summary>
        /// <param name="t">Type of the class.</param>
        /// <returns>mapping info about the class</returns>
        public virtual Class FindClass(Type t)
        {
            string name;
            if (t.IsGenericType)
                name = t.GetGenericTypeDefinition().FullName;
            else
                name = t.FullName;
            return FindClass(name);
        }


        /// <summary>
        /// Find the NDO provider for the connection associated with the given class mapping.
        /// Throws an Exception if no connection for the type is found.
        /// </summary>
        /// <param name="cl">Class mapping for the type, the provider is searched for.</param>
        /// <returns>If found, the provider will be returned. Else a NDOException with ErrorNumber 34 will be thrown.</returns>
        /// <remarks>
        /// If the Class object doesn't contain a valid reference to a Connection object, a NDOException with 
        /// ErrorNumber 16 will be thrown.
        /// </remarks>
        public IProvider GetProvider(Class cl)
        {
            Connection conn = this.FindConnection(cl.ConnectionId);
            if (conn == null)
                throw new NDOException(16, "Can't find a Connection object with the ID " + cl.ConnectionId + " in the mapping file; Check the mapping info for the class '" + cl.FullName + '\'');
            return GetProvider(conn.Type);
        }


        public IProvider GetProvider(Connection conn)
        {
            if (conn == null ||
                Connection.DummyConnectionString == conn.Name
                || null == conn.Name
                || string.Empty == conn.Name)
                throw new NDOException(44, "Connection object is null, or connection name is null or empty.");
            return GetProvider(conn.Type);
        }

        /// <summary>
        /// Finds the NDO provider with the given Id.
        /// </summary>
        /// <param name="providerShortName">
        /// A short name, which normally appears as Type property 
        /// in Connection objects.</param>
        /// <returns>If found, the provider will be returned. Else a NDOException with ErrorNumber 34 will be thrown.</returns>
        public IProvider GetProvider(string providerShortName)
        {
            IProvider p = NDOProviderFactory.Instance[providerShortName];
            if (p == null)
                throw new NDOException(34, "There is no provider of Type " + providerShortName + " in the NDOProviderFactory. Check, if you have to add a NDO provider plug-in, or use [SqlServer|Oracle|Access] as Connection Type");
            return p;
        }


        /// <summary>
        /// Find the NDO provider for the connection associated with the given type.
        /// Throws an Exception if no connection for the type is found.
        /// </summary>
        /// <param name="t">The type, the provider is searched for</param>
        /// <returns>If a provider exists, the provider will be returned, else null.</returns>
        public IProvider GetProvider(Type t)
        {
            Class cl = FindClass(t);
            if (cl == null)
                throw new NDOException(17, "Can't find mapping information for class " + t.FullName);
            return GetProvider(cl);
        }

        /// <summary>
        /// Add a Class object to the Classes list.
        /// </summary>
        /// <param name="c">The class mapping to add</param>
        public void AddClass(Class c)
        {
            c.SetParent(this);
            this.Changed = true;
            classes.Add(c);
        }


        public void RemoveClass(Class c)
        {
            if (classes.Contains(c))
            {
                classes.Remove(c);
                this.Changed = true;
            }
        }


        /// <summary>
        /// Adds a connection object to the Connections list.
        /// </summary>
        /// <param name="c">The connection object to add</param>
        public void AddConnection(Connection c)
        {
            this.Changed = true;
            Connections.Add(c);
        }


        /// <summary>
        /// Creates a new Connection object and adds it to the Connections list
        /// </summary>
        /// <param name="connStr">The connection string.</param>
        /// <param name="connType">The provider type to be used for the connection.</param>
        public Connection NewConnection(string connStr, string connType)
        {
            int maxNr = 0;
            Regex regex = new Regex(@"\d+");
            foreach(Connection oldc in this.connections)
            {
                Match match = regex.Match(oldc.ID);
                if (match.Success)
                    maxNr = Math.Max(int.Parse(match.Value) + 1, maxNr);
            }
            Connection conn = new Connection(this);
            conn.ID = "C" + maxNr;
            conn.Name = connStr;
            conn.Type = connType;
            this.connections.Add(conn);
            return conn;
        }


        /// <summary>
        /// Lookup for a Class object in the Classes list.
        /// </summary>
        /// <param name="c">The Class object to search for</param>
        /// <returns>The Class object or null, if the class doesn't exist</returns>
        public virtual Class FindClass(Class c)
        {
#if PRO
			return classes[c];
#else
            foreach (Class cl in classes)
                if (cl.FullName == c.FullName)
                    return cl;
            return null;
#endif
        }

        /// <summary>
        /// Find a Class object for the specified class.
        /// </summary>
        /// <param name="fullName">Fully qualified name of the class.</param>
        /// <returns>mapping info about the class</returns>
        public virtual Class FindClass(string fullName)
        {
#if PRO
			return classes[fullName];
#else
            foreach (Class cl in classes)
                if (cl.FullName == fullName)
                    return cl;
            return null;
#endif
        }


        /// <summary>
        /// Search for a connection with the specified connection id.
        /// </summary>
        /// <param name="connectionId">Connection id</param>
        /// <returns>The Connection object or null if the connection doesn't exist</returns>
        public Connection FindConnection(string connectionId)
        {
            foreach (Connection conn in Connections)
            {
                if (conn.ID == connectionId)
                    return conn;
            }
            return null;
        }

        /// <summary>
        /// Find the associated connection of the class.
        /// </summary>
        /// <param name="cl">the class</param>
        /// <returns>the mapping info of the connection</returns>
        public Connection FindConnection(Class cl)
        {
            return FindConnection(cl.ConnectionId);
        }


        /// <summary>
        /// Search for a connection with the same connection string.
        /// </summary>
        /// <param name="c">A Connection object</param>
        /// <returns>The Connection object or null, if no connection with the same connection string exists.</returns>
        private Connection FindEquivalentConnection(Connection c)
        {
            foreach (Connection conn in Connections)
            {
                if (conn.Name == c.Name)
                    return conn;
            }
            return null;
        }


        /// <summary>
        /// Hilfsfunktion für MergeMapping
        /// </summary>
        /// <param name="f1">Field zum Vergleich</param>
        /// <param name="f2">Field zum Vergleich</param>
        /// <returns></returns>
        private bool FieldsAreDifferent(Field f1, Field f2)
        {
            // The FieldName is the same
            return ColumnsAreDifferent(f1.Column, f2.Column);
        }

        private bool OidColumnsAreDifferent(OidColumn c1, OidColumn c2)
        {
            if (c1.FieldName != c2.FieldName) return true;
            if (c1.RelationName != c2.RelationName) return true;
            if (c1.AutoIncremented != c2.AutoIncremented) return true;
            if (c1.AutoIncrementStart != c2.AutoIncrementStart) return true;
            if (c1.AutoIncrementStep != c2.AutoIncrementStep) return true;
            return ColumnsAreDifferent(c1, c2);
        }

        private bool ColumnsAreDifferent(Column c1, Column c2)
        {
            if (c1.AllowDbNull != c2.AllowDbNull) return true;
            if (c1.DbType != c2.DbType) return true;
            if (c1.IgnoreColumnSizeInDDL != c2.IgnoreColumnSizeInDDL) return true;
            if (c1.Name != c2.Name) return true;
            if (c1.NetType != c2.NetType) return true;
            if (c1.Precision != c2.Precision) return true;
            if (c1.Size != c2.Size) return true;
            return false;
        }

        private bool ForeignKeyColumnsAreDifferent(Relation r1, Relation r2)
        {
            if (r1.ForeignKeyColumns.Count != r2.ForeignKeyColumns.Count)
                return true;

            for (int i = 0; i < r1.ForeignKeyColumns.Count; i++)
            {
                ForeignKeyColumn fkc1 = (ForeignKeyColumn)r1.ForeignKeyColumns[i];
                ForeignKeyColumn fkc2 = (ForeignKeyColumn)r2.ForeignKeyColumns[i];
                if (ColumnsAreDifferent(fkc1, fkc2)) return true;
            }
            return false;
        }

        private bool ForeignKeyColumnsAreDifferent(MappingTable t1, MappingTable t2)
        {
            if (t1.ChildForeignKeyColumns.Count != t2.ChildForeignKeyColumns.Count)
                return true;

            for (int i = 0; i < t1.ChildForeignKeyColumns.Count; i++)
            {
                ForeignKeyColumn fkc1 = (ForeignKeyColumn)t1.ChildForeignKeyColumns[i];
                ForeignKeyColumn fkc2 = (ForeignKeyColumn)t2.ChildForeignKeyColumns[i];
                if (ColumnsAreDifferent(fkc1, fkc2)) return true;
            }
            return false;
        }


        /// <summary>
        /// Hilfsfunktion für MergeMapping
        /// </summary>
        /// <param name="r1">Relation zum Vergleich</param>
        /// <param name="r2">Relation zum Vergleich</param>
        /// <returns></returns>
        private bool RelationsAreDifferent(Relation r1, Relation r2)
        {
            if (r1.ForeignKeyTypeColumnName != r2.ForeignKeyTypeColumnName) return true;
            if (ForeignKeyColumnsAreDifferent(r1, r2)) return true;
            if (r1.MappingTable == null && r2.MappingTable != null) return true;
            if (r2.MappingTable == null && r1.MappingTable != null) return true;
            if (r1.MappingTable != null)
            {
                if (r1.MappingTable.TableName != r2.MappingTable.TableName) return true;
                if (ForeignKeyColumnsAreDifferent(r1.MappingTable, r2.MappingTable)) return true;
            }
            return false;
        }

        private bool OidsAreDifferent(ClassOid oid1, ClassOid oid2)
        {
            if (oid1.OidColumns.Count != oid2.OidColumns.Count)
                return true;
            for (int i = 0; i < oid1.OidColumns.Count; i++)
            {
                OidColumn oidc1 = (OidColumn)oid1.OidColumns[i];
                OidColumn oidc2 = (OidColumn)oid2.OidColumns[i];
                if (OidColumnsAreDifferent(oidc1, oidc2)) return true;
            }
            return false;
        }


        /// <summary>
        /// Hilfsfunktion für MergeMapping
        /// </summary>
        /// <param name="c1">Klasse zum Vergleich</param>
        /// <param name="c2">Klasse zum Vergleich</param>
        /// <returns></returns>
        private bool ClassesAreDifferent(Class c1, Class c2)
        {
            if (c1.Fields.Count != c2.Fields.Count) return true;
            if (c1.Relations.Count != c2.Relations.Count) return true;
            if (c1.AssemblyName != c2.AssemblyName) return true;
            if (c1.TableName != c2.TableName) return true;
            if (OidsAreDifferent(c1.Oid, c2.Oid)) return true;

            foreach (Field f1 in c1.Fields)
            {
                Field f2 = c2.FindField(f1.Name);
                if (null == f2) return true;
                if (FieldsAreDifferent(f1, f2)) return true;
            }
            foreach (Relation r1 in c1.Relations)
            {
                Relation r2 = c1.FindRelation(r1.FieldName);
                if (null == r2) return true;
                if (RelationsAreDifferent(r1, r2)) return true;
            }
            return false;
        }


        /// <summary>
        /// Merge two mapping files. This is used by the enhancer to collect mapping information from several base assemblies.
        /// </summary>
        /// <param name="mergeMapping">Path to the mapping file to merge.</param>
        public void MergeMapping(NDOMapping mergeMapping)
        {
            /*
             * Für alle Conns. in mergeMapping: 
             * Gibt's ne gleiche Connection in this?
             * Ja: ist ID von mergeConn. != this.Conn.ID
             *     Ja: alle Klassen zu dieser ID anpassen 
             * Nein: 
             *		übertrage Conn nach this, ggf. neue ID anlegen
             *		ID neu?
             *      Ja: alle Klassen anpassen.
             * alle Klassen der Conn. kopieren, markieren
             */

            if (this.Connections.Count > 0 && mergeMapping.Connections.Count > 0)
            {
                Connection std1 = this.Connections[0] as Connection;
                Connection std2 = mergeMapping.Connections[0] as Connection;
                if (std1.Name == Connection.DummyConnectionString && std2.Name != Connection.DummyConnectionString)
                {
                    std1.Name = std2.Name;
                    std1.Type = std2.Type;
                }
                else if (std2.Name == Connection.DummyConnectionString && std1.Name != Connection.DummyConnectionString)
                {
                    std2.Name = std1.Name;
                    std2.Type = std1.Type;
                }
            }
            else
            {
                if (this.Connections.Count == 1 && ((Connection)this.Connections[0]).Name == Connection.DummyConnectionString)
                {
                    ((Connection)this.Connections[0]).Name = Connection.StandardConnection.Name;
                    ((Connection)this.Connections[0]).Type = Connection.StandardConnection.Type;
                }
                if (mergeMapping.Connections.Count == 1 && ((Connection)mergeMapping.Connections[0]).Name == Connection.DummyConnectionString)
                {
                    ((Connection)mergeMapping.Connections[0]).Name = Connection.StandardConnection.Name;
                    ((Connection)mergeMapping.Connections[0]).Type = Connection.StandardConnection.Type;
                }
            }


            foreach (Connection co in mergeMapping.Connections)
            {
                if (co.Name == Connection.DummyConnectionString)
                {
                    co.Name = Connection.StandardConnection.Name;
                    co.Type = Connection.StandardConnection.Type;
                }
                IList classesToCopy = new ArrayList();
                foreach (Class cl in mergeMapping.classes)
                {
                    if (cl.ConnectionId == co.ID)
                        classesToCopy.Add(cl);
                }

                // IDs werden umbenannt und könnten dann gleich einer vorhandenen
                // ID in mergeMapping.Connections werden.
                foreach (Class cl in classesToCopy)
                    mergeMapping.RemoveClass(cl);

                Connection thisConn = FindEquivalentConnection(co);
                // Gibt's schon eine Connection mit dem gleichen Conn.-String?
                if (null != thisConn)
                {
                    if (co.ID != thisConn.ID)
                    {
                        foreach (Class cl1 in classesToCopy)
                            cl1.ConnectionId = thisConn.ID;
                    }
                }
                else
                {
                    bool gleicheID = false;
                    int maxId = 0;
                    foreach (Connection co1 in this.Connections)
                    {
                        if (co1.ID == co.ID)
                        {
                            gleicheID = true;
                        }
                        if (co1.ID.StartsWith("C"))
                            maxId = System.Math.Max(maxId, int.Parse(co1.ID.Substring(1)));
                    }
                    if (gleicheID)
                    {
                        maxId++;
                        co.ID = "C" + maxId.ToString();
                        foreach (Class cl2 in classesToCopy)
                            cl2.ConnectionId = co.ID;
                    }
                    co.SetParent(this);
                    AddConnection(co);
                }

                // Alle Klassen der Connection einfügen
                foreach (NDO.Mapping.Class cl3 in classesToCopy)
                {
                    Class classToRemove = null;
                    bool classFound = false;
                    foreach (Class cl in classes)
                    {
                        if (cl.FullName == cl3.FullName)
                        {
                            if (ClassesAreDifferent(cl, cl3))
                                classToRemove = cl;
                            classFound = true;
                            break;
                        }
                    }
                    //					for (int i = 0; i < classes.Count; i++) {
                    //						if ( ((Class) classes[i]).FullName == cl3.FullName) {
                    //							if (ClassesAreDifferent( (Class) classes[i], cl3)) {
                    //								classes[i] = cl3; // Neue Informationen zur Klasse übernehmen
                    //								this.Changed = true;
                    //							}
                    //							classFound = true;
                    //							break;
                    //						}
                    //					}
                    if (classFound)
                    {
                        if (classToRemove != null)
                        {
                            classes.Remove(classToRemove);
                            AddClass(cl3);
                        }
                    }
                    else
                        AddClass(cl3);

                }
            }
        } // Ende von MergeMapping



        [Browsable(false)]
        bool IEnhancerSupport.IsEnhancing
        {
            get { return this.isEnhancing; }
            set { this.isEnhancing = value; }
        }

    }


    /// <summary>
    /// This class encapsulates a connection string and makes it available with a short ID.
    /// </summary>
    /// <remarks>This class is equivalent to the Connection element in the mapping file schema.</remarks>
    public class Connection : MappingNode
    {
        #region Standard Connection Stuff
        public const string DummyConnectionString = "Substitute this string with your connection string";

        static Connection standardConnection;

        public static Connection StandardConnection
        {
            get { return standardConnection; }
        }

        static Connection()
        {
            standardConnection = new Connection(null);
            standardConnection.name = DummyConnectionString;
            standardConnection.type = "";
        }

        //		public static void ImportOleDbConnectionString(string oleDbString)
        //		{
        //			if (oleDbString.StartsWith(sqlString))
        //			{
        //				standardConnection.type = "SqlServer";
        //				standardConnection.name = oleDbString.Substring(sqlString.Length);
        //			}
        //			else if (oleDbString.StartsWith(oracleString1))
        //			{
        //				standardConnection.type = "Oracle";
        //				standardConnection.name = oleDbString.Substring(oracleString1.Length);
        //			}
        //			else if (oleDbString.StartsWith(oracleString2))
        //			{
        //				standardConnection.type = "Oracle";
        //				standardConnection.name = oleDbString.Substring(oracleString2.Length);
        //			}
        //			else if (oleDbString.StartsWith(jetString))
        //			{
        //				standardConnection.type = "Access";
        //				// don't remove the provider part since Jet is an OleDb provider
        //			}
        //			else 
        //			{
        //				standardConnection.type = string.Empty;
        //				standardConnection.name = oleDbString;
        //			}
        //		}
        //
        //		public static string GetTypeFromOleDbString(string oleDbString)
        //		{
        //			if (oleDbString.StartsWith(sqlString))
        //				return "SqlServer";
        //			if (oleDbString.StartsWith(oracleString1))
        //				return "Oracle";
        //			if (oleDbString.StartsWith(oracleString2))
        //				return "Oracle";
        //			if (oleDbString.StartsWith(jetString))
        //				return "Access";
        //			return string.Empty;
        //		}

        #endregion

        # region State variables and accessor properties
        private string iD = "";
        /// <summary>
        /// Gets or sets the ID, with which the connection is referenced in the Class objects.
        /// </summary>
        [Description("The ID, with which the connection is referenced in the Class objects.")]
        public string ID
        {
            get { return iD; }
            set { iD = value; this.Changed = true; }
        }
        private string name = "";
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [Description("The connection string.")]
        public string Name
        {
            get { return name; }
            set { name = value; this.Changed = true; }
        }
        private string type = "Sql";
        /// <summary>
        /// Gets or sets the provider type string.
        /// </summary>
        /// <remarks>
        /// This string sould match the Name property of a 
        /// provider registered in the NDOProviderFactory.
        /// <seealso cref="NDOProviderFactory"/>
        /// </remarks>
        [Description("The provider type string.")]
        public string Type
        {
            get { return type; }
            set { type = value; this.Changed = true; }
        }

        [Browsable(false)]
        public NDOMapping Parent
        {
            get { return nodeParent as NDOMapping; }
            //			set { parent = value; }
        }

        /// <summary>
        /// Removes the Connection object from the Connection list of the parent object.
        /// </summary>
        public override void Remove()
        {
            Parent.Connections.Remove(this);
        }

        //private const string sqlString = "Provider=SQLOLEDB.1;";
        //private const string oracleString1 = "Provider=MSDAORA.1;";
        //private const string oracleString2 = "Provider=OraOLEDB.Oracle.1;";
        //private const string jetString = "Provider=Microsoft.Jet.OLEDB";

        #endregion


        /// <summary>
        /// Constructs a new Connection object
        /// </summary>
        /// <param name="parent">The NDOMapping object, which is the parent of the connection.</param>
        public Connection(NDOMapping parent)
            : base(parent)
        {
        }

#if NDO11
		string owner;
		/// <summary>
		///  Don't use this property - it's obsolete.
		/// </summary>
		public string Owner
		{
			get { return owner; }
			set { owner = value; }
		}
#endif

        /// <summary>
        /// Returns a connection type according to an OleDb connection string.
        /// </summary>
        /// <param name="connStr">An OleDb connection string.</param>
        /// <returns>A type name, if the connection string could be recognized, else null.</returns>
        public string GetDefaultTypeString(string connStr)
        {
            return null;
        }

        /// <summary>
        /// This function used by the enhancer only. Don't call it.
        /// </summary>
        public void FromStandardConnection()
        {
            name = standardConnection.name;
            type = standardConnection.type;

            if (null == name || string.Empty == name)
            {
                name = DummyConnectionString;
            }

        }

        /// <summary>
        /// Construct a Connection object based on a Connection node of the mapping file.
        /// </summary>
        /// <param name="connNode">Connection node</param>
        /// <param name="parent">An object of type NDOMapping, which is the parent of the connection object.</param>
        internal Connection(XmlNode connNode, NDOMapping parent)
            : base(connNode, parent)
        {
            name = connNode.Attributes["Name"].Value;
            iD = connNode.Attributes["ID"].Value;
            type = connNode.Attributes["Type"].Value;
            if (name == null || name == string.Empty || name == Connection.DummyConnectionString)
            {
                FromStandardConnection();
            }
        }

        /// <summary>
        /// Saves the Connection object as a Connection node. Don't use that funktion directly, 
        /// it will be called by the Save function of the NDOMapping class.
        /// </summary>
        /// <param name="parentNode">The parent node</param>		
        internal void Save(XmlNode parentNode)
        {
            XmlElement newNode = parentNode.OwnerDocument.CreateElement("Connection");
            newNode.SetAttribute("ID", iD.ToString());
            newNode.SetAttribute("Name", name);
            newNode.SetAttribute("Type", type);
            parentNode.AppendChild(newNode);
            base.SaveProperties(newNode);
        }

        /// <summary>
        /// String representation of the Connection object
        /// </summary>
        /// <returns>The connection string</returns>
        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// This class encapsulates a mapping table for the connection of two data tables.
    /// </summary>
    /// <remarks>This class is equivalent to the MappingTable element of the mapping file schema.</remarks>
    public class MappingTable : MappingNode
    {
        #region State variables and accessors

        private string tableName = "";
        /// <summary>
        /// Name of the table, where the relation information is stored.
        /// </summary>
        [Description("Name of the table, where the relation information is stored.")]
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; this.Changed = true; }
        }
        private ArrayList childForeignKeyColumns = new ArrayList();
        /// <summary>
        /// Contains the description of the ForeignKeyColumns pointing to 
        /// the related type. Under normal circumstances this list contains exactly
        /// one entry. If the related type has a multi column oid there need to be
        /// more foreign key columns. The order of the column descriptions must match
        /// the order of the oid columns in the related type.
        /// </summary>
        [Browsable(false)]
        public IList ChildForeignKeyColumns
        {
            get { return this.childForeignKeyColumns; }
        }

        /// <summary>
        /// Adds a new ForeignKeyColumn to the mapping table.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Used by the enhancer and the mapping tool.
        /// </remarks>
        public ForeignKeyColumn NewForeignKeyColumn()
        {
            ForeignKeyColumn fkColumn = new ForeignKeyColumn(this);
            this.childForeignKeyColumns.Add(fkColumn);
            return fkColumn;
        }

        private string childForeignKeyTypeColumnName = null;
        /// <summary>
        /// Name of the column, where the type code of the related object is stored.
        /// </summary>
        [Description("Name of the column, where the type code of the related object is stored.")]
        public string ChildForeignKeyTypeColumnName
        {
            get { return childForeignKeyTypeColumnName; }
            set { childForeignKeyTypeColumnName = value; this.Changed = true; }
        }

        /// <summary>
        /// Returns the relation the MappingTable belongs to. 
        /// </summary>
        [Browsable(false)]
        Relation Parent
        {
            get { return (Relation)nodeParent; }
        }

        /// <summary>
        /// Deletes the MappingTable object from the parent object.
        /// Note: In some scenarios a mapping table is mandatory. 
        /// Deleting the mapping table entry might cause exceptions thrown by NDO.
        /// </summary>
        public override void Remove()
        {
            Parent.MappingTable = null;
        }

#if nix
		private string childForeignKeyColumnName = "";
		/// <summary>
		/// Name of the column, where the foreign key of the related object is stored.
		/// </summary>
		[Description("Name of the column, where the foreign key of the related object is stored.")]
		public string ChildForeignKeyColumnName
		{
			get { return childForeignKeyColumnName; }
			set { childForeignKeyColumnName = value; this.Changed = true; }
		}
#endif
        private string connectionId = "";
        /// <summary>
        /// Connection string of the database, where the mapping table is stored.
        /// </summary>
        [Description("Connection string of the database, where the mapping table is stored.")]
        public string ConnectionId
        {
            get { return connectionId; }
            set { connectionId = value; this.Changed = true; }
        }
        #endregion
        /// <summary>
        /// Construct a new MappingTable
        /// </summary>
        public MappingTable(Relation parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Construct a mapping table based on a MappingTable node
        /// </summary>
        /// <param name="mtNode">The node</param>
        /// <param name="parent">A Relation object, which is the parent of the mapping table.</param>
        internal MappingTable(XmlNode mtNode, Relation parent)
            : base(parent)
        {
            tableName = mtNode.Attributes["TableName"].Value;

			if ( mtNode.Attributes["ChildForeignKeyTypeColumnName"] != null && mtNode.Attributes["ChildForeignKeyTypeColumnName"].Value != string.Empty )
                this.childForeignKeyTypeColumnName = mtNode.Attributes["ChildForeignKeyTypeColumnName"].Value;

            if (mtNode.Attributes["ChildForeignKeyColumnName"] != null) // Old mapping
            {
                ForeignKeyColumn fkColumn = new ForeignKeyColumn(this);
                fkColumn.Name = mtNode.Attributes["ChildForeignKeyColumnName"].Value;
                this.childForeignKeyColumns.Add(fkColumn);
            }
            else
            {
                XmlNodeList nl = mtNode.SelectNodes(parent.Parent.Parent.selectForeignKeyColumns);
                foreach (XmlNode fkcNode in nl)
                    this.childForeignKeyColumns.Add(new ForeignKeyColumn(fkcNode, this));
            }

            connectionId = mtNode.Attributes["ConnectionId"].Value;
        }

        internal void Save(XmlNode parentNode)
        {
            XmlElement mtNode = parentNode.OwnerDocument.CreateElement("MappingTable");
            parentNode.AppendChild(mtNode);
            base.SaveProperties(mtNode); // Save properties
            mtNode.SetAttribute("TableName", this.tableName);

            if (!string.IsNullOrEmpty(ChildForeignKeyTypeColumnName))
                mtNode.SetAttribute("ChildForeignKeyTypeColumnName", childForeignKeyTypeColumnName);

            foreach (ForeignKeyColumn fkColumn in this.childForeignKeyColumns)
                fkColumn.Save(mtNode);

            mtNode.SetAttribute("ConnectionId", connectionId);

#if nix
			if (null != childForeignKeyColumnName)
				mtNode.SetAttribute("ChildForeignKeyColumnName", childForeignKeyColumnName);
#endif
        }
    }

    /// <summary>
    /// This enum is used to determine, where the foreign key of a relation resides.
    /// </summary>
    public enum RelationMultiplicity
    {
        /// <summary>
        /// 1:1 relation - foreign key resides in the own table or in a mapping table
        /// </summary>
        Element,
        /// <summary>
        /// 1:n relation - foreign key resides in the foreign table or in a mapping table
        /// </summary>
        List
    }

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
            Parent.Relations.Remove(this);
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

        ArrayList foreignKeyColumns = new ArrayList();
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
        public IList ForeignKeyColumns
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
        public IList ReferencedSubClasses
        {
            get
            {
                ArrayList result = new ArrayList();
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

			if ( relNode.Attributes["ForeignKeyTypeColumnName"] != null && relNode.Attributes["ForeignKeyTypeColumnName"].Value != string.Empty )
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
                throw new NDOException(20, "Can't find field " + t.Name + "." + FieldName);

            FieldType = fi.FieldType;

            System.Attribute a = System.Attribute.GetCustomAttribute(fi, typeof(NDORelationAttribute), false);
            NDORelationAttribute ra = (NDORelationAttribute)a;

            this.composition = ra != null && (ra.Info & RelationInfo.Composite) != 0;

            if (fi.FieldType == typeof(IList) || fi.FieldType.GetInterface("IList") != null || fi.FieldType.FullName.StartsWith("System.Collections.Generic.IList`1"))
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
            hasSubclasses = (relatedClass.Subclasses.Count > 0)
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
                if (relatedClass.Oid.OidColumns.Count != this.mappingTable.ChildForeignKeyColumns.Count)
                    throw new NDOException(115, "Column count between relation and Oid doesn't match. Type " + relatedClass.FullName + " has an oid column count of " + relatedClass.Oid.OidColumns.Count + ". The Relation " + this.Parent.FullName + "." + this.fieldName + " has a foreign key column count of " + this.mappingTable.ChildForeignKeyColumns.Count + '.');
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

    /// <summary>
    /// This class encapsulates a field mapping
    /// </summary>
    /// <remarks>This class is equivalent to the Field element of the mapping file schema.</remarks>
    public class Field : MappingNode, IComparable
    {
        #region State variables and accessors
        /// <summary>
        /// Field name of the class
        /// </summary>
        [ReadOnly(true), Description("Field name of the class")]
        public string Name
        {
            get { return name; }
            set { name = value; this.Changed = true; }
        }
        private string name;

#if nix

		/// <summary>
		/// Name of the Database column, the field is mapped to
		/// </summary>
		[Description("Name of the Database column, the field is mapped to.")]
		public string ColumnName
		{
			get { return columnName; }
			set { columnName = value; this.Changed = true; }
		}
		private string columnName;
		/// <summary>
		/// Optional type string; use database specific types.
		/// </summary>
		[Description("Optional type string; use database specific types.")]
		public string ColumnType
		{
			get { return columnType; }
			set { columnType = value; this.Changed = true; }
		}
		private string columnType;
		/// <summary>
		/// Optional. Determines the length of a column.
		/// </summary>
		[Description("Optional; determines the length of a column.")]
		public int ColumnLength
		{
			get { return columnLength; }
			set { columnLength = value; this.Changed = true; }
		}
		private int columnLength;
		/// <summary>
		/// Optional. Determines the number of digits after the decimal point. If possible, the generated DDL for this column will contain a length/precision tupel.
		/// </summary>
		[Description("Optional; determines the number of digits after the decimal point.")]
		public int ColumnPrecision
		{
			get { return columnPrecision; }
			set { columnPrecision = value; this.Changed = true; }
		}
		private int columnPrecision;

        bool allowDbNull;

        /// <summary>
        /// Determines, if the column may contain NULL values.
        /// </summary>
        /// <remarks>
        /// The default value of this property is True. 
        /// NDO uses this property, to set the corresponding parameter in the DDL scripts. 
        /// NDO doesn't check the actual values passed as parameters. If you disallow null
        /// values for a certain column, make sure that the corresponding fields in your application
        /// don't contain null values before an object is going to be saved.
        /// </remarks>
        public bool AllowDbNull
        {
            get { return allowDbNull; }
            set { allowDbNull = value; }
        } 

		/// <summary>
		/// This value is set to True, if the DDL generator should ignore the ColumnLength and ColumnPrecision values
		/// while constructing the Column in DDL.
		/// </summary>
		[Description("True, if the DDL generator should ignore the ColumnLength and ColumnPrecision values.")]
		public bool IgnoreLengthInDDL
		{
			get { return ignoreLengthInDDL; }
			set { ignoreLengthInDDL = value; this.Changed = true; }
		}
		private bool ignoreLengthInDDL;

#endif

        /// <summary>
        ///  This is used only by the NDOPersistenceHander class. Don't use this member - it will only be initialized in rare situations.
        /// </summary>
        [Browsable(false)]
        internal object ColumnDbType
        {
            get { return columnDbType; }
            set { columnDbType = value; }
        }
        private object columnDbType;

        Column column;
        [Browsable(false)]
        public Column Column
        {
            get { return this.column; }
        }

        [Browsable(false)]
        public Class Parent
        {
            get { return nodeParent as Class; }
        }

        /// <summary>
        /// Removes the Field object from the field mappings list of the parent object.
        /// </summary>
        public override void Remove()
        {
            Parent.Fields.Remove(this);
        }

        int ordinal;
        /// <summary>
        /// This field is used by NDO for internal purposes. Don't use this field.
        /// </summary>
        [Browsable(false)]
        public int Ordinal
        {
            get { return ordinal; }
            set { ordinal = value; }
        }

        #endregion

        #region Constructors and Save function

        /// <summary>
        /// Constructs a new field
        /// </summary>
        public Field(Class parent)
            : base(parent)
        {
            Name = "";
            this.column = new Column(this);
        }

        internal Field(XmlNode fieldNode, Class parent)
            : base(fieldNode, parent)
        {
            Name = fieldNode.Attributes["Name"].Value;
            if (fieldNode.Attributes["ColumnName"] != null)
            {
                this.column = new Column(this);
                this.column.Name = fieldNode.Attributes["ColumnName"].Value;
                if (null != fieldNode.Attributes["ColumnType"])
                    column.DbType = fieldNode.Attributes["ColumnType"].Value;
                else
                    column.DbType = null;
                if (null != fieldNode.Attributes["ColumnLength"])
                    column.Size = int.Parse(fieldNode.Attributes["ColumnLength"].Value);
                else
                    column.Size = 0;
                if (null != fieldNode.Attributes["IgnoreLengthInDDL"])
                    column.IgnoreColumnSizeInDDL = bool.Parse(fieldNode.Attributes["IgnoreLengthInDDL"].Value);
                else
                    column.IgnoreColumnSizeInDDL = false;
                if (null != fieldNode.Attributes["ColumnPrecision"])
                    column.Precision = int.Parse(fieldNode.Attributes["ColumnPrecision"].Value);
                else
                    column.Precision = 0;
                if (null != fieldNode.Attributes["AllowDbNull"])
                    column.AllowDbNull = bool.Parse(fieldNode.Attributes["AllowDbNull"].Value);
                else
                    column.AllowDbNull = true;
            }
            else
            {
                // Field->Class->NDOMapping
                XmlNode columnNode = fieldNode.SelectSingleNode(parent.Parent.selectColumn);
                this.column = new Column(columnNode, this);
            }
        }

        internal void Save(XmlNode parentNode)
        {
            XmlElement fieldNode = parentNode.OwnerDocument.CreateElement("Field");
            parentNode.AppendChild(fieldNode);
            base.SaveProperties(fieldNode);
            fieldNode.SetAttribute("Name", name);
            this.column.Save(fieldNode);
#if nix
			fieldNode.SetAttribute("ColumnName", columnName);
			if (null != columnType && "" != columnType)
				fieldNode.SetAttribute("ColumnType", columnType);
			if (0 != columnLength)
				fieldNode.SetAttribute("ColumnLength", columnLength.ToString());
			if (true == ignoreLengthInDDL)
				fieldNode.SetAttribute("IgnoreLengthInDDL", "True");
			if (0 != columnPrecision)
				fieldNode.SetAttribute("ColumnPrecision", columnPrecision.ToString());
            if (!this.allowDbNull)
                fieldNode.SetAttribute("AllowDbNull", "False");
#endif
        }
        #endregion

        #region IComparable Member

        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(this.Name, ((Field)obj).Name);
        }

        #endregion
    }


    /// <summary>
    /// This class encapsulates a oid mapping
    /// </summary>
    /// <remarks>This class is equivalent to the Oid element of the mapping file schema.</remarks>
    public class ClassOid : MappingNode, IFieldInitializer
    {
        ArrayList oidColumns = new ArrayList();
        /// <summary>
        /// The column descriptions for the oid
        /// </summary>
        [Browsable (false)]
        public ArrayList OidColumns
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


#if nix
		/// <summary>
		/// Name of the column which holds the oid in the database.
		/// </summary>
		[Description("Name of the column which holds the oid in the database.")]
		public string ColumnName
		{
			get { return columnName; }
			set { columnName = value; this.Changed = true; }
		}
		private string columnName = "";

		/// <summary>
		/// If the oid isn't generated by the database, this member determines a persistent field which holds the self generated oid.
		/// </summary>		
		/// <remarks>
		/// Note that the FieldName attribute isn't necessary, if the IdGenerationHandler of the PersistenceManager is used.
		/// </remarks>
		[Description("Determines a persistent field which holds a self generated oid.")]
		public string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; this.Changed = true; }
		}
		private string fieldName = null; // Wenn Felder selbst generiert werden

		/// <summary>
		/// Optional field name of a list relation, which defines the first part of a MultiKey oid.
		/// </summary>
		/// <remarks>
		/// A MultiKey oid is used to map classes to table without an own primary key. 
		/// Normally this tables are intermediate tables for relations with additional data.
		/// </remarks>
		[Description("Optional; the field name of a list relation, which defines the first part of a MultiKey oid.")]
		public string ParentRelation
		{
			get { return parentRelation; }
			set { parentRelation = value; this.Changed = true; }
		}
		private string parentRelation = null;

		/// <summary>
		/// Optional field name of a list relation, which defines the second part of a MultiKey oid.
		/// </summary>
		/// <remarks>
		/// A MultiKey oid is used to map classes to table without an own primary key. 
		/// Normally this tables are intermediate tables for relations with additional data.
		/// </remarks>
		[Description("Optional; the field name of an element relation, which defines the second part of a MultiKey oid.")]
		public string ChildRelation
		{
			get { return childRelation; }
			set { childRelation = value; this.Changed = true; }
		}
		private string childRelation = null;

		/// <summary>
		/// This member is for use of the framework only. It will be initialized as parameter for IPersistenceHandler functions.
		/// </summary>
		private Type fieldType = null;  // Wird von Class.InitFields gesetzt

        /// <summary>
		/// This is for use of the NDO framework only. It will only be initialized, if passed to the IPersistenceHandler.
		/// </summary>
		[Browsable(false)]
		public Type FieldType {
			get { return fieldType; }
			set { fieldType = value;}
		}

#endif
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
#if nix
			oidNode.SetAttribute("ColumnName", ColumnName);
			if (fieldName != null)
				oidNode.SetAttribute("FieldName", fieldName);
			
			if (parentRelation != null)
				oidNode.SetAttribute("ParentRelation", parentRelation);

			if (childRelation != null)
				oidNode.SetAttribute("ChildRelation", childRelation);
#endif
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
        public IList Relations
        {
            get
            {
                ArrayList result = new ArrayList();
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
            Hashtable myPersistentFields = fieldMap.PersistentFields;
            ArrayList relationsReady = new ArrayList();

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
                        ArrayList allOidColumns = new ArrayList();
                        foreach (OidColumn oidc in this.oidColumns)
                        {
                            if (oidc.RelationName == column.RelationName)
                                allOidColumns.Add(oidc);
                        }

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


    /// <summary>
    /// Properties of this type can be assigned to any mapping element.
    /// Use the NDOProperty class to retrieve the binary representation of the
    /// property value in the given type.
    /// </summary>
    public class Property : MappingNode
    {
        string name;
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        [ReadOnly(true), Description("The name of the property.")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.Changed = true;
            }
        }

        string type;
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <remarks>
        /// Use the NDOProperty class to convert the value and type strings to a value of the given type.
        /// NDOProperty can read and write IConvertible objects automatically. All other types
        /// need to define a TypeConverter derivate and be marked with the TypeConverterAttribute.
        /// </remarks>
        [Description("The type of the property.")]
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                this.Changed = true;
            }
        }
        string value;
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        /// <remarks>
        /// Use the NDOProperty class to convert the value and type strings to a value of the given type.
        /// NDOProperty can read and write IConvertible objects automatically. All other types
        /// need to define a TypeConverter derivate and be marked with the TypeConverterAttribute.
        /// </remarks>
        [Description("The property value.")]
        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                this.Changed = true;
            }
        }

        /// <summary>
        /// Creates a Property object.
        /// </summary>
        /// <param name="parent"></param>
        public Property(MappingNode parent)
            : base(parent)
        {
            name = "";
            value = "";
            type = "System.String, mscorlib";
        }

        /// <summary>
        /// Creates a new Property object and sets the values.
        /// </summary>
        /// <param name="parent">The mapping tree object, to which the property is assigned.</param>
        /// <param name="name">The Property name.</param>
        /// <param name="type">The type of the Property.</param>
        /// <param name="value">The Property value.</param>
        public Property(MappingNode parent, string name, string type, string value)
            : base(parent)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }


        internal Property(XmlNode propertyNode, MappingNode parent)
            : base(parent)
        {
            this.name = propertyNode.Attributes["Name"].Value;
            this.value = propertyNode.Attributes["Value"].Value;
            if (propertyNode.Attributes["DotNetType"] == null)
            {
                this.type = "System.String, mscorlib";
            }
            else
            {
                this.type = propertyNode.Attributes["DotNetType"].Value;
            }
        }

        internal void Save(XmlNode parentNode)
        {
            XmlElement propElement = parentNode.OwnerDocument.CreateElement("Property");
            parentNode.AppendChild(propElement);
            propElement.SetAttribute("Name", this.Name);
            propElement.SetAttribute("Value", this.value);
            if (type != "System.String" && !type.StartsWith("System.String,"))
                propElement.SetAttribute("DotNetType", this.type);
        }

        /// <summary>
        /// Removes the Property from the Properties list of the parent object.
        /// </summary>
        public override void Remove()
        {
            nodeParent.RemoveProperty(this.name);
        }
    }



    /// <summary>
    /// This class encapsulates a foreign key column. It's a column with some
    /// additional data.
    /// </summary>
    public class ForeignKeyColumn : Column
    {
        #region Constructors and Save Method

        /// <summary>
        /// Constructs a Column element and assigns it to the given parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        public ForeignKeyColumn(MappingNode parent)
            : base(parent)
        {
        }


        internal ForeignKeyColumn(XmlNode columnNode, MappingNode parent)
            : base(columnNode, parent)
        {
        }

        internal override void Save(XmlNode parentNode)
        {
            XmlElement fkColumnNode = parentNode.OwnerDocument.CreateElement("ForeignKeyColumn");
            parentNode.AppendChild(fkColumnNode);
            base.SaveOwnNode(fkColumnNode);  // Saves properties too
        }
        #endregion

        /// <summary>
        /// Removes the ForeignKeyColumn object from the list of ForeignKeyColumns in the parent object.
        /// </summary>
        public override void Remove()
        {
            Relation r = Parent as Relation;
            if (r != null)
            {
                r.ForeignKeyColumns.Remove(this);
                return;
            }
            MappingTable mt = Parent as MappingTable;
            if (mt != null)
            {
                mt.ChildForeignKeyColumns.Remove(this);
            }
        }
    }

    /// <summary>
    /// This class encapsulates an Oid column. It contains some more data than its 
    /// base class Column.
    /// </summary>
    public class OidColumn : Column
    {
        #region State variables and accessors
        string fieldName;
        /// <summary>
        /// This property is used to map a oid column to a field. Leave this property blank, if the oid column doesn't have a mapping to a field.
        /// </summary>
        [Description("This property is used to map a oid column to a field. Leave this property blank, if the oid column doesn't have a mapping to a field.")]
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

        string relationName;
        /// <summary>
        /// This property is used to map a oid column to a relation. Leave this property blank, if the oid column doesn't have a mapping to a relation.
        /// </summary>
        [Description("This property is used to map a oid column to a relation. Leave this property blank, if the oid column doesn't have a mapping to a relation.")]
        public string RelationName
        {
            get { return relationName; }
            set { relationName = value; }
        }

        bool autoIncremented;
        /// <summary>
        /// Gets or sets a value, which determines if the column is autoincremented.
        /// </summary>
        [Description("This property determines if the column is autoincremented.")]
        public bool AutoIncremented
        {
            get { return autoIncremented; }
            set { autoIncremented = value; this.Changed = true; }
        }

        int autoIncrementStart = 1;
        /// <summary>
        /// If a column is autoincremented, this determines the start value. If the value not set NDO assumes 1.
        /// </summary>
        [Description("If a column is autoincremented, this determines the start value. If the value not set NDO assumes 1.")]
        public int AutoIncrementStart
        {
            get { return autoIncrementStart; }
            set { autoIncrementStart = value; this.Changed = true; }
        }

        int autoIncrementStep = 1;
        /// <summary>
        /// If a column is autoincremented, this determines the step value. If the value not set NDO assumes 1.
        /// </summary>
        [Description("If a column is autoincremented, this determines the step value. If the value not set NDO assumes 1.")]
        public int AutoIncrementStep
        {
            get { return autoIncrementStep; }
            set { autoIncrementStep = value; this.Changed = true; }
        }


        /// <summary>
        /// Parent class of relation
        /// </summary>
        [Browsable(false)]
        public new ClassOid Parent
        {
            get { return nodeParent as ClassOid; }
        }

        /// <summary>
        /// Removes the OidColumn from the OidColumn list of the parent object.
        /// </summary>
        public override void Remove()
        {
            Parent.OidColumns.Remove(this);
        }

        [Browsable(false)]
        internal int TypeLength
        {
            get
            {
                Type fieldType = this.SystemType;
                if (fieldType == typeof(Int64))
                    return 8;
                if (fieldType == typeof(Int32))
                    return 4;
                if (fieldType == typeof(string))
                    return 255;
                // OidColumn->Oid->Class->NDOMapping
                NDOMapping m = this.Parent.Parent.Parent;
                IProvider p = m.GetProvider(this.Parent.Parent);
                if (p.SupportsNativeGuidType)
                    return 16;
                else
                    return 36;
            }
        }


        [Browsable(false)]
        internal Relation Relation
        {
            get
            {
                if (this.RelationName == null)
                    return null;
                return Parent.Parent.FindRelation(this.relationName);
            }
        }


        #endregion

        #region Constructors and Save Method

        /// <summary>
        /// Constructs a Column element and assigns it to the given parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        public OidColumn(ClassOid parent)
            : base(parent)
        {
            base.AllowDbNull = false;
        }

        internal OidColumn(XmlNode columnNode, ClassOid parent)
            : base(columnNode, parent)
        {

            if (null != columnNode.Attributes["FieldName"])
                this.fieldName = columnNode.Attributes["FieldName"].Value;
            else
                this.fieldName = null;
            if (null != columnNode.Attributes["RelationName"])
                this.relationName = columnNode.Attributes["RelationName"].Value;
            else
                this.relationName = null;

            if (null != columnNode.Attributes["AutoIncremented"])
                this.autoIncremented = bool.Parse(columnNode.Attributes["AutoIncremented"].Value);
            else
                this.autoIncremented = false;

            if (this.autoIncremented)
            {
                if (null != columnNode.Attributes["AutoIncrementStart"])
                    this.autoIncrementStart = int.Parse(columnNode.Attributes["AutoincrementStart"].Value);
                //else // the value is defaulted to 1
                //    this.autoIncrementStart = 1;

                if (null != columnNode.Attributes["AutoIncrementStep"])
                    this.autoIncrementStep = int.Parse(columnNode.Attributes["AutoincrementStep"].Value);
                //else // the value is defaulted to 1
                //    this.autoIncrementStep = 1;
            }

        }

        internal override void Save(XmlNode parentNode)
        {
            XmlElement oidColumnNode = parentNode.OwnerDocument.CreateElement("OidColumn");
            parentNode.AppendChild(oidColumnNode);
            if (null != this.fieldName && "" != this.fieldName)
                oidColumnNode.SetAttribute("FieldName", this.fieldName);
            if (null != this.relationName && "" != this.relationName)
                oidColumnNode.SetAttribute("RelationName", this.relationName);

            if (this.autoIncremented)
            {
                oidColumnNode.SetAttribute("AutoIncremented", "True");
                if (this.autoIncrementStart != 1)
                    oidColumnNode.SetAttribute("AutoIncrementStart", this.autoIncrementStart.ToString());
                if (this.autoIncrementStep != 1)
                    oidColumnNode.SetAttribute("AutoIncrementStep", this.autoIncrementStep.ToString());
            }

            base.SaveOwnNode(oidColumnNode);  // Saves properties too
        }
        #endregion
    }

    /// <summary>
    /// This class encapsulates a column mapping.
    /// </summary>
    /// <remarks>This class is equivalent to the Column element of the mapping file schema.</remarks>
    public class Column : MappingNode
    {
        #region State variables and accessors

        string name;
        /// <summary>
        /// Gets or sets the name of the database column.
        /// </summary>
        [Description("Name of the database column.")]
        public string Name
        {
            get { return name; }
            set { name = value; this.Changed = true; }
        }

        string netType;
        /// <summary>
        /// In certain situations (i.e. Oid's) this tells NDO, which .NET type should be used to store members of this column. Leave this blank, if you don't have to deal with Oid's or Type columns.
        /// </summary>
        [Description("In certain situations (i.e. Oid's) this tells NDO, which .NET type should be used to store members of this column. Leave this blank, if you don't have to deal with Oid's or Type columns.")]
        public string NetType
        {
            get { return netType; }
            set { netType = value; this.Changed = true; }
        }

        Type systemType;
        /// <summary>
        /// Returns the .NET type according to the NetType property.
        /// </summary>
        [Browsable(false)]
        public virtual Type SystemType
        {
            get
            {
                if (this.systemType == null)
                {
                    if (this.netType == null)
                        return null;
                    this.systemType = Type.GetType(this.netType);
                }
                return this.systemType;
            }
            set
            {
                this.systemType = value;
                // don't adjust netType, because that would conflict with the enhancer.
            }
        }

        string dbType;
        /// <summary>
        /// Use this property, if you need another database type, than the one automatically assigned by the NDO provider.
        /// </summary>
        [Description("Use this property, if you need another database type, than the one automatically assigned by the NDO provider.")]
        public string DbType
        {
            get { return dbType; }
            set { dbType = value; this.Changed = true; }
        }

        int size;
        /// <summary>
        /// Gets or sets the size of the column.
        /// </summary>
        [Description("Size of the column.")]
        public int Size
        {
            get { return size; }
            set { size = value; this.Changed = true; }
        }
        int precision;
        /// <summary>
        /// Precision of the column. This appears in the DDL as (Size, Precision) tupel.
        /// </summary>
        [Description("Precision of the column. This appears in the DDL as (Size, Precision) tupel.")]
        public int Precision
        {
            get { return precision; }
            set { precision = value; this.Changed = true; }
        }

        private bool ignoreColumnSizeInDDL;
        /// <summary>
        /// True, if the DDL generator should not generate Size and Precision values for the given column.
        /// </summary>
        [Description("True, if the DDL generator should not generate Size and Precision values for the given column.")]
        public bool IgnoreColumnSizeInDDL
        {
            get { return ignoreColumnSizeInDDL; }
            set { ignoreColumnSizeInDDL = value; this.Changed = true; }
        }

        bool allowDbNull;

        /// <summary>
        /// Determines, if the column may contain NULL values.
        /// </summary>
        /// <remarks>
        /// The default value of this property is True. 
        /// NDO uses this property, to set the corresponding parameter in the DDL scripts. 
        /// NDO doesn't check the actual values passed as parameters. If you disallow null
        /// values for a certain column, make sure that the corresponding fields in your application
        /// don't contain null values before an object is going to be saved.
        /// </remarks>
        [Description("Determines, if the column may contain NULL values.")]
        public bool AllowDbNull
        {
            get { return allowDbNull; }
            set { allowDbNull = value; this.Changed = true; }
        }


        /// <summary>
        /// Returns the parent node of the Column object. The type of the parent depends on the type of the Column object.
        /// </summary>
        [Browsable(false)]
        public MappingNode Parent
        {
            get { return nodeParent; }
        }

        /// <summary>
        /// Calls to this method will be executed by the overrides in the derived classes.
        /// </summary>
        public override void Remove()
        {
            throw new InternalException(2994, "Column.Remove should be called by override. Type: " + this.GetType().FullName);
        }

        #endregion

        #region Constructors and Save Method

        /// <summary>
        /// Constructs a Column element and assigns it to the given parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        public Column(MappingNode parent)
            : base(parent)
        {
            size = 0;
            precision = 0;
            ignoreColumnSizeInDDL = false;
            allowDbNull = true;
        }

        internal Column(XmlNode columnNode, MappingNode parent)
            : base(columnNode, parent)
        {
            if (null != columnNode.Attributes["Name"])
                this.name = columnNode.Attributes["Name"].Value;
            else
                this.name = null;

            if (null != columnNode.Attributes["NetType"])
                this.netType = columnNode.Attributes["NetType"].Value;
            else
                this.netType = null;

            if (null != columnNode.Attributes["DbType"])
                this.dbType = columnNode.Attributes["DbType"].Value;
            else
                this.dbType = null;

            if (null != columnNode.Attributes["Size"])
                this.size = int.Parse(columnNode.Attributes["Size"].Value);
            else
                this.size = 0;

            if (null != columnNode.Attributes["Precision"])
                this.precision = int.Parse(columnNode.Attributes["Precision"].Value);
            else
                this.precision = 0;

            if (null != columnNode.Attributes["IgnoreColumnSizeInDDL"])
                ignoreColumnSizeInDDL = bool.Parse(columnNode.Attributes["IgnoreColumnSizeInDDL"].Value);
            else
                ignoreColumnSizeInDDL = false;

            if (null != columnNode.Attributes["AllowDbNull"])
                this.allowDbNull = bool.Parse(columnNode.Attributes["AllowDbNull"].Value);
            else
                this.allowDbNull = true;
        }

        internal void SaveOwnNode(XmlElement columnNode)
        {
            if (!string.IsNullOrEmpty(this.name))
                columnNode.SetAttribute("Name", this.name);
            if (null != netType && "" != netType)
                columnNode.SetAttribute("NetType", this.netType);
            if (null != dbType && "" != dbType)
                columnNode.SetAttribute("DbType", this.dbType);
            if (0 != this.size)
                columnNode.SetAttribute("Size", this.size.ToString());
            if (0 != precision)
                columnNode.SetAttribute("Precision", this.precision.ToString());
            if (this.ignoreColumnSizeInDDL)
                columnNode.SetAttribute("IgnoreColumnSizeInDDL", "True");
            if (!this.allowDbNull)
                columnNode.SetAttribute("AllowDbNull", "False");
        }

        internal virtual void Save(XmlNode parentNode)
        {
            XmlElement columnNode = parentNode.OwnerDocument.CreateElement("Column");
            parentNode.AppendChild(columnNode);

            base.SaveProperties(columnNode);   // Save properties
            SaveOwnNode(columnNode);
        }

        #endregion

        public override string ToString()
        {
            return this.name;
        }
    }


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
        public IList Fields
        {
            get { return fields; }
        }
        private IList fields = new ArrayList();
        /// <summary>
        /// All relation mappings of the class. See <see cref="NDO.Mapping.Relation"/>
        /// </summary>
        [Browsable(false)]
        public IList Relations
        {
            get { return relations; }
        }
        private IList relations = new ArrayList();

        /// <summary>
        /// A list of <code>Class</code> objects for all subclasses of this class.
        /// This is for use of the NDO framework only. It will be initialized, if passed to an IPersistenceHandler.
        /// </summary>
        private IList subclasses = new ArrayList();
        [Browsable(false)]
        public IList Subclasses
        {
            get { return subclasses; }
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
            get { return nodeParent as NDOMapping; }
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
            this.nodeParent = parent;
        }

        /// <summary>
        /// For use by the NDO framework only. It will be initialized if the Class object is passed to the IPersistenceHandler.
        /// </summary>
        public bool IsAbstract = false;  // true for abstract classes and interfaces

        private string[] myFields;  // wird in InitFields angelegt
        private ArrayList myEmbeddedTypes; // wird in InitFields angelegt
        internal int RelationOrdinalBase = -1;  // wird in InitFields angelegt
        internal IList FKColumnNames;  // InitFields - collects all foreign key column names used in LoadState


        private Column typeNameColumn = null;
        [ReadOnly(true)]
        public Column TypeNameColumn
        {
            get { return typeNameColumn; }
        }

        #endregion

        #region FieldMap stuff
        /// <summary>
        /// For use by the NDO framework only. It will be initialized if the Class object is passed to the IPersistenceHandler.
        /// </summary>
        [Browsable(false)]
        public string[] FieldNames
        {
            get { return myFields; }
        }

        /// <summary>
        /// For use by the NDO framework only. The list be initialized if the Class object is passed to the IPersistenceHandler.
        /// </summary>
        [Browsable(false)]
        public ArrayList EmbeddedTypes
        {
            get { return myEmbeddedTypes; }
        }

        /// <summary>
        /// Gets a hashtable, which contains all mappable fields. The key is the field name, the value is the FieldInfo structure.
        /// </summary>
        [Browsable(false)]
        public Hashtable PersistentFields
        {
            get { return new FieldMap(this, false).PersistentFields; }
        }

        /// <summary>
        /// Gets a list of FieldInfo entries, which reflects all relations of a class and its subclasses.
        /// </summary>
        [Browsable(false)]
        public IList RelationInfos
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
            if (classNode.Attributes["TimeStampColumn"] != null)
                this.timeStampColumn = classNode.Attributes["TimeStampColumn"].Value;

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
#if STD
			((ArrayList)Fields).Sort();
#endif
            foreach (Field f in Fields)
                f.Save(newNode);

            newNode = doc.CreateElement("Relations");
            classNode.AppendChild(newNode);
#if STD
			((ArrayList)Relations).Sort();
#endif
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
                this.RelationOrdinalBase += cl.Relations.Count;
                baseType = cl.SystemType.BaseType;
            }
        }


        void IFieldInitializer.InitFields()
        {
            systemType = Type.GetType(FullName + ", " + AssemblyName);
            if (SystemType == null) throw new NDOException(22, "Can't load type: " + FullName + ", " + AssemblyName);
            IsAbstract = SystemType.IsAbstract;
#if  PRO
			if(!IsAbstract) 
				AddToSuperClass(this);
#endif
            FieldMap fieldMap = new FieldMap(this);
            myFields = fieldMap.Fields;
            myEmbeddedTypes = fieldMap.EmbeddedTypes;
            ((IFieldInitializer)this.oid).InitFields();
        }

        internal void CollectForeignKeyNames()
        {
            if (this.oid.IsDependent)
            {
                string relationName = string.Empty;
                int j = 0;
                new OidColumnIterator(this.oid).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
                {
                    Relation rel = null;
                    if (oidColumn.RelationName != relationName)
                    {
                        relationName = oidColumn.RelationName;
                        rel = oidColumn.Relation;
                        j = 0;
                    }
                    oidColumn.SystemType = ((OidColumn)Parent.FindClass(rel.ReferencedType).Oid.OidColumns[j]).SystemType;
                    oidColumn.Size = this.Provider.GetDefaultLength(oidColumn.SystemType);
                    oidColumn.Name = ((ForeignKeyColumn)rel.ForeignKeyColumns[j]).Name;
                    j++;
                });


            }
            RelationCollector rc = new RelationCollector(this);
            rc.CollectRelations();
            IList fkc = rc.ForeignKeyColumns;
            if (fkc.Count > 0)
                this.FKColumnNames = fkc;
            else
                this.FKColumnNames = null;
        }


#if PRO
		private void AddSubClass(Class c) {
			if(!Subclasses.Contains(c)) {
				Subclasses.Add(c);
			}
			AddToSuperClass(c);
		}

		private void AddToSuperClass(Class c) {
			if (SystemType == null)
				systemType = Type.GetType(this.FullName + ", " + this.AssemblyName);
			Class baseClass = Parent.FindClass(SystemType.BaseType.FullName);
			if(baseClass != null) {
				baseClass.AddSubClass(c);
			}
			foreach(Type i in SystemType.GetInterfaces()) {
				Class interfaceClass = Parent.FindClass(i.FullName);
				if(interfaceClass != null) {
					interfaceClass.AddSubClass(c);
				}
			}
		}
#endif
        #endregion

        #region Find Functions

        /// <summary>
        /// Find a field mapping.
        /// </summary>
        /// <param name="fieldName">The name of the field</param>
        /// <returns>A <code>Field</code> object</returns>
        public Field FindField(string fieldName)
        {
            foreach (Field f in Fields)
            {
                if (f.Name == fieldName)
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
                if (r.FieldName == fieldName)
                    return r;
            }
            return null;
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
            Fields.Add(f);

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
        /// Adds a default relation mapping.
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="ReferencedTypeName">Type name of the referenced class</param>
        /// <param name="is1to1">True, if multiplicity is 1</param>
        /// <param name="relationName">Optional relation name</param>
        /// <param name="ownTypeIsPoly">True, if the class, containing the field, has a persistent base class</param>
        /// <param name="otherTypeIsPoly">True, if the related type has a persistent base class</param>
        /// <returns>A new constructed <code>Relation</code> object</returns>
        public Relation AddStandardRelation(string fieldName, string ReferencedTypeName, bool is1to1, string relationName, bool ownTypeIsPoly, bool otherTypeIsPoly)
        {
            //			if (null != Parent)
            //				Parent.this.Changed = true;

            Relation r = new Relation(this);
            r.FieldName = fieldName;
            r.ReferencedTypeName = ReferencedTypeName;
            //r.parent = this;
            r.RelationName = relationName;
            r.Multiplicity = is1to1 ? RelationMultiplicity.Element : RelationMultiplicity.List;

            int pos = ReferencedTypeName.LastIndexOf('.');
            string refShortName = ReferencedTypeName.Substring(pos + 1);
            refShortName = refShortName.Replace("`", string.Empty);

            pos = this.FullName.LastIndexOf('.');
            string myShortName = this.FullName.Substring(pos + 1);
            myShortName = myShortName.Replace("`", string.Empty);

            Relation foreignRelation = r.ForeignRelation;

            ForeignKeyColumn fkColumn = r.NewForeignKeyColumn();

            // Element->x?
            if (is1to1
                && !(otherTypeIsPoly && r.Multiplicity == RelationMultiplicity.List)
                && !(foreignRelation != null && foreignRelation.MappingTable != null))
            {
                r.MappingTable = null;
                // Foreign Key is in the own table and points to rows of another table
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
            else
            {
                // Liste->x
                // Foreign Key points to rows of our own table
                fkColumn.Name = "ID" + myShortName;
                if (ownTypeIsPoly)
                    r.ForeignKeyTypeColumnName = "TC" + myShortName;

                if (relationName != string.Empty)
                {
                    fkColumn.Name += "_" + relationName;
                    if (ownTypeIsPoly)
                        r.ForeignKeyTypeColumnName += "_" + relationName;
                }


                if (null != foreignRelation && foreignRelation.Multiplicity == RelationMultiplicity.List
                    ||
                    (/*ownTypeIsPoly || */otherTypeIsPoly) && r.Multiplicity != RelationMultiplicity.Element
                    ||
                    foreignRelation != null && foreignRelation.MappingTable != null
                    )
                {
                    AddMappingTable(r, refShortName, myShortName, otherTypeIsPoly);
                    if (foreignRelation != null)
                    {
                        AddMappingTable(foreignRelation, myShortName, refShortName, ownTypeIsPoly);
                        string frFkcName = "ID" + refShortName;
                        string frFtcName = null;
                        if (otherTypeIsPoly)
                            frFtcName = "TC" + refShortName;
                        if (relationName != string.Empty)
                        {
                            frFkcName += "_" + relationName;
                            if (otherTypeIsPoly)
                                frFtcName += "_" + relationName;
                        }
                        ForeignKeyColumn forFkColumn = (ForeignKeyColumn)foreignRelation.ForeignKeyColumns[0];
                        forFkColumn.Name = frFkcName;
                        foreignRelation.MappingTable.ChildForeignKeyTypeColumnName = frFtcName;
                    }
                }
                else r.MappingTable = null;
            }

            Relations.Add(r);

            return r;
        }


        private void AddMappingTable(Relation r, string typeShortName1, string typeShortName2, bool otherTypeIsPoly)
        {
            r.MappingTable = new MappingTable(r);
            ForeignKeyColumn fkColumn = r.MappingTable.NewForeignKeyColumn();
            fkColumn.Name = "ID" + typeShortName1;
            if (otherTypeIsPoly)
                r.MappingTable.ChildForeignKeyTypeColumnName = "TC" + typeShortName1;
            if (r.RelationName != null && r.RelationName != string.Empty)
            {
                fkColumn.Name += "_" + r.RelationName;
                if (otherTypeIsPoly)
                    r.MappingTable.ChildForeignKeyTypeColumnName += "_" + r.RelationName;
            }

            if (typeShortName1.CompareTo(typeShortName2) < 0)
                r.MappingTable.TableName = "rel" + typeShortName1 + typeShortName2;
            else
                r.MappingTable.TableName = "rel" + typeShortName2 + typeShortName1;
            r.MappingTable.ConnectionId = ((Connection)Parent.Connections[0]).ID;
        }
        #endregion

        #region IComparable Member

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
            Parent.Classes.Remove(this);
        }

    }

}
