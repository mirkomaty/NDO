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
using System.Collections.Generic;
using System.Text;
using NDO.Mapping;

namespace NDO
{
    /// <summary>
    /// Used to create new ObjectIds
    /// </summary>
    public class ObjectIdFactory
    {
        public static ObjectId NewObjectId(Type t, Class cl, DataRow row)
        {
            if (cl.SystemType == null)
                throw new InternalException(16, "ObjectIdFactory.NewObjectId: cl.SystemType is null.");
            Key key;
            if (cl.Oid.IsDependent)
                key = new DependentKey(t, cl, row);
            else
                key = new MultiKey(t, cl, row);
            return new ObjectId(key);
        }

        public static ObjectId NewObjectId(Type t, Class cl, DataRow row, MappingTable mt)
        {
            if (cl.SystemType == null)
                throw new InternalException(16, "ObjectIdFactory.NewObjectId: cl.SystemType is null.");
            ObjectId result;
            if (cl.Oid.IsDependent)
                result = new ObjectId(new DependentKey(t, cl));
            else
                result = new ObjectId(new MultiKey(t, cl));
            Key key = result.Id;
            int i = 0;
            new ForeignKeyIterator(mt).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
            {
                object o = row[fkColumn.Name];
				AssignKeyValue( cl, key, i, o );
                i++;
            });

            return result;
        }

		private static void AssignKeyValue( Class cl, Key key, int i, object o )
		{
			if ( o is string && ((OidColumn) cl.Oid.OidColumns[i]).SystemType == typeof( Guid ) )
				key[i] = new Guid( (string) o );
			else
				key[i] = o;
		}

        public static ObjectId NewObjectId(Type t, Class cl, object keydata)
        {
            ObjectId result;
            if (cl.Oid.IsDependent)
                result = new ObjectId(new DependentKey(t, cl));
            else
                result = new ObjectId(new MultiKey(t, cl));
            Key key = result.Id;
            object[] arr = keydata as object[]; 
            if (arr != null)
            {
				for ( int i = 0; i < arr.Length; i++ )
				{
					object o = arr[i];
					AssignKeyValue( cl, key, i, o );
				}
            }
            else
            {
				AssignKeyValue( cl, key, 0, keydata );
            }
            return result;
        }
    }
}
