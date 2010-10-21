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
using System.Text;
using NDO.Mapping;

namespace SimpleMappingTool
{
    internal class NDOMappingNode : NDOTreeNode
    {
        public NDOMappingNode(NDOMapping mapping) : base("NDOMapping", mapping)
        {
            this.ImageIndex = 13;
            this.SelectedImageIndex = 13;

            TreeNode classesNode;
            TreeNode connectionsNode;
            classesNode = new TreeNode("Classes");
            connectionsNode = new TreeNode("Connections");
            connectionsNode.ImageIndex = 12;
            connectionsNode.SelectedImageIndex = 12;
            classesNode.ImageIndex = 0;
            classesNode.SelectedImageIndex = 0;
            this.Nodes.Add(classesNode);
            ArrayList sortedClasses = new ArrayList(mapping.Classes);
            sortedClasses.Sort();
            foreach (Class cl in sortedClasses)
                classesNode.Nodes.Add(new ClassNode(cl));
            this.Nodes.Add(connectionsNode);
            foreach (Connection conn in mapping.Connections)
                connectionsNode.Nodes.Add(new ConnectionNode(conn));

            classesNode.Expand(); // RL 07-03-2008
            this.Expand(); // RL 07-03-2008
        
        }
    }
}
