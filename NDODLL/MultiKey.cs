﻿//
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
using System.Linq;
using System.Runtime.Serialization;
using System.Data;
using NDO.Mapping;

namespace NDO
{
    /// <summary>
    /// The MultiKey class manages situations where the primary key of a data row 
    /// consists of two foreign key values.
    /// </summary>
    public class MultiKey : Key
    {
		/// <summary>
		/// The internal data of the key
		/// </summary>
        protected object[] pm_keydata;

		/// <inheritdoc />
		public object[] Keydata
        {
            get { return pm_keydata; }
        }


        /// <summary>
        /// Constructs a new MultiKey object. The initial data are read from the data row.
        /// </summary>
        /// <param name="t">The type of the object.</param>
        /// <param name="cl">The class mapping of the type.</param>
        /// <param name="row">The DatasRow containing the object data.</param>
		/// <param name="tm">The type manager</param>
        /// <remarks>
        /// We have an extra parameter t, because in case of generic types
        /// cl.SystemType is the GenericTypeDefinition.
        /// </remarks>
        internal MultiKey(Type t, Class cl, DataRow row, TypeManager tm) : base(t, tm)
        {
            pm_keydata = new object[cl.Oid.OidColumns.Count()];
            FromRow(cl, row);
        }

        /// <summary>
        /// This is used for the constructors of classes, which are derived from MultiKey
        /// </summary>
        /// <param name="t"></param>
		/// <param name="tm"></param>
        internal MultiKey(Type t, TypeManager tm)
            : base(t, tm)
        {
        }

        internal MultiKey(Type t, Class cl, TypeManager tm) : base(t, tm)
        {
            pm_keydata = new object[cl.Oid.OidColumns.Count()];
        }

        /// <summary>
        /// This constructor is used for deserialisation.
        /// </summary>
        public MultiKey() : base(null, null)
        {
        }

		/// <inheritdoc />
        public override void FromRow(Class cl, DataRow row)
        {
            int i = 0;
            new OidColumnIterator(cl).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
            {
				// If we have a db without native Guid type, the oidColumn system type is Guid and the 
				// column type is String. We definitely want to have a Guid in the Oid, so we have 
				// to convert it.
				object o = row[oidColumn.Name];
				if ( oidColumn.SystemType == typeof( Guid ) && o is string )
					o = new Guid( (string) o );
                pm_keydata[i++] = o;
            });
        }

		/// <inheritdoc />
		public override void ToRow(Class cl, DataRow row)
        {
            int i = 0;
            new OidColumnIterator(cl).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
            {
                row[oidColumn.Name] = pm_keydata[i++];
            });
        }


		/// <inheritdoc />
		public override bool Equals(object obj)
        {
            if (base.Equals(obj))  // Key compares the types and checks for null
            {
                MultiKey k = obj as MultiKey;
                if ((object)k == null)
                    return false;
                bool result = true;
                for (int i = 0; i < pm_keydata.Length; i++)
                    result = result && pm_keydata[i].Equals(k.pm_keydata[i]);
                return result;
            }
            return false;
        }


        /// <summary>
        /// Overridden Operator ==, to make sure, that Equals is called. 
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator ==(MultiKey o1, MultiKey o2)
        {
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
        public static bool operator !=(MultiKey o1, MultiKey o2)
        {
            return !(o1 == o2);
        }

		/// <summary>
		/// Determines, if the key is valid.
		/// </summary>
        public bool IsValid
        {
            get 
            {
                if (base.Type == null)
                    return false;
                for (int i = 0; i < pm_keydata.Length; i++)
                {
                    if (pm_keydata[i] == null)
                        return false;
                }
                return true;
            }
        }

		/// <inheritdoc />
		public override int GetHashCode()
        {
            int hashcode = base.GetHashCode(); // Code of the type t
            for (int i = 0; i < pm_keydata.Length; i++)
            {
                hashcode = hashcode ^ pm_keydata[i].GetHashCode();
            }
            return hashcode;
        }

        /// <summary>
        /// Returns a string representation of the key
        /// </summary>
        /// <returns>String representation of the key</returns>
        public override string ToString()
        {
            string s = "Type = " + this.Type.FullName + ", Id = ";
            for (int i = 0; i < pm_keydata.Length; i++)
            {
                s += pm_keydata[i].ToString();
                if (i < pm_keydata.Length - 1)
                    s += ',';
            }
            return s;
        }

		/// <inheritdoc />
		public override void Serialize(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("KeyData", this.pm_keydata);
            info.AddValue("KeyType", this.GetType());
            info.AddValue("ObjectType", this.Type);
        }

		/// <inheritdoc />
		public override void Deserialize(SerializationInfo info, StreamingContext context)
        {
            object[] keydata = (object[])info.GetValue("KeyData", typeof(object[]));
            this.pm_keydata = keydata;
        }

		/// <inheritdoc />
		public override object this[int index]
        {
            get { return pm_keydata[index]; }
            set { pm_keydata[index] = value; }            
        }

		/// <inheritdoc />
		public override void ToForeignKey(Relation relation, DataRow row)
        {
            // The order of the ForeignKeyColumns is identical to the order
            // of the OidColumns.
            if (pm_keydata.Length < relation.ForeignKeyColumns.Count())
                throw new InternalException(175, "MultiKey.ToForeignKey: keydata array too short");
            int i = 0;
            foreach (ForeignKeyColumn fkColumn in relation.ForeignKeyColumns)
            {
                row[fkColumn.Name] = pm_keydata[i++];
            }

            // This is not to be confused with the type id's stored in DependentKeys

            if (relation.ForeignKeyTypeColumnName != null)
            {
                row[relation.ForeignKeyTypeColumnName] = this.TypeId;
            }
        }

		/// <inheritdoc />
        public override object Value
        {
            get
            {
                return pm_keydata[0];
            }
        }

		/// <inheritdoc />
		public override object[] Values
        {
            get
            {
                return pm_keydata.ToArray();
            }
        }

		/// <summary>
		/// Gets a clone of the Key.
		/// </summary>
		/// <returns></returns>
        public override Key Clone()
        {
            MultiKey newKey = new MultiKey(this.t, this.TypeManager);
            newKey.pm_keydata = this.pm_keydata;
            return newKey;
        }
    }
}
