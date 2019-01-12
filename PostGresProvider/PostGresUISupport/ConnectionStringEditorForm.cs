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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NDO.PostGresUISupport
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
            Close();
		}

		public string ConnectionString
		{
			get { return connectionString; }
		}

	}
}
