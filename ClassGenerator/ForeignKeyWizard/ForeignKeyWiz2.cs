//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
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
// there is a commercial licence available at www.netdataobjects.com.
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using WizardBase;

namespace ClassGenerator.ForeignKeyWizard
{
	/// <summary>
	/// Zusammenfassung für ForeignKeyWiz2.
	/// </summary>
	internal class ForeignKeyWiz2 : WizardBase.ViewBase
	{
		ForeignKeyWizModel model;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioComposite;
		private System.Windows.Forms.RadioButton radioAssoziation;
		private System.Windows.Forms.RadioButton radioIList;
		private System.Windows.Forms.TextBox txtFieldName;
		private System.Windows.Forms.RadioButton radioNDOArrayList;
		private System.Windows.Forms.RadioButton radioArrayList;
		private System.Windows.Forms.GroupBox gbCodingStyle;
		private System.Windows.Forms.Label lblClass;
		private System.Windows.Forms.Label label3;
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ForeignKeyWiz2(IModel model)
		{
			InitializeComponent();
			this.model = (ForeignKeyWizModel) model;
		}


		public ForeignKeyWiz2()
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();
#if !NDO11
            this.radioArrayList.Text = "List<T>";
            this.radioIList.Text = "IList<T>";
            this.radioNDOArrayList.Visible = false;
            this.radioArrayList.Checked = true;
#endif
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtFieldName = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioAssoziation = new System.Windows.Forms.RadioButton();
			this.radioComposite = new System.Windows.Forms.RadioButton();
			this.gbCodingStyle = new System.Windows.Forms.GroupBox();
			this.radioArrayList = new System.Windows.Forms.RadioButton();
			this.radioNDOArrayList = new System.Windows.Forms.RadioButton();
			this.radioIList = new System.Windows.Forms.RadioButton();
			this.lblClass = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.gbCodingStyle.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "Relation Field Name:";
			// 
			// txtFieldName
			// 
			this.txtFieldName.Location = new System.Drawing.Point(168, 72);
			this.txtFieldName.Name = "txtFieldName";
			this.txtFieldName.Size = new System.Drawing.Size(152, 22);
			this.txtFieldName.TabIndex = 2;
			this.txtFieldName.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioAssoziation);
			this.groupBox1.Controls.Add(this.radioComposite);
			this.groupBox1.Location = new System.Drawing.Point(328, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(160, 80);
			this.groupBox1.TabIndex = 5;
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
			// gbCodingStyle
			// 
			this.gbCodingStyle.Controls.Add(this.radioArrayList);
			this.gbCodingStyle.Controls.Add(this.radioNDOArrayList);
			this.gbCodingStyle.Controls.Add(this.radioIList);
			this.gbCodingStyle.Location = new System.Drawing.Point(24, 128);
			this.gbCodingStyle.Name = "gbCodingStyle";
			this.gbCodingStyle.Size = new System.Drawing.Size(464, 56);
			this.gbCodingStyle.TabIndex = 6;
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
			// lblClass
			// 
			this.lblClass.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblClass.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblClass.Location = new System.Drawing.Point(72, 16);
			this.lblClass.Name = "lblClass";
			this.lblClass.Size = new System.Drawing.Size(416, 24);
			this.lblClass.TabIndex = 7;
			this.lblClass.Text = "label2";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 24);
			this.label3.TabIndex = 8;
			this.label3.Text = "Class:";
			// 
			// ForeignKeyWiz2
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblClass);
			this.Controls.Add(this.gbCodingStyle);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.txtFieldName);
			this.Controls.Add(this.label1);
			this.Name = "ForeignKeyWiz2";
			this.Size = new System.Drawing.Size(512, 200);
			this.Load += new System.EventHandler(this.ForeignKeyWiz2_Load);
			this.groupBox1.ResumeLayout(false);
			this.gbCodingStyle.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ForeignKeyWiz2_Load(object sender, System.EventArgs e)
		{
			FkRelation relation = (FkRelation) model.RelationNode.Relation;

			this.lblClass.Text = relation.OwningType;
			if (relation.FieldName == string.Empty)
			{
				string name = relation.RelatedType;
				name = name.Substring(0, 1).ToLower() + name.Substring(1);
				relation.FieldName = name;
			}
			this.txtFieldName.DataBindings.Add("Text", relation, "FieldName");
			if (relation.IsComposite)
				this.radioComposite.Checked = true;
			else
				this.radioAssoziation.Checked = true;

			if (relation.CodingStyle == CodingStyle.IList)
				this.radioIList.Checked = true;
			else if (relation.CodingStyle == CodingStyle.ArrayList)
				this.radioArrayList.Checked = true;
			else if (relation.CodingStyle == CodingStyle.NDOArrayList)
				this.radioNDOArrayList.Checked = true;

#if !NDO11
            radioArrayList.Text = "List<T>";
            radioIList.Text = "IList<T>";
            radioNDOArrayList.Visible = false;
#endif


			this.gbCodingStyle.Visible = false;  // we own the foreign key, so cardinality is always 1

			Frame.Description = "Enter the name of the field, which will contain the relation.\n\nEnter the relation type.";
		}

		public override void OnLeaveView()
		{
			FkRelation relation = (FkRelation) model.RelationNode.Relation;

			if (this.radioIList.Checked)
				relation.CodingStyle = CodingStyle.IList;
			else if (this.radioArrayList.Checked)
				relation.CodingStyle = CodingStyle.ArrayList;
			else if (this.radioNDOArrayList.Checked)
				relation.CodingStyle = CodingStyle.NDOArrayList;

			relation.IsComposite = this.radioComposite.Checked;			
		}


	}
}
