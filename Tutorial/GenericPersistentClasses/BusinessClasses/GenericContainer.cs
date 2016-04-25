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
