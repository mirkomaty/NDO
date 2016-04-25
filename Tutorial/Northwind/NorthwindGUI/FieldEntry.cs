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
using System.Reflection;
using NDO.Mapping;

namespace NorthwindGUI
{
    class FieldEntry
    {
        public static List<FieldEntry> GetFieldEntries(Type t, NDOMapping mapping)
        {
            List<FieldEntry> result = new List<FieldEntry>();
            Class cl = mapping.FindClass(t);
            if (cl == null)
                throw new Exception("Class mapping for type " + t.FullName + " not found.");
            foreach (Field f in cl.Fields)
            {
                FieldInfo fi = t.GetField(f.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fi != null && fi.FieldType != typeof(byte[]))
                {
                    result.Add(new FieldEntry(fi, f));
                }
            }
            return result;
        }

        FieldEntry(FieldInfo fi, Field f)
        {
            this.fieldInfo = fi;
            this.fieldMapping = f;
        }

        Field fieldMapping;
        public Field FieldMapping
        {
            get { return fieldMapping; }
            set { fieldMapping = value; }
        }
        FieldInfo fieldInfo;
        public FieldInfo FieldInfo
        {
            get { return fieldInfo; }
            set { fieldInfo = value; }
        }
        object value;
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

    }
}
