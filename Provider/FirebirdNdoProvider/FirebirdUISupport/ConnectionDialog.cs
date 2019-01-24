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
using FirebirdSql.Data.FirebirdClient;

namespace FirebirdUISupport
{
	/// <summary>
	/// Zusammenfassung für ConnectionDialog.
	/// </summary>
	public class ConnectionDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtDataSource;
		private System.Windows.Forms.TextBox txtUserId;
		private System.Windows.Forms.TextBox txtPassword;
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtDatabase;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOpenFile;

		bool createDatabase = false;
		public bool CreateDatabase
		{
			get { return createDatabase; }
			set { createDatabase = value; }
		}

		FbConnectionStringBuilder connectionStringBuilder;
        ConnSbDummy connSbDummy;

        class ConnSbDummy
        {
            public ConnSbDummy(FbConnectionStringBuilder sb)
            {
                this.database = sb.Database;
                this.dataSource = sb.DataSource;
                this.password = sb.Password;
                this.userID = sb.UserID;
            }

            public void SetSb(FbConnectionStringBuilder sb)
            {
                sb.Database = this.database;
                sb.DataSource = this.dataSource;
                sb.Password = this.password;
                sb.UserID = this.userID;
            }

            string database;
            public string Database
            {
                get { return database; }
                set { database = value; }
            }
            string dataSource;
            public string DataSource
            {
                get { return dataSource; }
                set { dataSource = value; }
            }
            string password;
            public string Password
            {
                get { return password; }
                set { password = value; }
            }
            string userID;
            public string UserID
            {
                get { return userID; }
                set { userID = value; }
            }
        }

		public ConnectionDialog(FbConnectionStringBuilder sb)
		{
			//
			// Erforderlich für die Windows Form-Designerunterstützung
			//
			InitializeComponent();
			this.connectionStringBuilder = sb;
            this.connSbDummy = new ConnSbDummy(sb);	
            // Out of whatever reason we can't bind directly to the properties
            // of the connectionStringBuilder object.
			this.txtDatabase.DataBindings.Add("Text", this.connSbDummy, "Database");
            this.txtDataSource.DataBindings.Add("Text", this.connSbDummy, "DataSource");
            this.txtPassword.DataBindings.Add("Text", this.connSbDummy, "Password");
            this.txtUserId.DataBindings.Add("Text", this.connSbDummy, "UserID");
		}


		public FbConnectionStringBuilder ConnectionStringBuilder
		{
			get 
			{ 
				return this.connectionStringBuilder; 
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Id";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "Data Source";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Location = new System.Drawing.Point(144, 24);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(288, 22);
            this.txtDataSource.TabIndex = 3;
            // 
            // txtUserId
            // 
            this.txtUserId.Location = new System.Drawing.Point(144, 104);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(288, 22);
            this.txtUserId.TabIndex = 4;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(144, 144);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(288, 22);
            this.txtPassword.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(328, 184);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(104, 32);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(208, 184);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 32);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(144, 64);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(288, 22);
            this.txtDatabase.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 24);
            this.label4.TabIndex = 8;
            this.label4.Text = "Database";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(440, 64);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(32, 24);
            this.btnOpenFile.TabIndex = 10;
            this.btnOpenFile.Text = "...";
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // ConnectionDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(480, 232);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserId);
            this.Controls.Add(this.txtDataSource);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ConnectionDialog";
            this.Text = "Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btnOpenFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (createDatabase)
				ofd.Filter = "Firebird Database (*.FDB)|*.FDB";
			else
				ofd.Filter = "Firebird Database (*.FDB;*.FBD)|*.FDB;*.FBD";
			ofd.FileName = this.connectionStringBuilder.Database;
			if (createDatabase)
				ofd.CheckFileExists = false;
			else
				ofd.CheckFileExists = true;
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				this.txtDatabase.Text = ofd.FileName;
				// Binding doesn't automatically pass the string over to the 
				// component
				this.connectionStringBuilder.Database = ofd.FileName;
			}
		}

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.connectionStringBuilder.Database = this.connSbDummy.Database;
            this.connectionStringBuilder.DataSource = this.connSbDummy.DataSource;
            this.connectionStringBuilder.Password = this.connSbDummy.Password;
            this.connectionStringBuilder.UserID = this.connSbDummy.UserID;
        }
	}
}
