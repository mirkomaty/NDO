using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NETDataObjects.NDOVSPackage
{
    /// <summary>
    /// This is a small version of NDOMapping to get the pieces of mapping information necessary for configuration
    /// </summary>
    class NDOMapping
    {
        XElement ndoMappingElement;
        string schemaVersion = string.Empty;
        List<Connection> connections;
        string fileName;

        public NDOMapping(string fileName)
        {
            this.fileName = fileName;
            ndoMappingElement = XElement.Load(fileName);
            XAttribute schemaAttribute;
            if ((schemaAttribute = ndoMappingElement.Attribute("SchemaVersion")) != null)
                this.schemaVersion = schemaAttribute.Value;
        }

        public IEnumerable<Connection> Connections
        {
            get { return connections; }
            set { connections = value.ToList(); }
        }

        public string SchemaVersion
        {
            get
            {
                return this.schemaVersion;
            }
            set
            {
                this.schemaVersion = value;
            }
        }

        public Connection NewConnection(string name, string type)
        {
            XElement connectionElement = new XElement("Connection",
                new XAttribute("Name", name),
                new XAttribute("Type", type),
                new XAttribute("ID", "C0")
            );
            this.ndoMappingElement.Add(connectionElement);
            Connection result = new Connection(connectionElement);
            this.connections.Add(result);
            return result;
        }

        public void Save()
        {
            this.ndoMappingElement.Attribute("SchemaVersion").Value = this.schemaVersion;
            foreach (var conn in this.connections)
            {
                conn.Save();
            }
            this.ndoMappingElement.Save(this.fileName);
        }
    }
}
