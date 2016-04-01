//
// Copyright (C) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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
using System.Collections.Generic;
using System.Text;
using NDO.Mapping;

namespace NDO.Mapping.Attributes
{
    /// <summary>
    /// This is used to make sure, that a mapping table is used for the relation.
    /// </summary>
    /// <remarks>
    /// The following sample assigns two string properties, one for the Class entry for <c>MyType</c> 
    /// and one for the Field entry for the field <c>name</c>.
    ///[MappingTable("relMyClassToAnotherClass")]
	///[NDORelation]
    ///List&lt;AnotherClass&gt; myRelation;
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MappingTableAttribute : Attribute
    {
        /// <summary>
        /// Constructs a MappingTableAttribute object
        /// </summary>
		/// <param name="tableName">Determines the name of the table</param>
        public MappingTableAttribute(string tableName)
        {
			this.tableName = tableName;
        }

        public MappingTableAttribute()
        {
            this.tableName = null;
        }

        string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
    }
}
