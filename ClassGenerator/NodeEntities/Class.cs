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

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für Class.
	/// </summary>
	[Serializable]
#if DEBUG
	public class Class : IXmlStorable
#else
	internal class Class : IXmlStorable
#endif
	{
//		public event EventHandler NameChanged;

		string name;
		public string Name
		{
			get { return name; }
//			set 
//			{ 
//				name = value;
//				if (NameChanged != null)
//					NameChanged(this, EventArgs.Empty);
//			}
		}

		string tableName;
		public string TableName
		{
			get { return tableName; }
		}

		public Class(string name, string tableName)
		{
			this.name = name;
			this.tableName = tableName;
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public Class()
		{
		}
		

		#region IXmlStorable Member

		public void ToXml(XmlElement element)
		{			
			element.SetAttribute("Name", this.name);
			element.SetAttribute("TableName", this.tableName);
		}

		public void FromXml(XmlElement element)
		{
			this.name = element.Attributes["Name"].Value;
			this.tableName = element.Attributes["TableName"].Value;
		}

		#endregion
	}
}
