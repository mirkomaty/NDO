using NDO.Mapping;
using NDOql.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query.JoinExpressions
{
	class OnCompareExpression : RawIdentifierExpression
	{
		public OnCompareExpression(MappingNode mappingNode, OidColumn oidColumn, ForeignKeyColumn fkColumn)
			: base( null, 0, 0 )
		{
			Class fkParentClass = mappingNode as Class;
			if (fkParentClass != null)
			{
				Add( new RawIdentifierExpression( QualifiedColumnName.Get( oidColumn ), 0, 0 ) );
				Add( new RawIdentifierExpression( QualifiedColumnName.Get( fkParentClass, fkColumn ), 0, 0 ), "=" );
				return;
			}
			MappingTable mappingTable = mappingNode as MappingTable;
			if (mappingTable != null)
			{
				Add( new RawIdentifierExpression( QualifiedColumnName.Get( oidColumn ), 0, 0 ) );
				Add( new RawIdentifierExpression( QualifiedColumnName.Get( mappingTable, fkColumn ), 0, 0 ), "=" );
				return;
			}
			else
				throw new ArgumentException( "OnCompareExpression: Unexpected mapping node type", "mappingNode" );
		}

		/// <summary>
		/// Generate compares for type codes
		/// </summary>
		/// <param name="mappingNode"></param>
		/// <param name="columnName"></param>
		/// <param name="value"></param>
		public OnCompareExpression(MappingNode mappingNode, string columnName, int value)
			: base( null, 0, 0 )
		{
			Class fkParentClass = mappingNode as Class;
			if (fkParentClass != null)
			{
				Add( new RawIdentifierExpression( QualifiedColumnName.Get( fkParentClass, columnName ), 0, 0 ));
				Add( new NumberExpression( value, 0, 0 ), "=" );
				return;
			}
			MappingTable mappingTable = mappingNode as MappingTable;
			if (mappingTable != null)
			{
				Add( new RawIdentifierExpression( QualifiedColumnName.Get( mappingTable, columnName ), 0, 0 ));
				Add( new NumberExpression( value, 0, 0 ), "=" );
				return;
			}
			else
				throw new ArgumentException( "OnCompareExpression: Unexpected mapping node type", "mappingNode" );
		}
	}
}
