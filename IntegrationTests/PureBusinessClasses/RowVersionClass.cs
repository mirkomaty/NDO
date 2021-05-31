using System;
using System.Linq;
using System.Collections.Generic;
using NDO;
using NDO.Mapping.Attributes;
using NDO.Linq;

namespace TimeStampTestClasses
{
	/// <summary>
	/// Summary for RowVersionClass
	/// </summary>
	[NDOPersistent]
	public partial class RowVersionClass
	{
		[Column( DbType = "rowversion" )]
		[NDOReadOnly]
		byte[] rowVersion = null;
		string myData;

		public RowVersionClass()
		{
		}

		public ulong RowVersion => this.rowVersion == null ? 0L : BitConverter.ToUInt64( this.rowVersion.Reverse().ToArray(), 0 );		

		class SqlServerFunctions
		{
			[ServerFunction( "MIN_ACTIVE_ROWVERSION" )]
			public static byte[] MinActiveRowversion()
			{
				return null;
			}
		}

		public static IEnumerable<RowVersionClass> GetNewerThan( PersistenceManager pm, ulong version )
		{
			byte[] versionBytes = BitConverter.GetBytes( version ).Reverse().ToArray();

			return pm.Objects<RowVersionClass>().Where(
				u => u.rowVersion.GreaterThan( versionBytes )
				&& u.rowVersion.LowerThan( SqlServerFunctions.MinActiveRowversion() )
			).ResultTable;
		}

		public string MyData
		{
			get { return this.myData; }
			set { this.myData = value; }
		}
	}
}
