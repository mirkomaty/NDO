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