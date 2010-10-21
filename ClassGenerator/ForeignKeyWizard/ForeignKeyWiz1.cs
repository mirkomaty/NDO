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
	/// Zusammenfassung für ForeignKeyWiz1.
	/// </summary>
	internal class ForeignKeyWiz1 : WizardBase.ViewBase
	{
		ForeignKeyWizModel model;
		private System.Windows.Forms.ComboBox cbRelatedTable;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblToMe;
		private System.Windows.Forms.Label lblFromMe;
		private System.Windows.Forms.Label lblBi;
		private System.Windows.Forms.RadioButton radioBi;
		private System.Windows.Forms.RadioButton radioFromMe;
		private System.Windows.Forms.RadioButton radioToMe;
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ForeignKeyWiz1(IModel model)
		{
			InitializeComponent();
			this.model = (ForeignKeyWizModel) model;
		}

		public ForeignKeyWiz1()
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
			this.cbRelatedTable = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblBi = new System.Windows.Forms.Label();
			this.lblFromMe = new System.Windows.Forms.Label();
			this.lblToMe = new System.Windows.Forms.Label();
			this.radioBi = new System.Windows.Forms.RadioButton();
			this.radioFromMe = new System.Windows.Forms.RadioButton();
			this.radioToMe = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbRelatedTable
			// 
			this.cbRelatedTable.Location = new System.Drawing.Point(24, 48);
			this.cbRelatedTable.Name = "cbRelatedTable";
			this.cbRelatedTable.Size = new System.Drawing.Size(424, 24);
			this.cbRelatedTable.TabIndex = 0;
			this.cbRelatedTable.SelectedIndexChanged += new System.EventHandler(this.cbRelatedTable_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(416, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "Foreign Table:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblBi);
			this.groupBox1.Controls.Add(this.lblFromMe);
			this.groupBox1.Controls.Add(this.lblToMe);
			this.groupBox1.Controls.Add(this.radioBi);
			this.groupBox1.Controls.Add(this.radioFromMe);
			this.groupBox1.Controls.Add(this.radioToMe);
			this.groupBox1.Location = new System.Drawing.Point(24, 88);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(464, 104);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Relation Direction ";
			// 
			// lblBi
			// 
			this.lblBi.Location = new System.Drawing.Point(176, 75);
			this.lblBi.Name = "lblBi";
			this.lblBi.Size = new System.Drawing.Size(280, 16);
			this.lblBi.TabIndex = 5;
			this.lblBi.Text = "label3";
			// 
			// lblFromMe
			// 
			this.lblFromMe.Location = new System.Drawing.Point(176, 51);
			this.lblFromMe.Name = "lblFromMe";
			this.lblFromMe.Size = new System.Drawing.Size(280, 16);
			this.lblFromMe.TabIndex = 4;
			this.lblFromMe.Text = "label3";
			// 
			// lblToMe
			// 
			this.lblToMe.Location = new System.Drawing.Point(176, 27);
			this.lblToMe.Name = "lblToMe";
			this.lblToMe.Size = new System.Drawing.Size(280, 16);
			this.lblToMe.TabIndex = 3;
			this.lblToMe.Text = "label2";
			// 
			// radioBi
			// 
			this.radioBi.Location = new System.Drawing.Point(24, 72);
			this.radioBi.Name = "radioBi";
			this.radioBi.Size = new System.Drawing.Size(152, 24);
			this.radioBi.TabIndex = 2;
			this.radioBi.Text = "Bidirectional";
			// 
			// radioFromMe
			// 
			this.radioFromMe.Location = new System.Drawing.Point(24, 48);
			this.radioFromMe.Name = "radioFromMe";
			this.radioFromMe.Size = new System.Drawing.Size(152, 24);
			this.radioFromMe.TabIndex = 1;
			this.radioFromMe.Text = "Directed from own";
			// 
			// radioToMe
			// 
			this.radioToMe.Checked = true;
			this.radioToMe.Location = new System.Drawing.Point(24, 24);
			this.radioToMe.Name = "radioToMe";
			this.radioToMe.Size = new System.Drawing.Size(152, 24);
			this.radioToMe.TabIndex = 0;
			this.radioToMe.TabStop = true;
			this.radioToMe.Text = "Directed to own";
			// 
			// ForeignKeyWiz1
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbRelatedTable);
			this.Name = "ForeignKeyWiz1";
			this.Size = new System.Drawing.Size(512, 280);
			this.Load += new System.EventHandler(this.ForeignKeyWiz1_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ForeignKeyWiz1_Load(object sender, System.EventArgs e)
		{
			this.cbRelatedTable.DataSource = model.TableNodes;
			Frame.Description = "Enter the Table, the foreign key points to.\n\nThe relation direction determines, in which class the relation will be defined";
			RelationDirection dir = ((FkRelation)model.RelationNode.Relation).RelationDirection;
			if (dir == RelationDirection.DirectedToMe)
				radioToMe.Checked = true;
			else if(dir == RelationDirection.DirectedFromMe)
				radioFromMe.Checked = true;
			else 
				radioBi.Checked = true;
		}

		public override void OnLeaveView()
		{
			if (cbRelatedTable.SelectedIndex == -1)
				return;
			model.RelationNode.RelatedTableNode = (TableNode) model.TableNodes[cbRelatedTable.SelectedIndex];
			FkRelation fkRelation = (FkRelation) model.RelationNode.Relation;
			fkRelation.RelatedTable = model.RelationNode.RelatedTableNode.Table.Name;
			fkRelation.RelatedType = model.RelationNode.RelatedTableNode.Table.ClassName;
			RelationDirection dir;
			if (radioToMe.Checked)
				dir = RelationDirection.DirectedToMe;
			else if (radioFromMe.Checked)
				dir = RelationDirection.DirectedFromMe;
			else
				dir = RelationDirection.Bidirectional;
			((FkRelation)model.RelationNode.Relation).RelationDirection = dir;
		}


		private void cbRelatedTable_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			lblToMe.Text = cbRelatedTable.Text + "->" + model.RelationNode.Parent.Text;
			lblFromMe.Text = model.RelationNode.Parent.Text + "->" + cbRelatedTable.Text;
			lblBi.Text = model.RelationNode.Parent.Text + "<->" + cbRelatedTable.Text;
		}
	}
}
