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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using NDO;

namespace NorthwindGUI
{
    partial class ObjectPropertyForm : Form
    {        
        IPersistenceCapable displayedObject;
        bool isSearchTemplate;
        int controlCount = 0;
        int tabIndex = 0;

        const int indent = 30;
        const int labelwidth = 140;
        const int inputwidth = 173;
        const int columnGap = 15;
        const int columnDistance = labelwidth + inputwidth + columnGap;
        const int formWidth = indent * 2 + labelwidth * 2 + inputwidth * 2 + columnGap;

        static NDO.Mapping.NDOMapping mapping;

        List<FieldEntry> searchParams = new List<FieldEntry>();
        public List<FieldEntry> SearchParams
        {
            get { return searchParams; }
        }


        class BindingEntry
        {
            public BindingEntry(FieldEntry fe, Control c)
            {
                this.FieldEntry = fe;
                this.Control = c;
            }
            public FieldEntry FieldEntry;
            public Control Control;
        }

        List<BindingEntry> bindings = new List<BindingEntry>();

        static ObjectPropertyForm()
        {
            PersistenceManager pm = new PersistenceManager();
            mapping = pm.NDOMapping;
        }

        public ObjectPropertyForm(object displayedObject, bool isSearchTemplate)
        {
            this.displayedObject = (IPersistenceCapable) displayedObject;
            this.isSearchTemplate = isSearchTemplate;
            InitializeComponent();

            SuspendLayout();
            this.lblRetrieveAll.Visible = isSearchTemplate;

            this.Text = displayedObject.GetType().Name + " Properties";

            List<FieldEntry> fieldEntries = FieldEntry.GetFieldEntries(displayedObject.GetType(), mapping);
            foreach (FieldEntry fe in fieldEntries)
            {
                string name = fe.FieldMapping.Column.Name;
                Type t = fe.FieldInfo.FieldType;
                FieldInfo fi = fe.FieldInfo;
                CreateLabel(name);
                if (t == typeof(DateTime))
                {
                    DateTimePicker dtp = CreateDateTimePicker(name);
                    if (!isSearchTemplate)
                    {
                        DateTime dt = (DateTime)fi.GetValue(this.displayedObject);
                        if (dt == DateTime.MinValue)
                            dt = dtp.MinDate;
                        dtp.Value = dt;
                    }
                    else
                    {
                        dtp.Value = dtp.MinDate;
                    }
                    bindings.Add(new BindingEntry(fe, dtp));
                }
                else if (t == typeof(bool))
                {
                    CheckBox cb = CreateCheckBox(name);
                    cb.Checked = (bool) fi.GetValue(this.displayedObject);
                    bindings.Add(new BindingEntry(fe, cb));
                    if (isSearchTemplate)
                        cb.CheckState = CheckState.Indeterminate;
                }
                else
                {
                    TextBox tb = CreateTextBox(name);
                    if (!isSearchTemplate)
                    {
                        object o = fi.GetValue(this.displayedObject);
                        tb.Text = (null == o ? string.Empty : o.ToString());
                    }
                    else
                    {
                        tb.Text = string.Empty;
                    }
                    bindings.Add(new BindingEntry(fe, tb));
                }
            }

            this.ClientSize = new Size(formWidth, (Row + 1) * 36 + 100);
            
            ResumeLayout(true);
        }

        Label CreateLabel(string fieldName)
        {
            controlCount++;
            Label label = new Label();
            label.AutoSize = true;
            label.Location = new System.Drawing.Point(indent + Column * columnDistance, 24 + 36 * Row);
            label.Name = "label" + controlCount;
            label.Size = new System.Drawing.Size(labelwidth, 13);
            label.TabIndex = this.tabIndex++;
            label.Text = fieldName;
            this.Controls.Add(label);
            return label;
        }

        TextBox CreateTextBox(string fieldName)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new System.Drawing.Point(indent + labelwidth + Column * columnDistance, 24 + 36 * Row);
            textBox.Name = "txt" + fieldName;
            textBox.Size = new System.Drawing.Size(inputwidth, 20);
            textBox.TabIndex = this.tabIndex++;
            this.Controls.Add(textBox);
            return textBox;
        }

