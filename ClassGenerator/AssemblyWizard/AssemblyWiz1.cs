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
using NDO;
using NDOInterfaces;

namespace ClassGenerator.AssemblyWizard
{
	/// <summary>
	/// Zusammenfassung für AssemblyWiz1.
	/// </summary>
	internal class AssemblyWiz1 : WizardBase.ViewBase
	{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ComboBox cbType;
		private CheckBox chkIsXmlSchema;
        private Button btnLoad;
		AssemblyWizModel model;

		public AssemblyWiz1(IModel model)
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();
			this.model = (AssemblyWizModel) model;

		}

		public AssemblyWiz1()
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();

			// TODO: Initialisierungen nach dem Aufruf von InitializeComponent hinzufügen

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
            this.cbType = new System.Windows.Forms.ComboBox();
            this.chkIsXmlSchema = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbType
            // 
            this.cbType.Location = new System.Drawing.Point(65, 39);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(168, 21);
            this.cbType.TabIndex = 0;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // chkIsXmlSchema
            // 
            this.chkIsXmlSchema.AutoSize = true;
            this.chkIsXmlSchema.Location = new System.Drawing.Point(65, 12);
            this.chkIsXmlSchema.Name = "chkIsXmlSchema";
            this.chkIsXmlSchema.Size = new System.Drawing.Size(140, 17);
            this.chkIsXmlSchema.TabIndex = 1;
            this.chkIsXmlSchema.Text = "Analyze an Xml Schema";
            this.chkIsXmlSchema.UseVisualStyleBackColor = true;
            this.chkIsXmlSchema.CheckedChanged += new System.EventHandler(this.chkIsXmlSchema_CheckedChanged);
            // 
            // btnLoad
            // 
            this.btnLoad.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.btnLoad.Location = new System.Drawing.Point(356, 31);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(125, 28);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load .clgen File";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // AssemblyWiz1
            // 
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.chkIsXmlSchema);
            this.Controls.Add(this.cbType);
            this.Name = "AssemblyWiz1";
            this.Size = new System.Drawing.Size(520, 72);
            this.Load += new System.EventHandler(this.AssemblyWiz1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void AssemblyWiz1_Load(object sender, System.EventArgs e)
		{
			string[] arr = NDOProviderFactory.Instance.ProviderNames;
			//ArrayList al = new ArrayList();
			//foreach(string s in arr)
			//{
			//    if (s != "Sql" && s != "OleDb")
			//        al.Add(s);
			//}
			this.cbType.DataSource = arr;
			if (this.cbType.Items.Count > 0)
			{
				this.cbType.SelectedIndex = 0;
				model.ConnectionType = (string) this.cbType.Items[0];
			}
			Frame.Description = "Welcome to the NDO class generator. This wizard will help you to enter all project informations you need to generate a C# project from tables of a database.\n\nPlease choose a NDO provider type.";
		}

		private void cbType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			model.ConnectionType = (string) this.cbType.Items[cbType.SelectedIndex];		
		}

		private void chkIsXmlSchema_CheckedChanged( object sender, EventArgs e )
		{
			model.IsXmlSchema = this.chkIsXmlSchema.Checked;
		}

        private void btnLoad_Click(object sender, EventArgs e)
        {
            base.Frame.Ignore();
        }

	}
}
