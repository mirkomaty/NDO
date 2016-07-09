using NDO.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO
{
	public class ClassRank
	{
		Dictionary<Type, int> updateOrder;
		private List<Type> updateList = new List<Type>();
		private List<Type> clientGeneratedList = new List<Type>();
		private List<Class> updateSearchedFor = new List<Class>();
		IEnumerable<Class> classes;

		public Dictionary<Type, int> BuildUpdateRankOld( IEnumerable<Class> classes )
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
				updateOrder.Add( (Type) updateList[i], end - i );
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
		/// <param name="c">the class that should be inserted in the update list</param>
		private void BuildUpdateDependency( Class c )
		{
			//Console.WriteLine("Build dependency for " + c.FullName);
			if (c.SystemType == null)
			{
				throw new NDOException( 11, "Type.GetType for the type name " + c.FullName + " failed; check your mapping File." );
			}

			//Debug.WriteLine("BuildUpdateDependency " + c.FullName);

			// Avoid endless recursion
			if (updateSearchedFor.Contains( c ))
				return;
			updateSearchedFor.Add( c );
			// Can be stored at last, because the key is client generated
			if (!c.Oid.HasAutoincrementedColumn)
			{
				clientGeneratedList.Add( c.SystemType );
				//updateList.Add( c.SystemType );
				return;
			}
			int minIndex = int.MinValue;
			foreach (Relation r in c.Relations)
			{
				if (r.MappingTable != null)  // Foreign Keys will be managed by NDO
					continue;
				if (r.Multiplicity == RelationMultiplicity.Element)
				{
					// Here we have two keys in each table. There is some probability, that the owner in a composite is yet saved.
					if (!(r.Bidirectional && r.ForeignRelation.Multiplicity == RelationMultiplicity.Element && r.Composition))
						continue;
				}
				if (c.FullName == r.ReferencedTypeName)
					continue;
				foreach (Class sc in r.ReferencedSubClasses)
				{
					// The foreign key is in the table of the referenced class.
					// So the type has to be placed first in the update list.
					// This will result in a higher rank value, which means, that 
					// the type will be updated later than the current type.
					BuildUpdateDependency( sc );
					// This is used for sanity check. The current type cannot be
					// placed above the referenced type. This may happen in certain scenarios 
					// of circular references.
					minIndex = Math.Max( minIndex, updateList.IndexOf( sc.SystemType ) );
				}
			}

			foreach (Class c2 in classes)
			{
				if (c2.FullName == c.FullName)
					continue;

				foreach (Relation r in c2.Relations)
				{
					// Suche alle Relationen, die sich auf unsere Klasse beziehen
					bool found = false;
					foreach (Class cl in r.ReferencedSubClasses)
					{
						if (cl.FullName == c.FullName)
						{
							found = true;
							break;
						}
					}
					if (!found)
						continue;
					if (r.Multiplicity != RelationMultiplicity.Element)
						continue;
					// c2 has an element relation to c. This means, that c2 holds the foreign key
					// and thus must appear first in the updateList.
					// This will result in a higher rank value, which means, that 
					// the type will be updated later than the current type.
					BuildUpdateDependency( c2 );
					// This is used for sanity check. The current type cannot be
					// placed above the referenced type. This may happen in certain scenarios 
					// of circular references.
					minIndex = Math.Max( minIndex, updateList.IndexOf( c2.SystemType ) );
					break;
				}
			}

			int ix = updateList.IndexOf( c.SystemType );
			if (ix >= 0 && ix < minIndex)
			{
				throw new NDOException( 12, "Cannot construct update dependencies for class " + c.SystemType + ". Try to use client generated primary keys for this type or for the related types." );
			}
			else if (!updateList.Contains( c.SystemType ) && !clientGeneratedList.Contains( c.SystemType ))
			{
				updateList.Add( c.SystemType );
			}
			//Console.WriteLine("End build dependency for " + c.FullName);
		}

		public Dictionary<Type, int> BuildUpdateRank( IEnumerable<Class> inputClasses )
		{
			List<Class> classes = inputClasses.ToList();
			classes.Sort( ( cls1, cls2 ) => 
			{
				bool ai1 = cls1.Oid.HasAutoincrementedColumn;
				bool ai2 = cls2.Oid.HasAutoincrementedColumn;
				if (!ai1 && !ai2)
					return 0;

				var forwardRelations = cls1.Relations.Where(r=>r.ReferencedSubClasses.Any(r2=>r2.FullName == cls2.FullName)).ToList();
				var backRelations = cls2.Relations.Where(r=>r.ReferencedSubClasses.Any(r2=>r2.FullName == cls1.FullName)).ToList();
				if (forwardRelations.Count == 0 && backRelations.Count == 0)
					return 0; // Order doesn't matter
				int order = 0;
				foreach(var rel in forwardRelations)
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
								order--;
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
							// In case of a bidirectional relation the child of the composition should win,
							// because the parent must be saved in an own transaction, anyway
							if (rel.Composition && rel.Bidirectional && rel.ForeignRelation.Multiplicity == RelationMultiplicity.Element)
								order++;
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
				return order;
			} );

			Dictionary<Type, int> updateOrder = new Dictionary<Type, int>();
			for (int i = 0; i < classes.Count; i++)
				updateOrder.Add( classes[i].SystemType, i );

			return updateOrder;
		}
	}
}
