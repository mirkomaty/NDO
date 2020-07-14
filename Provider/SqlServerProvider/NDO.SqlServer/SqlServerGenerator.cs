using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlServerProvider
{
	public class SqlServerGenerator : AbstractSQLGenerator
	{
		public SqlServerGenerator()
		{
		}

		public override string ProviderName
		{
			get { return "SqlServer"; }
		}

		public override string DropTable( string tableName )
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( "if exists (select * from sysobjects where id = object_id(N'" );
			sb.Append( tableName );
			sb.Append( "') and OBJECTPROPERTY(id, N'IsUserTable') = 1)\n" );
			sb.Append( "DROP TABLE " );
			sb.Append( tableName );
			sb.Append( ";\n" );
			return sb.ToString();
		}

		public override bool LengthAllowed( Type t )
		{
			return t == typeof( string ) || t == typeof( decimal );
		}

		public override bool LengthAllowed( string dbType )
		{
			return string.Compare( dbType, "nvarchar", true ) == 0 || string.Compare( dbType, "decimal", true ) == 0;
		}

		public override string DbTypeFromType( Type t, int size )
		{
			if (t == typeof( DateTime ))
				return "DateTime";
			return ((SqlDbType)Provider.GetDbType( t )).ToString();
		}

		public override string AutoIncrementColumn( string columnName, Type dataType, string columnType, string width )
		{
			return columnName + " " + columnType + " " + width + " IDENTITY (1, 1) ";
		}


		public override bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
		}


		public override string AlterColumnType()
		{
			return "ALTER COLUMN";
		}
	}
}
