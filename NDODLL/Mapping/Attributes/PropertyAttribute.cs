//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.de.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


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

        public PropertyAttribute(string name, Type type, string value)
        {
            this.name = name;
            this.type = type.FullName + "," + type.Assembly.GetName().Name;
            this.value = value;
        }

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        string value;
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public Property CreateProperty(MappingNode parent)
        {
            Property prop = new Property(parent, name, type, value);
            return prop;
        }
    }
}
