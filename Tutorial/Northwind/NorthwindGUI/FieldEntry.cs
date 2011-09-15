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
