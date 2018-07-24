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


using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NDO;
using NDO.Mapping;
using NDO.Logging;
using Unity;
using NDO.SqlPersistenceHandling;

namespace NDO 
{
	/// <summary>
	/// Summary description for Mappings.
	/// </summary>
	internal class Mappings : NDOMapping
	{
        private Dictionary<Type, IPersistenceHandler> persistenceHandler = new Dictionary<Type, IPersistenceHandler>();
		private Dictionary<Type,int> updateOrder = new Dictionary<Type, int>();
		private DataSet ds;
		ILogAdapter logAdapter;
		private bool verboseMode;
		private readonly IUnityContainer configContainer;

		// Must be set after the schema is generated
		public DataSet DataSet
		{
			get { return ds; }
			set { ds = value; }
		}


		internal Mappings( string mappingFile, IUnityContainer configContainer )
			: base( mappingFile )
		{
			InitClassFields();
			this.updateOrder = new ClassRank().BuildUpdateRank( Classes );
			this.configContainer = configContainer;
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
				foreach ( var de in persistenceHandler )
				{
					de.Value.VerboseMode = value;
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
				foreach ( var de in persistenceHandler )
				{
					de.Value.LogAdapter = value;
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
			return GetPersistenceHandler( pc.GetType(), useSelfGeneratedIds );
		}

		public IPersistenceHandler GetPersistenceHandler( Type t, bool useSelfGeneratedIds )
		{
			if (t.IsGenericType)
				t = t.GetGenericTypeDefinition();

			IPersistenceHandler handler;
			if (this.persistenceHandler.ContainsKey( t ))
				return this.persistenceHandler[t];

			// 1. Standard-Handler des pm versuchen

			handler = this.configContainer.Resolve<IPersistenceHandler>();

			// 3. NDOPersistenceHandler versuchen
			if (handler == null)
				handler = new SqlPersistenceHandler(this.configContainer);

			handler.Initialize( this, t, ds );
			handler.VerboseMode = this.verboseMode;
			handler.LogAdapter = this.logAdapter;
			this.persistenceHandler.Add( t, handler );
			return handler;
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
		/// Returns the delete update order of a class. Lower values have higher delete priority. The insert and update priority is reversed.
		/// </summary>
		/// <param name="t">the type for which the update order is requested</param>
		/// <returns>the update order of the specified class</returns>
		public int GetUpdateOrder( Type t )
		{
			if ( t.IsGenericType && !t.IsGenericTypeDefinition )
				t = t.GetGenericTypeDefinition();
			return (int) updateOrder[t];
		}
	}
}
