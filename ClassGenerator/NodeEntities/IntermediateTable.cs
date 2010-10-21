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
using System.ComponentModel;
using WizardBase;

namespace ClassGenerator
{

	/// <summary>
	/// Zusammenfassung für IntermediateTable.
	/// </summary>
	[Serializable]
#if DEBUG
	public class IntermediateTable : IXmlStorable
#else
	internal class IntermediateTable : IXmlStorable
#endif
	{
		string name;
		public string Name
		{
			get { return name; }
		}

		public IntermediateTable(string name)
		{
			tableInfo[0] = new RelatedTableInfo();
			tableInfo[1] = new RelatedTableInfo();
			this.name = name;
		}
		RelatedTableInfo[] tableInfo = new RelatedTableInfo[2];

		public string RelatedTable1
		{
			get { return tableInfo[0].Table; }
		}

		public string RelatedTable2
		{
			get { return tableInfo[1].Table; }
		}

		public string RelatedClass1
		{
			get { return tableInfo[0].Type; }
		}

		public string RelatedClass2
		{
			get { return tableInfo[1].Type; }
		}

		[Browsable(false)]
		public RelatedTableInfo this[int i]
		{
			get { return tableInfo[i]; }
		}
		#region IXmlStorable Member

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public IntermediateTable()
		{
		}

		public void ToXml(XmlElement element)
		{
			element.SetAttribute("Name", this.name);
			for (int i = 0; i < 2; i++)
			{
				XmlElement tiElement = element.OwnerDocument.CreateElement("TableInfo" + i);
				element.AppendChild(tiElement);
				tableInfo[i].ToXml(tiElement);
			}
		}

		public void FromXml(XmlElement element)
		{
			this.name = element.Attributes["Name"].Value;
			for (int i = 0; i < 2; i++)
			{
				XmlElement tiElement = (XmlElement)element.SelectSingleNode("TableInfo" + i);
				tableInfo[i] = new RelatedTableInfo();
				tableInfo[i].FromXml(tiElement);
			}
		}

		#endregion
	}

	[Serializable]
#if DEBUG
	public class RelatedTableInfo : IXmlStorable
#else
	internal class RelatedTableInfo : IXmlStorable
#endif
	{
		public RelatedTableInfo()
		{
			this.relationName = string.Empty;
		}

		string foreignKeyColumnName = string.Empty;
		public string ForeignKeyColumnName
		{
			get { return foreignKeyColumnName; }
			set { foreignKeyColumnName = value; }
		}

		string childForeignKeyColumnName = string.Empty;
		public string ChildForeignKeyColumnName
		{
			get { return childForeignKeyColumnName; }
			set { childForeignKeyColumnName = value; }
		}

		string fieldName = string.Empty;
		public string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; }
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
		CodingStyle codingStyle;
		public CodingStyle CodingStyle
		{
			get { return codingStyle; }
			set { codingStyle = value; }
		}
		RelationDirection relationDirection = RelationDirection.Bidirectional;
		public RelationDirection RelationDirection
		{
			get { return relationDirection; }
			set { relationDirection = value; }
		}

		bool isComposite;
		public bool IsComposite
		{
			get { return isComposite; }
			set { isComposite = value; }
		}
		string relationName = string.Empty;
		public string RelationName
		{
			get { return relationName; }
			set { relationName = value; }
		}
		bool isElement;
		public bool IsElement
		{
			get { return isElement; }
			set { isElement = value; }
		}
		#region IXmlStorable Member

		public virtual void FromXml(XmlElement element)
		{
			this.CodingStyle = (CodingStyle) CodingStyle.Parse(typeof(CodingStyle), element.Attributes["CodingStyle"].Value);
			this.fieldName = element.Attributes["FieldName"].Value;
			this.table = element.Attributes["Table"].Value;
			this.foreignKeyColumnName = element.Attributes["ForeignKeyColumnName"].Value;
			this.isComposite = bool.Parse(element.Attributes["IsComposite"].Value);
			this.isElement = bool.Parse(element.Attributes["IsElement"].Value);
			this.relationDirection = (RelationDirection) RelationDirection.Parse(typeof(RelationDirection), element.Attributes["RelationDirection"].Value);
			this.type = element.Attributes["RelationType"].Value;			
		}

		public virtual void ToXml(XmlElement element)
		{
			element.SetAttribute("CodingStyle", this.CodingStyle.ToString());
			element.SetAttribute("FieldName", this.fieldName);
			element.SetAttribute("Table", this.table);
			element.SetAttribute("ForeignKeyColumnName", this.foreignKeyColumnName.ToString());
			element.SetAttribute("IsComposite", this.isComposite.ToString());
			element.SetAttribute("IsElement", this.isElement.ToString());
			element.SetAttribute("RelationDirection", this.relationDirection.ToString());
			element.SetAttribute("RelationType", this.type);			
		}


		#endregion
	}

}
