//
// Copyright (c) 2002-2024 Mirko Matytschak 
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
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;

using NDO.Mapping;
using NDOInterfaces;
using NDO.ShortId;
using System.Globalization;
using NDO.Linq;
using NDO.Query;
using NDO.ChangeLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NDO
{
	/// <summary>
	/// Delegate type of an handler, which can be registered by the CollisionEvent of the PersistenceManager.
	/// <see cref="NDO.PersistenceManager.CollisionEvent"/>
	/// </summary>
	public delegate void CollisionHandler(object o);
	/// <summary>
	/// Delegate type of an handler, which can be registered by the IdGenerationEvent event of the PersistenceManager.
	/// <see cref="NDO.PersistenceManagerBase.IdGenerationEvent"/>
	/// </summary>
	public delegate void IdGenerationHandler(Type t, ObjectId oid);
    /// <summary>
    /// Delegate type of an handler, which can be registered by the ServiceScopeEvent event of the PersistenceManager.
    /// <see cref="NDO.PersistenceManagerBase.IdGenerationEvent"/>
    /// </summary>
    public delegate IServiceProvider ServiceScopeHandler( Type t, ObjectId oid );

    /// <summary>
    /// Delegate type of an handler, which can be registered by the OnSaving event of the PersistenceManager.
    /// </summary>
    public delegate void OnSavingHandler(ICollection l);
	/// <summary>
	/// Delegate type for the OnSavedEvent.
	/// </summary>
	/// <param name="auditSet"></param>
	public delegate void OnSavedHandler(AuditSet auditSet);

	/// <summary>
	/// Delegate type of an handler, which can be registered by the ObjectNotPresentEvent of the PersistenceManager. The event will be called, if LoadData doesn't find an object with the given oid.
	/// </summary>
	/// <param name="pc"></param>
	/// <returns>A boolean value which determines, if the handler could solve the situation.</returns>
	/// <remarks>If the handler returns false, NDO will throw an exception.</remarks>
	public delegate bool ObjectNotPresentHandler( IPersistenceCapable pc );

	/// <summary>
	/// Standard implementation of the IPersistenceManager interface. Provides transaction like manipulation of data sets.
	/// This is the main class you'll work with in your application code. For more information see the topic "Persistence Manager" in the NDO Documentation.
	/// </summary>
	public class PersistenceManager : PersistenceManagerBase, IPersistenceManager
	{		
		private bool hollowMode = false;
		private Dictionary<Relation, IMappingTableHandler> mappingHandler = new Dictionary<Relation,IMappingTableHandler>(); // currently used handlers

		private Hashtable currentRelations = new Hashtable(); // Contains names of current bidirectional relations
		private ObjectLock removeLock = new ObjectLock();
		private ObjectLock addLock = new ObjectLock();
		private ArrayList createdDirectObjects = new ArrayList(); // List of newly created objects that need to be stored twice to update foreign keys.
		// List of created objects that use mapping table and need to be stored in mapping table 
		// after they have been stored to the database to update foreign keys.
		private ArrayList createdMappingTableObjects = new ArrayList();  
		private TypeManager typeManager;
		internal bool DeferredMode { get; private set; }
		private INDOTransactionScope transactionScope;
		internal INDOTransactionScope TransactionScope => transactionScope ?? (transactionScope = ServiceProvider.GetRequiredService<INDOTransactionScope>());		

		private OpenConnectionListener openConnectionListener;

		/// <summary>
		/// Register a listener to this event if you work in concurrent scenarios and you use TimeStamps.
		/// If a collision occurs, this event gets fired and gives the opportunity to handle the situation.
		/// </summary>
		public event CollisionHandler CollisionEvent;

		/// <summary>
		/// Register a listener to this event to handle situations where LoadData doesn't find an object.
		/// The listener can determine, whether an exception should be thrown, if the situation occurs.
		/// </summary>
		public event ObjectNotPresentHandler ObjectNotPresentEvent;

		/// <summary>
		/// Register a listener to this event, if you want to be notified about the end 
		/// of a transaction. The listener gets a ICollection of all objects, which have been changed
		/// during the transaction and are to be saved or deleted.
		/// </summary>
		public event OnSavingHandler OnSavingEvent;
		/// <summary>
		/// This event is fired at the very end of the Save() method. It provides lists of the added, changed, and deleted objects.
		/// </summary>
		public event OnSavedHandler OnSavedEvent;
		
		private const string hollowMarker = "Hollow";
		private byte[] encryptionKey;
		private List<RelationChangeRecord> relationChanges = new List<RelationChangeRecord>();
		private bool isClosing = false;

		/// <summary>
		/// Gets a list of structures which represent relation changes, i.e. additions and removals
		/// </summary>
		protected internal List<RelationChangeRecord> RelationChanges
		{
			get { return this.relationChanges; }
		}

		/// <summary>
		/// Initializes a new PersistenceManager instance.
		/// </summary>
		/// <param name="mappingFileName"></param>
		protected override void Init(string mappingFileName)
		{
            try
            {
				base.Init(mappingFileName);
            }
            catch (Exception ex)
            {
                if (ex is NDOException)
                    throw;
                throw new NDOException(30, "Persistence manager initialization error: " + ex.ToString());
            }

		}

		/// <summary>
		/// Initializes the persistence manager
		/// </summary>
		/// <remarks>
		/// Note: This is the method, which will be called from all different ways to instantiate a PersistenceManagerBase.
		/// </remarks>
		/// <param name="mapping"></param>
		internal override void Init( Mappings mapping )
		{
			base.Init( mapping );

			ServiceProvider.GetRequiredService<IPersistenceManagerAccessor>().PersistenceManager = this;

			string dir = Path.GetDirectoryName( mapping.FileName );

			string typesFile = Path.Combine( dir, "NDOTypes.xml" );
			typeManager = new TypeManager( typesFile, this.mappings );

			sm = new StateManager( this );

			InitClasses();
		}


		/// <summary>
		/// Standard Constructor.
		/// </summary>
		/// <param name="scopedServiceProvider">An IServiceProvider instance, which represents a scope (e.g. a request in an AspNet application)</param>
		/// <remarks>
		/// Searches for a mapping file in the application directory. 
		/// The constructor tries to find a file with the same name as
		/// the assembly, but with the extension .ndo.xml. If the file is not found the constructor tries to find a
		/// file called AssemblyName.ndo.mapping in the application directory.
		/// </remarks>
		public PersistenceManager( IServiceProvider scopedServiceProvider = null ) : base( scopedServiceProvider )
		{
		}

        /// <summary>
        /// Loads the mapping file from the specified location. This allows to use
        /// different mapping files with different classes mapped in it.
        /// </summary>
        /// <param name="mappingFile">Path to the mapping file.</param>
        /// <param name="scopedServiceProvider">An IServiceProvider instance, which represents a scope (e.g. a request in an AspNet application)</param>
        /// <remarks>Only the Professional and Enterprise
        /// Editions can handle more than one mapping file.</remarks>
        public PersistenceManager(string mappingFile, IServiceProvider scopedServiceProvider = null) : base (mappingFile, scopedServiceProvider)
		{
		}

        /// <summary>
        /// Constructs a PersistenceManager and reuses a cached NDOMapping.
        /// </summary>
        /// <param name="mapping">The cached mapping object</param>
        /// <param name="scopedServiceProvider">An IServiceProvider instance, which represents a scope (e.g. a request in an AspNet application)</param>
        public PersistenceManager(NDOMapping mapping, IServiceProvider scopedServiceProvider = null) : base (mapping, scopedServiceProvider)
		{
		}

		#region Object Container Stuff
		/// <summary>
		/// Gets a container of all loaded objects and tries to load all child objects, 
		/// which are reachable through composite relations.
		/// </summary>
		/// <returns>An ObjectContainer object.</returns>
		/// <remarks>
		/// It is not recommended, to transfer objects with a state other than Hollow, 
		/// Persistent, or Transient.
		/// The transfer format is binary.
		/// </remarks>
		public ObjectContainer GetObjectContainer()
		{
			IList l = this.cache.AllObjects;
			foreach(IPersistenceCapable pc in l)
			{
				if (pc.NDOObjectState == NDOObjectState.PersistentDirty)
				{
					if (Logger != null)
						Logger.LogWarning( "Call to GetObjectContainer returns changed objects." );
					System.Diagnostics.Trace.WriteLine("NDO warning: Call to GetObjectContainer returns changed objects.");
				}
			}

			ObjectContainer oc = new ObjectContainer();
			oc.AddList(l);
			return oc;
		}

		/// <summary>
		/// Returns a container of all objects provided in the objects list and searches for
		/// child objects according to the serFlags.
		/// </summary>
		/// <param name="objects">The list of the root objects to add to the container.</param>
		/// <returns>An ObjectContainer object.</returns>
		/// <remarks>
		/// It is not recommended, to transfer objects with a state other than Hollow, 
		/// Persistent, or Transient.
		/// </remarks>
		public ObjectContainer GetObjectContainer(IList objects)
		{
			foreach(object o in objects)
			{
				CheckPc(o);
				IPersistenceCapable pc = o as IPersistenceCapable;
				if (pc.NDOObjectState == NDOObjectState.Hollow)
					LoadData(pc);
			}
			ObjectContainer oc = new ObjectContainer();
			oc.AddList(objects);
			return oc;
		}


		/// <summary>
		/// Returns a container containing the provided object 
		/// and tries to load all child objects 
		/// reachable through composite relations.
		/// </summary>
		/// <param name="obj">The object to be added to the container.</param>
		/// <returns>An ObjectContainer object.</returns>
		/// <remarks>
		/// It is not recommended, to transfer objects with a state other than Hollow, 
		/// Persistent, or Transient.
        /// The transfer format is binary.
		/// </remarks>
		public ObjectContainer GetObjectContainer(Object obj)
		{
			CheckPc(obj);
			if (((IPersistenceCapable)obj).NDOObjectState == NDOObjectState.Hollow)
				LoadData(obj);
			ObjectContainer oc = new ObjectContainer();
			oc.AddObject(obj);
			return oc;
		}

		/// <summary>
		/// Merges an object container to the active objects in the pm. All changes and the state
		/// of the objects will be taken over by the pm.
		/// </summary>
		/// <remarks>
		/// The parameter can be either an ObjectContainer or a ChangeSetContainer.
		/// The flag MarkAsTransient can be used to perform a kind
		/// of object based replication using the ObjectContainer class. 
		/// Objects, which are persistent at one machine, can be transfered
		/// to a second machine and treated by the receiving PersistenceManager like a newly created 
		/// object. The receiving PersistenceManager will use MakePersistent to store the whole 
		/// transient object tree. 
		/// There is one difference to freshly created objects: If an object id exists, it will be 
		/// serialized. If the NDOOidType-Attribute is valid for the given class, the transfered 
		/// oids will be reused by the receiving PersistenceManager.
		/// </remarks>
		/// <param name="ocb">The object container to be merged.</param>
		public void MergeObjectContainer(ObjectContainerBase ocb)
		{
			ChangeSetContainer csc = ocb as ChangeSetContainer;
			if (csc != null)
			{
				MergeChangeSet(csc);
				return;
			}
			ObjectContainer oc = ocb as ObjectContainer;
			if (oc != null)
			{
				InternalMergeObjectContainer(oc);
				return;
			}
			throw new NDOException(42, "Wrong argument type: MergeObjectContainer expects either an ObjectContainer or a ChangeSetContainer object as parameter.");
		}


		void InternalMergeObjectContainer(ObjectContainer oc)
		{
			// TODO: Check, if other states are useful. Find use scenarios.
			foreach(IPersistenceCapable pc in oc.RootObjects)
			{
				if (pc.NDOObjectState == NDOObjectState.Transient)
					MakePersistent(pc);
			}
			foreach(IPersistenceCapable pc in oc.RootObjects)
			{
				new OnlineMergeIterator(this.sm, this.cache).Iterate(pc);
			}
		}

		void MergeChangeSet(ChangeSetContainer cs)
		{
			foreach(IPersistenceCapable pc in cs.AddedObjects)
			{
				InternalMakePersistent(pc, false);
			}
			foreach(ObjectId oid in cs.DeletedObjects)
			{
				IPersistenceCapable pc2 = FindObject(oid);
				Delete(pc2);
			}
			foreach(IPersistenceCapable pc in cs.ChangedObjects)
			{
				IPersistenceCapable pc2 = FindObject(pc.NDOObjectId);
				Class pcClass = GetClass(pc);
				// Make sure, the object is loaded.
				if (pc2.NDOObjectState == NDOObjectState.Hollow)
					LoadData(pc2);
				MarkDirty( pc2 );  // This locks the object and generates a LockEntry, which contains a row
				var entry = cache.LockedObjects.FirstOrDefault( e => e.pc.NDOObjectId == pc.NDOObjectId );
				DataRow row = entry.row;
				pc.NDOWrite(row, pcClass.ColumnNames, 0);
				pc2.NDORead(row, pcClass.ColumnNames, 0);
			}
			foreach(RelationChangeRecord rcr in cs.RelationChanges)
			{
				IPersistenceCapable parent = FindObject(rcr.Parent.NDOObjectId);
				IPersistenceCapable child = FindObject(rcr.Child.NDOObjectId);
				Class pcClass = GetClass(parent);
				Relation r = pcClass.FindRelation(rcr.RelationName);
				if (!parent.NDOLoadState.RelationLoadState[r.Ordinal])
					LoadRelation(parent, r, true);
				if (rcr.IsAdded)
				{
					InternalAddRelatedObject(parent, r, child, true);
					if (r.Multiplicity == RelationMultiplicity.Element)
					{
						mappings.SetRelationField(parent, r.FieldName, child);
					}
					else
					{
						IList l = mappings.GetRelationContainer(parent, r);
						l.Add(child);
					}
				}
				else
				{
					RemoveRelatedObject(parent, r.FieldName, child);
					if (r.Multiplicity == RelationMultiplicity.Element)
					{
						mappings.SetRelationField(parent, r.FieldName, null);
					}
					else
					{
						IList l = mappings.GetRelationContainer(parent, r);
						try
						{
							ObjectListManipulator.Remove(l, child);
						}
						catch
						{
							throw new NDOException(50, "Error while merging a ChangeSetContainer: Child " + child.NDOObjectId.ToString() + " doesn't exist in relation " + parent.GetType().FullName + '.' + r.FieldName);
						}
					}
				}
			}

		}
		#endregion

		#region Implementation of IPersistenceManager

		// Complete documentation can be found in IPersistenceManager


        void WriteDependentForeignKeysToRow(IPersistenceCapable pc, Class cl, DataRow row)
        {
            if (!cl.Oid.IsDependent)
                return;
            WriteForeignKeysToRow(pc, row);
        }

		void InternalMakePersistent(IPersistenceCapable pc, bool checkRelations)
		{
			// Object is now under control of the state manager
			pc.NDOStateManager = sm;

			Type pcType = pc.GetType();
			Class pcClass = GetClass(pc);

			// Create new object
			DataTable dt = GetTable(pcType);
			DataRow row = dt.NewRow();   // In case of autoincremented oid, the row has a temporary oid value

            // In case of a Guid oid the value will be computed now.
            foreach (OidColumn oidColumn in pcClass.Oid.OidColumns)
            {
                if (oidColumn.SystemType == typeof(Guid) && oidColumn.FieldName == null && oidColumn.RelationName ==null)
                {
                    if (dt.Columns[oidColumn.Name].DataType == typeof(string))
                        row[oidColumn.Name] = Guid.NewGuid().ToString();
                    else
                        row[oidColumn.Name] = Guid.NewGuid();
                }
            }

			WriteObject(pc, row, pcClass.ColumnNames, 0); // save current state in DS

			// If the object is merged from an ObjectContainer, the id should be reused,
			// if the id is client generated (not Autoincremented).
			// In every other case, the oid is set to null, to force generating a new oid.
            bool fireIdGeneration = (Object)pc.NDOObjectId == null;
			if ((object)pc.NDOObjectId != null)
			{
                bool hasAutoincrement = false;
                foreach (OidColumn oidColumn in pcClass.Oid.OidColumns)
                {
                    if (oidColumn.AutoIncremented)
                    {
                        hasAutoincrement = true;
                        break;
                    }
                }
                if (hasAutoincrement) // can't store existing id
                {
                    pc.NDOObjectId = null;
                    fireIdGeneration = true;
                }
            }

            // In case of a dependent class the oid has to be read from the fields according to the relations
            WriteDependentForeignKeysToRow(pc, pcClass, row);

            if ((object)pc.NDOObjectId == null)
            {
                pc.NDOObjectId = ObjectIdFactory.NewObjectId(pcType, pcClass, row, this.typeManager);
            }

			if (!pcClass.Oid.IsDependent) // Dependent keys can't be filled with user defined data
			{
				if (fireIdGeneration)
					FireIdGenerationEvent(pcType, pc.NDOObjectId);
                // At this place the oid might have been
                // - deserialized (MergeObjectContainer)
                // - created using NewObjectId
                // - defined by the IdGenerationEvent

                // At this point we have a valid oid.
                // If the object has a field mapped to the oid we have
                // to write back the oid to the field
                int i = 0;
                new OidColumnIterator(pcClass).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
                {
                    if (oidColumn.FieldName != null)
                    {
                        FieldInfo fi = new BaseClassReflector(pcType).GetField(oidColumn.FieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                        fi.SetValue(pc, pc.NDOObjectId.Id[i]);
                    }
                    i++;
                });



                // Now write back the data into the row
                pc.NDOObjectId.Id.ToRow(pcClass, row);
            }

			
			ReadLostForeignKeysFromRow(pcClass, pc, row);  // they contain all DBNull at the moment
			dt.Rows.Add(row);

			cache.Register(pc);

			// new object that has never been written to the DS
			pc.NDOObjectState = NDOObjectState.Created;
			// Mark all Relations as loaded
			SetRelationState(pc);

			if (checkRelations)
			{
				// Handle related objects:
				foreach(Relation r in pcClass.Relations) 
				{
					if (r.Multiplicity == RelationMultiplicity.Element) 
					{
						IPersistenceCapable child = (IPersistenceCapable) mappings.GetRelationField(pc, r.FieldName);
						if(child != null) 
						{
							AddRelatedObject(pc, r, child);
						}
					} 
					else 
					{
						IList list = mappings.GetRelationContainer(pc, r);
						if(list != null) 
						{
							foreach(IPersistenceCapable relObj in list) 
							{
								if (relObj != null)
									AddRelatedObject(pc, r, relObj);
							}
						}
					}
				}
			}

			var relations  = CollectRelationStates(pc);
			cache.Lock(pc, row, relations);
		}


		/// <summary>
		/// Make an object persistent.
		/// </summary>
		/// <param name="o">the transient object that should be made persistent</param>
		public void MakePersistent(object o) 
		{
			IPersistenceCapable pc = CheckPc(o);

			//Debug.WriteLine("MakePersistent: " + pc.GetType().Name);
			//Debug.Indent();

			if (pc.NDOObjectState != NDOObjectState.Transient)
				throw new NDOException(54, "MakePersistent: Object is already persistent: " + pc.NDOObjectId.Dump());

			InternalMakePersistent(pc, true);

		}



		//		/// <summary>
		//		/// Checks, if an object has a valid id, which was created by the database
		//		/// </summary>
		//		/// <param name="pc"></param>
		//		/// <returns></returns>
		//		private bool HasValidId(IPersistenceCapable pc)
		//		{
		//			if (this.IdGenerationEvent != null)
		//				return true;
		//			return (pc.NDOObjectState != NDOObjectState.Created && pc.NDOObjectState != NDOObjectState.Transient);
		//		}


		private void CreateAddedObjectRow(IPersistenceCapable pc, Relation r, IPersistenceCapable relObj, bool makeRelObjPersistent)
		{
			// for a "1:n"-Relation w/o mapping table, we add the foreign key here. 
			if(r.HasSubclasses) 
			{
				// we don't support 1:n with foreign fields in subclasses because we would have to
				// search for objects in all subclasses! Instead use a mapping table.
				// throw new NDOException(55, "1:n Relations with subclasses must use a mapping table: " + r.FieldName);
				Debug.WriteLine("CreateAddedObjectRow: Polymorphic 1:n-relation " + r.Parent.FullName + "." + r.FieldName + " w/o mapping table");
			}

			if (!makeRelObjPersistent)
				MarkDirty(relObj);
			// Because we just marked the object as dirty, we know it's in the cache, so we don't supply the idColumn
			DataRow relObjRow = this.cache.GetDataRow(relObj);

            if (relObjRow == null)
                throw new InternalException(537, "CreateAddedObjectRow: relObjRow == null");

            pc.NDOObjectId.Id.ToForeignKey(r, relObjRow);

            if (relObj.NDOLoadState.LostRowInfo == null)
			{
				ReadLostForeignKeysFromRow(GetClass(relObj), relObj, relObjRow);
			}
			else
			{
				relObj.NDOLoadState.ReplaceRowInfos(r, pc.NDOObjectId.Id);
			}
		}

		private void PatchForeignRelation(IPersistenceCapable pc, Relation r, IPersistenceCapable relObj)
		{
			switch(relObj.NDOObjectState) 
			{
				case NDOObjectState.Persistent:
					MarkDirty(relObj);
					break;
				case NDOObjectState.Hollow:
					LoadData(relObj);
					MarkDirty(relObj);
					break;
			}

			if(r.ForeignRelation.Multiplicity == RelationMultiplicity.Element) 
			{
				IPersistenceCapable newpc;
				if((newpc = (IPersistenceCapable) mappings.GetRelationField(relObj, r.ForeignRelation.FieldName)) != null) 
				{
					if (newpc != pc)
						throw new NDOException(56, "Object is already part of another relation: " + relObj.NDOObjectId.Dump());
				}
				else
				{
					mappings.SetRelationField(relObj, r.ForeignRelation.FieldName, pc);
				}
			} 
			else 
			{
				if (!relObj.NDOGetLoadState(r.ForeignRelation.Ordinal))
					LoadRelation(relObj, r.ForeignRelation, true);
				IList l = mappings.GetRelationContainer(relObj, r.ForeignRelation);
				if(l == null) 
				{
					try
					{
						l = mappings.CreateRelationContainer(relObj, r.ForeignRelation);
					}
					catch
					{
						throw new NDOException(57, "Can't construct IList member " + relObj.GetType().FullName + "." + r.FieldName + ". Initialize the field in the default class constructor."); 
					}
					mappings.SetRelationContainer(relObj, r.ForeignRelation, l);
				}
				// Hack: Es sollte erst gar nicht zu diesem Aufruf kommen.
				// Zusätzlicher Funktions-Parameter addObjectToList oder so.
				if (!ObjectListManipulator.Contains(l, pc))
					l.Add(pc);
			}
			//AddRelatedObject(relObj, r.ForeignRelation, pc);
		}


		/// <summary>
		/// Add a related object to the specified object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">the field name of the relation</param>
		/// <param name="relObj">the related object that should be added</param>
		internal virtual void AddRelatedObject(IPersistenceCapable pc, string fieldName, IPersistenceCapable relObj) 
		{
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient);
			Relation r = mappings.FindRelation(pc, fieldName);
			AddRelatedObject(pc, r, relObj);
		}

		/// <summary>
		/// Core functionality to add an object to a relation container or relation field.
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="r"></param>
		/// <param name="relObj"></param>
		/// <param name="isMerging"></param>
		protected virtual void InternalAddRelatedObject(IPersistenceCapable pc, Relation r, IPersistenceCapable relObj, bool isMerging)
		{
			
			// avoid recursion
			if (!addLock.GetLock(relObj))
				return;

			try
			{
				//TODO: We need a relation management, which is independent of
				//the state management of an object. Currently the relation
				//lists or elements are cached for restore, if an object is marked dirty.
				//Thus we have to mark dirty our parent object in any case at the moment.
				MarkDirty(pc);

				//We should mark pc as dirty if we have a 1:1 w/o mapping table
				//We should mark relObj as dirty if we have a 1:n w/o mapping table
				//The latter happens in CreateAddedObjectRow

				Class relClass = GetClass(relObj);

				if (r.Multiplicity == RelationMultiplicity.Element 
					&& r.HasSubclasses
					&& r.MappingTable == null				
					&& !this.HasOwnerCreatedIds
					&& GetClass(pc).Oid.HasAutoincrementedColumn
					&& !relClass.HasGuidOid)
				{
					if (pc.NDOObjectState == NDOObjectState.Created && (relObj.NDOObjectState == NDOObjectState.Created || relObj.NDOObjectState == NDOObjectState.Transient ))
						throw new NDOException(61, "Can't assign object of type " + relObj + " to relation " + pc.GetType().FullName + "." + r.FieldName + ". The parent object must be saved first to obtain the Id (for example with pm.Save(true)). As an alternative you can use client generated Id's or a mapping table.");
					if (r.Composition)
						throw new NDOException(62, "Can't assign object of type " + relObj + " to relation " + pc.GetType().FullName + "." + r.FieldName + ". Can't handle a polymorphic composite relation with cardinality 1 with autonumbered id's. Use a mapping table or client generated id's.");
					if (relObj.NDOObjectState == NDOObjectState.Created || relObj.NDOObjectState == NDOObjectState.Transient)
						throw new NDOException(63, "Can't assign an object of type " + relObj + " to relation " + pc.GetType().FullName + "." + r.FieldName + ". The child object must be saved first to obtain the Id (for example with pm.Save(true)). As an alternative you can use client generated Id's or a mapping table." );
				}

                bool isDependent = relClass.Oid.IsDependent;

				if (r.Multiplicity == RelationMultiplicity.Element && isDependent)
					throw new NDOException(28, "Relations to intermediate classes must have RelationMultiplicity.List.");

				// Need to patch pc into the relation relObj->pc, because
				// the oid is built on base of this information
				if (isDependent)
				{
					CheckDependentKeyPreconditions(pc, r, relObj, relClass);
				}

				if (r.Composition || isDependent)
				{
					if (!isMerging || relObj.NDOObjectState == NDOObjectState.Transient)
						MakePersistent(relObj);
				}

				if(r.MappingTable == null) 
				{
					if (r.Bidirectional)
					{
						// This object hasn't been saved yet, so the key is wrong.
						// Therefore, the child must be written twice to update the foreign key.
#if trace
						System.Text.StringBuilder sb = new System.Text.StringBuilder();
						if (r.Multiplicity == RelationMultiplicity.Element)
							sb.Append("1");
						else
							sb.Append("n");
						sb.Append(":");
						if (r.ForeignRelation.Multiplicity == RelationMultiplicity.Element)
							sb.Append("1");
						else
							sb.Append("n");
						sb.Append ("OwnCreatedOther");
						sb.Append(relObj.NDOObjectState.ToString());
						sb.Append(' ');

						sb.Append(types[0].ToString());
						sb.Append(' ');
						sb.Append(types[1].ToString());
						Debug.WriteLine(sb.ToString());
#endif
						//						if (r.Multiplicity == RelationMultiplicity.Element 
						//							&& r.ForeignRelation.Multiplicity == RelationMultiplicity.Element)
						//						{
						// Element means: 
						// pc is keyholder 
						// -> relObj is saved first 
						// -> UpdateOrder(pc) > UpdateOrder(relObj) 
						// Both are Created - use type sort order
						if (pc.NDOObjectState == NDOObjectState.Created && (relObj.NDOObjectState == NDOObjectState.Created || relObj.NDOObjectState == NDOObjectState.Transient)
                            && GetClass(pc).Oid.HasAutoincrementedColumn && GetClass(relObj).Oid.HasAutoincrementedColumn)
						{
							if (mappings.GetUpdateOrder(pc.GetType()) 
								< mappings.GetUpdateOrder(relObj.GetType()))
								createdDirectObjects.Add(pc);
							else
								createdDirectObjects.Add( relObj );
						}
					}
					if (r.Multiplicity == RelationMultiplicity.List)
					{
						CreateAddedObjectRow(pc, r, relObj, r.Composition);
					}
				}
				else 
				{
					createdMappingTableObjects.Add(new MappingTableEntry(pc, relObj, r));
				}
				if(r.Bidirectional)
				{
                    if (r.Multiplicity == RelationMultiplicity.List && mappings.GetRelationField(relObj, r.ForeignRelation.FieldName) == null)
					{
                        if ( r.ForeignRelation.Multiplicity == RelationMultiplicity.Element )
                            mappings.SetRelationField(relObj, r.ForeignRelation.FieldName, pc);
					}
					else if ( !addLock.IsLocked( pc ) )
					{
						PatchForeignRelation( pc, r, relObj );
					}
				}

				this.relationChanges.Add( new RelationChangeRecord( pc, relObj, r.FieldName, true ) );
			}
			finally
			{
				addLock.Unlock(relObj);
				//Debug.Unindent();
			}
		}

        /// <summary>
        /// Returns an integer value which determines the rank of the given type in the update order list.
        /// </summary>
        /// <param name="t">The type to determine the update order.</param>
        /// <returns>An integer value determining the rank of the given type in the update order list.</returns>
        /// <remarks>
        /// This method is used by NDO for diagnostic purposes. There is no value in using this method in user code.
        /// </remarks>
        public int GetUpdateRank(Type t)
        {
            return mappings.GetUpdateOrder(t);
        }

		/// <summary>
		/// Add a related object to the specified object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="r">the relation mapping info</param>
		/// <param name="relObj">the related object that should be added</param>
		protected virtual void AddRelatedObject(IPersistenceCapable pc, Relation r, IPersistenceCapable relObj) 
		{
			//			string idstr;
			//			if (relObj.NDOObjectId == null)
			//				idstr = relObj.GetType().ToString();
			//			else
			//				idstr = relObj.NDOObjectId.Dump();
			//Debug.WriteLine("AddRelatedObject " + pc.NDOObjectId.Dump() + " " + idstr);
			//Debug.Indent();

			Class relClass = GetClass(relObj);
			bool isDependent = relClass.Oid.IsDependent;

			// Do some checks to guarantee that the assignment is correct
			if(r.Composition) 
			{
				if(relObj.NDOObjectState != NDOObjectState.Transient) 
				{
					throw new NDOException(58, "Can only add transient objects in Composite relation " + pc.GetType().FullName + "->" + r.ReferencedTypeName + ".");
				}
			} 
			else 
			{
				if(relObj.NDOObjectState == NDOObjectState.Transient && !isDependent) 
				{
					throw new NDOException(59, "Can only add persistent objects in Assoziation " + pc.GetType().FullName + "->" + r.ReferencedTypeName + ".");
				}
			}

			if(!r.ReferencedType.IsAssignableFrom(relObj.GetType())) 
			{
				throw new NDOException(60, "AddRelatedObject: Related object must be assignable to type: " + r.ReferencedTypeName + ". Assigned object was: " + relObj.NDOObjectId.Dump() + " Type = " + relObj.GetType());
			}

			InternalAddRelatedObject(pc, r, relObj, false);

		}

		private void CheckDependentKeyPreconditions(IPersistenceCapable pc, Relation r, IPersistenceCapable relObj, Class relClass)
		{
			// Need to patch pc into the relation relObj->pc, because
			// the oid is built on base of this information
			// The second relation has to be set before adding relObj
			// to the relation list.
			PatchForeignRelation(pc, r, relObj);
            IPersistenceCapable parent;
            foreach (Relation oidRelation in relClass.Oid.Relations)
            {
                parent = (IPersistenceCapable)mappings.GetRelationField(relObj, oidRelation.FieldName);
                if (parent == null)
                    throw new NDOException(41, "'" + relClass.FullName + "." + oidRelation.FieldName + "': One of the defining relations of a dependent class object is null - have a look at the documentation about how to initialize dependent class objects.");
                if (parent.NDOObjectState == NDOObjectState.Transient)
                    throw new NDOException(59, "Can only add persistent objects in Assoziation " + relClass.FullName + "." + oidRelation.FieldName + ". Make the object of type " + parent.GetType().FullName + " persistent.");

            }
		}


		/// <summary>
		/// Remove a related object from the specified object.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="fieldName">Field name of the relation</param>
		/// <param name="relObj">the related object that should be removed</param>
		internal virtual void RemoveRelatedObject(IPersistenceCapable pc, string fieldName, IPersistenceCapable relObj) 
		{
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient);
			Relation r = mappings.FindRelation(pc, fieldName);
			InternalRemoveRelatedObject(pc, r, relObj, true);
		}

		/// <summary>
		/// Registers a listener which will be notified, if a new connection is opened.
		/// </summary>
		/// <param name="listener">Delegate of a listener function</param>
		/// <remarks>The listener is called the first time a certain connection is used. A call to Save() resets the connection list so that the listener is called again.</remarks>
		public virtual void RegisterConnectionListener(OpenConnectionListener listener)
		{
			this.openConnectionListener = listener;
		}

		internal string OnNewConnection(NDO.Mapping.Connection conn)
		{
			if (openConnectionListener != null)
				return openConnectionListener(conn);
			return conn.Name;
		}


		/*
		doCommit should be:
		 
					Query	Save	Save(true)
		Optimistic	1		1		0
		Pessimistic	0		1		0
			
		Deferred Mode			
					Query	Save	Save(true)
		Optimistic	0		1		0
		Pessimistic	0		1		0
		 */

		internal void CheckEndTransaction(bool doCommit)
		{
			if (doCommit)
			{
				TransactionScope.Complete();
			}
		}

		internal void CheckTransaction(IPersistenceHandlerBase handler, Type t)
		{
			CheckTransaction(handler, this.GetClass(t).Connection);
		}

		/// <summary>
		/// Each and every database operation has to be preceded by a call to this function.
		/// </summary>
		internal void CheckTransaction( IPersistenceHandlerBase handler, Connection ndoConn )
		{
			TransactionScope.CheckTransaction();
			
			if (handler.Connection == null)
			{
				handler.Connection = TransactionScope.GetConnection(ndoConn.ID, () =>
				{
					IProvider p = ndoConn.Parent.GetProvider( ndoConn );
					string connStr = this.OnNewConnection( ndoConn );
					var connection = p.NewConnection( connStr );
					if (connection == null)
						throw new NDOException( 119, $"Can't construct connection for {connStr}. The provider returns null." );
					LogIfVerbose( $"Creating a connection object for {ndoConn.DisplayName}" );
					return connection;
				} );
			}

			if (TransactionMode != TransactionMode.None)
			{
				handler.Transaction = TransactionScope.GetTransaction( ndoConn.ID );
			}

			// During the tests, we work with a handler mock that always returns zero for the Connection property.
			if (handler.Connection != null && handler.Connection.State != ConnectionState.Open)
			{
				handler.Connection.Open();
				LogIfVerbose( $"Opening connection {ndoConn.DisplayName}" );
			}
		}

		/// <summary>
		/// Event Handler for the ConcurrencyError event of the IPersistenceHandler.
		/// We try to tell the object which caused the concurrency exception, that a collicion occured.
		/// This is possible if there is a listener for the CollisionEvent.
		/// Else we throw an exception.
		/// </summary>
		/// <param name="ex">Concurrency Exception which was catched during update.</param>
		private void OnConcurrencyError(System.Data.DBConcurrencyException ex)
		{
			DataRow row = ex.Row;
			if (row == null || CollisionEvent == null || CollisionEvent.GetInvocationList().Length == 0)
				throw(ex);
			if (row.RowState == DataRowState.Detached)
				return;
			foreach (Cache.Entry e in cache.LockedObjects) 
			{
				if (e.row == row)
				{
					CollisionEvent(e.pc);
					return;
				}
			}
			throw ex;
		}


		private void ReadObject(IPersistenceCapable pc, DataRow row, string[] fieldNames, int startIndex)
		{
			Class cl = GetClass(pc);
			string[] etypes = cl.EmbeddedTypes.ToArray();
			Dictionary<string,MemberInfo> persistentFields = null;
			if (etypes.Length > 0)
			{
				FieldMap fm = new FieldMap(cl);
				persistentFields = fm.PersistentFields;
			}
			foreach(string s in etypes)
			{
				try
				{
					NDO.Mapping.Field f = cl.FindField(s);
					if (f == null)
						continue;
					object o = row[f.Column.Name];
					string[] arr = s.Split('.');
					// Suche Embedded Type-Feld mit Namen arr[0]
					BaseClassReflector bcr = new BaseClassReflector(pc.GetType());
					FieldInfo parentFi = bcr.GetField(arr[0], BindingFlags.NonPublic | BindingFlags.Instance);
					// Hole das Embedded Object
					object parentOb = parentFi.GetValue(pc);

					if (parentOb == null)
						throw new Exception(String.Format("Can't read subfield {0} of type {1}, because the field {2} is null. Initialize the field {2} in your default constructor.", s, pc.GetType().FullName, arr[0]));

					// Suche darin das Feld mit Namen Arr[1]

					FieldInfo childFi = parentFi.FieldType.GetField(arr[1], BindingFlags.NonPublic | BindingFlags.Instance);
					Type childType = childFi.FieldType;

					// Don't initialize value types, if DBNull is stored in the field.
					// Exception: DateTime and Guid.
					if (o == DBNull.Value && childType.IsValueType 
						&& childType != typeof(Guid) 
						&& childType != typeof(DateTime))
						continue;

					if (childType == typeof(DateTime))
					{
						if (o == DBNull.Value)
							o = DateTime.MinValue;
					}
					if (childType.IsClass)
					{
						if (o == DBNull.Value)
							o = null;
					}

					if (childType == typeof (Guid))
					{
						if (o == DBNull.Value)
							o = Guid.Empty;
						if (o is string)
						{
							childFi.SetValue(parentOb, new Guid((string)o));
						}
						else if (o is Guid)
						{
							childFi.SetValue(parentOb, o);
						}
						else if (o is byte[])
						{
							childFi.SetValue(parentOb, new Guid((byte[])o));
						}
						else
							throw new Exception(string.Format("Can't convert Guid field to column type {0}.", o.GetType().FullName));
					}
					else if (childType.IsSubclassOf(typeof(System.Enum)))
					{
						object childOb = childFi.GetValue(parentOb);
						FieldInfo valueFi = childType.GetField("value__");
						valueFi.SetValue(childOb, o);
						childFi.SetValue(parentOb, childOb);
					}
					else
					{
						childFi.SetValue(parentOb, o);
					}
				}
				catch (Exception ex)
				{
					string msg = "Error while writing the embedded object {0} of an object of type {1}. Check your mapping file.\n{2}";

					throw new NDOException(68, string.Format(msg, s, pc.GetType().FullName, ex.Message));
				}

			}
			
			try
			{
				if (cl.HasEncryptedFields)
				{
					foreach (var field in cl.Fields.Where( f => f.Encrypted ))
					{
						string name = field.Column.Name;
						string s = (string) row[name];
						string es = AesHelper.Decrypt( s, EncryptionKey );
						row[name] = es;
					}
				}
                pc.NDORead(row, fieldNames, startIndex);
			}
			catch (Exception ex)
			{
				throw new NDOException(69, "Error while writing to a field of an object of type " + pc.GetType().FullName + ". Check your mapping file.\n"
					+ ex.Message);
			}
		}

		/// <summary>
		/// Executes a sql script to generate the database tables. 
		/// The function will execute any sql statements in the script 
		/// which are valid according to the
		/// rules of the underlying database. Result sets are ignored.
		/// </summary>
		/// <param name="scriptFile">The script file to execute.</param>
		/// <param name="conn">A connection object, containing the connection 
		/// string to the database, which should be altered.</param>
		/// <returns>A string array with error messages or the string "OK" for each of the statements in the file.</returns>
		/// <remarks>
		/// If the Connection object is invalid or null, a NDOException with ErrorNumber 44 will be thrown.
		/// Any exceptions thrown while executing a statement in the script, will be caught.
		/// Their message property will appear in the result array.
		/// If the script doesn't exist, a NDOException with ErrorNumber 48 will be thrown.
		/// </remarks>
		public string[] BuildDatabase( string scriptFile, Connection conn )
		{
			return BuildDatabase( scriptFile, conn, Encoding.UTF8 );
		}

		/// <summary>
		/// Executes a sql script to generate the database tables. 
		/// The function will execute any sql statements in the script 
		/// which are valid according to the
		/// rules of the underlying database. Result sets are ignored.
		/// </summary>
		/// <param name="scriptFile">The script file to execute.</param>
		/// <param name="conn">A connection object, containing the connection 
		/// string to the database, which should be altered.</param>
		/// <param name="encoding">The encoding of the script file. Default is UTF8.</param>
		/// <returns>A string array with error messages or the string "OK" for each of the statements in the file.</returns>
		/// <remarks>
		/// If the Connection object is invalid or null, a NDOException with ErrorNumber 44 will be thrown.
		/// Any exceptions thrown while executing a statement in the script, will be caught.
		/// Their message property will appear in the result array.
		/// If the script doesn't exist, a NDOException with ErrorNumber 48 will be thrown.
		/// </remarks>
		public string[] BuildDatabase(string scriptFile, Connection conn, Encoding encoding)
		{
			StreamReader sr = new StreamReader(scriptFile, encoding);
			string s = sr.ReadToEnd();
			sr.Close();
			string[] arr = s.Split(';');
			string last = arr[arr.Length - 1];
			bool lastInvalid = (last == null || last.Trim() == string.Empty);
			string[] result = new string[arr.Length - (lastInvalid ? 1 : 0)];
			using (var handler = GetSqlPassThroughHandler())
			{
				int i = 0;
				string ok = "OK";
				foreach (string statement in arr)
				{
					if (statement != null && statement.Trim() != string.Empty)
					{
						try
						{
							handler.Execute(statement);
							result[i] = ok;
						}
						catch (Exception ex)
						{
							result[i] = ex.Message;
						}
					}
					i++;
				}
				CheckEndTransaction(true);
			}
			return result;
		}

		/// <summary>
		/// Executes a sql script to generate the database tables. 
		/// The function will execute any sql statements in the script 
		/// which are valid according to the
		/// rules of the underlying database. Result sets are ignored.
		/// </summary>
		/// <param name="scriptFile">The script file to execute.</param>
		/// <returns></returns>
		/// <remarks>
		/// This function takes the first Connection object in the Connections list
		/// of the Mapping file und executes the script using that connection.
		/// If no default connection exists, a NDOException with ErrorNumber 44 will be thrown.
		/// Any exceptions thrown while executing a statement in the script, will be caught.
		/// Their message property will appear in the result array.
		/// If the script file doesn't exist, a NDOException with ErrorNumber 48 will be thrown.
		/// </remarks>
		public string[] BuildDatabase(string scriptFile)
		{
			if (!File.Exists(scriptFile))
				throw new NDOException(48, "Script file " + scriptFile + " doesn't exist.");
			if (!this.mappings.Connections.Any())
				throw new NDOException(48, "Mapping file doesn't define a connection.");
			Connection conn = new Connection( this.mappings );
			Connection originalConnection = (Connection)this.mappings.Connections.First();
			conn.Name = OnNewConnection( originalConnection );
			conn.Type = originalConnection.Type;
			//Connection conn = (Connection) this.mappings.Connections[0];
			return BuildDatabase(scriptFile, conn);
		}

		/// <summary>
		/// Executes a sql script to generate the database tables. 
		/// The function will execute any sql statements in the script 
		/// which are valid according to the
		/// rules of the underlying database. Result sets are ignored.
		/// </summary>
		/// <returns>
		/// A string array, containing the error messages produced by the statements 
		/// contained in the script.
		/// </returns>
		/// <remarks>
		/// The sql script is assumed to be the executable name of the entry assembly with the 
		/// extension .ndo.sql. Use BuildDatabase(string) to provide a path to a script.
		/// If the executable name can't be determined a NDOException with ErrorNumber 49 will be thrown.
		/// This function takes the first Connection object in the Connections list
		/// of the Mapping file und executes the script using that connection.
		/// If no default connection exists, a NDOException with ErrorNumber 44 will be thrown.
		/// Any exceptions thrown while executing a statement in the script, will be caught.
		/// Their message property will appear in the result array.
		/// If the script file doesn't exist, a NDOException with ErrorNumber 48 will be thrown.
		/// </remarks>
		public string[] BuildDatabase()
		{
			Assembly ass = Assembly.GetEntryAssembly();
			if (ass == null)
				throw new NDOException(49, "Can't determine the path of the entry assembly - please pass a sql script path as argument to BuildDatabase.");
			string file = Path.ChangeExtension(ass.Location, ".ndo.sql");
			return BuildDatabase(file);
		}

		/// <summary>
		/// Gets a SqlPassThroughHandler object which can be used to execute raw Sql statements.
		/// </summary>
		/// <param name="conn">Optional: The NDO-Connection to the database to be used.</param>
		/// <returns>An ISqlPassThroughHandler implementation</returns>
		public ISqlPassThroughHandler GetSqlPassThroughHandler( Connection conn = null )
		{
			if (!this.mappings.Connections.Any())
				throw new NDOException( 48, "Mapping file doesn't define a connection." );
			if (conn == null)
			{
				conn = new Connection( this.mappings );
				Connection originalConnection = (Connection) this.mappings.Connections.First();
				conn.Name = OnNewConnection( originalConnection );
				conn.Type = originalConnection.Type;
			}

			return new SqlPassThroughHandler( this, conn );
		}

		/// <summary>
		/// Gets a SqlPassThroughHandler object which can be used to execute raw Sql statements.
		/// </summary>
		/// <param name="predicate">A predicate defining which connection has to be used.</param>
		/// <returns>An ISqlPassThroughHandler implementation</returns>
		public ISqlPassThroughHandler GetSqlPassThroughHandler( Func<Connection, bool> predicate )
		{
			if (!this.mappings.Connections.Any())
				throw new NDOException( 48, "The Mapping file doesn't define a connection." );
			Connection conn = this.mappings.Connections.FirstOrDefault( predicate );
			if (conn == null)
				throw new NDOException( 48, "The Mapping file doesn't define a connection with this predicate." );
			return GetSqlPassThroughHandler( conn );
		}

		/// <summary>
		/// Executes a xml script to generate the database tables. 
		/// The function will generate and execute sql statements to perform 
		/// the changes described by the xml.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// The script file is the first file found with the search string [AssemblyNameWithoutExtension].ndodiff.[SchemaVersion].xml.
		/// If several files match the search string biggest file name in the default sort order will be executed.
		/// This function takes the first Connection object in the Connections list
		/// of the Mapping file und executes the script using that connection.
		/// If no default connection exists, a NDOException with ErrorNumber 44 will be thrown.
		/// Any exceptions thrown while executing a statement in the script, will be caught.
		/// Their message property will appear in the result string array.
		/// If no script file exists, a NDOException with ErrorNumber 48 will be thrown.
		/// Note that an additional command is executed, which will update the NDOSchemaVersion entry.
		/// </remarks>
		public string[] PerformSchemaTransitions()
		{
			Assembly ass = Assembly.GetEntryAssembly();
			if (ass == null)
				throw new NDOException(49, "Can't determine the path of the entry assembly - please pass a sql script path as argument to PerformSchemaTransitions.");
			string mask = Path.GetFileNameWithoutExtension( ass.Location ) + ".ndodiff.*.xml";
			List<string> fileNames = Directory.GetFiles( Path.GetDirectoryName( ass.Location ), mask ).ToList();
			if (fileNames.Count == 0)
				return new String[] { String.Format( "No xml script file with a name like {0} found.", mask ) };
			if (fileNames.Count > 1)
				fileNames.Sort( ( fn1, fn2 ) => CompareFileName( fn1, fn2 ) );
			return PerformSchemaTransitions( fileNames[0] );
		}


		/// <summary>
		/// Executes a xml script to generate the database tables. 
		/// The function will generate and execute sql statements to perform 
		/// the changes described by the xml.
		/// </summary>
		/// <param name="scriptFile">The script file to execute.</param>
		/// <returns></returns>
		/// <remarks>
		/// This function takes the first Connection object in the Connections list
		/// of the Mapping file und executes the script using that connection.
		/// If no default connection exists, a NDOException with ErrorNumber 44 will be thrown.
		/// Any exceptions thrown while executing a statement in the script, will be caught.
		/// Their message property will appear in the result string array.
		/// If the script file doesn't exist, a NDOException with ErrorNumber 48 will be thrown.
		/// Note that an additional command is executed, which will update the NDOSchemaVersion entry.
		/// </remarks>
		public string[] PerformSchemaTransitions(string scriptFile)
		{
			if (!File.Exists(scriptFile))
				throw new NDOException(48, "Script file " + scriptFile + " doesn't exist.");

			if (!this.mappings.Connections.Any())
				throw new NDOException(48, "Mapping file doesn't define any connection.");
			Connection conn = new Connection( mappings );
			Connection originalConnection = mappings.Connections.First();
			conn.Name = OnNewConnection( originalConnection );
			conn.Type = originalConnection.Type;
			return PerformSchemaTransitions(scriptFile, conn);
		}


		int CompareFileName( string fn1, string fn2)
		{
			Regex regex = new Regex( @"ndodiff\.(.+)\.xml" );
			Match match = regex.Match( fn1 );
			string v1 = match.Groups[1].Value;
			match = regex.Match( fn2 );
			string v2 = match.Groups[1].Value;
			return new Version( v2 ).CompareTo( new Version( v1 ) );
		}

		string GetSchemaVersion(Connection ndoConn, string schemaName)
		{
			IProvider provider = this.mappings.GetProvider( ndoConn );
			string version = "0.0";  // Initial value
			var connection = provider.NewConnection( ndoConn.Name );
			using (var handler = GetSqlPassThroughHandler())
			{
				string[] TableNames = provider.GetTableNames( connection );
				if (TableNames.Any( t => String.Compare( t, "NDOSchemaVersion", true ) == 0 ))
				{
					string sql = "SELECT Version from NDOSchemaVersion WHERE SchemaName ";
					if (schemaName == null)
						sql += "IS NULL;";
					else
						sql += "LIKE '" + schemaName + "'";
					using(IDataReader dr = handler.Execute(sql, true))
					{
						if (dr.Read())
							version = dr.GetString( 0 );
					}
				}
				else
				{
					SchemaTransitionGenerator schemaTransitionGenerator = new SchemaTransitionGenerator( ProviderFactory, ndoConn.Type, this.mappings );
					string transition = @"<NdoSchemaTransition>
    <CreateTable name=""NDOSchemaVersion"">
      <CreateColumn name=""SchemaName"" type=""System.String,mscorlib"" allowNull=""True"" />
      <CreateColumn name=""Version"" type=""System.String,mscorlib"" size=""50"" />
    </CreateTable>
</NdoSchemaTransition>";
					XElement transitionElement = XElement.Parse(transition);

					string sql = schemaTransitionGenerator.Generate( transitionElement );
					handler.Execute(sql);
					var colSchemaName = provider.GetQuotedName("SchemaName");
					var colVersion = provider.GetQuotedName("Version");
					var schemaVal = schemaName == null ? "NULL" : provider.GetSqlLiteral( schemaName );
					sql = String.Format( $"INSERT INTO NDOSchemaVersion({colSchemaName},{colVersion}) VALUES({schemaVal},'0')" );
					handler.Execute( sql );
					handler.CommitTransaction();
				}
			}

			return version;
		}

		/// <summary>
		/// Executes a xml script to generate the database tables. 
		/// The function will generate and execute sql statements to perform 
		/// the changes described by the xml. 
		/// </summary>
		/// <param name="scriptFile">The xml script file.</param>
		/// <param name="ndoConn">The connection to be used to perform the schema changes.</param>
		/// <returns>A list of strings about the states of the different schema change commands.</returns>
		/// <remarks>Note that an additional command is executed, which will update the NDOSchemaVersion entry.</remarks>
		public string[] PerformSchemaTransitions(string scriptFile, Connection ndoConn)
		{
			string schemaName = null;
			// Gespeicherte Version ermitteln.
			XElement transitionElements = XElement.Load( scriptFile );
			if (transitionElements.Attribute( "schemaName" ) != null)
				schemaName = transitionElements.Attribute( "schemaName" ).Value;
			Version version = new Version( GetSchemaVersion( ndoConn, schemaName ) );
			SchemaTransitionGenerator schemaTransitionGenerator = new SchemaTransitionGenerator( ProviderFactory, ndoConn.Type, this.mappings );
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, System.Text.Encoding.UTF8);
			bool hasChanges = false;

			foreach (XElement transitionElement in transitionElements.Elements("NdoSchemaTransition").Where(e=>new Version(e.Attribute("schemaVersion").Value).CompareTo(version) > 0))
			{
				hasChanges = true;
				sw.Write( schemaTransitionGenerator.Generate( transitionElement ) );
			}

			if (!hasChanges)
				return new string[] { };

			sw.Write( "UPDATE NDOSchemaVersion SET Version = '" );
			sw.Write( transitionElements.Attribute( "schemaVersion" ).Value );
			sw.Write( "' WHERE SchemaName " );
			if (schemaName == null)
				sw.WriteLine( "IS NULL;" );
			else
				sw.WriteLine( "LIKE '" + schemaName + "'" );			

			sw.Flush();
			ms.Position = 0L;

			StreamReader sr = new StreamReader(ms, System.Text.Encoding.UTF8);
			string s = sr.ReadToEnd();
			sr.Close();

			return InternalPerformSchemaTransitions( ndoConn, s );
		}

		private string[] InternalPerformSchemaTransitions( Connection ndoConn, string sql )
		{
			string[] arr = sql.Split( ';' );
			string last = arr[arr.Length - 1];
			bool lastInvalid = (last == null || last.Trim() == string.Empty);
			string[] result = new string[arr.Length - (lastInvalid ? 1 : 0)];
			int i = 0;
			string ok = "OK";
			using (var handler = GetSqlPassThroughHandler())
			{
				foreach (string statement in arr)
				{
					if (statement != null && statement.Trim() != string.Empty)
					{
						try
						{
							handler.Execute( statement );
							result[i] = ok;
						}
						catch (Exception ex)
						{
							result[i] = ex.Message;
						}
					}
					i++;
				}

				handler.CommitTransaction();
			}
			return result;
		}
		
		/// <summary>
		/// Transfers Data from the object to the DataRow
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="row"></param>
		/// <param name="fieldNames"></param>
		/// <param name="startIndex"></param>
		protected virtual void WriteObject(IPersistenceCapable pc, DataRow row, string[] fieldNames, int startIndex)
		{
			Class cl = GetClass( pc );
			try
			{
				pc.NDOWrite(row, fieldNames, startIndex);
				if (cl.HasEncryptedFields)
				{
					foreach (var field in cl.Fields.Where( f => f.Encrypted ))
					{
						string name = field.Column.Name;
						string s = (string) row[name];
						string es = AesHelper.Encrypt( s, EncryptionKey );
						row[name] = es;
					}
				}
			}
			catch (Exception ex)
			{
				throw new NDOException(70, "Error while reading a field of an object of type " + pc.GetType().FullName + ". Check your mapping file.\n"
					+ ex.Message);
			}

            if (cl.TypeNameColumn != null)
            {
                Type t = pc.GetType();
                row[cl.TypeNameColumn.Name] = t.FullName + "," + t.Assembly.GetName().Name;
            }

			var etypes = cl.EmbeddedTypes;
			foreach(string s in etypes)
			{
				try
				{
					NDO.Mapping.Field f = cl.FindField(s);
					if (f == null)
						continue;
					string[] arr = s.Split('.');
					// Suche Feld mit Namen arr[0] als object
					BaseClassReflector bcr = new BaseClassReflector(pc.GetType());
					FieldInfo parentFi = bcr.GetField(arr[0], BindingFlags.NonPublic | BindingFlags.Instance);
					Object parentOb = parentFi.GetValue(pc);
					if (parentOb == null)
						throw new Exception(String.Format("The field {0} is null. Initialize the field in your default constructor.", arr[0]));
					// Suche darin das Feld mit Namen Arr[1]
					FieldInfo childFi = parentFi.FieldType.GetField(arr[1], BindingFlags.NonPublic | BindingFlags.Instance);
					object o = childFi.GetValue(parentOb);
					if (o == null 
						|| o is DateTime && (DateTime) o == DateTime.MinValue 
						|| o is Guid && (Guid) o == Guid.Empty)
						o = DBNull.Value;
					row[f.Column.Name] = o;
				}
				catch (Exception ex)
				{
					string msg = "Error while reading the embedded object {0} of an object of type {1}. Check your mapping file.\n{2}";

					throw new NDOException(71, string.Format(msg, s, pc.GetType().FullName, ex.Message));
				}
			}
		}

        /// <summary>
        /// Check, if the specific field is loaded. If not, LoadData will be called.
        /// </summary>
        /// <param name="o">The parent object.</param>
        /// <param name="fieldOrdinal">A number to identify the field.</param>
        public virtual void LoadField(object o, int fieldOrdinal)
        {
            IPersistenceCapable pc = CheckPc(o);
            if (pc.NDOObjectState == NDOObjectState.Hollow)
			{
				LoadState ls = pc.NDOLoadState;
				if (ls.FieldLoadState != null)
				{
					if (ls.FieldLoadState[fieldOrdinal])
						return;
				}
				else
				{
					ls.FieldLoadState = new BitArray( GetClass( pc ).Fields.Count() );
				}
                LoadData(o);
        }
        }

