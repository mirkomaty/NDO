//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
        List<Connection> connections = new List<Connection>();
        string fileName;

        public NDOMapping(string fileName)
        {
            this.fileName = fileName;
            this.ndoMappingElement = XElement.Load(fileName);
            XAttribute schemaAttribute;
            if ((schemaAttribute = ndoMappingElement.Attribute("SchemaVersion")) != null)
                this.schemaVersion = schemaAttribute.Value;
			XElement connectionsElement = this.ndoMappingElement.Element( "Connections" );
			if (connectionsElement != null)
			{
				foreach (var connectionElement in connectionsElement.Elements("Connection"))
				{
					this.connections.Add( new Connection( connectionElement ) );
				}
			}
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
			XElement connectionsElement = this.ndoMappingElement.Element( "Connections" );
			if (connectionsElement == null)
			{
				connectionsElement = new XElement( "Connections" );
				this.ndoMappingElement.AddFirst( connectionsElement );
			}
            connectionsElement.Add(connectionElement);
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
