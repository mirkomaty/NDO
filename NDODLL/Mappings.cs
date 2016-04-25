﻿//
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


using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Xml;
using NDO;
using NDO.Mapping;
using NDO.Logging;

namespace NDO 
{
	/// <summary>
	/// Summary description for Mappings.
	/// </summary>
	internal class Mappings : NDOMapping
	{
		private Hashtable persistenceHandler = new Hashtable();
		private ArrayList updateList = new ArrayList();
		private ArrayList clientGeneratedList = new ArrayList();
		private ArrayList updateSearchedFor = new ArrayList();
		private Hashtable updateOrder = new Hashtable();
		private DataSet ds;
		ILogAdapter logAdapter;
		private bool verboseMode;
		private Type defaultHandlerType;
		public Type DefaultHandlerType
		{
			get { return defaultHandlerType; }
			set { defaultHandlerType = value; }
		}


		// Must be set after the schema is generated
		public DataSet DataSet
		{
			get { return ds; }
			set { ds = value; }
		}


		internal Mappings( string mappingFile, Type defaultHandlerType )
			: base( mappingFile )
		{
			this.defaultHandlerType = defaultHandlerType;
			InitClassFields();
			BuildUpdateDependencies();
		}

		private void InitClassFields()
		{
			foreach ( Class c in Classes )
			{
				((IFieldInitializer) c).InitFields();
			}
			// New loop, because we need the SystemType entries of all classes
			foreach ( Class c in Classes )
			{
				c.ComputeRelationOrdinalBase();
				foreach ( Relation r in c.Relations )
				{
					((IFieldInitializer) r).InitFields();
				}
			}
			// New loop, we need all relations initialized
			foreach ( Class c in Classes )
			{
				c.CollectForeignKeyNames();
			}
		}


