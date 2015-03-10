//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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


using System;
using System.Runtime.Serialization;
using System.Data;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// Common base class for all keys. One object of this class is shared by all ObjectIds, refering to the same object.
	/// </summary>
	public abstract class Key
	{
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


        public abstract void ToRow(Class cl, DataRow row);

        public abstract void FromRow(Class cl, DataRow row);

        public abstract void Serialize(SerializationInfo info, StreamingContext context);

        public abstract void Deserialize(SerializationInfo info, StreamingContext context);

        public abstract void ToForeignKey(Relation relation, DataRow row);

        public virtual object this[int index]
        {
            get { throw new InternalException(59, "Key.this: Class " + this.GetType().FullName + " doesn't support access to the primary key column values."); }
            set { throw new InternalException(59, "Key.this: Class " + this.GetType().FullName + " doesn't support access to the primary key column values."); }
        }

        public static Key NewKey(SerializationInfo info, StreamingContext context)
        {
            Type keyType = (Type)info.GetValue("KeyType", typeof(Type));
            Key newKey = (Key) Activator.CreateInstance(keyType);
            newKey.t = (Type)info.GetValue("ObjectType", typeof(Type));
            newKey.Deserialize(info, context);
            return newKey;
        }

#if PRO
		/// <summary>
		/// Retrieve the unique type identifier of the persistent object.
		/// </summary>
		public int TypeId {
			get { return typeManager[t]; }
		}
#endif
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
		/// Generate hash code
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode() 
		{
			return t.GetHashCode();
		}

        public abstract Key Clone();

        public static Key NewKey(Key template, Type t)
        {
            Key newKey = template.Clone();
            newKey.t = t;
            return newKey;
        }


        /// <summary>
        /// Gets or sets the Key value of the first oid column. This property is equivalent to 
        /// key[0];
        /// </summary>
        /// <remarks>
        /// Use this property only, if you don't use multi-column primary keys. Otherwise use this[int].
        /// </remarks>
        public abstract object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a copy of the Key values.
        /// </summary>
        public abstract object[] Values
        {
            get;
        }
	}
}
