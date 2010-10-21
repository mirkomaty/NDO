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
using System.Reflection;
using System.Windows.Forms;
using ClassGenerator.ForeignKeyWizard;
using ClassGenerator.Serialisation;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für RelationNode.
	/// </summary>
	[Serializable]
#if DEBUG
	public class RelationNode : NDOTreeNode
#else
	internal class RelationNode : NDOTreeNode
#endif
	{
		ColumnNode originalColumnNode;
		public ColumnNode OriginalColumnNode
		{
			get { return originalColumnNode; }
			set { originalColumnNode = value; }
		}

		public Relation Relation
		{
			get { return this.myObject as Relation; }
		}

		TableNode relatedTableNode;
		public TableNode RelatedTableNode
		{
			get { return relatedTableNode; }
			set { relatedTableNode = value; }
		}

		protected override void CalculateIndex()
		{
			SetImageIndex(Relation.IsElement ? 10 : 4);
		}


		public RelationNode(Relation relation, NDOTreeNode parent) : base (relation.Name, parent)
		{
			this.myObject = relation;
			CalculateIndex();
		}


		public override System.Windows.Forms.ContextMenu GetContextMenu()
		{
			ContextMenu menu = new ContextMenu();
			if (!Relation.IsForeign)
				menu.MenuItems.Add(new MenuItem("Remove Relation", new EventHandler(this.DeleteFkRelation)));
			return menu;
		}

		public void DeleteFkRelation(Object sender, EventArgs e)
		{
			ApplicationController.Instance.DeleteRelation(this);
		}

		public override void FromXml(System.Xml.XmlElement element)
		{
			base.FromXml (element);
			Type thisType = this.GetType();
			if (element.Attributes["OriginalColumnNode"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["OriginalColumnNode"].Value),
					thisType.GetField("originalColumnNode", BindingFlags.Instance | BindingFlags.NonPublic),
					this);
			}
			if (element.Attributes["RelatedTableNode"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["RelatedTableNode"].Value),
					thisType.GetField("relatedTableNode", BindingFlags.Instance | BindingFlags.NonPublic),
					this);
			}
		}
		public override void ToXml(System.Xml.XmlElement element)
		{
			base.ToXml (element);
			if (this.originalColumnNode != null)
			element.SetAttribute("OriginalColumnNode", this.originalColumnNode.GetHashCode().ToString());
			if (this.relatedTableNode != null)
			element.SetAttribute("RelatedTableNode", this.relatedTableNode.GetHashCode().ToString());
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public RelationNode()
		{
		}

#if SoapSerialisation
		protected RelationNode(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
#endif
	}
}
