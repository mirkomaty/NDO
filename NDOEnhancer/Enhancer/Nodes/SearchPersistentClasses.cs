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
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Xml;


namespace NDOEnhancer
{
	/// <summary>
	/// Summary description for App.
	/// </summary>
	internal class SearchPersistentClasses
	{
		string assShortName = "";
		XmlDocument doc;
		string dllName;
        MessageAdapter messages;

        public SearchPersistentClasses(MessageAdapter messages)
        {
            this.messages = messages;
        }



		private string GetTypeFullName(Type t)
		{
            NDOAssemblyName ndoAssName = new NDOAssemblyName(t.Assembly.FullName);
			string assName = t.Assembly.FullName;
			string type = t.FullName.Replace("+", "/");
			return "[" + ndoAssName.Name + "]" + type;
		}

		ArrayList analyzedTypes = new ArrayList();


		// Ist auch in NDO -> FieldMap.cs, und im Mapping Tool enthalten.
		private bool IsStorableType(Type t)
		{
			if (t == typeof(System.IntPtr))
				return false;
			return t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid) || t.IsSubclassOf(typeof(System.Enum));
		}

		private void AnalyzeValueType(Type t, XmlElement pcs, XmlDocument doc)
		{
			if (t.FullName.IndexOf("+QueryHelper+") > -1)
				return;

			ArrayList publicFields = new ArrayList();
			PropertyInfo[] mis = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach(PropertyInfo mi in mis)
			{
				if (mi.CanRead && mi.CanWrite && IsStorableType(mi.PropertyType))
				{
					publicFields.Add(mi);
				}
			}
			FieldInfo[] fis = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach(FieldInfo fi in fis)
			{
				if (IsStorableType(fi.FieldType))
				{
					publicFields.Add(fi);
				}
			}
			if (publicFields.Count > 0)
			{
				XmlElement typeNode = doc.CreateElement("ValueType");
				typeNode.SetAttribute("Name", t.FullName);
				pcs.AppendChild(typeNode);
				//Console.WriteLine(t.Name);
				foreach (MemberInfo mi in publicFields)
				{
					bool isProperty = (mi is PropertyInfo);
					XmlElement el = CreateField(mi, doc);
					el.SetAttribute("Type", isProperty ? "Property" : "Field");
					typeNode.AppendChild(el);
					//Console.WriteLine("   " + mi.Name + "/" + mi.PropertyType.Name);
				}
			}
		}
	

		private bool IsPersistentType(Type t)
		{
			object[] attributes = t.GetCustomAttributes(false);
			foreach (System.Attribute attr in attributes)
			{
				if (attr.GetType().Name == "NDOPersistentAttribute")
				{
					return true;
				}
			}			
			return false;
		}

		private bool HasEnhancedAttribute(object[] attributes)
		{
			foreach (System.Attribute attr in attributes)
			{
				if (attr.GetType().FullName == "NDO.NDOEnhancedAttribute")
				{
					return true;
				}
			}			
			return false;
		}

		private string GetOidTypeNameFromAttributes(object[] attributes)
		{
			Type at;
			foreach (System.Attribute attr in attributes)
			{
				if ((at = attr.GetType()).Name == "NDOOidTypeAttribute")
				{
					FieldInfo fi = at.GetField("a", BindingFlags.Instance | BindingFlags.NonPublic);
					if (fi == null)
						fi = at.GetField("oidType", BindingFlags.Instance | BindingFlags.NonPublic);
					if (fi != null)
					{
						Type oidtype = fi.GetValue(attr) as Type;
						if (oidtype != null)
						{
							return oidtype.FullName;
						}
					}
					else
					{
						throw new Exception("Cant't read oidType field info from type " + at.FullName );
					}
				}
			}			
			return null;
		}

		private string GetOidTypeName(Type t)
		{
			object[] attributes = t.GetCustomAttributes(false);
			return GetOidTypeNameFromAttributes(attributes);
		}

