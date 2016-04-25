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


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NETDataObjects.NDOVSPackage
{
    /// <summary>
    /// This class encapsulates a connection string and makes it available with a short ID.
    /// </summary>
    /// <remarks>This class is equivalent to the Connection element in the mapping file schema.</remarks>
    public class Connection
    {
        public const string DummyConnectionString = "Substitute this string with your connection string";
        XElement xElement;

        private string iD = "";
        /// <summary>
        /// Gets or sets the ID, with which the connection is referenced in the Class objects.
        /// </summary>
        [Description("The ID, with which the connection is referenced in the Class objects.")]
        public string ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private string name = "";
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [Description("The connection string.")]
        public string Name
        {
            get { return name; }
            set { name = value; }
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
            set { type = value; }
        }

        /// <summary>
        /// Construct a Connection object based on a Connection node of the mapping file.
        /// </summary>
        /// <param name="connNode">Connection node</param>
        /// <param name="parent">An object of type NDOMapping, which is the parent of the connection object.</param>
        internal Connection(XElement connNode)            
        {
            this.xElement = connNode;
            name = connNode.Attribute("Name").Value;
            iD = connNode.Attribute("ID").Value;
            type = connNode.Attribute("Type").Value;
            if (String.IsNullOrEmpty( name ))
            {
                name = DummyConnectionString;
            }
            if (String.IsNullOrEmpty(type))
            {
                type = "SqlServer";
            }
            if (String.IsNullOrEmpty(iD))
            {
                iD = "C0";
            }
        }

        /// <summary>
        /// Saves the Connection object as a Connection node. Don't use that funktion directly, 
        /// it will be called by the Save function of the NDOMapping class.
        /// </summary>
        /// <param name="parentNode">The parent node</param>		
        internal void Save()
        {
            xElement.Attribute("ID").Value = iD.ToString();
            xElement.Attribute("Name").Value = name;
            xElement.Attribute("Type").Value = type;            
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
