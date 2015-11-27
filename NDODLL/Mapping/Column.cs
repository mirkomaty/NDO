using System;
using System.ComponentModel;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a column mapping.
    /// </summary>
    /// <remarks>This class is equivalent to the Column element of the mapping file schema.</remarks>
    public class Column : MappingNode
    {
        #region State variables and accessors

        string name;
        /// <summary>
        /// Gets or sets the name of the database column.
        /// </summary>
        [Description("Name of the database column.")]
        public string Name
        {
            get { return name; }
            set { name = value; this.Changed = true; }
        }

        string netType;
        /// <summary>
        /// In certain situations (i.e. Oid's) this tells NDO, which .NET type should be used to store members of this column. Leave this blank, if you don't have to deal with Oid's or Type columns.
        /// </summary>
        [Description("In certain situations (i.e. Oid's) this tells NDO, which .NET type should be used to store members of this column. Leave this blank, if you don't have to deal with Oid's or Type columns.")]
        public string NetType
        {
            get { return netType; }
            set { netType = value; this.Changed = true; }
        }

        Type systemType;
        /// <summary>
        /// Returns the .NET type according to the NetType property.
        /// </summary>
        [Browsable(false)]
        public virtual Type SystemType
        {
            get
            {
                if (this.systemType == null)
                {
                    if (this.netType == null)
                        return null;
                    this.systemType = Type.GetType(this.netType);
                }
                return this.systemType;
            }
            set
            {
                this.systemType = value;
                // don't adjust netType, because that would conflict with the enhancer.
            }
        }

        string dbType;
        /// <summary>
        /// Use this property, if you need another database type, than the one automatically assigned by the NDO provider.
        /// </summary>
        [Description("Use this property, if you need another database type, than the one automatically assigned by the NDO provider.")]
        public string DbType
        {
            get { return dbType; }
            set { dbType = value; this.Changed = true; }
        }

        int size;
        /// <summary>
        /// Gets or sets the size of the column.
        /// </summary>
        [Description("Size of the column.")]
        public int Size
        {
            get { return size; }
            set { size = value; this.Changed = true; }
        }
        int precision;
        /// <summary>
        /// Precision of the column. This appears in the DDL as (Size, Precision) tupel.
        /// </summary>
        [Description("Precision of the column. This appears in the DDL as (Size, Precision) tupel.")]
        public int Precision
        {
            get { return precision; }
            set { precision = value; this.Changed = true; }
        }

        private bool ignoreColumnSizeInDDL;
        /// <summary>
        /// True, if the DDL generator should not generate Size and Precision values for the given column.
        /// </summary>
        [Description("True, if the DDL generator should not generate Size and Precision values for the given column.")]
        public bool IgnoreColumnSizeInDDL
        {
            get { return ignoreColumnSizeInDDL; }
            set { ignoreColumnSizeInDDL = value; this.Changed = true; }
        }

        bool allowDbNull;

        /// <summary>
        /// Determines, if the column may contain NULL values.
        /// </summary>
        /// <remarks>
        /// The default value of this property is True. 
        /// NDO uses this property, to set the corresponding parameter in the DDL scripts. 
        /// NDO doesn't check the actual values passed as parameters. If you disallow null
        /// values for a certain column, make sure that the corresponding fields in your application
        /// don't contain null values before an object is going to be saved.
        /// </remarks>
        [Description("Determines, if the column may contain NULL values.")]
        public bool AllowDbNull
        {
            get { return allowDbNull; }
            set { allowDbNull = value; this.Changed = true; }
        }


        /// <summary>
        /// Returns the parent node of the Column object. The type of the parent depends on the type of the Column object.
        /// </summary>
        [Browsable(false)]
        public MappingNode Parent
        {
            get { return nodeParent; }
        }

        /// <summary>
        /// Calls to this method will be executed by the overrides in the derived classes.
        /// </summary>
        public override void Remove()
        {
            throw new InternalException(2994, "Column.Remove should be called by override. Type: " + this.GetType().FullName);
        }

        #endregion

        #region Constructors and Save Method

        /// <summary>
        /// Constructs a Column element and assigns it to the given parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        public Column(MappingNode parent)
            : base(parent)
        {
            size = 0;
            precision = 0;
            ignoreColumnSizeInDDL = false;
            allowDbNull = true;
        }

        internal Column(XmlNode columnNode, MappingNode parent)
            : base(columnNode, parent)
        {
            if (null != columnNode.Attributes["Name"])
                this.name = columnNode.Attributes["Name"].Value;
            else
                this.name = null;

            if (null != columnNode.Attributes["NetType"])
                this.netType = columnNode.Attributes["NetType"].Value;
            else
                this.netType = null;

            if (null != columnNode.Attributes["DbType"])
                this.dbType = columnNode.Attributes["DbType"].Value;
            else
                this.dbType = null;

            if (null != columnNode.Attributes["Size"])
                this.size = int.Parse(columnNode.Attributes["Size"].Value);
            else
                this.size = 0;

            if (null != columnNode.Attributes["Precision"])
                this.precision = int.Parse(columnNode.Attributes["Precision"].Value);
            else
                this.precision = 0;

            if (null != columnNode.Attributes["IgnoreColumnSizeInDDL"])
                ignoreColumnSizeInDDL = bool.Parse(columnNode.Attributes["IgnoreColumnSizeInDDL"].Value);
            else
                ignoreColumnSizeInDDL = false;

            if (null != columnNode.Attributes["AllowDbNull"])
                this.allowDbNull = bool.Parse(columnNode.Attributes["AllowDbNull"].Value);
            else
                this.allowDbNull = true;
        }

        internal void SaveOwnNode(XmlElement columnNode)
        {
            if (!string.IsNullOrEmpty(this.name))
                columnNode.SetAttribute("Name", this.name);
            if (null != netType && "" != netType)
                columnNode.SetAttribute("NetType", this.netType);
            if (null != dbType && "" != dbType)
                columnNode.SetAttribute("DbType", this.dbType);
            if (0 != this.size)
                columnNode.SetAttribute("Size", this.size.ToString());
            if (0 != precision)
                columnNode.SetAttribute("Precision", this.precision.ToString());
            if (this.ignoreColumnSizeInDDL)
                columnNode.SetAttribute("IgnoreColumnSizeInDDL", "True");
            if (!this.allowDbNull)
                columnNode.SetAttribute("AllowDbNull", "False");
        }

        internal virtual void Save(XmlNode parentNode)
        {
            XmlElement columnNode = parentNode.OwnerDocument.CreateElement("Column");
            parentNode.AppendChild(columnNode);

            base.SaveProperties(columnNode);   // Save properties
            SaveOwnNode(columnNode);
        }

        #endregion

        public override string ToString()
        {
            return this.name;
        }
    }
}
