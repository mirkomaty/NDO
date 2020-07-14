using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NDO.UISupport;

namespace NDO.OracleUISupport
{
    public class OracleUIProvider : DbUISupportBase
    {
        public override string Name => "Oracle";

        const string OraString = "Provider=MSDAORA.1;";
        public override NdoDialogResult ShowConnectionDialog( ref string connectionString )
        {
            string tempstr;
            if (connectionString == null || connectionString == string.Empty)
                tempstr = OraString;
            else
            {
                if (!(connectionString.IndexOf( OraString ) > -1))
                    tempstr = OraString + connectionString;
                else
                    tempstr = connectionString;
            }
            OleDbConnectionDialog dlg = new OleDbConnectionDialog( tempstr );
            NdoDialogResult result = (NdoDialogResult)dlg.Show();
            if (result != NdoDialogResult.Cancel)
            {
                connectionString = dlg.ConnectionString.Replace( OraString, string.Empty );
            }
            return result;
        }


        /// <summary>
        /// See <see cref="IProvider"> IProvider interface </see>
        /// </summary>
        /// <remarks>Note, that Oracle doesn't allow to programmatically create a database.</remarks>
        public override string CreateDatabase( object necessaryData )
        {
            return string.Empty;
        }

        /// <summary>
        /// See <see cref="IProvider"> IProvider interface </see>
        /// </summary>
        public override NdoDialogResult ShowCreateDbDialog( ref object necessaryData )
        {
            MessageBox.Show( "Oracle doesn't allow to programmatically create a database.\nPlease use the Enterprise Manager Console.\nSee also: https://docs.oracle.com/cd/B28359_01/server.111/b28310/create003.htm#ADMIN11073" );
            return NdoDialogResult.Cancel;
        }

    }
}