		private XmlElement CreateField(MemberInfo mi, XmlDocument doc)
		{
			bool isProperty = (mi is PropertyInfo);
			XmlElement el = doc.CreateElement("Field");
			el.SetAttribute("Name", mi.Name);
			Type memberType = isProperty ? ((PropertyInfo)mi).PropertyType : ((FieldInfo)mi).FieldType;
			el.SetAttribute("DataType", MakeElementTypeName(memberType));
			if (!isProperty)
			{
				if (mi.DeclaringType != mi.ReflectedType)
					el.SetAttribute("DeclaringType", MakeElementTypeName(mi.DeclaringType));
			}
			if (memberType.IsSubclassOf(typeof(System.Enum)))
				el.SetAttribute("IsEnum", "True");
			return el;
		}

		private void AnalyzeEmbeddedType(FieldInfo parentField, XmlNode pc, XmlDocument doc)
		{
			Type t = parentField.FieldType;
			FieldInfo[] fis = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

			XmlElement etype = doc.CreateElement("EmbeddedType");
			etype.SetAttribute("Name", parentField.Name);
			etype.SetAttribute("DataType", GetTypeFullName(t));
			if (parentField.DeclaringType != parentField.ReflectedType)
				etype.SetAttribute("DeclaringType", MakeElementTypeName(parentField.DeclaringType));
			foreach (FieldInfo fi in fis)
			{
				string fname = fi.Name;
				if (fname.StartsWith("_ndo"))
					continue;
				if (fi.FieldType.IsSubclassOf(typeof(System.Delegate)))
					continue;

				object[] attributes = fi.GetCustomAttributes(false);
				bool cont = false;
				foreach (System.Attribute attr in attributes)
				{
					string name;
					if ((name = attr.GetType().Name) == "NDOTransientAttribute")
						cont = true;
				}
				if (cont) continue;
				if (!IsStorableType(fi.FieldType))
					return;  // Resultate verwerfen

				XmlElement fn = CreateField(fi, doc);
				etype.AppendChild(fn);
			}
			pc.AppendChild(etype);
		}

		private void AnalyzeType(Type t, XmlElement pcs, XmlDocument doc)
		{
			string tname = GetTypeFullName(t);
			if (analyzedTypes.Contains(tname))
				return;
			if (t.IsPublic && t.IsInterface && IsPersistentType(t))
			{
				string oidTypeName = GetOidTypeName(t);
				XmlElement pc = doc.CreateElement("PersistentClass");
				pcs.AppendChild(pc);
				pc.SetAttribute("Name", tname);
				pc.SetAttribute("IsInterface", "true");
				if (oidTypeName != null)
					pc.SetAttribute("OidType", oidTypeName);
			}			
			if (t.IsClass && (t.IsPublic || (t.MemberType & MemberTypes.NestedType) != 0))
			{
				this.AnalyzeTypes(t.GetNestedTypes(), pcs, doc);
				// All persistent types implement IPersistenceCapable
				// In Order to not break the inheritance chain all
				// nonpersistent classes inheriting from persistent classes
				// will be listed.
				// Not enhanced Assemblies will be analyzed too, so
				// we only have NDOPersistentAttribute to check if
				// a class is persistent - inheritance chain doesn't matter in that case
				Type ifc = t.GetInterface("NDO.IPersistenceCapable");
				bool hasAttribute = IsPersistentType(t);

				if (null != ifc || hasAttribute)
				{
					string oidTypeName = GetOidTypeName(t);
					XmlElement pc = doc.CreateElement("PersistentClass");
					if (oidTypeName != null)
						pc.SetAttribute("OidType", oidTypeName);
					if (!hasAttribute)
						pc.SetAttribute("IsPersistent", "false");
					pcs.AppendChild(pc);
					pc.SetAttribute("Name", tname);
					Type basetype = t.BaseType;
					if (t.IsAbstract)
						pc.SetAttribute("IsAbstract", "true");
					pc.SetAttribute("BaseName", GetTypeFullName(basetype));
					AnalyzeFields(t, pc, doc);
					if (basetype.FullName != "System.Object")
						AnalyzeType(basetype, pcs, doc);
					analyzedTypes.Add(tname);
				}
			}
			if (t.IsValueType && !t.IsEnum)
			{
				AnalyzeValueType(t, pcs, doc);
			}
		}

