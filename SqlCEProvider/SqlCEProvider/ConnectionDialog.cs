using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NDO.SqlCeProvider
{
    public partial class ConnectionDialog : Form
    {
        public ConnectionDialog()
        {
            InitializeComponent();
        }

        public string Connection
        {
            get
            {
                return String.Format("DataSource=\"{0}\"; Password='{1}'", this.txtDatabaseFile.Text, this.txtPassword.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (!string.IsNullOrEmpty(this.txtDatabaseFile.Text))
            {
                try
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(this.txtDatabaseFile.Text);
                }
                catch { }
            }
            ofd.Filter = "SQL CE Database File (*.sdf)|*.sdf";
            ofd.DefaultExt = ("sdf");
            ofd.CheckFileExists = false;

            if ((ofd.ShowDialog()) == DialogResult.OK)
            {
                this.txtDatabaseFile.Text = ofd.FileName;
            }            
        }
    }
}
