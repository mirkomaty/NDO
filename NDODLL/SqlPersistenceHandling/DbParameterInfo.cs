using System.Data;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// This Class describes a database parameter
	/// </summary>
	public class DbParameterInfo
	{
		/// <summary>
		/// Constructs a DbParameterInfo
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="dbType"></param>
		/// <param name="size"></param>
		/// <param name="isNullable"></param>
		/// <param name="precision"></param>
		/// <param name="scale"></param>
		/// <param name="dir"></param>
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

		/// <summary>
		/// The source column name 
		/// </summary>
		public string ColumnName;
		/// <summary>
		/// The type information
		/// </summary>
		public object DbType;
		/// <summary>
		/// The Size of the Parameter
		/// </summary>
		public int Size;
		/// <summary>
		/// The parameter direction (in our out)
		/// </summary>
		public ParameterDirection Direction;
		/// <summary>
		/// Determines, if the parameter is allowed to be null
		/// </summary>
		public bool IsNullable;
		/// <summary>
		/// The precision of the parameter, if it is a number
		/// </summary>
		public byte Precision;
		/// <summary>
		/// The scale of the parameter, if it is a number
		/// </summary>
		public byte Scale;
	}
}
