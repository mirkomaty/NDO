using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDO.UISupport;

namespace NDO.SqlCeUISupport
{
    public class SqlCeUIProvider : DbUISupportBase
    {
        public override string Name => "SqlCe";

        public override NdoDialogResult ShowConnectionDialog( ref string connectionString )
        {
            ConnectionDialog cd = new ConnectionDialog();
            var result = (NdoDialogResult)cd.ShowDialog();
            if (result == NdoDialogResult.Cancel)
                return result;

            connectionString = cd.Connection;
            return NdoDialogResult.OK;
        }

        public override NdoDialogResult ShowCreateDbDialog( ref object necessaryData )
        {
            string connectionString = string.Empty;
            var result = ShowConnectionDialog( ref connectionString );
            if (result == NdoDialogResult.OK)
            {
                necessaryData = connectionString;
            }

            return result;
        }

        public override string CreateDatabase( object necessaryData )
        {
            // DataSource="c:\Sqltest\hallo.sdf"; Password='lucky'
            return EnsureProvider().CreateDatabase( (string)necessaryData, (string)necessaryData );
        }
    }
}
