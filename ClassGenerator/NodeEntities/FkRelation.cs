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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ClassGenerator
{
	[Serializable]
#if DEBUG
	public class FkRelation : Relation
#else
	internal class FkRelation : Relation
#endif
	{		
		RelationDirection relationDirection = RelationDirection.Bidirectional;
		public override RelationDirection RelationDirection
		{
			get { return relationDirection; }
			set { relationDirection = value; }
		}

		bool isComposite;
		public override bool IsComposite
		{
			get { return isComposite; }
			set { isComposite = value; }
		}

		bool foreignIsComposite;
		[Browsable(false)]
		public bool ForeignIsComposite
		{
			get { return foreignIsComposite; }
			set { foreignIsComposite = value; }
		}

		CodingStyle codingStyle;		
		public override CodingStyle CodingStyle
		{
			get { return codingStyle; }
			set { codingStyle = value; }
		}

		CodingStyle foreignCodingStyle;
		[Browsable(false)]
		public CodingStyle ForeignCodingStyle
		{
			get { return foreignCodingStyle; }
			set { foreignCodingStyle = value; }
		}

		string fieldName = string.Empty;
		public override string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; }
		}

		string foreignFieldName = string.Empty;
		[Browsable(false)]
		public string ForeignFieldName
		{
			get { return foreignFieldName; }
			set { foreignFieldName = value; }
		}

		string relationName = string.Empty;
		public override string RelationName
		{
			get { return relationName; }
			set { relationName = value; }
		}

		string relatedTable = string.Empty;
		[ReadOnly(true)]
		public string RelatedTable
		{
			get { return relatedTable; }
			set { relatedTable = value; }
		}

		string relatedType = string.Empty;
		[ReadOnly(true)]
		public string RelatedType
		{
			get { return relatedType; }
			set { relatedType = value; }
		}

		string owningTable;
		[ReadOnly(true)]
		public string OwningTable
		{
			get { return owningTable; }
			set { owningTable = value; }
		}

		string owningType;
		[ReadOnly(true)]
		public string OwningType
		{
			get { return owningType; }
			set { owningType = value; }
		}

		string name = string.Empty;
		public override string Name
		{
			get { return name; }
		}

		ForeignFkRelation foreignRelation;

		public FkRelation(string name)
		{
			this.name = name;
			base.IsForeign = false;
		}

		[Browsable(false)]
		public override bool IsElement
		{
			get
			{
				return true;
			}
		}

		public ForeignFkRelation ForeignRelation
		{
			get
			{ 
				if (this.foreignRelation == null)
					this.foreignRelation = new ForeignFkRelation(this);
				return this.foreignRelation;
			}
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public FkRelation()
		{
		}

		public override void FromXml(XmlElement element)
		{
			base.FromXml (element);
			this.CodingStyle = (CodingStyle) CodingStyle.Parse(typeof(CodingStyle), element.Attributes["CodingStyle"].Value);
			this.fieldName = element.Attributes["FieldName"].Value;
			this.foreignCodingStyle = (CodingStyle) CodingStyle.Parse(typeof(CodingStyle), element.Attributes["ForeignCodingStyle"].Value);
			this.foreignFieldName = element.Attributes["ForeignFieldName"].Value;
			this.foreignIsComposite = bool.Parse(element.Attributes["ForeignIsComposite"].Value);
			this.isComposite = bool.Parse(element.Attributes["IsComposite"].Value);
			this.name = element.Attributes["Name"].Value;
			this.owningTable = element.Attributes["OwningTable"].Value;
			this.relatedTable = element.Attributes["RelatedTable"].Value;
			this.relationDirection = (RelationDirection) RelationDirection.Parse(typeof(RelationDirection), element.Attributes["RelationDirection"].Value);
			this.relationName = element.Attributes["RelationName"].Value;			
		}

		public override void ToXml(XmlElement element)
		{
			base.ToXml (element);
			element.SetAttribute("CodingStyle", this.CodingStyle.ToString());
			element.SetAttribute("FieldName", this.fieldName);
			element.SetAttribute("ForeignCodingStyle", this.foreignCodingStyle.ToString());
			element.SetAttribute("ForeignFieldName", this.foreignFieldName);
			element.SetAttribute("ForeignIsComposite", this.foreignIsComposite.ToString());
			element.SetAttribute("IsComposite", this.isComposite.ToString());
			element.SetAttribute("Name", this.name);
			element.SetAttribute("OwningTable", this.owningTable);
			element.SetAttribute("RelatedTable", this.relatedTable);
			element.SetAttribute("RelationDirection", this.relationDirection.ToString());
			element.SetAttribute("RelationName", this.relationName);			
		}



	}

	[Serializable]
#if DEBUG
	public enum RelationDirection
#else
	internal enum RelationDirection
#endif
	{
		DirectedToMe,
		DirectedFromMe,
		Bidirectional
	}

#if DEBUG
	public class RelationDirectionClass
#else
	internal class RelationDirectionClass
#endif
	{
		public static RelationDirection Reverse(RelationDirection dir)
		{
			if (dir == RelationDirection.Bidirectional)
				return dir;
			return (dir == RelationDirection.DirectedFromMe) 
				? RelationDirection.DirectedToMe
				: RelationDirection.DirectedFromMe;
		}
	}

	[Serializable]
#if DEBUG
	public enum CodingStyle
#else
	internal enum CodingStyle
#endif
	{
		ArrayList,
        IList,
        NDOArrayList
	}


}
