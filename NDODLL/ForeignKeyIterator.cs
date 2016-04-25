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
using System.Text;

namespace NDO.Mapping
{
    /// <summary>
    /// Handler type for the ForeignKeyIterator.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="isLastElement"></param>
    public delegate void FkIteratorHandler(ForeignKeyColumn column, bool isLastElement);

    /// <summary>
    /// Helper class to iterate over all foreign key columns of a given relation.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <c>
    /// new ForeignKeyIterator(relationMapping).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
    /// {
    ///     Debug.WriteLine("Column: " + fkColumn.Name);
    /// });
    /// </c>
    /// </remarks>
    public class ForeignKeyIterator
    {
        List<ForeignKeyColumn> columns;
        /// <summary>
        /// Constructs a ForeignKeyIterator object, which iterates over all foreign key columns of the given relation.
        /// </summary>
        /// <param name="r">The relaion.</param>
        public ForeignKeyIterator(Relation r)
        {
            this.columns = r.ForeignKeyColumns.ToList();
        }

        /// <summary>
        /// Constructs a ForeignKeyIterator object, which iterates over all child foreign key columns of the given mapping table.
        /// </summary>
        /// <param name="mt"></param>
        public ForeignKeyIterator(MappingTable mt)
        {
            this.columns = mt.ChildForeignKeyColumns.ToList();
        }

        /// <summary>
        /// This method performs the iteration.
        /// </summary>
        /// <param name="handler">A delegate which will be called back for each element in the collection.</param>
        public void Iterate(FkIteratorHandler handler)
        {
            int count = columns.Count;
            int end = columns.Count - 1;
            for (int i = 0; i < count; i++)
            {
                handler((ForeignKeyColumn)columns[i], i == end);
            }
        }
    }
}
