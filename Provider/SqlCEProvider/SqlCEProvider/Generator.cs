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
