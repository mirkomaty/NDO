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
using System.ComponentModel;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für ForeignIntermediateTableRelation.
	/// </summary>
	[Serializable]
#if DEBUG	
	public class ForeignIntermediateTableRelation : Relation
#else
	internal class ForeignIntermediateTableRelation : Relation
#endif
	{
		RelatedTableInfo relatedTableInfo;

		public ForeignIntermediateTableRelation(RelatedTableInfo relatedTableInfo, string tableName)
		{
			this.relatedTableInfo = relatedTableInfo;
			this.tableName = tableName;
			base.IsForeign = true;
		}

		[ReadOnly(true)]
		public override string FieldName
		{
			get { return relatedTableInfo.FieldName; }
			set {}
		}

		[ReadOnly(true)]
		public override bool IsComposite
		{
			get { return relatedTableInfo.IsComposite; }
			set { }
		}

		public override bool IsElement
		{
			get { return relatedTableInfo.IsElement; }
		}

		public override string Name
		{
			get
			{
				return relatedTableInfo.ForeignKeyColumnName;
			}
		}

		[ReadOnly(true)]
		public override RelationDirection RelationDirection
		{
			get { return relatedTableInfo.RelationDirection; }
			set { }
		}

		public string ForeignKeyColumnName
		{
			get { return relatedTableInfo.ForeignKeyColumnName; }
		}

		public string ChildForeignKeyColumnName
		{
			get { return relatedTableInfo.ChildForeignKeyColumnName; }
		}

		string tableName;
		public string TableName
		{
			get { return tableName; }
		}

		[ReadOnly(true)]
		public override string RelationName
		{
			get
			{
				return relatedTableInfo.RelationName;
			}
			set { }
		}		

		[ReadOnly(true)]
		public override CodingStyle CodingStyle
		{
			get { return relatedTableInfo.CodingStyle; }
			set {  }
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public ForeignIntermediateTableRelation()
		{
		}

		public override void FromXml(XmlElement element)
		{
			base.FromXml (element);
			this.tableName = element.Attributes["TableName"].Value;
			this.relatedTableInfo = new RelatedTableInfo();
			XmlElement tiElement = (XmlElement)element.SelectSingleNode("RelatedTableInfo");
			this.relatedTableInfo.FromXml(tiElement);
		}

		public override void ToXml(XmlElement element)
		{
			base.ToXml (element);
			element.SetAttribute("TableName", this.tableName);
			XmlElement tiElement = element.OwnerDocument.CreateElement("RelatedTableInfo");
			element.AppendChild(tiElement);
			relatedTableInfo.ToXml(tiElement);
		}



	}
}
