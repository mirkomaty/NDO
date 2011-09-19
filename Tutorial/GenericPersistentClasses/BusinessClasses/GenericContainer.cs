using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using NDO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace BusinessClasses
{
    public interface IProperty
    {
        object PolymorphicValue { get; }
        string Name { get; set; }
    }

    [NDOPersistent]
    public class GenericProperty<T> : IProperty
    {
        public class MyQueryHelper
        {
            string __ndoqhname;
            public MyQueryHelper()
            {
                __ndoqhname = string.Empty;
            }
            public MyQueryHelper(string n)
            {
                __ndoqhname = n;
            }
            public string name
            {
                get { return __ndoqhname + "name"; }
            }
        }
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        T value;
        public T Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public object PolymorphicValue
        {
            get { return this.value; }
        }


        public GenericProperty(string name)
        {
            this.name = name;
        }        

    }
}
