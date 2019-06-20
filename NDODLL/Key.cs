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
using System.Runtime.Serialization;
using System.Data;
using NDO.Mapping;
using System.Reflection;
using System.Text;
using NDO.ShortId;

namespace NDO
{
	/// <summary>
	/// Common base class for all keys. One object of this class is shared by all ObjectIds, refering to the same object.
	/// </summary>
	public abstract class Key
	{
		/// <summary>
		/// The type of the object
		/// </summary>
		protected Type t;
		private TypeManager typeManager;

		/// <summary>
		/// Gets the TypeManager of the key which originates in the pm
		/// </summary>
		internal TypeManager TypeManager
		{
			get { return this.typeManager; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="t">Persistent class type</param>
		/// <param name="typeManager">The TypeManager to get the type code</param>
		internal Key(Type t, TypeManager typeManager)
		{
			this.typeManager = typeManager;
			this.t = t;
		}

		/// <summary>
		/// Mark a key as invalid
		/// </summary>
		public virtual void Invalidate() {
			t = null;
		}

		/// <summary>
		/// Retrieve the type of the persistent object the key belongs to
		/// </summary>
		public Type Type {
			get { return t; }
		}

		/// <summary>
		/// Writes the key data into the DataRow using the class mapping information.
		/// </summary>
		/// <param name="cl"></param>
		/// <param name="row"></param>
		public abstract void ToRow(Class cl, DataRow row);

		/// <summary>
		/// Initializes the key data from the given DataRow using the class mapping information.
		/// </summary>
		/// <param name="cl"></param>
		/// <param name="row"></param>
		public abstract void FromRow(Class cl, DataRow row);

		/// <summary>
		/// Serializes the key values
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public abstract void Serialize(SerializationInfo info, StreamingContext context);

		/// <summary>
		/// Deserializes a key
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public abstract void Deserialize(SerializationInfo info, StreamingContext context);

		/// <summary>
		/// Writes the key data as foreign key to a DataRow
		/// </summary>
		/// <param name="relation"></param>
		/// <param name="row"></param>
        public abstract void ToForeignKey(Relation relation, DataRow row);

		/// <summary>
		/// Gets key data.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
        public virtual object this[int index]
        {
            get { throw new InternalException(59, "Key.this: Class " + this.GetType().FullName + " doesn't support access to the primary key column values."); }
            set { throw new InternalException(59, "Key.this: Class " + this.GetType().FullName + " doesn't support access to the primary key column values."); }
        }

		/// <summary>
		/// Creates a new key from a serialized stream
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <returns></returns>
        public static Key NewKey(SerializationInfo info, StreamingContext context)
        {
            Type keyType = (Type)info.GetValue("KeyType", typeof(Type));
            Key newKey = (Key) Activator.CreateInstance(keyType);
            newKey.t = (Type)info.GetValue("ObjectType", typeof(Type));
            newKey.Deserialize(info, context);
            return newKey;
        }

		/// <summary>
		/// Retrieve the unique type identifier of the persistent object.
		/// </summary>
		public int TypeId {
			get { return typeManager[t]; }
		}

		/// <summary>
		/// Test if two keys are equal
		/// </summary>
		/// <param name="obj">Object to compare with</param>
		/// <returns>True if keys are of the same host type and Key value</returns>
		public override bool Equals(object obj) 
		{
			Key k = obj as Key;
			return (object)k != null && t == k.t;
		}

        /// <summary>
        /// Overridden Operator ==, to make sure, that Equals is called. 
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator ==(Key o1, Key o2)
        {
            // Must cast to object, otherwise we get a recursive call to operator ==
            if (((object)o1) == null && ((object)o2) == null)
                return true;
            if (((object)o1) == null)
                return false;
            return o1.Equals(o2);
        }

        /// <summary>
        /// Overridden Operator !=, to make sure, that Equals is called.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator !=(Key o1, Key o2)
        {
            return !(o1 == o2);
        }

		/// <summary>
		/// Overridden cast operator
		/// </summary>
		/// <param name="key">A key instance</param>
		public static implicit operator object[]( Key key )  // explicit byte to digit conversion operator
		{
			return key?.Values;
		}

		/// <summary>
		/// Generate hash code
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode() 
		{
			return t.GetHashCode();
		}

		/// <summary>
		/// Clones a key
		/// </summary>
		/// <returns></returns>
        public abstract Key Clone();

		/// <summary>
		/// Creates a key from another key
		/// </summary>
		/// <param name="template"></param>
		/// <param name="t"></param>
		/// <returns></returns>
        public static Key NewKey(Key template, Type t)
        {
            Key newKey = template.Clone();
            newKey.t = t;
            return newKey;
        }


		/// <summary>
		/// Gets the Key value of the first oid column. This property is equivalent to 
		/// key[0];
		/// </summary>
		/// <remarks>
		/// This property is for convenience, since most objects have only one key value. Use this property only, if your object doesn't use a multi-column primary key. Otherwise use this[int].
		/// </remarks>
		public abstract object Value
        {
            get;
        }

        /// <summary>
        /// Gets a copy of the Key values.
        /// </summary>
        public abstract object[] Values
        {
            get;
        }

		/// <summary>
		/// Returns a string representation of the Key object.
		/// </summary>
		/// <returns>A Json-String</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("[");
			foreach (object v in Values)
			{
				if (v is string || v is Guid)
					sb.Append( '"' );
				sb.Append( v.ToString() );
				if (v is string || v is Guid)
					sb.Append( '"' );
				sb.Append( "," );
			}
			sb.Length -= 1;
			sb.Append( ']' );

			StringBuilder sb2 = new StringBuilder();
			sb2.Append( "{\"Type\":\"" );
			sb2.Append( Type.FullName );
			sb2.Append( "," );
			sb2.Append( new AssemblyName( Type.Assembly.FullName ).Name );
			sb2.Append( "\",\"Values\":" );
			sb2.Append( sb.ToString() );
			sb2.Append( "}" );
			return sb2.ToString();
		}

		/// <summary>
		/// Serializes the ObjectId to a ShortId, which can be used to identify an object of any type.
		/// </summary>
		/// <returns>A string consisting of the type information and the oid of the object.</returns>
		internal virtual string ToShortId()
		{
			StringBuilder valueSb = new StringBuilder();
			foreach (var value in Values)
			{
				Type oidType = value.GetType();

				if (oidType != typeof( int ) && oidType != typeof( Guid ) && oidType != typeof( string ))
					throw new Exception( "The oid type of the object does not allow the indication by a ShortId: " + oidType.FullName );

				if (oidType == typeof(string) && ((string)value).IndexOf(' ') > -1)
					throw new Exception( "The string oid value contains a ' ' char. It can't be converted to a ShortId. Type: " + oidType.FullName );

				if (valueSb.Length > 0)
					valueSb.Append( ' ' );  // Will be convertet to '+' by Encode()

				if (value is int i)
					valueSb.Append( i < 0 ? "?" : i.ToString() );
				else
					// In case of a Guid the '-' chars should be removed.
					// Guid.Parse can parse a string without these dashes.
					valueSb.Append( Value.ToString().Replace( "-", "" ) );
			}

			return (Type.Name + "-" + this.typeManager[Type].ToString( "X8" ) + "-" + valueSb.ToString()).Encode();
		}
	}
}