		private void AnalyzeTypes(Type[] theTypes, XmlElement pcs, XmlDocument doc)
		{
			foreach (Type tt in theTypes)
			{
				AnalyzeType(tt, pcs, doc);
			}
		}

        public XmlDocument DoIt(Assembly ass)
        {
			try 
			{
				assShortName = ass.FullName.Substring(0, ass.FullName.IndexOf(','));
                dllName = ass.Location;

				this.doc = new XmlDocument();				
				XmlElement pcs = doc.CreateElement("PersistentClasses");
				doc.AppendChild(pcs);

				Type[] theTypes = ass.GetTypes();
				AnalyzeTypes(theTypes, pcs, doc);
				//pcs.SetAttribute("AssemblyName", ass.FullName.Substring(0, ass.FullName.IndexOf(',')));
				pcs.SetAttribute("AssemblyFullName", ass.FullName);
				object[] attrs = ass.GetCustomAttributes(false);
				if (HasEnhancedAttribute(attrs))
					pcs.SetAttribute("IsEnhanced", "True");
				string oidTypeName = GetOidTypeNameFromAttributes(attrs);
				if (oidTypeName != null)
				{
					pcs.SetAttribute("OidType", oidTypeName);
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				messages.ShowError("SearchPersistenClasses(" + dllName + "): " + ex);
#else
				messages.ShowError("SearchPersistenClasses(" + dllName + "): " + ex.Message);
#endif
			}

			return this.doc;
        }

#if nix
		public int DoIt(string[] args)
		{
			dllName = "";
			string xmlName = "";
			if (args.Length < 2) 
			{
				messages.ShowError("SearchPersistentClasses: " + args.Length.ToString() + " Argumente");
				sw.Flush();
				return -1;
			}
			dllName = args[0];
			xmlName = args[1];
			if (!File.Exists(dllName))
				throw new Exception("Datei '" + dllName + "' nicht gefunden.");
			
			Assembly ass = Assembly.LoadFrom(dllName);
            return DoIt(ass);
		}
#endif

		private void AnalyzeRelation(FieldInfo fi, XmlElement pc, XmlDocument doc, System.Attribute attr, IList relations)
		{
			XmlElement fn = doc.CreateElement("Relation");
			bool isElement = false;
			if (fi.FieldType.Name == "IList" || fi.FieldType.GetInterface("IList") != null)
				fn.SetAttribute("Type", "Liste");
			else
			{
				fn.SetAttribute("Type", "Element");
				isElement = true;
			}

			fn.SetAttribute("DataType", MakeElementTypeName(fi.FieldType));
			if (fi.DeclaringType != fi.ReflectedType)
				fn.SetAttribute("DeclaringType", MakeElementTypeName(fi.DeclaringType));
			fn.SetAttribute("Name", fi.Name);

			Type attrType = attr.GetType();

			string relName = string.Empty;
			string relTypeName = string.Empty;
			FieldInfo[] fields = attrType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo attrf in fields)
			{
				Type fieldType = attrf.FieldType;
				// RelationName auslesen
				if (fieldType == typeof(string))
				{
					if (null != attrf)
						relName = (string) attrf.GetValue(attr);
					fn.SetAttribute("RelationName", relName);
				}
				if (fieldType.Name == "RelationInfo")
				{
					// RelationInfo auslesen
					object relationInfo = attrf.GetValue(attr);
					FieldInfo riValue = relationInfo.GetType().GetField("value__", BindingFlags.Public | BindingFlags.Instance);
					int val = (Int16) riValue.GetValue(relationInfo);
					fn.SetAttribute("RelationInfo", val.ToString());
				}

				if (fieldType.Name == "Type")
				{
					Type theType;
					string assName;
					if (isElement)
					{
						theType = fi.FieldType;
					}
					else
					{
						theType = (Type) attrf.GetValue(attr);
					}
					if (theType != null)
					{
						assName = theType.Assembly.FullName;
						assName = assName.Substring(0, assName.IndexOf(","));
						if (assName != this.assShortName)
							relTypeName = "[" + assName + "]" + theType.FullName;
						else
							relTypeName = theType.FullName;
						fn.SetAttribute("RelatedType", relTypeName);
					}

				}

			}
			System.Diagnostics.Debug.WriteLine("Field: " + fi.Name + " Type: " + relTypeName + " Name: " + fn.Attributes["RelationName"].Value);
			System.Diagnostics.Debug.Indent();
			int nr = 1;
			bool repeat = true;
			while(repeat)
			{
				repeat = false;
				foreach(XmlElement rel in relations)
				{
					System.Diagnostics.Debug.WriteLine("Name: " + rel.Attributes["Name"].Value + " RelType: " + rel.Attributes["RelatedType"].Value + " Name: " + rel.Attributes["RelationName"].Value);
					if (rel.Attributes["RelatedType"].Value == relTypeName && rel.Attributes["RelationName"].Value == fn.Attributes["RelationName"].Value)
					{
						fn.SetAttribute("RelationName", relName + nr.ToString());
						nr++;
						repeat = true;
						break;
					}
				}
			}
			System.Diagnostics.Debug.Unindent();
			relations.Add(fn);
			pc.AppendChild(fn);
		}


