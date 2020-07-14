using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NDO;
using NDO.UISupport;

namespace TestUIProviders
{
	public partial class Form1 : Form
	{
		string connectionString = String.Empty;
		public Form1()
		{
			InitializeComponent();
			this.comboBox1.Items.AddRange( NDOProviderFactory.Instance.ProviderNames );
			if (this.comboBox1.Items.Count > 0)
				this.comboBox1.SelectedIndex = 0;
		}

		private void button1_Click( object sender, EventArgs e )
		{
			NdoUIProviderFactory.Instance[this.comboBox1.Text].ShowConnectionDialog( ref this.connectionString );
			this.txtConnectionString.Text = this.connectionString;
		}

		private void button2_Click( object sender, EventArgs e )
		{
			var uiProvider =(DbUISupportBase) NdoUIProviderFactory.Instance[this.comboBox1.Text];
			var ndoProvider = NDOProviderFactory.Instance[this.comboBox1.Text];

			object necessaryData = null;
			var result = uiProvider.ShowCreateDbDialog(ref necessaryData);
			if (result == NdoDialogResult.OK)
			{
				this.txtConnectionString.Text = this.connectionString = necessaryData.ToString();
			}
		}

		private void btnCreate_Click( object sender, EventArgs e )
		{
			var uiProvider = (DbUISupportBase)NdoUIProviderFactory.Instance[this.comboBox1.Text];
			var ndoProvider = NDOProviderFactory.Instance[this.comboBox1.Text];

			object necessaryData = null;
			var result = uiProvider.ShowCreateDbDialog( ref necessaryData );
			if (result == NdoDialogResult.OK)
			{
				try
				{
					this.txtConnectionString.Text = this.connectionString = uiProvider.CreateDatabase( necessaryData );
				}
				catch (Exception ex)
				{
					MessageBox.Show( ex.Message );
					return;
				}

				MessageBox.Show( "OK" );
			}
		}
	}
}
