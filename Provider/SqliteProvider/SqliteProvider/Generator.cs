//
// Copyright (c) 2002-2019 Mirko Matytschak 
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

namespace NDO.SqliteProvider
{
	public class SqliteGenerator : AbstractSQLGenerator
	{

		public SqliteGenerator()
		{			
		}

		public override string ProviderName
		{
			get { return "Sqlite"; }
		}

		public override string DropTable(string tableName)
		{
			return "DROP TABLE " + tableName + ";";
		}


		public override bool LengthAllowed(Type t)
		{
			return true;
		}

		public override bool LengthAllowed(string dbType)
		{
			return true;
		}

		public override string DbTypeFromType(Type t, int size)
		{
			// CLOBs: Strings don't have a limit in Sqlite. So there is nothing to do.
			return NDO.SqliteProvider.Provider.GetInternalDbType( t ).ToString();
		}

		public override string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + " Integer PRIMARY KEY AUTOINCREMENT";
		}

		public override bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
		}

        public override PrimaryConstraintPlacement PrimaryConstraintPlacement
        {
            get { return PrimaryConstraintPlacement.InColumn; }
        }

        public override string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
        {
			string coldef = columnName + " " + columnType;
            if (dataType == typeof(string))
                coldef += '(' + width + ')'; 
            return coldef + " PRIMARY KEY";
        }

	}

}
