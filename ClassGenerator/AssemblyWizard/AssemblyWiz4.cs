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
using System.Windows.Forms;
using WizardBase;
using NDO;

namespace ClassGenerator.AssemblyWizard
{
	internal class AssemblyWiz4 : WizardBase.ViewBase
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioUseClassField;
		private System.Windows.Forms.RadioButton radioUseOidType;
		private System.Windows.Forms.CheckBox cbMapStringsAsGuids;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.IContainer components = null;

		public AssemblyWiz4()
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();

			// TODO: Initialisierungen nach dem Aufruf von InitializeComponent hinzufügen
		}
		AssemblyWizModel model;
		public AssemblyWiz4(IModel model)
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();
			this.model = (AssemblyWizModel) model;
		}

		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Vom Designer generierter Code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioUseOidType = new System.Windows.Forms.RadioButton();
			this.radioUseClassField = new System.Windows.Forms.RadioButton();
			this.cbMapStringsAsGuids = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(160, 16);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(320, 22);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioUseOidType);
			this.groupBox1.Controls.Add(this.radioUseClassField);
			this.groupBox1.Controls.Add(this.cbMapStringsAsGuids);
			this.groupBox1.Location = new System.Drawing.Point(16, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(464, 80);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Primary Key Mapping";
			// 
			// radioUseOidType
			// 
			this.radioUseOidType.Location = new System.Drawing.Point(16, 48);
			this.radioUseOidType.Name = "radioUseOidType";
			this.radioUseOidType.Size = new System.Drawing.Size(200, 24);
			this.radioUseOidType.TabIndex = 2;
			this.radioUseOidType.Text = "Use NDOOidType attribute";
			// 
			// radioUseClassField
			// 
			this.radioUseClassField.Location = new System.Drawing.Point(240, 48);
			this.radioUseClassField.Name = "radioUseClassField";
			this.radioUseClassField.Size = new System.Drawing.Size(192, 24);
			this.radioUseClassField.TabIndex = 3;
			this.radioUseClassField.Text = "Use class field";
			// 
			// cbMapStringsAsGuids
			// 
			this.cbMapStringsAsGuids.Location = new System.Drawing.Point(16, 24);
			this.cbMapStringsAsGuids.Name = "cbMapStringsAsGuids";
			this.cbMapStringsAsGuids.Size = new System.Drawing.Size(352, 24);
			this.cbMapStringsAsGuids.TabIndex = 1;
			this.cbMapStringsAsGuids.Text = "Map Strings as Guids";
			this.cbMapStringsAsGuids.CheckedChanged += new System.EventHandler(this.cbMapStringsAsGuids_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 24);
			this.label1.TabIndex = 4;
			this.label1.Text = "Default Namespace:";
			// 
			// AssemblyWiz4
			// 
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.Name = "AssemblyWiz4";
			this.Size = new System.Drawing.Size(512, 144);
			this.Load += new System.EventHandler(this.AssemblyWiz4_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void AssemblyWiz4_Load(object sender, System.EventArgs e)
		{
            if (NDOProviderFactory.Instance[model.ConnectionType].SupportsNativeGuidType)
                this.cbMapStringsAsGuids.Enabled = false;
			this.textBox1.DataBindings.Add("Text", this.model, "DefaultNamespace"); 		
			Frame.Description = "Choose a Namespace, in which the classes will be defined.\n\n"
				+ "Select, how primary key columns are initialized, if they are strings:\n1. NDO will be generate Guids\n2. PK colums are mapped to a class field\n3. The NDOOidType attribute and a callback function of the Persistence Manager will be used.";
			this.cbMapStringsAsGuids.Checked = model.MapStringsAsGuids;
			this.radioUseClassField.Checked = model.UseClassField;
		}

		public override void OnLeaveView()
		{
			if (model.ProjectName == null || model.ProjectName == string.Empty)
				model.ProjectName = model.DefaultNamespace;
			model.MapStringsAsGuids = this.cbMapStringsAsGuids.Checked;
			model.UseClassField = this.radioUseClassField.Checked;
		}

		private void cbMapStringsAsGuids_CheckedChanged(object sender, System.EventArgs e)
		{
			bool dontMapAsGuids = !this.cbMapStringsAsGuids.Checked;
			if (!dontMapAsGuids)
				this.radioUseOidType.Checked = true;		
			this.radioUseClassField.Enabled = dontMapAsGuids;
			this.radioUseOidType.Enabled = dontMapAsGuids;
		}



	}
}