        CheckBox CreateCheckBox(string fieldName)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.AutoSize = true;
            checkBox.Location = new System.Drawing.Point(indent + labelwidth + Column * columnDistance + 3, 25 + 36 * Row);
            checkBox.Name = "chk" + fieldName;
            checkBox.Size = new System.Drawing.Size(15, 14);
            checkBox.TabIndex = this.tabIndex++;
            checkBox.UseVisualStyleBackColor = true;
            this.Controls.Add(checkBox);
            return checkBox;
        }

        DateTimePicker CreateDateTimePicker(string fieldName)
        {
            DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dateTimePicker.Location = new System.Drawing.Point(indent + labelwidth + Column * columnDistance, 24 + 36 * Row);
            dateTimePicker.Name = "dateTimePicker1";
            dateTimePicker.Size = new System.Drawing.Size(116, 20);
            dateTimePicker.TabIndex = this.tabIndex++;
            this.Controls.Add(dateTimePicker);
            return dateTimePicker;
        }

        int Column
        {
            get { return (controlCount - 1) % 2; }
        }

        int Row
        {
            get { return (controlCount - 1) / 2; }
        }

        void AssignProperties()
        {
            // We check first, if there is a property change.
            // If there was no property change, the object won't be touched 
            // and thus won't be saved.
            foreach (BindingEntry be in this.bindings)
            {
                FieldEntry fe = be.FieldEntry;
                FieldInfo fi = fe.FieldInfo;
                Control control = be.Control;

                if (fi.FieldType == typeof(DateTime))
                {
                    DateTime oldDt = (DateTime)fi.GetValue(displayedObject);
                    DateTime newDt = ((DateTimePicker)control).Value;
                    if (newDt == ((DateTimePicker)control).MinDate)
                        newDt = DateTime.MinValue;
                    if (oldDt != newDt)
                    {
                        fi.SetValue(this.displayedObject, newDt);
                        this.displayedObject.NDOMarkDirty();
                    }
                }
                else if (fi.FieldType == typeof(bool))
                {
                    bool oldBool = (bool)fi.GetValue(displayedObject);
                    bool newBool = ((CheckBox)control).Checked;
                    if (oldBool != newBool)
                    {
                        fi.SetValue(this.displayedObject, newBool);
                        this.displayedObject.NDOMarkDirty();
                    }
                }
                else if (fi.FieldType == typeof(string))
                {
                    string oldString = (string)fi.GetValue(displayedObject);
                    if (oldString == null)
                        oldString = string.Empty;
                    string newString = ((TextBox)control).Text.Trim();
                    if (oldString != newString)
                    {
                        fi.SetValue(this.displayedObject, newString);
                        this.displayedObject.NDOMarkDirty();
                    }
                }
                else
                {
                    object oldObject = fi.GetValue(displayedObject);
                    string oldVal = (oldObject == null ? string.Empty : oldObject.ToString());

                    string newVal = ((TextBox)control).Text.Trim();
                    // Empty strings are invalid, because the PropertyType is not a string.
                    if (newVal != string.Empty && oldVal != newVal)
                    {
                        object o = null;
                        try
                        {
                            o = System.Convert.ChangeType(newVal, fi.FieldType);
                        }
                        catch { } // Input not convertible
                        if (o != null)
                            fi.SetValue(this.displayedObject, o);

                    }
                }
            }
        }


        void ConstructSearchParameters()
        {
            foreach (BindingEntry be in this.bindings)
            {
                Type t = be.FieldEntry.FieldInfo.FieldType;
                string name = be.FieldEntry.FieldInfo.Name;
                FieldEntry fe = be.FieldEntry;
                if (t == typeof(bool))
                {
                    CheckBox cb = (CheckBox) be.Control;
                    if (cb.CheckState != CheckState.Indeterminate)
                    {
                        fe.Value = cb.Checked;
                        this.searchParams.Add(fe);
                    }
                }
                else if (t == typeof(DateTime))
                {
                    DateTimePicker dtp = (DateTimePicker)be.Control;
                    if (dtp.Value != dtp.MinDate)
                    {
                        fe.Value = dtp.Value;
                        this.searchParams.Add(fe);
                    }
                }
                else
                {
                    TextBox tb = (TextBox)be.Control;
                    if (tb.Text.Trim() != string.Empty)
                    {
                        try
                        {
                            fe.Value = Convert.ChangeType(tb.Text, t);
                        }
                        catch {}
                        if (fe.Value != null)
                            this.searchParams.Add(fe);
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!isSearchTemplate)
                AssignProperties();
            else
                ConstructSearchParameters();
        }
    }
}