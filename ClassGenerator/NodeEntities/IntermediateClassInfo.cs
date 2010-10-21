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
using System.Diagnostics;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für IntermediateClass.
	/// </summary>
	[Serializable]
#if DEBUG
	public class IntermediateClassInfo : IXmlStorable
#else
	internal class IntermediateClassInfo : IXmlStorable
#endif
	{
		public IntermediateClassInfo()
		{
		}

		// Composite: Since DualKey relations always show composite behavior,
		// we don't mark it as composite explicitly.

		// IsElement: We always assume, the own relation is Element, 
		// the foreign relation is List.

		string foreignKeyColumnName = string.Empty;
		public string ForeignKeyColumnName
		{
			get { return foreignKeyColumnName; }
			set { foreignKeyColumnName = value; }
		}

		string ownFieldName = string.Empty;
		/// <summary>
		/// Each relation has a field in the intermediate class.
		/// Name of the field in the intermediate class
		/// </summary>
		public string OwnFieldName
		{
			get { return ownFieldName; }
			set { ownFieldName = value; }
		}

		string foreignFieldName = string.Empty;
		/// <summary>
		/// Name of the field in the related class, if the relation
		/// is visible there.
		/// </summary>
		public string ForeignFieldName
		{
			get { return foreignFieldName; }
			set { foreignFieldName = value; }
		}

		string table = string.Empty;
		public string Table
		{
			get { return table; }
			set { table = value; }
		}
		string type = string.Empty;
		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		// Coding Style of the foreign relation, if it is visible
		CodingStyle codingStyle;
		public CodingStyle CodingStyle
		{
			get { return codingStyle; }
			set { codingStyle = value; }
		}

		// Only DirectedFromMe or Bidirectional is possible
		RelationDirection relationDirection;
		public RelationDirection RelationDirection
		{
			get { return relationDirection; }
			set 
			{ 
				relationDirection = value; 
			}
		}
		#region IXmlStorable Member

		public virtual void ToXml(XmlElement element)
		{
			element.SetAttribute("CodingStyle", this.CodingStyle.ToString());
			element.SetAttribute("OwnFieldName", this.ownFieldName);
			element.SetAttribute("ForeignFieldName", this.foreignFieldName);
			element.SetAttribute("ForeignKeyColumnName", this.foreignKeyColumnName);
			element.SetAttribute("RelationDirection", this.relationDirection.ToString());
			element.SetAttribute("Table", this.table);
			element.SetAttribute("RelationType", this.type);			
		}

		public virtual void FromXml(XmlElement element)
		{
			this.codingStyle = (CodingStyle) Enum.Parse(typeof(CodingStyle), element.Attributes["CodingStyle"].Value);
			this.ownFieldName = element.Attributes["OwnFieldName"].Value;
			this.foreignFieldName = element.Attributes["ForeignFieldName"].Value;
			this.foreignKeyColumnName = element.Attributes["ForeignKeyColumnName"].Value;
			this.relationDirection = (RelationDirection) Enum.Parse(typeof(RelationDirection), element.Attributes["RelationDirection"].Value);
			this.table = element.Attributes["Table"].Value;
			this.type = element.Attributes["RelationType"].Value;
		}

		#endregion
	}
}
