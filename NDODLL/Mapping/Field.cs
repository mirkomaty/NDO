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
        [Browsable(false)]
        public Column Column
        {
            get { return this.column; }
        }

        [Browsable(false)]
        public Class Parent
        {
            get { return nodeParent as Class; }
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
			if (this.accessorName != null)
				fieldNode.SetAttribute( "AccessorName", accessorName );
            this.column.Save(fieldNode);
        }
        #endregion

        #region IComparable Member

        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(this.Name, ((Field)obj).Name);
        }

        #endregion
    }
}
