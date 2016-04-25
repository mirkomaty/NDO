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

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für XmlHelper.
	/// </summary>
	internal class XmlHelper
	{
		public static object GetNode(XmlNode parent, string name)
		{
			return GetNode(parent, name, null);
		}

		public static object GetNode(XmlNode parent, string name, object defaultValue)
		{
			XmlNode nd = parent.SelectSingleNode(name, nsmgr);
			if (nd == null)
			{
				if (defaultValue == null)
					throw new Exception("Can't find node " + name);
				else
					return defaultValue;
			}
			else
			{
				string innerTextL = nd.InnerText.ToLower();
				if (innerTextL == "true" || innerTextL == "false")
					return bool.Parse(nd.InnerText);
				return nd.InnerText;
			}
		}

		public static object GetAttribute(XmlNode parent, string name)
		{
			return GetAttribute(parent, name, null);
		}

		public static object GetAttribute(XmlNode parent, string name, object defaultValue)
		{
			string s = parent.Attributes[name].Value;
			if (s == null)
			{
				if (defaultValue == null)
					throw new Exception("Can't find node " + name);
				else
					return defaultValue;
			}
			else
			{
				if (s == "True" || s == "False")
					return bool.Parse(s);
				return s;
			}
		}

		public const string ParameterNamespace = "http://tempuri.org/EnhancerParameters.xsd";
		private const string _pns = "pns:";
		static XmlNamespaceManager nsmgr = null;

		public static XmlNamespaceManager Nsmgr
		{
			get { return nsmgr; }
			set { nsmgr = value; }
		}

		public static string Pns (XmlDocument doc)
		{
			if (HasNamespace(doc))
				return _pns;
			else
				return string.Empty;
		}

		public static bool HasNamespace(XmlDocument doc)
		{
			string ns = doc.DocumentElement.GetAttribute("xmlns");
			return (null != ns && ns == ParameterNamespace);
		}


		public static void AddNamespace(XmlDocument doc)
		{
			string ns = doc.DocumentElement.GetAttribute("xmlns");
			if (null != ns && ns == ParameterNamespace) 
			{
				nsmgr = new XmlNamespaceManager(doc.NameTable);
				nsmgr.AddNamespace("pns", ParameterNamespace);				
			} 
		}
	}
}
