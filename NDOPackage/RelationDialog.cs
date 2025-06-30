//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.Drawing;
using WinForms.FontSize;
using System.ComponentModel;
using System.Windows.Forms;

namespace NDOVsPackage
{
	internal enum ListType
	{
		IList,
		ArrayList
	}

	/// <summary>
	/// Zusammenfassung für RelationDialog.
	/// </summary>
	internal class RelationDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtFieldName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioSingle;
		private System.Windows.Forms.RadioButton radioList;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioComposite;
		private System.Windows.Forms.RadioButton radioAggregate;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtElementType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TextBox txtRoleName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioArrayList;
		private System.Windows.Forms.RadioButton radioIList;
		private System.Windows.Forms.CheckBox chkUseGenerics;
		private System.ComponentModel.IContainer components;

		public RelationDialog()
		{
			//
			// Erforderlich für die Windows Form-Designerunterstützung
			//
			InitializeComponent();
			// Calculate the new font size after InitializeComponent
			var newFontSize = FontCalculator.Calculate(Screen.FromControl(this), Font.Size);
			if (newFontSize > Font.Size)
				Font = new Font( Font.FontFamily, newFontSize, FontStyle.Regular, GraphicsUnit.Point, 0 );

			this.chkUseGenerics.Checked = true;			
		}

		public string FieldName
		{
			get { return this.txtFieldName.Text; }
		}

		public string Type
		{
			get { return this.txtElementType.Text; }
		}

		public bool Composite
		{
			get { return this.radioComposite.Checked; }
		}

		public bool List
		{
			get { return this.radioList.Checked; }
		}

		public string RelationName
		{
			get { return this.txtRoleName.Text; }
		}

		public ListType ListType
		{
			get 
			{
				if (radioIList.Checked)
					return ListType.IList;
				return ListType.ArrayList;
			}
		}

