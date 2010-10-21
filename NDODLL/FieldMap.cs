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


using System;
using System.Reflection;
using System.Collections;

namespace NDO.Mapping
{
	/// <summary>
	/// Collects field information for persistent types. This class is used by the NDO Enhancer.
	/// </summary>
	public class FieldMap
	{
		Class cl;
        Type type;
		ArrayList myFields;
		ArrayList myEmbeddedTypes;
		ArrayList myRelations;

		bool checkIfMappingsExist = true;

		// used by NDOPersistenceHandler
		Hashtable myPersistentFields;

        /// <summary>
        /// Gets all persistent fields.
        /// </summary>
		public Hashtable PersistentFields
		{
			get 
			{ 
				GenerateFields();
				return myPersistentFields; 
			}
		}

        /// <summary>
        /// Gets all Embedded Types.
        /// </summary>
		public ArrayList EmbeddedTypes
		{
			get { return myEmbeddedTypes; }
		}


        /// <summary>
        /// Constructor used by NDO and the Enhancer. No sanity check.
        /// </summary>
        /// <param name="cl"></param>
		public FieldMap(Class cl)
		{
			this.cl = cl;
            this.type = cl.SystemType;
		}

		/// <summary>
		/// Constructor used by NDO.
		/// </summary>
		/// <param name="cl"></param>
		/// <param name="checkIfMappingsExist"></param>
        public FieldMap(Class cl, bool checkIfMappingsExist) : this(cl)
		{
			this.checkIfMappingsExist = checkIfMappingsExist;
		}

        /// <summary>
        /// Constructor used by NDO.
        /// </summary>
        /// <param name="t"></param>
        public FieldMap(Type t)
        {
            this.type = t;
            this.cl = null;
            this.checkIfMappingsExist = false;
        }

		private void AddValueType(FieldInfo parent)
		{
			Type t = parent.FieldType;

			if (t.FullName.IndexOf("+QueryHelper+") > -1)
				return;

			ArrayList publicFields = new ArrayList();
			ArrayList publicProps = new ArrayList();

			PropertyInfo[] mis = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach(PropertyInfo mi in mis)
			{
				if (mi.CanRead && mi.CanWrite && StorableTypes.Contains(mi.PropertyType))
				{
					publicProps.Add(mi);
				}
			}
			FieldInfo[] fis = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach(FieldInfo fi in fis)
			{
				if (StorableTypes.Contains(fi.FieldType))
				{
					publicFields.Add(fi);
				}
			}
			if (publicProps.Count > 0 || publicFields.Count > 0)
			{
				foreach (PropertyInfo pi in publicProps)
				{
					myFields.Add (parent.Name + "." + pi.Name);
                    if (!myPersistentFields.Contains(parent.Name + "." + pi.Name))
					    myPersistentFields.Add(parent.Name + "." + pi.Name, pi);
				}
				foreach (FieldInfo fi in publicFields)
				{
					myFields.Add (parent.Name + "." + fi.Name);
                    if (!myPersistentFields.Contains(parent.Name + "." + fi.Name))
                        myPersistentFields.Add(parent.Name + "." + fi.Name, fi);
				}
			}
		}


		private void AddEmbeddedType(FieldInfo parent)
		{
			ArrayList publicFields = new ArrayList();
			
			FieldInfo[] fis = parent.FieldType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach(FieldInfo fi in fis)
			{
				if (fi.Name.StartsWith("_ndo"))
					continue;
				Type t = fi.FieldType;
				object[] attributes = fi.GetCustomAttributes(typeof(NDOTransientAttribute), false);
				if (attributes.Length > 0)
					continue;
				if (StorableTypes.Contains(t))
				{
					string name = parent.Name + "." + fi.Name;
					myEmbeddedTypes.Add(name);
                    if (!myPersistentFields.Contains(name))
					    myPersistentFields.Add(name, fi);
				}
			}
		}

		private bool IsPersistentType(Type t)
		{
            return t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length > 0;
		}

