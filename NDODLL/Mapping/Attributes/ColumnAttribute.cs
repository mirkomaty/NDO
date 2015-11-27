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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NDO.Mapping.Attributes
{
    /// <summary>
    /// This attribute is used to place hints for the mapping information of columns in the source code
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ColumnAttribute : Attribute
    {
        string name;
        /// <summary>
        /// Gets or sets the name of the database column.
        /// </summary>
        [Description("Name of the database column.")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        Type netType;
        /// <summary>
        /// In certain situations (i.e. Oid's) this tells NDO, which .NET type should be used to store members of this column. Leave this blank, if you don't have to deal with Oid's or Type columns.
        /// </summary>
        [Description("This tells NDO, which .NET type should be used to store members of this column. Leave this blank, if you don't have to deal with Oid's or Type columns.")]
        public Type NetType
        {
            get { return netType; }
            set { netType = value; }
        }

        string dbType;
        /// <summary>
        /// Use this property, if you need another database type, than the one automatically assigned by the NDO provider.
        /// </summary>
        [Description("Use this property, if you need another database type, than the one automatically assigned by the NDO provider.")]
        public string DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        int? size;
        /// <summary>
        /// Gets or sets the size of the column.
        /// </summary>
        [Description("Size of the column.")]
        public int Size
        {
            get { return size.HasValue ? this.size.Value : 0; }
            set { size = value; }
        }

        int? precision;
        /// <summary>
        /// Precision of the column. This appears in the DDL as (Size, Precision) tupel.
        /// </summary>
        [Description("Precision of the column. This appears in the DDL as (Size, Precision) tupel.")]
        public int Precision
        {
            get { return precision.HasValue ? precision.Value : 0; }
            set { precision = value; }
        }

        private bool? ignoreColumnSizeInDDL;
        /// <summary>
        /// True, if the DDL generator should not generate Size and Precision values for the given column.
        /// </summary>
        [Description("True, if the DDL generator should not generate Size and Precision values for the given column.")]
        public bool IgnoreColumnSizeInDDL
        {
            get { return ignoreColumnSizeInDDL.HasValue ? ignoreColumnSizeInDDL.Value : false; }
            set { ignoreColumnSizeInDDL = value; }
        }

        bool? allowDbNull = true;

        /// <summary>
        /// Determines, if the column may contain NULL values.
        /// </summary>
        /// <remarks>
        /// For a OidColumnAttribute the default value of this property is False, otherwise it is True. 
        /// NDO uses this property, to set the corresponding parameter in the DDL scripts. 
        /// NDO doesn't check the actual values passed as parameters. If you disallow null
        /// values for a certain column, make sure that the corresponding fields in your application
        /// don't contain null values before an object is going to be saved.
        /// </remarks>
        [Description("Determines, if the column may contain NULL values.")]
        public bool AllowDbNull
        {
            get { return allowDbNull.HasValue ? allowDbNull.Value : true; }
            set { allowDbNull = value; }
        }

        /// <summary>
        /// Determines if a given Column complies to the requirements defined by the ColumnAttribute instance.
        /// </summary>
        /// <param name="column">The column to check.</param>
        /// <returns>True, if the column complies to the requirements defined by the ColumnAttribute instance.</returns>
        public bool MatchesColumn(Column column)
        {
            if (this.allowDbNull != column.AllowDbNull)
                return false;
            if (this.dbType != null && this.dbType != column.DbType)
                return false;
            if (this.ignoreColumnSizeInDDL != column.IgnoreColumnSizeInDDL)
                return false;
            if (this.name != null && this.name != column.Name)
                return false;
            if (this.netType != null && this.netType != Type.GetType(column.NetType))
                return false;
            if (this.precision != column.Precision)
                return false;
            if (this.size != column.Size)
                return false;
            return true;
        }

        /// <summary>
        /// Initializes a given column to the values defined in this ColumnAttribute.
        /// </summary>
        /// <param name="column">The column to be initialized.</param>
        public void SetColumnValues(Column column)
        {
			if (this.allowDbNull.HasValue)
				column.AllowDbNull = this.allowDbNull.Value;
            if (this.dbType != null)
                column.DbType = this.dbType;
			if (this.ignoreColumnSizeInDDL.HasValue)
				column.IgnoreColumnSizeInDDL = this.ignoreColumnSizeInDDL.Value;
            if (this.name != null) 
                column.Name = this.name;
            if (this.netType != null)
                column.NetType = this.netType.FullName + "," + this.netType.Assembly.GetName().Name;
			if (this.precision.HasValue)
				column.Precision = this.precision.Value;
			if (this.size.HasValue)
				column.Size = this.size.Value;
        }

        /// <summary>
        /// Creats a column object and initializes it to the values defined in this ColumnAttribute.
        /// </summary>
        /// <returns>A new Column object.</returns>
        public Column CreateColum(MappingNode parent)
        {
            Column column = new Column(parent);
            SetColumnValues(column);
            return column;
        }

    }
}
