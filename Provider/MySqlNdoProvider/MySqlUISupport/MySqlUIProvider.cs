using NDO.UISupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MySqlUISupport
{
    public class MySqlUIProvider : DbUISupportBase
	{
		public override string Name => "MySql";

		public override NdoDialogResult ShowCreateDbDialog( ref object necessaryData )
		{
			NDOCreateDbParameter par;
			if (necessaryData == null)
				par = new NDOCreateDbParameter( string.Empty, "Data Source=localhost;User Id=root;" );
			else
				par = necessaryData as NDOCreateDbParameter;
			if (par == null)
				throw new ArgumentException( "MySql provider: parameter type " + necessaryData.GetType().FullName + " is wrong.", "necessaryData" );
			if (par.ConnectionString == null || par.ConnectionString == string.Empty)
				par.ConnectionString = "Data Source=localhost;User Id=root";
			ConnectionDialog dlg = new ConnectionDialog( par.ConnectionString, true );
			if (dlg.ShowDialog() == DialogResult.Cancel)
				return NdoDialogResult.Cancel;
			par.ConnectionString = dlg.ConnectionString;
			par.DatabaseName = dlg.Database;
			necessaryData = par;
			return NdoDialogResult.OK;
		}

		public override NdoDialogResult ShowConnectionDialog( ref string connectionString )
		{
			ConnectionDialog dlg = new ConnectionDialog( connectionString, false );
			if (dlg.ShowDialog() == DialogResult.Cancel)
				return NdoDialogResult.Cancel;
			connectionString = dlg.ConnectionString;
			return NdoDialogResult.OK;
		}

		public override string CreateDatabase( object necessaryData )
		{
			// Don't need to check, if type is OK, since that happens in CreateDatabase
			NDOCreateDbParameter par = necessaryData as NDOCreateDbParameter;

			return Provider.CreateDatabase( par.DatabaseName, par.ConnectionString );
		}
	}
}
