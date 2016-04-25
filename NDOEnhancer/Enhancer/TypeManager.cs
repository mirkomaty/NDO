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


#if PRO
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NDO;
using NDO.Mapping;

namespace NDOEnhancer
{
	/// <summary>
	/// This class handles the mapping from types to integer ids.
	/// </summary>
	internal class TypeManager 
	{

		/// <summary>
		/// Map from id to type.
		/// </summary>
		private Hashtable types = new Hashtable();
		/// <summary>
		/// Map from type to id.
		/// </summary>
        private Hashtable ids = new Hashtable();

		private string filename;
        private NDOMapping mapping;

		public TypeManager(string filename, NDOMapping mapping) 
		{
            this.mapping = mapping;
            this.filename = filename;
            this.Load();
		}


		public void CheckTypeList(Hashtable allTypes)
		{
			foreach(DictionaryEntry e in allTypes)
			{
				ClassNode classNode = (ClassNode) e.Value;
                if (classNode.IsAbstractOrInterface)
                    classNode.IsPoly = true;    // Must be polymorphic

				// Make sure, all types get a type code
				if (!classNode.IsAbstractOrInterface)
					CheckAndAddType(classNode.Name, classNode.AssemblyName);

				if (classNode.BaseName == null) // Keine Basisklasse angegeben
					continue;					// sollte eigentlich nicht vorkommen
				ClassNode baseNode = (ClassNode)allTypes[classNode.BaseName];
				if (baseNode == null)  // kein persistenter Typ
					continue;
				
				if (!baseNode.IsAbstractOrInterface)
					CheckAndAddType(baseNode.Name, baseNode.AssemblyName);

				baseNode.IsPoly = true;
			}
            this.Store();
		}


		private void CheckAndAddType(string typeFullName, string assName)
		{
            Class cls = this.mapping.FindClass(typeFullName);
            if (ids.Contains(cls))  // we have already a type code.
                return;
			if(cls != null) 
			{
                // We make sure, that a type of a given name has always the same id.
                // Therefore we compute a Hash Code from the type name.
                // In the rare case, that two types have the same HashCode, we must decline from
                // this rule.
                int newId = TypeHashGenerator.GetHash(typeFullName);
                while (0 == newId || types.Contains(newId))
                    newId++;
                cls.TypeCode = newId;
				types[newId] = cls;
                ids[cls] = newId;
			}
		}

        public Class[] Entries
        {
            get
            {
                Class[] arr = new Class[types.Count];
                int i = 0;
                foreach (DictionaryEntry de in types)
                    arr[i++] = (Class)de.Value;
                return arr;
            }
        }

		public void Load() 
		{
            foreach (Class cls in mapping.Classes)
            {
                if (cls.TypeCode != 0)
                {
                    this.types[cls.TypeCode] = cls;
                    ids[cls] = cls.TypeCode;
                }
            }
            if (this.types.Count == 0)
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists)
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
                                Class cls = this.mapping.FindClass(d.TypeName);
                                if (cls == null)  // NDOTypes.xml describes types which don't exist in this context
                                    continue;
                                cls.TypeCode = d.TypeId;
                                types[d.TypeId] = cls;
                                ids[cls] = d.TypeId;
                            }
                        }
                    }
                }
            }
		}

		public void Store() 
		{
            if (File.Exists(this.filename))
            {
                if (!File.Exists(this.filename + ".deprecated"))
                    File.Move(this.filename, this.filename + ".deprecated");
                else
                    File.Delete(this.filename);
            }
		}

		public void Update() 
		{
			Store();
		}
	}
}
#endif