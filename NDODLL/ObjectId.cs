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
namespace NDO
{
    /// <summary>
    /// Generic class for object ids; When a new object is created it gets a unique object id. 
    /// </summary>
    /// <remarks>
    /// The id might be overwritten when the object is written to a database. Therefore,
    /// the object id is a handle class to an internal id that will be updated. All object ids of the
    /// same object point to the same internal id.
    /// All object ids are typesafe, e.g. two ids are only equal if they reference an object with the same key and
    /// the same type (i.e. two different classes might have the same key value, but the object ids are different because they are
    /// different types). That's why all key classes must be derived from the common Key base class, which encapsulates the
    /// type of the object.
    /// </remarks>
    [Serializable]
    public sealed class ObjectId : ICloneable, ISerializable
    {
        private Key id;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Construct an object id with the given key</param>
        public ObjectId(Key id)
        {
            this.id = id;
        }

        /// <summary>
        /// Construct an object id using another object id
        /// </summary>
        /// <param name="oi"></param>
        public ObjectId(ObjectId oi)
        {
            id = oi.id;
        }

        /// <summary>
        /// Construct an ObjectId using an existing key and a certain type
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public ObjectId(Key key, Type t)
        {
            this.id = Key.NewKey(key, t);
        }

        /// <summary>
        /// Retrieve the key of the object id
        /// </summary>
        public Key Id
        {
            get { return id; }
        }

        /// <summary>
        /// Mark an object id as invalid
        /// </summary>
        internal void Invalidate()
        {
            id.Invalidate();
        }

        /// <summary>
        /// Test if an object id is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return id.Type != null;
        }

        /// <summary>
        /// Construct a clone of the given object id
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();  // handle class always provides shallow copy
        }

        /// <summary>
        /// Override of Equals
        /// </summary>
        /// <param name="o">Object to compare with</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object o)
        {
            ObjectId oi = o as ObjectId;
            return (object)oi != null && id.Equals(oi.id);
        }

        /// <summary>
        /// Overloaded Operator ==
        /// </summary>
        /// <param name="o1">Value to compare with</param>
        /// <param name="o2">Value to compare with</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(ObjectId o1, object o2)
        {
            if (((object)o1) == null && ((object)o2) == null)
                return true;
            if (((object)o1) == null)
                return false;
            return o1.Equals(o2);
        }

        /// <summary>
        /// Overloaded Operator !=, to make sure, that Equals is called.
        /// </summary>
        /// <param name="o1">Value to compare with</param>
        /// <param name="o2">Value to compare with</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator !=(ObjectId o1, object o2)
        {
            return !(o1 == o2);
        }

        /// <summary>
        /// Override of GetHashCode
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        /// <summary>
        /// String representation of the id
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToShortId();
        }

        /// <summary>
        /// String representation for debugging purposes
        /// </summary>
        /// <returns></returns>
        public string Dump()
        {
            return id.ToString();
        }

		/// <summary>
		/// Serializes the ObjectId to a ShortId, which can be used to identify an object of any type.
		/// </summary>
		/// <returns>A string consisting of the type information and the oid of the object.</returns>
		internal string ToShortId()
		{
			if (!IsValid())
				throw new NDOException(86, "ObjectId is not valid.");
			return id.ToShortId();
		}

		/// <summary>
		/// Gets the oid data for serialization purposes.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.id.Serialize(info, context);
        }

		/// <summary>
		/// Gets the key value of an oid.
		/// </summary>
		/// <remarks>
		/// You can get the id value of an oid like that:
		/// <code>
		/// var key = pc.NDOObjectId[0];
		/// </code>
		/// Or use it in a Linq query:
		/// <code>
		/// var vt = pm.Objects&lt;YourType&gt;.Where(o=>o.NDOObjectId[0] == yourValue);
		/// var yourObject = vt.ExecuteSingle();
		/// </code>
		/// </remarks>
		/// <param name="i"></param>
		/// <returns></returns>
		public object this[int i]
		{
			get => Id.Values[i];
		}

        /// <summary>
        /// Constructs an ObjectId from a deserialization stream
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ObjectId(SerializationInfo info, StreamingContext context)
        {
            Type keyType = (Type)info.GetValue("KeyType", typeof(Type));
            this.id = Key.NewKey(info, context);
        }
    }
}
