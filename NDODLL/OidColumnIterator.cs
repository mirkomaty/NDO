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
