﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOql.Expressions;
using NDO.Mapping;
using System.Globalization;
using NDO.Query;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// Helper class to generate Column names of the select statement
	/// </summary>
	internal class WhereGenerator
	{
		Class cls;
		Dictionary<Relation, Class> queryContext;

		internal WhereGenerator(Class cls, Dictionary<Relation, Class> queryContext)
		{
			this.cls = cls;
			this.queryContext = queryContext;
		}

		internal string GenerateWhereClause(OqlExpression expressionTree)
		{
			if (expressionTree == null)
				return string.Empty;

			// The root expression tree might get exchanged.
			// To make this possible we make it the child of a dummy expression.
			OqlExpression newParent = new OqlExpression( 0, 0 );
			newParent.Add( expressionTree );
			((IManageExpression)expressionTree).SetParent( newParent );
			AnnotateExpressionTree( newParent.Children[0] );

			// We remove the dummy expression here.
			return "WHERE " + WhereString( newParent.Children[0] );
		}

		private void MoveParameterExpression(OqlExpression expressionTree, int fromOrdinal, int additionalSpace)
		{
			// Moves the ordinal numbers of ParameterExpressions above the current expression to leave parameters for 
			// the additional columns.
			foreach (ParameterExpression parExp in expressionTree.GetAll( e => 
				{
					ParameterExpression pe = e as ParameterExpression;
					if (pe == null)
						return false;
					return pe.Ordinal > fromOrdinal;
				} ))
			{
				parExp.Ordinal += additionalSpace;
			}
		}

		private void AnnotateExpressionTree( OqlExpression expressionTree )
		{
			foreach (IdentifierExpression exp in expressionTree.GetAll( e => e is IdentifierExpression ))
			{
				string[] arr = ((string)exp.Value).Split( '.' );
				string fieldName = arr[arr.Length - 1]; // In case of embedded or value types this will be overwritten
				Relation relation;
				Class parentClass = GetParentClass( exp, arr, out fieldName, out relation );

				if (fieldName == "oid")
				{
					string[] oidColumns = (from c in parentClass.Oid.OidColumns select QualifiedColumnName.Get( c )).ToArray();
					if (relation != null)
					{
						// In these cases we don't need the join to the table of the class owning the oid.
						// It's sufficient to compare against the foreign keys stored in the owner class' table
						// or in the mapping table
						if (relation.MappingTable != null)
						{
							oidColumns = (from c in relation.MappingTable.ChildForeignKeyColumns select QualifiedColumnName.Get( relation.MappingTable, c )).ToArray();
						}
						else if (relation.Multiplicity == RelationMultiplicity.Element)
						{
							oidColumns = (from c in relation.ForeignKeyColumns select QualifiedColumnName.Get( c )).ToArray();
						}
					}

					if (oidColumns.Length > 1 && exp.Children.Count == 0)
                    {
                        OqlExpression parent = exp.Parent;  // Must be a = expression like 'xxx.oid = {0}'.
                        ParameterExpression parExp = exp.Siblings[0] as ParameterExpression;
                        if (parExp == null)
                            throw new QueryException(10010, String.Format("Expression {0} resolves to multiple columns. It's sibling expression must be a ParameterExpression. But the sibling is {1}", exp.ToString(), parent.Children[1].ToString()));
                        // We need some additional parameters for the additional columns.
                        MoveParameterExpression(expressionTree, parExp.Ordinal, oidColumns.Length - 1);
                        // Replace the parent expression with a new AND expression
                        OqlExpression andExpression = new OqlExpression(0, 0);
                        ((IManageExpression)andExpression).SetParent(parent.Parent);
                        parent.Parent.Children.Remove(parent);
                        parent.Parent.Add(andExpression);
                        // We need to set Parent and Child explicitly.
                        // See comment in IManageExpression.SetParent.
                        // Reuse the original equality expression as first child of the AND expression
                        ((IManageExpression)parent).SetParent(andExpression);
                        andExpression.Add(parent);
                        exp.AdditionalInformation = oidColumns[0];
                        int currentOrdinal = parExp.Ordinal;
                        // Now add the additional children of the AND expression
                        for (int i = 1; i < oidColumns.Length; i++)
                        {
                            OqlExpression newParent = parent.DeepClone; // equality expression and it's both children
                            andExpression.Add(newParent, "AND");
                            ((IManageExpression)newParent).SetParent(andExpression);
                            // Now patch the Annotation and a new parameter to the children
                            IdentifierExpression newIdentExp = (IdentifierExpression)newParent.Children.Where(e => e is IdentifierExpression).First();
                            newIdentExp.AdditionalInformation = oidColumns[i];
                            ParameterExpression newParExp = (ParameterExpression)newParent.Children.Where(e => e is ParameterExpression).First();
                            newParExp.Ordinal = ++currentOrdinal;
                        }
                    }
                    else
                    {
                        int index = 0;
                        if (exp.Children.Count > 0 && exp.Children[0] is IndexExpression)
                            index = (int)exp.Children[0].Value;
                        if (index >= oidColumns.Length)
                            throw new IndexOutOfRangeException("oid index exceeds oid column count");
                        exp.AdditionalInformation = oidColumns[index];                                                
                    }
				}
				else
				{
					Field field = parentClass.FindField( fieldName );
					if (field == null)
						throw new Exception( "Can't find Field mapping for " + fieldName + " in " + exp.Value );
					exp.AdditionalInformation = QualifiedColumnName.Get( field.Column );
				}
			}
		}

		private Class GetParentClass( OqlExpression exp, string[] arr, out string fieldName, out Relation relation )
		{
			Class relClass = this.cls;
			NDOMapping mappings = relClass.Parent;
			int i;
			relation = null;

			for (i = 0; i < arr.Length - 1; i++)
			{
				relation = relClass.FindRelation( arr[i] );
				if (relation == null)  // must be a value type or embedded type
				{
					break;
				}

 				if (this.queryContext.ContainsKey(relation))
					relClass = this.queryContext[relation];
				else
					relClass = mappings.FindClass( relation.ReferencedType );
			}

			fieldName = String.Join( ".", arr, i, arr.Length - i );
			return relClass;
		}

        string WhereString(OqlExpression thisExpression)
        {
            StringBuilder sb = new StringBuilder();

            if (thisExpression.IsTerminating || thisExpression is IdentifierExpression)
            {
                if (thisExpression.UnaryOp != null)
                {
                    sb.Append(thisExpression.UnaryOp);
                    sb.Append(' ');
                }
				if (thisExpression.Value.GetType().IsPrimitive)
				{
					sb.Append( Convert.ToString( thisExpression.Value, CultureInfo.InvariantCulture ) );
				}
				else
				{
					IdentifierExpression iexp = thisExpression as IdentifierExpression;
					if (iexp != null)
					{
						sb.Append( iexp.AdditionalInformation );
					}
					else
					{
						sb.Append( thisExpression.Value.ToString() );
					}
				}
            }
            else
            {
				if (thisExpression.UnaryOp != null)
				{
					sb.Append( thisExpression.UnaryOp );
					sb.Append( ' ' );
				}
				if (thisExpression.HasBrackets)
                    sb.Append('(');
                string op1 = thisExpression.Operator;

                int childExpEndIndex = thisExpression.Children.Count - 1;
                for (int i = 0; i <= childExpEndIndex; i++)
                {
                    OqlExpression child = thisExpression.Children[i];
                    sb.Append(WhereString(child));
                    if (i < childExpEndIndex)
                    {
                        if (op1 != ",")
                            sb.Append(' ');
                        sb.Append(op1);
                        if (op1 == "BETWEEN")
                        {
                            op1 = "AND";
                        }
                        sb.Append(' ');
                    }
                }
                
                if (thisExpression.HasBrackets)
                    sb.Append(')');
            }
            return sb.ToString();
        }
	}
}
