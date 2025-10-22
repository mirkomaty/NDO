using Microsoft.Data.SqlClient;
using NDOInterfaces;
using System.Data;

namespace NDO.SqlServer
{
	internal class NdoSqlConnection : NdoDbConnection
	{
		public NdoSqlConnection( IDbConnection innerConnection ) : base( innerConnection )
		{
		}

		public override object ConnectionId => ((SqlConnection)InnerConnection).ClientConnectionId;
	}
}
