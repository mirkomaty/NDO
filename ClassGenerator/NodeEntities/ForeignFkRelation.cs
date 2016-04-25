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
using System.Collections.Specialized;
using System.ComponentModel;

namespace ClassGenerator
{
	[Serializable]
#if DEBUG
	public class ForeignFkRelation : Relation
#else
	internal class ForeignFkRelation : Relation
#endif
	{		
		FkRelation master;

		public ForeignFkRelation(FkRelation master)
		{
			this.master = master;
			this.isComposite = master.ForeignIsComposite;
			this.codingStyle = master.ForeignCodingStyle;
			this.fieldName = master.ForeignFieldName;
			base.IsForeign = true;
		}


		RelationDirection relationDirection;
		[ReadOnly(true)]
		public override RelationDirection RelationDirection
		{
			get {  return RelationDirectionClass.Reverse(master.RelationDirection); }
			set { }
		}

		bool isComposite;
		public override bool IsComposite
		{
			get { return isComposite; }
			set { isComposite = value; }
		}

		CodingStyle codingStyle;
		public override CodingStyle CodingStyle
		{
			get { return codingStyle; }
			set { codingStyle = value; }
		}

		string fieldName;
		public override string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; }
		}


		[ReadOnly(true)]
		public override string RelationName
		{
			get { return master.RelationName; }
			set { master.RelationName = value; }
		}

		public string RelatedTable
		{
			get { return master.OwningTable; }
		}

		public string RelatedType
		{
			get { return master.OwningType; }
		}

		public string OwningTable
		{
			get { return master.RelatedTable; }
		}

		public string OwningType
		{
			get { return master.RelatedType; }
		}

		public override string Name
		{
			get { return master.Name; }
		}


		[Browsable(false)]
		public override bool IsElement
		{
			get
			{
				return false;
			}
		}

		string xPath;		
		public string XPath
		{
			get { return xPath; }
			set { xPath = value; }
		}

        string singularFieldName;
        [Browsable(false)]
        public string SingularFieldName
        {
            get { return singularFieldName; }
            set { singularFieldName = value; }
        }


		/// <summary>
		/// Used by serialization only
		/// </summary>
		public ForeignFkRelation()
		{
		}

		public override void FromXml(XmlElement element)
		{
			base.FromXml (element);
			this.CodingStyle = (CodingStyle) CodingStyle.Parse(typeof(CodingStyle), element.Attributes["CodingStyle"].Value);
			this.fieldName = element.Attributes["FieldName"].Value;
			this.isComposite = bool.Parse(element.Attributes["IsComposite"].Value);
			this.relationDirection = (RelationDirection) RelationDirection.Parse(typeof(RelationDirection), element.Attributes["RelationDirection"].Value);
			XmlElement masterElement = (XmlElement) element.SelectSingleNode("Master");
			master = new FkRelation();
			master.FromXml(masterElement);
		}

		public override void ToXml(XmlElement element)
		{
			base.ToXml (element);
			element.SetAttribute("CodingStyle", this.CodingStyle.ToString());
			element.SetAttribute("FieldName", this.fieldName);
			element.SetAttribute("IsComposite", this.isComposite.ToString());
			element.SetAttribute("RelationDirection", this.relationDirection.ToString());
			XmlElement masterElement = element.OwnerDocument.CreateElement("Master");
			element.AppendChild(masterElement);
			master.ToXml(masterElement);
		}



	}

}
