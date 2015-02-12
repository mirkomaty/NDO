using System.Collections;
using System.ComponentModel;
using System.Xml;

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
}