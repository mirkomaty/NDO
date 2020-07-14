using NDO.Mapping;
using NDOql.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query.JoinExpressions
{
	class OnExpression : RawIdentifierExpression
	{
		/// <summary>
		/// Creates the ON part of a JOIN expression.
		/// </summary>
		/// <param name="relation"></param>
		/// <param name="relationContext"></param>
		public OnExpression( Relation relation, Dictionary<Relation, Class> relationContext )
			: base( "ON", 0, 0 )
		{
			IEnumerable<OidColumn> oidColumns;
			IEnumerable<ForeignKeyColumn> fkColumns;

			Class childClass = relationContext.ContainsKey( relation )
				? relationContext[relation]
				: relation.Parent.Parent.FindClass( relation.ReferencedType );

			KeyValuePair<string,int>? typeCodeInfo = null;

			Class ownClass = (relation.Multiplicity == RelationMultiplicity.List ? relation.Parent : childClass);
			Class otherClass = (relation.Multiplicity == RelationMultiplicity.List ? childClass : relation.Parent);

			if (relation.ForeignKeyTypeColumnName != null)
			{
				typeCodeInfo = new KeyValuePair<string, int>( relation.ForeignKeyTypeColumnName, otherClass.TypeCode );
			}

			oidColumns = ownClass.Oid.OidColumns;
			fkColumns = relation.ForeignKeyColumns;

			Initialize( otherClass, oidColumns, fkColumns, typeCodeInfo );
		}

		/// <summary>
		/// Creates an ON expression of a JOIN expression. With a mapping table we need two JOIN expressions:
		/// 1. Between the parent table and the mapping table
		/// 2. Between the mapping table and the child table
		/// </summary>
		/// <param name="mappingTable"></param>
		/// <param name="relationContext"></param>
		/// <param name="isFirstExpression">Determines, if this is the first or second of the two necessary JOIN expressions.</param>
		public OnExpression( MappingTable mappingTable, Dictionary<Relation, Class> relationContext, bool isFirstExpression )
			: base( "ON", 0, 0 )
		{
			Relation relation = mappingTable.Parent;

			IEnumerable<OidColumn> oidColumns;
			IEnumerable<ForeignKeyColumn> fkColumns;

			Class childClass;
			KeyValuePair<string,int>? typeCodeInfo = null;
			if (relationContext.ContainsKey( relation ))
			{
				childClass = relationContext[relation];
			}
			else
			{
				childClass = relation.Parent.Parent.FindClass( relation.ReferencedType );
			}

			if (isFirstExpression)
			{
				oidColumns = relation.Parent.Oid.OidColumns;
				fkColumns = relation.ForeignKeyColumns;
				if ( relation.ForeignKeyTypeColumnName != null )
				{
					typeCodeInfo = new KeyValuePair<string, int>( relation.ForeignKeyTypeColumnName, relation.Parent.TypeCode );
				}
			}
			else
			{
				oidColumns = childClass.Oid.OidColumns;
				fkColumns = mappingTable.ChildForeignKeyColumns;
				if ( mappingTable.ChildForeignKeyTypeColumnName != null )
				{
					typeCodeInfo = new KeyValuePair<string, int>( mappingTable.ChildForeignKeyTypeColumnName, childClass.TypeCode );
				}
			}
			Initialize( mappingTable, oidColumns, fkColumns, typeCodeInfo );
		}

		void Initialize( MappingNode mappingNode, IEnumerable<OidColumn> oidColumns, IEnumerable<ForeignKeyColumn> fkColumns, KeyValuePair<string,int>? typeCodeInfo )
		{
			IEnumerator<OidColumn> oidEnumerator = oidColumns.GetEnumerator();
			oidEnumerator.MoveNext();
			List<OqlExpression> compareExpressions = new List<OqlExpression>();

			foreach (ForeignKeyColumn fkColumn in fkColumns)
			{
				OidColumn oidColumn = oidEnumerator.Current;
				compareExpressions.Add( new OnCompareExpression( mappingNode, oidColumn, fkColumn ) );
				oidEnumerator.MoveNext();
			}

			if (typeCodeInfo.HasValue)
			{
				compareExpressions.Add( new OnCompareExpression( mappingNode, typeCodeInfo.Value.Key, typeCodeInfo.Value.Value ) );
			}

			if (compareExpressions.Count == 1)
			{
				Add( compareExpressions[0] );
			}
			else
			{
				OqlExpression andExpression = new OqlExpression( 0, 0 );
				int i = 0;
				foreach (var compareExpression in compareExpressions)
				{
					if (i++ == 0)
						andExpression.Add( compareExpression );
					else
						andExpression.Add( compareExpression, "AND" );
				}

				Add( andExpression );
			}
		}
	}
}
