using NDO.Mapping.Attributes;
using NDOInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace NDO.Mapping
{
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
        public IEnumerable<Connection> Connections
        {
            get { return connections; }
            set { connections = value.ToList(); }
        }
        private IList<Connection> connections = new List<Connection>();
        /// <summary>
        /// Collection of all class mappings in the mapping file.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Class> Classes
        {
            get { return classes.Values; }
        }

        private Dictionary<string,Class> classes = new Dictionary<string,Class>();


        /// <summary>
        /// For NDO internal use.
        /// </summary>
        const string NDONamespace = "http://www.netdataobjects.de/NDOMapping";
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
                AddClass(new Class(classNode, this));
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

            foreach (Class cl in Classes.OrderBy(c=>c.FullName))
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
            if (this.connections.Count > 0)
                return;

            Connection c = new Connection(this);
            c.ID = "C0";
            c.FromStandardConnection();
            this.connections.Add(c);
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
                foreach (Class c2 in classes.Values)
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

            if (connections.Count == 0)
                AddStandardConnection();
            c.ConnectionId = this.connections[0].ID;
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
            this.classes.Add(c.FullName,c);
        }


        public void RemoveClass(Class c)
        {
            if (this.classes.ContainsKey(c.FullName))
            {
                this.classes.Remove(c.FullName);
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
            this.connections.Add(c);
        }

        /// <summary>
        /// Removes a connection from the Connections list.
        /// </summary>
        /// <param name="c"></param>
        public void RemoveConnection(Connection c)
        {
            this.Changed = true;
            this.connections.Remove(c);
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
            foreach (Connection oldc in this.connections)
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
        public virtual Class FindClass(Class cls)
        {
            return this.classes[cls.FullName];
        }

        /// <summary>
        /// Find a Class object for the specified class.
        /// </summary>
        /// <param name="fullName">Fully qualified name of the class.</param>
        /// <returns>mapping info about the class</returns>
        public virtual Class FindClass(string fullName)
        {
            return this.classes[fullName];
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
			var r1ForeignKeyColumns = r1.ForeignKeyColumns.ToList();
			var r2ForeignKeyColumns = r2.ForeignKeyColumns.ToList();
            if (r1ForeignKeyColumns.Count != r2ForeignKeyColumns.Count)
                return true;

            for (int i = 0; i < r1ForeignKeyColumns.Count; i++)
            {
                ForeignKeyColumn fkc1 = (ForeignKeyColumn)r1ForeignKeyColumns[i];
                ForeignKeyColumn fkc2 = (ForeignKeyColumn)r2ForeignKeyColumns[i];
                if (ColumnsAreDifferent(fkc1, fkc2)) return true;
            }
            return false;
        }

        private bool ForeignKeyColumnsAreDifferent(MappingTable t1, MappingTable t2)
        {
			var t1ChildForeignKeyColumns = t1.ChildForeignKeyColumns.ToList();
			var t2ChildForeignKeyColumns = t2.ChildForeignKeyColumns.ToList();
            if (t1ChildForeignKeyColumns.Count != t2ChildForeignKeyColumns.Count)
                return true;

            for (int i = 0; i < t1ChildForeignKeyColumns.Count; i++)
            {
                ForeignKeyColumn fkc1 = t1ChildForeignKeyColumns[i];
                ForeignKeyColumn fkc2 = t2ChildForeignKeyColumns[i];
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
			var c1Fields = c1.Fields.ToList();
			var c2Fields = c2.Fields.ToList();
			var c1Relations = c1.Relations.ToList();
			var c2Relations = c2.Relations.ToList();

            if (c1Fields.Count != c2Fields.Count) return true;
            if (c1Relations.Count != c2Relations.Count) return true;
            if (c1.AssemblyName != c2.AssemblyName) return true;
            if (c1.TableName != c2.TableName) return true;
            if (OidsAreDifferent(c1.Oid, c2.Oid)) return true;

            foreach (Field f1 in c1Fields)
            {
                Field f2 = c2.FindField(f1.Name);
                if (null == f2) return true;
                if (FieldsAreDifferent(f1, f2)) return true;
            }
            foreach (Relation r1 in c1Relations)
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

            if (this.connections.Count > 0 && mergeMapping.connections.Count > 0)
            {
                Connection std1 = this.connections[0] as Connection;
                Connection std2 = mergeMapping.connections[0] as Connection;
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
                if (this.connections.Count == 1 && ((Connection)this.connections[0]).Name == Connection.DummyConnectionString)
                {
                    ((Connection)this.connections[0]).Name = Connection.StandardConnection.Name;
                    ((Connection)this.connections[0]).Type = Connection.StandardConnection.Type;
                }
                if (mergeMapping.connections.Count == 1 && ((Connection)mergeMapping.connections[0]).Name == Connection.DummyConnectionString)
                {
                    ((Connection)mergeMapping.connections[0]).Name = Connection.StandardConnection.Name;
                    ((Connection)mergeMapping.connections[0]).Type = Connection.StandardConnection.Type;
                }
            }


            foreach (Connection co in mergeMapping.Connections)
            {
                if (co.Name == Connection.DummyConnectionString)
                {
                    co.Name = Connection.StandardConnection.Name;
                    co.Type = Connection.StandardConnection.Type;
                }

                IEnumerable<Class> classesToCopy =  mergeMapping.Classes.Where(c=>c.ConnectionId == co.ID);

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
                    foreach (Class cl in Classes)
                    {
                        if (cl.FullName == cl3.FullName)
                        {
                            if (ClassesAreDifferent(cl, cl3))
                                classToRemove = cl;
                            classFound = true;
                            break;
                        }
                    }

                    if (classFound)
                    {
                        if (classToRemove != null)
                        {
                            RemoveClass(classToRemove);
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
}
