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
