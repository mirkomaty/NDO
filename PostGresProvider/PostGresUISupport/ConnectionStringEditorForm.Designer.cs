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


namespace NDO.PostGresUISupport
{
	partial class ConnectionStringEditorForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.txtDatabase = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server";
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(88, 24);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(198, 20);
			this.txtServer.TabIndex = 1;
			// 
			// txtDatabase
			// 
			this.txtDatabase.Location = new System.Drawing.Point(88, 50);
			this.txtDatabase.Name = "txtDatabase";
			this.txtDatabase.Size = new System.Drawing.Size(198, 20);
			this.txtDatabase.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Database";
			// 
			// txtUser
			// 
			this.txtUser.Location = new System.Drawing.Point(88, 76);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(198, 20);
			this.txtUser.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(22, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "User";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(88, 102);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Size = new System.Drawing.Size(198, 20);
			this.txtPassword.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(22, 107);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Password";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(182, 158);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(104, 32);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(72, 158);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(104, 32);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// ConnectionStringEditorForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(313, 218);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtUser);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtDatabase);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.label1);
			this.Name = "ConnectionStringEditorForm";
			this.Text = "PostGre Connection String";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.TextBox txtDatabase;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtUser;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}