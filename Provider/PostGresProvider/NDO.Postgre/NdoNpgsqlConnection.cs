using NDOInterfaces;
using Npgsql;
using System.Data;

namespace NDO.Postgre
{
	internal class NdoNpgsqlConnection : NdoDbConnection
	{
		public NdoNpgsqlConnection( IDbConnection innerConnection ) : base( innerConnection )
		{
		}

		public override object ConnectionId => ((NpgsqlConnection)InnerConnection).ProcessID;
	}
}
