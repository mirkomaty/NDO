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
using System.Globalization;
using System.Windows.Forms;

using FirebirdSql.Data.FirebirdClient;

namespace NDO.FirebirdProvider
{
	public class ConnectionDialog : System.Windows.Forms.Form
	{
        private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button cmdTest;
		private System.Windows.Forms.Button cmdAccept;
		private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.TextBox txtDataSource;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblDatabase;
		private System.Windows.Forms.Button cmdGetFile;
		private System.Windows.Forms.GroupBox grbLogin;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.Label lblUser;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.GroupBox grbSettings;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.TextBox txtDatabase;
		private System.Windows.Forms.CheckBox chkPooling;
		private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.TextBox txtRole;
		private System.Windows.Forms.NumericUpDown txtLifeTime;
		private System.Windows.Forms.Label lblTimeout;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown txtTimeout;
		private System.Windows.Forms.ComboBox cboCharset;
		private System.Windows.Forms.Label lblDialect;
		private System.Windows.Forms.ComboBox cboPacketSize;
		private System.Windows.Forms.ComboBox cboServerType;
		private System.Windows.Forms.Label lblServerType;
		private System.Windows.Forms.Label lblDataSource;
		private System.Windows.Forms.Label lblCharset;
		private System.Windows.Forms.ComboBox cboDialect;
        private Label lblLifeTime;

        FbConnectionStringBuilder connectionStringBuilder;

		public ConnectionDialog(FbConnectionStringBuilder connectionStringBuilder)
		{
			this.connectionStringBuilder = connectionStringBuilder;

            InitializeComponent();
			InitializeData();
		}

