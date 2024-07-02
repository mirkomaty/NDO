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
    /// This attribute is used to place hints for the mapping information of foreign key columns in the source code. 
    /// The use of this attribute forces NDO to generate a mapping table and use the ChildForeignKeyColumnAttributes to 
    /// generate foreign key columns for the related type in the mapping table.
    /// If no ForeignKeyColumnAttribute is used, NDO automatically generates the columns based on the information about
    /// the OidColumns of the owner type and the related type.
    /// </summary>
    /// <remarks>
    /// The sample below shows, how to define the name of foreign key columns. The type of the column will be automatically determined
    /// according to the type of the oid column of the related type. Since no OidColumnAttribute exists for <c>RelatedType</c>, an integer 
    /// column is assumed.
    /// <code>
    /// [NDOPersistent]
    /// public class Owner
    /// {
    ///     [NDORelation]
    ///     [ForeignKeyColumn(Name = "IDOwner")]
    ///     [ChildForeignKeyColumn(Name = "IDRelatedType")]
    ///     List&lt;RelatedType&gt; relatedObjects;
    ///     ...
    /// }
    /// 
    /// [NDOPersistent]
    /// public class RelatedType
    /// {
    ///     ...
    /// }
    /// </code>
    /// The count, order and type of the foreign key columns must exactly match the count, order and type of the oid
    /// columns of the related type. The following sample leads to an error, because the types don't match.
    /// <code>
    /// [NDOPersistent]
    /// public class Owner
    /// {
    ///     [NDORelation]
    ///     [ForeignKeyColumn(Name = "IDOwner")]
    ///     [ForeignKeyColumn(Name = "IDRelatedType", NetType="System.Guid,mscorlib")] // wrong type
    ///     List&lt;RelatedType&gt; relatedObjects;
    ///     ...
    /// }
    /// 
    /// [NDOPersistent]
    /// [OidColumn(NetType="System.Int32,mscorlib")]  // foreign key column must match this type
    /// public class RelatedType
    /// {
    ///     ...
    /// }
    /// </code>
    /// It's strongly recommended not to specify a column type, since NDO determines the column type 
    /// of the foreign key columns automatically.
    /// Note: In order to avoid versioning problems if a new .NET version appears, 
    /// do not use a string like that:
    /// <code>
    /// System.Int32,mscorlib, Version=2.0.0.0, PublicKeyToken=abc1234
    /// </code>
    /// Instead provide the assembly name "mscorlib" (or any other assembly name) 
    /// without a version string as shown in the sample above. 
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ChildForeignKeyColumnAttribute : ColumnAttribute
    {
        // Currently no additional properties
    }
}
