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
