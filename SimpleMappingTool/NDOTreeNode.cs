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
		protected object? o;
		public object? Object
		{
			get { return o; }
			set { o = value; }
		}
		public NDOTreeNode(string s, object? o) : base (s)
		{
			this.o = o;
			if (!(o is MappingNode)) // Base class of all NDO entities
				return;
			if (o is NDO.Mapping.Property)  // Properties don't have Properties
				return;
			foreach(DictionaryEntry de in ((MappingNode)o).Properties)
			{
				this.Nodes.Add(new PropertyNode(this, (Property?)de.Value));
			}
		}

		public virtual ContextMenuStrip GetContextMenu()
		{
			var menu = new ContextMenuStrip();
			if (!( this is PropertyNode ))
				menu.Items.Add( new ToolStripMenuItem( "Add Property", null, AddProperty ));
            menu.Items.Add(new ToolStripMenuItem("Delete item", null, DeleteItem)); 
			return menu;
		}

        protected virtual void DeleteItem(object? sender, EventArgs ea)
        {
			if (o != null)
				((MappingNode)o).Remove();
            this.Remove();
        }


		public void RemoveProperty(PropertyNode propNode)
		{
			this.Nodes.Remove(propNode);
			if (o != null)
				((MappingNode)this.o).RemoveProperty(propNode.Text);
		}

		void AddProperty(object? sender, EventArgs args)
		{
			AddPropertyDialog dlg = new AddPropertyDialog();
			if (dlg.ShowDialog() == DialogResult.OK && this.o != null)
			{
				MappingNode mappingNode = (MappingNode)this.o;
				Property prop = new Property(mappingNode, dlg.PropName, dlg.Type, dlg.Value);
				mappingNode.AddProperty(prop);
				this.Nodes.Add(new PropertyNode(this, prop));
			}
		}
	}
}
