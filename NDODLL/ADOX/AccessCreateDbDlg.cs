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
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ADOX
{
	/// <summary>
	/// Zusammenfassung für AccessCreateDbDlg.
	/// </summary>
	internal class AccessCreateDbDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtFileName;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnEditFile;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AccessCreateDbDlg()
		{
			//
			// Erforderlich für die Windows Form-Designerunterstützung
			//
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

		#region Vom Windows Form-Designer generierter Code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtFileName = new System.Windows.Forms.TextBox();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnEditFile = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtFileName
			// 
			this.txtFileName.Location = new System.Drawing.Point(104, 24);
			this.txtFileName.Name = "txtFileName";
			this.txtFileName.Size = new System.Drawing.Size(320, 22);
			this.txtFileName.TabIndex = 0;
			this.txtFileName.Text = "";
			// 
			// txtUserName
			// 
			this.txtUserName.Location = new System.Drawing.Point(104, 56);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size(320, 22);
			this.txtUserName.TabIndex = 1;
			this.txtUserName.Text = "Admin";
			this.txtUserName.Visible = false;
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(104, 88);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Size = new System.Drawing.Size(320, 22);
			this.txtPassword.TabIndex = 2;
			this.txtPassword.Text = "";
			this.txtPassword.Visible = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 24);
			this.label1.TabIndex = 3;
			this.label1.Text = "File Name";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 24);
			this.label2.TabIndex = 4;
			this.label2.Text = "User Name";
			this.label2.Visible = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 24);
			this.label3.TabIndex = 5;
			this.label3.Text = "Password";
			this.label3.Visible = false;
			// 
			// btnEditFile
			// 
			this.btnEditFile.Location = new System.Drawing.Point(432, 24);
			this.btnEditFile.Name = "btnEditFile";
			this.btnEditFile.Size = new System.Drawing.Size(40, 24);
			this.btnEditFile.TabIndex = 6;
			this.btnEditFile.Text = "...";
			this.btnEditFile.Click += new System.EventHandler(this.btnEditFile_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(296, 128);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 32);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(400, 128);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 32);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			// 
			// AccessCreateDbDlg
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(488, 168);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnEditFile);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.txtUserName);
			this.Controls.Add(this.txtFileName);
			this.Name = "AccessCreateDbDlg";
			this.Text = "Create Access Database";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnEditFile_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title="Enter an Access Database Filename";
			dlg.Filter="Access Database File|*.mdb";
			dlg.FilterIndex=1;
			dlg.OverwritePrompt=true;

			if(dlg.ShowDialog() == DialogResult.Cancel)
				return;
			this.txtFileName.Text = dlg.FileName;
		}

		public string ConnectionString
		{

			get 
			{
				//Provider=Microsoft.Jet.OLEDB.4.0;Password=schnulli;User ID=Admin3;Data Source=222;Persist Security Info=True
				StringBuilder sb = new StringBuilder("Provider=Microsoft.Jet.OLEDB.4.0;");
//				bool persistSi = false;
//				if (this.txtPassword.Text.Trim() != string.Empty)
//				{
//					sb.Append("Password=");
//					sb.Append(this.txtPassword.Text.Trim());
//					sb.Append(';');
//				}
//				if (this.txtUserName.Text != string.Empty && this.txtUserName.Text.Trim() != "Admin")
//				{
//					sb.Append("User ID=");
//					sb.Append(this.txtPassword.Text.Trim());
//					sb.Append(';');
//					persistSi = true;
//				}
				sb.Append("Data Source=");
				sb.Append(this.txtFileName.Text);
				sb.Append(';');
//				sb.Append("Persist Security Info=");
//				if (persistSi)
//					sb.Append("True");
//				else
//					sb.Append("False");
				return sb.ToString();
			}
		}
	}
}
