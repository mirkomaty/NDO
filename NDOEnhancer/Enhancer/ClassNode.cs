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
using System.Xml;
using System.Collections;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für ClassNode.
	/// </summary>
	internal class ClassNode
	{
		XmlNode node;
		public ClassNode(XmlNode node)
		{
			this.node = node;
		}

		public XmlNodeList Relations
		{
			get
			{
				return node.SelectNodes("Relation");
			}
		}

		public XmlNodeList Fields
		{
			get
			{
				return node.SelectNodes("Field");
			}
		}

		public string Name
		{
			get 
			{ 
				string s = node.Attributes["Name"].Value; 
				return s.Substring(s.IndexOf(']') + 1).Replace("/", "+");
			}
		}

		public string ILName
		{
			get { return node.Attributes["Name"].Value; }
		}


		public Type OidType
		{
			get 
			{
				XmlAttribute attr = node.Attributes["OidType"];
				if (attr == null)
					return null;
				return Type.GetType(attr.Value, false);
			}
		}

		public string OidTypeName
		{
			get 
			{
				XmlAttribute attr = node.Attributes["OidType"];
				if (attr == null)
					return null;
				return attr.Value;
			}
		}

		public string BaseName
		{
			get 
			{ 
				string s = node.Attributes["BaseName"].Value; 
				if (s == null)
					return null;
				return s.Substring(s.IndexOf(']') + 1).Replace("/", "+");
			}
		}

		public string BaseFullName
		{
			get 
			{ 
				return node.Attributes["BaseName"].Value; 
			}
		}

		public string AssemblyName
		{
			get 
			{ 
				string s = node.Attributes["Name"].Value; 
				return s.Substring(1, s.IndexOf(']') - 1);
			}
		}

		public override bool Equals(object obj)
		{
			if (! (obj is ClassNode))
				return false;
			return ((ClassNode) obj).Name == this.Name;
		}

		public bool IsAbstractOrInterface
		{
			get
			{
				if (node.Attributes["IsAbstract"] != null)
					return true;
				if (node.Attributes["IsInterface"] != null)
					return true;
				return false;
			}
		}
		
		public bool IsAbstract
		{
			get
			{
				if (node.Attributes["IsAbstract"] != null)
					return true;
				return false;
			}
		}
		public bool IsInterface
		{
			get
			{
				if (node.Attributes["IsInterface"] != null)
					return true;
				return false;
			}
		}
		public bool IsPoly
		{
#if PRO
			get { return node.Attributes["Poly"] != null; }
#else
			get { return false; }
#endif
			set
			{
#if PRO
				if (value == false)
					((XmlElement)node).RemoveAttribute("Poly");
				else
					((XmlElement)node).SetAttribute("Poly", string.Empty);
#endif
			}
		}

		public bool IsNull
		{
			get { return node == null; }
		}

	}
}