		/// <summary>
		/// Gets or sets a value which determines, if database operations will be logged in a logging file.
		/// </summary>
		internal bool VerboseMode
		{
			get { return this.verboseMode; }
			set
			{
				this.verboseMode = value;
				foreach ( DictionaryEntry de in persistenceHandler )
				{
					((IPersistenceHandler) de.Value).VerboseMode = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the Adapter, log entries are written to, if VerboseMode is true.
		/// </summary>
		internal ILogAdapter LogAdapter
		{
			set
			{
				this.logAdapter = value;
				foreach ( DictionaryEntry de in persistenceHandler )
				{
					((IPersistenceHandler) de.Value).LogAdapter = value;
				}
			}
		}


		/// <summary>
		/// Find info about the specified class.
		/// </summary>
		/// <param name="pc">the persistent object of the class</param>
		/// <returns>mapping info about the class</returns>
		public Class FindClass( IPersistenceCapable pc )
		{
			return FindClass( pc.GetType().FullName );
		}


		/// <summary>
		/// Find the relation of the given object with the specified name.
		/// </summary>
		/// <param name="pc">the persistent object with the relation</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <returns>mapping info about the relation.</returns>
		public Relation FindRelation( IPersistenceCapable pc, string fieldName )
		{
			Class c = FindClass( pc );
			return c.FindRelation( fieldName );
		}

		/// <summary>
		/// Look for a FieldInfo in the given object; search all base types for the FieldInfo
		/// </summary>
		/// <param name="pc">Object to search the FieldInfo</param>
		/// <param name="name">Name of the field</param>
		/// <returns>The FieldInfo of the given field; An exception will be thrown, if the field can't be found.</returns>
		private FieldInfo GetFieldInfo( IPersistenceCapable pc, string name )
		{
			Type t = pc.GetType();
			FieldInfo fi = new BaseClassReflector( t ).GetField( name, BindingFlags.Instance | BindingFlags.NonPublic );
			if ( fi == null )
			{
				throw new NDOException( 9, "Invalid mapping: unknown field name " + name + " in type " + t );
			}
			return fi;
		}

		/// <summary>
		/// Retrieve the specified relation from the parent object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="r">the relation mapping info</param>
		/// <returns>the content of the relation</returns>
		public IList GetRelationContainer( IPersistenceCapable pc, Relation r )
		{
			FieldInfo fi = GetFieldInfo( pc, r.FieldName );
			return (IList) fi.GetValue( pc );
		}

		/// <summary>
		/// Set the specified relation to the new content.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="r">the relation mapping info</param>
		/// <param name="list">the new content of the relation</param>
		public void SetRelationContainer( IPersistenceCapable pc, Relation r, IList list )
		{
			FieldInfo fi = GetFieldInfo( pc, r.FieldName );
			fi.SetValue( pc, list );
		}

		/// <summary>
		/// Construct a relation container
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="r">the relation mapping info</param>
		public IList CreateRelationContainer( IPersistenceCapable pc, Relation r )
		{
			FieldInfo fi = GetFieldInfo( pc, r.FieldName );
			return (IList) ListCloner.CreateList( fi.FieldType );
		}


		/// <summary>
		/// Set the related object.
		/// </summary>
		/// <param name="pc">the parent</param>
		/// <param name="fieldName">the name of the relation field</param>
		/// <param name="theObj">the related object</param>
		public void SetRelationField( IPersistenceCapable pc, string fieldName, object theObj )
		{
			FieldInfo fi = GetFieldInfo( pc, fieldName );
			fi.SetValue( pc, theObj );
		}

		/// <summary>
		/// Determine the type of a relation field.
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public Type GetRelationFieldType( Relation r )
		{
			FieldInfo fi = new BaseClassReflector( r.Parent.SystemType ).GetField( r.FieldName, BindingFlags.NonPublic | BindingFlags.Instance );
			return fi.FieldType;
		}

		/// <summary>
		/// Return the related object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the name of the relation field</param>
		/// <returns>the related object</returns>
		public object GetRelationField( IPersistenceCapable pc, string fieldName )
		{
			FieldInfo fi = GetFieldInfo( pc, fieldName );
			return fi.GetValue( pc );
		}


		/// <summary>
		/// Get a persistence handler for the given object.
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="useSelfGeneratedIds"></param>
		/// <returns></returns>
		public IPersistenceHandler GetPersistenceHandler( IPersistenceCapable pc, bool useSelfGeneratedIds )
		{
			Type t = pc.GetType();
#if !NDO11
			if ( t.IsGenericType )
				t = t.GetGenericTypeDefinition();
#endif
			IPersistenceHandler handler;
			if ( (handler = (IPersistenceHandler) persistenceHandler[t]) != null )
				return handler;
			// 1. Handler des Objekts versuchen
			handler = pc.NDOHandler;
			// 2. Standard-Handler des pm versuchen
			if ( handler == null )
			{
				if ( defaultHandlerType != null )
					handler = Activator.CreateInstance( defaultHandlerType )
						as IPersistenceHandler;
			}
			// 3. NDOPersistenceHandler versuchen
			if ( handler == null )
				handler = new NDOPersistenceHandler();

			handler.Initialize( this, t, ds );
			handler.VerboseMode = this.verboseMode;
			handler.LogAdapter = this.logAdapter;
			persistenceHandler.Add( t, handler );
			return handler;
		}

		public IPersistenceHandler GetPersistenceHandler( Type t, bool useSelfGeneratedIds )
		{
			IPersistenceHandler handler;
			if ( (handler = (IPersistenceHandler) persistenceHandler[t]) != null )
				return handler;
			//Assembly ass = Assembly.GetAssembly(t);
			//if (null == ass)
			//    throw new NDOException(10, "Assembly for Type " + t.FullName + " not found.");
#if !NDO11
			if ( t.IsGenericTypeDefinition )
				t = t.MakeGenericType( typeof( int ) );
#endif
			IPersistenceCapable pc = (IPersistenceCapable) Activator.CreateInstance( t );//ass.CreateInstance(t.FullName);
			return GetPersistenceHandler( pc, useSelfGeneratedIds );
		}

		internal ICollection Get1to1Relations( Type t )
		{
			// this will be optimized later:
			// we can cache the list of 1:1-relations for each type
			Type t2 = t;
			if ( t.IsGenericType )
				t2 = t.GetGenericTypeDefinition();
			ArrayList relations = new ArrayList();
			foreach ( Relation r in FindClass( t2.FullName ).Relations )
			{
				if ( r.Multiplicity == RelationMultiplicity.Element && r.MappingTable == null )
				{
					relations.Add( r );
				}
			}
			return relations;
		}

		/// <summary>
		/// Build dependency list for DataAdapter updates. Classes which are in front of the list must be
		/// updated first. Note that the order is reversed for creation and deletion of objects.
		/// This is handled transparently by the PM.
		/// </summary>
		private void BuildUpdateDependencies()
		{
			foreach ( Class c in Classes )
			{
				BuildUpdateDependency( c );
				/*				Console.WriteLine("Class {0} Subclasses:", c.FullName);
								foreach(Class s in c.Subclasses) {
									Console.WriteLine("  - Class {0}", s.FullName);					
								}*/
			}

			//Console.WriteLine("Dependencies:");
			// The list will copied into a hashtable together with the
			// rank of the type in the list. The rank is reverted.
			// Lower rank values have more priority in the update order.
			int end = updateList.Count - 1;
			int i;
			for ( i = 0; i <= end; i++ )
			{
				updateOrder.Add( (Type) updateList[i], end - i );
			}

			int rank = i;

			// Now add the types using client generated oids. These don't need a 
			// high rank.
			int end2 = clientGeneratedList.Count - 1;
			for ( i = 0; i <= end2; i++ )
			{
				updateOrder.Add( (Type) clientGeneratedList[i], rank++ );
			}
		}

		/// <summary>
		/// Returns the update order of a class. Higher values means that the class should be updated after
		/// classes with lower values.
		/// </summary>
		/// <param name="t">the type for which the update order is requested</param>
		/// <returns>the update order of the specified class</returns>
		public int GetUpdateOrder( Type t )
		{
			if ( t.IsGenericType && !t.IsGenericTypeDefinition )
				t = t.GetGenericTypeDefinition();
			return (int) updateOrder[t];
		}


		/// <summary>
		/// Used internally to build the list of update dependencies.
		/// </summary>
		/// <param name="c">the class that should be inserted in the update list</param>
		private void BuildUpdateDependency( Class c )
		{
			//Console.WriteLine("Build dependency for " + c.FullName);
			if ( c.SystemType == null )
			{
				throw new NDOException( 11, "Type.GetType for the type name " + c.FullName + " failed; check your mapping File." );
			}

			//Debug.WriteLine("BuildUpdateDependency " + c.FullName);

			// Avoid endless recursion
			if ( updateSearchedFor.Contains( c ) )
				return;
			updateSearchedFor.Add( c );
			// Can be stored at last, because the key is client generated
			if ( !c.Oid.HasAutoincrementedColumn )
			{
				clientGeneratedList.Add( c.SystemType );
				//updateList.Add( c.SystemType );
				return;
			}
			int minIndex = int.MinValue;
			foreach ( Relation r in c.Relations )
			{
				if ( r.MappingTable != null )  // Foreign Keys will be managed by NDO
					continue;
				if ( r.Multiplicity == RelationMultiplicity.Element )
				{
					// Here we have two keys in each table. There is some probability, that the owner in a composite is yet saved.
					if ( !(r.Bidirectional && r.ForeignRelation.Multiplicity == RelationMultiplicity.Element && r.Composition) )
						continue;
				}
				if ( c.FullName == r.ReferencedTypeName )
					continue;
				foreach ( Class sc in r.ReferencedSubClasses )
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

			foreach ( Class c2 in Classes )
			{
				if ( c2.FullName == c.FullName )
					continue;

				foreach ( Relation r in c2.Relations )
				{
					// Suche alle Relationen, die sich auf unsere Klasse beziehen
					bool found = false;
					foreach ( Class cl in r.ReferencedSubClasses )
					{
						if ( cl.FullName == c.FullName )
						{
							found = true;
							break;
						}
					}
					if ( !found )
						continue;
					if ( r.Multiplicity != RelationMultiplicity.Element )
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
			if ( ix >= 0 && ix < minIndex )
			{
				throw new NDOException( 12, "Cannot construct update dependencies for class " + c.SystemType + ". Try to use client generated primary keys for this type or for the related types." );
			}
			else if ( !updateList.Contains( c.SystemType ) && !clientGeneratedList.Contains(c.SystemType) )
			{
				updateList.Add( c.SystemType );
			}
			//Console.WriteLine("End build dependency for " + c.FullName);
		}
	}
}
