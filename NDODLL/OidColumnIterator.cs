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
    /// Handler type for the OidColumnIterator.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="isLastElement"></param>
    public delegate void OidIteratorHandler(OidColumn column, bool isLastElement);

    /// <summary>
    /// Helper class to iterate over all oid columns of a given class mapping.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <c>
    /// new OidColumnIterator(classMapping).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
    /// {
    ///     Debug.WriteLine("Column: " + oidColumn.Name);
    /// });
    /// </c>
    /// </remarks>
    public class OidColumnIterator
    {
        List<OidColumn> columns;
        /// <summary>
        /// Constructs the OidColumnIterator for a given oid mapping.
        /// </summary>
        /// <param name="oid"></param>
        public OidColumnIterator(ClassOid oid)
        {
            this.columns = oid.OidColumns.ToList();
        }

        /// <summary>
        /// Constructs the OidColumnIterator for a Oid of a given class mapping.
        /// </summary>
        /// <param name="cl"></param>
        public OidColumnIterator(Class cl)
        {
            this.columns = cl.Oid.OidColumns.ToList();
        }

        /// <summary>
        /// This method performs the iteration.
        /// </summary>
        /// <param name="handler">A delegate which will be called back for each element in the collection.</param>
        public void Iterate(OidIteratorHandler handler)
        {
            int count = columns.Count;
            int end = columns.Count - 1;
            for (int i = 0; i < count; i++)
            {
                handler((OidColumn)columns[i], i == end);
            }
        }
    }
}
