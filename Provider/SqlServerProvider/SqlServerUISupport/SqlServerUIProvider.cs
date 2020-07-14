using NDO.UISupport;

namespace SqlServerUISupport
{
	public class SqlServerUIProvider : DbUISupportBase
	{
		const string OleDbProviderString = "Provider=SQLOLEDB.1;";

		public override string Name => "SqlServer";

		public override NdoDialogResult ShowConnectionDialog( ref string connectionString )
		{
			string tempstr;
			if (connectionString == null || connectionString == string.Empty)
			{
				tempstr = OleDbProviderString;
			}
			else
			{
				if (!(connectionString.IndexOf( OleDbProviderString ) > -1))
					tempstr = OleDbProviderString + connectionString;
				else
					tempstr = connectionString;
			}

			OleDbConnectionDialog dlg = new OleDbConnectionDialog( tempstr );
			var result = (NdoDialogResult)dlg.Show();
			if (result != NdoDialogResult.Cancel)
				connectionString = dlg.ConnectionString.Replace( OleDbProviderString, string.Empty );

			return result;
		}
	}
}
