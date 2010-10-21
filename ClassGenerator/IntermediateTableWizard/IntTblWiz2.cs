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
	/// Zusammenfassung für IntTblWiz2.
	/// </summary>
#if DEBUG
	public class IntTblWiz2 : ViewBase
#else
	internal class IntTblWiz2 : ViewBase
#endif
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblBi;
		private System.Windows.Forms.Label lblToMe;
		private System.Windows.Forms.RadioButton radioBi;
		private System.Windows.Forms.RadioButton radioToMe;
		private System.Windows.Forms.Label lblFromMe;
		private System.Windows.Forms.RadioButton radioFromMe;

		IntermediateTableWizardModel model;
		public IntTblWiz2(IModel model)
		{
			InitializeComponent();
			this.model = (IntermediateTableWizardModel) model;
		}

		public IntTblWiz2()
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
			this.lblBi = new System.Windows.Forms.Label();
			this.lblToMe = new System.Windows.Forms.Label();
			this.radioBi = new System.Windows.Forms.RadioButton();
			this.radioToMe = new System.Windows.Forms.RadioButton();
			this.lblFromMe = new System.Windows.Forms.Label();
			this.radioFromMe = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblFromMe);
			this.groupBox1.Controls.Add(this.radioFromMe);
			this.groupBox1.Controls.Add(this.lblBi);
			this.groupBox1.Controls.Add(this.lblToMe);
			this.groupBox1.Controls.Add(this.radioBi);
			this.groupBox1.Controls.Add(this.radioToMe);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(464, 104);
			this.groupBox1.TabIndex = 16;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Relation Direction ";
			// 
			// lblBi
			// 
			this.lblBi.Location = new System.Drawing.Point(144, 75);
			this.lblBi.Name = "lblBi";
			this.lblBi.Size = new System.Drawing.Size(280, 16);
			this.lblBi.TabIndex = 5;
			this.lblBi.Text = "lblBi";
			// 
			// lblToMe
			// 
			this.lblToMe.Location = new System.Drawing.Point(144, 27);
			this.lblToMe.Name = "lblToMe";
			this.lblToMe.Size = new System.Drawing.Size(280, 16);
			this.lblToMe.TabIndex = 3;
			this.lblToMe.Text = "lblToMe";
			// 
			// radioBi
			// 
			this.radioBi.Location = new System.Drawing.Point(24, 72);
			this.radioBi.Name = "radioBi";
			this.radioBi.Size = new System.Drawing.Size(120, 24);
			this.radioBi.TabIndex = 2;
			this.radioBi.Text = "Bidirectional";
			// 
			// radioToMe
			// 
			this.radioToMe.Checked = true;
			this.radioToMe.Location = new System.Drawing.Point(24, 24);
			this.radioToMe.Name = "radioToMe";
			this.radioToMe.Size = new System.Drawing.Size(120, 24);
			this.radioToMe.TabIndex = 0;
			this.radioToMe.Text = "Directed";
			// 
			// lblFromMe
			// 
			this.lblFromMe.Location = new System.Drawing.Point(144, 51);
			this.lblFromMe.Name = "lblFromMe";
			this.lblFromMe.Size = new System.Drawing.Size(280, 16);
			this.lblFromMe.TabIndex = 7;
			this.lblFromMe.Text = "lblFromMe";
			// 
			// radioFromMe
			// 
			this.radioFromMe.Location = new System.Drawing.Point(24, 48);
			this.radioFromMe.Name = "radioFromMe";
			this.radioFromMe.Size = new System.Drawing.Size(120, 24);
			this.radioFromMe.TabIndex = 6;
			this.radioFromMe.Text = "Directed";
			// 
			// IntTblWiz2
			// 
			this.Controls.Add(this.groupBox1);
			this.Name = "IntTblWiz2";
			this.Size = new System.Drawing.Size(480, 216);
			this.Load += new System.EventHandler(this.IntTblWiz2_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void IntTblWiz2_Load(object sender, System.EventArgs e)
		{
			Frame.Description = "Choose, if the relation is directed or bidirectional.";

			RelatedTableInfo rti1 = model.IntermediateTableNode.IntermediateTable[0];
			RelatedTableInfo rti2 = model.IntermediateTableNode.IntermediateTable[1];

			RelationDirection dir = rti1.RelationDirection;
			if (dir == RelationDirection.Bidirectional)
				radioBi.Checked = true;
			else if (dir == RelationDirection.DirectedFromMe)
				radioFromMe.Checked = true;
			else 
				radioToMe.Checked = true;
			this.lblBi.Text = rti1.Table + " <-> " + rti2.Table;
			this.lblFromMe.Text = rti1.Table + " -> " + rti2.Table;
			this.lblToMe.Text = rti1.Table + " <- " + rti2.Table;

			// This would be the right view to specify a relation name.
			// But we don't implement it, because it doesn't make much sense,
			// as long as a mapping table can't be mapped twice.
			
		}

		public override void OnLeaveView()
		{
			RelatedTableInfo rti1 = model.IntermediateTableNode.IntermediateTable[0];
			RelatedTableInfo rti2 = model.IntermediateTableNode.IntermediateTable[1];
			if (radioBi.Checked)
				rti1.RelationDirection = RelationDirection.Bidirectional;
			else if (radioFromMe.Checked)
				rti1.RelationDirection = RelationDirection.DirectedFromMe;
			else
				rti1.RelationDirection = RelationDirection.DirectedToMe;
			rti2.RelationDirection = RelationDirectionClass.Reverse(rti1.RelationDirection);
		}

	}
}
