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
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using WizardBase;

namespace ClassGenerator.IntermediateTableWizard
{
	/// <summary>
	/// Zusammenfassung für IntTblWiz1.
	/// </summary>
#if DEBUG
	public class IntTblWiz1 : ViewBase
#else
	internal class IntTblWiz1 : ViewBase
#endif
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox gbTable1;
		private System.Windows.Forms.ComboBox cbForeignKey1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbRelatedTable1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox cbForeignKey2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbRelatedTable2;

		IntermediateTableWizardModel model;
		public IntTblWiz1(IModel model)
		{
			this.model = (IntermediateTableWizardModel)model;
			InitializeComponent();
		}

		public IntTblWiz1()
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
			this.gbTable1 = new System.Windows.Forms.GroupBox();
			this.cbForeignKey1 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cbRelatedTable1 = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbForeignKey2 = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cbRelatedTable2 = new System.Windows.Forms.ComboBox();
			this.gbTable1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbTable1
			// 
			this.gbTable1.Controls.Add(this.cbForeignKey1);
			this.gbTable1.Controls.Add(this.label3);
			this.gbTable1.Controls.Add(this.label1);
			this.gbTable1.Controls.Add(this.cbRelatedTable1);
			this.gbTable1.Location = new System.Drawing.Point(20, 8);
			this.gbTable1.Name = "gbTable1";
			this.gbTable1.Size = new System.Drawing.Size(472, 96);
			this.gbTable1.TabIndex = 9;
			this.gbTable1.TabStop = false;
			this.gbTable1.Text = " Related Table 1 ";
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
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbForeignKey2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cbRelatedTable2);
			this.groupBox1.Location = new System.Drawing.Point(20, 112);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(472, 96);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Related Table 2 ";
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
			// 
			// IntTblWiz1
			// 
			this.Controls.Add(this.gbTable1);
			this.Controls.Add(this.groupBox1);
			this.Name = "IntTblWiz1";
			this.Size = new System.Drawing.Size(512, 224);
			this.Load += new System.EventHandler(this.IntTblWiz1_Load);
			this.gbTable1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetStartIndex()
		{
			TreeNodeCollection fkNodes = model.IntermediateTableNode.OriginalTableNode.Nodes;
			IntermediateTable intermediateTable = model.IntermediateTableNode.IntermediateTable;

			string fkColumnName = intermediateTable[0].ForeignKeyColumnName;
			if(fkColumnName == string.Empty)
			{
				cbForeignKey1.SelectedIndex = 0;
			}
			else
			{
				for (int i = 0; i < fkNodes.Count; i++)
				{
					if (fkNodes[i].Text == fkColumnName)
					{
						cbForeignKey1.SelectedIndex = i;
						break;
					}
				}
			}
			fkColumnName = intermediateTable[1].ForeignKeyColumnName;
			if(fkColumnName == string.Empty)
			{
				cbForeignKey2.SelectedIndex = 1;
			}
			else
			{
				for (int i = 0; i < fkNodes.Count; i++)
				{
					if (fkNodes[i].Text == fkColumnName)
					{
						cbForeignKey2.SelectedIndex = i;
						break;
					}
				}
			}

			string tableName = intermediateTable[0].Table;
			if (tableName == string.Empty)
			{
				cbRelatedTable1.SelectedIndex = 0;
			}
			else
			{
				for (int i = 0; i < model.TableNodes.Count; i++)
				{
					if (((TableNode)model.TableNodes[i]).Text == tableName)
					{
						cbRelatedTable1.SelectedIndex = i;
						break;
					}
				}
			}
			tableName = intermediateTable[1].Table;
			if (tableName == string.Empty)
			{
				cbRelatedTable2.SelectedIndex = 1;
			}
			else
			{
				for (int i = 0; i < model.TableNodes.Count; i++)
				{
					if (((TableNode)model.TableNodes[i]).Text == tableName)
					{
						cbRelatedTable2.SelectedIndex = i;
						break;
					}
				}
			}
		}


		private void IntTblWiz1_Load(object sender, System.EventArgs e)
		{
			Frame.Description = "Intermediate Table: '" + model.IntermediateTableNode.Text 
				+ "'.\n\nSelect two other tables which are related using the intermediate table.\n\n" 
				+ "Assign a column of the intermediate table as foreign key to each of the two related tables.";

			ArrayList al2 = new ArrayList(model.TableNodes);
			this.cbRelatedTable1.DataSource = model.TableNodes;
			this.cbRelatedTable2.DataSource = al2;
			ArrayList fkNodes2 = new ArrayList(model.IntermediateTableNode.OriginalTableNode.Nodes);
			this.cbForeignKey1.DataSource = model.IntermediateTableNode.OriginalTableNode.Nodes;
			this.cbForeignKey2.DataSource = fkNodes2;

			// Selected Index nach model einstellen.
			SetStartIndex();		
		}

		public override void OnLeaveView()
		{
			IntermediateTable intTable = model.IntermediateTableNode.IntermediateTable;
			intTable[0].Table = cbRelatedTable1.Text;
			intTable[1].Table = cbRelatedTable2.Text;
			intTable[0].ForeignKeyColumnName = cbForeignKey1.Text;
			intTable[1].ForeignKeyColumnName = cbForeignKey2.Text;
			intTable[0].ChildForeignKeyColumnName = intTable[1].ForeignKeyColumnName;
			intTable[1].ChildForeignKeyColumnName = intTable[0].ForeignKeyColumnName;
			TableNode tn = (TableNode) cbRelatedTable1.SelectedItem;
			intTable[0].Type = tn.Table.ClassName;
			tn = (TableNode) cbRelatedTable2.SelectedItem;
			intTable[1].Type = tn.Table.ClassName;
		}

	}
}
