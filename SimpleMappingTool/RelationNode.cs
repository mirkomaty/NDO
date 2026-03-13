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
using System.Windows.Forms;
using NDO;
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

		Relation? MyRelation
		{
			get { return o as Relation; }
		}

        public override ContextMenuStrip GetContextMenu()
		{
            ContextMenuStrip menu = base.GetContextMenu();
			if (MyRelation != null)
			{
				if (MyRelation.MappingTable == null)
					menu.Items.Add( new ToolStripMenuItem( "Add mapping table", null, AddMappingTable ) );
				else
					menu.Items.Add( new ToolStripMenuItem( "Delete mapping table", null, new EventHandler( DeleteMappingTable ) ) );
			}
			return menu;
		}

		void DeleteMappingTable(object? sender, EventArgs ea)
		{
			if (MyRelation != null)
			{
				MyRelation.MappingTable = null;
				while (this.Nodes.Count > 0)
					this.Nodes.RemoveAt( 0 );
				this.TreeView.Refresh();
			}
		}

		void AddMappingTable(object? sender, EventArgs ea)
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
