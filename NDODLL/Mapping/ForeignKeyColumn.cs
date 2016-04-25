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
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a foreign key column. It's a column with some
    /// additional data.
    /// </summary>
    public class ForeignKeyColumn : Column
    {
        #region Constructors and Save Method

        /// <summary>
        /// Constructs a Column element and assigns it to the given parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        public ForeignKeyColumn(MappingNode parent)
            : base(parent)
        {
        }


        internal ForeignKeyColumn(XmlNode columnNode, MappingNode parent)
            : base(columnNode, parent)
        {
        }

        internal override void Save(XmlNode parentNode)
        {
            XmlElement fkColumnNode = parentNode.OwnerDocument.CreateElement("ForeignKeyColumn");
            parentNode.AppendChild(fkColumnNode);
            base.SaveOwnNode(fkColumnNode);  // Saves properties too
        }
        #endregion

        /// <summary>
        /// Removes the ForeignKeyColumn object from the list of ForeignKeyColumns in the parent object.
        /// </summary>
        public override void Remove()
        {
            Relation r = Parent as Relation;
            if (r != null)
            {
                r.RemoveForeignKeyColumn(this);
                return;
            }
            MappingTable mt = Parent as MappingTable;
            if (mt != null)
            {
                mt.RemoveChildForeignKeyColumn(this);
            }
        }
    }
}
