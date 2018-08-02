using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOql.Expressions;
using NDO.Mapping;
using NDO.Query.JoinExpressions;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// Helper class to generate Column names of the select statement
	/// </summary>
	internal class FromGenerator
	{
		Class cls;
		Dictionary<Relation, Class> relationContext;
		NDOMapping mappings;

		internal FromGenerator( Class cls, Dictionary<Relation, Class> relationContext )
		{
			this.cls = cls;
			this.mappings = cls.Parent;
			this.relationContext = relationContext;
		}

		internal string GenerateFromExpression(OqlExpression expressionTree)
		{
			StringBuilder sb = new StringBuilder();
			if (expressionTree != null)
			{
				AnnotateExpressionTree( expressionTree );
				List<IdentifierExpression> identifiers = expressionTree.GetAll( e => e is IdentifierExpression && !String.Empty.Equals( e.AdditionalInformation ) ).Select( e => (IdentifierExpression)e ).ToList();
				identifiers.Sort( ( i1, i2 ) => ((string)i1.Value).CompareTo( (string)i2.Value ) );
				bool isFirst = true;
				foreach (IdentifierExpression exp in identifiers)
				{
					if (!String.IsNullOrEmpty( (string)exp.AdditionalInformation ))
					{
						if (isFirst)
						{
							sb.Append( ' ' );
							isFirst = false;
						}
						sb.Append( exp.AdditionalInformation );
						sb.Append( ' ' );
					}
				}

				if (sb.Length > 0)
				{
					sb.Length--;
				}
			}
			return "FROM " + cls.GetQualifiedTableName() + sb.ToString();
		}

		private void AnnotateExpressionTree( OqlExpression expressionTree )
		{
			Dictionary<Relation, object> allJoins = new Dictionary<Relation, object>();
			foreach (IdentifierExpression exp in expressionTree.GetAll( e => e is IdentifierExpression ))
			{
				string fullName = (string)exp.Value;
				if (fullName.IndexOf( '.' ) < 0)
					continue;

				StringBuilder sb = new StringBuilder();
				string[] arr = ((string)exp.Value).Split( '.' );
				Class startClass = this.cls;
				bool isFirst = true;

				for (int i = 0; i < arr.Length - 1; i++)  // at least the last element is the field name
				{
					string relationName = arr[i];

					if (relationName == "oid")
						break;

					Relation relation = startClass.FindRelation( relationName );

					if (relation == null)
						break;

					if (allJoins.ContainsKey( relation ))
						continue;

					allJoins.Add( relation, null );

					Class childClass = this.relationContext.ContainsKey( relation ) 
						? this.relationContext[relation] 
						: this.mappings.FindClass( relation.ReferencedType );

					if (!isFirst)
						sb.Append( ' ' );

					// In the cases where the following condition doesn't apply, we don't need the join to the table of the class owning the oid.
					// It's sufficient to compare against the foreign keys stored in the owner class' table.
					if ((relation.Multiplicity == RelationMultiplicity.List || relation.MappingTable != null) || arr[i + 1] != "oid")
						sb.Append( new InnerJoinExpression( relation, this.relationContext, arr[i + 1] == "oid" ).ToString() );

					startClass = childClass;
					isFirst = false;
				}
				string join = sb.ToString();
				exp.AdditionalInformation = join;
			}
		}
	}
}
