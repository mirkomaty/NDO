using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOql.Expressions;
using NDO.Mapping;

namespace NDO.Query.JoinExpressions
{
	class InnerJoinExpression : RawIdentifierExpression
	{
		internal InnerJoinExpression( Relation relation, Dictionary<Relation, Class> relationContext, bool fieldIsOid )
			: base( "INNER JOIN", 0, 0 )
		{
			this.Add( new ChildTableExpression( relation, relationContext, fieldIsOid ) );
		}

		internal InnerJoinExpression( MappingTable mappingTable, Dictionary<Relation, Class> relationContext )
			: base( "INNER JOIN", 0, 0 )
		{
			this.Add( new ChildTableExpression( mappingTable, relationContext ) );
		}
	}
}
