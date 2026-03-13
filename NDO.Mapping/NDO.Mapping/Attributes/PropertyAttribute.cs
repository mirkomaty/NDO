//
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
using System.Text;
using NDO.Mapping;

namespace NDO.Mapping.Attributes
{
    /// <summary>
    /// This is used to make sure, that a certain property appears in the mapping file.
    /// </summary>
    /// <remarks>
    /// The following sample assigns two string properties, one for the Class entry for <c>MyType</c> 
    /// and one for the Field entry for the field <c>name</c>.
    /// [Property("Xpath", "System.String,mscorlib", "//PersistentTypes/MyType")]
    /// [NDOPersistent]
    /// public class MyType
    /// {
    ///     [Property("Wrapper", "System.String,mscorlib", "Name")]
    ///     string name;
    ///     public string Name
    ///     {
    ///         get { return name; }
    ///         set { name = value; }
    ///     }
    /// }
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class PropertyAttribute : Attribute
    {
        /// <summary>
        /// Constructs a PropertyAttribute object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public PropertyAttribute(string name, string type, string value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }

        /// <summary>
        /// Constructs a PropertyAttribute object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public PropertyAttribute(string name, Type type, string value)
        {
            this.name = name;
            this.type = type.FullName + "," + type.Assembly.GetName().Name;
            this.value = value;
        }

        string name;

        /// <summary>
        /// Gets or sets the Name of the property
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string type;
        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        string value;
        /// <summary>
        /// Gets or sets the Value of the property
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Property CreateProperty(MappingNode parent)
        {
            Property prop = new Property(parent, name, type, value);
            return prop;
        }
    }
}
