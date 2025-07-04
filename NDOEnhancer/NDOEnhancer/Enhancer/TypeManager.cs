﻿//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.IO;
using System.Xml.Serialization;
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
		private Dictionary<int,Class> types = new Dictionary<int,Class>();
		/// <summary>
		/// Map from type to id.
		/// </summary>
        private Dictionary<Class,int> ids = new Dictionary<Class,int>();

		private string filename;
        private NDOMapping mapping;

		public TypeManager(string filename, NDOMapping mapping) 
		{
            this.mapping = mapping;
            this.filename = filename;
		}


		public void CheckTypeList(ClassDictionary<ClassNode> allTypes)
		{
			foreach(var entry in allTypes)
			{
				ClassNode classNode = (ClassNode) entry.Value;
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
		}


		private void CheckAndAddType(string typeFullName, string assName)
		{
            Class cls = this.mapping.FindClass(typeFullName);
            
            if (cls == null)
                throw new Exception( $"Can't find class {typeFullName} in the mapping file" );
            
            if (ids.ContainsKey(cls))  // we have already a type code.
                return;

			if(cls != null) 
			{
                // We make sure, that a type of a given name has always the same id.
                // Therefore we compute a Hash Code from the type name.
                // In the rare case, that two types have the same HashCode, we must decline from
                // this rule.
                int newId = TypeHashGenerator.GetHash(typeFullName);
                
                while (0 == newId || types.ContainsKey(newId))
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
                
                foreach (var de in types)
                    arr[i++] = (Class)de.Value;

                return arr;
            }
        }
	}
}
