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
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using WizardBase;

namespace ClassGenerator.ForeignKeyWizard
{
	/// <summary>
	/// Zusammenfassung für ForeignKeyWiz3.
	/// </summary>
	internal class ForeignKeyWiz3 : WizardBase.ViewBase
	{
		ForeignKeyWizModel model;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioComposite;
		private System.Windows.Forms.RadioButton radioAssoziation;
		private System.Windows.Forms.RadioButton radioIList;
		private System.Windows.Forms.TextBox txtFieldName;
		private System.Windows.Forms.RadioButton radioNDOArrayList;
		private System.Windows.Forms.RadioButton radioArrayList;
		private System.Windows.Forms.GroupBox gbCodingStyle;
		private System.Windows.Forms.Label lblClass;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox gbRelationType;
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ForeignKeyWiz3(IModel model)
		{
			InitializeComponent();
			this.model = (ForeignKeyWizModel) model;
		}

		public ForeignKeyWiz3()
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtFieldName = new System.Windows.Forms.TextBox();
			this.gbRelationType = new System.Windows.Forms.GroupBox();
			this.radioAssoziation = new System.Windows.Forms.RadioButton();
			this.radioComposite = new System.Windows.Forms.RadioButton();
			this.gbCodingStyle = new System.Windows.Forms.GroupBox();
			this.radioArrayList = new System.Windows.Forms.RadioButton();
			this.radioNDOArrayList = new System.Windows.Forms.RadioButton();
			this.radioIList = new System.Windows.Forms.RadioButton();
			this.lblClass = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.gbRelationType.SuspendLayout();
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
			// gbRelationType
			// 
			this.gbRelationType.Controls.Add(this.radioAssoziation);
			this.gbRelationType.Controls.Add(this.radioComposite);
			this.gbRelationType.Location = new System.Drawing.Point(328, 40);
			this.gbRelationType.Name = "gbRelationType";
			this.gbRelationType.Size = new System.Drawing.Size(160, 80);
			this.gbRelationType.TabIndex = 5;
			this.gbRelationType.TabStop = false;
			this.gbRelationType.Text = " Relation Type";
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
			// ForeignKeyWiz3
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblClass);
			this.Controls.Add(this.gbCodingStyle);
			this.Controls.Add(this.gbRelationType);
			this.Controls.Add(this.txtFieldName);
			this.Controls.Add(this.label1);
			this.Name = "ForeignKeyWiz3";
			this.Size = new System.Drawing.Size(512, 200);
			this.Load += new System.EventHandler(this.ForeignKeyWiz3_Load);
			this.gbRelationType.ResumeLayout(false);
			this.gbCodingStyle.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ForeignKeyWiz3_Load(object sender, System.EventArgs e)
		{
			FkRelation relation = (FkRelation) model.RelationNode.Relation;

			this.lblClass.Text = relation.RelatedType;

			if (relation.ForeignFieldName == string.Empty)
			{
				string name = relation.OwningTable;
				name = name.Substring(0, 1).ToLower() + name.Substring(1);
				relation.ForeignFieldName = name;
			}

			this.txtFieldName.DataBindings.Add("Text", relation, "ForeignFieldName");

			if (relation.ForeignCodingStyle == CodingStyle.IList)
				this.radioIList.Checked = true;
			else if (relation.ForeignCodingStyle == CodingStyle.ArrayList)
				this.radioArrayList.Checked = true;
			else if (relation.ForeignCodingStyle == CodingStyle.NDOArrayList)
				this.radioNDOArrayList.Checked = true;
#if !NDO11
            radioArrayList.Text = "List<T>";
            radioIList.Text = "IList<T>";
            radioNDOArrayList.Visible = false;
#endif

			this.gbCodingStyle.Enabled = (relation.RelationDirection == RelationDirection.DirectedToMe || relation.RelationDirection == RelationDirection.Bidirectional);
			if (relation.RelationDirection != RelationDirection.DirectedToMe && relation.IsComposite)
			{
				this.gbRelationType.Enabled = false;
				this.radioAssoziation.Checked = true;
			}
			else
			{
				this.gbRelationType.Enabled = true;
				if (relation.ForeignIsComposite)
					this.radioComposite.Checked = true;
				else
					this.radioAssoziation.Checked = true;
			}

			Frame.Description = "Enter the name of the field, which will contain the relation.\n\nEnter the relation type and the list coding style.";
		}

		public override void OnLeaveView()
		{
			FkRelation relation = (FkRelation) model.RelationNode.Relation;
			if (relation.RelationDirection == RelationDirection.DirectedFromMe)
				return;	

			if (this.radioIList.Checked)
				relation.ForeignCodingStyle = CodingStyle.IList;
			else if (this.radioArrayList.Checked)
				relation.ForeignCodingStyle = CodingStyle.ArrayList;
			else if (this.radioNDOArrayList.Checked)
				relation.ForeignCodingStyle = CodingStyle.NDOArrayList;

			relation.ForeignIsComposite = this.radioComposite.Checked;
		}


	}
}
