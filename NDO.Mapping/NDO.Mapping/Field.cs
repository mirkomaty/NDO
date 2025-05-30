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
using System.ComponentModel;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a field mapping
    /// </summary>
    /// <remarks>This class is equivalent to the Field element of the mapping file schema.</remarks>
    public class Field : MappingNode, IComparable
    {
        #region State variables and accessors
        /// <summary>
        /// Field name 
        /// </summary>
        [ReadOnly(true), Description("Field name")]
        public string Name
        {
            get { return name; }
            set { name = value; this.Changed = true; }
        }
        private string name;
		private bool encrypted;

		/// <summary>
		/// Determines, if the field should be encrypted
		/// </summary>
		[Description( "Determines, if the field should be encrypted" )]	
		public bool Encrypted
		{
			get { return this.encrypted; }
			set { this.encrypted = value; this.Changed = true; }
		}

        /// <summary>
        /// Accessor name 
        /// </summary>
        [Description("Accessor name")]
        public string AccessorName
        {
            get { return accessorName; }
            set { accessorName = value; this.Changed = true; }
        }
        private string accessorName;


        /// <summary>
        ///  This is used only by the NDOPersistenceHander class. Don't use this member - it will only be initialized in rare situations.
        /// </summary>
        [Browsable(false)]
        internal object ColumnDbType
        {
            get { return columnDbType; }
            set { columnDbType = value; }
        }
        private object columnDbType;

        Column column;
        /// <summary>
        /// Gets the column to which the field is associated.
        /// </summary>
        [Browsable(false)]
        public Column Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Gets the parent object
        /// </summary>
        [Browsable(false)]
        public Class Parent
        {
            get { return NodeParent as Class; }
        }

        /// <summary>
        /// Removes the Field object from the field mappings list of the parent object.
        /// </summary>
        public override void Remove()
        {
            Parent.RemoveField(this);
        }

        int ordinal;
        /// <summary>
        /// This field is used by NDO for internal purposes. Don't use this field.
        /// </summary>
        [Browsable(false)]
        public int Ordinal
        {
            get { return ordinal; }
            set { ordinal = value; }
        }

        #endregion

        #region Constructors and Save function

        /// <summary>
        /// Constructs a new field
        /// </summary>
        public Field(Class parent)
            : base(parent)
        {
            Name = "";
            this.column = new Column(this);
        }

        internal Field(XmlNode fieldNode, Class parent)
            : base(fieldNode, parent)
        {
            Name = fieldNode.Attributes["Name"].Value;
			var attr = fieldNode.Attributes["Encrypted"];
			if (attr != null)
				this.encrypted = bool.Parse( attr.Value );

			if (null != fieldNode.Attributes["AccessorName"])
			{
				this.accessorName = fieldNode.Attributes["AccessorName"].Value;
			}
            if (fieldNode.Attributes["ColumnName"] != null)  // Legacy mapping files
            {
                this.column = new Column(this);
                this.column.Name = fieldNode.Attributes["ColumnName"].Value;
                if (null != fieldNode.Attributes["ColumnType"])
                    column.DbType = fieldNode.Attributes["ColumnType"].Value;
                else
                    column.DbType = null;
                if (null != fieldNode.Attributes["ColumnLength"])
                    column.Size = int.Parse(fieldNode.Attributes["ColumnLength"].Value);
                else
                    column.Size = 0;
                if (null != fieldNode.Attributes["IgnoreLengthInDDL"])
                    column.IgnoreColumnSizeInDDL = bool.Parse(fieldNode.Attributes["IgnoreLengthInDDL"].Value);
                else
                    column.IgnoreColumnSizeInDDL = false;
                if (null != fieldNode.Attributes["ColumnPrecision"])
                    column.Precision = int.Parse(fieldNode.Attributes["ColumnPrecision"].Value);
                else
                    column.Precision = 0;
                if (null != fieldNode.Attributes["AllowDbNull"])
                    column.AllowDbNull = bool.Parse(fieldNode.Attributes["AllowDbNull"].Value);
                else
                    column.AllowDbNull = true;
            }
            else
            {
                // Field->Class->NDOMapping
                XmlNode columnNode = fieldNode.SelectSingleNode(parent.Parent.selectColumn);
                this.column = new Column(columnNode, this);
            }
        }

        internal void Save(XmlNode parentNode)
        {
            XmlElement fieldNode = parentNode.OwnerDocument.CreateElement("Field");
            parentNode.AppendChild(fieldNode);
            base.SaveProperties(fieldNode);
            fieldNode.SetAttribute("Name", name);
			if (!String.IsNullOrEmpty( this.accessorName ))
				fieldNode.SetAttribute( "AccessorName", accessorName );
			if (this.encrypted)
				fieldNode.SetAttribute( "Encrypted", "True" );
            this.column.Save(fieldNode);
        }
        #endregion

        #region IComparable Member

        ///<inheritdoc/>
        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(this.Name, ((Field)obj).Name);
        }

        #endregion
    }
}