		private void AddFields(Type t)
		{
			int startind = myFields.Count;
			if (!IsPersistentType(t))
				return;
			FieldInfo[] finfos = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach(FieldInfo fi in finfos)
			{
				string fname;
				if ((fname = fi.Name).StartsWith("_ndo"))
					continue;

				if (!fi.IsPrivate)
					continue;

				Type ft = fi.FieldType;

				// Typen, die nicht speicherbar sind.
				if (ft.IsInterface)
					continue;

				object[] attributes = fi.GetCustomAttributes(false);
				bool cont = false;
				foreach (System.Attribute attr in attributes)
				{
					string name;
					if ((name = attr.GetType().Name) == "NDOTransientAttribute")
						cont = true;
					if (name == "NDORelationAttribute")
						cont = true;
				}
				if (cont)
					continue;

				if (StorableTypes.Contains(ft))
				{
					this.myFields.Add(fname);
                    if (!myPersistentFields.Contains(fname))
                        myPersistentFields.Add(fname, fi);
				}
				else if (ft.IsValueType)
					AddValueType(fi);
				else if (ft.IsClass)
				{
					AddEmbeddedType(fi);
				}
				// Alle anderen Fälle werden ignoriert
			}
			// Pro Klasse werden die Strings sortiert
			// Sortiert wird ohne Länderberücksichtigung, sonst sortiert die
			// Anwendung auf norwegischen Systemen anders als auf deutschen.
			if (myFields.Count - startind > 0)
			{
				FieldSorter fs = new FieldSorter(); 
				myFields.Sort(startind, myFields.Count - startind, fs);
			}
		}

		private class FieldSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				if (!(x is string) || !(y is string))
					throw new ArgumentException("FieldSorter.Compare: String parameter expected");
				return String.CompareOrdinal((string) x, (string) y);
			}
		}		

		private void GenerateFields()
		{
			myPersistentFields = new Hashtable();
			myFields = new ArrayList();
			this.myEmbeddedTypes = new ArrayList();
			AddFields(this.type);
			Type t = this.type.BaseType;
            int persCount = t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length;
			while (persCount > 0)
			{
				AddFields(t);				
				t = t.BaseType;
                persCount = t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length;
			}
			// Jetzt stehen alle Felder in myFields

			if (checkIfMappingsExist && !this.type.IsAbstract)
			{
				for (int i = 0; i < myFields.Count; i++)
				{
					NDO.Mapping.Field field;
					if ((field = cl.FindField((string)myFields[i])) != null)
						myFields[i] = field.Column.Name;
					else
						throw new NDOException(7, "Can't find mapping information for field " + cl.FullName + "." + myFields[i]);
				}
			}
		}

        /// <summary>
        /// Gets all field names.
        /// </summary>
		public string[] Fields
		{
			get 
			{
				// Wir suchen alle Fields, für die ein Mapping existieren muss.
				// wir sortieren sie nach Field-Namen.
				// Dann ermitteln wir die Mapping-Namen, die dann ins Array an die gleiche
				// Stelle eingetragen werden.
				GenerateFields();
				string[] newArr = new string[myFields.Count];
				int j = 0;
				foreach(string s in myFields)
					newArr[j++] = s;
				return newArr;
			} // get
		} // Fields

		private void AddRelations(Type t)
		{
			FieldInfo[] fieldInfos = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach(FieldInfo fi in fieldInfos)
			{
				if (fi.GetCustomAttributes(typeof(NDORelationAttribute), false).Length > 0)
					myRelations.Add(fi);
			}
		}

		private void GenerateRelations()
		{
			myRelations = new ArrayList();
			AddRelations(this.type);
			Type t = this.type.BaseType;
            int persCount = t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length;
			while (persCount > 0)
			{
				if (t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length > 0)
					AddRelations(t);				
				t = t.BaseType;
                persCount = t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length;
			}
			// Jetzt stehen alle Relations in myRelations

			if (checkIfMappingsExist && !this.type.IsAbstract)
			{
				for (int i = 0; i < myRelations.Count; i++)
				{
					if (cl.FindRelation(((FieldInfo)myRelations[i]).Name) == null)
						throw new NDOException(8, "Can't find mapping information for relation " + cl.FullName + "." + myRelations[i] + ".");
				}
			}
		}


		/// <summary>
		/// Gets all Relations of a class and its persistent subclasses.
		/// </summary>
		public IList Relations
		{
			get 
			{ 
				if (myRelations == null)
					GenerateRelations();
				return myRelations; 
			}
		}


	}  // class FieldMap
}  // Namespace
