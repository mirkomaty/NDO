using System.Windows.Forms;
using NDO.UISupport;

namespace NDO.PostGresUISupport
{
    public class PostGresUIProvider : DbUISupportBase
    {
        public override string Name => "Postgre";

        public override NdoDialogResult ShowConnectionDialog( ref string connectionString )
        {
            var csef = new ConnectionStringEditorForm( connectionString );
            if (csef.ShowDialog() == DialogResult.Cancel)
                return NdoDialogResult.Cancel;
            connectionString = csef.ConnectionString;
            return NdoDialogResult.OK;
        }

        // For CreateDatabase we can use the standard implementation
    }
}