#pragma warning disable 419
		/// <summary>
		/// Load the data of a persistent object. This forces the transition of the object state from hollow to persistent.
		/// </summary>
		/// <param name="o">The hollow object.</param>
		/// <remarks>Note, that the relations won't be resolved with this function, with one Exception: 1:1 relations without mapping table will be resolved during LoadData. In all other cases, use <see cref="LoadRelation">LoadRelation</see>, to force resolving a relation.<seealso cref="NDOObjectState"/></remarks>
#pragma warning restore 419
		public virtual void LoadData( object o ) 
		{
			IPersistenceCapable pc = CheckPc(o);
			Debug.Assert(pc.NDOObjectState == NDOObjectState.Hollow, "Can only load hollow objects");
			if (pc.NDOObjectState != NDOObjectState.Hollow)
				return;
			Class cl = GetClass(pc);
			IQuery q;
            q = CreateOidQuery(pc, cl);
			cache.UpdateCache(pc); // Make sure the object is in the cache

			var objects = q.Execute();
			var count = objects.Count;

			if (count > 1)
			{
				throw new NDOException( 72, "Load Data: " + count + " result objects with the same oid" );
			}
			else if (count == 0)
			{
				if (ObjectNotPresentEvent == null || !ObjectNotPresentEvent(pc))
				throw new NDOException( 72, "LoadData: Object " + pc.NDOObjectId.Dump() + " is not present in the database." );
			}
		}

		/// <summary>
		/// Creates a new IQuery object for the given type
		/// </summary>
		/// <param name="t"></param>
		/// <param name="oql"></param>
		/// <param name="hollow"></param>
		/// <param name="queryLanguage"></param>
		/// <returns></returns>
		public IQuery NewQuery(Type t, string oql, bool hollow = false, QueryLanguage queryLanguage = QueryLanguage.NDOql)
		{
			Type template = typeof( NDOQuery<object> ).GetGenericTypeDefinition();
			Type qt = template.MakeGenericType( t );
			return (IQuery)Activator.CreateInstance( qt, this, oql, hollow, queryLanguage );
		}

        private IQuery CreateOidQuery(IPersistenceCapable pc, Class cl) 
        {
            ArrayList parameters = new ArrayList();
			string oql = "oid = {0}";
			IQuery q = NewQuery(pc.GetType(), oql, false);
			q.Parameters.Add( pc.NDOObjectId );
			q.AllowSubclasses = false;
            return q;
        }

		/// <summary>
		/// Mark the object dirty. The current state is
		/// saved in a DataRow, which is stored in the DS. This is done, to allow easy rollback later. Also, the
		/// object is locked in the cache.
		/// </summary>
		/// <param name="pc"></param>
		internal virtual void MarkDirty(IPersistenceCapable pc) 
		{
			if (pc.NDOObjectState != NDOObjectState.Persistent)
				return;
			SaveObjectState(pc);
			pc.NDOObjectState = NDOObjectState.PersistentDirty;
		}

		/// <summary>
		/// Mark the object dirty, but make sure first, that the object is loaded. 
		/// The current or loaded state is saved in a DataRow, which is stored in the DS. 
		/// This is done, to allow easy rollback later. Also, the
		/// object is locked in the cache.
		/// </summary>
		/// <param name="pc"></param>
		internal void LoadAndMarkDirty(IPersistenceCapable pc) 
		{
			Debug.Assert(pc.NDOObjectState != NDOObjectState.Transient, "Transient objects can't be marked as dirty.");

			if(pc.NDOObjectState == NDOObjectState.Deleted) 
			{
				throw new NDOException(73, "LoadAndMarkDirty: Access to deleted objects is not allowed.");
			}

			if (pc.NDOObjectState == NDOObjectState.Hollow)
				LoadData(pc);

			// state is either (Created), Persistent, PersistentDirty
			if(pc.NDOObjectState == NDOObjectState.Persistent) 
			{
				MarkDirty(pc);
			}
		}



		/// <summary>
		/// Save current object state in DS and lock the object in the cache.
		/// The saved state can be used later to retrieve the original object value if the
		/// current transaction is aborted. Also the state of all relations (not related objects) is stored.
		/// </summary>
		/// <param name="pc">The object that should be saved</param>
		/// <param name="isDeleting">Determines, if the object is about being deletet.</param>
		/// <remarks>
		/// In a data row there are the following things:
		/// Item								Responsible for writing
		/// State (own, inherited, embedded)	WriteObject
		/// TimeStamp							NDOPersistenceHandler
		/// Oid									WriteId
		/// Foreign Keys and their Type Codes	WriteForeignKeys
		/// </remarks>
		protected virtual void SaveObjectState(IPersistenceCapable pc, bool isDeleting = false) 
		{
			Debug.Assert(pc.NDOObjectState == NDOObjectState.Persistent, "Object must be unmodified and persistent but is " + pc.NDOObjectState);
			
			DataTable table = GetTable(pc);
			DataRow row = table.NewRow();
			Class cl = GetClass(pc);
			WriteObject(pc, row, cl.ColumnNames, 0);
			WriteIdToRow(pc, row);
			if (!isDeleting)
				WriteLostForeignKeysToRow(cl, pc, row);
			table.Rows.Add(row);
			row.AcceptChanges();
			
			var relations = CollectRelationStates(pc);
			cache.Lock(pc, row, relations);
		}

		private void SaveFakeRow(IPersistenceCapable pc) 
		{
			Debug.Assert(pc.NDOObjectState == NDOObjectState.Hollow, "Object must be hollow but is " + pc.NDOObjectState);
			
			DataTable table = GetTable(pc);
			DataRow row = table.NewRow();
			Class pcClass = GetClass(pc);
			row.SetColumnError(GetFakeRowOidColumnName(pcClass), hollowMarker);
			Class cl = GetClass(pc);
			//WriteObject(pc, row, cl.FieldNames, 0);
			WriteIdToRow(pc, row);
			table.Rows.Add(row);
			row.AcceptChanges();
			
			cache.Lock(pc, row, null);
		}

        /// <summary>
        /// This defines one column of the row, in which we use the 
        /// ColumnError property to determine, if the row is a fake row.
        /// </summary>
        /// <param name="pcClass"></param>
        /// <returns></returns>
		private string GetFakeRowOidColumnName(Class pcClass)
		{
            // In case of several OidColumns the first column defined in the mapping
            // will be the one, holding the fake row info.
			return ((OidColumn)pcClass.Oid.OidColumns[0]).Name;
		}

		private bool IsFakeRow(Class cl, DataRow row)
		{
			return (row.GetColumnError(GetFakeRowOidColumnName(cl)) == hollowMarker);
		}

		/// <summary>
		/// Make a list of objects persistent.
		/// </summary>
		/// <param name="list">the list of IPersistenceCapable objects</param>
		public void MakePersistent(System.Collections.IList list) 
		{
			foreach (IPersistenceCapable pc in list) 
			{
				MakePersistent(pc);
			}
		}

		/// <summary>
		/// Save state of related objects in the cache. Only the list itself is duplicated and stored. The related objects are
		/// not duplicated.
		/// </summary>
		/// <param name="pc">the parent object of all relations</param>
		/// <returns></returns>
		protected internal virtual List<KeyValuePair<Relation,object>> CollectRelationStates(IPersistenceCapable pc) 
		{
			// Save state of relations
			Class c = GetClass(pc);
			List<KeyValuePair<Relation, object>> relations = new List<KeyValuePair<Relation, object>>( c.Relations.Count());
			foreach(Relation r in c.Relations)
			{
				if (r.Multiplicity == RelationMultiplicity.Element) 
				{
					relations.Add( new KeyValuePair<Relation, object>( r, mappings.GetRelationField( pc, r.FieldName ) ) );
				} 
				else 
				{
					IList l = mappings.GetRelationContainer(pc, r);
					if(l != null) 
					{
						l = (IList) ListCloner.CloneList(l);
					}
					relations.Add( new KeyValuePair<Relation, object>( r, l ) );
				}
			}

			return relations;
		}


		/// <summary>
		/// Restore the saved relations.  Note that the objects are not restored as this is handled transparently
		/// by the normal persistence mechanism. Only the number and order of objects are restored, e.g. the state,
		/// the list had at the beginning of the transaction.
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="relations"></param>
		private void RestoreRelatedObjects(IPersistenceCapable pc, List<KeyValuePair<Relation, object>> relations ) 
		{
			Class c = GetClass(pc);

			foreach(var entry in relations) 
			{
				var r = entry.Key;
				if (r.Multiplicity == RelationMultiplicity.Element)
				{
					mappings.SetRelationField(pc, r.FieldName, entry.Value);
				} 
				else 
				{
					if (pc.NDOGetLoadState(r.Ordinal))
					{
						// Help GC by clearing lists
						IList l = mappings.GetRelationContainer(pc, r);
						if(l != null) 
						{
							l.Clear();
						}
						// Restore relation
						mappings.SetRelationContainer(pc, r, (IList)entry.Value);
					}
				}
			}
		}


		/// <summary>
		/// Generates a query for related objects without mapping table.
		/// Note: this function can't be called in polymorphic scenarios,
		/// since they need a mapping table.
		/// </summary>
		/// <returns></returns>
		IList QueryRelatedObjects(IPersistenceCapable pc, Relation r, IList l, bool hollow)
		{
			// At this point of execution we know,
			// that the target type is not polymorphic and is not 1:1.

			// We can't fetch these objects with an NDOql query
			// since this would require a relation in the opposite direction

			IList relatedObjects;
			if (l != null)
				relatedObjects = l;
			else
				relatedObjects = mappings.CreateRelationContainer( pc, r );

			Type t = r.ReferencedType;
			Class cl = GetClass( t );
			var provider = cl.Provider;

			StringBuilder sb = new StringBuilder("SELECT * FROM ");
			var relClass = GetClass( r.ReferencedType );
			sb.Append( GetClass( r.ReferencedType ).GetQualifiedTableName() );
			sb.Append( " WHERE " );
			int i = 0;
			List<object> parameters = new List<object>();
			new ForeignKeyIterator( r ).Iterate( delegate ( ForeignKeyColumn fkColumn, bool isLastElement )
			   {
				   sb.Append( fkColumn.GetQualifiedName(relClass) );
				   sb.Append( " = {" );
				   sb.Append(i);
				   sb.Append( '}' );
				   parameters.Add( pc.NDOObjectId.Id[i] );
				   if (!isLastElement)
					   sb.Append( " AND " );
				   i++;
			   } );

			if (!(String.IsNullOrEmpty( r.ForeignKeyTypeColumnName )))
			{
				sb.Append( " AND " );
				sb.Append( provider.GetQualifiedTableName( relClass.TableName + "." + r.ForeignKeyTypeColumnName ) );
				sb.Append( " = " );
				sb.Append( pc.NDOObjectId.Id.TypeId );
			}

			IQuery q = NewQuery( t, sb.ToString(), hollow, Query.QueryLanguage.Sql );

			foreach (var p in parameters)
			{
				q.Parameters.Add( p );
			}

			q.AllowSubclasses = false;  // Remember: polymorphic relations always have a mapping table

			IList l2 = q.Execute();

			foreach (object o in l2)
				relatedObjects.Add( o );

			return relatedObjects;
		}


		/// <summary>
		/// Resolves an relation. The loaded objects will be hollow.
		/// </summary>
		/// <param name="o">The parent object.</param>
		/// <param name="fieldName">The field name of the container or variable, which represents the relation.</param>
		/// <param name="hollow">True, if the fetched objects should be hollow.</param>
		/// <remarks>Note: 1:1 relations without mapping table will be resolved during the transition from the hollow to the persistent state. To force this transition, use the <see cref="LoadData">LoadData</see> function.<seealso cref="LoadData"/></remarks>
		public virtual void LoadRelation(object o, string fieldName, bool hollow)
		{
			IPersistenceCapable pc = CheckPc(o);
			LoadRelationInternal(pc, fieldName, hollow);
		}

		

		internal IList LoadRelation(IPersistenceCapable pc, Relation r, bool hollow)
		{
			IList result = null;

			if (pc.NDOObjectState == NDOObjectState.Created)
				return null;

			if (pc.NDOObjectState == NDOObjectState.Hollow)
				LoadData(pc);

			if(r.MappingTable == null) 
			{
				// 1:1 are loaded with LoadData
				if (r.Multiplicity == RelationMultiplicity.List) 
				{ 
					// Help GC by clearing lists
					IList l = mappings.GetRelationContainer(pc, r);
					if(l != null)
						l.Clear();
					IList relatedObjects = QueryRelatedObjects(pc, r, l, hollow);
					mappings.SetRelationContainer(pc, r, relatedObjects);
					result = relatedObjects;
				}
			} 
			else 
			{
				DataTable dt = null;

				using (IMappingTableHandler handler = PersistenceHandlerManager.GetPersistenceHandler( pc ).GetMappingTableHandler( r ))
				{
					CheckTransaction( handler, r.MappingTable.Connection );
					dt = handler.FindRelatedObjects(pc.NDOObjectId, this.ds);
				}

				IList relatedObjects;
				if(r.Multiplicity == RelationMultiplicity.Element)
					relatedObjects = GenericListReflector.CreateList(r.ReferencedType, dt.Rows.Count);
				else
				{
					relatedObjects = mappings.GetRelationContainer(pc, r);
					if(relatedObjects != null) 
						relatedObjects.Clear();  // Objects will be reread
					else
						relatedObjects = mappings.CreateRelationContainer(pc, r);
				}
					
				foreach(DataRow objRow in dt.Rows) 
				{
					Type relType;

					if (r.MappingTable.ChildForeignKeyTypeColumnName != null)						
					{
						object typeCodeObj = objRow[r.MappingTable.ChildForeignKeyTypeColumnName];
						if (typeCodeObj is System.DBNull)
							throw new NDOException( 75, String.Format( "Can't resolve subclass type code of type {0} in relation '{1}' - the type code in the data row is null.", r.ReferencedTypeName, r.ToString() ) );
						relType = typeManager[(int)typeCodeObj];
						if (relType == null)
							throw new NDOException(75, String.Format("Can't resolve subclass type code {0} of type {1} - check, if your NDOTypes.xml exists.", objRow[r.MappingTable.ChildForeignKeyTypeColumnName], r.ReferencedTypeName));
					}						
					else
					{
						relType = r.ReferencedType;
					}

					//TODO: Generic Types: Exctract the type description from the type name column
                    if (relType.IsGenericTypeDefinition)
                        throw new NotImplementedException("NDO doesn't support relations to generic types via mapping tables.");
					ObjectId id = ObjectIdFactory.NewObjectId(relType, GetClass(relType), objRow, r.MappingTable, this.typeManager);
					IPersistenceCapable relObj = FindObject(id);
					relatedObjects.Add(relObj);
				}	
				if (r.Multiplicity == RelationMultiplicity.Element) 
				{
					Debug.Assert(relatedObjects.Count <= 1, "NDO retrieved more than one object for relation with cardinality 1");
					mappings.SetRelationField(pc, r.FieldName, relatedObjects.Count > 0 ? relatedObjects[0] : null);
				} 
				else 
				{
					mappings.SetRelationContainer(pc, r, relatedObjects);
					result = relatedObjects;
				}
			}
			// Mark relation as loaded
			pc.NDOSetLoadState(r.Ordinal, true);
			return result;
		}

		/// <summary>
		/// Loads elements of a relation
		/// </summary>
		/// <param name="pc">The object which needs to load the relation</param>
		/// <param name="relationName">The name of the relation</param>
		/// <param name="hollow">Determines, if the related objects should be hollow.</param>
		internal IList LoadRelationInternal(IPersistenceCapable pc, string relationName, bool hollow)
		{
			if (pc.NDOObjectState == NDOObjectState.Created)
				return null;
			Class cl = GetClass(pc);

			Relation r = cl.FindRelation(relationName);

			if ( r == null )
				throw new NDOException( 76, String.Format( "Error while loading related objects: Can't find relation mapping for the field {0}.{1}. Check your mapping file.", pc.GetType().FullName, relationName ) );

			if ( pc.NDOGetLoadState( r.Ordinal ) )
				return null;

			return LoadRelation(pc, r, hollow);
		}

		/// <summary>
		/// Load the related objects of a parent object. The current value of the relation is replaced by the
		/// a list of objects that has been read from the DB.
		/// </summary>
		/// <param name="pc">The parent object of the relations</param>
		/// <param name="row">A data row containing the state of the object</param>
		private void LoadRelated1To1Objects(IPersistenceCapable pc, DataRow row) 
		{
			// Stripped down to only serve 1:1-Relations w/out mapping table
			Class cl = GetClass(pc);
			foreach(Relation r in cl.Relations) 
			{
				if(r.MappingTable == null) 
				{
					if (r.Multiplicity == RelationMultiplicity.Element) 
					{
                        bool isNull = false;
                        foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
                        {
                            isNull = isNull || (row[fkColumn.Name] == DBNull.Value);
                        }
						if (isNull)
						{
							mappings.SetRelationField(pc, r.FieldName, null);
						}
						else
						{
							Type relType;
                            if (r.HasSubclasses)
                            {
                                object o = row[r.ForeignKeyTypeColumnName];
                                if (o == DBNull.Value)
                                    throw new NDOException(75, String.Format(
                                        "Can't resolve subclass type code {0} of type {1} - type code value is DBNull.",
                                        row[r.ForeignKeyTypeColumnName], r.ReferencedTypeName));
                                relType = typeManager[(int)o];
                            }
                            else
                            {
                                relType = r.ReferencedType;
                            }
                            if (relType == null)
                            {
                                throw new NDOException(75, String.Format(
                                    "Can't resolve subclass type code {0} of type {1} - check, if your NDOTypes.xml exists.",
                                    row[r.ForeignKeyTypeColumnName], r.ReferencedTypeName));
                            }
	
                            int count = r.ForeignKeyColumns.Count();
                            object[] keydata = new object[count];
                            int i = 0;
                            foreach(ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
                            {
                                keydata[i++] = row[fkColumn.Name];
                            }

                            Type oidType = relType;
                            if (oidType.IsGenericTypeDefinition)
                                oidType = mappings.GetRelationFieldType(r);

							ObjectId childOid = ObjectIdFactory.NewObjectId(oidType, GetClass(relType), keydata, this.typeManager);
							if(childOid.IsValid()) 
								mappings.SetRelationField(pc, r.FieldName, FindObject(childOid));
							else
								mappings.SetRelationField(pc, r.FieldName, null);

						}
						pc.NDOSetLoadState(r.Ordinal, true);
					} 
				} 
			}
		}

		
		/// <summary>
		/// Creates a new ObjectId with the same Key value as a given ObjectId.
		/// </summary>
		/// <param name="oid">An ObjectId, which Key value will be used to build the new ObjectId.</param>
		/// <param name="t">The type of the object, the id will belong to.</param>
		/// <returns>An object of type ObjectId, or ObjectId.InvalidId</returns>
		/// <remarks>If the type t doesn't have a mapping in the mapping file an Exception of type NDOException is thrown.</remarks>
		public ObjectId NewObjectId(ObjectId oid, Type t)
		{
			return new ObjectId(oid.Id, t);
		}

        /*
        /// <summary>
		/// Creates a new ObjectId which can be used to retrieve objects from the database.
		/// </summary>
		/// <param name="keyData">The id, which will be used to search for the object in the database</param>
		/// <param name="t">The type of the object.</param>
		/// <returns>An object of type ObjectId, or ObjectId.InvalidId</returns>
		/// <remarks>The keyData parameter must be one of the types Int32, String, Byte[] or Guid. If keyData is null or the type of keyData is invalid the function returns ObjectId.InvalidId. If the type t doesn't have a mapping in the mapping file, an Exception of type NDOException is thrown.</remarks>
		public ObjectId NewObjectId(object keyData, Type t) 
		{
			if (keyData == null || keyData == DBNull.Value)
				return ObjectId.InvalidId;

			Class cl =  GetClass(t);			
			
			if (cl.Oid.FieldType == typeof(int))
				return new ObjectId(new Int32Key(t, (int)keyData));
			else if (cl.Oid.FieldType == typeof(string))
				return new ObjectId(new StringKey(t, (String) keyData));
			else if (cl.Oid.FieldType == typeof(Guid))
				if (keyData is string)
					return new ObjectId(new GuidKey(t, new Guid((String) keyData)));
				else
					return new ObjectId(new GuidKey(t, (Guid) keyData));
			else if (cl.Oid.FieldType == typeof(MultiKey))
				return new ObjectId(new MultiKey(t, (object[]) keyData));
			else 
				return ObjectId.InvalidId;
		}
        */


        /*
         * 					if (cl.Oid.FieldName != null && HasOwnerCreatedIds)
                    {
                        // The column, which hold the oid gets overwritten, if
                        // Oid.FieldName contains a value.
*/
        private void WriteIdFieldsToRow(IPersistenceCapable pc, DataRow row)
        {
            Class cl = GetClass(pc);

            if (cl.Oid.IsDependent)
                return;

            Key key = pc.NDOObjectId.Id;

            for (int i = 0; i < cl.Oid.OidColumns.Count; i++)
            {
                OidColumn oidColumn = (OidColumn)cl.Oid.OidColumns[i];
                if (oidColumn.FieldName != null)
                {
                    row[oidColumn.Name] = key[i];
                }
            }
        }

        private void WriteIdToRow(IPersistenceCapable pc, DataRow row) 
        {
            NDO.Mapping.Class cl = GetClass(pc);
            ObjectId oid = pc.NDOObjectId;

            if (cl.TimeStampColumn != null)
                row[cl.TimeStampColumn] = pc.NDOTimeStamp;

            if (cl.Oid.IsDependent)  // Oid data is in relation columns
                return;

            Key key = oid.Id;

            for (int i = 0; i < cl.Oid.OidColumns.Count; i++)
            {
                OidColumn oidColumn = (OidColumn)cl.Oid.OidColumns[i];
                row[oidColumn.Name] = key[i];
            }
        }

        private void ReadIdFromRow(IPersistenceCapable pc, DataRow row)
        {
            ObjectId oid = pc.NDOObjectId;
            NDO.Mapping.Class cl = GetClass(pc);

            if (cl.Oid.IsDependent)  // Oid data is in relation columns
                return;

            Key key = oid.Id;

            for (int i = 0; i < cl.Oid.OidColumns.Count; i++)
            {
                OidColumn oidColumn = (OidColumn)cl.Oid.OidColumns[i];
                object o = row[oidColumn.Name];
                if (!(o is Int32) && !(o is Guid) && !(o is String) && !(o is Int64))
                    throw new NDOException(78, "ReadId: invalid Id Column type in " + oidColumn.Name + ": " + o.GetType().FullName);
                if (oidColumn.SystemType == typeof(Guid) && (o is String))
                    key[i] = new Guid((string)o);
                else
                    key[i] = o;
            }

        }

        private void ReadId (Cache.Entry e) 
        {
            ReadIdFromRow(e.pc, e.row);
        }

        private void WriteObject(IPersistenceCapable pc, DataRow row, string[] fieldNames)
        {
            WriteObject(pc, row, fieldNames, 0);
        }

        private void ReadTimeStamp(Class cl, IPersistenceCapable pc, DataRow row) 
        {
            if (cl.TimeStampColumn == null)
                return;
            object col = row[cl.TimeStampColumn];
            if (col is String)
                pc.NDOTimeStamp = new Guid(col.ToString());
            else
                pc.NDOTimeStamp = (Guid) col;
        }



        private void ReadTimeStamp(Cache.Entry e) 
        {
            IPersistenceCapable pc = e.pc;
            NDO.Mapping.Class cl = GetClass(pc);
            Debug.Assert(!IsFakeRow(cl, e.row));
            if (cl.TimeStampColumn == null)
                return;
            if (e.row[cl.TimeStampColumn] is String)
                e.pc.NDOTimeStamp = new Guid(e.row[cl.TimeStampColumn].ToString());
            else
                e.pc.NDOTimeStamp = (Guid) e.row[cl.TimeStampColumn];
        }

        /// <summary>
        /// Determines, if any objects are new, changed or deleted.
        /// </summary>
        public virtual bool HasChanges
        {
            get 
            {
                return cache.LockedObjects.Count > 0;
            }
        }

        /// <summary>
		/// Do the update for all rows in the ds.
		/// </summary>
		/// <param name="types">Types with changes.</param>
		/// <param name="delete">True, if delete operations are to be performed.</param>
		/// <remarks>
		/// Delete and Insert/Update operations are to be separated to maintain the type order.
		/// </remarks>
		private void UpdateTypes(IList types, bool delete)
		{
			foreach(Type t in types) 
			{
				//Debug.WriteLine("Update Deleted Objects: "  + t.Name);
				using (IPersistenceHandler handler = PersistenceHandlerManager.GetPersistenceHandler( t ))
				{
					CheckTransaction( handler, t );
					ConcurrencyErrorHandler ceh = new ConcurrencyErrorHandler(this.OnConcurrencyError);
					handler.ConcurrencyError += ceh;
					try
					{
						DataTable dt = GetTable(t);
						if (delete)
							handler.UpdateDeletedObjects( dt );
						else
							handler.Update( dt );
					}
					finally
					{
						handler.ConcurrencyError -= ceh;
					}
				}
			}
		}

        internal void UpdateCreatedMappingTableEntries()
        {
            foreach (MappingTableEntry e in createdMappingTableObjects)
            {
                if (!e.DeleteEntry)
                    WriteMappingTableEntry(e);
            }
            // Now update all mapping tables
            foreach (IMappingTableHandler handler in mappingHandler.Values)
            {
				CheckTransaction( handler, handler.Relation.MappingTable.Connection );
                handler.Update(ds);
            }
        }

        internal void UpdateDeletedMappingTableEntries()
        {
            foreach (MappingTableEntry e in createdMappingTableObjects)
            {
                if (e.DeleteEntry)
                    WriteMappingTableEntry(e);
            }
            // Now update all mapping tables
            foreach (IMappingTableHandler handler in mappingHandler.Values)
            {
				CheckTransaction( handler, handler.Relation.MappingTable.Connection );
				handler.Update(ds);
            }
        }

		/// <summary>
		/// Save all changed object into the DataSet and update the DB.
		/// When a newly created object is written to DB, the key might change. Therefore,
		/// the id is updated and the object is removed and re-inserted into the cache.
		/// </summary>
		public virtual void Save(bool deferCommit = false) 
		{
			this.DeferredMode = deferCommit;
			var htOnSaving = new HashSet<ObjectId>();
			for(;;)
			{
				// We need to work on a copy of the locked objects list, 
				// since the handlers might add more objects to the cache
				var lockedObjects = cache.LockedObjects.ToList();
				int count = lockedObjects.Count;
				foreach(Cache.Entry e in lockedObjects)
				{
					if (e.pc.NDOObjectState != NDOObjectState.Deleted)
					{
						IPersistenceNotifiable ipn = e.pc as IPersistenceNotifiable;
						if (ipn != null)
						{
							if (!htOnSaving.Contains(e.pc.NDOObjectId))
							{
								ipn.OnSaving();
								htOnSaving.Add(e.pc.NDOObjectId);
							}
						}
					}
				}
				// The system is stable, if the count doesn't change
				if (cache.LockedObjects.Count == count)
					break;
			}

			if (this.OnSavingEvent != null)
			{
				IList onSavingObjects = new ArrayList(cache.LockedObjects.Count);
				foreach(Cache.Entry e in cache.LockedObjects)
					onSavingObjects.Add(e.pc);
				OnSavingEvent(onSavingObjects);
			}

			List<Type> types = new List<Type>();
			List<IPersistenceCapable> deletedObjects = new List<IPersistenceCapable>();
			List<IPersistenceCapable> hollowModeObjects = hollowMode ? new List<IPersistenceCapable>() : null;
			List<IPersistenceCapable> changedObjects = new List<IPersistenceCapable>();
			List<IPersistenceCapable> addedObjects = new List<IPersistenceCapable>();
			List<Cache.Entry> addedCacheEntries = new List<Cache.Entry>();

			// Save current state in DataSet
			foreach (Cache.Entry e in cache.LockedObjects) 
			{
				Type objType = e.pc.GetType();

                if (objType.IsGenericType && !objType.IsGenericTypeDefinition)
                    objType = objType.GetGenericTypeDefinition();

				Class cl = GetClass(e.pc);
				//Debug.WriteLine("Saving: " + objType.Name + " id = " + e.pc.NDOObjectId.Dump());
				if(!types.Contains(objType)) 
				{
					//Debug.WriteLine("Added  type " + objType.Name);
					types.Add(objType);
				}
				NDOObjectState objectState = e.pc.NDOObjectState;
				if(objectState == NDOObjectState.Deleted) 
				{
					deletedObjects.Add(e.pc);
				} 
				else if(objectState == NDOObjectState.Created) 
				{
					WriteObject(e.pc, e.row, cl.ColumnNames);                    
					WriteIdFieldsToRow(e.pc, e.row);  // If fields are mapped to Oid, write them into the row
					WriteForeignKeysToRow(e.pc, e.row);
					//					Debug.WriteLine(e.pc.GetType().FullName);
					//					DataRow[] rows = new DataRow[cache.LockedObjects.Count];
					//					i = 0;
					//					foreach(Cache.Entry e2 in cache.LockedObjects)
					//					{
					//						rows[i++] = e2.row;
					//					}
					//					System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("testCommand");
					//					new SqlDumper(new DebugLogAdapter(), NDOProviderFactory.Instance["Sql"], cmd, cmd, cmd, cmd).Dump(rows);

					addedCacheEntries.Add(e);
					addedObjects.Add( e.pc );
					//					objectState = NDOObjectState.Persistent;
				} 
				else 
				{
					if (e.pc.NDOObjectState == NDOObjectState.PersistentDirty)
						changedObjects.Add( e.pc );
					WriteObject(e.pc, e.row, cl.ColumnNames);
					WriteForeignKeysToRow(e.pc, e.row); 					
				}
				if(hollowMode && (objectState == NDOObjectState.Persistent || objectState == NDOObjectState.Created || objectState == NDOObjectState.PersistentDirty) ) 
				{
					hollowModeObjects.Add(e.pc);
				}
			}

            // Before we delete any db rows, we have to make sure, to delete mapping
            // table entries first, which might have relations to the db rows to be deleted
            UpdateDeletedMappingTableEntries();

			// Update DB
			if (ds.HasChanges()) 
			{

				// We need the reversed update order for deletions.
				types.Sort( ( t1, t2 ) => 
				{
					int i1 = mappings.GetUpdateOrder( t1 );
					int i2 = mappings.GetUpdateOrder( t2 );
					if (i1 < i2)
					{
						if (!addedObjects.Any( pc => pc.GetType() == t1 ))
							i1 += 100000;
					}
					else
					{
						if (!addedObjects.Any( pc => pc.GetType() == t2 ))
							i2 += 100000;
					}
					return i2 - i1;
				} );

				// Delete records first

				UpdateTypes(types, true);

				// Now do all other updates in correct order.
				types.Reverse();

				UpdateTypes(types, false);
				
				ds.AcceptChanges();
				if(createdDirectObjects.Count > 0)
				{
					// Rewrite all children that have foreign keys to parents which have just been saved now.
					// They must be written again to store the correct foreign keys.
					foreach(IPersistenceCapable pc in createdDirectObjects) 
					{
						Class cl = GetClass(pc);
						DataRow r = this.cache.GetDataRow(pc);
						string fakeColumnName = GetFakeRowOidColumnName(cl);
						object o = r[fakeColumnName];
						r[fakeColumnName] = o;
					}

					UpdateTypes(types, false);
				}

				// Because object id might have changed during DB insertion, re-register newly created objects in the cache.
				foreach(Cache.Entry e in addedCacheEntries) 
				{
					cache.DeregisterLockedObject(e.pc);
					ReadId(e);
					cache.RegisterLockedObject(e.pc, e.row, e.relations);
				}

                // Now update all mapping tables. Because of possible subclasses, there is no
                // relation between keys in the dataset schema. Therefore, we can update mapping
                // tables only after all other objects have been written to ensure correct foreign keys.
                UpdateCreatedMappingTableEntries();

				// The rows may contain now new Ids, which should be 
				// stored in the lostRowInfo's before the rows get detached
				foreach(Cache.Entry e in cache.LockedObjects)
				{
					if (e.row.RowState != DataRowState.Detached)
					{
						IPersistenceCapable pc = e.pc;
						ReadLostForeignKeysFromRow(GetClass(pc), pc, e.row);
					}
				}

				ds.AcceptChanges();
			}

			EndSave(!deferCommit);

			foreach(IPersistenceCapable pc in deletedObjects) 
			{
				MakeObjectTransient(pc, false);
			}

			ds.Clear();
			mappingHandler.Clear();
			createdDirectObjects.Clear();
			createdMappingTableObjects.Clear();
			this.relationChanges.Clear();

			if(hollowMode) 
			{
				MakeHollow(hollowModeObjects);
			}

			if (this.OnSavedEvent != null)
			{
				AuditSet auditSet = new AuditSet()
				{
					ChangedObjects = changedObjects,
					CreatedObjects = addedObjects,
					DeletedObjects = deletedObjects
				};
				this.OnSavedEvent( auditSet );
			}
		}

		private void EndSave(bool forceCommit)
		{
			foreach(Cache.Entry e in cache.LockedObjects)
			{
				if (e.pc.NDOObjectState == NDOObjectState.Created || e.pc.NDOObjectState == NDOObjectState.PersistentDirty)
					this.ReadTimeStamp(e);
				e.pc.NDOObjectState = NDOObjectState.Persistent;
			}

			cache.UnlockAll();

			CheckEndTransaction(forceCommit);
		}

		/// <summary>
		/// Write all foreign keys for 1:1-relations.
		/// </summary>
		/// <param name="pc">The persistent object.</param>
		/// <param name="pcRow">The DataRow of the pesistent object.</param>
		private void WriteForeignKeysToRow(IPersistenceCapable pc, DataRow pcRow) 
		{
			foreach(Relation r in mappings.Get1to1Relations(pc.GetType())) 
			{
                IPersistenceCapable relObj = (IPersistenceCapable)mappings.GetRelationField(pc, r.FieldName);
                bool isDependent = GetClass(pc).Oid.IsDependent;

				if ( relObj != null )
				{
					int i = 0;
					foreach ( ForeignKeyColumn fkColumn in r.ForeignKeyColumns )
					{
						pcRow[fkColumn.Name] = relObj.NDOObjectId.Id[i++];
					}
					if ( r.ForeignKeyTypeColumnName != null )
					{
						pcRow[r.ForeignKeyTypeColumnName] = relObj.NDOObjectId.Id.TypeId;
					}
				}
				else
				{
					foreach ( ForeignKeyColumn fkColumn in r.ForeignKeyColumns )
					{
						pcRow[fkColumn.Name] = DBNull.Value;
					}
					if ( r.ForeignKeyTypeColumnName != null )
					{
						pcRow[r.ForeignKeyTypeColumnName] = DBNull.Value;
					}
				}
			}
		}



		/// <summary>
		/// Write a mapping table entry to it's corresponding table. This is a pair of foreign keys.
		/// </summary>
		/// <param name="e">the mapping table entry</param>
		private void WriteMappingTableEntry(MappingTableEntry e) 
		{
			IPersistenceCapable pc = e.ParentObject;
			IPersistenceCapable relObj = e.RelatedObject;
			Relation r = e.Relation;
			DataTable dt = GetTable(r.MappingTable.TableName);
			DataRow row = dt.NewRow();
			int i = 0;
            foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns )
            {
                row[fkColumn.Name] = pc.NDOObjectId.Id[i++];
            }
			i = 0;
            foreach (ForeignKeyColumn fkColumn in r.MappingTable.ChildForeignKeyColumns)
            {
                row[fkColumn.Name] = relObj.NDOObjectId.Id[i++];
            }

			if (r.ForeignKeyTypeColumnName != null)
				row[r.ForeignKeyTypeColumnName] = pc.NDOObjectId.Id.TypeId;
			if (r.MappingTable.ChildForeignKeyTypeColumnName != null)
				row[r.MappingTable.ChildForeignKeyTypeColumnName] = relObj.NDOObjectId.Id.TypeId;

			dt.Rows.Add(row);
			if(e.DeleteEntry) 
			{
				row.AcceptChanges();
				row.Delete();
			}

			IMappingTableHandler handler;
			if (!mappingHandler.TryGetValue( r, out handler ))
			{
				mappingHandler[r] = handler = PersistenceHandlerManager.GetPersistenceHandler( pc ).GetMappingTableHandler( r );
			}
		}


		/// <summary>
		/// Undo changes of a certain object
		/// </summary>
		/// <param name="o">Object to undo</param>
		public void Restore(object o)
		{			
			IPersistenceCapable pc = CheckPc(o);
			Cache.Entry e = null;
			foreach (Cache.Entry entry in cache.LockedObjects) 
			{
				if (entry.pc == pc)
				{
					e = entry;
					break;
				}
			}
			if (e == null)
				return;
			Class cl = GetClass(e.pc);
			switch (pc.NDOObjectState)
			{
				case NDOObjectState.PersistentDirty:
					ObjectListManipulator.Remove(createdDirectObjects, pc);
					foreach(Relation r in cl.Relations)
					{
						if (r.Multiplicity == RelationMultiplicity.Element)
						{
							IPersistenceCapable subPc = (IPersistenceCapable) mappings.GetRelationField(pc, r.FieldName);
							if (subPc != null && cache.IsLocked(subPc))
								Restore(subPc);
						}
						else
						{
							if (!pc.NDOGetLoadState(r.Ordinal))
								continue;
							IList subList = (IList) mappings.GetRelationContainer(pc, r);
							if (subList != null)
							{
								foreach(IPersistenceCapable subPc2 in subList)
								{
									if (cache.IsLocked(subPc2))
										Restore(subPc2);
								}
							}
						}
					}
					RestoreRelatedObjects(pc, e.relations);
					e.row.RejectChanges();
					ReadObject(pc, e.row, cl.ColumnNames, 0);
					cache.Unlock(pc);
					pc.NDOObjectState = NDOObjectState.Persistent;
					break;
				case NDOObjectState.Created:
					ReadObject(pc, e.row, cl.ColumnNames, 0);
					cache.Unlock(pc);
					MakeObjectTransient(pc, true);
					break;
                case NDOObjectState.Deleted:
                    if (!this.IsFakeRow(cl, e.row))
                    {
                        e.row.RejectChanges();
                        ReadObject(e.pc, e.row, cl.ColumnNames, 0);
                        e.pc.NDOObjectState = NDOObjectState.Persistent;
                    }
                    else
                    {
                        e.row.RejectChanges();
                        e.pc.NDOObjectState = NDOObjectState.Hollow;
                    }
                    cache.Unlock(pc);
                    break;

			}
		}

		/// <summary>
		/// Aborts a pending transaction without restoring the object state.
		/// </summary>
		/// <remarks>Supports both local and EnterpriseService Transactions.</remarks>
		public virtual void AbortTransaction()
		{
			TransactionScope.Dispose();
		}

		/// <summary>
		/// Rejects all changes and restores the original object state. Added Objects will be made transient.
		/// </summary>
		public virtual void Abort() 
		{
			// RejectChanges of the DS cannot be called because newly added rows would be deleted,
			// and therefore, couldn't be restored. Instead we call RejectChanges() for each
			// individual row.
			createdDirectObjects.Clear();
			createdMappingTableObjects.Clear();
			ArrayList deletedObjects = new ArrayList();
			ArrayList hollowModeObjects = hollowMode ? new ArrayList() : null;

			// Read all objects from DataSet
			foreach (Cache.Entry e in cache.LockedObjects) 
			{
				//Debug.WriteLine("Reading: " + e.pc.GetType().Name);

				Class cl = GetClass(e.pc);
				bool isFakeRow = this.IsFakeRow(cl, e.row);
				if (!isFakeRow)
				{
					RestoreRelatedObjects(e.pc, e.relations);
				}
				else
				{
					Debug.Assert(e.pc.NDOObjectState == NDOObjectState.Deleted, "Fake row objects can only exist in deleted state");
				}

				switch(e.pc.NDOObjectState) 
				{
					case NDOObjectState.Created:
						ReadObject(e.pc, e.row, cl.ColumnNames, 0);
						deletedObjects.Add(e.pc);
						break;

					case NDOObjectState.PersistentDirty:
						e.row.RejectChanges();
						ReadObject(e.pc, e.row, cl.ColumnNames, 0);
						e.pc.NDOObjectState = NDOObjectState.Persistent;
						break;

					case NDOObjectState.Deleted:
						if (!isFakeRow)
						{
							e.row.RejectChanges();
							ReadObject(e.pc, e.row, cl.ColumnNames, 0);
							e.pc.NDOObjectState = NDOObjectState.Persistent;
						}
						else
						{
							e.row.RejectChanges();
							e.pc.NDOObjectState = NDOObjectState.Hollow;
						}
						break;

					default:
						throw new InternalException(2082, "Abort(): wrong state detected: " + e.pc.NDOObjectState + " id = " + e.pc.NDOObjectId.Dump());
						//Debug.Assert(false, "Object with wrong state detected: " + e.pc.NDOObjectState);
						//break;
				}
				if(hollowMode && e.pc.NDOObjectState == NDOObjectState.Persistent) 
				{
					hollowModeObjects.Add(e.pc);
				}
			}
			cache.UnlockAll();
			foreach(IPersistenceCapable pc in deletedObjects) 
			{
				MakeObjectTransient(pc, true);
			}
			ds.Clear();
			mappingHandler.Clear();
			if(hollowMode) 
			{
				MakeHollow(hollowModeObjects);
			}

			this.relationChanges.Clear();


			AbortTransaction();
		}


		/// <summary>
		/// Reset object to its transient state and remove it from the cache. Optinally, remove the object id to
		/// disable future access.
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="removeId">Indicates if the object id should be nulled</param>
		private void MakeObjectTransient(IPersistenceCapable pc, bool removeId) 
		{
			cache.Deregister(pc);
			// MakeTransient doesn't remove the ID, because delete makes objects transient and we need the id for the ChangeLog			
			if(removeId) 
			{
				pc.NDOObjectId = null;
			}
			pc.NDOObjectState = NDOObjectState.Transient;
			pc.NDOStateManager = null;
		}

		/// <summary>
		/// Makes an object transient.
		/// The object can be used afterwards, but changes will not be saved in the database.
		/// </summary>
		/// <remarks>
		/// Only persistent or hollow objects can be detached. Hollow objects are loaded to ensure valid data.
		/// </remarks>
		/// <param name="o">The object to detach.</param>
		public void MakeTransient(object o) 
		{
			IPersistenceCapable pc = CheckPc(o);
			if(pc.NDOObjectState != NDOObjectState.Persistent && pc.NDOObjectState  != NDOObjectState.Hollow) 
			{
				throw new NDOException(79, "MakeTransient: Illegal state '" + pc.NDOObjectState + "' for this operation");
			}

			if(pc.NDOObjectState  == NDOObjectState.Hollow) 
			{
				LoadData(pc);
			}
			MakeObjectTransient(pc, true);
		}


		/// <summary>
		/// Make all objects of a list transient.
		/// </summary>
		/// <param name="list">the list of transient objects</param>
		public void MakeTransient(System.Collections.IList list) 
		{
			foreach (IPersistenceCapable pc in list)
				MakeTransient(pc);
		}

		/// <summary>
		/// Remove an object from the DB. Note that the object itself is not destroyed and may still be used.
		/// </summary>
		/// <param name="o">The object to remove</param>
		public void Delete(object o) 
		{
			IPersistenceCapable pc = CheckPc(o);
			if (pc.NDOObjectState == NDOObjectState.Transient)
			{
				throw new NDOException( 120, "Can't delete transient object" );
			}
			if (pc.NDOObjectState != NDOObjectState.Deleted) 
			{
				Delete(pc, true);
			}
		}


		private void LoadAllRelations(object o)
		{
			IPersistenceCapable pc = CheckPc(o);
			Class cl = GetClass(pc);
			foreach(Relation r in cl.Relations)
			{
				if (!pc.NDOGetLoadState(r.Ordinal))
					LoadRelation(pc, r, true);
			}
		}


		/// <summary>
		/// Remove an object from the DB. Note that the object itself is not destroyed and may still be used.
		/// </summary>
		/// <remarks>
		/// If checkAssoziations it true, the object cannot be deleted if it is part of a bidirectional assoziation.
		/// This is the case if delete was called from user code. Internally, an object may be deleted because it is called from
		/// the parent object.
		/// </remarks>
		/// <param name="pc">the object to remove</param>
		/// <param name="checkAssoziations">true if child of a composition can't be deleted</param>
		private void Delete(IPersistenceCapable pc, bool checkAssoziations) 
		{
			//Debug.WriteLine("Delete " + pc.NDOObjectId.Dump());
			//Debug.Indent();
			IDeleteNotifiable idn = pc as IDeleteNotifiable;
			if (idn != null)
				idn.OnDelete();

			LoadAllRelations(pc);
			DeleteRelatedObjects(pc, checkAssoziations);

			switch(pc.NDOObjectState) 
			{
				case NDOObjectState.Transient:
					throw new NDOException(80, "Cannot delete transient object: " + pc.NDOObjectId);

				case NDOObjectState.Created:
					DataRow row = cache.GetDataRow(pc);
					row.Delete();
                    ArrayList cdosToDelete = new ArrayList();
                    foreach (IPersistenceCapable cdo in createdDirectObjects)
                        if ((object)cdo == (object)pc)
                            cdosToDelete.Add(cdo);
                    foreach (object o in cdosToDelete)
                        ObjectListManipulator.Remove(createdDirectObjects, o);
                    MakeObjectTransient(pc, true);
                    break;
				case NDOObjectState.Persistent:
					if (!cache.IsLocked(pc)) // Deletes können durchaus mehrmals aufgerufen werden
						SaveObjectState(pc, true);
					row = cache.GetDataRow(pc);
					row.Delete();
					pc.NDOObjectState = NDOObjectState.Deleted;
					break;
				case NDOObjectState.Hollow:
					if (!cache.IsLocked(pc)) // Deletes können durchaus mehrmals aufgerufen werden
						SaveFakeRow(pc);
					row = cache.GetDataRow(pc);
					row.Delete();
					pc.NDOObjectState = NDOObjectState.Deleted;
					break;

				case NDOObjectState.PersistentDirty:
					row = cache.GetDataRow(pc);
					row.Delete();
					pc.NDOObjectState  = NDOObjectState.Deleted;
					break;

				case NDOObjectState.Deleted:
					break;
			}

			//Debug.Unindent();
		}


		private void DeleteMappingTableEntry(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
			MappingTableEntry mte = null;
			foreach(MappingTableEntry e in createdMappingTableObjects) 
			{
				if(e.ParentObject.NDOObjectId == pc.NDOObjectId && e.RelatedObject.NDOObjectId == child.NDOObjectId && e.Relation == r) 
				{
					mte = e;
					break;
				}
			}

			if(pc.NDOObjectState == NDOObjectState.Created || child.NDOObjectState == NDOObjectState.Created) 
			{
				if (mte != null)
					createdMappingTableObjects.Remove(mte);
			} 
			else 
			{
				if (mte == null)
					createdMappingTableObjects.Add(new MappingTableEntry(pc, child, r, true));
			}
		}

		private void DeleteOrNullForeignRelation(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
			// Two tasks: a) Null the foreign key
			//			  b) remove the element from the foreign container

			if (!r.Bidirectional)
				return;

			// this keeps the oid valid
			if (GetClass(child.GetType()).Oid.IsDependent)
				return;

			if(r.ForeignRelation.Multiplicity == RelationMultiplicity.Element) 
			{
				LoadAndMarkDirty(child);
				mappings.SetRelationField(child, r.ForeignRelation.FieldName, null);
			} 
			else //if(r.Multiplicity == RelationMultiplicity.List && 
				// r.ForeignRelation.Multiplicity == RelationMultiplicity.List)  
			{
				if (!child.NDOGetLoadState(r.ForeignRelation.Ordinal))
					LoadRelation(child, r.ForeignRelation, true);
				IList l = mappings.GetRelationContainer(child, r.ForeignRelation);
				if (l == null)
					throw new NDOException(67, "Can't remove object from the list " + child.GetType().FullName + "." + r.ForeignRelation.FieldName + ". The list is null.");
				ObjectListManipulator.Remove(l, pc);
				// Don't need to delete the mapping table entry, because that was done
				// through the parent.
			}
		}

		private void DeleteOrNullRelation(IPersistenceCapable pc, Relation r, IPersistenceCapable child)
		{
			// 1) Element	nomap	ass
			// 2) Element	nomap	comp
			// 3) Element	map		ass
			// 4) Element	map		comp
			// 5) List		nomap	ass
			// 6) List		nomap	comp
			// 7) List		map		ass
			// 8) List		map		comp

			// Two tasks: Null foreign key and, if Composition, delete the child

			// If Mapping Table, delete the entry - 3,7
			// If List and assoziation, null the foreign key in the foreign class - 5
			// If Element, null the foreign key in the own class 1,2,3,4
			// If composition, delete the child 2,4,6,8

			// If the relObj is newly created
			ObjectListManipulator.Remove(createdDirectObjects, child);
			Class childClass = GetClass(child);

			if (r.MappingTable != null)  // 3,7
			{				
				DeleteMappingTableEntry(pc, r, child);
			}
			else if (r.Multiplicity == RelationMultiplicity.List 
				&& !r.Composition && !childClass.Oid.IsDependent) // 5
			{				
				LoadAndMarkDirty(child);
				DataRow row = this.cache.GetDataRow(child);
                foreach (ForeignKeyColumn fkColumnn in r.ForeignKeyColumns)
                {
                    row[fkColumnn.Name] = DBNull.Value;
                    child.NDOLoadState.ReplaceRowInfo(fkColumnn.Name, DBNull.Value);
                }
			}
			else if (r.Multiplicity == RelationMultiplicity.Element) // 1,2,3,4
			{
				LoadAndMarkDirty(pc);
			}
			if (r.Composition || childClass.Oid.IsDependent)
			{
#if DEBUG
				if (child.NDOObjectState == NDOObjectState.Transient)
				    Debug.WriteLine("***** Object shouldn't be transient: " + child.GetType().FullName);
#endif
				// Deletes the foreign key in case of List multiplicity
				// In case of Element multiplicity, the parent is either deleted,
				// or RemoveRelatedObject is called because the relation has been nulled.
				Delete(child);  
			}
		}

		/// <summary>
		/// Remove a related object
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="r"></param>
		/// <param name="child"></param>
		/// <param name="calledFromStateManager"></param>
		protected virtual void InternalRemoveRelatedObject(IPersistenceCapable pc, Relation r, IPersistenceCapable child, bool calledFromStateManager)
		{
			//TODO: We need a relation management, which is independent of
			//the state management of an object. At the moment the relation
			//lists or elements are cached for restore, if an object is marked dirty.
			//Thus we have to mark dirty our parent object in any case at the moment.
			if (calledFromStateManager)
				MarkDirty(pc);

			// Object can be deleted in an OnDelete-Handler
			if (child.NDOObjectState == NDOObjectState.Deleted)
				return;
			//			Debug.WriteLine("InternalRemoveRelatedObject " + pc.GetType().Name + " " + r.FieldName + " " + child.GetType());
			// Preconditions
			// This is called either by DeleteRelatedObjects or by RemoveRelatedObject
			// The Object state is checked there

			// If there is a composition in the opposite direction
			// && the other direction hasn't been processed
			// throw an exception.
			// If an exception is thrown at this point, have a look at IsLocked....
			if (r.Bidirectional && r.ForeignRelation.Composition
				&& !removeLock.IsLocked(child, r.ForeignRelation, pc))
				throw new NDOException(82, "Cannot remove related object " + child.GetType().FullName + " from parent " + pc.NDOObjectId.Dump() + ". Object must be removed through the parent.");

			if (!removeLock.GetLock(pc, r, child))
				return;

			try
			{
				// must be in this order, since the child
				// can be deleted in DeleteOrNullRelation
				//if (changeForeignRelations)
				DeleteOrNullForeignRelation(pc, r, child);
				DeleteOrNullRelation(pc, r, child);
			}
			finally
			{
				removeLock.Unlock(pc, r, child);
			}

			this.relationChanges.Add( new RelationChangeRecord( pc, child, r.FieldName, false ) );
		}

		private void DeleteRelatedObjects2(IPersistenceCapable pc, Class parentClass, bool checkAssoziations, Relation r)
		{
			//			Debug.WriteLine("DeleteRelatedObjects2 " + pc.GetType().Name + " " + r.FieldName);
			//			Debug.Indent();
			if (r.Multiplicity == RelationMultiplicity.Element) 
			{
				IPersistenceCapable child = (IPersistenceCapable) mappings.GetRelationField(pc, r.FieldName);
				if(child != null) 
				{
					//					if(checkAssoziations && r.Bidirectional && !r.Composition) 
					//					{
					//						if (!r.ForeignRelation.Composition)
					//						{
					//							mappings.SetRelationField(pc, r.FieldName, null);
					//							mappings.SetRelationField(child, r.ForeignRelation.FieldName, null);
					//						}
					//							//System.Diagnostics.Debug.WriteLine("Nullen: pc = " + pc.GetType().Name + " child = " + child.GetType().Name);
					//						else
					//							throw new NDOException(83, "Can't remove object of type " + pc.GetType().FullName + "; It is contained by an object of type " + child.GetType().FullName);
					//					}
					InternalRemoveRelatedObject(pc, r, child, false);
				}
			} 
			else 
			{
				IList list = mappings.GetRelationContainer(pc, r);
				if(list != null && list.Count > 0) 
				{
					//					if(checkAssoziations && r.Bidirectional && !r.Composition) 
					//					{
					//						throw new xxNDOException(84, "Cannot delete object " + pc.NDOObjectId + " in an assoziation. Remove related objects first.");
					//					}
					// Since RemoveRelatedObjects probably changes the list,
					// we iterate through a copy of the list.
					ArrayList al = new ArrayList(list); 
					foreach(IPersistenceCapable relObj in al) 
					{
						InternalRemoveRelatedObject(pc, r, relObj, false);
					}
				}
			}
			//			Debug.Unindent();
		}

		/// <summary>
		/// Remove all related objects from a parent.
		/// </summary>
		/// <param name="pc">the parent object</param>
		/// <param name="checkAssoziations"></param>
		private void DeleteRelatedObjects(IPersistenceCapable pc, bool checkAssoziations) 
		{
			//			Debug.WriteLine("DeleteRelatedObjects " + pc.NDOObjectId.Dump());
			//			Debug.Indent();
			// delete all related objects:
			Class parentClass = GetClass(pc);
			// Remove Assoziations first
			foreach(Relation r in parentClass.Relations) 
			{
				if (!r.Composition)
					DeleteRelatedObjects2(pc, parentClass, checkAssoziations, r);
			}
			foreach(Relation r in parentClass.Relations) 
			{
				if (r.Composition)
					DeleteRelatedObjects2(pc, parentClass, checkAssoziations, r);
			}

			//			Debug.Unindent();
		}

		/// <summary>
		/// Delete a list of objects
		/// </summary>
		/// <param name="list">the list of objects to remove</param>
		public void Delete(IList list) 
		{
            for (int i = 0; i < list.Count; i++)
            {
                IPersistenceCapable pc = (IPersistenceCapable) list[i];
                Delete(pc);
            }
		}

		/// <summary>
		/// Make object hollow. All relations will be unloaded and object data will be 
		/// newly fetched during the next touch of a persistent field.
		/// </summary>
		/// <param name="o"></param>
		public virtual void MakeHollow(object o) 
		{
			IPersistenceCapable pc = CheckPc(o);
			MakeHollow(pc, false);
		}

		/// <summary>
		/// Make the object hollow if it is persistent. Unload all complex data.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="recursive">if true then unload related objects as well</param>
		public virtual void MakeHollow(object o, bool recursive) 
		{
			IPersistenceCapable pc = CheckPc(o);
			if(pc.NDOObjectState == NDOObjectState.Hollow) 
				return;
			if(pc.NDOObjectState != NDOObjectState.Persistent) 
			{
				throw new NDOException(85, "MakeHollow: Illegal state for this operation (" + pc.NDOObjectState.ToString() + ")");
			}
			pc.NDOObjectState = NDOObjectState.Hollow;
			MakeRelationsHollow(pc, recursive);
		}

		/// <summary>
		/// Make all objects of a list hollow.
		/// </summary>
		/// <param name="list">the list of objects that should be made hollow</param>
		public virtual void MakeHollow(System.Collections.IList list) 
		{
			MakeHollow(list, false);
		}

		/// <summary>
		/// Make all objects of a list hollow.
		/// </summary>
		/// <param name="list">the list of objects that should be made hollow</param>
		/// <param name="recursive">if true then unload related objects as well</param>
		public void MakeHollow(System.Collections.IList list, bool recursive) 
		{
			foreach (IPersistenceCapable pc in list)
				MakeHollow(pc, recursive);				
		}

		/// <summary>
		/// Make all unlocked objects in the cache hollow.
		/// </summary>
		public virtual void MakeAllHollow() 
		{
			foreach(var pc in cache.UnlockedObjects) 
			{
				MakeHollow(pc, false);
			}
		}

		/// <summary>
		/// Make relations hollow.
		/// </summary>
		/// <param name="pc">The parent object</param>
		/// <param name="recursive">If true, the function unloads related objects as well.</param>
		private void MakeRelationsHollow(IPersistenceCapable pc, bool recursive) 
		{
			Class c = GetClass(pc);
			foreach(Relation r in c.Relations) 
			{
				if (r.Multiplicity == RelationMultiplicity.Element) 
				{
					mappings.SetRelationField(pc, r.FieldName, null);
					//					IPersistenceCapable child = (IPersistenceCapable) mappings.GetRelationField(pc, r.FieldName);
					//					if((null != child) && recursive) 
					//					{
					//						MakeHollow(child, true);
					//					}
				} 
				else 
				{
					if (!pc.NDOGetLoadState(r.Ordinal))
						continue;
					// Help GC by clearing lists
					IList l = mappings.GetRelationContainer(pc, r);
					if(l != null) 
					{
						if(recursive) 
						{
							MakeHollow(l, true);
						}
						l.Clear();
					}
					// Hollow relation
					mappings.SetRelationContainer(pc, r, null);
				}
			}
			ClearRelationState(pc);
		}

		private void ClearRelationState(IPersistenceCapable pc)
		{
			Class cl = GetClass(pc);
			foreach(Relation r in cl.Relations)
				pc.NDOSetLoadState(r.Ordinal, false);
		}

		private void SetRelationState(IPersistenceCapable pc)
		{
			Class cl = GetClass(pc);
            // Due to a bug in the enhancer the constructors are not always patched right,
            // so NDOLoadState might be uninitialized
            if (pc.NDOLoadState == null)
            {
                FieldInfo fi = new BaseClassReflector(pc.GetType()).GetField("_ndoLoadState", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fi == null)
                    throw new InternalException(3131, "pm.SetRelationState: No FieldInfo for _ndoLoadState");
                fi.SetValue(pc, new LoadState());
            }

            // After serialization the relation load state is null
			if (pc.NDOLoadState.RelationLoadState == null)
				pc.NDOLoadState.RelationLoadState = new BitArray(LoadState.RelationLoadStateSize);
			foreach(Relation r in cl.Relations)
				pc.NDOSetLoadState(r.Ordinal, true);
		}

		/// <summary>
		/// Creates an object of a given type and resolves constructor parameters using the container.
		/// </summary>
		/// <param name="t">The type of the persistent object</param>
		/// <returns></returns>
		public IPersistenceCapable CreateObject(Type t) 
		{
			return (IPersistenceCapable) ActivatorUtilities.CreateInstance( ServiceProvider, t );
		}

		/// <summary>
		/// Creates an object of a given type and resolves constructor parameters using the container.
		/// </summary>
		/// <typeparam name="T">The type of the object to create.</typeparam>
		/// <returns></returns>
		public T CreateObject<T>()
		{
			return (T)CreateObject( typeof( T ) );
		}

		/// <summary>
		/// Gets the requested object. It first builds an ObjectId using the type and the 
		/// key data. Then it uses FindObject to retrieve the object. No database access 
		/// is performed.
		/// </summary>
		/// <param name="t">The type of the object to retrieve.</param>
		/// <param name="keyData">The key value to build the object id.</param>
		/// <returns>A hollow object</returns>
		/// <remarks>If the key value is of a wrong type, an exception will be thrown, if the object state changes from hollow to persistent.</remarks>
		public IPersistenceCapable FindObject(Type t, object keyData) 
		{
            ObjectId oid = ObjectIdFactory.NewObjectId(t, GetClass(t), keyData, this.typeManager);
			return FindObject(oid);
		}

		/// <summary>
		/// Finds an object using a short id.
		/// </summary>
		/// <param name="encodedShortId"></param>
		/// <returns></returns>
		public IPersistenceCapable FindObject(string encodedShortId)
		{
			string shortId = encodedShortId.Decode();
			string[] arr = shortId.Split( '-' );
			if (arr.Length != 3)
				throw new ArgumentException( "The format of the string is not valid", "shortId" );
			Type t = shortId.GetObjectType(this);  // try readable format
			if (t == null)
			{
				int typeCode = 0;
				if (!int.TryParse( arr[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out typeCode )) 
					throw new ArgumentException( "The string doesn't represent a loadable type", "shortId" );
				t = this.typeManager[typeCode];
				if (t == null) 
					throw new ArgumentException( "The string doesn't represent a loadable type", "shortId" );
			}

			Class cls = GetClass( t );
			if (cls == null)
				throw new ArgumentException( "The type identified by the string is not persistent or is not managed by the given mapping file", "shortId" );

			object[] keydata = new object[cls.Oid.OidColumns.Count];
			string[] oidValues = arr[2].Split( ' ' );

			int i = 0;
			foreach (var oidValue in oidValues)
			{
				Type oidType = cls.Oid.OidColumns[i].SystemType;
				if (oidType == typeof( int ))
				{
					int key;
					if (!int.TryParse( oidValue, out key ))
						throw new ArgumentException( $"The ShortId value at index {i} doesn't contain an int value", nameof(encodedShortId) );
                    if (key > (int.MaxValue >> 1))
                        key = -(int.MaxValue - key);
					keydata[i] = key;
				}
				else if (oidType == typeof( Guid ))
				{
					Guid key;
					if (!Guid.TryParse( oidValue, out key ))
						throw new ArgumentException( $"The ShortId value at index {i} doesn't contain an Guid value", nameof( encodedShortId ) );
					keydata[i] = key;
				}
				else if (oidType == typeof( string ))
				{
					keydata[i] = oidValue;
				}
				else
				{
					throw new ArgumentException( $"The oid type at index {i} of the persistent type {t} can't be used by a ShortId: {oidType.FullName}", nameof( encodedShortId ) );
				}

				i++;
			}

			if (keydata.Length == 1)
				return FindObject( t, keydata[0] );

			return FindObject( t, keydata );			
		}

		/// <summary>
		/// Gets the requested object. If it is in the cache, the cached object is returned, otherwise, a new (hollow)
		/// instance of the object is returned. In either case, the DB is not accessed!
		/// </summary>
		/// <param name="id">Object id</param>
		/// <returns>The object to retrieve in hollow state</returns>		
		public IPersistenceCapable FindObject(ObjectId id) 
		{
			if(id == null) 
			{
				throw new ArgumentNullException("id");
			}

			if(!id.IsValid()) 
			{
				throw new NDOException(86, "FindObject: Invalid object id. Object does not exist");
			}

			IPersistenceCapable pc = cache.GetObject(id);
			if(pc == null) 
			{
				pc = CreateObject(id.Id.Type);
				pc.NDOObjectId = id;
				pc.NDOStateManager = sm;
				pc.NDOObjectState = NDOObjectState.Hollow;
				cache.UpdateCache(pc);
			}
			return pc;
		}


		/// <summary>
		/// Reload an object from the database.
		/// </summary>
		/// <param name="o">The object to be reloaded.</param>
		public virtual void Refresh(object o) 
		{
			IPersistenceCapable pc = CheckPc(o);
			if(pc.NDOObjectState == NDOObjectState.Transient || pc.NDOObjectState == NDOObjectState.Deleted) 
			{
				throw new NDOException(87, "Refresh: Illegal state " + pc.NDOObjectState + " for this operation");
			}

			if(pc.NDOObjectState == NDOObjectState.Created || pc.NDOObjectState == NDOObjectState.PersistentDirty)
				return; // Cannot update objects in current transaction

			MakeHollow(pc);
			LoadData(pc);
		}

		/// <summary>
		/// Refresh a list of objects.
		/// </summary>
		/// <param name="list">The list of objects to be refreshed.</param>
		public virtual void Refresh(IList list) 
		{
			foreach (IPersistenceCapable pc in list)
				Refresh(pc);						
		}

		/// <summary>
		/// Refreshes all unlocked objects in the cache.
		/// </summary>
		public virtual void RefreshAll() 
		{
			Refresh( cache.UnlockedObjects.ToList() );
		}

		/// <summary>
		/// Closes the PersistenceManager and releases all resources.
		/// </summary>
		public override void Close() 
		{
			if (this.isClosing)
				return;
			this.isClosing = true;
			TransactionScope.Dispose();
			UnloadCache();
			base.Close();
		}

		internal void LogIfVerbose( string msg )
		{
			if (Logger != null && Logger.IsEnabled( LogLevel.Information ))
				Logger.LogInformation( msg );
		}


		#endregion


#region Class extent
		/// <summary>
		/// Gets all objects of a given class.
		/// </summary>
		/// <param name="t">the type of the class</param>
		/// <returns>A list of all persistent objects of the given class. Subclasses will not be included in the result set.</returns>
		public virtual IList GetClassExtent(Type t) 
		{
			return GetClassExtent(t, true);
		}

		/// <summary>
		/// Gets all objects of a given class.
		/// </summary>
		/// <param name="t">The type of the class.</param>
		/// <param name="hollow">If true, return objects in hollow state instead of persistent state.</param>
		/// <returns>A list of all persistent objects of the given class.</returns>
		/// <remarks>Subclasses of the given type are not fetched.</remarks>
		public virtual IList GetClassExtent(Type t, bool hollow) 
		{
			IQuery q = NewQuery( t, null, hollow );
			return q.Execute();
		}

#endregion

#region Query engine


		/// <summary>
		/// Returns a virtual table for Linq queries.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public VirtualTable<T> Objects<T>() //where T: IPersistenceCapable
		{
			return new VirtualTable<T>( this );
		}

		
		/// <summary>
        /// Suppose, you had a directed 1:n relation from class A to class B. If you load an object of type B, 
        /// a foreign key pointing to a row in the table A is read as part of the B row. But since B doesn't have
        /// a relation to A the foreign key would get lost if we discard the row after building the B object. To
        /// not lose the foreign key it will be stored as part of the object.
        /// </summary>
        /// <param name="cl"></param>
        /// <param name="pc"></param>
        /// <param name="row"></param>
		void ReadLostForeignKeysFromRow(Class cl, IPersistenceCapable pc, DataRow row)
		{
			if (cl.FKColumnNames != null && pc.NDOLoadState != null)
			{
                //				Debug.WriteLine("GetLostForeignKeysFromRow " + pc.NDOObjectId.Dump());
				KeyValueList kvl = new KeyValueList(cl.FKColumnNames.Count());
				foreach(string colName in cl.FKColumnNames)
					kvl.Add(new KeyValuePair(colName, row[colName]));
				pc.NDOLoadState.LostRowInfo = kvl;				
			}
		}

		/// <summary>
		/// Writes information into the data row, which cannot be stored in the object.
		/// </summary>
		/// <param name="cl"></param>
		/// <param name="pc"></param>
		/// <param name="row"></param>
		protected virtual void WriteLostForeignKeysToRow(Class cl, IPersistenceCapable pc, DataRow row)
		{
			if (cl.FKColumnNames != null)
			{
				//				Debug.WriteLine("WriteLostForeignKeys " + pc.NDOObjectId.Dump());
				KeyValueList kvl = (KeyValueList)pc.NDOLoadState.LostRowInfo;
				if (kvl == null)
					throw new NDOException(88, "Can't find foreign keys for the relations of the object " + pc.NDOObjectId.Dump());
				foreach (KeyValuePair pair in kvl)
					row[(string) pair.Key] = pair.Value;
			}
		}

		void Row2Object(Class cl, IPersistenceCapable pc, DataRow row)
		{
			ReadObject(pc, row, cl.ColumnNames, 0);
			ReadTimeStamp(cl, pc, row);
			ReadLostForeignKeysFromRow(cl, pc, row);
			LoadRelated1To1Objects(pc, row);
			pc.NDOObjectState = NDOObjectState.Persistent;
		}


		/// <summary>
		/// Convert a data table to objects. Note that the table might only hold objects of the specified type.
		/// </summary>
		internal IList DataTableToIList(Type t, ICollection rows, bool hollow) 
		{
			IList queryResult = GenericListReflector.CreateList(t, rows.Count);
            if (rows.Count == 0)
                return queryResult;

			IList callbackObjects = new ArrayList();
			Class cl = GetClass(t);
            if (t.IsGenericTypeDefinition)
            {
                if (cl.TypeNameColumn == null)
                    throw new NDOException(104, "No type name column defined for generic type '" + t.FullName + "'. Check your mapping file.");
                IEnumerator en = rows.GetEnumerator();
                en.MoveNext();  // Move to the first element
                DataRow testrow = (DataRow)en.Current;
                if (testrow.Table.Columns[cl.TypeNameColumn.Name] == null)
                    throw new InternalException(3333, "DataTableToIList: TypeNameColumn isn't defined in the schema.");
            }

			foreach(DataRow row in rows) 
			{
                Type concreteType = t;
                if (t.IsGenericTypeDefinition)  // type information is in the row
                {
                    if (row[cl.TypeNameColumn.Name] == DBNull.Value)
                    {
                        ObjectId tempid = ObjectIdFactory.NewObjectId(t, cl, row, this.typeManager);
                        throw new NDOException(105, "Null entry in the TypeNameColumn of the type '" + t.FullName + "'. Oid = " + tempid.ToString());
                    }
                    string typeStr = (string)row[cl.TypeNameColumn.Name];
                    concreteType = Type.GetType(typeStr);
                    if (concreteType == null)
                        throw new NDOException(106, "Can't load generic type " + typeStr);
                }
				ObjectId id = ObjectIdFactory.NewObjectId(concreteType, cl, row, this.typeManager);
				IPersistenceCapable pc = cache.GetObject(id);                
				if(pc == null) 
				{
                    pc = CreateObject( concreteType );
                    pc.NDOObjectId = id;
					pc.NDOStateManager = sm;
					// If the object shouldn't be hollow, this will be overwritten later.
					pc.NDOObjectState = NDOObjectState.Hollow;
				}
				// If we have found a non hollow object, the time stamp remains the old one.
				// In every other case we use the new time stamp.
				// Note, that there could be a hollow object in the cache.
				// We need the time stamp in hollow objects in order to correctly
				// delete objects using fake rows.
				if (pc.NDOObjectState == NDOObjectState.Hollow)
				{
					ReadTimeStamp(cl, pc, row);
				}
				if(!hollow && pc.NDOObjectState != NDOObjectState.PersistentDirty) 
				{
					Row2Object(cl, pc, row);
					if ((pc as IPersistenceNotifiable) != null)
						callbackObjects.Add(pc);
				} 

				cache.UpdateCache(pc);
				queryResult.Add(pc);
			}
			// Make shure this is the last statement before returning
			// to the caller, so the user can recursively use persistent
			// objects
			foreach(IPersistenceNotifiable ipn in callbackObjects)
				ipn.OnLoaded();

			return queryResult;
		}

#endregion

#region Cache Management
		/// <summary>
		/// Remove all unused entries from the cache.
		/// </summary>
		public void CleanupCache() 
		{
			GC.Collect(GC.MaxGeneration);
			GC.WaitForPendingFinalizers();
			cache.Cleanup();
		}

		/// <summary>
		/// Remove all unlocked objects from the cache. Use with care!
		/// </summary>
		public void UnloadCache() 
		{
			cache.Unload();
		}
#endregion

		/// <summary>
		/// Default encryption key for NDO
		/// </summary>
		/// <remarks>
		/// We recommend strongly to use an own encryption key, which can be set with this property.
		/// </remarks>
		public virtual byte[] EncryptionKey
		{
			get 
			{
				if (this.encryptionKey == null)
					this.encryptionKey = new byte[] { 0x09,0xA2,0x79,0x5C,0x99,0xFF,0xCB,0x8B,0xA3,0x37,0x76,0xC8,0xA6,0x5D,0x6D,0x66,
													  0xE2,0x74,0xCF,0xF0,0xF7,0xEA,0xC4,0x82,0x1E,0xD5,0x19,0x4C,0x5A,0xB4,0x89,0x4D };
				return this.encryptionKey; 
			}
			set { this.encryptionKey = value; }
		}

		/// <summary>
		/// Hollow mode: If true all objects are made hollow after each transaction.
		/// </summary>
		public virtual bool HollowMode 
		{
			get { return hollowMode; }
			set { hollowMode = value; }
		}

		internal TypeManager TypeManager
		{
			get { return typeManager; }
		}

		/// <summary>
		/// Sets or gets transaction mode. Uses TransactionMode enum.
		/// </summary>
		/// <remarks>
		/// Set this value before you start any transactions.
		/// </remarks>
		public TransactionMode TransactionMode
		{
			get { return TransactionScope.TransactionMode; }
			set { TransactionScope.TransactionMode = value; }
		}

		/// <summary>
		/// Sets or gets the Isolation Level.
		/// </summary>
		/// <remarks>
		/// Set this value before you start any transactions.
		/// </remarks>
		public IsolationLevel IsolationLevel
		{
			get { return TransactionScope.IsolationLevel; }
			set { TransactionScope.IsolationLevel = value; }
		}

		internal class MappingTableEntry 
		{
			private IPersistenceCapable parentObject;
			private IPersistenceCapable relatedObject;
			private Relation relation;
			private bool deleteEntry;
			
			public MappingTableEntry(IPersistenceCapable pc, IPersistenceCapable relObj, Relation r) : this(pc, relObj, r, false) 
			{
			}
			
			public MappingTableEntry(IPersistenceCapable pc, IPersistenceCapable relObj, Relation r, bool deleteEntry) 
			{
				parentObject = pc;
				relatedObject = relObj;
				relation = r;
				this.deleteEntry = deleteEntry;
			}

			public bool DeleteEntry 
			{
				get 
				{
					return deleteEntry;
				}
			}

			public IPersistenceCapable ParentObject 
			{
				get 
				{
					return parentObject;
				}
			}

			public IPersistenceCapable RelatedObject 
			{
				get 
				{
					return relatedObject;
				}
			}

			public Relation Relation
			{
				get 
				{
					return relation;
				}
			}
		}

		/// <summary>
		/// Get a DataRow representing the given object.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public DataRow GetClonedDataRow( object o )
		{
			IPersistenceCapable pc = CheckPc( o );

			if (pc.NDOObjectState == NDOObjectState.Deleted || pc.NDOObjectState == NDOObjectState.Transient)
				throw new Exception( "GetDataRow: State of the object must not be Deleted or Transient." );

			DataRow row = cache.GetDataRow( pc );
			DataTable newTable = row.Table.Clone();
			newTable.ImportRow( row );
			row = newTable.Rows[0];

			Class cls = mappings.FindClass(o.GetType());
			WriteObject( pc, row, cls.ColumnNames );
			WriteForeignKeysToRow( pc, row );

			return row;
		}

		/// <summary>
		/// Gets an object, which shows all changes applied to the given object.
		/// This function can be used to build an audit system.
		/// </summary>
		/// <param name="o"></param>
		/// <returns>An ExpandoObject</returns>
		public ChangeLog GetChangeSet( object o )
		{
			var changeLog = new ChangeLog(this);
			changeLog.Initialize( o );
			return changeLog;
		}

		/// <summary>
		/// Outputs a revision number representing the assembly version.
		/// </summary>
		/// <remarks>This can be used for debugging purposes</remarks>
		public int Revision 
		{ 
			get 
			{
				Version v = new AssemblyName( GetType().Assembly.FullName ).Version;
				string vstring = String.Format( "{0}{1:D2}{2}{3:D2}", v.Major, v.Minor, v.Build, v.MinorRevision );
				return int.Parse( vstring );
			} 
		}
	}
}
