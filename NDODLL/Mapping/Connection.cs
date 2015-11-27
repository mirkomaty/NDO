using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace NDO.Mapping
{
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
            Parent.RemoveConnection(this);
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
            if (name == null || name == string.Empty)
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

}
