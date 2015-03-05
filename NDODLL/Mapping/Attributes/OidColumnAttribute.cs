//
// Copyright (C) 2002-2014 Mirko Matytschak 
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
using System.ComponentModel;

namespace NDO.Mapping.Attributes
{
    /// <summary>
    /// This attribute is used to place hints for the mapping information of Oid columns in the source code. 
    /// If no OidColumnAttribute is used, NDO tries to us an autoincremented integer column. If the database
    /// product doesn't support autoincremented columns, an integer column is used.
    /// </summary>
    /// <remarks>
    /// You can set the value of an Oid column using an <see cref="NDO.IdGenerationHandler">IdGenerationHandler</see> 
    /// or you can map a field of your class to an Oid.
    /// The following sample shows, how to define a Guid Oid.
    /// <code>
    /// [OidColumn(typeof(Guid))]
    /// [NDOPersistent]
    /// public class MyClass {...}
    /// </code>
    /// The following sample shows, how to define a string Oid.
    /// <code>
    /// [OidColumn(typeof(String))]
    /// [NDOPersistent]
    /// public class MyClass {...}
    /// ...
    /// PersistenceManager pm = new PersistenceManager();
    /// pm.IdGenerationEvent += new IdGenerationHandler(this.OnIdGeneration);
    /// MyClass myObject = new MyClass();
    /// pm.MakePersistent(myObject); // calls OnIdGeneration
    /// ...
    /// void OnIdGeneration(type t, ObjectId oid)
    /// {
    ///     if (t == typeof(MyClass))
    ///     {
    ///         string oidVal = ComputeMyOidValue();
    ///         oid.Id[0] = oidVal;
    ///     }
    /// }
    /// </code>
    /// The following sample shows, how to map a string Oid to a field of the class.
    /// <code>
    /// [OidColumn(FieldName="myoid")]
    /// [NDOPersistent]
    /// public class MyClass 
    /// {
    ///     string myoid;  // will hold the oid value
    ///     public string Myoid
    ///     {
    ///         get { return this.myoid; }
    ///         set { this.myoid = value; }
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public class OidColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// Constructs an OidColumnAttribute instance.
        /// </summary>
        public OidColumnAttribute()
        {
            this.AllowDbNull = false;
        }

        /// <summary>
        /// Constructs an OidColumnAttribute instance and initializes the type to the given Type.
        /// </summary>
        /// <param name="netType">The type of the oid.</param>
        /// <remarks>
        /// Valid oid types are Int32, Int64, String, and Guid
        /// </remarks>
        public OidColumnAttribute(Type netType)
            : this()
        {
            this.NetType = netType;
        }


        public OidColumnAttribute(bool autoIncremented)
            : this()
        {
            this.autoIncremented = autoIncremented;
        }


        string fieldName;
        /// <summary>
        /// This property is used to map a oid column to a field. Leave this property blank, if the oid column doesn't have a mapping to a field.
        /// </summary>
        [Description("This property is used to map a oid column to a field. Leave this property blank, if the oid column doesn't have a mapping to a field.")]
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

        string relationName;
        /// <summary>
        /// This property is used to map a oid column to a relation. Leave this property blank, if the oid column doesn't have a mapping to a relation.
        /// </summary>
        [Description("This property is used to map a oid column to a relation. Leave this property blank, if the oid column doesn't have a mapping to a relation.")]
        public string RelationName
        {
            get { return relationName; }
            set { relationName = value; }
        }

        bool autoIncremented;
        /// <summary>
        /// Gets or sets a value, which determines if the column is autoincremented.
        /// </summary>
        [Description("This property determines if the column is autoincremented.")]
        public bool AutoIncremented
        {
            get { return autoIncremented; }
            set { autoIncremented = value; }
        }

        int autoIncrementStart = 1;
        /// <summary>
        /// If a column is autoincremented, this determines the start value. The value defaults to 1.
        /// </summary>
        [Description("If a column is autoincremented, this determines the start value. The value defaults to 1.")]
        public int AutoIncrementStart
        {
            get { return autoIncrementStart; }
            set { autoIncrementStart = value; }
        }

        int autoIncrementStep = 1;
        /// <summary>
        /// If a column is autoincremented, this determines the step value. The value defaults to 1.
        /// </summary>
        [Description("If a column is autoincremented, this determines the step value. The value defaults to 1.")]
        public int AutoIncrementStep
        {
            get { return autoIncrementStep; }
            set { autoIncrementStep = value; }
        }


        /// <summary>
        /// Determines if a given OidColumn complies to the requirements defined by this OidColumnAttribute instance.
        /// </summary>
        /// <param name="oidColumn">The OidColumn to check.</param>
        /// <returns>True, if the OidColumn complies to the requirements defined by this OidColumnAttribute instance.</returns>
        public bool MatchesOidColumn(OidColumn oidColumn)
        {
            if (this.autoIncremented != oidColumn.AutoIncremented)
                return false;
            if (this.autoIncrementStart != oidColumn.AutoIncrementStart)
                return false;
            if (this.autoIncrementStep != oidColumn.AutoIncrementStep)
                return false;
            if (this.fieldName != oidColumn.FieldName)  // No Test for null, because column must be null too, if attribute defines null
                return false;
            if (this.relationName != oidColumn.RelationName)  // No Test for null, because column must be null too, if attribute defines null
                return false;
            return base.MatchesColumn(oidColumn);
        }

        /// <summary>
        /// Initializes a given column to the values defined in this ColumnAttribute.
        /// </summary>
        /// <param name="column"></param>
        public void SetOidColumnValues(OidColumn column)
        {
            column.AutoIncremented = this.autoIncremented;
            column.AutoIncrementStart = this.autoIncrementStart;
            column.AutoIncrementStep = this.autoIncrementStep;
            column.FieldName = this.fieldName;
            column.RelationName = this.relationName;
            base.SetColumnValues(column);
        }

        /// <summary>
        /// Creates a new OidColumn object and initializes it to the values defined in this OidColumnAttribute.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public OidColumn CreateOidColumn(ClassOid parent)
        {
            OidColumn oidColumn = new OidColumn(parent);
            SetOidColumnValues(oidColumn);
            return oidColumn;
        }


    }
}
