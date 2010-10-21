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
using System.Windows.Forms;
using NDO.Mapping;

namespace SimpleMappingTool
{
	/// <summary>
	/// Zusammenfassung für RelationNode.
	/// </summary>
	internal class RelationNode : NDOTreeNode
	{
		public RelationNode(Relation relation) : base (relation.FieldName, relation)
		{
			this.ImageIndex = 4;
			this.SelectedImageIndex = 4;
			if (relation.MappingTable != null)
				this.Nodes.Add(new MappingTableNode(relation.MappingTable));
            foreach (ForeignKeyColumn fkCol in relation.ForeignKeyColumns)
                this.Nodes.Add(new ColumnNode(this, fkCol));
		}

		Relation MyRelation
		{
			get { return o as Relation; }
		}
		public override ContextMenu GetContextMenu()
		{
			ContextMenu menu = base.GetContextMenu();
			if (MyRelation.MappingTable == null)
				menu.MenuItems.Add(new MenuItem("Add mapping table", new EventHandler(AddMappingTable)));
			else
				menu.MenuItems.Add(new MenuItem("Delete mapping table", new EventHandler(DeleteMappingTable)));
			return menu;
		}

		void DeleteMappingTable(object sender, EventArgs ea)
		{
			MyRelation.MappingTable = null;
			while (this.Nodes.Count > 0)
				this.Nodes.RemoveAt(0);
			this.TreeView.Refresh();
		}

		void AddMappingTable(object sender, EventArgs ea)
		{
            MessageBox.Show("Sorry, we didn't implement this part yet.");
#if nix
			MappingTable mt = new MappingTable(MyRelation);
			MyRelation.MappingTable = mt;			
			mt.ConnectionId = MyRelation.Parent.ConnectionId;
			Class cl = MyRelation.Parent;
			string tableName;
			string myName = cl.FullName.Substring(cl.FullName.LastIndexOf(".") + 1);
			string otherName = MyRelation.ReferencedTypeName.Substring(MyRelation.ReferencedTypeName.LastIndexOf(".") + 1);
			if (myName.CompareTo(otherName) < 0)
				tableName = "rel" + myName + otherName;
			else
				tableName = "rel" + otherName + myName;
			mt.TableName = tableName;

			MyRelation.ForeignKeyColumnName = "ID" + myName;
			if (cl.SystemType != null)
			{
				if (cl.Subclasses.Count > 0)
					MyRelation.ForeignKeyTypeColumnName = "TC" + myName;					
			}
			mt.ChildForeignKeyColumnName = "ID" + otherName;
			if (MyRelation.ReferencedType != null)
			{
				if (MyRelation.ReferencedSubClasses.Count > 1)
					mt.ChildForeignKeyTypeColumnName = "TC" + otherName;
			}
			MappingTableNode mtn = new MappingTableNode(mt);
			this.Nodes.Add(mtn);
			this.TreeView.Refresh();
#endif
		}

		
	}
}
