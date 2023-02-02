using System;
using NDO.Mapping;
using NDOql.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace NDO.Query
{
	internal class RelationContextGenerator
	{
		NDOMapping mappings;
		List<string> names;
		HashSet<Relation> allRelations;
		List<Relation> relations;

		public RelationContextGenerator(IMappingsAccessor mappingsAccessor)
		{
			this.mappings = mappingsAccessor.Mappings;
		}

		/// <summary>
		/// Get the where clause of the SQL string
		/// </summary>
		/// <returns>Where clause string</returns>
		public QueryContexts GetContexts( Class resultClass, OqlExpression expressionTree )
		{
			var result = new QueryContexts();

			if (expressionTree == null)
				return result;

			this.names = (from oex in expressionTree.GetAll( e => e is IdentifierExpression ) select (string)oex.Value).ToList();
			this.allRelations = new HashSet<Relation>();

			if (names.Count == 0) 
				return result;

			foreach (string name in names)
			{
				CreateContextForName(resultClass, name, allRelations);
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

		private void BuildMutations(int start, QueryContexts queryContexts, Stack<Class> stack)
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

		/// <summary>
		/// Generates all relations for a given relation name
		/// </summary>
		/// <param name="resultClass"></param>
		/// <param name="name"></param>
		/// <param name="relationSet"></param>
		public void CreateContextForName(Class resultClass, string name, ICollection<Relation> relationSet)
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
					if (!relationSet.Contains( resultRelation ))  // relationSet might be a HashSet
						relationSet.Add( resultRelation );
				}
				
				// will be overwritten until we have the innermost table
				parentClass = relClass;
			}
		}
	}
}
