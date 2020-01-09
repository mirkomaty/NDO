//
// Copyright (c) 2002-2020 Mirko Matytschak 
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NDO;
using NDO.ShortId;
using NDO.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NDO.JsonFormatter
{
	public class NdoJsonFormatter : IFormatter
	{
		public ISurrogateSelector SurrogateSelector { get; set; }
		public SerializationBinder Binder { get; set; }
		public StreamingContext Context { get; set; }

		PersistenceManager pm;


		public NdoJsonFormatter()
		{
			this.pm = null;
		}

		public NdoJsonFormatter( PersistenceManager pm )
		{
			this.pm = pm;
		}

		public IPersistenceCapable DeserializeObject( JToken jobj )
		{
			var shortId = (string)jobj["_oid"];

			if (shortId != null)
			{
				var pc = this.pm.FindObject(shortId);
				pc.NDOStateManager = null;  // Detach object
				pc.FromJToken( jobj );
				return pc;
			}

			return null;
		}

		public IPersistenceCapable DeserializeHollowObject(JToken jobj)
		{
			var shortId = (string)jobj["_oid"];

			if (shortId != null)
			{
				var pc = this.pm.FindObject(shortId);
				pc.NDOStateManager = null;  // Detach object
				return pc;
			}

			return null;
		}

		public RelationChangeRecord DeserializeRelationChangeRecord(JToken jobj)
		{
			var parent = this.pm.FindObject((string)jobj["parent"]["_oid"]);
			parent.NDOStateManager = null;  // Detach object
			var child = this.pm.FindObject((string)jobj["child"]["_oid"]);
			child.NDOStateManager = null;  // Detach object

			var relationName = (string)jobj["relationName"];
			var isAdded = (bool)jobj["isAdded"];
			return new RelationChangeRecord(parent, child, relationName, isAdded);
		}

		void FixRelations( JToken jObj, IPersistenceCapable e, List<IPersistenceCapable> rootObjects, List<IPersistenceCapable> additionalObjects )
		{
			var t = e.GetType();
			FieldMap fm = new FieldMap(t);
			var mc = Metaclasses.GetClass(t);
			foreach (var fi in fm.Relations)
			{
				var token = jObj[fi.Name];
				if (token == null)
					continue;
				bool isArray = typeof(IList).IsAssignableFrom( fi.FieldType );
				if (token is JArray jarray)
				{
					if (isArray)
					{
						IList container = (IList)fi.GetValue(e);						
						if (container == null)
							throw new NDOException( 20002, $"Container object of relation {t.Name}.{fi.Name} is not initialized. Please initialize the field in your class constructor." );

						container.Clear();
						foreach (var relJObj in jarray)
						{
							container.Add( DeserializeObject( relJObj ) );
						}
					}
				}
				else
				{
					if (!isArray)
					{
						if (token.Type == JTokenType.Null)
							fi.SetValue( e, null );
						else
							fi.SetValue( e, DeserializeObject( token ) );
					}
				}
			}
		}

		IList DeserializeChangeSetContainer(JToken rootArray)
		{
			// 0: AddedObjects = new List<IPersistenceCapable>();
			// 1: DeletedObjects = new List<ObjectId>();
			// 2: ChangedObjects = new List<IPersistenceCapable>();
			// 3: RelationChanges = new List<RelationChangeRecord>();

			ArrayList arrayList = new ArrayList(new object[4]);

			for (int i = 0; i < 4; i++)
			{
				var partialArray = rootArray[i];
				if (i == 0 || i == 2)
				{
					var partialList = new List<IPersistenceCapable>();
					arrayList[i] = partialList;
					foreach (var item in partialArray)
					{
						partialList.Add(DeserializeObject(item));
					}
				}
				if (i == 1)
				{
					var partialList = new List<IPersistenceCapable>();
					arrayList[i] = partialList;
					foreach (var item in partialArray)
					{
						partialList.Add(DeserializeHollowObject(item));
					}
				}
				if (i == 3)
				{
					var partialList = new List<RelationChangeRecord>();
					arrayList[i] = partialList;
					foreach (var item in partialArray)
					{
						partialList.Add(DeserializeRelationChangeRecord(item));
					}
				}
			}

			return arrayList;
		}

		object DeserializeRootArray( JToken rootArray )
		{
			var rootObjectsToken = (JArray)rootArray["rootObjects"];
			var additionalObjectsToken = (JArray)rootArray["additionalObjects"];
			if (rootObjectsToken.Count >= 4 && rootObjectsToken[0] is JArray)
				return DeserializeChangeSetContainer(rootObjectsToken);

			List<IPersistenceCapable> rootObjects = new List<IPersistenceCapable>();
			List<IPersistenceCapable> additionalObjects = new List<IPersistenceCapable>();

			foreach (var jObj in rootObjectsToken)
			{
				IPersistenceCapable e = DeserializeObject(jObj);
				if (e != null)
					rootObjects.Add( e );
			}

			foreach (var jObj in additionalObjectsToken)
			{
				IPersistenceCapable e = DeserializeObject(jObj);
				if (e != null)
					additionalObjects.Add( e );
			}

			foreach (var jObj in rootObjectsToken)
			{
				var shortId = (string)jObj["_oid"];
				IPersistenceCapable e = rootObjects.First(o=>((IPersistenceCapable)o).ShortId() == shortId);
				FixRelations( jObj, e, rootObjects, additionalObjects );
			}

			foreach (var jObj in additionalObjectsToken)
			{
				var shortId = (string)jObj["_oid"];
				IPersistenceCapable e = additionalObjects.First(o=>((IPersistenceCapable)o).ShortId() == shortId);
				FixRelations( jObj, e, rootObjects, additionalObjects );
			}

			return new ArrayList( rootObjects );
		}

		class Metaclasses
		{
			private static Hashtable theClasses = new Hashtable();

			internal static IMetaClass GetClass( Type t )
			{
				if (t.IsGenericTypeDefinition)
					return null;

				IMetaClass mc;

				lock (theClasses)
				{
					if (null == ( mc = (IMetaClass) theClasses[t] ))
					{
						Type mcType = t.GetNestedType("MetaClass", BindingFlags.NonPublic | BindingFlags.Public);
						if (null == mcType)
							throw new NDOException( 13, "Missing nested class 'MetaClass' for type '" + t.Name + "'; the type doesn't seem to be enhanced." );
						Type t2 = mcType;
						if (t2.IsGenericTypeDefinition)
							t2 = t2.MakeGenericType( t.GetGenericArguments() );
						mc = (IMetaClass) Activator.CreateInstance( t2 );
						theClasses.Add( t, mc );
					}
				}

				return mc;
			}
		}

		public object Deserialize( Stream serializationStream )
		{
			if (this.pm == null)
				throw new NDOException( 20001, "PersistenceManager is not initialized. Provide a PersistenceManager in the formatter constructor." );

			JsonSerializer serializer = new JsonSerializer();
			TextReader textReader = new StreamReader( serializationStream );
			var rootObject = (JToken)serializer.Deserialize(textReader, typeof(JToken));
			if (rootObject == null || rootObject.Type == JTokenType.Null)
				return null;
			var result = DeserializeRootArray( rootObject );
			this.pm.UnloadCache();
			return result;
		}

		IDictionary<string, object> MakeDict( IPersistenceCapable pc )
		{
			var dict = pc.ToDictionary(pm);
			var shortId = ((IPersistenceCapable)pc).ShortId();
			var t = pc.GetType();
			FieldMap fm = new FieldMap(t);
			var mc = Metaclasses.GetClass(t);
			foreach (var fi in fm.Relations)
			{
                var fiName = fi.Name;
				if (( (IPersistenceCapable) pc ).NDOGetLoadState( mc.GetRelationOrdinal( fiName ) ))
				{
					object relationObj = fi.GetValue(pc);
					if (relationObj is IList list)
					{
						List<object> dictList = new List<object>();
						foreach (IPersistenceCapable relObject in list)
						{
							shortId = ( (IPersistenceCapable) relObject ).ShortId();
							dictList.Add( new { _oid = shortId } );
						}
						dict.Add( fiName, dictList );
					}
					else
					{
                        // Hollow object means, that we don't want to transfer the object to the other side.
						if (relationObj == null || ((IPersistenceCapable)relationObj).NDOObjectState == NDOObjectState.Hollow)
						{
							dict.Add( fiName, null );
						}
						else
						{
							IPersistenceCapable relIPersistenceCapable = (IPersistenceCapable) relationObj;
							shortId = ( (IPersistenceCapable) relIPersistenceCapable ).ShortId();
							dict.Add( fiName, new { _oid = shortId } );
						}
					}
				}
			}

			return dict;
		}

		void InitializePm( object graph )
		{
			IPersistenceCapable pc;
			if (graph is IList list)
			{
				if (list.Count == 0)
					return;
				pc = (IPersistenceCapable) list[0];
			}
			else
			{
				pc = (IPersistenceCapable) graph;
			}

			this.pm = (PersistenceManager) ( pc.NDOStateManager.PersistenceManager );
		}

		void RecursivelyAddAdditionalObjects( IPersistenceCapable e, List<IPersistenceCapable> rootObjects, List<IPersistenceCapable> additionalObjects )
		{
			var t = e.GetType();
			FieldMap fm = new FieldMap(t);
			var mc = Metaclasses.GetClass(t);
			foreach (var fi in fm.Relations)
			{
				if (( (IPersistenceCapable) e ).NDOGetLoadState( mc.GetRelationOrdinal( fi.Name ) ))
				{
					object relationObj = fi.GetValue(e);
					if (relationObj is IList list)
					{
						List<object> dictList = new List<object>();
						foreach (IPersistenceCapable relIPersistenceCapable in list)
						{
							if (!rootObjects.Contains( relIPersistenceCapable ) && !additionalObjects.Contains( relIPersistenceCapable ))
							{
								additionalObjects.Add( relIPersistenceCapable );
								RecursivelyAddAdditionalObjects( relIPersistenceCapable, rootObjects, additionalObjects );
							}
						}
					}
					else
					{
                        // Hollow object means, that we don't want to transfer the object to the other side.
                        if (relationObj != null && ((IPersistenceCapable)relationObj).NDOObjectState != NDOObjectState.Hollow)
						{
							IPersistenceCapable relIPersistenceCapable = (IPersistenceCapable) relationObj;
							if (!rootObjects.Contains( relIPersistenceCapable ) && !additionalObjects.Contains( relIPersistenceCapable ))
							{
								additionalObjects.Add( relIPersistenceCapable );
								RecursivelyAddAdditionalObjects( relIPersistenceCapable, rootObjects, additionalObjects );
							}
						}
					}
				}
			}
		}

		object MapRelationChangeRecord(RelationChangeRecord rcr)
		{
			return new
			{
				parent = new { _oid = rcr.Parent.NDOObjectId.ToShortId() },
				child  = new { _oid = rcr.Child.NDOObjectId.ToShortId() },
				isAdded = rcr.IsAdded,
				relationName = rcr.RelationName,
				_oid = "RelationChangeRecord"
			};
		}

		void SerializeChangeSet(Stream serializationStream, IList graph)
		{
			// A ChangeSetContainer consists of 4 lists of deleted, added, changed objects, and the relation changes.
			// AddedObjects = new List<IPersistenceCapable>();
			// DeletedObjects = new List<ObjectId>();
			// ChangedObjects = new List<IPersistenceCapable>();
			// RelationChanges = new List<RelationChangeRecord>();
			List<List<object>> resultObjects = new List<List<object>>(graph.Count);
			foreach (IList list in graph)
			{
				List<object> partialResult = new List<object>();
				resultObjects.Add(partialResult);
				foreach (var item in list)
				{
					if (item is IPersistenceCapable pc)
						partialResult.Add(MakeDict(pc));
					else if (item is ObjectId oid)
						partialResult.Add(new { _oid = oid.ToShortId() });
					else if (item is RelationChangeRecord rcr)
						partialResult.Add(MapRelationChangeRecord(rcr));
					else
						throw new NDOException(20003, $"NDOJsonFormatter: unknown element of type {item.GetType().FullName} in ChangeSetContainer.");
				}
			}

			var json = JsonConvert.SerializeObject(new { rootObjects = resultObjects, additionalObjects = new object[] { } });
			var byteArray = Encoding.UTF8.GetBytes(json);
			serializationStream.Write(byteArray, 0, byteArray.Length);
		}

		public void Serialize( Stream serializationStream, object graph )
		{
			string json = null;
			if (this.pm == null)
				InitializePm( graph );
			List<object> rootObjects = new List<object>();
			List<object> additionalObjects = new List<object>();
			List<IPersistenceCapable> rootObjectList = new List<IPersistenceCapable>();
			List<IPersistenceCapable> additionalObjectList = new List<IPersistenceCapable>();

			IList list = graph as IList;
			if (list != null)
			{
				if (list.Count > 0 && list[0] is IList)
				{
					// Change Set
					SerializeChangeSet(serializationStream, list);
					return;
				}
				foreach (IPersistenceCapable e in list)
				{
					rootObjectList.Add( e );
				}
			}
			else if (graph is IPersistenceCapable e)
			{
				rootObjectList.Add( e );
			}

			foreach (var e in rootObjectList)
			{
				RecursivelyAddAdditionalObjects( e, rootObjectList, additionalObjectList );
			}

			foreach (var e in rootObjectList)
			{
				rootObjects.Add( MakeDict( e ) );
			}

			foreach (var e in additionalObjectList)
			{
				additionalObjects.Add( MakeDict( e ) );
			}

			json = JsonConvert.SerializeObject( new { rootObjects, additionalObjects } );
			var byteArray = Encoding.UTF8.GetBytes(json);
			serializationStream.Write( byteArray, 0, byteArray.Length );
		}
	}
}
