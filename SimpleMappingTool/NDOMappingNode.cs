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
using System.Linq;
using System.Collections.Generic;
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
            List<Class> sortedClasses = mapping.Classes.ToList();
            sortedClasses.Sort();
            foreach (Class cl in sortedClasses)
                classesNode.Nodes.Add(new ClassNode(cl));
            this.Nodes.Add(connectionsNode);
            foreach (Connection conn in mapping.Connections)
                connectionsNode.Nodes.Add(new ConnectionNode(conn));

            classesNode.Expand();
            this.Expand();
        
        }
    }
}
