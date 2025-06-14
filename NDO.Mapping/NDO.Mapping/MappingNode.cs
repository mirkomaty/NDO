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


using System.Collections;
using System.ComponentModel;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// Base class for mapping
    /// </summary>
    public abstract class MappingNode
    {
        MappingNode nodeParent;
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
		/// Returns the parent of the node.
		/// </summary>
		/// <remarks>Note, that the concrete MappingNode classes may have a specialiced Parent property.</remarks>
		[Browsable(false)]
		public MappingNode NodeParent
		{
			get { return this.nodeParent; }
			set { this.nodeParent = value; }
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

        /// <summary>
        /// Constructs a MappingNode
        /// </summary>
        /// <param name="parent"></param>
        public MappingNode(MappingNode parent)
        {
            this.nodeParent = parent;
        }

        /// <summary>
        /// Constructs a MappingNode
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        public MappingNode(XmlNode node, MappingNode parent)
        {
            this.nodeParent = parent;
            LoadProperties(node);
        }

        /// <summary>
        /// Reads additional properties from the xml code
        /// </summary>
        /// <param name="node"></param>
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
}