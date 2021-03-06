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


#if nix
using System;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;

namespace NDOEnhancer
{

	internal class VtFieldInfo
	{
		public string Name;
		public bool IsProperty = false;
		private const string property = "Property";
		public string DataType;
		public bool IsEnum = false;

		public VtFieldInfo(XmlNode node)
		{
			Name = node.Attributes["Name"].Value;
			IsProperty = (node.Attributes["Type"].Value == property);
			DataType = node.Attributes["DataType"].Value;
			IsEnum = node.Attributes["IsEnum"] != null;
		}

		public VtFieldInfo(string name, bool isprop, string datatype)
		{
			Name = name;
			IsProperty = isprop;
			DataType = datatype;
		}
	}

	internal class ValueTypeInfo
	{
		public String FullName = null;
		public IList Fields = new ArrayList();

		public ValueTypeInfo()
		{
		}
			
		public ValueTypeInfo(XmlNode node)
		{
			FullName = node.Attributes["Name"].Value;
			XmlNodeList nodes = node.SelectNodes("Field");
			foreach(XmlNode el in nodes)
			{
				Fields.Add(new VtFieldInfo(el));
			}
		}
	}

	/// <summary>
	/// Summary description for ValueTypes.
	/// </summary>
	internal class ValueTypesx
	{
		static ValueTypesx instance; 
		private Hashtable theTypes = new Hashtable();

		private ValueTypesx()
		{
			string typesFileName = Path.Combine(Enhancer.AssemblyPath, "ValueTypes.Xml");
            //if (!File.Exists(typesFileName))
            //    throw new Exception("File not found: " + typesFileName);
			XmlDocument doc = new XmlDocument();
            if (File.Exists(typesFileName))
			    doc.Load(typesFileName);
			XmlNodeList nl = doc.SelectNodes("//ValueTypes/Type");
			foreach (XmlNode n in nl)
			{
				theTypes.Add(n.Attributes["Name"].Value, new ValueTypeInfo(n));
			}
		}

		static public void ClearInstance()
		{
			instance = null;
		}

		static public ValueTypesx Instance
		{
			get
			{
				return (instance == null) ? (instance = new ValueTypes()) : instance;
			}
		}

		internal ValueTypeInfo this[string Item]
		{
			get
			{
				return (ValueTypeInfo) theTypes[Item];
			}
		}

		internal void Add(ValueTypeInfo vti)
		{
			string name = vti.FullName;
			if (!theTypes.Contains(name))
				theTypes.Add(name, vti);
		}


		internal void Merge(XmlNodeList vtnl)
		{
			foreach (XmlNode n in vtnl)
			{
				if (!theTypes.Contains(n.Attributes["Name"].Value))
				{
					theTypes.Add(n.Attributes["Name"].Value, new ValueTypeInfo(n));
				}
			}
		}

	}
}

#endif

using System;
using System.Collections;
namespace NDOEnhancer
{

	public class ValueTypes
	{
		static ValueTypes instance = new ValueTypes();
		public static ValueTypes Instance
		{
			get { return instance; } 
		}

		// Private Constructor
		ValueTypes()
		{
		}

		Hashtable entries = new Hashtable();

		public ValueTypeNode this[string item]
		{
			get { return (ValueTypeNode) entries[item.Replace("'", string.Empty)]; }
		}

		public void Add(ValueTypeNode node)
		{
			if (!entries.Contains(node.Type.FullName))
				entries.Add(node.Type.FullName, node);
		}
	}
}