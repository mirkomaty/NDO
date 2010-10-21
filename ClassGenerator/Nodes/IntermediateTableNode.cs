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
using WizardBase;
using ClassGenerator.Serialisation;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für IntermediateTableNode.
	/// </summary>
	[Serializable]
#if DEBUG
	public class IntermediateTableNode : NDOTreeNode
#else
	internal class IntermediateTableNode : NDOTreeNode
#endif
	{
		TableNode originalTableNode;
		public IntermediateTable IntermediateTable
		{
			get { return this.myObject as IntermediateTable; }
		}

		public TableNode OriginalTableNode
		{
			get { return originalTableNode; }
		}

		public IntermediateTableNode(IntermediateTable intTable, TableNode originalTableNode) : base(originalTableNode.Text, originalTableNode.Parent)
		{
			this.originalTableNode = originalTableNode;
			this.myObject = intTable;
			CalculateIndex();
		}

		public override System.Windows.Forms.ContextMenu GetContextMenu()
		{
			ContextMenu menu = new ContextMenu();
			menu.MenuItems.Add(new MenuItem("Unmap Intermediate Table", new EventHandler(this.UnmapIntermediateTable)));
			return menu;
		}

		public void UnmapIntermediateTable(Object sender, EventArgs e)
		{
			ApplicationController.Instance.UnmapIntermediateTable(this);
		}

		protected override void CalculateIndex()
		{
			this.SetImageIndex(13);
		}

		public override void ToXml(System.Xml.XmlElement element)
		{
			base.ToXml (element);
			if (this.originalTableNode != null)
				element.SetAttribute("OriginalTableNode", this.originalTableNode.GetHashCode().ToString());
		}

		public override void FromXml(System.Xml.XmlElement element)
		{
			base.FromXml (element);	
			if (element.Attributes["OriginalTableNode"] != null)
			{
				NodeListSerializer.AddFixup(int.Parse(element.Attributes["OriginalTableNode"].Value),
					this.GetType().GetField("originalTableNode", BindingFlags.Instance | BindingFlags.NonPublic),
					this);
			}
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public IntermediateTableNode()
		{

		}


#if SoapSerialisation
		protected IntermediateTableNode(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
#endif
	}
}
