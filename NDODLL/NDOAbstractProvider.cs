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


#if nix
using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Collections;
using NDOInterfaces;

namespace NDO {
	/// <summary>
	/// Contains standard implementation for providers.
	/// </summary>
	public abstract class NDOAbstractProvider : IProvider {
		
	
		#region Implementation of IProvider
		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract System.Data.IDbConnection NewConnection(string parameters);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract System.Data.IDbCommand NewSqlCommand(System.Data.IDbConnection connection);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract System.Data.Common.DbDataAdapter NewDataAdapter(System.Data.IDbCommand select, System.Data.IDbCommand update, System.Data.IDbCommand insert, System.Data.IDbCommand delete);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract object NewCommandBuilder(System.Data.Common.DbDataAdapter dataAdapter);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract void AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, string columnName);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract void AddParameter(System.Data.IDbCommand command, string parameterName, object dbType, int size, System.Data.ParameterDirection dir, bool isNullable, byte precision, byte scale, string srcColumn, System.Data.DataRowVersion srcVersion, object value);

		
		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract object GetDbType(System.Type t);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract object GetDbType(string dbTypeName);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual int GetDefaultLength(string dbTypeName) 
		{
			return GetDefaultLength(GetSystemType(dbTypeName));			
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual int GetDefaultLength(System.Type t) 
		{
			if ( t == typeof(bool) )
				return 4;
			else if ( t == typeof(byte) )
				return 1;
			else if ( t == typeof(sbyte) )
				return 1;
			else if ( t == typeof(char) )
				return 2;
			else if ( t == typeof(short))
				return 2;
			else if ( t == typeof(ushort))
				return 2;
			else if ( t == typeof(int))
				return 4;
			else if ( t == typeof(uint))
				return 4;
			else if ( t == typeof(long))
				return 8;
			else if ( t == typeof(ulong))
				return 8;
			else if ( t == typeof(float))
				return 8;
			else if ( t == typeof(double))
				return 8;
			else if ( t == typeof(string))
				return 255;
			else if ( t == typeof(decimal))
				return 11;
			else if ( t == typeof(DateTime))
				return 4;
			else if ( t == typeof(Guid))
				return 16;
			else if ( t == typeof(Byte[]))
				return 1024;
			else if ( t.IsSubclassOf(typeof(System.Enum)))
				return 4;
			else
				return 0;
		}

		private static Hashtable dbTypes = null;

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual Type GetSystemType(string typeName) 
		{
			if (dbTypes == null) {
				dbTypes = new Hashtable(51);
				dbTypes.Add("BigInt", typeof(System.Int64));
				dbTypes.Add("Binary", typeof(System.Byte[]));
				dbTypes.Add("Bit", typeof(System.Boolean));
				dbTypes.Add("Char", typeof(System.Byte));
				dbTypes.Add("DateTime", typeof(System.DateTime));
				dbTypes.Add("Decimal", typeof(System.Decimal));
				dbTypes.Add("Float", typeof(System.Single));
				dbTypes.Add("Image", typeof(System.Byte[]));
				dbTypes.Add("Int", typeof(System.Int32));
				dbTypes.Add("Money", typeof(System.Decimal));
				dbTypes.Add("NChar", typeof(string));
				dbTypes.Add("NText", typeof(System.String));
				dbTypes.Add("NVarChar", typeof(System.String));
				dbTypes.Add("Real", typeof(System.Double));
				dbTypes.Add("SmallDateTime", typeof(System.DateTime));
				dbTypes.Add("SmallInt", typeof(System.Int16));
				dbTypes.Add("SmallMoney", typeof(System.Decimal));
				dbTypes.Add("Text", typeof(System.String));
				dbTypes.Add("Timestamp", typeof(System.DateTime));
				dbTypes.Add("TinyInt", typeof(System.Byte));
				dbTypes.Add("UniqueIdentifier", typeof(System.Guid));
				dbTypes.Add("VarBinary", typeof(System.Byte[]));
				dbTypes.Add("VarChar", typeof(System.String));
				dbTypes.Add("Variant", typeof(System.Object));
				dbTypes.Add("Boolean", typeof(System.Boolean));
				dbTypes.Add("BSTR", typeof(System.String));
				dbTypes.Add("Currency", typeof(System.Decimal));
				dbTypes.Add("Date", typeof(System.DateTime));
				dbTypes.Add("DBDate", typeof(System.DateTime));
				dbTypes.Add("DBTime", typeof(System.TimeSpan));
				dbTypes.Add("DBTimeStamp", typeof(System.DateTime));
				dbTypes.Add("Double", typeof(System.Double));
				dbTypes.Add("Empty", typeof(System.Object));
				dbTypes.Add("Error", typeof(System.Exception));
				dbTypes.Add("Filetime", typeof(System.DateTime));
				dbTypes.Add("IDispatch", typeof(System.Object));
				dbTypes.Add("IUnknown", typeof(System.Object));
				dbTypes.Add("LongVarBinary", typeof(System.Byte));
				dbTypes.Add("LongVarChar", typeof(System.String));
				dbTypes.Add("LongVarWChar", typeof(System.String));
				dbTypes.Add("Numeric", typeof(System.Decimal));
				dbTypes.Add("PropVariant", typeof(System.Object));
				dbTypes.Add("Single", typeof(System.Single));
				dbTypes.Add("UnsignedBigInt", typeof(System.UInt64));
				dbTypes.Add("UnsignedInt", typeof(System.UInt32));
				dbTypes.Add("UnsignedSmallInt", typeof(System.UInt16));
				dbTypes.Add("UnsignedTinyInt", typeof(System.Byte));
				dbTypes.Add("VarNumeric", typeof(System.Decimal));
				dbTypes.Add("VarWChar", typeof(System.String));
				dbTypes.Add("WChar", typeof(System.String));
			}
			Type result = (Type)dbTypes[typeName];
			if(result == null)
				throw new NDOException(14, "Can't convert " + typeName + " into a .NET type.");
			return result;
		}

	
		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string Wildcard
		{
			get { return "%"; }
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool UseNamedParams
		{
			get { return false; }
		}


		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string NamedParamPrefix
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool UseStoredProcedure
		{
			get { return false; }
		}

	
		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GetNamedParameter(string plainParameterName)
		{
			return plainParameterName;
		}
	
		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GetQuotedName(string plainName)
		{
			return plainName;
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GetSqlLiteral(object o)
		{
			if (o is string || o.GetType().IsSubclassOf(typeof(string)) || o is Guid)
				return "'" + o.ToString() + "'";
			if (o is byte[])
			{
				StringBuilder sb = new StringBuilder(((byte[])o).Length * 2 + 2);
				sb.Append('\'');
				foreach (byte b in (byte[])o)
				{
					sb.Append(b.ToString("X2"));
				}
				sb.Append('\'');
				return sb.ToString();
			}
			return o.ToString();
		}


		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool SupportsLastInsertedId 
		{
			get {return false;}
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GetLastInsertedId
		{
			get { return null; }
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool SupportsBulkCommands 
		{
			get { return false; }
		}
		

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual string GenerateBulkCommand(string[] commands)
		{
			return null;
		}


		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual int MaxBulkCommandLength 
		{
			get { return 0; }
		}


		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract string[] TypeNames { get; }

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public string[] GetTableNames (IDbConnection conn)
		{
			return GetTableNames(conn, null);
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract string[] GetTableNames(IDbConnection conn, string owner);

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual bool SupportsInsertBatch
		{
			get { return false; }
		}

		/// <summary>
		/// See <see cref="NDO.IProvider">IProvider interface</see>.
		/// </summary>
		public virtual void RegisterRowUpdateHandler(IRowUpdateListener handler)
		{
		}

		public abstract bool SupportsNativeGuidType { get; }

		#endregion
	}
}
#endif