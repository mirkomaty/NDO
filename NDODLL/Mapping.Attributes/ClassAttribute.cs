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
using System.Collections.Generic;
using System.Text;

namespace NDO.Mapping.Attributes
{
    /// <summary>
    /// Use this attribute to influence the properties of a Class entry in the mapping file. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassAttribute : Attribute
    {
        string connectionId;
        /// <summary>
        /// Determines the connection id of the database connection, the mapping table resides in. 
        /// It's recommended to let NDO assign the connection ids.
        /// </summary>
        public string ConnectionId
        {
            get { return connectionId; }
            set { connectionId = value; }
        }
        string tableName;
        /// <summary>
        /// Determines the name of the table used to store the objects.
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        string timestampColumn;
        /// <summary>
        /// Determines the name of the column, which holds the time stamp of the last database update.
        /// Setting the TimestampColumn property to a value != null enables the NDO collision detection for the given type.
        /// </summary>
        public string TimestampColumn
        {
            get { return timestampColumn; }
            set { timestampColumn = value; }
        }

        string typenameColumn;
        /// <summary>
        /// Determines the name of the column, which holds the type instance of a generic type. 
        /// Do not specify the TypenameColumn for non-generic types.
        /// </summary>
        public string TypenameColumn
        {
            get { return typenameColumn; }
            set { typenameColumn = value; }
        }
    }
}
