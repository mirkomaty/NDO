using NDO.Mapping;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Query
{
	/// <summary>
	/// Helper class to define sorting of the resultset of a query
	/// </summary>
	public abstract class QueryOrder
	{
		string orderType;
		string fieldName;

		/// <summary>
		/// If true, the order object represents an ascending order, if false, the 
		/// order object represents an descending order.
		/// </summary>
		public abstract bool IsAscending
		{
			get;
		}

		/// <summary>
		/// Gets the field name of the order object.
		/// </summary>
		public string FieldName
		{
			get { return fieldName; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ot">Order type, which is ASC or DESC</param>
		/// <param name="fn">Field name to be used to sort the data</param>
		public QueryOrder( string ot, string fn )
		{
			orderType = ot;
			fieldName = fn;
		}

		/// <summary>
		/// Returns the Order By clause string
		/// </summary>
		/// <returns>Order By clause string</returns>
		public string ToString( Class cl )
		{
			IProvider provider = cl.Provider;
			Column column = null;
			if (string.Compare( fieldName, "oid", true ) == 0)
			{
				//TODO: support multiple columns
				column = cl.Oid.OidColumns[0];
			}
			else
			{
				Field field = cl.FindField( fieldName );
				if (field == null)
					throw new NDOException( 7, "Can't find mapping information for field " + cl.FullName + "." + fieldName );
				column = field.Column;
			}
			if (provider == null)
				return cl.TableName + "." + column.Name + " " + orderType;
			else
				return QualifiedTableName.Get( cl.TableName, provider ) + "." + provider.GetQuotedName( column.Name ) + " " + orderType;

		}
	}
}
