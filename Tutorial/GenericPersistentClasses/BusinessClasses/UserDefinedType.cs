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
