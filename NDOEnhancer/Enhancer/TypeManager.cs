//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


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
		private int lastId;

		private string filename;
		private bool modified;

		public bool IsModified
		{
		    get { return modified; }
		}

		public TypeManager(string filename) 
		{
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

				if (classNode.BaseName == null) // Keine Basisklasse angegeben
					continue;					// sollte eigentlich nicht vorkommen
				ClassNode baseNode = (ClassNode)allTypes[classNode.BaseName];
				if (baseNode == null)  // kein persistenter Typ
					continue;
				
				if (!baseNode.IsAbstractOrInterface)
					CheckAndAddType(baseNode.Name, baseNode.AssemblyName);

				if (!classNode.IsAbstractOrInterface)
					CheckAndAddType(classNode.Name, classNode.AssemblyName);

				baseNode.IsPoly = true;
				//classNode.IsPoly = true;
			}
		}


		private void CheckAndAddType(string typeFullName, string assName)
		{
			if(!ids.Contains(typeFullName)) 
			{
				modified = true;
                // We make sure, that a type of a given name has always the same id.
                // Therefore we compute a Hash Code from the type name.
                // In the rare case, that two types have the same HashCode, we must decline from
                // this rule.
				NDOTypeDescriptor td = new NDOTypeDescriptor();
                int newId = TypeHashGenerator.GetHash(typeFullName);
                while (types.Contains(newId))
                    newId++;
				ids[typeFullName] = td.TypeId = newId;
				types[newId] = td;
				td.TypeName = typeFullName;
				td.AssemblyName = assName;
			}
		}

        public NDOTypeDescriptor[] Entries
        {
            get
            {
                NDOTypeDescriptor[] arr = new NDOTypeDescriptor[types.Count];
                int i = 0;
                foreach (DictionaryEntry de in types)
                    arr[i++] = (NDOTypeDescriptor)de.Value;
                return arr;
            }
        }

		public void Load() 
		{
			FileInfo fi = new FileInfo(filename);
			if(fi.Exists) 
			{
				XmlSerializer xs = new XmlSerializer(typeof(NDOTypeMapping));
				using (FileStream fs = 
						   fi.Open(FileMode.Open, FileAccess.Read)) 
				{
					NDOTypeMapping mapping = (NDOTypeMapping)xs.Deserialize(fs);
					if(mapping.TypeDescriptor != null) 
					{
						foreach(NDOTypeDescriptor d in mapping.TypeDescriptor) 
						{
							types[d.TypeId] = d;
							ids[d.TypeName] = d.TypeId;
							lastId = Math.Max(lastId, d.TypeId);
						}
					}
				}
			}
		}

		public void Store() 
		{
			//FileInfo fi = new FileInfo(filename);
			XmlSerializer xs = new XmlSerializer(typeof(NDOTypeMapping));

			NDOTypeMapping m = new NDOTypeMapping();
			m.TypeDescriptor = new NDOTypeDescriptor[types.Count];
			int index = 0;
			foreach(DictionaryEntry entry in types) 
			{
				NDOTypeDescriptor d = (NDOTypeDescriptor)entry.Value;
				m.TypeDescriptor[index++] = d;
			}
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				xs.Serialize(fs, m);
				fs.Close();
			}
		}

		public void Update() 
		{
			if(modified) 
			{
				Store();
				modified = false;
			}
		}
	}
}
#endif