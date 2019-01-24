using System.IO;
using System.Windows.Forms;
using NDO.UISupport;

namespace NDO.SqliteUISupport
{
    public class SqliteUIProvider : DbUISupportBase
    {
		public override string Name => "Sqlite";

		public override NdoDialogResult ShowConnectionDialog( ref string connectionString )
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (!string.IsNullOrEmpty( connectionString ))
			{
				try
				{
					string fileName = connectionString.Substring( connectionString.IndexOf( '=' ) + 1 );
					ofd.InitialDirectory = Path.GetDirectoryName( connectionString );
				}
				catch { }
			}
			ofd.Filter = "SQLite Database File (*.db)|*.db";
			ofd.DefaultExt = ("db");
			ofd.CheckFileExists = false;
			DialogResult result = DialogResult.OK;
			if ((result = ofd.ShowDialog()) == DialogResult.OK)
			{
				connectionString = "Data Source=" + ofd.FileName;
			}
			return (NdoDialogResult)result;
		}

		public override NdoDialogResult ShowCreateDbDialog( ref object necessaryData )
		{
			string connectionString = string.Empty;
			NdoDialogResult result = ShowConnectionDialog( ref connectionString );
			if (result == NdoDialogResult.OK)
			{
				necessaryData = connectionString;
			}

			return result;
		}

		public override string CreateDatabase( object necessaryData )
		{
			string connectionString = (string)necessaryData;
			string path = connectionString.Substring( connectionString.IndexOf( '=' ) + 1 ).Trim();

			return Provider.CreateDatabase( path, connectionString );
		}
	}
}
