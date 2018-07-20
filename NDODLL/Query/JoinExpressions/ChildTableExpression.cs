using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOql.Expressions;
using NDO.Mapping;


namespace NDO.Query.JoinExpressions
{
	class ChildTableExpression : RawIdentifierExpression
	{
		internal ChildTableExpression( Relation relation, Dictionary<Relation, Class> relationContext )
			: base( null, 0, 0 )
		{
			Class childClass = relationContext.ContainsKey(relation) 
				? relationContext[relation]
				: relation.Parent.Parent.FindClass( relation.ReferencedType );

			if (relation.MappingTable == null)
			{
				Value = childClass.GetQualifiedTableName();

				Add( new OnExpression( relation, relationContext ) );
			}
			else
			{
				Value = relation.MappingTable.GetQualifiedTableName();
				Add( new OnExpression( relation.MappingTable, relationContext, true ) );
				Add( new InnerJoinExpression( relation.MappingTable, relationContext ) );
			}			
		}

		/// <summary>
		/// This constructs the second INNER JOIN from the mapping table to the child class table.
		/// </summary>
		/// <param name="mappingTable"></param>
		internal ChildTableExpression( MappingTable mappingTable, Dictionary<Relation, Class> relationContext )
			: base( null, 0, 0 )
		{
			Relation relation = mappingTable.Parent;

			Class childClass = relationContext.ContainsKey(relation) 
				? relationContext[relation]
				: relation.Parent.Parent.FindClass( relation.ReferencedType );
			
			Value = childClass.GetQualifiedTableName();
			Add( new OnExpression( relation.MappingTable, relationContext, false ) );
		}
	}
}
