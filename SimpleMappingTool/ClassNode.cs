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
using NDO.Mapping;

namespace SimpleMappingTool
{
	/// <summary>
	/// Zusammenfassung für ClassNode.
	/// </summary>
	internal class ClassNode : NDOTreeNode
	{
		public ClassNode(Class cl) : base (cl.FullName, cl)
		{
			this.ImageIndex = 0;
			this.SelectedImageIndex = 0;

			this.Nodes.Add(new OidNode(cl.Oid));

			foreach(Relation r in cl.Relations)
			{
				this.Nodes.Add(new RelationNode(r));
			}
            List<Field> sortedFields = cl.Fields.ToList();
            sortedFields.Sort();
			foreach(Field f in sortedFields)
			{
				this.Nodes.Add(new FieldNode(f));
			}
			
		}

	}
}
