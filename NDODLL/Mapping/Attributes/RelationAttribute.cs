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
using System.Collections.Generic;
using System.Text;

namespace NDO.Mapping.Attributes
{
    /// <summary>
    /// Use this attribute to influence the properties of a Relation entry in the mapping file.
    /// </summary>
    /// <remarks>
    /// Note: To determine the semantics of the relation, use the <see cref="NDO.NDORelationAttribute">NDORelationAttribute</see> class.
    /// Note: To influence the mapping entries for the foreign key columns use <see cref="NDO.Mapping.Attributes.ForeignKeyColumnAttribute">ForeignKeyColumnAttribute</see> and
    /// <see cref="ChildForeignKeyColumnAttribute">ChildForeignKeyColumnAttribute</see>.
    /// The sample below shows, how to define a relation using different attributes.
    /// <code>
    /// [NDOPersistent]
    /// public class Owner
    /// {
    ///     [NDORelation]
    ///     [Relation(ForeignKeyTypeColumnName = "TCRelatedType")]
    ///     [ForeignKeyColumn(Name = "IDRelatedType")]
    ///     List&lt;RelatedType&gt; relatedObjects;
    ///     ...
    /// }
    /// 
    /// [NDOPersistent]
    /// public class RelatedType
    /// {
    ///     ...
    /// }
    /// [NDOPersistent]
    /// public class DerivedRelatedType : RelatedType
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class RelationAttribute : Attribute
    {
        string foreignKeyTypeColumnName;
        /// <summary>
        /// Determines the name of the column holding the type code of a polymorphic owner type. 
        /// Do not provide a ForeignKeyTypeColumnName for a non-polymorphic relation.
        /// </summary>
        public string ForeignKeyTypeColumnName
        {
            get { return foreignKeyTypeColumnName; }
            set { foreignKeyTypeColumnName = value; }
        }
    }
}
