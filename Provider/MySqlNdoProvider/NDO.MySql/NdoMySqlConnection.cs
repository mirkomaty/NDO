using MySql.Data.MySqlClient;
using NDOInterfaces;
using System.Data;

namespace NDO.MySql
{
	internal class NdoMySqlConnection : NdoDbConnection
	{
		public NdoMySqlConnection( IDbConnection innerConnection ) : base( innerConnection )
		{
		}

		public override object ConnectionId => ( (MySqlConnection) InnerConnection ).ServerThread;
	}
}
