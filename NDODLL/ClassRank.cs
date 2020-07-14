//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NDO
{
	/// <summary>
	/// Helper Class to compute a sort order for updates and deletes.
	/// </summary>
	public class ClassRank
	{
		Dictionary<Type, int> updateOrder;
		private List<Type> updateList = new List<Type>();
		private List<Type> clientGeneratedList = new List<Type>();
		private List<Class> updateSearchedFor = new List<Class>();
		IEnumerable<Class> classes;
		Dictionary<string, Dictionary<string, int>> rankCache = new Dictionary<string, Dictionary<string, int>>();
		
		/// <summary>
		/// Builds the class sort order
		/// </summary>
		/// <param name="classes"></param>
		/// <returns></returns>
		public Dictionary<Type, int> BuildUpdateRank( IEnumerable<Class> classes )
		{
			this.classes = classes;
			this.updateOrder = new Dictionary<Type, int>();
			foreach (Class c in classes)
			{
				BuildUpdateDependency( c );
				/*				
				Console.WriteLine("Class {0} Subclasses:", c.FullName);
				foreach(Class s in c.Subclasses) 
				{
					Console.WriteLine("  - Class {0}", s.FullName);					
				}*/
			}

			//Console.WriteLine("Dependencies:");
			// The list will copied into a hashtable together with the
			// rank of the type in the list. The rank is reverted.
			// Lower rank values have more priority in the update order.
			int end = updateList.Count - 1;
			int i;
			for (i = 0; i <= end; i++)
			{
				updateOrder.Add( (Type) updateList[i], i );
			}

			int rank = i;

			// Now add the types using client generated oids. These don't need a 
			// high rank.
			int end2 = clientGeneratedList.Count - 1;
			for (i = 0; i <= end2; i++)
			{
				updateOrder.Add( (Type) clientGeneratedList[i], rank++ );
			}

			return this.updateOrder;
		}

		/// <summary>
		/// Builds update ranks.
		/// </summary>
		/// <param name="cls">the class that should be inserted in the update list</param>
		private void BuildUpdateDependency( Class cls )
		{
			//Console.WriteLine("Build dependency for " + c.FullName);
			if (cls.SystemType == null)
			{
				throw new NDOException( 11, "Type.GetType for the type name " + cls.FullName + " failed; check your mapping File." );
			}

			//Debug.WriteLine("BuildUpdateDependency " + c.FullName);

			// Avoid endless recursion
			if (updateSearchedFor.Contains( cls ))
				return;
			updateSearchedFor.Add( cls );
			// Can be stored at last, because the key is client generated
			if (!cls.Oid.HasAutoincrementedColumn)
			{
				clientGeneratedList.Add( cls.SystemType );
				//updateList.Add( c.SystemType );
				return;
			}

			// Collect all classes which are related to cls
			// First collect from forward relations
			List<Class> relatedClasses = new List<Class>();
			foreach (var r in cls.Relations)
			{
				foreach (var rsc in r.ReferencedSubClasses)
				{
					if (!relatedClasses.Contains(rsc) && rsc != cls)
						relatedClasses.Add( rsc );
				}
			}

			// Now collect from backward relations
			foreach (var refCls in (from c in this.classes where c.Relations.Any( r => r.ReferencedSubClasses.Any( rsc => rsc == cls ) ) select c))
			{
				if (!relatedClasses.Contains( refCls ) && refCls != cls)
					relatedClasses.Add( refCls );
			}
			
			foreach (var refCls in relatedClasses)
			{
				// Get the relative order from cls to refCls
				int order = GetOrder(cls, refCls);
				if (order > 0)  // refCls has a lower order value and must have precedence
					BuildUpdateDependency( refCls );
			}

			// After all related classes with precedence we add cls to the updateList
			if (!this.updateList.Contains( cls.SystemType ) && !clientGeneratedList.Contains( cls.SystemType ))
			{
				this.updateList.Add( cls.SystemType );
			}
			//Console.WriteLine("End build dependency for " + c.FullName);
		}

		private int GetOrder( Class cls1, Class cls2 )
		{
			int? cachedRank = GetCachedRank(cls1,cls2);

			if (cachedRank.HasValue)
				return cachedRank.Value;

			bool ai1 = cls1.Oid.HasAutoincrementedColumn;
			bool ai2 = cls2.Oid.HasAutoincrementedColumn;
			if (!ai1 && !ai2)
				return SetCachedRank( cls1, cls2, 0 );

			var forwardRelations = cls1.Relations.Where(r=>r.ReferencedSubClasses.Any(sc=>sc.FullName == cls2.FullName)).ToList();
			var backRelations = cls2.Relations.Where(r=>r.ReferencedSubClasses.Any(sc=>sc.FullName == cls1.FullName)).ToList();
			if (forwardRelations.Count == 0 && backRelations.Count == 0)
				return SetCachedRank( cls1, cls2, 0 ); // Order doesn't matter

			int order = 0;
			foreach (var rel in forwardRelations)
			{
				if (rel.MappingTable != null)
					continue;

				if (rel.Multiplicity == RelationMultiplicity.Element)
				{
					if (ai2) // Ordering only matters, if the oid of the related class (cls2) is autoincremented
					{
						// cls1 has the foreign key. So cls2 must be saved first.
						order++;
						// In case of a bidirectional relation the child of the composition should win,
						// because the parent must be saved in an own transaction, anyway
						if (rel.Composition && rel.Bidirectional && rel.ForeignRelation.Multiplicity == RelationMultiplicity.Element)
							order++;  // give way to the counterpart of the relation
					}
				}
				else
				{
					if (ai1)  // Ordering only matters, if the oid of cls1 is autoincremented
					{
						// cls2 has the foreign key, so cls1 has to be saved first.
						order--;
					}
				}
			}

			foreach (var rel in backRelations)
			{
				if (rel.MappingTable != null)
					continue;

				if (rel.Multiplicity == RelationMultiplicity.Element)
				{
					if (ai1)
					{
						// cls2 has the foreign key. So cls1 must be saved first. Note, that we reverse the order, so that the delete statements will be performed first.
						order--;
						if (rel.Composition && rel.Bidirectional && rel.ForeignRelation.Multiplicity == RelationMultiplicity.Element)
							order--;  // give way to the counterpart of the relation
					}
				}
				else
				{
					if (ai2)
					{
						// cls1 has the foreign key, so cls2 has to be saved first.
						order++;
					}
				}
			}

			return SetCachedRank( cls1, cls2, order );
		}

		int? GetCachedRank(Class cls1, Class cls2)
		{
			// If cls2.FullName < cls1.FullName the order must be reversed,
			// and factor must be -1
			int factor = cls2.FullName.CompareTo(cls1.FullName);
			string key1;
			string key2;

			if (factor >= 0)
			{
				key1 = cls1.FullName;
				key2 = cls2.FullName;
			}
			else
			{
				key1 = cls2.FullName;
				key2 = cls1.FullName;
			}

			Dictionary<string, int> dict;
			if (!rankCache.TryGetValue( key1, out dict ))
				return null;
			int result;
			if (!dict.TryGetValue( key2, out result ))
				return null;
			return result * factor;
		}

		int SetCachedRank( Class cls1, Class cls2, int value )
		{
			// If cls2.FullName < cls1.FullName the order must be reversed,
			// and factor must be -1
			int factor = cls2.FullName.CompareTo(cls1.FullName);
			string key1;
			string key2;
			if (factor >= 0)
			{
				key1 = cls1.FullName;
				key2 = cls2.FullName;
			}
			else
			{
				key1 = cls2.FullName;
				key2 = cls1.FullName;
			}
			Dictionary<string, int> dict;
			if (!rankCache.TryGetValue( key1, out dict ))
			{
				dict = new Dictionary<string, int>();
				rankCache.Add( key1, dict );
			}
			dict[key2] = value * factor;
			return value;
		}
	}
}