		private void InitializeData()
		{
            this.txtDataSource.DataBindings.Add("Text", this.connectionStringBuilder, "DataSource");
			this.txtPort.DataBindings.Add("Text", this.connectionStringBuilder, "Port");
			this.txtDatabase.DataBindings.Add("Text", this.connectionStringBuilder, "Database");
			this.txtUserName.DataBindings.Add("Text", this.connectionStringBuilder, "UserID");
			this.txtPassword.DataBindings.Add("Text", this.connectionStringBuilder, "Password");
			this.txtRole.DataBindings.Add("Text", this.connectionStringBuilder, "Role");
//            this.cboPacketSize.Text = this.connectionStringBuilder.PacketSize.ToString();
			this.cboDialect.Text = this.connectionStringBuilder.Dialect.ToString();
//			this.txtLifeTime.Value = this.connectionStringBuilder.ConnectionLifeTime;
//			this.txtTimeout.Value = this.connectionStringBuilder.ConnectionTimeout;
//			this.cboCharset.Text = this.connectionStringBuilder.Charset;
//            this.chkPooling.Checked = this.connectionStringBuilder.Pooling;

            //switch (this.connectionStringBuilder.ServerType)
            //{
            //    case FbServerType.Default:
            //        this.cboServerType.Text = "Super/Classic Server";
            //        break;

            //    case FbServerType.Embedded:
            //        this.cboServerType.Text = "Embedded Server";
            //        break;
            //}
		}

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

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
            this.cmdTest = new System.Windows.Forms.Button();
            this.cmdAccept = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDataSource = new System.Windows.Forms.Label();
            this.cboDialect = new System.Windows.Forms.ComboBox();
            this.grbSettings = new System.Windows.Forms.GroupBox();
            this.cboPacketSize = new System.Windows.Forms.ComboBox();
            this.txtLifeTime = new System.Windows.Forms.NumericUpDown();
            this.txtTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.chkPooling = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboServerType = new System.Windows.Forms.ComboBox();
            this.lblServerType = new System.Windows.Forms.Label();
            this.grbLogin = new System.Windows.Forms.GroupBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.txtRole = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.cmdGetFile = new System.Windows.Forms.Button();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDialect = new System.Windows.Forms.Label();
            this.lblCharset = new System.Windows.Forms.Label();
            this.cboCharset = new System.Windows.Forms.ComboBox();
            this.lblLifeTime = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.grbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLifeTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeout)).BeginInit();
            this.grbLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdTest
            // 
            this.cmdTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdTest.Location = new System.Drawing.Point(7, 253);
            this.cmdTest.Name = "cmdTest";
            this.cmdTest.Size = new System.Drawing.Size(75, 23);
            this.cmdTest.TabIndex = 2;
            this.cmdTest.Text = "&Test";
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            // 
            // cmdAccept
            // 
            this.cmdAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdAccept.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdAccept.Location = new System.Drawing.Point(358, 253);
            this.cmdAccept.Name = "cmdAccept";
            this.cmdAccept.Size = new System.Drawing.Size(75, 23);
            this.cmdAccept.TabIndex = 3;
            this.cmdAccept.Text = "OK";
            this.cmdAccept.Click += new System.EventHandler(this.cmdAccept_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdCancel.Location = new System.Drawing.Point(435, 253);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(72, 23);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "&Cancel";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Location = new System.Drawing.Point(8, 24);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(136, 22);
            this.txtDataSource.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDataSource);
            this.panel1.Controls.Add(this.cboDialect);
            this.panel1.Controls.Add(this.grbSettings);
            this.panel1.Controls.Add(this.grbLogin);
            this.panel1.Controls.Add(this.cmdGetFile);
            this.panel1.Controls.Add(this.txtDatabase);
            this.panel1.Controls.Add(this.lblDatabase);
            this.panel1.Controls.Add(this.txtPort);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblDialect);
            this.panel1.Controls.Add(this.txtDataSource);
            this.panel1.Controls.Add(this.lblCharset);
            this.panel1.Controls.Add(this.cboCharset);
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(520, 239);
            this.panel1.TabIndex = 0;
            // 
            // lblDataSource
            // 
            this.lblDataSource.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDataSource.Location = new System.Drawing.Point(8, 8);
            this.lblDataSource.Name = "lblDataSource";
            this.lblDataSource.Size = new System.Drawing.Size(64, 16);
            this.lblDataSource.TabIndex = 13;
            this.lblDataSource.Text = "Data Source";
            // 
            // cboDialect
            // 
            this.cboDialect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDialect.FormattingEnabled = true;
            this.cboDialect.Items.AddRange(new object[] {
            "1",
            "3"});
            this.cboDialect.Location = new System.Drawing.Point(238, 24);
            this.cboDialect.Name = "cboDialect";
            this.cboDialect.Size = new System.Drawing.Size(73, 24);
            this.cboDialect.TabIndex = 4;
            // 
            // grbSettings
            // 
            this.grbSettings.Controls.Add(this.lblLifeTime);
            this.grbSettings.Controls.Add(this.cboPacketSize);
            this.grbSettings.Controls.Add(this.txtLifeTime);
            this.grbSettings.Controls.Add(this.txtTimeout);
            this.grbSettings.Controls.Add(this.lblTimeout);
            this.grbSettings.Controls.Add(this.chkPooling);
            this.grbSettings.Controls.Add(this.label3);
            this.grbSettings.Controls.Add(this.cboServerType);
            this.grbSettings.Controls.Add(this.lblServerType);
            this.grbSettings.Location = new System.Drawing.Point(229, 96);
            this.grbSettings.Name = "grbSettings";
            this.grbSettings.Size = new System.Drawing.Size(282, 120);
            this.grbSettings.TabIndex = 11;
            this.grbSettings.TabStop = false;
            this.grbSettings.Text = "Connection Settings";
            // 
            // cboPacketSize
            // 
            this.cboPacketSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPacketSize.FormattingEnabled = true;
            this.cboPacketSize.Items.AddRange(new object[] {
            "1024",
            "2048",
            "4096",
            "8192",
            "16384"});
            this.cboPacketSize.Location = new System.Drawing.Point(207, 54);
            this.cboPacketSize.Name = "cboPacketSize";
            this.cboPacketSize.Size = new System.Drawing.Size(64, 24);
            this.cboPacketSize.TabIndex = 5;
            // 
            // txtLifeTime
            // 
            this.txtLifeTime.Location = new System.Drawing.Point(72, 24);
            this.txtLifeTime.Name = "txtLifeTime";
            this.txtLifeTime.Size = new System.Drawing.Size(48, 22);
            this.txtLifeTime.TabIndex = 0;
            this.txtLifeTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(215, 24);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(56, 22);
            this.txtTimeout.TabIndex = 2;
            this.txtTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblTimeout
            // 
            this.lblTimeout.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTimeout.Location = new System.Drawing.Point(155, 26);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(54, 16);
            this.lblTimeout.TabIndex = 1;
            this.lblTimeout.Text = "Timeout";
            // 
            // chkPooling
            // 
            this.chkPooling.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkPooling.Location = new System.Drawing.Point(16, 52);
            this.chkPooling.Name = "chkPooling";
            this.chkPooling.Size = new System.Drawing.Size(121, 25);
            this.chkPooling.TabIndex = 3;
            this.chkPooling.Text = "Enable pooling";
            // 
            // label3
            // 
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Location = new System.Drawing.Point(143, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Packet size";
            // 
            // cboServerType
            // 
            this.cboServerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboServerType.FormattingEnabled = true;
            this.cboServerType.Items.AddRange(new object[] {
            "Super/Classic Server",
            "Embedded Server"});
            this.cboServerType.Location = new System.Drawing.Point(127, 88);
            this.cboServerType.Name = "cboServerType";
            this.cboServerType.Size = new System.Drawing.Size(144, 24);
            this.cboServerType.TabIndex = 13;
            // 
            // lblServerType
            // 
            this.lblServerType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblServerType.Location = new System.Drawing.Point(16, 91);
            this.lblServerType.Name = "lblServerType";
            this.lblServerType.Size = new System.Drawing.Size(87, 22);
            this.lblServerType.TabIndex = 13;
            this.lblServerType.Text = "Server Type";
            // 
            // grbLogin
            // 
            this.grbLogin.Controls.Add(this.lblRole);
            this.grbLogin.Controls.Add(this.txtRole);
            this.grbLogin.Controls.Add(this.lblPassword);
            this.grbLogin.Controls.Add(this.txtPassword);
            this.grbLogin.Controls.Add(this.lblUser);
            this.grbLogin.Controls.Add(this.txtUserName);
            this.grbLogin.Location = new System.Drawing.Point(9, 96);
            this.grbLogin.Name = "grbLogin";
            this.grbLogin.Size = new System.Drawing.Size(215, 120);
            this.grbLogin.TabIndex = 10;
            this.grbLogin.TabStop = false;
            this.grbLogin.Text = "Login";
            // 
            // lblRole
            // 
            this.lblRole.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblRole.Location = new System.Drawing.Point(8, 86);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(48, 14);
            this.lblRole.TabIndex = 4;
            this.lblRole.Text = "Role";
            // 
            // txtRole
            // 
            this.txtRole.Location = new System.Drawing.Point(95, 88);
            this.txtRole.Name = "txtRole";
            this.txtRole.Size = new System.Drawing.Size(112, 22);
            this.txtRole.TabIndex = 5;
            // 
            // lblPassword
            // 
            this.lblPassword.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblPassword.Location = new System.Drawing.Point(8, 55);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(81, 17);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(95, 56);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(112, 22);
            this.txtPassword.TabIndex = 3;
            // 
            // lblUser
            // 
            this.lblUser.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblUser.Location = new System.Drawing.Point(8, 27);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(48, 14);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "User";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(95, 24);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(112, 22);
            this.txtUserName.TabIndex = 1;
            // 
            // cmdGetFile
            // 
            this.cmdGetFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdGetFile.Location = new System.Drawing.Point(478, 64);
            this.cmdGetFile.Name = "cmdGetFile";
            this.cmdGetFile.Size = new System.Drawing.Size(33, 23);
            this.cmdGetFile.TabIndex = 9;
            this.cmdGetFile.Text = "...";
            this.cmdGetFile.Click += new System.EventHandler(this.cmdGetFile_Click);
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(8, 64);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(464, 22);
            this.txtDatabase.TabIndex = 8;
            // 
            // lblDatabase
            // 
            this.lblDatabase.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDatabase.Location = new System.Drawing.Point(8, 48);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(106, 13);
            this.lblDatabase.TabIndex = 7;
            this.lblDatabase.Text = "Database";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(152, 24);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(76, 22);
            this.txtPort.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(152, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data Source Port";
            // 
            // lblDialect
            // 
            this.lblDialect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDialect.Location = new System.Drawing.Point(238, 8);
            this.lblDialect.Name = "lblDialect";
            this.lblDialect.Size = new System.Drawing.Size(48, 16);
            this.lblDialect.TabIndex = 3;
            this.lblDialect.Text = "Dialect";
            // 
            // lblCharset
            // 
            this.lblCharset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblCharset.Location = new System.Drawing.Point(320, 8);
            this.lblCharset.Name = "lblCharset";
            this.lblCharset.Size = new System.Drawing.Size(48, 16);
            this.lblCharset.TabIndex = 5;
            this.lblCharset.Text = "Charset";
            // 
            // cboCharset
            // 
            this.cboCharset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCharset.FormattingEnabled = true;
            this.cboCharset.Items.AddRange(new object[] {
            "NONE",
            "ASCII",
            "BIG_5Big5",
            "DOS437",
            "DOS850",
            "DOS860",
            "DOS861",
            "DOS863",
            "DOS865",
            "EUCJ_0208",
            "GB_2312",
            "ISO8859_1",
            "ISO8859_2",
            "KSC_5601",
            "ISO2022-JP",
            "SJIS_0208",
            "UNICODE_FSS",
            "WIN1250",
            "WIN1251",
            "WIN1252",
            "WIN1253",
            "WIN1254",
            "WIN1257"});
            this.cboCharset.Location = new System.Drawing.Point(320, 24);
            this.cboCharset.Name = "cboCharset";
            this.cboCharset.Size = new System.Drawing.Size(121, 24);
            this.cboCharset.TabIndex = 6;
            // 
            // lblLifeTime
            // 
            this.lblLifeTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblLifeTime.Location = new System.Drawing.Point(18, 26);
            this.lblLifeTime.Name = "lblLifeTime";
            this.lblLifeTime.Size = new System.Drawing.Size(48, 16);
            this.lblLifeTime.TabIndex = 14;
            this.lblLifeTime.Text = "Life Time";
            // 
            // FbConnectionStringEditor
            // 
            this.ClientSize = new System.Drawing.Size(536, 285);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdAccept);
            this.Controls.Add(this.cmdTest);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FbConnectionStringEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connection String Editor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grbSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtLifeTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeout)).EndInit();
            this.grbLogin.ResumeLayout(false);
            this.grbLogin.PerformLayout();
            this.ResumeLayout(false);

        }
		#endregion

        bool createDatabase = false;
        public bool CreateDatabase
        {
            get { return createDatabase; }
            set { createDatabase = value; }
        }


        private void cmdAccept_Click(object sender, System.EventArgs e)
		{
//            this.connectionStringBuilder.ConnectionLifeTime = (int) this.txtLifeTime.Value;
            this.connectionStringBuilder.PacketSize = int.Parse(cboPacketSize.Text);
            this.connectionStringBuilder.Dialect = int.Parse(this.cboDialect.Text);
//            this.connectionStringBuilder.Pooling = this.chkPooling.Checked;
//            this.connectionStringBuilder.Charset = this.cboDialect.Text;
            //if (this.cboServerType.Text == "Embedded Server")
            //    this.connectionStringBuilder.ServerType = FbServerType.Embedded;
            //else
            //    this.connectionStringBuilder.ServerType = FbServerType.Default;
    //try
            //{
            //    this.connection.ConnectionString = this.GetConnectionString();
            //}
            //catch
            //{
            //    MessageBox.Show("Invalid Connection String.");
            //}
		}

		private void cmdTest_Click(object sender, System.EventArgs e)
		{
            FbConnection connection = null;
			try
			{
                connection = new FbConnection(this.GetConnectionString());
                connection.Open();

				MessageBox.Show("Connection test successful.", "NDO Connection Dialog");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);	
			}
			finally
			{
                if (connection != null)
				    connection.Close();
			}
		}

		private void cmdGetFile_Click(object sender, System.EventArgs e)
		{
            // 
            // openFirebirdDB
            // 
		    OpenFileDialog openFirebirdDB = new System.Windows.Forms.OpenFileDialog();
            if (createDatabase)
                openFirebirdDB.Filter = "Firebird Database (*.FDB)|*.FDB";
            else
                openFirebirdDB.Filter = "Firebird Database (*.FDB;*.FBD)|*.FDB;*.FBD";
            openFirebirdDB.FileName = this.connectionStringBuilder.Database;

            if (createDatabase)
                openFirebirdDB.CheckFileExists = false;
            else
                openFirebirdDB.CheckFileExists = true;
            
            DialogResult result = openFirebirdDB.ShowDialog();

			if (result == DialogResult.OK)
			{
				this.txtDatabase.Text = openFirebirdDB.FileName;
                this.connectionStringBuilder.Database = openFirebirdDB.FileName;
			}
		}

		public string GetConnectionString()
		{
            return this.connectionStringBuilder.ConnectionString;
		}
	}
}

