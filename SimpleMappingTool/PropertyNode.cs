﻿//
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
	/// Zusammenfassung für PropertyNode.
	/// </summary>
	internal class PropertyNode : NDOTreeNode
	{
		NDOTreeNode parent;
		public PropertyNode(NDOTreeNode parent, Property? prop) : base(prop?.Name??"--", prop)
		{
			this.parent = parent;
			this.SelectedImageIndex = 14;
			this.ImageIndex = 14;
		}		

		public Property? Property
		{
			get { return this.o as Property; }
		}

#if nix
		void ChangeProperty(object sender, EventArgs args)
		{
			AddPropertyDialog dlg = new AddPropertyDialog();
			dlg.PropName = Property.Name;
			dlg.Type = Property.Type;
			dlg.Value = Property.Value;
			dlg.EditProperty = true;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Property.Type = dlg.Type;
				Property.Value = dlg.Value;
				MappingNode mn = (MappingNode)parent.Object;
				mn.AddProperty(Property);
			}
			this.TreeView.Refresh();			
		}
#endif
	}
}
