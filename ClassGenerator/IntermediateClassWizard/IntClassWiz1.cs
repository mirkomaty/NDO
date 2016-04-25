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

namespace ClassGenerator.IntermediateClassWizard
{
	/// <summary>
	/// Zusammenfassung für IntClassWiz1.
	/// </summary>
#if DEBUG
		public class IntClassWiz1 : WizardBase.ViewBase
#else
	internal class IntClassWiz1 : WizardBase.ViewBase
#endif
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox gbTable1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbRelatedTable1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbRelatedTable2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbForeignKey1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbForeignKey2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtFieldName2;
		private System.Windows.Forms.TextBox txtFieldName1;
		private System.Windows.Forms.Label label6;

		IntermediateClassWizardModel model;
		public IntClassWiz1(IModel model)
		{
			this.model = (IntermediateClassWizardModel) model;
			InitializeComponent();
		}

		public IntClassWiz1()
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtFieldName2 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cbForeignKey2 = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cbRelatedTable2 = new System.Windows.Forms.ComboBox();
			this.gbTable1 = new System.Windows.Forms.GroupBox();
			this.txtFieldName1 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.cbForeignKey1 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cbRelatedTable1 = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.gbTable1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtFieldName2);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.cbForeignKey2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cbRelatedTable2);
			this.groupBox1.Location = new System.Drawing.Point(16, 152);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(472, 128);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Related Table 2 ";
			// 
			// txtFieldName2
			// 
			this.txtFieldName2.Location = new System.Drawing.Point(112, 88);
			this.txtFieldName2.Name = "txtFieldName2";
			this.txtFieldName2.Size = new System.Drawing.Size(344, 22);
			this.txtFieldName2.TabIndex = 11;
			this.txtFieldName2.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 88);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 24);
			this.label5.TabIndex = 10;
			this.label5.Text = "Field Name:";
			// 
			// cbForeignKey2
			// 
			this.cbForeignKey2.Location = new System.Drawing.Point(112, 56);
			this.cbForeignKey2.Name = "cbForeignKey2";
			this.cbForeignKey2.Size = new System.Drawing.Size(352, 24);
			this.cbForeignKey2.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 24);
			this.label4.TabIndex = 8;
			this.label4.Text = "Foreign Key:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 24);
			this.label2.TabIndex = 7;
			this.label2.Text = "Table Name:";
			// 
			// cbRelatedTable2
			// 
			this.cbRelatedTable2.ItemHeight = 16;
			this.cbRelatedTable2.Location = new System.Drawing.Point(112, 24);
			this.cbRelatedTable2.Name = "cbRelatedTable2";
			this.cbRelatedTable2.Size = new System.Drawing.Size(352, 24);
			this.cbRelatedTable2.TabIndex = 6;
			this.cbRelatedTable2.SelectedIndexChanged += new System.EventHandler(this.cbRelatedTable2_SelectedIndexChanged);
			// 
			// gbTable1
			// 
			this.gbTable1.Controls.Add(this.txtFieldName1);
			this.gbTable1.Controls.Add(this.label6);
			this.gbTable1.Controls.Add(this.cbForeignKey1);
			this.gbTable1.Controls.Add(this.label3);
			this.gbTable1.Controls.Add(this.label1);
			this.gbTable1.Controls.Add(this.cbRelatedTable1);
			this.gbTable1.Location = new System.Drawing.Point(16, 8);
			this.gbTable1.Name = "gbTable1";
			this.gbTable1.Size = new System.Drawing.Size(472, 128);
			this.gbTable1.TabIndex = 7;
			this.gbTable1.TabStop = false;
			this.gbTable1.Text = " Related Table 1 ";
			// 
			// txtFieldName1
			// 
			this.txtFieldName1.Location = new System.Drawing.Point(112, 88);
			this.txtFieldName1.Name = "txtFieldName1";
			this.txtFieldName1.Size = new System.Drawing.Size(344, 22);
			this.txtFieldName1.TabIndex = 13;
			this.txtFieldName1.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 88);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 24);
			this.label6.TabIndex = 12;
			this.label6.Text = "Field Name:";
			// 
			// cbForeignKey1
			// 
			this.cbForeignKey1.Location = new System.Drawing.Point(112, 56);
			this.cbForeignKey1.Name = "cbForeignKey1";
			this.cbForeignKey1.Size = new System.Drawing.Size(352, 24);
			this.cbForeignKey1.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 24);
			this.label3.TabIndex = 6;
			this.label3.Text = "Foreign Key:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 24);
			this.label1.TabIndex = 5;
			this.label1.Text = "Table Name:";
			// 
			// cbRelatedTable1
			// 
			this.cbRelatedTable1.Location = new System.Drawing.Point(112, 24);
			this.cbRelatedTable1.Name = "cbRelatedTable1";
			this.cbRelatedTable1.Size = new System.Drawing.Size(352, 24);
			this.cbRelatedTable1.TabIndex = 4;
			this.cbRelatedTable1.SelectedIndexChanged += new System.EventHandler(this.cbRelatedTable1_SelectedIndexChanged);
			// 
			// IntClassWiz1
			// 
			this.Controls.Add(this.gbTable1);
			this.Controls.Add(this.groupBox1);
			this.Name = "IntClassWiz1";
			this.Size = new System.Drawing.Size(512, 300);
			this.Load += new System.EventHandler(this.IntClassWiz1_Load);
			this.groupBox1.ResumeLayout(false);
			this.gbTable1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetStartIndex()
		{
			//TODO: Index nach Model bestimmen.
			cbForeignKey1.SelectedIndex = 0;
			cbForeignKey2.SelectedIndex = 1;
			cbRelatedTable1.SelectedIndex = 0;
			cbRelatedTable2.SelectedIndex = 1;
		}

		private void IntClassWiz1_Load(object sender, System.EventArgs e)
		{
			this.cbRelatedTable1.DataSource = model.AllTableNodes;
			ArrayList al = new ArrayList(model.AllTableNodes);
			this.cbRelatedTable2.DataSource = al;
			Frame.Description = "Enter the Table, the foreign key points to.\n\nThe relation direction determines, in which class the relation will be defined";
			// We need two separate lists with the same content.
			// Otherwise Winforms Binding would navigate in both combo boxes,
			// if we change the index of one.
			ArrayList al1 = new ArrayList();
			ArrayList al2 = new ArrayList();
			foreach(NDOTreeNode tn in model.TableNode.Nodes)
			{
				al1.Add(tn.Text);
				al2.Add(tn.Text);
			}
			cbForeignKey1.DataSource = al1;
			cbForeignKey2.DataSource = al2;
			SetStartIndex();
			this.txtFieldName1.DataBindings.Add("Text", model[0], "OwnFieldName");
			this.txtFieldName2.DataBindings.Add("Text", model[1], "OwnFieldName");
		}

		TableNode FindTable(string tableName)
		{
			foreach(TableNode tn in model.AllTableNodes)
			{
				if (tn.Text == tableName)
					return tn;
			}
			return null;
		}

		public override void OnLeaveView()
		{
			IntermediateClassInfo ici1 = model[0];
			IntermediateClassInfo ici2 = model[1];
			ici1.Table = cbRelatedTable1.Text;
			ici2.Table = cbRelatedTable2.Text;
			ici1.Type = FindTable(ici1.Table).Table.ClassName;
			ici2.Type = FindTable(ici2.Table).Table.ClassName;
			ici1.ForeignKeyColumnName = cbForeignKey1.Text;
			ici2.ForeignKeyColumnName = cbForeignKey2.Text;
		}

		private void cbRelatedTable1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			IntermediateClassInfo classInfo = model[0];

			string name = FindTable(cbRelatedTable1.Text).Table.ClassName;
			name = name.Substring(0,1).ToLower() + name.Substring(1);
			this.txtFieldName1.Text = name;
			classInfo.OwnFieldName = name;
		}

		private void cbRelatedTable2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			IntermediateClassInfo classInfo = model[1];

			string name = FindTable(cbRelatedTable2.Text).Table.ClassName;
			name = name.Substring(0,1).ToLower() + name.Substring(1);
			this.txtFieldName2.Text = name;
			classInfo.OwnFieldName = name;
		}



	}
}
