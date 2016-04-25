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


#if nix // moved to NDO dll
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using System.IO;
using NDO.Mapping;
using NDO;
using NDOInterfaces;

namespace NDOEnhancer
{
	internal class AccessGenerator : AbstractSQLGenerator
	{

		public AccessGenerator()
		{
		}

		public override string ProviderName
		{
			get { return "Access"; }
		}


		public override PrimaryConstraintPlacement PrimaryConstraintPlacement
		{
			get { return PrimaryConstraintPlacement.InColumn; }
		}

		public override bool LengthAllowed(Type t)
		{
			return t == typeof(string);
		}

		public override bool LengthAllowed(string dbType)
		{
			return string.Compare(dbType, "text", true) == 0
				|| string.Compare(dbType, "decimal", true) == 0;
		}

		public override string AutoIncrementColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + " COUNTER CONSTRAINT PrimaryKey PRIMARY KEY";
		}

		public override string PrimaryKeyColumn(string columnName, Type dataType, string columnType, string width)
		{
			return columnName + ' ' + columnType + " CONSTRAINT PrimaryKey PRIMARY KEY";
		}

		public override bool HasSpecialAutoIncrementColumnFormat
		{
			get { return true; }
		}


		public override string DbTypeFromType(Type t)
		{
			return GetDbTypeFromCsType(t);
		}


		public string CreateConstraint(DataColumn[] primaryKeyColumns, string constraintName, string tableName)
		{
			return string.Empty;
		}

		public string GetDbTypeFromCsType(System.Type t) 
		{
			if ( t == typeof(bool) )
				return "Yesno";
			else if ( t == typeof(byte) )
				return "Byte";
			else if ( t == typeof(sbyte) )
				return "Byte";
			else if ( t == typeof(char) )
				return "Integer";
			else if ( t == typeof(short))
				return "Short";
			else if ( t == typeof(ushort))
				return "Short";
			else if ( t == typeof(int))
				return "Long";
			else if ( t == typeof(uint))
				return "Long";
			else if ( t == typeof(long))
				return "Decimal";
			else if ( t == typeof(ulong))
				return "Decimal";
			else if ( t == typeof(float))
				return "Single";
			else if ( t == typeof(double))
				return "Double";
			else if ( t == typeof(string))
				return "Text";
			else if ( t == typeof(byte[]))
				return "Oleobject";
			else if ( t == typeof(decimal))
				return "Currency";
			else if ( t == typeof(DateTime))
				return "datetime";
			else if ( t == typeof(Guid))
				return "Guid";
			else
				throw new Exception("NDOOleDbProvider.GetDbType: Typ " + t.Name + " kann nicht in OleDbType konvertiert werden");
		}


		public override string RemoveColumn(string colName)
		{
			return "DROP " + colName;
		}


	}
}
#endif