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
using System.Globalization;
using System.ComponentModel;

namespace TestApp
{
    // This is a complex type having a type converter, which is able to 
    // convert objects to/from strings.
    [TypeConverter(typeof(UserDefinedTypeConverter))]
    public class UserDefinedType
    {
        int a;
        public int A
        {
            get { return a; }
            set { a = value; }
        }
        double b;
        public double B
        {
            get { return b; }
            set { b = value; }
        }

        #region Conversion to/from string

        // You may move this code into the converter to not overburden this class
        // with your conversion code.
        public override string ToString()
        {
            // Always use InvariantCulture, since otherwise your application may break
            // if it is used in another country.
            return a.ToString() + "|" + ((IConvertible)b).ToString(CultureInfo.InvariantCulture);
        }

        public static UserDefinedType FromString(string s)
        {
            string[] arr = s.Split('|');
            if (arr.Length != 2)
                throw new Exception("UserDefinedType.FromString: Wrong input string");
            UserDefinedType result = new UserDefinedType();
            result.a = int.Parse(arr[0]);
            // Always use InvariantCulture, since otherwise your application may break
            // if it is used in another country.
            result.b = double.Parse(arr[1], CultureInfo.InvariantCulture);
            return result;
        }
        #endregion

    }


    // The type converter class. It uses the code of UserDefinedType to convert to/from strings.
    public class UserDefinedTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (!(value is string))
                throw new Exception("Can't convert " + value.GetType().FullName + " to UserDefindedType.");

            return UserDefinedType.FromString((string)value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                throw new Exception("Can't convert UserDefindedType to " + destinationType.FullName + ".");
            return ((UserDefinedType)value).ToString();
        }
    }
}
