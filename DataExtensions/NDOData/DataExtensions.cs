using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Dynamic;

using NDO;

namespace NDOData
{
    public static class DataExtensions
    {
		/// <summary>
		/// Gets a DataRow containing the values of a persistent object.
		/// </summary>
		/// <param name="pm"></param>
		/// <param name="o"></param>
		/// <returns>A DataRow representing the object state.</returns>
		/// <remarks>If the object has been changed (NDOObjectState == PersistentDirty) the DataRow contains both the original and current state of the object.</remarks>
		public static DataRow GetDataRow(this PersistenceManager pm, object o)
		{
			IPersistenceCapable pc = CheckPc( o );
			if (pc.NDOObjectState == NDOObjectState.Deleted || pc.NDOObjectState == NDOObjectState.Transient)
				throw new Exception( "GetDataRow: State of the object must not be Deleted or Transient." );

			FieldInfo fi = pm.GetType().GetField( "cache", BindingFlags.Instance | BindingFlags.NonPublic );
			if (fi == null)
				throw new Exception( String.Format( "PersistenceManager of type {0} doesn't provide a field 'cache'", pm.GetType().FullName ) );

			object cache = fi.GetValue( pm );

			MethodInfo mi = cache.GetType().GetMethod( "GetDataRow" );

			if (mi == null)
				throw new Exception( String.Format( "Cache of type {0} doesn't provide a method 'GetDataRow'", cache.GetType().FullName ) );

			DataRow row = (DataRow) mi.Invoke( cache, new object[] { pc } );
			DataTable newTable = row.Table.Clone();
			newTable.ImportRow( row );
			row = newTable.Rows[0];

			NDO.Mapping.Class cls = pm.NDOMapping.FindClass(o.GetType());
			pc.NDOWrite( row, cls.FieldNames, 0 );

			return row;
		}

		/// <summary>
		/// Gets an object representing the original values of all changed values of an object.
		/// </summary>
		/// <param name="pm"></param>
		/// <param name="o"></param>
		/// <returns></returns>
		public static ExpandoObject GetChangeSet(this PersistenceManager pm, object o)
		{
			IPersistenceCapable pc = CheckPc( o );

			ExpandoObject result = new ExpandoObject();
			IDictionary<string, object> values = (IDictionary<string, object>)result;

			// No changes
			if (pc.NDOObjectState == NDOObjectState.Hollow || pc.NDOObjectState == NDOObjectState.Persistent)
			{
				return result;
			}

			// Created: No Changes, but mark the change set
			if (pc.NDOObjectState == NDOObjectState.Created)
			{
				values.Add( "_ndoAdded", true );
				return result;
			}

			DataRow row = GetDataRow( pm, o );

			NDO.Mapping.Class cls = pm.NDOMapping.FindClass(o.GetType());

			foreach (NDO.Mapping.Field field in cls.Fields)
			{
				string colName = field.Column.Name;
				object currentVal = row[colName, DataRowVersion.Current];
				object originalVal = row[colName, DataRowVersion.Original];

				if (!currentVal.Equals( originalVal ))
					values.Add( field.Name, originalVal );
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="changeSets"></param>
		/// <param name="o"></param>
		/// <returns></returns>
		//public static ExpandoObject GetOriginalVersion(List<object> changeSets, object o)
		//{
		//	IPersistenceCapable pc = CheckPc( o );
			
		//}

		private static IPersistenceCapable CheckPc(object o)
		{
			IPersistenceCapable pc = o as IPersistenceCapable;
			if (pc == null && !(o == null))
				throw new NDOException(31, "DataExtensions.GetDataRow: Parameter should implement IPersistenceCapable. Check, if the type " + o.GetType().FullName + "," + o.GetType().Assembly.FullName + " is enhanced.");
			return pc;
		}
    }
}
