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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.IO;
using NDO;
using NDOInterfaces;

namespace NDO.SqlCeProvider
{
	public class SqlCeGenerator : AbstractSQLGenerator
	{

		public SqlCeGenerator()
		{
		}

		public override string ProviderName
		{
			get { return "SqlCe"; }
		}

		public override string DropTable(string tableName)
		{
			return "DROP TABLE " + tableName + ";";
		}


        public override bool LengthAllowed(Type t)
        {
            return t == typeof(string) || t == typeof(decimal);
        }

        public override bool LengthAllowed(string dbType)
        {
            return string.Compare(dbType, "nvarchar", true) == 0 || string.Compare(dbType, "decimal", true) == 0;
        }

        public override string DbTypeFromType(Type t)
        {
            if (t == typeof(DateTime))
                return "DateTime";
            return ((SqlDbType)Provider.GetDbType(t)).ToString();
        }

        public override string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
        {
            return columnName + " " + columnType + " " + width + " IDENTITY (1, 1) ";
        }


        public override bool HasSpecialAutoIncrementColumnFormat
        {
            get { return true; }
        }


        public override string AlterColumnType()
        {
            return "ALTER COLUMN";
        }
	}
}
