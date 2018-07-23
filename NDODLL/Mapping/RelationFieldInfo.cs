using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Mapping
{
	internal class RelationFieldInfo
	{
		public Relation Rel;
		public string TableName;

		public RelationFieldInfo( Relation rel, string tableName )
		{
			Rel = rel;
			TableName = tableName;
		}
	}
}
