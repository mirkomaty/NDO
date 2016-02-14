using NDO.Mapping;
using NDOql.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	internal class RelationContextGenerator
	{
		Class resultClass;
		OqlExpression expressionTree;
		NDOMapping mappings;
		List<string> names;
		HashSet<Relation> allRelations;
		List<Relation> relations;

		public RelationContextGenerator(Class resultClass, OqlExpression expressionTree, NDOMapping mappings)
		{
			this.resultClass = resultClass;
			this.expressionTree = expressionTree;
			this.mappings = mappings;
		}

		/// <summary>
		/// Get the where clause of the SQL string
		/// </summary>
		/// <returns>Where clause string</returns>
		public List<Dictionary<Relation,Class>> GetContexts()
		{
			List<Dictionary<Relation, Class>> result = new List<Dictionary<Relation, Class>>();

			if (expressionTree == null)
				return result;

			this.names = (from oex in expressionTree.GetAll( e => e is IdentifierExpression ) select (string)oex.Value).ToList();
			this.allRelations = new HashSet<Relation>();

			if (names.Count == 0) 
				return result;

			foreach (string name in names)
			{
				CreateContextForName(name);
			}
			if (allRelations.Count > 0)
			{
				this.relations = new List<Relation>();
				foreach(var de in allRelations)
					relations.Add(de);
				BuildMutations(0, result, new Stack<Class>());
			}
			return result;
		}

		private void BuildMutations(int start, List<Dictionary<Relation, Class>> queryContexts, Stack<Class> stack)
		{
			Relation r = (Relation) relations[start];
			HashSet<string> tables = new HashSet<string>();
			foreach(Class cl in r.ReferencedSubClasses)
			{
				if (cl.IsAbstract)
					continue;
				// If 2 classes are mapped to the same table we'd get the same expression twice.
				if (tables.Contains( cl.TableName ))
					continue;
				tables.Add( cl.TableName );
				stack.Push(cl);
				if (start == relations.Count - 1)
				{
					Dictionary<Relation,Class> dict = new Dictionary<Relation,Class>();
					int i = relations.Count - 1;
					foreach(Class cl2 in stack)
					{
						dict.Add(relations[i], cl2);
						i--;
					}
					queryContexts.Add(dict);
				}
				else
				{
					BuildMutations(start + 1, queryContexts, stack);
				}
				stack.Pop();				
			}
		}

		private void CreateContextForName(string name)
		{
			if (name.IndexOf(".") == -1)
				return;
			
			string[] namearr = name.Split(new char[]{'.'});
			Class parentClass = resultClass; // parentClass will be overwritten in each iteration

			Relation resultRelation = null;

			for (int i = 0; i < namearr.Length - 1; i++)
			{
				string fieldName = namearr[i];
				Relation rel = parentClass.FindRelation(fieldName);
				if (null == rel)
					break;
				resultRelation = rel;
				Class relClass = mappings.FindClass(rel.ReferencedTypeName);

				if (null == relClass)
					throw new NDOException(17, "Can't find mapping information for class " + rel.ReferencedTypeName);

				if (relClass.Subclasses.Count() > 0)
				{
					if (!allRelations.Contains( resultRelation ))
						allRelations.Add( resultRelation );
				}
				
				// will be overwritten until we have the innermost table
				parentClass = relClass;
			}
		}
	}
}
