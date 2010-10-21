//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
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
// there is a commercial licence available at www.netdataobjects.com.
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
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using System.Text;

namespace NDO
{
	/// <summary>
	/// This class is a generic way to handle Name/Value pairs and to store it in Xml Documents.
	/// </summary>
	/// <remarks>
	/// The type of the value will be stored in the Xml document.  
	/// NDO can read and write IConvertible objects automatically. All other types
	/// need to define a TypeConverter derivate and be marked with the TypeConverterAttribute.
	/// </remarks>
	public class NDOProperty
	{
		string name;
		/// <summary>
		/// The name of the property.
		/// </summary>
		[ReadOnly(true)]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}


		public static readonly string[] BuiltInTypes = new string[] 
		{
			"System.Boolean",
			"System.Char",
			"System.SByte",
			"System.Byte",
			"System.Int16",
			"System.UInt16",
			"System.Int32",
			"System.UInt32",
			"System.Int64",
			"System.UInt64",
			"System.Single",
			"System.Double",
			"System.Decimal",
			"System.DateTime",
			"System.String"
		};

		
		object value;
		/// <summary>
		/// The property value.
		/// </summary>
		public object Value
		{
			get { return value; }
			set { this.value = value; }
		}

		/// <summary>
		/// Constructs a new Property instance.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		public NDOProperty(string name, object value)
		{
			this.name = name;
			this.value = value;
		}

        /// <summary>
        /// Constructs a new Property instance.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="valString">String representation of the value of the property.</param>
        /// <param name="typeString">String representation of the type of the property.</param>
        public NDOProperty(string name, string valString, string typeString)
        {
            this.name = name;
            Type t = Type.GetType(typeString);
            if (t == null)
                throw new ArgumentException("Unknown Type '" + typeString + "'", "typeString");
            SetValueFromString(valString, Type.GetType(typeString));
        }

		/// <summary>
		/// Constructs a new Property from a string in the same format, as it is delivered by ToString().
		/// </summary>
		/// <param name="s">The string to parse.</param>
		/// <remarks>
		/// The property string consists of three parts separated by a '|' character.
		/// The first part is the name of the property, the second part the string representation of the value, 
		/// and the thirt part is the string representation of the type of the value.
		/// </remarks>
		public NDOProperty(string s)
		{
			string[] arr = s.Split('|');
			if (arr.Length != 3)
				throw new ArgumentException("NDOProperty ctor: Wrong input format. The string must contain a name, a value, and a type, separated with a '|' character.", "s");
			this.name = arr[0];
			SetValueFromString(arr[1], Type.GetType(arr[2]));
		}
		
		/// <summary>
		/// Constructs a new Property instance from a Xml document.
		/// </summary>
		/// <param name="propertyNode">The node representing the Property.</param>
		public NDOProperty(XmlNode propertyNode)
		{
			this.name = propertyNode.Attributes["Name"].Value;
			string val = propertyNode.Attributes["Value"].Value;
			if (propertyNode.Attributes["DotNetType"] != null)
			{
				this.value = val;
			}
			else
			{
				this.SetValueFromString(val, Type.GetType(propertyNode.Attributes["DotNetType"].Value));
			}
		}

		/// <summary>
		/// Saves a property into a Xml document.
		/// </summary>
		/// <param name="parentNode"></param>
		public void Save(XmlNode parentNode)
		{
			XmlElement element = parentNode.OwnerDocument.CreateElement("Property");
			parentNode.AppendChild(element);
			element.SetAttribute("Name", this.name);
			if (this.value is string)
				element.SetAttribute("Value", (string) this.value);
			else
			{
				element.SetAttribute("Value", this.ValueString);
				Type t = this.value.GetType();

				element.SetAttribute("DotNetType", this.TypeString); 
			}
		}

		/// <summary>
		/// Sets the value of the property to the value represented by the valueString. 
		/// The string will be converted to the type given by t.
		/// </summary>
		/// <param name="valueString">The string representing the property value.</param>
		/// <param name="t">The type of the property value.</param>
		public void SetValueFromString(string valueString, Type t)
		{
            this.value = GenericFieldConverter.FromString(valueString, t);
        }

		/// <summary>
		/// Converts the property value to a string.
		/// </summary>
		/// <returns>A string representing the property value.</returns>
		[Browsable(false)]
		public string ValueString
		{
			get
			{
				if (this.value == null)
					throw new NullReferenceException("The value of the NDOProperty is null");
                return GenericFieldConverter.ToString(this.value);
			}
		}

		/// <summary>
		/// Gets a string representation of the type of the value.
		/// </summary>
		public string TypeString
		{
			get
			{
				if (this.value == null)
					throw new NullReferenceException("The value of the NDOProperty is null");
				Type t = this.value.GetType();
				return t.FullName + ", " + t.Assembly.GetName().Name;
			}
		}

		/// <summary>
		/// Gets a string representation of the property.
		/// </summary>
		/// <returns>A result string, representing the property.</returns>
		/// <remarks>
		/// The result string can be used by the Property constructor Property(string), 
		/// to rebuild the Property object.
		/// </remarks>
		public override string ToString()
		{
			if (this.name == null)
				throw new NullReferenceException("The name of the NDOProperty is null");
			return this.name + '|' + this.ValueString + '|' + this.TypeString;
		}


	}
}