		//		private bool IsStorable(FieldInfo fi)
		//		{
		//			Type t = fi.FieldType;
		//			if ( t == typeof(bool) )
		//				goto truecase;
		//			if ( t == typeof(byte) )
		//				goto truecase;
		//			if ( t == typeof(sbyte) )
		//				goto truecase;
		//			if ( t == typeof(char) )
		//				goto truecase;
		//			if ( t == typeof(short))
		//				goto truecase;
		//			if ( t == typeof(ushort))
		//				goto truecase;
		//			if ( t == typeof(int))
		//				goto truecase;
		//			if ( t == typeof(uint))
		//				goto truecase;
		//			if ( t == typeof(long))
		//				goto truecase;
		//			if ( t == typeof(System.Guid))
		//				goto truecase;
		//			if ( t == typeof(ulong))
		//				goto truecase;
		//			if ( t == typeof(float))
		//				goto truecase;
		//			if ( t == typeof(double))
		//				goto truecase;
		//			if ( t == typeof(string))
		//				goto truecase;
		//			if ( t == typeof(byte[]))
		//				goto truecase;
		//			if ( t == typeof(decimal))
		//				goto truecase;
		//			if ( t == typeof(System.DateTime))
		//				goto truecase;
		//			if ( t.IsSubclassOf(typeof(System.Enum)) )
		//				goto truecase;
		//			return false;
		//
		//			truecase:
		//				return true;
		//		}

	

