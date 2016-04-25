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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using WizardBase;
using System.Diagnostics;

namespace ClassGenerator.IntermediateTableWizard
{
	/// <summary>
	/// Zusammenfassung für IntTblWiz3.
	/// </summary>
#if DEBUG
	public class IntTblWiz3 : ViewBase
#else
	internal class IntTblWiz3 : ViewBase
#endif
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblClass;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox gbCodingStyle;
		private System.Windows.Forms.RadioButton radioArrayList;
		private System.Windows.Forms.RadioButton radioNDOArrayList;
		private System.Windows.Forms.RadioButton radioIList;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioAssoziation;
		private System.Windows.Forms.RadioButton radioComposite;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioList;
		private System.Windows.Forms.RadioButton radioElement;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtFieldName;

		IntermediateTableWizardModel model;
		public IntTblWiz3(IModel model)
		{
			InitializeComponent();
			this.model = (IntermediateTableWizardModel)model;
#if !NDO11
            this.radioArrayList.Text = "List<T>";
            this.radioIList.Text = "IList<T>";
            this.radioNDOArrayList.Visible = false;
#endif
        }

		public IntTblWiz3()
		{
			InitializeComponent();
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

		#region Vom Komponenten-Designer generierter Code
		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblClass = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.gbCodingStyle = new System.Windows.Forms.GroupBox();
			this.radioArrayList = new System.Windows.Forms.RadioButton();
			this.radioNDOArrayList = new System.Windows.Forms.RadioButton();
			this.radioIList = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioAssoziation = new System.Windows.Forms.RadioButton();
			this.radioComposite = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioList = new System.Windows.Forms.RadioButton();
			this.radioElement = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.txtFieldName = new System.Windows.Forms.TextBox();
			this.gbCodingStyle.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblClass
			// 
			this.lblClass.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblClass.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblClass.Location = new System.Drawing.Point(72, 8);
			this.lblClass.Name = "lblClass";
			this.lblClass.Size = new System.Drawing.Size(416, 24);
			this.lblClass.TabIndex = 13;
			this.lblClass.Text = "label2";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 24);
			this.label3.TabIndex = 14;
			this.label3.Text = "Class:";
			// 
			// gbCodingStyle
			// 
			this.gbCodingStyle.Controls.Add(this.radioArrayList);
			this.gbCodingStyle.Controls.Add(this.radioNDOArrayList);
			this.gbCodingStyle.Controls.Add(this.radioIList);
			this.gbCodingStyle.Location = new System.Drawing.Point(24, 152);
			this.gbCodingStyle.Name = "gbCodingStyle";
			this.gbCodingStyle.Size = new System.Drawing.Size(464, 56);
			this.gbCodingStyle.TabIndex = 12;
			this.gbCodingStyle.TabStop = false;
			this.gbCodingStyle.Text = " Coding Style";
			// 
			// radioArrayList
			// 
			this.radioArrayList.Location = new System.Drawing.Point(152, 24);
			this.radioArrayList.Name = "radioArrayList";
			this.radioArrayList.Size = new System.Drawing.Size(136, 24);
			this.radioArrayList.TabIndex = 3;
			this.radioArrayList.Text = "ArrayList";
			// 
			// radioNDOArrayList
			// 
			this.radioNDOArrayList.Location = new System.Drawing.Point(312, 24);
			this.radioNDOArrayList.Name = "radioNDOArrayList";
			this.radioNDOArrayList.Size = new System.Drawing.Size(136, 24);
			this.radioNDOArrayList.TabIndex = 2;
			this.radioNDOArrayList.Text = "NDOArrayList";
			// 
			// radioIList
			// 
			this.radioIList.Checked = true;
			this.radioIList.Location = new System.Drawing.Point(16, 24);
			this.radioIList.Name = "radioIList";
			this.radioIList.Size = new System.Drawing.Size(80, 24);
			this.radioIList.TabIndex = 0;
			this.radioIList.TabStop = true;
			this.radioIList.Text = "IList";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioAssoziation);
			this.groupBox1.Controls.Add(this.radioComposite);
			this.groupBox1.Location = new System.Drawing.Point(256, 64);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(232, 80);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Relation Type";
			// 
			// radioAssoziation
			// 
			this.radioAssoziation.Location = new System.Drawing.Point(16, 48);
			this.radioAssoziation.Name = "radioAssoziation";
			this.radioAssoziation.Size = new System.Drawing.Size(120, 24);
			this.radioAssoziation.TabIndex = 1;
			this.radioAssoziation.Text = "Assoziation";
			// 
			// radioComposite
			// 
			this.radioComposite.Checked = true;
			this.radioComposite.Location = new System.Drawing.Point(16, 20);
			this.radioComposite.Name = "radioComposite";
			this.radioComposite.TabIndex = 0;
			this.radioComposite.TabStop = true;
			this.radioComposite.Text = "Composite";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioList);
			this.groupBox2.Controls.Add(this.radioElement);
			this.groupBox2.Location = new System.Drawing.Point(24, 64);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(216, 80);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Relation Cardinality ";
			// 
			// radioList
			// 
			this.radioList.Location = new System.Drawing.Point(32, 48);
			this.radioList.Name = "radioList";
			this.radioList.Size = new System.Drawing.Size(136, 16);
			this.radioList.TabIndex = 1;
			this.radioList.Text = "n";
			// 
			// radioElement
			// 
			this.radioElement.Checked = true;
			this.radioElement.Location = new System.Drawing.Point(32, 24);
			this.radioElement.Name = "radioElement";
			this.radioElement.Size = new System.Drawing.Size(136, 16);
			this.radioElement.TabIndex = 0;
			this.radioElement.TabStop = true;
			this.radioElement.Text = "1";
			this.radioElement.CheckedChanged += new System.EventHandler(this.radioElement_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 24);
			this.label1.TabIndex = 16;
			this.label1.Text = "Field Name:";
			// 
			// txtFieldName
			// 
			this.txtFieldName.Location = new System.Drawing.Point(120, 36);
			this.txtFieldName.Name = "txtFieldName";
			this.txtFieldName.Size = new System.Drawing.Size(304, 22);
			this.txtFieldName.TabIndex = 17;
			this.txtFieldName.Text = "";
			// 
			// IntTblWiz3
			// 
			this.Controls.Add(this.txtFieldName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.lblClass);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gbCodingStyle);
			this.Controls.Add(this.groupBox1);
			this.Name = "IntTblWiz3";
			this.Size = new System.Drawing.Size(512, 228);
			this.Load += new System.EventHandler(this.IntTblWiz3_Load);
			this.gbCodingStyle.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void IntTblWiz3_Load(object sender, System.EventArgs e)
		{
			RelatedTableInfo rti = model.IntermediateTableNode.IntermediateTable[model.Index];
			string description = "Enter the field name of the relation in the class '" + rti.Type + "'. ";
			description += "The relation type Composite couples the life cycle of the related object to the life cycle of the parent.\n\n";
			description += "If the cardinality is n, the coding style determines, which type is used for the relation variable. Otherwise the type of the relation variable is the related class.";
			Frame.Description = description;

			lblClass.Text = rti.Type;
			if (rti.FieldName == string.Empty)
			{
				RelatedTableInfo otherRti = model.IntermediateTableNode.IntermediateTable[(model.Index + 1) % 2];
				string name;
				if (rti.IsElement)
				{
					name = model.FindTable(rti.Table).Table.ClassName;
				}
				else
				{
					name = otherRti.Table.Replace(" ", string.Empty);
				}
				rti.FieldName = name.Substring(0, 1).ToLower() + name.Substring(1);
			}
			if (rti.IsElement)
				radioElement.Checked = true;
			else
				radioList.Checked = true;

			if (rti.IsComposite)
				radioComposite.Checked = true;
			else
				radioAssoziation.Checked = true;

			gbCodingStyle.Enabled = !rti.IsElement;

			if (rti.CodingStyle == CodingStyle.ArrayList)
				radioArrayList.Checked = true;
			else if (rti.CodingStyle == CodingStyle.IList)
				radioIList.Checked = true;
			else 
				radioNDOArrayList.Checked = true;

#if !NDO11
            radioArrayList.Text = "List<T>";
            radioIList.Text = "IList<T>";
            radioNDOArrayList.Visible = false;
#endif


			this.txtFieldName.DataBindings.Add("Text", rti, "FieldName");		
		}

		public override void OnLeaveView()
		{
			RelatedTableInfo rti = model.IntermediateTableNode.IntermediateTable[model.Index];

			rti.IsElement = radioElement.Checked;
			rti.IsComposite = radioComposite.Checked;
			if (rti.RelationDirection == RelationDirection.Bidirectional
				&& model.Index == 1 && rti.IsComposite)
			{
				RelatedTableInfo rti2 = model.IntermediateTableNode.IntermediateTable[0];
				if (rti2.IsComposite)
				{
					MessageBox.Show("Only one side of a bidirectional relation can be a composite. Setting '"
						+ rti2.Type + "." + rti2.FieldName + "' to Assoziation."); 
				}
				rti2.IsComposite = false;
			}
			if (radioArrayList.Checked)
				rti.CodingStyle = CodingStyle.ArrayList;
			if (radioIList.Checked)
				rti.CodingStyle = CodingStyle.IList;
			if (radioNDOArrayList.Checked)
				rti.CodingStyle = CodingStyle.NDOArrayList;
		}

		private void radioElement_CheckedChanged(object sender, System.EventArgs e)
		{
			gbCodingStyle.Enabled = radioList.Checked;
		}
		
	}
}
