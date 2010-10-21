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
		public PropertyNode(NDOTreeNode parent, Property prop) : base(prop.Name, prop)
		{
			this.parent = parent;
			this.SelectedImageIndex = 14;
			this.ImageIndex = 14;
		}		

		public Property Property
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
