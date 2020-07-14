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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDO.Mapping;

namespace NDO
{
    /// <summary>
    /// A key class that implements ObjectIds as foreign keys to other classes.
    /// </summary>
    public class DependentKey : MultiKey
    {
        /// <summary>
        /// Creates a DependentKey object.
        /// </summary>
        /// <param name="t">The type of the object.</param>
        /// <param name="cl">The class mapping of the type.</param>
        /// <param name="tm">The type manager.</param>
        /// <remarks>
        /// Note: In case of generic types t is not equal to cl.SystemType. That's why
        /// we need the extra parameter t.
        /// </remarks>
        internal DependentKey(Type t, Class cl, TypeManager tm) : base (t, tm)
        {
            // Dependent keys have some additional values in it's keyvalues array.
            // At the beginning of the array there are the values as defined by
            // the OidColumns. After the oid values there are type column values
            // for each relation, having a ForeignKeyTypeColumnName.
            string relationName = string.Empty;
            int relCount = 0;
            foreach (OidColumn oidc in cl.Oid.OidColumns)
            {
                if (oidc.RelationName != relationName)
                {
                    relationName = oidc.RelationName;
                    Relation rel = cl.FindRelation(relationName);
                    // InitFields checks, if all relations exist
                    System.Diagnostics.Debug.Assert(rel != null);
                    if (rel.ForeignKeyTypeColumnName != null)
                        relCount++;
                }
            }
            base.pm_keydata = new object[cl.Oid.OidColumns.Count + relCount];
        }


        /// <summary>
        /// This constructor is used for deserialisation.
        /// </summary>
        public DependentKey()
        {
        }

        /// <summary>
        /// Creates a DependentKey object.
        /// </summary>
        /// <param name="t">The type of the object.</param>
        /// <param name="cl">The class mapping of the type.</param>
        /// <param name="row">A DataRow, containing the object data.</param>
        /// <param name="tm">The type manager.</param>
        /// <remarks>
        /// Note: In case of generic types t is not equal to cl.SystemType. That's why
        /// we need the extra parameter t.
        /// </remarks>
        internal DependentKey(Type t, Class cl, DataRow row, TypeManager tm) : base (t, tm)
        {
            // Dependent keys have some additional values in it's keyvalues array.
            // At the beginning of the array there are the values as defined by
            // the OidColumns. After the oid values there are type column values
            // for each relation, having a ForeignKeyTypeColumnName.
            string relationName = string.Empty;
            int relCount = 0;
            List<Relation> relations = new List<Relation>();
            foreach (OidColumn oidc in cl.Oid.OidColumns)
            {
                if (oidc.RelationName != relationName)
                {
                    relationName = oidc.RelationName;
                    Relation rel = cl.FindRelation(relationName);
                    // InitFields checks, if all relations exist
                    System.Diagnostics.Debug.Assert(rel != null);
                    if (rel.ForeignKeyTypeColumnName != null)
                    {
                        relations.Add(rel);
                        relCount++;
                    }
                }
            }
            base.pm_keydata = new object[cl.Oid.OidColumns.Count + relCount];
            base.FromRow(cl, row);  // the oidColumns have the same column names as the relation columns
            int i = cl.Oid.OidColumns.Count;
            foreach (Relation rel in relations)
            {
                base.pm_keydata[i++] = row[rel.ForeignKeyTypeColumnName];
            }
        }

        /// <summary>
        /// Calls to this methods are ignored.
        /// </summary>
        /// <param name="cl"></param>
        /// <param name="row"></param>
        public override void FromRow(NDO.Mapping.Class cl, DataRow row)
        {
            // Nothing to do
        }

        /// <summary>
        /// Calls to this methods are ignored.
        /// </summary>
        /// <param name="cl"></param>
        /// <param name="row"></param>
        public override void ToRow(NDO.Mapping.Class cl, DataRow row)
        {
            // Nothing to do
        }

        /// <summary>
        /// Calls to this method will cause an exception. NDO can't build a foreign key from a dependent oid.
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="row"></param>
        public override void ToForeignKey(Relation relation, DataRow row)
        {
            throw new InternalException(52, "DependentKey: Can't build a foreign key from a dependent oid. Type: " + base.Type.FullName);
        }


        /// <summary>
        /// Overridden Operator ==, to make sure, that Equals is called. 
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static bool operator ==(DependentKey o1, DependentKey o2)
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
        public static bool operator !=(DependentKey o1, DependentKey o2)
        {
            return !(o1 == o2);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
