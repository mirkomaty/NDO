using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a mapping table for the connection of two data tables.
    /// </summary>
    /// <remarks>This class is equivalent to the MappingTable element of the mapping file schema.</remarks>
    public class MappingTable : MappingNode
    {
        #region State variables and accessors

        private string tableName = "";
        /// <summary>
        /// Name of the table, where the relation information is stored.
        /// </summary>
        [Description("Name of the table, where the relation information is stored.")]
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; this.Changed = true; }
        }
        private ArrayList childForeignKeyColumns = new ArrayList();
        /// <summary>
        /// Contains the description of the ForeignKeyColumns pointing to 
        /// the related type. Under normal circumstances this list contains exactly
        /// one entry. If the related type has a multi column oid there need to be
        /// more foreign key columns. The order of the column descriptions must match
        /// the order of the oid columns in the related type.
        /// </summary>
        [Browsable(false)]
        public IList ChildForeignKeyColumns
        {
            get { return this.childForeignKeyColumns; }
        }

        /// <summary>
        /// Adds a new ForeignKeyColumn to the mapping table.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Used by the enhancer and the mapping tool.
        /// </remarks>
        public ForeignKeyColumn NewForeignKeyColumn()
        {
            ForeignKeyColumn fkColumn = new ForeignKeyColumn(this);
            this.childForeignKeyColumns.Add(fkColumn);
            return fkColumn;
        }

        private string childForeignKeyTypeColumnName = null;
        /// <summary>
        /// Name of the column, where the type code of the related object is stored.
        /// </summary>
        [Description("Name of the column, where the type code of the related object is stored.")]
        public string ChildForeignKeyTypeColumnName
        {
            get { return childForeignKeyTypeColumnName; }
            set { childForeignKeyTypeColumnName = value; this.Changed = true; }
        }

        /// <summary>
        /// Returns the relation the MappingTable belongs to. 
        /// </summary>
        [Browsable(false)]
        Relation Parent
        {
            get { return (Relation)nodeParent; }
        }

        /// <summary>
        /// Deletes the MappingTable object from the parent object.
        /// Note: In some scenarios a mapping table is mandatory. 
        /// Deleting the mapping table entry might cause exceptions thrown by NDO.
        /// </summary>
        public override void Remove()
        {
            Parent.MappingTable = null;
        }

#if nix
		private string childForeignKeyColumnName = "";
		/// <summary>
		/// Name of the column, where the foreign key of the related object is stored.
		/// </summary>
		[Description("Name of the column, where the foreign key of the related object is stored.")]
		public string ChildForeignKeyColumnName
		{
			get { return childForeignKeyColumnName; }
			set { childForeignKeyColumnName = value; this.Changed = true; }
		}
#endif
        private string connectionId = "";
        /// <summary>
        /// Connection string of the database, where the mapping table is stored.
        /// </summary>
        [Description("Connection string of the database, where the mapping table is stored.")]
        public string ConnectionId
        {
            get { return connectionId; }
            set { connectionId = value; this.Changed = true; }
        }
        #endregion
        /// <summary>
        /// Construct a new MappingTable
        /// </summary>
        public MappingTable(Relation parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Construct a mapping table based on a MappingTable node
        /// </summary>
        /// <param name="mtNode">The node</param>
        /// <param name="parent">A Relation object, which is the parent of the mapping table.</param>
        internal MappingTable(XmlNode mtNode, Relation parent)
            : base(parent)
        {
            tableName = mtNode.Attributes["TableName"].Value;

            if (mtNode.Attributes["ChildForeignKeyTypeColumnName"] != null && mtNode.Attributes["ChildForeignKeyTypeColumnName"].Value != string.Empty)
                this.childForeignKeyTypeColumnName = mtNode.Attributes["ChildForeignKeyTypeColumnName"].Value;

            if (mtNode.Attributes["ChildForeignKeyColumnName"] != null) // Old mapping
            {
                ForeignKeyColumn fkColumn = new ForeignKeyColumn(this);
                fkColumn.Name = mtNode.Attributes["ChildForeignKeyColumnName"].Value;
                this.childForeignKeyColumns.Add(fkColumn);
            }
            else
            {
                XmlNodeList nl = mtNode.SelectNodes(parent.Parent.Parent.selectForeignKeyColumns);
                foreach (XmlNode fkcNode in nl)
                    this.childForeignKeyColumns.Add(new ForeignKeyColumn(fkcNode, this));
            }

            connectionId = mtNode.Attributes["ConnectionId"].Value;
        }

        internal void Save(XmlNode parentNode)
        {
            XmlElement mtNode = parentNode.OwnerDocument.CreateElement("MappingTable");
            parentNode.AppendChild(mtNode);
            base.SaveProperties(mtNode); // Save properties
            mtNode.SetAttribute("TableName", this.tableName);

            if (!string.IsNullOrEmpty(ChildForeignKeyTypeColumnName))
                mtNode.SetAttribute("ChildForeignKeyTypeColumnName", childForeignKeyTypeColumnName);

            foreach (ForeignKeyColumn fkColumn in this.childForeignKeyColumns)
                fkColumn.Save(mtNode);

            mtNode.SetAttribute("ConnectionId", connectionId);

#if nix
			if (null != childForeignKeyColumnName)
				mtNode.SetAttribute("ChildForeignKeyColumnName", childForeignKeyColumnName);
#endif
        }
    }
}
