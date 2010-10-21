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

		private string filename;
		private bool modified;

		static TypeManager instance = null;

		public static TypeManager Instance
		{
			get { return instance; }
		}



		public TypeManager(string filename) {
			this.filename = filename;
			instance = this;
		}

		public bool Contains(Type t)
		{
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                t = t.GetGenericTypeDefinition();
            return ids.Contains(t);
		}

		public bool Contains(int id)
		{
			return types.Contains(id);
		}

		public int this[Type t] {
			get { 
				if(t == null) {
					throw new InternalException(55, "TypeManager: Null-Argument in this[Type]");
				}
                if (t.IsGenericType && !t.IsGenericTypeDefinition)
                    t = t.GetGenericTypeDefinition();

				if(!ids.Contains(t)) {
					throw new NDOException(94, "No Type Code for " + t.FullName + ". Check NDOTypes.xml");
				}
				return (int)ids[t];
			}
		}

		public Type this[int id] {
			get { return (Type)types[id]; }
		}

		public void Load() {
			types = new Hashtable();
			ids = new Hashtable();

			FileInfo fi = new FileInfo(filename);
            if (fi.Exists)
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(NDOTypeMapping));
                    using (FileStream fs =
                               fi.Open(FileMode.Open, FileAccess.Read))
                    {
                        NDOTypeMapping mapping = (NDOTypeMapping)xs.Deserialize(fs);
                        if (mapping.TypeDescriptor != null)
                        {
                            foreach (NDOTypeDescriptor d in mapping.TypeDescriptor)
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
            else
            {
                System.Diagnostics.Trace.WriteLine("NDO: No types file at " + filename);
            }
		}

		private string GetAssemblyName(string fullName)
		{
			int p = fullName.IndexOf(',');
			if (p == -1)
				throw new NDOException(95, String.Format("Invalid Assembly Fullname: {0}", fullName));
			return fullName.Substring(0, p);
		}


		public void Store() {
			FileInfo fi = new FileInfo(filename);
			XmlSerializer xs = new XmlSerializer(typeof(NDOTypeMapping));

			NDOTypeMapping m = new NDOTypeMapping();
			m.TypeDescriptor = new NDOTypeDescriptor[types.Count];
			int index = 0;
			foreach(DictionaryEntry entry in types) {
				Type t = (Type)entry.Value;
				NDOTypeDescriptor d = new NDOTypeDescriptor();
				d.AssemblyName = GetAssemblyName(t.Assembly.FullName);
				d.TypeName = t.FullName;
				d.TypeId = (int)entry.Key;
				m.TypeDescriptor[index++] = d;
			}
			using (FileStream fs =
					   fi.Open(FileMode.Create, FileAccess.Write)) {
				xs.Serialize(fs, m);
			}
		}

		public void Update() {
			if(modified) {
				Store();
				modified = false;
			}
		}
	}
}
#endif