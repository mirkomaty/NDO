﻿//
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
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NDO.Mapping {
	/// <summary>
	/// This class handles the mapping from types to integer ids.
	/// </summary>
	internal class TypeManager {

		/// <summary>
		/// Map from id to type.
		/// </summary>
		private Hashtable types;
		/// <summary>
		/// Map from type to id.
		/// </summary>
		private Hashtable ids;

		private string filename;  // backwards compatibility
        NDOMapping mapping;

		public TypeManager(string filename, NDOMapping mapping) 
		{
			this.filename = filename;
            this.mapping = mapping;
		}

		public bool Contains(Type t)
		{
			if(t == null) 
				throw new InternalException(55, "TypeManager: Null-Argument in this[Type]");

			Load();

            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                t = t.GetGenericTypeDefinition();

            return ids.Contains(t);
		}

		public bool Contains(int id)
		{
			return types.Contains(id);
		}

		public int this[Type t] {
			get 
			{ 
				if(t == null)
					throw new InternalException(55, "TypeManager: Null-Argument in this[Type]");

				Load();

                if (t.IsGenericType && !t.IsGenericTypeDefinition)
                    t = t.GetGenericTypeDefinition();

				if(!ids.Contains(t)) 
					throw new NDOException(94, "No Type Code for " + t.FullName + ". Check your Type Codes in the Mapping File");

				return (int)ids[t];
			}
		}

		public Type this[int id] {
			get 
			{
				Load();
				return (Type)types[id]; 
			}
		}

		void Load() 
		{
			if (ids != null)
				return;

			types = new Hashtable();
			ids = new Hashtable();

            foreach (Class cls in this.mapping.Classes)
            {
                if (cls.TypeCode != 0)
                {
                    Type t = cls.SystemType;
                    if (t == null)
                    {
                        t = Type.GetType(cls.FullName + "," + cls.AssemblyName);
                    }
                    if (t == null)
                        continue;
                    types[cls.TypeCode] = t;
                    ids[t] = cls.TypeCode;
                }
            }

            if (types.Count > 0)
                return;

            // If the mapping file doesn't contain Type Codes, maybe
            // there exists a NDOTypes.xml file describing the types.
            // Recompile your project to make sure to have the Type Codes in the mapping file.
			FileInfo fi = new FileInfo(filename);
            if (fi.Exists)
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(NDOTypeMapping));
                    using (FileStream fs =
                               fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        NDOTypeMapping typeMapping = (NDOTypeMapping)xs.Deserialize(fs);
                        if (typeMapping.TypeDescriptor != null)
                        {
                            foreach (NDOTypeDescriptor d in typeMapping.TypeDescriptor)
                            {
                                Type t = Type.GetType(d.TypeName + ", " + d.AssemblyName);
                                if (t == null)
                                    continue;
                                types[d.TypeId] = t;
                                ids[t] = d.TypeId;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
		}

		//private string GetAssemblyName(string fullName)
		//{
		//	int p = fullName.IndexOf(',');
		//	if (p == -1)
		//		throw new NDOException(95, String.Format("Invalid Assembly Fullname: {0}", fullName));
		//	return fullName.Substring(0, p);
		//}

		///// <summary>
		///// 
		///// </summary>
		//public void Store() {
		//	FileInfo fi = new FileInfo(filename);
		//	XmlSerializer xs = new XmlSerializer(typeof(NDOTypeMapping));

		//	NDOTypeMapping m = new NDOTypeMapping();
		//	m.TypeDescriptor = new NDOTypeDescriptor[types.Count];
		//	int index = 0;
		//	foreach(DictionaryEntry entry in types) {
		//		Type t = (Type)entry.Value;
		//		NDOTypeDescriptor d = new NDOTypeDescriptor();
		//		d.AssemblyName = GetAssemblyName(t.Assembly.FullName);
		//		d.TypeName = t.FullName;
		//		d.TypeId = (int)entry.Key;
		//		m.TypeDescriptor[index++] = d;
		//	}
		//	using (FileStream fs =
		//			   fi.Open(FileMode.Create, FileAccess.Write)) {
		//		xs.Serialize(fs, m);
		//	}
		//}

	}
}
