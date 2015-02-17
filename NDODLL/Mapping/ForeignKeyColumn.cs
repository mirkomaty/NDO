using System;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates a foreign key column. It's a column with some
    /// additional data.
    /// </summary>
    public class ForeignKeyColumn : Column
    {
        #region Constructors and Save Method

        /// <summary>
        /// Constructs a Column element and assigns it to the given parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        public ForeignKeyColumn(MappingNode parent)
            : base(parent)
        {
        }


        internal ForeignKeyColumn(XmlNode columnNode, MappingNode parent)
            : base(columnNode, parent)
        {
        }

        internal override void Save(XmlNode parentNode)
        {
            XmlElement fkColumnNode = parentNode.OwnerDocument.CreateElement("ForeignKeyColumn");
            parentNode.AppendChild(fkColumnNode);
            base.SaveOwnNode(fkColumnNode);  // Saves properties too
        }
        #endregion

        /// <summary>
        /// Removes the ForeignKeyColumn object from the list of ForeignKeyColumns in the parent object.
        /// </summary>
        public override void Remove()
        {
            Relation r = Parent as Relation;
            if (r != null)
            {
                r.RemoveForeignKeyColumn(this);
                return;
            }
            MappingTable mt = Parent as MappingTable;
            if (mt != null)
            {
                mt.RemoveChildForeignKeyColumn(this);
            }
        }
    }
}
