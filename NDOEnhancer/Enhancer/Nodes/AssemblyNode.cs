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
using System.Text;
using System.Collections;
using System.Reflection;
using NDO;
using NDO.Mapping;

namespace NDOEnhancer
{
	/// <summary>
	/// Represents some properties of assemblies.
	/// </summary>
	internal class AssemblyNode
	{

		NDOMapping mappings;
		
		bool isEnhanced;
		public bool IsEnhanced
		{
			get { return isEnhanced; }
		}
		string oidTypeName;
		public string OidTypeName
		{
			get { return oidTypeName; }
		}
		Type oidType;
		public Type OidType
		{
			get { return oidType; }
		}

		ArrayList persistentClasses = new ArrayList();
		public ArrayList PersistentClasses
		{
			get { return persistentClasses; }
		}

		string shortName;
		public string ShortName
		{
			get { return shortName; }
		}

		string fullName;
		public string FullName
		{
			get { return fullName; }
		}


	
		ArrayList analyzedTypes = new ArrayList();

		private void AnalyzeType(Type t)
		{
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
                return;
			string tname = t.FullName;
			if (analyzedTypes.Contains(tname))
				return;
			analyzedTypes.Add(tname);
			Type ifc = t.GetInterface("NDO.IPersistenceCapable");
			if (t.IsPublic && t.IsInterface && IsPersistentType(t))
			{
				this.persistentClasses.Add(new ClassNode(t, mappings));
			}			
			if ((ifc != null || IsPersistentType(t)) 
				&& t.IsClass 
				&& (t.IsPublic || (t.MemberType & MemberTypes.NestedType) != 0))
			{
				this.persistentClasses.Add(new ClassNode(t, mappings));
				if (t.BaseType.FullName != "System.Object")
					AnalyzeType(t.BaseType);
				this.AnalyzeTypes(t.GetNestedTypes());
			}
		}

		private bool IsPersistentType(Type t)
		{
			return t.GetCustomAttributes(typeof(NDOPersistentAttribute), false).Length > 0;
		}


		private void AnalyzeTypes(Type[] types)
		{
			foreach (Type t in types)
			{
				AnalyzeType(t);
			}
		}

		public AssemblyNode(Assembly ass, NDOMapping mappings)
		{
			this.mappings = mappings;
			this.shortName = ass.FullName.Substring(0, ass.FullName.IndexOf(','));
			string dllName = ass.Location;

            try
            {
                Type[] theTypes = ass.GetTypes();
            }
            catch (ReflectionTypeLoadException tlex)
            {
                string s = string.Empty;
                foreach (Exception ex in tlex.LoaderExceptions)
                {
                    s += ex.Message + "\r\n";
                }
                throw new Exception("AssemblyNode: ReflectionTypeLoadException occured: " + s);
            }
			AnalyzeTypes(ass.GetTypes());
			this.fullName = ass.FullName;
			object[] attrs = ass.GetCustomAttributes(false);
			this.isEnhanced = (HasEnhancedAttribute(attrs));
			this.oidTypeName = GetOidTypeNameFromAttributes(attrs);
		}

		private string GetOidTypeNameFromAttributes(object[] attributes)
		{
			foreach (System.Attribute attr in attributes)
			{
				NDOOidTypeAttribute oidAttr = attr as NDOOidTypeAttribute;
				if (oidAttr != null)
				{
					this.oidType = oidAttr.OidType;
					return oidType.FullName;
				}
			}
			return null;
		}

		private bool HasEnhancedAttribute(object[] attributes)
		{
			foreach (System.Attribute attr in attributes)
			{
				if (attr is NDOEnhancedAttribute)
				{
					return true;
				}
			}			
			return false;
		}

	}
}
