//
// Copyright (c) 2002-2024 Mirko Matytschak 
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
