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

namespace ClassGenerator.IntermediateClassWizard
{
	/// <summary>
	/// Zusammenfassung für IntClassWiz2.
	/// </summary>
	internal class IntClassWiz2 : ViewBase
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblClass;
		private System.Windows.Forms.GroupBox gbCodingStyle;
		private System.Windows.Forms.RadioButton radioArrayList;
		private System.Windows.Forms.RadioButton radioNDOArrayList;
		private System.Windows.Forms.RadioButton radioIList;
		private System.Windows.Forms.TextBox txtFieldName;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblBi;
		private System.Windows.Forms.Label lblToMe;
		private System.Windows.Forms.RadioButton radioBi;
		private System.Windows.Forms.RadioButton radioToMe;
		private System.Windows.Forms.Label lblRelationFieldName;

		IntermediateClassWizardModel model;
		public IntClassWiz2(IModel model)
		{
			this.model = (IntermediateClassWizardModel) model;
			InitializeComponent();
#if !NDO11
            this.radioArrayList.Text = "List<T>";
            this.radioIList.Text = "IList<T>";
            this.radioNDOArrayList.Visible = false;
#endif
        }

		public IntClassWiz2()
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
			this.label3 = new System.Windows.Forms.Label();
			this.lblClass = new System.Windows.Forms.Label();
			this.gbCodingStyle = new System.Windows.Forms.GroupBox();
			this.radioArrayList = new System.Windows.Forms.RadioButton();
			this.radioNDOArrayList = new System.Windows.Forms.RadioButton();
			this.radioIList = new System.Windows.Forms.RadioButton();
			this.txtFieldName = new System.Windows.Forms.TextBox();
			this.lblRelationFieldName = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblBi = new System.Windows.Forms.Label();
			this.lblToMe = new System.Windows.Forms.Label();
			this.radioBi = new System.Windows.Forms.RadioButton();
			this.radioToMe = new System.Windows.Forms.RadioButton();
			this.gbCodingStyle.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 24);
			this.label3.TabIndex = 14;
			this.label3.Text = "Class:";
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
			// gbCodingStyle
			// 
			this.gbCodingStyle.Controls.Add(this.radioArrayList);
			this.gbCodingStyle.Controls.Add(this.radioNDOArrayList);
			this.gbCodingStyle.Controls.Add(this.radioIList);
			this.gbCodingStyle.Location = new System.Drawing.Point(24, 72);
			this.gbCodingStyle.Name = "gbCodingStyle";
			this.gbCodingStyle.Size = new System.Drawing.Size(464, 48);
			this.gbCodingStyle.TabIndex = 12;
			this.gbCodingStyle.TabStop = false;
			this.gbCodingStyle.Text = " Coding Style";
			// 
			// radioArrayList
			// 
			this.radioArrayList.Location = new System.Drawing.Point(152, 16);
			this.radioArrayList.Name = "radioArrayList";
			this.radioArrayList.Size = new System.Drawing.Size(136, 24);
			this.radioArrayList.TabIndex = 3;
			this.radioArrayList.Text = "ArrayList";
			// 
			// radioNDOArrayList
			// 
			this.radioNDOArrayList.Location = new System.Drawing.Point(312, 16);
			this.radioNDOArrayList.Name = "radioNDOArrayList";
			this.radioNDOArrayList.Size = new System.Drawing.Size(136, 24);
			this.radioNDOArrayList.TabIndex = 2;
			this.radioNDOArrayList.Text = "NDOArrayList";
			// 
			// radioIList
			// 
			this.radioIList.Checked = true;
			this.radioIList.Location = new System.Drawing.Point(16, 16);
			this.radioIList.Name = "radioIList";
			this.radioIList.Size = new System.Drawing.Size(80, 24);
			this.radioIList.TabIndex = 0;
			this.radioIList.TabStop = true;
			this.radioIList.Text = "IList";
			// 
			// txtFieldName
			// 
			this.txtFieldName.Location = new System.Drawing.Point(168, 37);
			this.txtFieldName.Name = "txtFieldName";
			this.txtFieldName.Size = new System.Drawing.Size(192, 22);
			this.txtFieldName.TabIndex = 10;
			this.txtFieldName.Text = "";
			// 
			// lblRelationFieldName
			// 
			this.lblRelationFieldName.Location = new System.Drawing.Point(24, 40);
			this.lblRelationFieldName.Name = "lblRelationFieldName";
			this.lblRelationFieldName.Size = new System.Drawing.Size(136, 24);
			this.lblRelationFieldName.TabIndex = 9;
			this.lblRelationFieldName.Text = "Relation Field Name:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblBi);
			this.groupBox1.Controls.Add(this.lblToMe);
			this.groupBox1.Controls.Add(this.radioBi);
			this.groupBox1.Controls.Add(this.radioToMe);
			this.groupBox1.Location = new System.Drawing.Point(24, 128);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(464, 80);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Relation Direction ";
			// 
			// lblBi
			// 
			this.lblBi.Location = new System.Drawing.Point(144, 48);
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
			this.radioBi.Location = new System.Drawing.Point(24, 48);
			this.radioBi.Name = "radioBi";
			this.radioBi.Size = new System.Drawing.Size(120, 24);
			this.radioBi.TabIndex = 2;
			this.radioBi.Text = "Bidirectional";
			this.radioBi.CheckedChanged += new System.EventHandler(this.OnDirectionChanged);
			// 
			// radioToMe
			// 
			this.radioToMe.Checked = true;
			this.radioToMe.Location = new System.Drawing.Point(24, 24);
			this.radioToMe.Name = "radioToMe";
			this.radioToMe.Size = new System.Drawing.Size(120, 24);
			this.radioToMe.TabIndex = 0;
			this.radioToMe.TabStop = true;
			this.radioToMe.Text = "Directed";
			this.radioToMe.CheckedChanged += new System.EventHandler(this.OnDirectionChanged);
			// 
			// IntClassWiz2
			// 
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblClass);
			this.Controls.Add(this.gbCodingStyle);
			this.Controls.Add(this.txtFieldName);
			this.Controls.Add(this.lblRelationFieldName);
			this.Name = "IntClassWiz2";
			this.Size = new System.Drawing.Size(512, 224);
			this.Load += new System.EventHandler(this.IntClassWiz2_Load);
			this.gbCodingStyle.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		public override void OnLeaveView()
		{
			IntermediateClassInfo classInfo = model[model.Index];

			if (this.radioIList.Checked)
				classInfo.CodingStyle = CodingStyle.IList;
			else if (this.radioArrayList.Checked)
				classInfo.CodingStyle = CodingStyle.ArrayList;
			else if (this.radioNDOArrayList.Checked)
				classInfo.CodingStyle = CodingStyle.NDOArrayList;

			RelationDirection dir;
			if (radioToMe.Checked)
				dir = RelationDirection.DirectedFromMe;// View from the intermediate class
			else
				dir = RelationDirection.Bidirectional;
			classInfo.RelationDirection = dir;
		}

		private void IntClassWiz2_Load(object sender, System.EventArgs e)
		{
			IntermediateClassInfo classInfo = model[model.Index];

			RelationDirection dir = classInfo.RelationDirection;
			this.lblBi.Text = model.TableNode.Text + "<->" + classInfo.Table;
			this.lblToMe.Text = model.TableNode.Text + "->" + classInfo.Table;
			if (dir == RelationDirection.DirectedToMe)
			{
				radioToMe.Checked = true;
			}
			else
			{
				radioBi.Checked = true;
			}

			this.lblClass.Text = classInfo.Type;
			if (classInfo.ForeignFieldName == string.Empty)
			{
				string name = model.TableNode.Text.Replace(" ", string.Empty);
				name = name.Substring(0, 1).ToLower() + name.Substring(1);
				classInfo.ForeignFieldName = name;
			}
			this.txtFieldName.DataBindings.Add("Text", classInfo, "ForeignFieldName");

			if (classInfo.CodingStyle == CodingStyle.IList)
				this.radioIList.Checked = true;
			else if (classInfo.CodingStyle == CodingStyle.ArrayList)
				this.radioArrayList.Checked = true;
			else if (classInfo.CodingStyle == CodingStyle.NDOArrayList)
				this.radioNDOArrayList.Checked = true;
#if !NDO11
            radioArrayList.Text = "List<T>";
            radioIList.Text = "IList<T>";
            radioNDOArrayList.Visible = false;
#endif

			Frame.Description = "The relation is visible in class '" + classInfo.Type +"', if Relation Direction is bidirectional.\n\nIf this is the case, enter the name of the field, which will contain the relation, and the relation coding style.";
		}

		private void OnDirectionChanged(object sender, System.EventArgs e)
		{
			bool bi = this.radioBi.Checked;
			this.gbCodingStyle.Enabled = bi;
			this.lblRelationFieldName.Enabled = bi;
			this.txtFieldName.Enabled = bi;
		}

	}
}
