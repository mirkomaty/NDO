using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Npgsql.Design
{
	public partial class ConnectionStringEditorForm : Form
	{
		string connectionString;

		public ConnectionStringEditorForm()
		{
			InitializeComponent();
		}

		public ConnectionStringEditorForm(string initialConnection)
		{
			this.connectionString = initialConnection;
			string[] parts = connectionString.Split( ';' );
			foreach ( string part in parts )
			{
				string[] sides = part.Split( '=' );
				switch ( sides[0].Trim() )
				{
					case "Server":
						if ( sides.Length > 1 )
							this.txtServer.Text = sides[1].Trim();
						break;
					case "Database":
						if ( sides.Length > 1 )
							this.txtDatabase.Text = sides[1].Trim();
						break;
					case "User":
						if ( sides.Length > 1 )
							this.txtUser.Text = sides[1].Trim();
						break;
					case "Password":
						if ( sides.Length > 1 )
							this.txtPassword.Text = sides[1].Trim();
						break;
					default:
						break;
				}
			}
			InitializeComponent();
		}

		private void btnOK_Click( object sender, EventArgs e )
		{
			this.connectionString = string.Empty;
			if ( this.txtServer.Text != string.Empty )
				this.connectionString += "Server=" + this.txtServer.Text + ";";
			if ( this.txtDatabase.Text != string.Empty )
				this.connectionString += "Database=" + this.txtDatabase.Text + ";";
			if ( this.txtUser.Text != string.Empty )
				this.connectionString += "User=" + this.txtUser.Text + ";";
			if ( this.txtPassword.Text != string.Empty )
				this.connectionString += "Password=" + this.txtPassword.Text + ";";
		}

		public string ConnectionString
		{
			get { return connectionString; }
		}

	}
}
