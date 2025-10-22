using MySqlConnector;
using NDOInterfaces;
using System.Data;

namespace NDO.MySqlConnector
{
	internal class NdoMySqlConnectorConnection : NdoDbConnection
	{
		public NdoMySqlConnectorConnection( IDbConnection innerConnection ) : base( innerConnection )
		{
		}

		public override object ConnectionId => ((MySqlConnection)InnerConnection).ServerThread;
	}
}
