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
using System.Text;
using System.Collections;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;


namespace OracleProvider
{
    public class OracleGenerator : AbstractSQLGenerator
    {

        public OracleGenerator()
        {
        }

        public override string CreateIndex(string indexName, string tableName, DataColumn[] indexColums)
        {
            string indexName2 = indexName.Substring(1, indexName.Length - 2);
            uint hash = (uint)indexName2.GetHashCode();
            if (indexName2.Length > 30)
                indexName2 = indexName2.Substring(0, 22) + hash.ToString("X");

            return base.CreateIndex(Provider.GetQuotedName(indexName2), tableName, indexColums);
        }


        public override string ProviderName
        {
            get { return "Oracle"; }
        }

        public override string DropTable(string tableName)
        {
            if (tableName.Length > 35)  // 30 + 4 * '"' + '.'
            {
                string s = "Warning: TableName " + tableName + " is too long.";
                if(messages != null)
                    messages.WriteLine(s);
                else
                    Console.WriteLine(s);
            }
            return "DROP TABLE " + tableName + ";";
        }


        public override bool LengthAllowed(Type t)
        {
            return t != typeof(DateTime);
        }

        public override bool LengthAllowed(string dbType)
        {
            return dbType != "Date";
        }

        public override string DbTypeFromType(Type t)
        {
            // The Oracle ADO.NET provider gives us an OracleType.DateTime instead
            // of an OracleType.Date
			t = ((NDOAbstractProvider)this.Provider).ConvertNullableType(t);
			
            if (t == typeof(DateTime))
                return "Date";
			OracleDbType dbType = (OracleDbType)Provider.GetDbType(t);

			if ( t == typeof(bool) || t == typeof(byte) || t == typeof(sbyte) || t == typeof(ushort) || t == typeof(int) || t == typeof(short) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong) || t == typeof(float) || t == typeof(double) || t == typeof(decimal) )
				return "Number";

            return ((OracleDbType)Provider.GetDbType(t)).ToString();
        }


        private NDO.Mapping.Class FindClass(string tableName, NDOMapping mappings)
        {
            Class result = null;
            foreach (Class cl in mappings.Classes)
            {
                if (cl.TableName == tableName)
                {
                    result = cl;
                    break;
                }
            }
            return result;
        }


        public override string AlterColumnType()
        {
            return ("MODIFY");
        }
    }
}
