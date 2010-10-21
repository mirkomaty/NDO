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
using System.Windows.Forms;
using WizardBase;

namespace ClassGenerator.AssemblyWizard
{
	internal class AssemblyWiz3 : WizardBase.ViewBase
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private ComboBox cbTargetLanguage;
		private Label label1;
		private System.ComponentModel.IContainer components = null;

		public AssemblyWiz3()
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();
		}

		AssemblyWizModel model;
		public AssemblyWiz3(IModel model)
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();
			this.cbTargetLanguage.DataSource = Enum.GetNames(typeof(TargetLanguage));
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
			this.button1 = new System.Windows.Forms.Button();
			this.cbTargetLanguage = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point( 16, 18 );
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size( 424, 20 );
			this.textBox1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point( 456, 18 );
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size( 40, 24 );
			this.button1.TabIndex = 1;
			this.button1.Text = "...";
			this.button1.Click += new System.EventHandler( this.button1_Click );
			// 
			// cbTargetLanguage
			// 
			this.cbTargetLanguage.FormattingEnabled = true;
			this.cbTargetLanguage.Location = new System.Drawing.Point( 137, 51 );
			this.cbTargetLanguage.Name = "cbTargetLanguage";
			this.cbTargetLanguage.Size = new System.Drawing.Size( 159, 21 );
			this.cbTargetLanguage.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 16, 51 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 89, 13 );
			this.label1.TabIndex = 3;
			this.label1.Text = "Target Language";
			// 
			// AssemblyWiz3
			// 
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.cbTargetLanguage );
			this.Controls.Add( this.button1 );
			this.Controls.Add( this.textBox1 );
			this.Name = "AssemblyWiz3";
			this.Size = new System.Drawing.Size( 512, 80 );
			this.Load += new System.EventHandler( this.AssemblyWiz2_Load );
			this.ResumeLayout( false );
			this.PerformLayout();

		}
		#endregion

		private void AssemblyWiz2_Load(object sender, System.EventArgs e)
		{
			this.textBox1.DataBindings.Add("Text", this.model, "ProjectDirectory");
			this.cbTargetLanguage.Text = this.model.TargetLanguage.ToString();
			Frame.Description = "Choose a Directory, where the generated Code will reside. You can create a new directory in the dialog.";
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = textBox1.Text;
			DialogResult dr = fbd.ShowDialog();
			if (dr == DialogResult.Cancel)
				return;
			model.ProjectDirectory = textBox1.Text = fbd.SelectedPath;					
		}

		public override void OnLeaveView()
		{
			this.model.TargetLanguage = (TargetLanguage) Enum.Parse(typeof(TargetLanguage), this.cbTargetLanguage.Text);
		}

	}
}

