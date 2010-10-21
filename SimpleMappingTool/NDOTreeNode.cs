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
using System.Collections;
using System.Windows.Forms;
using NDO;
using NDO.Mapping;

namespace SimpleMappingTool
{
	/// <summary>
	/// Zusammenfassung für NDOTreeNode.
	/// </summary>
	internal class NDOTreeNode : TreeNode
	{
		protected object o;
		public object Object
		{
			get { return o; }
			set { o = value; }
		}
		public NDOTreeNode(string s, object o) : base (s)
		{
			this.o = o;
			if (!(o is MappingNode)) // Base class of all NDO entities
				return;
			if (o is NDO.Mapping.Property)  // Properties don't have Properties
				return;
			foreach(DictionaryEntry de in ((MappingNode)o).Properties)
			{
				this.Nodes.Add(new PropertyNode(this, (Property)de.Value));
			}
		}

		public virtual ContextMenu GetContextMenu()
		{
			ContextMenu menu = new ContextMenu();
			if (!(this is PropertyNode))
				menu.MenuItems.Add(new MenuItem("Add Property", new EventHandler(AddProperty)));
            menu.MenuItems.Add(new MenuItem("Delete item", new EventHandler(DeleteItem))); 
			return menu;
		}

        protected virtual void DeleteItem(Object sender, EventArgs ea)
        {
            ((MappingNode)o).Remove();
            this.Remove();
        }


		public void RemoveProperty(PropertyNode propNode)
		{
			this.Nodes.Remove(propNode);
			((MappingNode)this.o).RemoveProperty(propNode.Text);
		}

		void AddProperty(object sender, EventArgs args)
		{
			AddPropertyDialog dlg = new AddPropertyDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				MappingNode mappingNode = (MappingNode)this.o;
				Property prop = new Property(mappingNode, dlg.PropName, dlg.Type, dlg.Value);
				mappingNode.AddProperty(prop);
				this.Nodes.Add(new PropertyNode(this, prop));
			}
		}
	}
}
