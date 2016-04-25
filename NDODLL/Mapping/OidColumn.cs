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


using NDOInterfaces;
using System;
using System.ComponentModel;
using System.Xml;

namespace NDO.Mapping
{
    /// <summary>
    /// This class encapsulates an Oid column. It contains some more data than its 
    /// base class Column.
    /// </summary>
    public class OidColumn : Column
    {
        #region State variables and accessors
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
            set { autoIncremented = value; this.Changed = true; }
        }

        int autoIncrementStart = 1;
        /// <summary>
        /// If a column is autoincremented, this determines the start value. If the value not set NDO assumes 1.
        /// </summary>
        [Description("If a column is autoincremented, this determines the start value. If the value not set NDO assumes 1.")]
        public int AutoIncrementStart
        {
            get { return autoIncrementStart; }
            set { autoIncrementStart = value; this.Changed = true; }
        }

        int autoIncrementStep = 1;
        /// <summary>
        /// If a column is autoincremented, this determines the step value. If the value not set NDO assumes 1.
        /// </summary>
        [Description("If a column is autoincremented, this determines the step value. If the value not set NDO assumes 1.")]
        public int AutoIncrementStep
        {
            get { return autoIncrementStep; }
            set { autoIncrementStep = value; this.Changed = true; }
        }


        /// <summary>
        /// Parent class of relation
        /// </summary>
        [Browsable(false)]
        public new ClassOid Parent
        {
            get { return nodeParent as ClassOid; }
        }

        /// <summary>
        /// Removes the OidColumn from the OidColumn list of the parent object.
        /// </summary>
        public override void Remove()
        {
            Parent.OidColumns.Remove(this);
        }

        [Browsable(false)]
        internal int TypeLength
        {
            get
            {
                Type fieldType = this.SystemType;
                if (fieldType == typeof(Int64))
                    return 8;
                if (fieldType == typeof(Int32))
                    return 4;
                if (fieldType == typeof(string))
                    return 255;
                // OidColumn->Oid->Class->NDOMapping
                NDOMapping m = this.Parent.Parent.Parent;
                IProvider p = m.GetProvider(this.Parent.Parent);
                if (p.SupportsNativeGuidType)
                    return 16;
                else
                    return 36;
            }
        }


        [Browsable(false)]
        internal Relation Relation
        {
            get
            {
                if (this.RelationName == null)
                    return null;
                return Parent.Parent.FindRelation(this.relationName);
            }
        }


        #endregion

        #region Constructors and Save Method

        /// <summary>
        /// Constructs a Column element and assigns it to the given parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        public OidColumn(ClassOid parent)
            : base(parent)
        {
            base.AllowDbNull = false;
        }

        internal OidColumn(XmlNode columnNode, ClassOid parent)
            : base(columnNode, parent)
        {

            if (null != columnNode.Attributes["FieldName"])
                this.fieldName = columnNode.Attributes["FieldName"].Value;
            else
                this.fieldName = null;
            if (null != columnNode.Attributes["RelationName"])
                this.relationName = columnNode.Attributes["RelationName"].Value;
            else
                this.relationName = null;

            if (null != columnNode.Attributes["AutoIncremented"])
                this.autoIncremented = bool.Parse(columnNode.Attributes["AutoIncremented"].Value);
            else
                this.autoIncremented = false;

            if (this.autoIncremented)
            {
                if (null != columnNode.Attributes["AutoIncrementStart"])
                    this.autoIncrementStart = int.Parse(columnNode.Attributes["AutoincrementStart"].Value);
                //else // the value is defaulted to 1
                //    this.autoIncrementStart = 1;

                if (null != columnNode.Attributes["AutoIncrementStep"])
                    this.autoIncrementStep = int.Parse(columnNode.Attributes["AutoincrementStep"].Value);
                //else // the value is defaulted to 1
                //    this.autoIncrementStep = 1;
            }

        }

        internal override void Save(XmlNode parentNode)
        {
            XmlElement oidColumnNode = parentNode.OwnerDocument.CreateElement("OidColumn");
            parentNode.AppendChild(oidColumnNode);
            if (null != this.fieldName && "" != this.fieldName)
                oidColumnNode.SetAttribute("FieldName", this.fieldName);
            if (null != this.relationName && "" != this.relationName)
                oidColumnNode.SetAttribute("RelationName", this.relationName);

            if (this.autoIncremented)
            {
                oidColumnNode.SetAttribute("AutoIncremented", "True");
                if (this.autoIncrementStart != 1)
                    oidColumnNode.SetAttribute("AutoIncrementStart", this.autoIncrementStart.ToString());
                if (this.autoIncrementStep != 1)
                    oidColumnNode.SetAttribute("AutoIncrementStep", this.autoIncrementStep.ToString());
            }

            base.SaveOwnNode(oidColumnNode);  // Saves properties too
        }
        #endregion
    }
}
