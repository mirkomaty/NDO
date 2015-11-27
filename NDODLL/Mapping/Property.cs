using System;
using System.ComponentModel;
using System.Xml;

namespace NDO.Mapping
{
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
}
