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


using System;
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
        protected object[] pm_keydata;

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
        /// <remarks>
        /// We have an extra parameter t, because in case of generic types
        /// cl.SystemType is the GenericTypeDefinition.
        /// </remarks>
        public MultiKey(Type t, Class cl, DataRow row) : base(t)
        {
            pm_keydata = new object[cl.Oid.OidColumns.Count];
            FromRow(cl, row);
        }

        /// <summary>
        /// This is used for the constructors of classes, which are derived from MultiKey
        /// </summary>
        /// <param name="t"></param>
        protected MultiKey(Type t)
            : base(t)
        {
        }

        public MultiKey(Type t, Class cl) : base(t)
        {
            pm_keydata = new object[cl.Oid.OidColumns.Count];
        }

        /// <summary>
        /// This constructor is used for deserialisation.
        /// </summary>
        public MultiKey() : base(null)
        {
        }

        /// <summary>
        /// Writes the keydata into the data row using the class mapping information.
        /// </summary>
        /// <param name="cl"></param>
        /// <param name="row"></param>
        public override void FromRow(Class cl, DataRow row)
        {
            int i = 0;
            new OidColumnIterator(cl).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
            {
                pm_keydata[i++] = row[oidColumn.Name];
            });
        }

        /// <summary>
        /// Initializes the keydata from the given datarow using the class mapping information.
        /// </summary>
        /// <param name="cl"></param>
        /// <param name="row"></param>
        public override void ToRow(Class cl, DataRow row)
        {
            int i = 0;
            new OidColumnIterator(cl).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
            {
                row[oidColumn.Name] = pm_keydata[i++];
            });
        }


        /// <summary>
        /// Checks two keys for equality
        /// </summary>
        /// <param name="obj">Key object to compare with</param>
        /// <returns>True if keys are equal</returns>
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

        /// <summary>
        /// Gets a hash code to be used in hash tables
        /// </summary>
        /// <returns>Hash code</returns>
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

        public override void Serialize(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("KeyData", this.pm_keydata);
            info.AddValue("KeyType", this.GetType());
            info.AddValue("ObjectType", this.Type);            
        }

        public override void Deserialize(SerializationInfo info, StreamingContext context)
        {
            object[] keydata = (object[])info.GetValue("KeyData", typeof(object[]));
            this.pm_keydata = keydata;
        }

        public override object this[int index]
        {
            get { return pm_keydata[index]; }
            set { pm_keydata[index] = value; }            
        }

        public override void ToForeignKey(Relation relation, DataRow row)
        {
            // The order of the ForeignKeyColumns is identical to the order
            // of the OidColumns.
            if (pm_keydata.Length < relation.ForeignKeyColumns.Count)
                throw new InternalException(175, "MultiKey.ToForeignKey: keydata array too short");
            for (int i = 0; i < relation.ForeignKeyColumns.Count; i++)
            {
                ForeignKeyColumn fkColumn = (ForeignKeyColumn)relation.ForeignKeyColumns[i];
                //relObjRow[fkColumn.Name] =
                row[fkColumn.Name] = pm_keydata[i];
            }

            // This is not to be confused with the type id's stored in DependentKeys
#if PRO
            if (relation.ForeignKeyTypeColumnName != null)
            {
                row[relation.ForeignKeyTypeColumnName] = this.TypeId;
            }
#endif
        }

        public static Key OldDeserialization(SerializationInfo info, StreamingContext context)
        {
            string idType = info.GetString("IdType");
            string key = (string)info.GetValue("Key", typeof(string));
            Type t = (Type)info.GetValue("Type", typeof(System.Type));
            MultiKey newKey = new MultiKey(t);
            object[] keydata = new object[1];
            switch (idType)
            {
                case ("String"):
                    keydata[0] = key;
                    break;
                case ("Int32"):
                    keydata[0] = int.Parse(key);
                    break;
                case ("Guid"):
                    keydata[0] = new Guid(key);
                    break;
            }
            newKey.pm_keydata = keydata;
            return newKey;
        }

        /// <summary>
        /// Gets or sets the Key value of the first oid column. This property is obsolete, since NDO supports multi-column primary keys and thus is
        /// not limited to one value per oid.
        /// </summary>
        public override object Value
        {
            get
            {
                return pm_keydata[0];
            }
            set
            {
                pm_keydata[0] = value;
            }
        }

        public override Key Clone()
        {
            MultiKey newKey = new MultiKey(this.t);
            newKey.pm_keydata = this.pm_keydata;
            return newKey;
        }

    }
}
