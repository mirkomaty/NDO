using System.Data;

namespace NDO.SqlPersistenceHandling
{
	internal class DbParameterInfo
	{
		public DbParameterInfo(string columnName, object dbType, int size, bool isNullable, byte precision = 0, byte scale = 0, ParameterDirection dir = ParameterDirection.Input )
		{
			ColumnName = columnName;
			DbType = dbType;
			Size = size;
			Direction = dir;
			IsNullable = isNullable;
			Precision = precision;
			Scale = scale;
		}

		public string ColumnName;
		public object DbType;
		public int Size;
		public ParameterDirection Direction;
		public bool IsNullable;
		public byte Precision;
		public byte Scale;
	}
}