		private void AnalyzeFields(Type t, XmlElement pc, XmlDocument doc)
		{
			IList relations = new ArrayList();
			FieldInfo[] fis = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo fi in fis)
			{
				string fname = fi.Name;
				if (fname.StartsWith("_ndo"))
					continue;
				if (fi.FieldType.IsSubclassOf(typeof(System.Delegate)))
					continue;

				object[] attributes = fi.GetCustomAttributes(false);
				bool cont = false;
				bool isOid = false;
				foreach (System.Attribute attr in attributes)
				{
					string name;
					if ((name = attr.GetType().Name) == "NDOTransientAttribute")
						cont = true;
					if (name == "NDORelationAttribute")
					{
						AnalyzeRelation(fi, pc, doc, attr, relations);
						cont = true;
					}
					if (name == "NDOObjectIdAttribute")
						isOid = true;
				}
				if (cont) continue;

				if (fi.FieldType.IsClass && fi.FieldType != typeof(string) && fi.FieldType != typeof(byte[]))
				{
					AnalyzeEmbeddedType(fi, pc, doc);
				}
				else
				{
					XmlElement fn = CreateField(fi, doc);
					pc.AppendChild(fn);
					if (isOid)
						fn.SetAttribute("Oid", "True");
				}
			}
		}

		private String MakeElementTypeName(Type ft)
		{
			string prefix = string.Empty;
			if (ft.IsValueType && !ft.IsPrimitive)
				prefix = "valuetype ";
			else if (ft.IsClass || ft.IsInterface)
				prefix = "class ";
			string fullName = GetTypeFullName(ft);
			string fullName2 = MakeILType(fullName);
			if (fullName2 != fullName)
				return fullName2;
			return prefix + fullName;
		}

		private string MakeILType(string typeName)
		{
			if ( typeName == "[mscorlib]System.String" )
				return "string";
			else if ( typeName == "[mscorlib]System.Int32" )
				return "int32";
			else if ( typeName == "[mscorlib]System.Boolean" )
				return "bool";
			else if ( typeName == "[mscorlib]System.Byte")
				return "unsigned int8";
			else if ( typeName == "[mscorlib]System.SByte")
				return "int8";
			else if ( typeName == "[mscorlib]System.Byte[]")
				return "unsigned int8[]";
			else if ( typeName == "[mscorlib]System.SByte" )
				return "sbyte";
			else if ( typeName == "[mscorlib]System.Char" )
				return "char";
			else if ( typeName == "[mscorlib]System.UChar" )
				return "unsigned char";
			else if ( typeName == "[mscorlib]System.Int16" )
				return "int16";
			else if ( typeName == "[mscorlib]System.UInt16" )
				return "unsigned int16";
			else if ( typeName == "[mscorlib]System.UInt32" )
				return "unsigned int32";
			else if ( typeName == "[mscorlib]System.Int64" )
				return "int64";
			else if ( typeName == "[mscorlib]System.UInt64" )
				return "unsigned int64";
			else if ( typeName == "[mscorlib]System.Single" )
				return "float32";  
			else if ( typeName == "[mscorlib]System.Double" )
				return "float64";
			else return typeName;
		}


/*
		Hashtable assemblies = new Hashtable();

		private void AdjustVersion(NDOAssemblyName an)
		{
			Version v = an.AssemblyVersion;
			Version v2 = new Version(v.Major, v.Minor);
			an.AssemblyVersion = v2;
		}

		private Assembly CheckAndLoadAssembly(string path, string argsName)
		{
			NDOAssemblyName nameToSearch = new NDOAssemblyName(argsName);
			System.Reflection.AssemblyName an = AssemblyName.GetAssemblyName(path);
			NDOAssemblyName foundName = new NDOAssemblyName(an.FullName);
			AdjustVersion(foundName);
			AdjustVersion(nameToSearch);
			if (nameToSearch.Name.ToLower() == foundName.Name.ToLower()
				&& nameToSearch.Version == foundName.Version
				&& nameToSearch.PublicKeyToken == foundName.PublicKeyToken)
			{
				Assembly ass = Assembly.LoadFrom(path);
				if (ass != null)
				{
					assemblies.Add(argsName, ass);
					return ass;
				}
			}		
			return null;
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly ass;
			if (assemblies.Contains(args.Name))
				return (Assembly) assemblies[args.Name];
			string path = Path.GetDirectoryName(dllName);
			NDOAssemblyName nameToSearch = new NDOAssemblyName(args.Name);
			string shortTestPath = Path.Combine(path, nameToSearch.Name + ".dll");
			if (File.Exists(shortTestPath))
			{
				if ((ass = CheckAndLoadAssembly(shortTestPath, 
					args.Name)) != null)
					return ass;
			}
			foreach(string s in Directory.GetFiles(path, "*.dll"))
			{
				if (s == dllName)
					continue;
				if ((ass = CheckAndLoadAssembly(s, args.Name)) != null)
					return ass;
			}
			return null;
		}
		*/
	}
}