		public bool UseGenerics
		{
			get { return this.chkUseGenerics.Checked; }
		}


		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Vom Windows Form-Designer generierter Code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelationDialog));
			this.txtFieldName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioList = new System.Windows.Forms.RadioButton();
			this.radioSingle = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioAggregate = new System.Windows.Forms.RadioButton();
			this.radioComposite = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtElementType = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.txtRoleName = new System.Windows.Forms.TextBox();
			this.radioArrayList = new System.Windows.Forms.RadioButton();
			this.radioIList = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.chkUseGenerics = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtFieldName
			// 
			this.txtFieldName.Location = new System.Drawing.Point(137, 59);
			this.txtFieldName.Name = "txtFieldName";
			this.txtFieldName.Size = new System.Drawing.Size(267, 20);
			this.txtFieldName.TabIndex = 1;
			this.toolTip1.SetToolTip(this.txtFieldName, "Name of the variable in which the relation is stored");
			this.txtFieldName.TextChanged += new System.EventHandler(this.txtFieldName_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(44, 59);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 20);
			this.label1.TabIndex = 15;
			this.label1.Text = "Field Name";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioList);
			this.groupBox1.Controls.Add(this.radioSingle);
			this.groupBox1.Location = new System.Drawing.Point(230, 128);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(174, 90);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Multiplicity ";
			// 
			// radioList
			// 
			this.radioList.Checked = true;
			this.radioList.Location = new System.Drawing.Point(37, 60);
			this.radioList.Name = "radioList";
			this.radioList.Size = new System.Drawing.Size(120, 28);
			this.radioList.TabIndex = 1;
			this.radioList.TabStop = true;
			this.radioList.Text = "List";
			this.toolTip1.SetToolTip(this.radioList, "Relation to a list of objects");
			// 
			// radioSingle
			// 
			this.radioSingle.Location = new System.Drawing.Point(37, 35);
			this.radioSingle.Name = "radioSingle";
			this.radioSingle.Size = new System.Drawing.Size(120, 21);
			this.radioSingle.TabIndex = 0;
			this.radioSingle.Text = "Single Element";
			this.toolTip1.SetToolTip(this.radioSingle, "Relation to one single object");
			this.radioSingle.CheckedChanged += new System.EventHandler(this.radioSingle_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioAggregate);
			this.groupBox2.Controls.Add(this.radioComposite);
			this.groupBox2.Location = new System.Drawing.Point(44, 128);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(173, 90);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Relation Type  ";
			// 
			// radioAggregate
			// 
			this.radioAggregate.Location = new System.Drawing.Point(30, 60);
			this.radioAggregate.Name = "radioAggregate";
			this.radioAggregate.Size = new System.Drawing.Size(127, 21);
			this.radioAggregate.TabIndex = 1;
			this.radioAggregate.Text = "Assoziation";
			this.toolTip1.SetToolTip(this.radioAggregate, "Relation contains independent objects with own life-cycles");
			// 
			// radioComposite
			// 
			this.radioComposite.Checked = true;
			this.radioComposite.Location = new System.Drawing.Point(30, 35);
			this.radioComposite.Name = "radioComposite";
			this.radioComposite.Size = new System.Drawing.Size(127, 21);
			this.radioComposite.TabIndex = 0;
			this.radioComposite.TabStop = true;
			this.radioComposite.Text = "Composition";
			this.toolTip1.SetToolTip(this.radioComposite, "Life-cycle of the related objects are coupled to the lifetime of the owner");
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Enabled = false;
			this.btnOK.Location = new System.Drawing.Point(297, 343);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(107, 28);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(177, 343);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(107, 28);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			// 
			// txtElementType
			// 
			this.txtElementType.Location = new System.Drawing.Point(137, 24);
			this.txtElementType.Name = "txtElementType";
			this.txtElementType.Size = new System.Drawing.Size(267, 20);
			this.txtElementType.TabIndex = 0;
			this.toolTip1.SetToolTip(this.txtElementType, "Type of the related objects");
			this.txtElementType.TextChanged += new System.EventHandler(this.txtElementType_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(44, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 21);
			this.label2.TabIndex = 8;
			this.label2.Text = "Element Type";
			// 
			// txtRoleName
			// 
			this.txtRoleName.Location = new System.Drawing.Point(137, 93);
			this.txtRoleName.Name = "txtRoleName";
			this.txtRoleName.Size = new System.Drawing.Size(267, 20);
			this.txtRoleName.TabIndex = 2;
			this.toolTip1.SetToolTip(this.txtRoleName, "Name of the variable in which the relation is stored");
			// 
			// radioArrayList
			// 
			this.radioArrayList.Checked = true;
			this.radioArrayList.Location = new System.Drawing.Point(30, 60);
			this.radioArrayList.Name = "radioArrayList";
			this.radioArrayList.Size = new System.Drawing.Size(127, 21);
			this.radioArrayList.TabIndex = 1;
			this.radioArrayList.TabStop = true;
			this.radioArrayList.Text = "List<T>";
			this.toolTip1.SetToolTip(this.radioArrayList, "Relation contains independent objects with own life-cycles");
			// 
			// radioIList
			// 
			this.radioIList.Location = new System.Drawing.Point(30, 35);
			this.radioIList.Name = "radioIList";
			this.radioIList.Size = new System.Drawing.Size(127, 21);
			this.radioIList.TabIndex = 0;
			this.radioIList.Text = "IList<T>";
			this.toolTip1.SetToolTip(this.radioIList, "Life-cycle of the related objects are coupled to the lifetime of the owner");
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(43, 91);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(86, 28);
			this.label3.TabIndex = 15;
			this.label3.Text = "Optional Role Name";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.chkUseGenerics);
			this.groupBox3.Controls.Add(this.radioArrayList);
			this.groupBox3.Controls.Add(this.radioIList);
			this.groupBox3.Location = new System.Drawing.Point(44, 225);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(360, 91);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Coding Style  ";
			// 
			// chkUseGenerics
			// 
			this.chkUseGenerics.Checked = true;
			this.chkUseGenerics.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUseGenerics.Location = new System.Drawing.Point(204, 35);
			this.chkUseGenerics.Name = "chkUseGenerics";
			this.chkUseGenerics.Size = new System.Drawing.Size(160, 21);
			this.chkUseGenerics.TabIndex = 3;
			this.chkUseGenerics.Text = "Use Generics";
			this.chkUseGenerics.CheckedChanged += new System.EventHandler(this.chkUseGenerics_CheckedChanged);
			// 
			// RelationDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(448, 403);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.txtRoleName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtElementType);
			this.Controls.Add(this.txtFieldName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RelationDialog";
			this.Text = "Add Relation";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void txtElementType_TextChanged(object sender, System.EventArgs e)
		{
			this.btnOK.Enabled = (txtElementType.Text.Trim() != "" && txtFieldName.Text.Trim() != "");
		}

		private void txtFieldName_TextChanged(object sender, System.EventArgs e)
		{
			this.btnOK.Enabled = (txtElementType.Text.Trim() != "" && txtFieldName.Text.Trim() != "");
		}

		private void radioSingle_CheckedChanged(object sender, System.EventArgs e)
		{
            this.chkUseGenerics.Enabled =
			    this.radioArrayList.Enabled = 
			    this.radioIList.Enabled = !radioSingle.Checked;
		}

        private void chkUseGenerics_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseGenerics.Checked)
            {
                radioArrayList.Text = "List<T>";
                radioIList.Text = "IList<T>";
            }
            else
            {
                radioArrayList.Text = "ArrayList";
                radioArrayList.Checked = true;
                radioIList.Text = "IList";
            }
        }
	}
}
