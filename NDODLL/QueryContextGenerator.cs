//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Diagnostics;
using System.Collections;
using NDO.Mapping;
using NDOInterfaces;

namespace NDO
{

	internal class QueryContextGenerator
	{
		NDO.Mapping.Class resultClass;
		IList names;
		Mappings mappings;
		ArrayList queryContexts;
		Hashtable allRelations;
		ArrayList relations;


		public QueryContextGenerator(Class resultClass, IList names, Mappings mappings)
		{
			this.resultClass = resultClass;
			this.names = names;
			this.mappings = mappings;
			this.queryContexts = new ArrayList();
			this.allRelations = new Hashtable(names.Count);
		}
		/// <summary>
		/// Get the where clause of the SQL string
		/// </summary>
		/// <returns>Where clause string</returns>
		public ArrayList GetContexts()
		{
			if (names.Count == 0) 
				return queryContexts;

			foreach (string name in names)
			{
				CreateContextForName(name);
			}
			if (allRelations.Count > 0)
			{
				relations = new ArrayList();
				foreach(DictionaryEntry de in allRelations)
					relations.Add(de.Key);

				BuildMutations(0, queryContexts, new Stack());
			}
			return queryContexts;
		}

		private void BuildMutations(int start, IList queryContexts, Stack stack)
		{
			Relation r = (Relation) relations[start];
			foreach(Class cl in r.ReferencedSubClasses)
			{
				if (cl.IsAbstract)
					continue;
				stack.Push(cl);
				if (start == relations.Count - 1)
				{
					Hashtable ht = new Hashtable();
					int i = relations.Count - 1;
					foreach(Class cl2 in stack)
					{
						ht.Add(relations[i], cl2);
						i--;
					}
					queryContexts.Add(ht);
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

				if (relClass.Subclasses.Count > 0)
				{
					if (!allRelations.Contains(resultRelation))
						allRelations.Add(resultRelation, null);
				}
				
				// wird so oft überschrieben, bis wir wirklich beim innersten Table sind
				parentClass = relClass;
			}
		}
	}
}