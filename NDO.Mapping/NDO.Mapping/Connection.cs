﻿//
// Copyright (c) 2002-2024 Mirko Matytschak 
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// <summary>
        /// 
        /// </summary>
        public const string DummyConnectionString = "Substitute this string with your connection string";

        static Connection standardConnection;
        
        /// <summary>
        /// Creates a new, empty Connection object.
        /// </summary>
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

        /// <summary>
        /// This is used to log the connection without password.
        /// </summary>
		[Browsable( false )]
		public string DisplayName
		{
			get
			{
				Regex regex = new Regex( "password=[^;]*" );
				return regex.Replace( this.name, "password=***" );				
			}
		}

		private string type = "Sql";
        /// <summary>
        /// Gets or sets the provider type string.
        /// </summary>
        /// <remarks>
        /// This string sould match the Name property of a 
        /// provider registered in the NDOProviderFactory.
        /// <seealso cref="NDOInterfaces.INDOProviderFactory"/>
        /// </remarks>
        [Description("The provider type string.")]
        public string Type
        {
            get { return type; }
            set { type = value; this.Changed = true; }
        }

        /// <summary>
        /// Gets the parent of the Connection object
        /// </summary>
        [Browsable(false)]
        public NDOMapping Parent
        {
            get { return NodeParent as NDOMapping; }
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
