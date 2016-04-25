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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NDOInterfaces
{
	/// <summary>
	/// Zusammenfassung für DefaultCreateDbDialog.
	/// </summary>
	internal class DefaultCreateDbDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtDbName;
		private System.Windows.Forms.Label label1;
		IProvider provider;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtConnection;
		private System.Windows.Forms.Button btnConnection;

		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Gets the data entered by the user.
		/// </summary>
		public NDOCreateDbParameter NecessaryData
		{
			get { return new NDOCreateDbParameter(this.DatabaseName, this.Connection); }
		}
		
		string DatabaseName
		{
			get { return this.txtDbName.Text; }
		}

		string Connection
		{
			get { return this.txtConnection.Text; } 
		}

		string SaveString(string input)
		{
			if (input == null)
				return string.Empty;
			return input;
		}

		public DefaultCreateDbDialog(IProvider provider, NDOCreateDbParameter data)
		{
			//
			// Erforderlich für die Windows Form-Designerunterstützung
			//
			InitializeComponent();
			this.provider = provider;
			if (data != null)
			{
				this.txtConnection.Text = SaveString(data.Connection);
				this.txtDbName.Text = SaveString(data.DatabaseName);
			}
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

		#region Vom Windows Form-Designer generierter Code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DefaultCreateDbDialog));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtDbName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtConnection = new System.Windows.Forms.TextBox();
			this.btnConnection = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(368, 96);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(104, 32);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(248, 96);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(104, 32);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// txtDbName
			// 
			this.txtDbName.Location = new System.Drawing.Point(200, 56);
			this.txtDbName.Name = "txtDbName";
			this.txtDbName.Size = new System.Drawing.Size(272, 22);
			this.txtDbName.TabIndex = 2;
			this.txtDbName.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Database Name";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(176, 24);
			this.label3.TabIndex = 6;
			this.label3.Text = "Connection used for creation";
			// 
			// txtConnection
			// 
			this.txtConnection.Location = new System.Drawing.Point(200, 24);
			this.txtConnection.Name = "txtConnection";
			this.txtConnection.Size = new System.Drawing.Size(232, 22);
			this.txtConnection.TabIndex = 7;
			this.txtConnection.Text = "";
			// 
			// btnConnection
			// 
			this.btnConnection.Location = new System.Drawing.Point(440, 24);
			this.btnConnection.Name = "btnConnection";
			this.btnConnection.Size = new System.Drawing.Size(32, 24);
			this.btnConnection.TabIndex = 8;
			this.btnConnection.Text = "...";
			this.btnConnection.Click += new System.EventHandler(this.btnConnection_Click);
			// 
			// DefaultCreateDbDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(488, 144);
			this.Controls.Add(this.btnConnection);
			this.Controls.Add(this.txtConnection);
			this.Controls.Add(this.txtDbName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DefaultCreateDbDialog";
			this.Text = "Create Database";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnConnection_Click(object sender, System.EventArgs e)
		{
			string conn = null;
			if (this.provider.ShowConnectionDialog(ref conn) == DialogResult.Cancel)
				return;
			this.txtConnection.Text = conn;
		}
	}
}
