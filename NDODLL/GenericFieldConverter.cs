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
using System.ComponentModel;
using System.Globalization;

namespace NDO
{
    /// <summary>
    /// This helper class converts strings to objects of arbitrary types and vice versa.
    /// </summary>
    public class GenericFieldConverter
    {
        /// <summary>
        /// Converts the string o to an object of type t.
        /// </summary>
        /// <param name="o">The string to convert.</param>
        /// <param name="t">The result type.</param>
        /// <returns>An object of the result type.</returns>
        /// <remarks>
        /// The type t should either be an IConvertible or have a TypeConverterAttribute, 
        /// which points to a converter type capable of converting objects of type t into a string 
        /// and vice versa.
        /// </remarks>
        public static object FromString(object o, Type t)
        {
            string valueString = (string)o;

            // First try, if we can use IConvertible
            if (typeof(IConvertible).IsAssignableFrom(t))
            {
                return ((IConvertible)valueString).ToType(t, CultureInfo.InvariantCulture);
            }

            // Now try TypeConverters
            object[] attrs = t.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            if (attrs.Length == 0)
                goto throwEx;

            for (int i = 0; i < attrs.Length; i++)
            {
                TypeConverterAttribute tca = (TypeConverterAttribute)attrs[i];
                Type convType = Type.GetType(tca.ConverterTypeName);
                TypeConverter tc = Activator.CreateInstance(convType) as TypeConverter;
                if (!tc.CanConvertFrom(typeof(string)))
                    continue;
                return tc.ConvertFrom(valueString);
            }

            throwEx:
            throw new NDOException(107, "Can't convert " + valueString.GetType().FullName + " to " + t.FullName + ". The source type has to be an IConvertible or has to provide a TypeConverter using a TypeConverterAttribute.");
        }

        /// <summary>
        /// Converts an object o of type t to a string.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>An object of the result type.</returns>
        /// <remarks>
        /// The object type should either be an IConvertible or have a TypeConverterAttribute, 
        /// which points to a converter type capable of converting the object into a string 
        /// and vice versa.
        /// </remarks>
        public static string ToString(object o)
        {
            IConvertible convertible = o as IConvertible;
            if (convertible != null)
                return convertible.ToString(CultureInfo.InvariantCulture);

            Type t = o.GetType();
            object[] attrs = t.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            if (attrs.Length == 0)
                throw new Exception("Can't convert " + t.FullName + " to a string.");
            for (int i = 0; i < attrs.Length; i++)
            {
                TypeConverterAttribute tca = (TypeConverterAttribute)attrs[i];
                Type convType = Type.GetType(tca.ConverterTypeName);
                TypeConverter tc = Activator.CreateInstance(convType) as TypeConverter;
                if (!tc.CanConvertTo(typeof(string)))
                    continue;
                return (string)tc.ConvertTo(o, typeof(string));
            }
            throw new Exception("Can't convert " + t.FullName + " to a string.");
        }
    }
}
