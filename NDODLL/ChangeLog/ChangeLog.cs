using NDO.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Unity;

namespace NDO.ChangeLogging
{
	/// <summary>
	/// This class represents changes to objects which can be serialized to an audit log.
	/// </summary>
	public class ChangeLog
	{
		/// <summary>
		/// The current objects.
		/// </summary>
		/// <remarks>This member is public because it should be serialized. Change it only if you know what you're doing.</remarks>
		public IDictionary<string,object> current = new Dictionary<string, object>();
		/// <summary>
		/// The original objects.
		/// </summary>
		/// <remarks>This member is public because it should be serialized. Change it only if you know what you're doing.</remarks>
		public IDictionary<string,object> original = new Dictionary<string, object>();
		private readonly PersistenceManager pm;

		/// <summary>
		/// Initializes the ChangeLog object.
		/// </summary>
		/// <param name="pm"></param>
		public ChangeLog(PersistenceManager pm)
		{			
			this.pm = pm;
		}

		/// <summary>
		/// Initializes the ChangeLog from an object
		/// </summary>
		/// <param name="o"></param>
		public void Initialize(object o)
		{
			IPersistenceCapable pc = pm.CheckPc( o );

			// No changes
			if (pc.NDOObjectState == NDOObjectState.Hollow || pc.NDOObjectState == NDOObjectState.Persistent)
			{
				return;
			}

			DataRow row = pm.GetClonedDataRow( o );

			NDO.Mapping.Class cls = pm.NDOMapping.FindClass(o.GetType());

			foreach (Field field in cls.Fields)
			{
				string colName = field.Column.Name;
				object currentVal = row[colName, DataRowVersion.Current];
				object originalVal = row[colName, DataRowVersion.Original];

				if (!currentVal.Equals( originalVal ))
				{
					original.Add( field.Name, originalVal );
					current.Add( field.Name, currentVal );
				}
			}

			var objRelationChanges = pm.RelationChanges.Where( ce => ce.Parent.NDOObjectId == pc.NDOObjectId ).GroupBy(ce=>ce.RelationName).ToList();
			if (objRelationChanges.Count > 0)
			{
				var relStates = pm.CollectRelationStates( pc, row );
				foreach (var group in objRelationChanges)
				{
					string fieldName = group.Key;
					object fieldValue = (from rs in relStates where rs.Key.FieldName == fieldName select rs.Value).SingleOrDefault();
					var relCurrent = new ObjectIdList();
					if (fieldValue is IList l)
					{
						foreach (IPersistenceCapable item in l)
						{
							relCurrent.Add( item.NDOObjectId );
						}
					}
					else
					{
						if (fieldValue != null)
							relCurrent.Add( ( (IPersistenceCapable) fieldValue ).NDOObjectId );
					}

					// Make a copy
					var relOriginal = relCurrent.Clone();

					// In order to get the original array, we remove added objects
					// and add removed objects
					foreach (var changeEntry in group)
					{
						if (changeEntry.IsAdded)
						{
							var oid = changeEntry.Child.NDOObjectId;
							if (relOriginal.Contains( oid ))
								relOriginal.Remove( oid );
						}
						else
						{
							relOriginal.Add( changeEntry.Child.NDOObjectId );
						}
					}
					original.Add( fieldName, relOriginal );
					current.Add( fieldName, relCurrent );
				}
			}
		}

		/// <summary>
		/// Gets a serializable clone of the object, in which all ObjectId objects are replaced by ShortIds.
		/// </summary>
		/// <returns></returns>
		public ChangeLog SerializableClone()
		{
			ChangeLog result = new ChangeLog(this.pm);
			result.current = new Dictionary<string, object>( this.current );
			result.original = new Dictionary<string, object>( this.original );
			foreach (var kvp in result.current.ToList())
			{
				if (result.current[kvp.Key] is ObjectIdList oidList)
				{
					result.current[kvp.Key] = oidList.ToStringList();
				}
			}
			foreach (var kvp in result.original.ToList())
			{
				if (result.original[kvp.Key] is ObjectIdList oidList)
				{
					result.original[kvp.Key] = oidList.ToStringList();
				}
			}
			return result;
		}

		/// <summary>
		/// Serializes the ChangeLog to a string using the IChangeLogConverter implementation
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var changeLogConverter = pm.ConfigContainer.Resolve<IChangeLogConverter>();
			if (changeLogConverter == null)
				throw new Exception( "Can't serialize the ChangeLog. Please register an implementation of IChangeLogConverter" );

			return changeLogConverter.ToString( SerializableClone() );
		}
	}
}
