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
using System.Text;
using System.Collections.Specialized;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NDO.OracleProvider
{
	/// <summary>
	/// Zusammenfassung für ConnectionDialog.
	/// </summary>
	internal class ConnectionDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtDatabaseName;
		private System.Windows.Forms.TextBox txtHost;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.TextBox txtPassword;
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		NameValueCollection values = new NameValueCollection();

		bool createDatabase;

		public ConnectionDialog(string connectionString, bool createDatabase)
		{
			//
			// Erforderlich für die Windows Form-Designerunterstützung
			//
			InitializeComponent();

			this.createDatabase = createDatabase;

			if (connectionString != null)
			{
				string[] arr = connectionString.Split(';');
				foreach(string s in arr)
				{
					if (s.Trim() == string.Empty)
						continue;
					string[] arr2 = s.Split('=');
					if (arr2[1].Trim().Length > 0)
						values.Add(arr2[0].Trim(), arr2[1].Trim());
					object o;
					if ((o = values["Database"]) != null)
						this.txtDatabaseName.Text = (string) o;
					if ((o = values["Password"]) != null)
						this.txtPassword.Text = (string) o;
					if ((o = values["Data Source"]) != null)
						this.txtHost.Text = (string) o;
					if ((o = values["User Id"]) != null)
						this.txtUserName.Text = (string) o;
				}
			}
		}

		public string Database
		{
			get { return this.txtDatabaseName.Text; }
		}

		public string ConnectionString
		{
			get 
			{ 
				StringBuilder sb = new StringBuilder();
				if (!this.createDatabase && this.txtDatabaseName.Text.Trim() != string.Empty)
				{
					sb.Append("Database=");
					sb.Append(this.txtDatabaseName.Text);
					sb.Append(';');
				}
				if (this.txtHost.Text.Trim() != string.Empty)
				{
					sb.Append("Data Source=");
					sb.Append(this.txtHost.Text);
					sb.Append(';');
				}
				if (this.txtUserName.Text.Trim() != string.Empty)
				{
					sb.Append("User Id=");
					sb.Append(this.txtUserName.Text);
					sb.Append(';');
				}
				if (this.txtPassword.Text.Trim() != string.Empty)
				{
					sb.Append("Password=");
					sb.Append(this.txtPassword.Text);
					sb.Append(';');
				}
				return sb.ToString();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConnectionDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtDatabaseName = new System.Windows.Forms.TextBox();
			this.txtHost = new System.Windows.Forms.TextBox();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(136, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Database Name";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "Host";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 24);
			this.label3.TabIndex = 2;
			this.label3.Text = "User Name";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(136, 24);
			this.label4.TabIndex = 3;
			this.label4.Text = "Password";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(312, 152);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(88, 32);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(208, 152);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(88, 32);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			// 
			// txtDatabaseName
			// 
			this.txtDatabaseName.Location = new System.Drawing.Point(144, 16);
			this.txtDatabaseName.Name = "txtDatabaseName";
			this.txtDatabaseName.Size = new System.Drawing.Size(256, 22);
			this.txtDatabaseName.TabIndex = 6;
			this.txtDatabaseName.Text = "";
			// 
			// txtHost
			// 
			this.txtHost.Location = new System.Drawing.Point(144, 48);
			this.txtHost.Name = "txtHost";
			this.txtHost.Size = new System.Drawing.Size(256, 22);
			this.txtHost.TabIndex = 7;
			this.txtHost.Text = "localhost";
			// 
			// txtUserName
			// 
			this.txtUserName.Location = new System.Drawing.Point(144, 80);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size(256, 22);
			this.txtUserName.TabIndex = 8;
			this.txtUserName.Text = "root";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(144, 112);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Size = new System.Drawing.Size(256, 22);
			this.txtPassword.TabIndex = 9;
			this.txtPassword.Text = "";
			// 
			// ConnectionDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(416, 192);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.txtUserName);
			this.Controls.Add(this.txtHost);
			this.Controls.Add(this.txtDatabaseName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConnectionDialog";
			this.Text = "Oracle Connection Dialog";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
