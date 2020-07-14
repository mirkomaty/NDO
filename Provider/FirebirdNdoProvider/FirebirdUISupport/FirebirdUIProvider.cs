using FirebirdSql.Data.FirebirdClient;
using NDO.UISupport;
using System;

namespace FirebirdUISupport
{
    public class FirebirdUIProvider : DbUISupportBase
    {
        public override string Name => "Firebird";

        public override NdoDialogResult ShowCreateDbDialog(ref object necessaryData)
        {
            FbConnectionStringBuilder sb = necessaryData as FbConnectionStringBuilder;
            if (sb == null)
            {
                sb = new FbConnectionStringBuilder();
                sb.UserID = "SYSDBA";
                sb.Password = "masterkey";
                sb.DataSource = "localhost";
                sb.Database = string.Empty;
            }
            sb.Dialect = 3;
            ConnectionDialog dlg = new ConnectionDialog(sb);
            dlg.CreateDatabase = true;
            NdoDialogResult result = (NdoDialogResult)dlg.ShowDialog();
            if (result == NdoDialogResult.Cancel)
                return result;
            necessaryData = sb;
            return result;
        }

        public override string CreateDatabase(object necessaryData)
        {
            FbConnectionStringBuilder sb = necessaryData as FbConnectionStringBuilder;
            if (sb == null)
                throw new ArgumentException("FirebirdProvider: parameter type " + necessaryData.GetType().FullName + " is wrong.", "necessaryData");
            try
            {
                sb.Dialect = 3;
                FbConnection.CreateDatabase(sb.ConnectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while attempting to create a database: Exception Type: " + ex.GetType().Name + " Message: " + ex.Message + "\nConnection String: " + sb.ConnectionString);
            }
            return sb.ConnectionString;
        }

        public override NdoDialogResult ShowConnectionDialog(ref string connectionString)
        {
            string cs;
            if (connectionString != null)
                cs = connectionString;
            else
                cs = string.Empty;
            FbConnectionStringBuilder sb = new FbConnectionStringBuilder(cs);
            sb.Dialect = 3;
            if (cs == string.Empty)
            {
                sb.UserID = "SYSDBA";
                sb.Password = "masterkey";
                sb.DataSource = "localhost";
                sb.Database = string.Empty;
            }
            ConnectionDialog dlg = new ConnectionDialog(sb);
            if ((NdoDialogResult)dlg.ShowDialog() == NdoDialogResult.Cancel)
                return NdoDialogResult.Cancel;
            connectionString = sb.ConnectionString;
            return NdoDialogResult.OK;
        }
    }
}
