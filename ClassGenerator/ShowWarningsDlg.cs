using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassGenerator
{
    public partial class ShowWarningsDlg : Form
    {
        public ShowWarningsDlg()
        {
            InitializeComponent();
        }

        public ShowWarningsDlg(string text) : this()
        {
            this.textBox1.Text = text;
        }
    }
}
