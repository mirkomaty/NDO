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
using System.Text;
using System.Collections;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;


namespace NDO
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
            if (t == typeof(DateTime))
                return "Date";
            return ((OracleType)Provider.GetDbType(t)).ToString();
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
