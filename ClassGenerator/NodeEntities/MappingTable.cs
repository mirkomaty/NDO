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
using System.Reflection;
using System.Collections;
using System.Xml;
using ClassGenerator.Serialisation;

namespace ClassGenerator.NodeEntities
{
	[Serializable]
#if DEBUG
	public class MappingTable : IXmlStorable	
#else
	internal class MappingTable : IXmlStorable
#endif
	{
		TableNode relatedNode1;
		public TableNode RelatedNode1
		{
			get { return relatedNode1; }
			set { relatedNode1 = value; }
		}
		TableNode relatedNode2;
		public TableNode RelatedNode2
		{
			get { return relatedNode2; }
			set { relatedNode2 = value; }
		}
		public ColumnNode relatedColumn1;
		public ColumnNode RelatedColumn1
		{
			get { return relatedColumn1; }
			set { relatedColumn1 = value; }
		}
		public ColumnNode relatedColumn2;
		public ColumnNode RelatedColumn2
		{
			get { return relatedColumn2; }
			set { relatedColumn2 = value; }
		}
		
		RelationCardinality relationCardinality1;
		public RelationCardinality RelationCardinality1
		{
			get { return relationCardinality1; }
			set { relationCardinality1 = value; }
		}
		RelationCardinality relationCardinality2;
		public RelationCardinality RelationCardinality2
		{
			get { return relationCardinality2; }
			set { relationCardinality2 = value; }
		}

		bool isComposite1;
		public bool IsComposite1
		{
			get { return isComposite1; }
			set { isComposite1 = value; }
		}
		bool isComposite2;
		public bool IsComposite2
		{
			get { return isComposite2; }
			set { isComposite2 = value; }
		}

		CodingStyle codingStyle1;
		public CodingStyle CodingStyle1
		{
			get { return codingStyle1; }
			set { codingStyle1 = value; }
		}
		CodingStyle codingStyle2;
		public CodingStyle CodingStyle2
		{
			get { return codingStyle2; }
			set { codingStyle2 = value; }
		}
		string fieldName1;
		public string FieldName1
		{
			get { return fieldName1; }
			set { fieldName1 = value; }
		}
		string fieldName2;
		public string FieldName2
		{
			get { return fieldName2; }
			set { fieldName2 = value; }
		}

		string relationName = string.Empty;
		public string RelationName
		{
			get { return relationName; }
			set { relationName = value; }
		}

		public MappingTable()
		{
		}

		public virtual void ToXml(XmlElement element)
		{
			element.SetAttribute("CodingStyle1", this.codingStyle1.ToString());
			element.SetAttribute("CodingStyle2", this.codingStyle2.ToString());
			element.SetAttribute("FieldName1", this.fieldName1);
			element.SetAttribute("FieldName2", this.fieldName2);
			element.SetAttribute("IsComposite1", this.isComposite1.ToString());
			element.SetAttribute("IsComposite2", this.isComposite2.ToString());
			if (this.relatedColumn1 != null)
				element.SetAttribute("RelatedColumn1", this.relatedColumn1.GetHashCode().ToString());
			if (this.relatedColumn2 != null)
				element.SetAttribute("RelatedColumn2", this.relatedColumn2.GetHashCode().ToString());
			if (this.relatedNode1 != null)
				element.SetAttribute("RelatedTable1", this.relatedNode1.GetHashCode().ToString());
			if (this.relatedNode2 != null)
				element.SetAttribute("RelatedTable2", this.relatedNode2.GetHashCode().ToString());
			element.SetAttribute("RelationCardinality1", this.relationCardinality1.ToString());
			element.SetAttribute("RelationCardinality2", this.relationCardinality2.ToString());
			element.SetAttribute("RelationName", this.relationName);
		}

		public virtual void FromXml(XmlElement element)
		{
			this.codingStyle1 = (CodingStyle) Enum.Parse(typeof(CodingStyle), element.Attributes["CodingStyle1"].Value);
			this.codingStyle2 = (CodingStyle) Enum.Parse(typeof(CodingStyle), element.Attributes["CodingStyle2"].Value);
			this.fieldName1 = element.Attributes["FieldName1"].Value;
			this.fieldName2 = element.Attributes["FieldName2"].Value;
			this.isComposite1 = bool.Parse(element.Attributes["IsComposite1"].Value);
			this.isComposite2 = bool.Parse(element.Attributes["IsComposite2"].Value);
			Type thisType = this.GetType();
			if (element.Attributes["RelatedColumn1"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["RelatedColumn1"].Value),
					thisType.GetField("relatedColumn1", BindingFlags.Instance | BindingFlags.NonPublic),
					this);
			}
			if (element.Attributes["RelatedColumn2"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["RelatedColumn2"].Value),
					thisType.GetField("relatedColumn2", BindingFlags.Instance | BindingFlags.NonPublic),
					this);
			}
			if (element.Attributes["RelatedTable1"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["RelatedTable1"].Value),
					thisType.GetField("relatedNode1", BindingFlags.Instance | BindingFlags.NonPublic),
					this);
			}
			if (element.Attributes["RelatedTable2"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["RelatedTable2"].Value),
					thisType.GetField("relatedNode2", BindingFlags.Instance | BindingFlags.NonPublic),
					this);
			}

			this.relationCardinality1 = (RelationCardinality)Enum.Parse(typeof(RelationCardinality), element.Attributes["RelationCardinality1"].Value);
			this.relationCardinality2 = (RelationCardinality)Enum.Parse(typeof(RelationCardinality), element.Attributes["RelationCardinality2"].Value);
			this.relationName = element.Attributes["RelationName"].Value;
		}

	}
}

