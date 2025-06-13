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

namespace NDO.PostGreProvider
{
	public class PostGreGenerator : AbstractSQLGenerator
	{

		public PostGreGenerator()
		{			
		}

		public override string ProviderName
		{
			get { return "Postgre"; }
		}

		public override string DropTable(string tableName)
		{
			return "DROP TABLE " + tableName + ";";
		}


		public override bool LengthAllowed(Type t)
		{
			return t == typeof(string) || t == typeof(decimal) || t == typeof(Guid);
		}

		public override bool LengthAllowed(string dbType)
		{
            return (string.Compare(dbType, "char", true) == 0 || string.Compare(dbType, "varchar", true) == 0 || string.Compare(dbType, "numeric", true) == 0);
		}

		public override string DbTypeFromType(Type t, int size)
		{
            if (t == typeof(bool))
                return("bool");
            else if (t == typeof(Byte))
				return "int2";
            else if (t == typeof(Byte[]))
                return "bytea";
            else if (t == typeof(DateTime))
				return "date";
			else if (t == typeof(decimal))
				return "numeric";
			else if (t == typeof(double))
				return "float8";
			else if (t.IsEnum)
				return "int4";
			else if (t == typeof(float))
				return "real";
			else if (t == typeof(Guid))
				return "varchar";
			else if (t == typeof(Int16) || t == typeof(UInt16))
				return "int2";
			else if (t == typeof(Int32) || t == typeof(UInt32))
				return "int4";
			else if (t == typeof(Int64) || t == typeof(UInt64))
				return "int8";
			else if (t == typeof( string ) && size == -1)
				return "text";
			else if (t == typeof(string))
				return "varchar";
			throw new Exception("Can't resolve type " + t.FullName + " as storable.");
		}

		public override string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width, bool isPrimary)
		{
			// This always results in a primary key column.
			return $"{columnName} {columnType} GENERATED ALWAYS AS IDENTITY";
		}

		public override bool HasSpecialAutoIncrementColumnFormat => true;

        public override PrimaryConstraintPlacement PrimaryConstraintPlacement
        {
            get { return PrimaryConstraintPlacement.InColumn; }
        }

        public override string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
        {
            string coldef = columnName + " " + columnType;
            if (dataType == typeof(string) || dataType == typeof(Guid))
                coldef += '(' + width + ')'; 
            return coldef + " PRIMARY KEY";
        }

	}

}
