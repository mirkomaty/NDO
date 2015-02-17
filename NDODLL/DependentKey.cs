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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDO.Mapping;

namespace NDO
{
    public class DependentKey : MultiKey
    {
        /// <summary>
        /// Creates a DependentKey object.
        /// </summary>
        /// <param name="t">The type of the object.</param>
        /// <param name="cl">The class mapping of the type.</param>
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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
